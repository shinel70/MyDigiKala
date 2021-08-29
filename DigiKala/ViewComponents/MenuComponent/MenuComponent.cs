using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DigiKala.Core.Interfaces;
using DigiKala.Core.Services;

using DigiKala.DataAccessLayer.Entities;

namespace DigiKala.ViewComponents.MenuComponent
{
    public class MenuComponent : ViewComponent
    {
        private ITemp _temp;

        public MenuComponent(ITemp temp)
        {
            _temp = temp;
        }

        public async Task<IViewComponentResult>InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("MyMenuView", _temp.GetCategories()));
        }
    }
}
