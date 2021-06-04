namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RestrictionToFriendships : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Friendships", "friendshipTier", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Friendships", "friendshipTier", c => c.String());
        }
    }
}
