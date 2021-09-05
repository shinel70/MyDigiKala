using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using DigiKala.Core.Interfaces;
using DigiKala.Core.Services;
using DigiKala.Core.ViewModels;
using DigiKala.Core.Classes;

using DigiKala.DataAccessLayer.Entities;

using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using DigiKala.DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore.Internal;

namespace DigiKala.Controllers
{
    public class AccountsController : Controller
    {
        private IAccount _account;
        private IViewRenderService _render;
        private DatabaseContext _context;
        MessageSender sender;

        public AccountsController(IAccount account, IViewRenderService render, DatabaseContext context,MessageSender messageSender)
        {
            _account = account;
            _render = render;
            _context = context;
            sender = messageSender;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserRegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Mobile == viewModel.Mobile);
                if (user != null)
                {
                    ModelState.AddModelError("username", "کابری با این شماره موبایل قبلا ثبت نام کرده است");
                    return View(viewModel);
                }
                else
                {
                    try
                    {
                        //var setting = await _unitOfWork.Setting.GetFirstOrDefaultAsync();
                        //TemporaryCode temporaryCode = new()
                        //{
                        //    ActiveCode = CodeGenerators.ActiveCode(),
                        //    ExpireDateTime = DateTime.UtcNow,
                        //    Identity = viewModel.Mobile
                        //};
                        //MessageSender sender = new MessageSender();
                        //viewModel.ActiveCode = CodeGenerators.ActiveCode();
                        //sender.SendSms(viewModel.Mobile, "به فروشگاه اینترنتی خوش آمدید" + Environment.NewLine + "کد فعالسازی : " + temporaryCode.ActiveCode,setting);
                        //await _unitOfWork.TemporaryCode.AddAsync(temporaryCode);
                        var tc = _context.TemporaryCodes.FirstOrDefault(tc => tc.Identity == viewModel.Mobile && tc.ActiveCode == viewModel.ActiveCode);
                        if ((DateTime.UtcNow - tc.ExpireDateTime).TotalHours > 1)
                        {
                            ModelState.AddModelError("ActiveCode", "کد منقضی شده است لطفا دوباره کد دریافت کنید");
                            return View(viewModel);
                        }
                        var CustomerRole = _context.Roles.FirstOrDefault(r => r.Name == StaticData.User);
                        user = new User()
                        {
                            Mobile = viewModel.Mobile,
                            Password = HashGenerators.MD5Encoding(viewModel.Password),
                            IsActive = true,
                            DateTime = DateTime.Now,
                            RoleId = CustomerRole.Id,
                        };
                        _context.Add(user);
                        _context.SaveChanges();
                        return RedirectToAction("Login","Account");
                    }
                    catch
                    {
                        return View(viewModel);
                    }
                }
            }
            return View(viewModel);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserLoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                User user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Mobile == viewModel.Mobile);
                if (user is null)
                {
                    ModelState.AddModelError("Password", "مشخصات کاربری اشتباه است");
                    return View(viewModel);
                }
                string hashPassword = HashGenerators.MD5Encoding(viewModel.Password);

                if (hashPassword != user.Password)
                {
                    ModelState.AddModelError("Password", "مشخصات کاربری اشتباه است");
                    return View(viewModel);
                }
                var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Name, user.Mobile),
                            new Claim(ClaimTypes.Role, user.Role.Name)
                        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                var properties = new AuthenticationProperties()
                {
                    IsPersistent = true
                };

                HttpContext.SignInAsync(principal, properties);

                if (user.Role.Name.IndexOf(StaticData.Admin) > -1)
                {
                    return RedirectToAction("Dashboard", "Panel");
                }
                else if (user.Role.Name.IndexOf(StaticData.Store) > -1)
                    return RedirectToAction("Dashboard", "Panel");
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                ModelState.AddModelError("Password", "مشخصات کاربری اشتباه است");
                return View(viewModel);
            }
        }

        public IActionResult Reset()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Reset(UserRegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Mobile == viewModel.Mobile);
                if (user is null)
                {
                    ModelState.AddModelError("Password", "مشخصات کاربری اشتباه است");
                    return View(viewModel);
                }
                if (!_context.TemporaryCodes.Any(tc => tc.Identity == viewModel.Mobile && tc.ActiveCode == viewModel.ActiveCode))
                {
                    ModelState.AddModelError("Password", "مشخصات کاربری اشتباه است");
                    return View(viewModel);
                }
                string hashPassword = HashGenerators.MD5Encoding(viewModel.Password);
                user.Password = hashPassword;
                _context.Users.Update(user);
                _context.SaveChanges();
                return Redirect("/Accounts/Login");
            }
            return View(viewModel);
        }

        public IActionResult Store()
        {
            ViewBag.MyMessage = false;

            return View();
        }

        [HttpPost]
        public IActionResult Store(StoreRegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Store store = _context.Stores.FirstOrDefault(u => u.Mail == viewModel.Mail);
                if (store != null)
                {
                    ViewBag.MyMessage = false;
                    ModelState.AddModelError("Mail", "نمی توانید از این ایمیل استفاده کنید");
                    return View(viewModel);
                }

                //if he\she is sign up before as User
                User user = _context.Users.FirstOrDefault(u => u.Mobile == viewModel.Mobile);
                bool IsEmailActivated = _context.TemporaryCodes.Any(tc => tc.Identity == viewModel.Mail && tc.ActiveCode == viewModel.ActiveEmailCode);
                bool IsPhoneActivated = _context.TemporaryCodes.Any(tc => tc.Identity == viewModel.Mobile && tc.ActiveCode == viewModel.ActiveMobileCode);
                if (user != null)
                {
                    if (!IsEmailActivated && !IsPhoneActivated)
                    {
                        ViewBag.MyMessage = false;
                        ModelState.AddModelError("ActiveEmailCode", "کد شما اشتباه است یا منقضی شده است");
                        ModelState.AddModelError("ActiveMobileCode", "کد شما اشتباه است یا منقضی شده است");
                        return View(viewModel);
                    }
                    store = new Store()
                    {
                        Tel = viewModel.Mobile,
                        Mail = viewModel.Mail,
                        UserId = user.Id
                    };
                    user.Role = _context.Roles.FirstOrDefault(r => r.Name == StaticData.Store);
                    user.Store = store;
                    _context.Stores.Add(store);
                    _context.Update(user);
                    _context.SaveChanges();
                    ViewBag.MyMessage = true;
                    return View(viewModel);
                }

                if (!IsEmailActivated && !IsPhoneActivated)
                {
                    ViewBag.MyMessage = false;
                    ModelState.AddModelError("ActiveEmailCode", "کد شما اشتباه است یا منقضی شده است");
                    ModelState.AddModelError("ActiveMobileCode", "کد شما اشتباه است یا منقضی شده است");
                    return View(viewModel);
                }
                Role storeRole = _context.Roles.FirstOrDefault(r => r.Name == StaticData.Store);
                user = new User()
                {
                    IsActive = true,
                    Mobile = viewModel.Mobile,
                    Password = HashGenerators.MD5Encoding(viewModel.Password),
                    DateTime = DateTime.UtcNow,
                    RoleId = storeRole.Id
                };
                store = new Store()
                {
                    Mail = viewModel.Mail,
                    Tel = viewModel.Mobile,
                    User = user
                };
                _context.Stores.AddAsync(store);
                _context.SaveChanges();
                ViewBag.MyMessage = true;
                return View(viewModel);
            }
            return View(viewModel);
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/");
        }


        #region Api
        [HttpPost]
        public IActionResult Activate(string mobile = "")
        {
            if (mobile.Length == 11)
            {
                try
                {
                    if (_context.Users.Any(u => u.Mobile == mobile))
                    {
                        return Json(new { success = false, data = "با این شماره نمیتوانید ثبت نام کنید اگر قبلا ثبت نام کرده روی فراموشی رمز عبور کلیک کنید" });
                    }
                    string activeCode = CodeGenerators.ActiveCode();
                    TemporaryCode temporaryCode = new TemporaryCode()
                    {
                        ActiveCode = activeCode,
                        ExpireDateTime = DateTime.Now.AddHours(1),
                        Identity = mobile
                    };
                    sender.SMS(mobile, "به فروشگاه اینترنتی خوش آمدید" + Environment.NewLine + "کد فعالسازی : " + temporaryCode.ActiveCode);
                    _context.TemporaryCodes.Add(temporaryCode);
                    _context.SaveChanges();
                    return Json(new { success = true, data = "کد با موفقیت ارسال شد" });
                }
                catch
                {
                    return Json(new { success = false, data = "ارسال کد با خطا مواجه شده لطفا کمی بعد مجددا تلاش نمایید" });
                }
            }
            else
            {
                return Json(new { success = false, data = "لطفا شماره تلفن صحیح وارد کنید" });
            }
        }
        [HttpPost]
        public IActionResult ActivateStore(string mobile = "", string mail = "")
        {
            if (mobile.Length == 11 && !String.IsNullOrEmpty(mail))
            {
                Store store = _context.Stores.FirstOrDefault(u => u.Mail == mail);
                if (store != null)
                {
                    return Json(new { success = false, data = "با این ایمیل نمی توانید فروشگاه بسازید" });
                }
                try
                {
                    string mobileCode = CodeGenerators.ActiveCode();
                    string mailCode = CodeGenerators.ActiveCode();
                    List<TemporaryCode> temporaryCode = new List<TemporaryCode>()
                    {
                        new TemporaryCode()
                        {
                            ActiveCode = mobileCode,
                            ExpireDateTime = DateTime.Now.AddHours(1),
                            Identity = mobile
                        },
                        new TemporaryCode()
                        {
                            ActiveCode = mailCode,
                            ExpireDateTime = DateTime.Now.AddHours(1),
                            Identity = mail
                        }
                    };
                    sender.SMS(mobile, "به فروشگاه اینترنتی خوش آمدید" + Environment.NewLine + "کد فعالسازی : " + mobileCode);
                    string messageBody = _render.RenderToStringAsync("_ActivateMail", temporaryCode[1]);
                    sender.Email(mail, "فعالسازی فروشگاه", messageBody);
                    _context.AddRange(temporaryCode);
                    _context.SaveChanges();
                    return Json(new { success = true, data = "کد با موفقیت ارسال شد" });
                }
                catch
                {
                    return Json(new { success = false, data = "ارسال کد با خطا مواجه شده لطفا کمی بعد مجددا تلاش نمایید" });
                }

            }
            else
            {
                return Json(new { success = false, data = "لطفا شماره تلفن صحیح وارد کنید" });
            }
        }
        #endregion

        /*public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (_account.ExistsMobileNumber(viewModel.Mobile))
                {
                    // Go To Login
                }
                else
                {
                    User user = new User()
                    {
                        Mobile = viewModel.Mobile,
                        ActiveCode = CodeGenerators.ActiveCode(),
                        Code = null,
                        Date = pc.GetYear(DateTime.Now).ToString("0000") + "/" + pc.GetMonth(DateTime.Now).ToString("00") +
                             "/" + pc.GetDayOfMonth(DateTime.Now).ToString("00"),
                        FullName = null,
                        IsActive = false,
                        Password = HashGenerators.MD5Encoding(viewModel.Password),
                        RoleId = _account.GetMaxRole()
                    };

                    _account.AddUser(user);

                    try
                    {
                        MessageSender sender = new MessageSender();

                        sender.SMS(viewModel.Mobile, "به فروشگاه اینترنتی خوش آمدید" + Environment.NewLine + "کد فعالسازی : " + user.ActiveCode);
                    }
                    catch
                    {

                    }

                    return RedirectToAction(nameof(Activate));
                }
            }

            return View(viewModel);
        }

        public IActionResult Activate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Activate(ActivateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (_account.ActivateUser(viewModel.ActiveCode))
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ModelState.AddModelError("ActiveCode", "کد فعالسازی شما معتبر نیست");
                }
            }

            return View(viewModel);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string hashPassword = HashGenerators.MD5Encoding(viewModel.Password);

                User user = _account.LoginUser(viewModel.Mobile, hashPassword);

                if (user != null)
                {
                    if (user.Role.Name == "فروشگاه")
                    {
                        var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Name, user.Mobile)
                        };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        var properties = new AuthenticationProperties()
                        {
                            IsPersistent = true
                        };

                        HttpContext.SignInAsync(principal, properties);

                        return RedirectToAction("Dashboard", "Panel");
                    }
                    else
                    {
                        if (user.IsActive)
                        {
                            var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Name, user.Mobile)
                        };

                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identity);

                            var properties = new AuthenticationProperties()
                            {
                                IsPersistent = true
                            };

                            HttpContext.SignInAsync(principal, properties);

                            if (user.Role.Name == "کاربر")
                            {
                                return RedirectToAction("Dashboard", "Home");
                            }
                            else
                            {
                                return RedirectToAction("Dashboard", "Panel");
                            }
                        }
                        else
                        {
                            return RedirectToAction(nameof(Activate));
                        }
                    }


                }
                else
                {
                    ModelState.AddModelError("Password", "مشخصات کاربری اشتباه است");
                }
            }

            return View(viewModel);
        }

        public IActionResult Forget()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Forget(ForgetViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (_account.ExistsMobileNumber(viewModel.Mobile))
                {
                    try
                    {
                        MessageSender sender = new MessageSender();

                        sender.SMS(viewModel.Mobile, "امکان تغییر کلمه عبور با کد تایید " + _account.GetUserActiveCode(viewModel.Mobile));
                    }
                    catch
                    {

                    }

                    return RedirectToAction(nameof(Reset));
                }
                else
                {
                    ModelState.AddModelError("Mobile", "کاربری با این شماره پیدا نشد");
                }
            }

            return View(viewModel);
        }

        public IActionResult Reset()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Reset(ResetViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (_account.ResetPassword(viewModel.ActiveCode, viewModel.Password))
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ModelState.AddModelError("ActiveCode", "کد تایید شما اشتباه است");
                }
            }

            return View(viewModel);
        }

        public IActionResult Store()
        {
            ViewBag.MyMessage = false;

            return View();
        }

        [HttpPost]
        public IActionResult Store(StoreRegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (_account.ExistsMailAddress(viewModel.Mail))
                {
                    ViewBag.MyMessage = false;
                    ModelState.AddModelError("Mail", "نمی توانید از این ایمیل استفاده کنید");
                }
                else
                {
                    int userID = 0;
                    string mobileCode = "";

                    if (_account.ExistsMobileNumber(viewModel.Mobile))
                    {
                        _account.UpdateUserRole(viewModel.Mobile);

                        userID = _account.GetUserId(viewModel.Mobile);

                        mobileCode = _account.GetUserActiveCode(viewModel.Mobile);
                    }
                    else
                    {
                        mobileCode = CodeGenerators.ActiveCode();

                        User user = new User()
                        {
                            ActiveCode = mobileCode,
                            Code = null,
                            FullName = null,
                            IsActive = false,
                            Mobile = viewModel.Mobile,
                            Password = HashGenerators.MD5Encoding(viewModel.Password),
                            Date = pc.GetYear(DateTime.Now).ToString("0000") + "/" + pc.GetMonth(DateTime.Now).ToString("00") +
                                 "/" + pc.GetDayOfMonth(DateTime.Now).ToString("00"),
                            RoleId = _account.GetStoreRole()
                        };

                        _account.AddUser(user);

                        userID = user.Id;
                    }

                    Store store = new Store()
                    {
                        Address = null,
                        Desc = null,
                        Logo = null,
                        Mail = viewModel.Mail,
                        MailActivate = false,
                        MobileActivate = false,
                        Tel = null,
                        UserId = userID,
                        Name = null,
                        MailActivateCode = CodeGenerators.ActiveCode()
                    };

                    _account.AddStore(store);

                    ViewBag.MyMessage = true;

                    MessageSender sender = new MessageSender();

                    string messageBody = _render.RenderToStringAsync("_ActivateMail", store);

                    try
                    {
                        sender.Email(store.Mail, "فعالسازی فروشگاه", messageBody);
                        //sender.SMS(viewModel.Mobile, "درخواست ثبت فروشگاه انجام شد" + Environment.NewLine + "کد فعالسازی : " + mobileCode);
                    }
                    catch
                    {

                    }
                }
            }

            return View(viewModel);
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/");
        }*/
    }
}