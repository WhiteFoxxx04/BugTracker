namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateTicketStatus : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO TicketStatus (Name) VALUES ('Waiting for support')");
            Sql("INSERT INTO TicketStatus (Name) VALUES ('Waiting for customer')");
            Sql("INSERT INTO TicketStatus (Name) VALUES ('Resolved')");
            Sql("INSERT INTO TicketStatus (Name) VALUES ('On hold')");
        }

        public override void Down()
        {
        }
    }
}
