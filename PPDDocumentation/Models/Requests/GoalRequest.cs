using PPDDocumentation.Models.Goal;

namespace PPDDocumentation.Models.Requests
{
    public class GoalRequest : BaseRequest
    {
        public GoalModel Goal { get; set; }
    }
}
