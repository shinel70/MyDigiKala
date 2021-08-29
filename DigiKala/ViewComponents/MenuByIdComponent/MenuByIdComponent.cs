using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DigiKala.Core.Interfaces;
using DigiKala.Core.Services;

using DigiKala.DataAccessLayer.Entities;

using Microsoft.AspNetCore.Mvc;

namespace DigiKala.ViewComponents.MenuByIdComponent
{
    public class MenuByIdComponent : ViewComponent
    {
        private ITemp _temp;

        public MenuByIdComponent(ITemp temp)
        {
            _temp = temp;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            return await Task.FromResult((IViewComponentResult)View("MyMenuByIdView", _temp.GetCategoryById(id)));
        }
    }
}
