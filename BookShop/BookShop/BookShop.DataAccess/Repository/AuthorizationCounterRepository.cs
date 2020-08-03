using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookShop.DataAccess.Repository
{
    class AuthorizationCounterRepository : Repository<AuthorizationCounter>, IAuthorizationCounterRepository
    {
        private readonly ApplicationDbContext _db;

        public AuthorizationCounterRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(AuthorizationCounter AuthCounter)
        {
            _db.Update(AuthCounter);
        }
    }
}
