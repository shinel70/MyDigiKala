using System;
using System.Collections.Generic;
using System.Text;

using DigiKala.DataAccessLayer.Entities;

using DigiKala.Core.Interfaces;
using DigiKala.Core.Services;

namespace DigiKala.Core.Classes
{
    public class TemplateScope
    {
        private ITemp _temp;

        public TemplateScope(ITemp temp)
        {
            _temp = temp;
        }

        public bool CheckBanner(int id)
        {
            return _temp.CheckBannerImg(id);
        }

        public BannerDetails GetBanner(int id)
        {
            return _temp.GetBannerDetails(id);
        }
    }
}
