using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Abstractions.DataObjects
{
    public class Feedback : BaseDataObject
    {
        public string UserId { get; set; }
        public string SessionId { get; set; }
        public int SessionRating { get; set; }
        public string WorkshopId { get; set; }
        public int WorkshopRating { get; set; }
    }
}
