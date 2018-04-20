namespace todo.Models
{
    using Microsoft.Azure.Documents;
    using Newtonsoft.Json;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;

    using DADataType = System.ComponentModel.DataAnnotations.DataType;

    public class Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        [DataType(DADataType.MultilineText)]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "isComplete")]
        public bool Completed { get; set; }
    }

    public class ItemCollection : Collection<Item>
    {
    }
}