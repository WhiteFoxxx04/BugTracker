namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplyAnnotationsToProject : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Projects", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Projects", "Description", c => c.String(nullable: false, maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Projects", "Description", c => c.String());
            AlterColumn("dbo.Projects", "Name", c => c.String());
        }
    }
}
