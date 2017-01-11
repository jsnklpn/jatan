using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JatanWebApp.Models.DAL
{
    public class JatanDbContext : IdentityDbContext<ApplicationUser>
    {
        public JatanDbContext()
            : base("JatanDbContext", throwIfV1Schema: false)
        {
        }

        public static JatanDbContext Create()
        {
            return new JatanDbContext();
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // The User/Identity DbSet (tables) properties are added automatically by IdentityDbContext.
    }
}