using PPDDocumentation.Models.Requests;
using PPDDocumentation.Models.Responses;

namespace PPDDocumentation.BusinessLogic
{
    /// <summary>
    /// CRUD operations for Actions
    /// </summary>
    public interface IActionService
    {
        public ActionResponse GetActionById(Guid id);
        public ActionResponse DeleteAction(Guid id);
        public ActionResponse AddAction(ActionRequest request);
        public ActionResponse UpdateAction(ActionRequest request);
    }
}
