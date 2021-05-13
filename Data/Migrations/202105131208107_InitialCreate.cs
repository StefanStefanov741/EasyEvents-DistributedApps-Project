namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        title = c.String(nullable: false),
                        description = c.String(nullable: false),
                        location = c.String(nullable: false),
                        host_id = c.Int(nullable: false),
                        likes = c.Int(nullable: false),
                        createdOn = c.DateTime(nullable: false),
                        begins = c.DateTime(nullable: false),
                        ended = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Friends",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        user1_id = c.Int(nullable: false),
                        user2_id = c.Int(nullable: false),
                        befriend_date = c.DateTime(nullable: false),
                        pending = c.Boolean(nullable: false),
                        friendshipTier = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        username = c.String(nullable: false, maxLength: 16),
                        password = c.String(nullable: false),
                        displayName = c.String(nullable: false, maxLength: 16),
                        email = c.String(nullable: false),
                        phone_number = c.String(),
                        bio = c.String(maxLength: 500),
                        socialLink = c.String(),
                        rating = c.Single(nullable: false),
                        birthday = c.DateTime(nullable: false),
                        gender = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
            DropTable("dbo.Friends");
            DropTable("dbo.Events");
        }
    }
}
