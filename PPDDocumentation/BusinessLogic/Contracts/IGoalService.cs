using PPDDocumentation.Models.Goal;
using PPDDocumentation.Models.Requests;
using PPDDocumentation.Models.Responses;

namespace PPDDocumentation.BusinessLogic
{
    /// <summary>
    /// CRUD operations for Goals
    /// </summary>
    public interface IGoalService
    {
        public List<GoalModel> GetGoals(bool includeDeleted = false);

        /// <summary>
        /// Returns Goals information in the format to display in the table on Index.cshtml
        /// </summary>
        /// <returns></returns>
        public List<GoalsTableModel> GetGoalsTable();
        public GoalResponse GetGoalById(Guid id);
        public GoalResponse DeleteGoal(Guid id);
        public GoalResponse AddGoal(GoalRequest request);
        public GoalResponse UpdateGoal(GoalRequest request);

        /// <summary>
        /// Builds up the HTML for the coloured Progress circles on index.cshtml
        /// </summary>
        /// <param name="goals"></param>
        public void SetGoalsProgressHtml(List<GoalModel> goals);
    }
}
