
using Microsoft.EntityFrameworkCore;

namespace ParserWeb
{
    public class Db
    {
        public static async Task AddImageToDb(ProductIImage product1)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.ProductImages.Add(product1);
                await db.SaveChangesAsync();
               
            }
        }
        public static async Task ClearDatabases()
        {
            using (var db = new ApplicationContext())
            {
                db.ProductsDb.RemoveRange(db.ProductsDb);
                db.ProductImages.RemoveRange(db.ProductImages);
                //db.ProcessedUrls.RemoveRange(db.ProcessedUrls);
                await db.SaveChangesAsync();
            }

            
                using (var db = new ApplicationContext())
                {
                    db.ProcessedUrls.RemoveRange(db.ProcessedUrls);
                    await db.SaveChangesAsync();
                }
           
           
        }

       
        public static async Task AddDb(Product product1)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.ProductsDb.Add(product1);
                var help = product1.OriginUrl;
                ProcessedUrl help1 = new ProcessedUrl();
                help1.Url = help;
                db.ProcessedUrls.Add(help1);
                await db.SaveChangesAsync();
                Console.WriteLine($"Объект успешно сохранены url:{product1.OriginUrl}");
            }
        }
    }
    
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            using (var db = new ApplicationContext())
            {
                if (!db.Database.CanConnect())
                {
                    Console.WriteLine("База данных не существует. Создание новой базы данных...");
                    db.Database.EnsureCreated();
                }
            }
        }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<Product> ProductsDb { get; set; }
        public DbSet<ProductIImage> ProductImages { get; set; }
        public DbSet<ProcessedUrl> ProcessedUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductIImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dataSourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataSource");
            if (!Directory.Exists(dataSourcePath))
            {
                Directory.CreateDirectory(dataSourcePath);
            }
            var databasePath = Path.Combine(dataSourcePath, "ProductDb.db");
            optionsBuilder.UseSqlite($"Data Source={databasePath}");
        }
    }

}
    