using System;
using System.Collections.Generic;
using System.Text;

namespace open.conference.app.server.backend.DataObjects
{
    public class Favorite : BaseDataObject
    {
        public string UserId { get; set; }
        public string SessionId { get; set; }
        public string WorkshopId { get; set; }
    }
}
