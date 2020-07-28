using System;
using System.Collections.Generic;
using System.Text;

namespace BookShop.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        ICompanyRepository Company { get; }
        ISP_Call SP_Call { get; }
        ICoverTypeRepository CoverType { get; }
        IProductRepository Product { get; }
        IApplicationUserRepository ApplicationUser { get; }

        void Save();
    }
}
