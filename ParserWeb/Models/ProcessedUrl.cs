namespace ParserWeb
{
    public class ProcessedUrl
    {
        private int id; 
        private string url { get; set; }
        public int Id 
        { 
            get => id;
        }

        public string Url
        {
            get => url;
            set
            {
                if (!string.IsNullOrEmpty(value))
                    url = value;
            }
        }
    }
}