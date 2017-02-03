using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace open.conference.app.Clients.ViewModels.Models
{
    public class MenuItem : BindableBase
    {
        string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }
        string subtitle;
        public string Subtitle
        {
            get { return subtitle; }
            set { SetProperty(ref subtitle, value); }
        }

        public string Icon { get; set; }
        public string Parameter { get; set; }

        public AppPage Page { get; set; }
        public ICommand Command { get; set; }
        public string Pagename { get; set; }
    }
}
