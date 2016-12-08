using System;
using System.Collections.Generic;
using System.Text;

namespace open.conference.app.server.backend.DataObjects
{
    public class Notification : BaseDataObject
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
