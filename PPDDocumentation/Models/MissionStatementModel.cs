using Newtonsoft.Json;
using PPDDocumentation.Models.Goal;
using System.ComponentModel;

namespace PPDDocumentation.Models
{
    public class MissionStatementModel
    {
        [JsonProperty]
        public Guid Id { get; private set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public string Description { get; set; }

        [DisplayName("Percentage Complete")]
        public int PercentageComplete { get; set; }

        [DisplayName("Actions for Me")]
        public List<GoalModel> GoalsMe { get; set; }

        [DisplayName("Actions for the Boss")]
        public List<GoalModel> GoalsBoss { get; set; }

        [DisplayName("What did I learn?")]
        public string WhatILearnt { get; set; }

        /// <summary>
        /// Not sure what to do with this yet but it could act as evidence of Actions completed probably images saved as base64 string.
        /// </summary>
        public List<byte[]> Attachments { get; set; }

        [DisplayName("Deleted?")]
        public bool IsDeleted { get; set; }
    }
}