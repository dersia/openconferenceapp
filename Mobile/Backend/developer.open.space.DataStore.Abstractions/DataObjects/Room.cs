using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Abstractions.DataObjects
{
    public class Room : BaseDataObject
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the image URL if there is one
        /// </summary>
        /// <value>The image URL.</value>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the latitude if there is one
        /// </summary>
        /// <value>The latitude.</value>
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude if there is one
        /// </summary>
        /// <value>The longitude.</value>
        public double? Longitude { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Uri ImageUri
        {
            get
            {
                try
                {
                    return new Uri(ImageUrl);
                }
                catch
                {

                }
                return null;
            }
        }
    }
}
