using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace open.conference.app.Clients.ViewModels.Models
{
    public class Tweet
    {
        public Tweet()
        {
        }
        string tweetedImage;
        string fullImage;
        [JsonIgnore]
        public bool HasImage
        {
            get { return !string.IsNullOrWhiteSpace(tweetedImage); }
        }
        [JsonProperty("tweetedImage")]
        public string TweetedImage
        {
            get { return tweetedImage; }
            set
            {
                tweetedImage = value;
                fullImage = value;
                if (!string.IsNullOrWhiteSpace(tweetedImage))
                {
                    tweetedImage += ":thumb";
                }
            }
        }

        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("screenName")]
        public string ScreenName { get; set; }
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public string TitleDisplay { get { return Name; } }
        [JsonIgnore]
        public string SubtitleDisplay { get { return "@" + ScreenName; } }
        [JsonIgnore]
        public string DateDisplay { get { return CreatedDate.ToString(); } }
        [JsonIgnore]
        public Uri TweetedImageUri
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(TweetedImage))
                        return null;

                    return new Uri(TweetedImage);
                }
                catch
                {

                }
                return null;
            }
        }
    }
}
