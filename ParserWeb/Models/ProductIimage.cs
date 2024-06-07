using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ParserWeb.Models
{
    public class ProductIImage
    {
        private int _id;
        private Dictionary<string, string> _image;
        private int _productId;
        private Product _product;

        public int Id
        {
            get => _id;
            set
            {
                if (value >= 0)
                    _id = value;
            }
        }

        [NotMapped]
        public Dictionary<string, string> Image
        {
            get => _image;
            set => _image = value ;
        }

        [Column(TypeName = "json")]
        public string? ImageJson
        {
            get
            {
                var options = new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };
                return JsonSerializer.Serialize(Image, options);
            }
            set
            {
                Image = JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? new Dictionary<string, string>();
            }
        }

        public int ProductId
        {
            get => _productId;
            set
            {
                if (value >= 0)
                    _productId = value;
            }
        }

        public Product Product
        {
            get => _product;
            set => _product = value ;
        }
    }
}