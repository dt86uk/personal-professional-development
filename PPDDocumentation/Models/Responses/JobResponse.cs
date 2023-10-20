using PPDDocumentation.Models.Job;

namespace PPDDocumentation.Models.Responses
{
    public class JobResponse : BaseResponse
    {
        public JobModel Job { get; set; }
    }
}
