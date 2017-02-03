using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace open.conference.app.DataStore.Abstractions.DataObjects
{
    public class FloorImage
    {
        public string ImageUrl { get; set; }
        public ImageSource Image { get; set; }
        public string ImageTitle { get; set; }
    }
}
