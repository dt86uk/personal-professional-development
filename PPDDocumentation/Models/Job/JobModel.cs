using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PPDDocumentation.Models.Job
{
    public class JobModel
    {
        public JobModel() { }

        public JobModel(Guid id)
        {
            Id = id;
        }

        [JsonProperty("id")]
        public Guid Id { get; private set; }

        [Required(ErrorMessage = "* Required")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "* Required")]
        public string Description { get; set; }
        public bool IsComplete { get; set; }
        public bool IsDeleted { get; set; }
    }
}
