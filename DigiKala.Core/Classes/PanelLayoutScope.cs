using System;
using System.Collections.Generic;
using System.Text;

using DigiKala.DataAccessLayer.Entities;

using DigiKala.Core.Interfaces;
using DigiKala.Core.Services;

namespace DigiKala.Core.Classes
{
    public class PanelLayoutScope
    {
        private IUser _user;
        private IAdmin _admin;
        private IStore _store;

        public PanelLayoutScope(IUser user, IAdmin admin, IStore store)
        {
            _user = user;
            _admin = admin;
            _store = store;
        }

        public string GetUserRoleName(string username)
        {
            return _user.GetUserRoleName(username);
        }

        public int GetBrandsCount()
        {
            return _admin.GetBrandsNotShowCount();
        }

        public bool ExistsFieldCategory(int id, int catid)
        {
            return _admin.ExistsFieldCategory(id, catid);
        }

        public ProductField GetProductField(int id, int pid)
        {
            return _store.GetProductField(id, pid);
        }

        public int GetProductSeen(int id)
        {
            return _store.GetProductSeen(id);
        }
    }
}
