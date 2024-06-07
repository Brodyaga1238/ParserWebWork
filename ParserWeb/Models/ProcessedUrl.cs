namespace ParserWeb.Models
{
    public class ProcessedUrl
    {
        private int _id; 
        private string _url { get; set; }
        public int Id 
        { 
            get => _id;
            set
            {
                if (value >= 0)
                    _id = value;
            }
        }

        public string Url
        {
            get => _url;
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _url = value;
            }
        }
    }
}