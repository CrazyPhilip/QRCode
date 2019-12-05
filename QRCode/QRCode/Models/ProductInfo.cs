using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QRCode.Models
{
    public class ProductInfo
    {
        [JsonProperty("ProductName")]
        public string ProductName { get; set; }   //货物名称

        [JsonProperty("Weight")]
        public string Weight { get; set; }   //重量

        [JsonProperty("RecipientName")]
        public string RecipientName { get; set; }   //收件人姓名

        [JsonProperty("RecipientPhone")]
        public string RecipientPhone { get; set; }   //收件人电话

        [JsonProperty("RecipientAddress")]
        public string RecipientAddress { get; set; }   //收件人地址

        [JsonProperty("SenderName")]
        public string SenderName { get; set; }   //发件人姓名

        [JsonProperty("SenderPhone")]
        public string SenderPhone { get; set; }   //发件人电话

        [JsonProperty("SenderAddress")]
        public string SenderAddress { get; set; }   //发件人地址


    }
}
