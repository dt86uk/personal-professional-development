namespace PPDDocumentation.Models.Requests
{
    public class BaseRequest
    {
        public Guid Id { get; set; }

        public TaskViewModelBase NewTaskViewModel { get; set; }
    }
}
