using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using developer.open.space.server.backend.DataObjects;
using developer.open.space.server.backend.Models;
using Owin;
using System.Data.Entity.Migrations;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin;
using developer.open.space.server.backend.Helpers;

namespace developer.open.space.server.backend
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            new MobileAppConfiguration()
                .UseDefaultConfiguration()
                .ApplyTo(config);
            
            // Use Entity Framework Code First to create database tables based on your DbContext
            //Database.SetInitializer(new DevopenspaceContextInitializer());
            var migrator = new DbMigrator(new developer.open.space.server.backend.Migrations.Configuration());
            migrator.Update();

            MobileAppSettingsDictionary settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    // This middleware is intended to be used locally for debugging. By default, HostName will
                    // only have a value when running in an App Service application.
                    SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            }
            app.UseFileServer(new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                RequestPath = new PathString("/static"),
                FileSystem = new PhysicalFileSystem("Static"),
                StaticFileOptions = { ContentTypeProvider = new CustomContentTypeProvider() }
            });
            app.UseWebApi(config);
            ConfigureSwagger(config);
        }
    }
}

