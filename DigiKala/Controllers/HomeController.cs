using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using System.Globalization;

using DigiKala.Core.ViewModels;
using DigiKala.Core.Interfaces;
using DigiKala.Core.Services;

using DigiKala.DataAccessLayer.Entities;

using DigiKala.Core.Classes;
using DigiKala.DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using DigiKala.Core.Dtoes;
using Z.EntityFramework.Plus;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using DigiKala.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Parbad;
using Parbad.AspNetCore;
using Parbad.Internal;
using X.PagedList;

namespace DigiKala.Controllers
{
	public class HomeController : Controller
	{
		private readonly ITemp _temp;
		private readonly DatabaseContext _context;
		private readonly IMapper _mapper;
		private readonly IMemoryCache _cache;
		private readonly IOnlinePayment _onlinePayment;

		public HomeController(ITemp temp, DatabaseContext context, IMapper mapper, IMemoryCache cache, IOnlinePayment onlinePayment)
		{
			_temp = temp;
			_context = context;
			_mapper = mapper;
			_cache = cache;
			_onlinePayment = onlinePayment;
		}

		[Route("Home/SearchByCategory/{CategoryName}")]
		public IActionResult SearchByCategory(string categoryName)
		{
			Category selectCategory = _context.Categories.Include(c => c.Parent).ThenInclude(parent => parent.SubCategories).FirstOrDefault(c => c.Name == categoryName);
			List<Category> categories = _context.Categories.ToList();
			List<int> knownCategoryIds = new List<int>() { selectCategory.Id };
			List<Category> knownCategories = new List<Category>();
			if (selectCategory.Parent != null)
				knownCategories.AddRange(selectCategory.Parent.SubCategories);
			else
				knownCategories.AddRange(categories.Where(c => c.ParentId == null).ToList());
			foreach (var category in categories)
			{
				if (knownCategoryIds.Contains(category.ParentId ?? 0))
				{
					knownCategoryIds.Add(category.Id);
					knownCategories.Add(category);
				}
			}

			List<Product> products = _context.Products.Include(p => p.Brand).Include(p => p.Category).Include(p => p.Store)
				.Where(p => knownCategoryIds.Contains(p.CategoryId)).ToList();


			List<Brand> brands = products.Select(p => p.Brand).Distinct().ToList();
			List<Store> stores = products.Select(p => p.Store).Distinct().ToList();

			var viewmodel = new SearchCategoryViewModel();

			viewmodel.FillBrands = brands;
			viewmodel.FillCategories = knownCategories;
			viewmodel.FillProducts = products;
			viewmodel.FillParentCategory = selectCategory.Parent;
			viewmodel.FillSelectCategory = selectCategory;
			viewmodel.FillStores = stores;

			return View(viewmodel);
		}
		public IActionResult Index(int? page)
		{
			List<BannerDetails> bannerDetails = _temp.GetBannerDetailsNoExpire();

			if (bannerDetails.Count() > 0)
			{
				foreach (var item in bannerDetails)
				{
					if (item.ExpireDateTime.CompareTo(DateTime.Now) < 0)
					{
						_temp.UpdateBannerExpire(item.Id);
					}
				}
			}
			var products = _context.Products.Include(p => p.Store).ToPagedList(page ?? 1, 18);
			ViewBag.products = products;
			return View();
		}

		public IActionResult Banners()
		{
			List<Banner> banners = _temp.GetBanners();

			return View(banners);
		}

