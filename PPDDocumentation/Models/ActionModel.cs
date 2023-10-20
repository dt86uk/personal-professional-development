using Newtonsoft.Json;
using System.ComponentModel;

namespace PPDDocumentation.Models
{
    public class ActionModel
    {
        public ActionModel() { }

        public ActionModel(Guid id)
        {
            Id = id;   
        }

        [JsonProperty("id")]
        public Guid Id { get; private set; }
        public int OrderId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PercentageComplete { get; set; }

        [DisplayName("Complete?")]
        public bool IsComplete { get; set; }

        [DisplayName("What did I learn?")]
        public string WhatILearnt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
