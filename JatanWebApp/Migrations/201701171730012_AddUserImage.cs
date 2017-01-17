namespace JatanWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserImage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserImages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ImagePath = c.String(),
                        UserFileName = c.String(),
                        CreatedOnUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "UserImageId", c => c.Long());
            CreateIndex("dbo.AspNetUsers", "UserImageId");
            AddForeignKey("dbo.AspNetUsers", "UserImageId", "dbo.UserImages", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "UserImageId", "dbo.UserImages");
            DropIndex("dbo.AspNetUsers", new[] { "UserImageId" });
            DropColumn("dbo.AspNetUsers", "UserImageId");
            DropTable("dbo.UserImages");
        }
    }
}
