using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AzureFuncHtmlResume.Models
{
    public class Item
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("visitorCount")]
        public int VisitorCount { get; set; }
    }
}
