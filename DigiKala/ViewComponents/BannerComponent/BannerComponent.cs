using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DigiKala.Core.Interfaces;
using DigiKala.Core.Services;

using Microsoft.AspNetCore.Mvc;

namespace DigiKala.ViewComponents.BannerComponent
{
    public class BannerComponent : ViewComponent
    {
        private ITemp _temp;

        public BannerComponent(ITemp temp)
        {
            _temp = temp;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("MyBannerView", _temp.GetBanners()));
        }
    }
}
