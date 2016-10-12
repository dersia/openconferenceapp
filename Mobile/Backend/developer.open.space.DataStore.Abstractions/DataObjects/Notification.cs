using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Abstractions.DataObjects
{
    public class Notification : BaseDataObject
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
