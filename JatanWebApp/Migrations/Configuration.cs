namespace JatanWebApp.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<JatanWebApp.Models.DAL.JatanDbContext>
    {
        // With regards to DB migrations, there are 2 important commands:
        //
        // PM> Add-Migration <MigrationName>
        //  * Takes a snapshot of the current DB context/models and creates a migration to that state.
        //  * You can run this command with an existing migration if you want to change it.
        //
        // PM> Update-Database
        //  * Updates local DB to the latest migration.

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "JatanWebApp.Models.DAL.JatanDbContext";
        }

        protected override void Seed(JatanWebApp.Models.DAL.JatanDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
