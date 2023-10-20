using PPDDocumentation.Models.Job;

namespace PPDDocumentation.Models.Requests
{
    public class JobRequest : BaseRequest
    {
        public JobModel Job { get; set; }
    }
}
