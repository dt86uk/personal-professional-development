using PPDDocumentation.Models;

namespace PPDDocumentation.Helpers
{
    public static class GoalHelper
    {
        /// <summary>
        /// Calculates the average perecentage for a goal via all it's action's percentage complete value(s).
        /// </summary>
        /// <param name="missionStatement"></param>
        /// <returns>int</returns>
        public static int CalculatePercentageComplete(MissionStatementModel missionStatement, Guid goalId)
        {
            var goal = missionStatement.GoalsMe.Where(p => p.Id == goalId).SingleOrDefault();
            var totalPercentage = 0;
            foreach (var action in goal.Actions)
            {
                totalPercentage += action.PercentageComplete;
            }

            var totalActions = goal.Actions.Count;
            var goalPercentage = totalPercentage / totalActions;

            return goalPercentage;
        }
    }
}
