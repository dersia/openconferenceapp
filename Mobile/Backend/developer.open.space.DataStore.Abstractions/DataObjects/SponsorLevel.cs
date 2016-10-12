using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Abstractions.DataObjects
{
    public class SponsorLevel : BaseDataObject
    {
        /// <summary>
        /// Gets or sets the name.
        /// Such as Platinum
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the rank.
        /// 0 means show this sponsor level at top
        /// </summary>
        /// <value>The rank.</value>
        public int Rank { get; set; }
    }
}
