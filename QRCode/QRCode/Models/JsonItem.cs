using Newtonsoft.Json;

namespace QRCode.Models
{
    public class JsonItem
    {
        [JsonIgnore]
        public bool Checked { get; set; }   //勾选

        [JsonIgnore]
        public int Id { get; set; }   //编号

        [JsonProperty("Key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }   //键

        [JsonProperty("Value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }   //值

    }
}
