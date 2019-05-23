using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace ApiParser
{
    public class Story
    {
        [JsonProperty ("storyId")]
        public int storyId { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("html")]
        public string html { get; set; }

        public Story()
        {

        }
    }
}
