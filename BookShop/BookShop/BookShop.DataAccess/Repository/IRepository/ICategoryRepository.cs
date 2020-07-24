using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category);
    }
}