		[Route("/Product/{id}/{title}")]
		public IActionResult Product(int id, string title)
		{
			var viewModel = new ProductDetailsViewModel();
			Product product = _temp.GetProductDetail(id);

			ProductSeen productSeen = new ProductSeen()
			{
				DateTime = DateTime.Now,
				IP = HttpContext.Connection.RemoteIpAddress.ToString(),
				ProductId = product.Id
			};
			if (!_cache.TryGetValue($"p{HttpContext.Connection.RemoteIpAddress.ToString()}", out var data))
			{
				_cache.Set($"p{HttpContext.Connection.RemoteIpAddress.ToString()}", "", new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
				_context.ProductSeens.Add(productSeen);
				_context.SaveChanges();
			}

			var sessionProductsSeen = new List<int>();
			if (string.IsNullOrEmpty(HttpContext.Session.GetString(StaticData.productSeen)))
			{
				sessionProductsSeen.Add(product.Id);
				HttpContext.Session.SetObject(StaticData.productSeen, sessionProductsSeen);
			}
			else
			{
				sessionProductsSeen = HttpContext.Session.GetObject<List<int>>(StaticData.productSeen);
				if (!sessionProductsSeen.Contains(product.Id))
				{
					sessionProductsSeen.Add(product.Id);
					HttpContext.Session.SetObject(StaticData.productSeen, sessionProductsSeen);
				}
			}

			viewModel.FillProduct = product;
			return View(viewModel);
		}
		[Authorize, HttpGet]
		public IActionResult AddOrder([FromQuery(Name = "productIdQties")] string productIdQties)
		{
			var requestedProducts = JsonConvert.DeserializeObject<List<RequestedProductIds>>(productIdQties);
			Order order = new Order()
			{
				User = _context.Users.Include(u => u.Addresses).FirstOrDefault(u => u.Mobile == User.Identity.Name),
				StartDateTime = DateTime.Now,
				OrderProducts = new List<OrderProduct>()
			};
			List<Product> products = _context.Products.ToList().Where(p => requestedProducts.Any(vm => vm.Id == p.Id)).ToList();
			foreach (var product in products)
			{
				var foo = requestedProducts.Find(vm => vm.Id == product.Id);
				order.OrderProducts.Add(new OrderProduct() { Product = product, ProductId = product.Id, Qty = (foo.Qty < product.Exist && foo.Qty > 0 /*&& foo.Qty<20*/) ? foo.Qty : 0 });
			}
			return View(order);
		}
		[Authorize, HttpPost]
		public IActionResult AddOrder(string productIdQties, [FromForm(Name = "UserAddress")] int userAddressId, string address, bool isAddressBecomeSave)
		{
			var requestedProducts = JsonConvert.DeserializeObject<List<RequestedProductIds>>(productIdQties);
			Order order = new Order()
			{
				User = _context.Users.Include(u => u.Addresses).FirstOrDefault(u => u.Mobile == User.Identity.Name),
				StartDateTime = DateTime.Now,
				OrderProducts = new List<OrderProduct>()
			};
			List<Product> products = _context.Products.ToList().Where(p => requestedProducts.Any(vm => vm.Id == p.Id)).ToList();
			foreach (var product in products)
			{
				var foo = requestedProducts.Find(vm => vm.Id == product.Id);
				order.OrderProducts.Add(new OrderProduct()
				{
					Product = product,
					ProductId = product.Id,
					Qty = (foo.Qty < product.Exist && foo.Qty > 0 /*&& foo.Qty<20*/) ? foo.Qty : 0,
					Price = product.Price,
					DeletePrice = product.DeletePrice,
				});
			}
			if (address == null)
			{
				if (userAddressId == 0)
				{
					ModelState.AddModelError("address", "لطفا آدرسی انتخاب کنید");
					return View(order);
				}
				var findAddress = order.User.Addresses.FirstOrDefault(a => a.Id == userAddressId);
				if (findAddress == null)
				{
					ModelState.AddModelError("address", "لطفا آدرس صحیح انتخاب کنید");
					return View(order);
				}
				order.Address = findAddress.Address;
			}
			else
			{
				if (isAddressBecomeSave)
				{
					UserAddress userAddress = new UserAddress() { UserId = order.User.Id, Address = order.Address };
					_context.UserAddresses.Add(userAddress);
					_context.SaveChanges();
				}
				order.Address = address;
			}
			string callBackUrl = Url.Action("ConfirmOrder", "Home", new { query = JsonConvert.SerializeObject(requestedProducts) }, Request.Scheme);
			var result = _onlinePayment.Request(invoice =>
			{
				invoice.SetAmount(order.OrderProducts.Aggregate(Convert.ToUInt64(0), (acc, op) => acc + (op.Price * Convert.ToUInt64(op.Qty))))
				.UseAutoIncrementTrackingNumber()
				.SetCallbackUrl(callBackUrl)
				.SetGateway("ParbadVirtual");
			});
			if (result.IsSucceed)
			{
				order.TrackingNumber = result.TrackingNumber;
				_context.Orders.Add(order);
				_context.SaveChanges();
				return result.GatewayTransporter.TransportToGateway();
			}
			ModelState.AddModelError("address", "در ارتباط به درگاه پرداخت مشکلی پیش آمده لطفا مجددا تلاش کنید");
			return View(order);
		}
		[Authorize, HttpGet, HttpPost]
		public IActionResult ConfirmOrder(string query)
		{
			IPaymentFetchResult invoice = _onlinePayment.Fetch();
			if (invoice.Status == PaymentFetchResultStatus.AlreadyProcessed)
			{
				return Content("این پرداخت قبلا انجام شده است");
			}
			var requestedProducts = JsonConvert.DeserializeObject<List<RequestedProductIds>>(query);
			List<Product> products = _context.Products.Include(p => p.Store).ToList().Where(p => requestedProducts.Any(vm => vm.Id == p.Id)).ToList();

			if (!products.All(p => requestedProducts.First(rp => rp.Id == p.Id).Qty < p.Exist))
			{
				var cancelResult = _onlinePayment.Cancel(invoice, cancellationReason: "متاسفانه برخی از محصولات موجودی شان تمام شده لطفا روند خرید را از ابتدا انجام بدهید");
				return View("CancelResult", cancelResult);
			}
			products.ForEach(p =>
			{
				var reqProduct = requestedProducts.First(rp => rp.Id == p.Id);
				p.Exist -= reqProduct.Qty;
				p.Store.Wallet += Convert.ToUInt64(reqProduct.Qty) * p.Price;
			});
			_context.Products.UpdateRange(products);
			_context.SaveChanges();
			var verifyResult = _onlinePayment.Verify(invoice);
			return View(verifyResult);
		}
		[Authorize]
		public IActionResult ShowOrders()
		{
			int userId = _context.Users.FirstOrDefault(u => u.Mobile == User.Identity.Name).Id;
			var orders = _context.Orders.Include(o => o.OrderProducts).ThenInclude(op => op.Product).Where(o => o.UserId == userId).ToList();
			orders.ForEach(o =>
			{
				if (o.Status == OrderStatusEnum.درحالارسال)
					if (o.OrderProducts.All(op => op.Status == OrderStatusEnum.ارسالشده))
						o.Status = OrderStatusEnum.ارسالشده;
			});
			return View(orders);
		}
		[HttpGet]
		public IActionResult DetailsOrder(int id)
		{
			var userId = _context.Users.FirstOrDefault(u => u.Mobile == User.Identity.Name).Id;
			var order = _context.Orders.Include(o => o.OrderProducts).ThenInclude(op => op.Product).ThenInclude(p => p.Store).FirstOrDefault(o => o.UserId == userId && o.Id == id);
			return View(order);
		}
		[HttpGet, Authorize]
		public IActionResult CancelOrder(int? id)
		{
			return View();
		}
		[HttpPost, Authorize]
		public IActionResult CancelOrder(int id)
		{
			var userId = _context.Users.FirstOrDefault(u => u.Mobile == User.Identity.Name).Id;
			var order = _context.Orders.Include(o => o.OrderProducts).FirstOrDefault(o => o.UserId == userId && o.Id == id);
			if (order == null)
				return RedirectToAction(nameof(ShowOrders));
			if (order.Status == OrderStatusEnum.درحالارسال)
			{
				var result = _onlinePayment.RefundCompletely(order.TrackingNumber);
				order.Status = OrderStatusEnum.لغوشده;
				order.OrderProducts.AsParallel().ForAll(op => op.Status = OrderStatusEnum.لغوشده);
				_context.Update(order);
				_context.SaveChanges();
			}
			return RedirectToAction(nameof(ShowOrders));
		}
		[Authorize, HttpGet]
		public IActionResult UserInformation()
		{
			var user = _context.Users.Include(u => u.Addresses).FirstOrDefault(u => u.Mobile == User.Identity.Name);
			ViewBag.MyMessage = false;
			return View(user);
		}
		[Authorize, HttpPost]
		public IActionResult UserInformation(User user)
		{
			ModelState.Remove("Mobile");
			if (!ModelState.IsValid)
			{
				ViewBag.MyMessage = false;
				return View(user);
			}
			var userEntity = _context.Users.Include(u => u.Addresses).FirstOrDefault(u => u.Mobile == User.Identity.Name);
			userEntity.Addresses = user.Addresses;
			userEntity.FullName = user.FullName;
			userEntity.Code = user.Code;
			if (user.Password != userEntity.Password)
			{
				userEntity.Password = HashGenerators.MD5Encoding(user.Password);
			}
			_context.Update(userEntity);
			_context.SaveChanges();
			ViewBag.MyMessage = true;
			return View(user);
		}

		#region api
		public IActionResult GetComments(int productId)
		{
			int userId = 0;
			if (User.Identity.IsAuthenticated)
			{
				var mobile = User.Identity.Name;
				userId = _context.Users.FirstOrDefault(u => u.Mobile == mobile).Id;
			}
			List<Comment> comments = _context.Comments.Include(c => c.ChildComments)
				.IncludeFilter(c => c.CommentLikes.Where(cl => cl.UserId == userId))
				.Include(c => c.User)
				.Where(c => c.ProductId == productId)
				.ToList();

			List<Comment> replyProduct = comments.Where(c => c.ReplyCommentId == null).OrderByDescending(c => c.Id).ToList();

			var replyCommentsDto = replyProduct.Select(c => _mapper.Map<GetCommentDto>(c)).ToList();

			return Ok(replyCommentsDto);
		}
		public IActionResult LikeComment(int commentId)
		{
			ResponseService<bool> response = new Core.Dtoes.ResponseService<bool>();
			if (!User.Identity.IsAuthenticated)
			{
				response.Data = false;
				response.Message = "لطفا ابتدا احراز هویت کنید";
				return BadRequest(response);
			}
			if (!_context.Comments.Any(c => c.Id == commentId))
			{
				response.Data = false;
				response.Message = "لطفا ما را امتحان نکنید";
				return BadRequest(response);
			}
			var mobileNumber = User.Identity.Name;
			var user = _context.Users.Include(u => u.CommentLikes).FirstOrDefault(u => u.Mobile == mobileNumber);
			var commentLike = user.CommentLikes.FirstOrDefault(cl => cl.CommentId == commentId);
			if (commentLike != null)
				_context.CommentLikes.Remove(commentLike);
			else
			{
				commentLike = new CommentLike() { UserId = user.Id, CommentId = commentId };
				_context.CommentLikes.Add(commentLike);
			}
			_context.SaveChanges();
			response.Data = true;
			response.Message = "عملیات با موفقیت انجام شد";
			return Ok(response);
		}
		[HttpGet]
		public IActionResult AddComment(int productId, int replyCommentId, string text)
		{
			ResponseService<GetCommentDto> response = new Core.Dtoes.ResponseService<GetCommentDto>();

			var addCommentDto = new AddCommentDto();
			if (!User.Identity.IsAuthenticated)
			{
				response.Success = false;
				response.Message = "لطفا ابتدا احراز هویت کنید";
				return Unauthorized(response);
			}
			int depth = 0;
			if (replyCommentId != 0)
			{
				Comment parent = _context.Comments.FirstOrDefault(c => c.Id == replyCommentId);
				for (depth = 1; parent != null && parent.ReplyCommentId != null; depth++)
				{
					parent = _context.Comments.FirstOrDefault(c => c.Id == parent.ReplyCommentId);
				}
				addCommentDto.ReplyCommentId = replyCommentId;
			}
			addCommentDto.UserId = _context.Users.FirstOrDefault(u => u.Mobile == User.Identity.Name).Id;
			addCommentDto.ProductId = productId;
			addCommentDto.Text = text;
			addCommentDto.Depth = depth;
			if (!ModelState.IsValid || text.Length > 100)
			{
				response.Success = false;
				response.Message = "لطفا مقادیر را بررسی کنید و دوباره ارسال کنید";
				return BadRequest(response);
			}
			var comment = _mapper.Map<Comment>(addCommentDto);
			_context.Add(comment);
			_context.SaveChanges();
			response.Message = "نظر شما ثبت شد";
			response.Data = _mapper.Map<GetCommentDto>(comment);
			return Ok(response);
		}

		#endregion
	}
}