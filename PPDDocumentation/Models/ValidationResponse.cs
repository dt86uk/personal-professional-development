namespace PPDDocumentation.Models
{
    public class ValidationResponse
    {
        public ValidationResponse()
        {
            ErrorMessages = new List<string>();
        }

        public bool IsValid { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}
