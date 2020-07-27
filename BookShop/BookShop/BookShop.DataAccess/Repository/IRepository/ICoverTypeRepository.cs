using BookShop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookShop.DataAccess.Repository.IRepository
{
    public interface ICoverTypeRepository : IRepository<CoverType>
    {
        void Update(CoverType coverType);
    }
}
