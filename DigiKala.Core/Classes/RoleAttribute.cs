using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DigiKala.Core.Interfaces;
using DigiKala.Core.Services;
using DigiKala.DataAccessLayer.Context;
using DigiKala.DataAccessLayer.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace DigiKala.Core.Classes
{
    public class RoleAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        int _permissionID;

        //IUser _iuser;

        //public RoleAttribute(int permissionID)
        //{
        //    _permissionID = permissionID;
        //}

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                string username = context.HttpContext.User.Identity.Name;

                //_iuser = (IUser)context.HttpContext.RequestServices.GetService(typeof(IUser));

                var dbContext = (DatabaseContext)context.HttpContext.RequestServices.GetService(typeof(DatabaseContext));
                var cad = context.ActionDescriptor as ControllerActionDescriptor;
                List<RolePermission> rolePermissions = dbContext.RolePermissions.ToList();
                int permissionId = dbContext.Permissions.FirstOrDefault(p => p.Controller == cad.ControllerName && p.Action == cad.ActionName).Id;
                int roleId = dbContext.Users.FirstOrDefault(u => u.Mobile == username).RoleId;
                //int roleID = _iuser.GetUserRole(username);

                //if (!_iuser.ExistsPermission(_permissionID, roleID))
                if (!rolePermissions.Any(rp => rp.RoleId == roleId && rp.PermissionId == permissionId))
                {
                    context.Result = new RedirectResult("/Accounts/Login");
                }
            }
            else
            {
                context.Result = new RedirectResult("/Accounts/Login");
            }
        }
    }
}
