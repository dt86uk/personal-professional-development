namespace PPDDocumentation.Models.Responses
{
    public class BaseResponse
    {
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}
