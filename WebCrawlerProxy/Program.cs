using System.Data.SQLite;
using System.Text.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebCrawlerProxy.Entidades;

namespace WebCrawlerProxy
{
    class Program
    {
        static SemaphoreSlim semaphore = new SemaphoreSlim(3); // Limita a 3 threads simultâneas
        static List<ProxyModel> proxies = new List<ProxyModel>(); // Lista global para armazenar os dados dos proxies
        static DateTime startTime;
        static DateTime endTime;
        static int totalPages = 0;
        static int totalRows = 0;
        static IWebDriver driver;

        static async Task Main(string[] args)
        {
            startTime = DateTime.Now;
            driver = new ChromeDriver(); // Inicializa o navegador

            List<Task> tasks = new List<Task>();

            int numberOfPages = 15; 

            for (int i = 1; i <= numberOfPages; i++)
            {
                int pageNumber = i;
                tasks.Add(Task.Run(async () => await ScrapePage(pageNumber)));
            }

            await Task.WhenAll(tasks);

            endTime = DateTime.Now;

            // Salvar os dados extraídos em um arquivo JSON
            string jsonString = JsonSerializer.Serialize(proxies);
            await File.WriteAllTextAsync("proxies.json", jsonString);

            // Salvar os metadados no banco de dados
            SaveMetadataToDatabase(startTime, endTime, totalPages, totalRows, "proxies.json");

            driver.Quit(); // Fecha o navegador após a conclusão
            Console.WriteLine("Raspagem concluída. Dados salvos em proxies.json e metadados salvos no banco de dados.");
        }

        static async Task ScrapePage(int pageNumber)
        {
            await semaphore.WaitAsync(); // Espera até que uma thread esteja disponível

            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                wait.Until(driver => driver.FindElements(By.XPath("//table[@id='proxy-list-table']/tbody/tr")).Count > 0);

                var rows = driver.FindElements(By.XPath("//table[@id='proxy-list-table']/tbody/tr"));
                foreach (var row in rows)
                {
                    try
                    {
                        var cells = row.FindElements(By.TagName("td"));
                        if (cells.Count >= 4)
                        {
                            ProxyModel proxy = new ProxyModel
                            {
                                IP = cells[0].Text,
                                Port = cells[1].Text,
                                Country = cells[2].Text,
                                Protocol = cells[3].Text
                            };
                            lock (proxies)
                            {
                                proxies.Add(proxy);
                            }
                        }
                    }
                    catch (StaleElementReferenceException)
                    {
                        // Se ocorrer uma exceção, espere por um curto período de tempo e tente novamente
                        Thread.Sleep(1000); // Espera por 1 segundo

                        // Tente novamente a operação que falhou
                        // Você pode optar por colocar este bloco de código em um loop para tentar várias vezes
                        var cells = row.FindElements(By.TagName("td"));
                        if (cells.Count >= 4)
                        {
                            ProxyModel proxy = new ProxyModel
                            {
                                IP = cells[0].Text,
                                Port = cells[1].Text,
                                Country = cells[2].Text,
                                Protocol = cells[3].Text
                            };
                            lock (proxies)
                            {
                                proxies.Add(proxy);
                            }
                        }
                    }
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        static void SaveMetadataToDatabase(DateTime start, DateTime end, int pages, int rows, string jsonFile)
        {
            string dbFileName = "webcrawler_metadata.db";
            string connectionString = $"Data Source={dbFileName};Version=3;";

            if (!File.Exists(dbFileName))
            {
                SQLiteConnection.CreateFile(dbFileName);
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string tableCreationQuery = @"
                    CREATE TABLE IF NOT EXISTS Metadata (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        StartTime TEXT NOT NULL,
                        EndTime TEXT NOT NULL,
                        TotalPages INTEGER NOT NULL,
                        TotalRows INTEGER NOT NULL,
                        JsonFileName TEXT NOT NULL
                    )";
                var command = new SQLiteCommand(tableCreationQuery, connection);
                command.ExecuteNonQuery();

                string insertQuery = @"
                    INSERT INTO Metadata (StartTime, EndTime, TotalPages, TotalRows, JsonFileName)
                    VALUES (@StartTime, @EndTime, @TotalPages, @TotalRows, @JsonFileName)";
                command = new SQLiteCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@StartTime", start.ToString("o"));
                command.Parameters.AddWithValue("@EndTime", end.ToString("o"));
                command.Parameters.AddWithValue("@TotalPages", pages);
                command.Parameters.AddWithValue("@TotalRows", rows);
                command.Parameters.AddWithValue("@JsonFileName", jsonFile);
                command.ExecuteNonQuery();

                connection.Close();
            }

            Console.WriteLine("Metadados salvos no banco de dados SQLite.");
        }


        static async Task SavePagePrint(string fileName)
        {
            // Captura a screenshot
            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();

            // Salva a screenshot em um arquivo de imagem PNG
            await Task.Run(() =>
            {
                screenshot.SaveAsFile(fileName);
            });

            Console.WriteLine($"Print da página salvo em: {fileName}");
        }
    }
}
