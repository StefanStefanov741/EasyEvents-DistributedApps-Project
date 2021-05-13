namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Friends", newName: "FriendShips");
            AlterColumn("dbo.Events", "title", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Events", "description", c => c.String(nullable: false, maxLength: 2500));
            AlterColumn("dbo.Events", "location", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.Users", "password", c => c.String(nullable: false, maxLength: 16));
            AlterColumn("dbo.Users", "email", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Users", "phone_number", c => c.String(maxLength: 15));
            AlterColumn("dbo.Users", "socialLink", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "socialLink", c => c.String());
            AlterColumn("dbo.Users", "phone_number", c => c.String());
            AlterColumn("dbo.Users", "email", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "password", c => c.String(nullable: false));
            AlterColumn("dbo.Events", "location", c => c.String(nullable: false));
            AlterColumn("dbo.Events", "description", c => c.String(nullable: false));
            AlterColumn("dbo.Events", "title", c => c.String(nullable: false));
            RenameTable(name: "dbo.FriendShips", newName: "Friends");
        }
    }
}
