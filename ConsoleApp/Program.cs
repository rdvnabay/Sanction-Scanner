using ConsoleApp;
using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;

/// <summary>
/// Proje .Net 6 ile geliştirildi.
/// </summary>

string BASE_URL = "https://www.sahibinden.com";

int counter = 0;
double totalPrice = 0;
List<Product> products = new();


var driverService = ChromeDriverService.CreateDefaultService();
driverService.HideCommandPromptWindow = true;

var driver = new ChromeDriver(driverService, new ChromeOptions());
driver.Manage().Window.Maximize();
driver.Navigate().GoToUrl(BASE_URL);

var document = new HtmlDocument();
document.LoadHtml(driver.PageSource);



// Anasayfa Vitrininde bulunan ilanlar listeye eklendi.
var listItem = document.DocumentNode.SelectNodes("//div[@class='uiBox showcase']//ul//li//a[@href]");

foreach (var item in listItem)
{
    try
    {
        var documentDetail = new HtmlDocument();

        var product = new Product
        {
            Title = item.GetAttributeValue("title", string.Empty),
            Link = $"{BASE_URL}{item.GetAttributeValue("href", string.Empty)}"
        };
        products.Add(product);

        //İlana açıklama ve link bilgileri alındıktan sonra fiyat bilgisini almak için "detay" sayfasına yönlendirme işlemi yapıldı.
        driver.Navigate().GoToUrl(products[counter].Link);
        documentDetail.LoadHtml(driver.PageSource);

        //Detay sayfası üzerinden fiyat bilgisi alıdnı.
        product.Price = documentDetail.DocumentNode.SelectSingleNode("//div[@class='classifiedInfo ']//h3").GetDirectInnerText().Trim();

        counter++;
        totalPrice += double.Parse(product.Price.Split(product.Price.Substring(product.Price.Length - 3)).First()); // 120.000 TL bilgisinden son 3 karakter itibaren parçalanıp ilk fiyat bilgisi alındı.

        //txt dosyası bin/debug klasörü altında tutulmaktadır.
        string path = Directory.GetCurrentDirectory() + "VitrinList.txt";
        using (StreamWriter file = new(path))
        {
            file.WriteLine($"İlan Adı: {product.Title} - Fiyatı:{product.Price}");
        }

        Console.WriteLine(product.ToString());
        Console.WriteLine("---------------");
    }
    catch (Exception)
    {
        Console.WriteLine("An error occurred while importing content. Please try again");
        continue;
    }
}

double averageAmount = totalPrice / listItem.Count;
Console.WriteLine("Average Amount " + averageAmount);



