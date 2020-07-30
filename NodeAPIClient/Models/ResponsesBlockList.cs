using System.Collections.Generic;

namespace NodeAPIClient.Models
{
    public class ResponsesBlockList
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<ResponseBlock> Blocks { get; }

        public void SetState(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public ResponsesBlockList()
        {
            Blocks = new List<ResponseBlock>();
        }
    }
}
