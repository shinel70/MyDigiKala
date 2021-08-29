using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DigiKala.Core.Interfaces;
using DigiKala.Core.Services;

using DigiKala.DataAccessLayer.Entities;

namespace DigiKala.ViewComponents.SliderComponent
{
    public class SliderComponent : ViewComponent
    {
        private ITemp _temp;

        public SliderComponent(ITemp temp)
        {
            _temp = temp;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("MySliderView", _temp.GetSliders()));
        }
    }
}
