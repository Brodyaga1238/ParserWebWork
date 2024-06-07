using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ParserWeb.Models
{ 
    public class Product
    {
        private int _id;
        private string _category;
        private string _name;
        private string _description;
        private int _price;
        private int _stock;
        private bool _avaible ;
        private Dictionary<string, string> _characteristics;
        private string _originUrl;
        private List<ProductIImage> _images;

        public int Id 
        { 
            get => _id;
            set
            {
                if (value >= 0)
                    _id = value;
            }
        }

        public string Category 
        { 
            get => _category; 
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _category = value;
            }
        }

        public string Name 
        { 
            get => _name; 
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _name = value;
            }
        }

        public string Description 
        { 
            get => _description; 
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _description = value;
            }
        }

        public int Price 
        { 
            get => _price; 
            set
            {
                if (value >= 0)
                    _price = value;
            }
        }

        public int Stock 
        { 
            get => _stock; 
            set
            {
                if (value >= 0)
                    _stock = value;
            }
        }

        [NotMapped] 
        public bool Avaible  
        { 
            get => _avaible  ; 
            set => _avaible   = value; 
        }

        [NotMapped]
        public Dictionary<string, string> Characteristics 
        { 
            get => _characteristics; 
            set => _characteristics = value ?? new Dictionary<string, string>(); 
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
            get => _originUrl; 
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _originUrl = value;
            }
        }

        [NotMapped]
        public List<ProductIImage> Images 
        { 
            get => _images; 
            set => _images = value ?? new List<ProductIImage>(); 
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
