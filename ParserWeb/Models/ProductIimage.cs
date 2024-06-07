using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ParserWeb
{
    public class ProductIImage
    {
        private int id;
        private Dictionary<string, string> image;
        private int productId;
        private Product product;

        public int Id
        {
            get => id;
        }

        [NotMapped]
        public Dictionary<string, string> Image
        {
            get => image;
            set => image = value ;
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
            get => productId;
            set
            {
                if (value >= 0)
                    productId = value;
            }
        }

        public Product Product
        {
            get => product;
            set => product = value ;
        }
    }
}