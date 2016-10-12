using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.Utils.Helpers
{
    public static class NavigationUriFactory
    {
        public static Uri RelativeUri(params string[] pageNames)
        {
            return CreateUri(false, pageNames);
        }

        public static Uri AbsoluteUri(params string[] pageNames)
        {
            return CreateUri(true, pageNames);
        }

        static Uri CreateUri(bool absolute, params string[] pageNames)
        {
            var result = new StringBuilder(absolute ? "devopenspace:///" : string.Empty);
            foreach (var pageName in pageNames)
            {
                result.Append(pageName + "/");
            }

            var uri = new Uri(result.ToString(), absolute ? UriKind.Absolute : UriKind.Relative);
            return uri;
        }
    }
}
