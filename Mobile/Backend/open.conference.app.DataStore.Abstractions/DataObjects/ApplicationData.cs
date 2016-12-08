using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Abstractions.DataObjects
{
    public class ApplicationData : BaseDataObject
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
