using BookShop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookShop.DataAccess.Repository.IRepository
{
    public interface IAuthorizationCounterRepository : IRepository<AuthorizationCounter>
    {
        void Update(AuthorizationCounter AuthCounter);
    }
}
