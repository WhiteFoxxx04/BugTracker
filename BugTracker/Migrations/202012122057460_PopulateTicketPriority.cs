namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateTicketPriority : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO TicketPriorities (Name) VALUES ('Low')");
            Sql("INSERT INTO TicketPriorities (Name) VALUES ('Medium')");
            Sql("INSERT INTO TicketPriorities (Name) VALUES ('High')");
            Sql("INSERT INTO TicketPriorities (Name) VALUES ('Critical')");

        }

        public override void Down()
        {
        }
    }
}
