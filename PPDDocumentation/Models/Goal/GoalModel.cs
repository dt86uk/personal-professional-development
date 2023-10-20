using Newtonsoft.Json;
using System.ComponentModel;

namespace PPDDocumentation.Models.Goal
{
    public class GoalModel
    {
        public GoalModel() { }

        public GoalModel(Guid id)
        {
            Id = id;
        }

        [JsonProperty("id")]
        public Guid Id { get; private set; }
        public int OrderId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DueBy { get; set; }

        [DisplayName("Percentage Complete")]
        public int PercentageComplete { get; set; }
        public string ProgressHtml { get; set; }
        public List<ActionModel> Actions { get; set; }

        [DisplayName("Completed?")]
        public bool IsComplete { get; set; }

        [DisplayName("What did I learn?")]
        public string WhatILearnt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
