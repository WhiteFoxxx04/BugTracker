namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateTicketType : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO TicketTypes (Name) VALUES ('Report Error')");
            Sql("INSERT INTO TicketTypes (Name) VALUES ('Request Feature')");
            Sql("INSERT INTO TicketTypes (Name) VALUES ('Request Service')");
            Sql("INSERT INTO TicketTypes (Name) VALUES ('Others')");
        }

        public override void Down()
        {
        }
    }
}
