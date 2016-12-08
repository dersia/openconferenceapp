using Microsoft.Azure.Mobile.Server.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;

namespace open.conference.app.server.backend.Helpers
{
    sealed class MobileConfigProvider : MobileAppControllerConfigProvider
    {
        readonly Lazy<JsonSerializerSettings> settings = new Lazy<JsonSerializerSettings>(JsonConvert.DefaultSettings);

        public JsonSerializerSettings Settings => settings.Value;

        public override void Configure(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            base.Configure(controllerSettings, controllerDescriptor);
            controllerSettings.Formatters.JsonFormatter.SerializerSettings = Settings;
        }
    }
}