using developer.open.space.server.backend.DataObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace developer.open.space.server.backend.Models
{
    public class Photo : BaseDataObject
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}