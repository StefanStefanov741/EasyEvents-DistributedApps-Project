namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConnectionsUpdateGetSet : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HostToEvents", "Host_id", c => c.Int(nullable: false));
            AddColumn("dbo.HostToEvents", "Event_id", c => c.Int(nullable: false));
            AddColumn("dbo.ParticipantToEvents", "Participant_id", c => c.Int(nullable: false));
            AddColumn("dbo.ParticipantToEvents", "Event_id", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParticipantToEvents", "Event_id");
            DropColumn("dbo.ParticipantToEvents", "Participant_id");
            DropColumn("dbo.HostToEvents", "Event_id");
            DropColumn("dbo.HostToEvents", "Host_id");
        }
    }
}
