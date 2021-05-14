namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserBirthdayEdit : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "birthday", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "birthday", c => c.DateTime(nullable: false));
        }
    }
}
