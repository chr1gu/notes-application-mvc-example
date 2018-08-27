namespace NotesApplication.Models.ViewModels
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        
        public string Message { get; set; }
        
        public int StatusCode { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}