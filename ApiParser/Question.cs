using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ApiParser
{

    public class Question
    {
        public int feedbackId { get; set; }
        public string question { get; set; }

        [JsonProperty("answers")]
        public List<Anwser> anwsers { get; set; }

        public Question()
        {

        }
    }
}
