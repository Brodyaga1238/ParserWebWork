

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ParserWeb
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParserController : ControllerBase
    {
        public async static Task Main(string[] args)
        {
           await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        [HttpGet("startparsing")]
        public async Task<IActionResult> StartParsing()
        {
            try
            {
                DatabaseInitializer.Initialize();
                ISite site = new Fackel();
                var sitemapUrl = site.SitemapUrl;
                var urls = await ParserFunc.LoadUrlsFromSitemap(sitemapUrl, site);
                var processedurls = await GetProcessedUrls();
                var urlsToProcess = urls.Except(processedurls).ToList();
                await Db.ClearDatabases();
                await ParserFunc.ProcessUrlsAsync(urlsToProcess, site);
                return Ok("Parsing completed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("deleteDb")]
        public  async Task<IActionResult> DeleteDb()
        {
            await Db.ClearDatabases();
            return Ok("Db deleted");
        }
        static async Task<List<string>> GetProcessedUrls()
        {
            using (var db = new ApplicationContext())
            {   
                if (db.ProcessedUrls.Select(u => u.Url).ToListAsync<string>().ToString()=="")
                {
                    return new List<string>();
                }
                return await db.ProcessedUrls.Select(u => u.Url).ToListAsync<string>();
                
            }
        }
        [HttpGet("stopParsing")]
        public IActionResult StopParsing()
        {
            try
            {
                return Ok("Parsing process stopped successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while stopping parsing process: {ex.Message}");
            }
        }
    }
}
