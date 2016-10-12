using Microsoft.Owin.StaticFiles.ContentTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace developer.open.space.server.backend.Helpers
{
    public class CustomContentTypeProvider : FileExtensionContentTypeProvider
    {
        public CustomContentTypeProvider()
        {
            Mappings.Add(".json", "application/json");
        }
    }
}