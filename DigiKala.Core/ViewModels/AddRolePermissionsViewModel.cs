using DigiKala.DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigiKala.Core.ViewModels
{
    public class AddRolePermissionsViewModel
    {
        public Role Role { get; set; }
        public List<int> PermissionIds { get; set; }
    }
}
