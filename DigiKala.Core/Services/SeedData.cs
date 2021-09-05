using DigiKala.Core.Classes;
using DigiKala.DataAccessLayer.Context;
using DigiKala.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace DigiKala.Core.Services
{
    public class SeedData
    {
        public void Seed(DatabaseContext context, IActionDescriptorCollectionProvider actionDescriptorCollection)
        {
            #region Roles
            List<Role> roles = context.Roles.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission).ToList();
            var newRoles = new List<Role>();
            if (!roles.Any(r => r.Name == StaticData.Admin))
                newRoles.Add(new Role() { Name = StaticData.Admin, RolePermissions = new List<RolePermission>() });
            if (!roles.Any(r => r.Name == StaticData.Store))
                newRoles.Add(new Role() { Name = StaticData.Store, RolePermissions = new List<RolePermission>() });
            if (!roles.Any(r => r.Name == StaticData.User))
                newRoles.Add(new Role() { Name = StaticData.User, RolePermissions = new List<RolePermission>() });
            #endregion

            #region permissions
            var permissions = context.Permissions.ToList();
            List<Permission> newPermissions = new List<Permission>();
            IReadOnlyList<ActionDescriptor> actionDescriptors = actionDescriptorCollection.ActionDescriptors.Items;
            foreach (var actionDescriptor in actionDescriptors)
            {
                var controllerAD = actionDescriptor as ControllerActionDescriptor;
                if (!StaticData.AuthorizedControllers.Contains(controllerAD.ControllerName))
                    continue;
                if (permissions.Any(p => p.Controller == controllerAD.ControllerName && p.Action == controllerAD.ActionName))
                    continue;
                if (newPermissions.Any(p => p.Controller == controllerAD.ControllerName && p.Action == controllerAD.ActionName))
                    continue;
                newPermissions.Add(new Permission()
                {
                    Name = ProcessPermissionName(controllerAD.ControllerName, controllerAD.ActionName),
                    Controller = controllerAD.ControllerName,
                    Action = controllerAD.ActionName
                });
            }
            #endregion

            #region RolePermissions
            foreach (Permission permission in permissions)
            {
                //check permission type is store?
                if (permission.Controller == StaticData.AuthorizedControllers[0])
                {
                    //if it is for Store but store role not exist go next
                    if (!roles.Any(r => r.Name == StaticData.Store))
                    {
                        //then it must be in new Roles we add this permission to that
                        newRoles.FirstOrDefault(r => r.Name == StaticData.Store).RolePermissions.Add(new RolePermission() { Permission = permission });
                        continue;
                    }
                    //if role exist then check for permission is exist? if not then add permission
                    if (!roles.FirstOrDefault(r => r.Name == StaticData.Store).RolePermissions.Any(rp => rp.PermissionId == permission.Id))
                        roles.FirstOrDefault(r => r.Name == StaticData.Store).RolePermissions.Add(new RolePermission() { Permission = permission });
                }
                else if (permission.Controller == StaticData.AuthorizedControllers[1])
                {
                    if (!roles.Any(r => r.Name == StaticData.Admin))
                    {
                        newRoles.FirstOrDefault(r => r.Name == StaticData.Admin).RolePermissions.Add(new RolePermission() { Permission = permission });
                        continue;
                    }
                    if (!roles.FirstOrDefault(r => r.Name == StaticData.Admin).RolePermissions.Any(rp => rp.PermissionId == permission.Id))
                        roles.FirstOrDefault(r => r.Name == StaticData.Admin).RolePermissions.Add(new RolePermission() { Permission = permission });
                }
            }
            foreach (Permission newPermission in newPermissions)
            {
                //permission type - store or admin
                if (newPermission.Controller == StaticData.AuthorizedControllers[0])
                {
                    if (!roles.Any(r => r.Name == StaticData.Store))
                    {
                        newRoles.FirstOrDefault(r => r.Name == StaticData.Store).RolePermissions.Add(new RolePermission() { Permission = newPermission });
                        continue;
                    }
                    roles.FirstOrDefault(r => r.Name == StaticData.Store).RolePermissions.Add(new RolePermission() { Permission = newPermission });
                }
                else if (newPermission.Controller == StaticData.AuthorizedControllers[1])
                {
                    if (!roles.Any(r => r.Name == StaticData.Admin))
                    {
                        newRoles.FirstOrDefault(r => r.Name == StaticData.Admin).RolePermissions.Add(new RolePermission() { Permission = newPermission });
                        continue;
                    }
                    roles.FirstOrDefault(r => r.Name == StaticData.Admin).RolePermissions.Add(new RolePermission() { Permission = newPermission });
                }
            }
            #endregion

            context.Permissions.UpdateRange(permissions);
            context.Permissions.AddRange(newPermissions);
            context.Roles.UpdateRange(roles);
            context.Roles.AddRange(newRoles);
            //a point you cant take all roles withour calling SaveChanges() from context
            context.SaveChanges();

            #region Users
            List<User> users = context.Users.Where(u => u.Mobile == StaticData.AdminMobile || u.Mobile == StaticData.StoreMobile || u.Mobile == StaticData.UserMobile).ToList();
            List<Role> AllRoles = context.Roles.ToList();
            var newUsers = new List<User>();

            if (users.Any(u => u.Mobile == StaticData.AdminMobile))
            {
                users.FirstOrDefault(u => u.Mobile == StaticData.AdminMobile).RoleId = AllRoles.FirstOrDefault(r => r.Name == StaticData.Admin).Id;
            }
            else
            {
                newUsers.Add(new User()
                {
                    Mobile = StaticData.AdminMobile,
                    DateTime = DateTime.Now,
                    IsActive = true,
                    Password = HashGenerators.MD5Encoding(StaticData.AdminMobile),
                    RoleId = AllRoles.FirstOrDefault(r => r.Name == StaticData.Admin).Id
                });
            }

            if (users.Any(u => u.Mobile == StaticData.StoreMobile))
            {
                users.FirstOrDefault(u => u.Mobile == StaticData.StoreMobile).RoleId = AllRoles.FirstOrDefault(r => r.Name == StaticData.Store).Id;
            }
            else
            {
                newUsers.Add(new User()
                {
                    Mobile = StaticData.StoreMobile,
                    DateTime = DateTime.Now,
                    IsActive = true,
                    Password = HashGenerators.MD5Encoding(StaticData.StoreMobile),
                    RoleId = AllRoles.FirstOrDefault(r => r.Name == StaticData.Store).Id,
                    Store = new Store()
                    {
                        Mail = StaticData.StoreMobile,
                        Tel = StaticData.StoreMobile,
                    }
                });
            }

            if (users.Any(u => u.Mobile == StaticData.UserMobile))
            {
                users.FirstOrDefault(u => u.Mobile == StaticData.UserMobile).RoleId = AllRoles.FirstOrDefault(r => r.Name == StaticData.User).Id;
            }
            else
            {
                newUsers.Add(new User()
                {
                    Mobile = StaticData.UserMobile,
                    DateTime = DateTime.Now,
                    IsActive = true,
                    Password = HashGenerators.MD5Encoding(StaticData.UserMobile),
                    RoleId = AllRoles.FirstOrDefault(r => r.Name == StaticData.User).Id
                });
            }
            #endregion

            context.Users.UpdateRange(users);
            context.Users.AddRange(newUsers);
            context.SaveChanges();


        }
        private string ProcessPermissionName(string controller, string action)
        {
            string name = "دسترسی ";
            if (controller == StaticData.AuthorizedControllers[1])
            {
                name += StaticData.Admin;
            }
            else if (controller == StaticData.AuthorizedControllers[0])
            {
                name += StaticData.Store;
            }
            else
            {
                return "";
            }
            if (action.Contains("Add"))
            {
                name += " اضافه کردن " + action.Replace("Add", "");
            }
            else if (action.Contains("Create"))
            {
                name += " اضافه کردن " + action.Replace("Create", "");
            }
            else if (action.Contains("Show"))
            {
                name += " نمایش " + action.Replace("Show", "");
            }
            else if (action.Contains("Delete"))
            {
                name += " پاک کردن " + action.Replace("Delete", "");
            }
            else if (action.Contains("Details"))
            {
                name += " نمایش جزئیات " + action.Replace("Details", "");
            }
            else if (action.Contains("Detail"))
            {
                name += " نمایش جزئیات " + action.Replace("Detail", "");
            }
            else if (action.Contains("Edit"))
            {
                name += " ویرایش " + action.Replace("Edit", "");
            }
            else if (action.Contains("Update"))
            {
                name += " ویرایش " + action.Replace("Update", "");
            }
            else
            {
                name += " نمایش " + action;
            }
            return name;
        }
    }
}
