using open.conference.app.server.backend.DataObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace open.conference.app.server.backend.Models
{
    public class Photo : BaseDataObject
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}