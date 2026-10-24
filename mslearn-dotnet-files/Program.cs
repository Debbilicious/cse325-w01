using System.Globalization;
using System.Text;
using Newtonsoft.Json;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

GenerateSalesSummary(salesFiles, storesDirectory, salesTotalDir);

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

void GenerateSalesSummary(IEnumerable<string> salesFiles, string storesDir, string outputDir)
{
    // Use US formatting so amounts always show a $ sign
    var culture = CultureInfo.GetCultureInfo("en-US");
    var details = new StringBuilder();
    double total = 0;

    foreach (var file in salesFiles)
    {
        string json = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(json);
        double fileTotal = data?.Total ?? 0;
        total += fileTotal;

        details.AppendLine($"  {Path.GetRelativePath(storesDir, file)}: {fileTotal.ToString("C", culture)}");
    }

    var report = new StringBuilder();
    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");
    report.AppendLine($" Total Sales: {total.ToString("C", culture)}");
    report.AppendLine();
    report.AppendLine(" Details:");
    report.Append(details);

    File.WriteAllText(Path.Combine(outputDir, "salesSummary.txt"), report.ToString());
}

record SalesData (double Total);