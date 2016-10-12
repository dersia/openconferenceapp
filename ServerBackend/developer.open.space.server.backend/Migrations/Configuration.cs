namespace developer.open.space.server.backend.Migrations
{
    using Microsoft.Azure.Mobile.Server.Tables;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<developer.open.space.server.backend.Models.DevopenspaceContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("System.Data.SqlClient", new EntityTableSqlGenerator());
            ContextKey = "developer.open.space.server.backend.Models.DevopenspaceContext";
        }

        protected override void Seed(developer.open.space.server.backend.Models.DevopenspaceContext context)
        {
        }
    }
}
