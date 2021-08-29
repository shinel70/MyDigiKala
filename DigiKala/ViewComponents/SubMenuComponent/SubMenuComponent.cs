using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DigiKala.Core.Interfaces;
using DigiKala.Core.Services;

using DigiKala.DataAccessLayer.Entities;

using Microsoft.AspNetCore.Mvc;

namespace DigiKala.ViewComponents.SubMenuComponent
{
    public class SubMenuComponent : ViewComponent
    {
        private ITemp _temp;

        public SubMenuComponent(ITemp temp)
        {
            _temp = temp;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            return await Task.FromResult((IViewComponentResult)View("MySubMenuView", _temp.GetCategoryById(id)));
        }
    }
}
