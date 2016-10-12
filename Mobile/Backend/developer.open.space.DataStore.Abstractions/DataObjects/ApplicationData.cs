using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Abstractions.DataObjects
{
    public class ApplicationData : BaseDataObject
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
