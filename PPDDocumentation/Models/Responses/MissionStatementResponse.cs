namespace PPDDocumentation.Models.Responses
{
    public class ActionResponse : BaseResponse
    {
        public Guid GoalId { get; set; }
        public ActionModel Action { get; set; }
    }
}
