﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DocumentFlow.Models
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext() : base("SomeCompany") { }

        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }

        public DbSet<DocumentModel> Documents { get; set; }
        public DbSet<DocumentTemplate> Templates { get; set; }
        public DbSet<Position> Positions { get; set; }

        public DbSet<DocumentType> DocumentTypes { get; set; }
    }
}