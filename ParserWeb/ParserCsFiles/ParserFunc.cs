using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;

namespace ParserWeb
{
    public class ParserFunc
    {
        public static async Task<List<string>> LoadUrlsFromSitemap(string sitemapUrl, ISite site)
        {
            using (var client = new WebClient())
            {
                var sitemapXml = await client.DownloadStringTaskAsync(sitemapUrl);
                var urls = new List<string>();

                using (var reader = new XmlTextReader(new StringReader(sitemapXml)))
                {
                    try
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "loc")
                            {
                                reader.Read();
                                var url = reader.Value;
                                if (IsProductUrl(url, site))
                                {
                                    urls.Add(url);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }

                return urls;
            }
        }

        public static async Task ProcessUrlsAsync(List<string> urls, ISite site)
        {
            await Task.Run(async () =>
            {
                const int semaphoreCount = 25;
                var stopwatch = Stopwatch.StartNew();
                Console.WriteLine($"Parsing started at {DateTime.Now}");
                var semaphore = new SemaphoreSlim(semaphoreCount);

                try
                {
                    var tasks = urls.Select(url => ProcessUrlAsync(url, semaphore, site));
                    await Task.WhenAll(tasks);
                }
                finally
                {
                    stopwatch.Stop();
                    var elapsedTime = stopwatch.Elapsed;
                    Console.WriteLine($"Time taken for parsing: {elapsedTime}");
                }
            });
        }

        public static async Task ProcessUrlAsync(string url, SemaphoreSlim semaphore, ISite site)
        {
            await semaphore.WaitAsync();
            Console.WriteLine(url);
            var product = await ParserAsync(url, site);
            if (product.Avaible)
                await Db.AddDb(product);
            if (product.Avaible)
                await ImageParse(url, product, site);
            semaphore.Release();
        }



        private static bool IsProductUrl(string url, ISite site)
        {
            var pattern = site.IsProductPattern;
            return Regex.IsMatch(url, pattern);
        }

        private static async Task<Product> ParserAsync(string url, ISite site)
        {
            var product = new Product { OriginUrl = url };
            var doc = await LoadHtmlDocumentAsync(url);

            if (doc != null)
            {
                ParseName(doc, product, site);
                if (!product.Avaible) return product;
                ParseStock(doc, product, site);
                ParseDescription(doc, product, site);
                if (!product.Avaible) return product;
                ParseCategory(doc, product, site);
                ParsePrice(doc, product, site);
                ParseCharacteristics(doc, product, site);
            }

            return product;
        }

        private static async Task<HtmlDocument> LoadHtmlDocumentAsync(string url)
        {
            var web = new HtmlWeb();
            return await web.LoadFromWebAsync(url);
        }

        private static async Task ImageParse(string url, Product product, ISite site)
        {
            var doc = await LoadHtmlDocumentAsync(url);
            if (doc != null)
            {
                var imgNodes = doc.DocumentNode.SelectNodes(site.ImageUrl);
                if (imgNodes != null)
                {
                    foreach (var imgNode in imgNodes)
                    {
                        var imageUrl = imgNode.GetAttributeValue("href", "");
                        var fullImageUrl = site.BaseUrl + imageUrl;
                        var image = new ProductIImage { ProductId = product.Id };
                        image.Image = new Dictionary<string, string> { { "urls", fullImageUrl } };
                        await Db.AddImageToDb(image);
                    }
                }
            }
        }

        private static void ParseStock(HtmlDocument doc, Product product, ISite site)
        {
            var stock = doc.DocumentNode.SelectNodes(site.StockXpath);
            if (stock != null)
            {
                var count = stock.SelectMany(c => Regex.Matches(c.InnerText.Trim(), @"-?\d+"))
                    .Sum(match => int.Parse(match.Value));
                product.Stock = count;
                product.Avaible = count != 0;
            }
        }

        private static void ParseCategory(HtmlDocument doc, Product product, ISite site)
        {
            var category = doc.DocumentNode.SelectSingleNode(site.CategoryXpath);
            if (category != null)
            {
                product.Category = TextCorrector(category.InnerText);
            }
        }

        private static void ParseName(HtmlDocument doc, Product product, ISite site)
        {
            var name = doc.DocumentNode.SelectSingleNode(site.NameXpath);
            if (name != null)
            {
                product.Name = TextCorrector(name.InnerText);
            }
            else
            {
                product.Avaible = false;
            }
        }

        private static void ParseDescription(HtmlDocument doc, Product product, ISite site)
        {
            var description = doc.DocumentNode.SelectNodes(site.DescriptionXpath);
            if (description != null)
            {
                foreach (var d in description)
                {
                    product.Description = TextCorrector(d.InnerText);
                }

                if (string.IsNullOrEmpty(product.Description))
                {
                    product.Avaible = false;
                    Console.WriteLine($"Empty string {product.OriginUrl}");
                }
            }
        }

        private static void ParsePrice(HtmlDocument doc, Product product, ISite site)
        {
            var price = doc.DocumentNode.SelectSingleNode(site.PriceXpath);
            if (price != null)
            {
                var priceText = price.InnerText;
                var match = Regex.Match(priceText, @"(\d+\s?)*(\d+\.\d+)?");
                if (int.TryParse(match.Value.Replace(" ", ""), out var price1))
                {
                    product.Price = price1;
                }
            }
        }

        private static void ParseCharacteristics(HtmlDocument doc, Product product, ISite site)
        {
            var test = new Dictionary<string, string>();
            var characteristicspar = doc.DocumentNode.SelectNodes(site.CharacteristicsXpathPar);
            if (characteristicspar != null)
            {
                var characteristicsparhelp = doc.DocumentNode.SelectNodes(site.CharacteristicsXpathParHelp);
                var type = characteristicsparhelp?[0].InnerText;
                var text = characteristicspar.Aggregate("",
                    (current, c) => current + (TextCorrector(c.InnerText) + " "));
                test.Add(type, text);
            }

            var characteristics = doc.DocumentNode.SelectNodes(site.CharacteristicsXpath);
            if (characteristics != null)
            {
                var characteristicshelp = doc.DocumentNode.SelectNodes(site.CharacteristicsXpathHelp);
                for (var i = 0; i < characteristics.Count; i++)
                {
                    var type = TextCorrector(characteristics[i].InnerText);
                    var text = TextCorrector(characteristicshelp?[i].InnerText);
                    test.Add(type, text);
                }
            }

            var characteristicscolororequ = doc.DocumentNode.SelectSingleNode(site.CharacteristicsColOrEqu);
            if (characteristicscolororequ != null)
            {
                var type = TextCorrector(characteristicscolororequ.InnerText);
                var characteristicscolorhelp = doc.DocumentNode.SelectNodes(site.CharacteristicsColOrEquHelp);
                if (characteristicscolorhelp != null)
                {
                    foreach (var c in characteristicscolorhelp)
                    {
                        var text = TextCorrector(c.InnerText);
                        test.Add(type, text);
                    }
                }
            }

            var characteristicscolororequ2 = doc.DocumentNode.SelectSingleNode(site.CharacteristicsColOrEqu2);
            if (characteristicscolororequ2 != null)
            {
                var type = TextCorrector(characteristicscolororequ2.InnerText);
                var characteristicscolorhelp = doc.DocumentNode.SelectNodes(site.CharacteristicsColOrEquHelp2);
                if (characteristicscolorhelp != null)
                {
                    foreach (var c in characteristicscolorhelp)
                    {
                        var text = TextCorrector(c.InnerText);
                        test.Add(type, text);
                    }
                }
            }

            product.Characteristics = test;
        }

        private static string TextCorrector(string text)
        {
            var cleanedText = Regex.Replace(text, @"\s+", " ");
            cleanedText = cleanedText.Replace("\n", "").Replace("\t", "").Replace("&quot;", "\"")
                .Replace("&#40;", "(").Replace("&#41;", ")").Replace("&nbsp;", " ")
                .Replace("&mdash;", "-").Replace("&#43;", "+").Trim();
            return cleanedText;
        }
    }
}