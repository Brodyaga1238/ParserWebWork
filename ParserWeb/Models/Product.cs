using System.ComponentModel.DataAnnotations.Schema;

using System.Text.Json;
namespace ParserWeb
{ 
    public class Product
    {
        private int id;
        private string category;
        private string name;
        private string description;
        private int price;
        private int stock;
        private bool avaible ;
        private Dictionary<string, string> characteristics;
        private string originUrl;
        private List<ProductIImage> images;

        public int Id 
        { 
            get => id;
        }

        public string Category 
        { 
            get => category; 
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    category = value;
            }
        }

        public string Name 
        { 
            get => name; 
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    name = value;
            }
        }

        public string Description 
        { 
            get => description; 
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    description = value;
            }
        }

        public int Price 
        { 
            get => price; 
            set
            {
                if (value >= 0)
                    price = value;
            }
        }

        public int Stock 
        { 
            get => stock; 
            set
            {
                if (value >= 0)
                    stock = value;
            }
        }

        [NotMapped] 
        public bool Avaible  
        { 
            get => avaible  ; 
            set => avaible   = value; 
        }

        [NotMapped]
        public Dictionary<string, string> Characteristics 
        { 
            get => characteristics; 
            set => characteristics = value ?? new Dictionary<string, string>(); 
        }

        [Column(TypeName = "json")] 
        public string? CharacteristicsJson 
        {
            get
            {
                var options = new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };
                return JsonSerializer.Serialize(Characteristics, options);
            }
            set
            {
                Characteristics = JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? new Dictionary<string, string>();
            }
        } 

        public string OriginUrl 
        { 
            get => originUrl; 
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    originUrl = value;
            }
        }

        [NotMapped]
        public List<ProductIImage> Images 
        { 
            get => images; 
            set => images = value ?? new List<ProductIImage>(); 
        }

        // Конструкторы класса Product
        public Product()
        {
            Category = "-";
            Name = "-";
            Description = "-";
            Price = 0;
            Stock = 0;
            Characteristics = new Dictionary<string, string>();
            OriginUrl = "test";
            Avaible   = true;
            Images = new List<ProductIImage>();
        }

        public Product(string category, string name, string description, int price, int stock,
            string originUrl, Dictionary<string, string> characteristics)
        {
            Category = category;
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
            OriginUrl = originUrl;
            Characteristics = characteristics;
            Avaible  = true;
            Images = new List<ProductIImage>();
        }
    }
}
