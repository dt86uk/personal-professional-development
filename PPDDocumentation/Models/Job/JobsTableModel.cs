using Newtonsoft.Json;

namespace PPDDocumentation.Models.Job
{
    public class JobsTableModel
    {
        [JsonProperty("id")]
        public Guid Id { get; private set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; }
    }
}