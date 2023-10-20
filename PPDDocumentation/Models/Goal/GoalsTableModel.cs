using Newtonsoft.Json;

namespace PPDDocumentation.Models.Goal
{
    public class GoalsTableModel
    {
        [JsonProperty("id")]
        public Guid Id { get; private set; }
        public int OrderId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DueBy { get; set; }
        public string ProgressHtml { get; set; }
        public bool IsComplete { get; set; }
    }
}
