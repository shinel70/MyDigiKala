using System;
using System.Collections.Generic;
using System.Text;

namespace DigiKala.Core.Classes
{
    public static class StaticData
    {
        public const string sessionCard = "Cart";

        public const string Admin = "مدیر";
        public const string Store = "فروشگاه";
        public const string User = "کاربر";

        public const string AdminMobile = "01234567890";
        public const string StoreMobile = "09876543210";
        public const string UserMobile = "01357924680";
        //be carefull change this because of DataSeeding in RolePermission section and Permission Section and is CaseSensitive
        public static readonly string[] AuthorizedControllers = { "Panel", "Admin" };
    }
}
