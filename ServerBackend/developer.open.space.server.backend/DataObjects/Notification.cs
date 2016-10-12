using System;
using System.Collections.Generic;
using System.Text;

namespace developer.open.space.server.backend.DataObjects
{
    public class Notification : BaseDataObject
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
