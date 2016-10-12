using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace developer.open.space.server.backend.Models
{
    public class Tweet
    {
        public string TweetedImage { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ScreenName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    }
}