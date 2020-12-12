namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TicketStatus : DbMigration
    {
        public string Name { get; internal set; }

        public override void Up()
        {
            CreateTable(
                "dbo.TicketStatus",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateIndex("dbo.Tickets", "TicketStatusId");
            AddForeignKey("dbo.Tickets", "TicketStatusId", "dbo.TicketStatus", "Id", cascadeDelete: true);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "TicketStatusId", "dbo.TicketStatus");
            DropIndex("dbo.Tickets", new[] { "TicketStatusId" });
            DropTable("dbo.TicketStatus");
        }
    }
}
