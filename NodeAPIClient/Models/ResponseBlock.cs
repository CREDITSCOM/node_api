namespace NodeAPIClient.Models
{
    public class ResponseBlock
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Block Block { get; set; }
    }
}
