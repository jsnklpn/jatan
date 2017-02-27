namespace JatanWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserStats : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Money", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "AverageTurnLength", c => c.Single(nullable: false));
            AddColumn("dbo.AspNetUsers", "TotalResourcesCollected", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "TotalMinutesPlayed", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "GamesWon", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "GamesPlayed", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "GamesPlayed");
            DropColumn("dbo.AspNetUsers", "GamesWon");
            DropColumn("dbo.AspNetUsers", "TotalMinutesPlayed");
            DropColumn("dbo.AspNetUsers", "TotalResourcesCollected");
            DropColumn("dbo.AspNetUsers", "AverageTurnLength");
            DropColumn("dbo.AspNetUsers", "Money");
        }
    }
}
