using CMS.CoreLib.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.App_Start
{
    public class DbInitializer
    {
        public static void Initialize()
        {
            WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);

            //var roleService = DependencyResolver.Current.GetService<IRoleService>();
            var uowFactory = DependencyResolver.Current.GetService<CoreLib.Data.IUnitOfWorkFactory<UnitOfWork>>();
        }
    }
}
