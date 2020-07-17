﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVC5_Online_shop.Models.Data
{
    public class Db: DbContext
    {
        public DbSet<PagesDTO> Pages { get; set; }

        public DbSet<SidebarDTO> Sidebars { get; set; }

    }
}