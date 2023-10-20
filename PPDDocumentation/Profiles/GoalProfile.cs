using AutoMapper;
using PPDDocumentation.Models.Goal;

namespace PPDDocumentation.Profiles
{
    public class GoalProfile : Profile
    {
        public GoalProfile()
        {
            CreateMap<GoalModel, GoalsTableModel>();
        }
    }
}
