namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigrationAfterWipe : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        title = c.String(nullable: false, maxLength: 100),
                        description = c.String(nullable: false, maxLength: 2500),
                        location = c.String(nullable: false, maxLength: 200),
                        host_id = c.Int(nullable: false),
                        likes = c.Int(nullable: false),
                        createdOn = c.DateTime(nullable: false),
                        begins = c.DateTime(nullable: false),
                        ends = c.DateTime(nullable: false),
                        ended = c.Boolean(nullable: false),
                        participants = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Friendships",
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
                "dbo.HostToEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ParticipantToEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        username = c.String(nullable: false, maxLength: 16),
                        password = c.String(nullable: false, maxLength: 16),
                        displayName = c.String(nullable: false, maxLength: 16),
                        email = c.String(nullable: false, maxLength: 50),
                        phone_number = c.String(maxLength: 15),
                        bio = c.String(maxLength: 500),
                        socialLink = c.String(maxLength: 200),
                        rating = c.Single(nullable: false),
                        birthday = c.DateTime(),
                        gender = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
            DropTable("dbo.ParticipantToEvents");
            DropTable("dbo.HostToEvents");
            DropTable("dbo.Friendships");
            DropTable("dbo.Events");
        }
    }
}
