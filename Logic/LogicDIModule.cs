using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Context;

namespace Logic
{
    public class LogicDIModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<OnlineShopContext>().ToSelf();
        }
    }
}
