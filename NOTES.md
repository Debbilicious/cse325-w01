# W01 Notes: Build .NET Applications with C#

## 1. Pizza list evidence (web API module)

GET http://localhost:5263/pizza returned status 200 with the two original pizzas plus my additional pizza (Margherita, id 3):

```json
[{"id":1,"name":"Classic Italian","isGlutenFree":false},{"id":2,"name":"Veggie","isGlutenFree":true},{"id":3,"name":"Margherita","isGlutenFree":false}]
```

Original pizzas: Classic Italian (id 1), Veggie (id 2).
My additional pizza: Margherita (id 3), added to the `Pizzas` list in `PizzaService`.

## 2. API operations (request, response, status code)

### GET
Request: `GET http://localhost:5263/pizza`
Response status: 200 OK

```json
[{"id":1,"name":"Classic Italian","isGlutenFree":false},{"id":2,"name":"Veggie","isGlutenFree":true},{"id":3,"name":"Margherita","isGlutenFree":false}]
```

### POST
Request: `POST http://localhost:5263/pizza` with header `Content-Type: application/json` and body:

```json
{"name":"Hawaii","isGlutenFree":false}
```

Response status: 201 Created

```json
{"id":4,"name":"Hawaii","isGlutenFree":false}
```

### PUT
Request: `PUT http://localhost:5263/pizza/4` with header `Content-Type: application/json` and body:

```json
{"id":4,"name":"Hawaiian","isGlutenFree":false}
```

Response status: 204 No Content (no body)

Check after the update: `GET http://localhost:5263/pizza/4` returned 200 OK:

```json
{"id":4,"name":"Hawaiian","isGlutenFree":false}
```

### DELETE
Request: `DELETE http://localhost:5263/pizza/4`
Response status: 204 No Content (no body)

## 3. Sales summary function (files and directories module)

The function writes `salesTotalDir/salesSummary.txt` using a StringBuilder and currency formatting:

```csharp
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
```

Output produced (`mslearn-dotnet-files/salesTotalDir/salesSummary.txt`):

```
Sales Summary
----------------------------
 Total Sales: $2,012.20

 Details:
  sales.json: $88.88
  201\sales.json: $501.22
  201\salestotals.json: $0.00
  202\sales.json: $1,234.22
  202\salestotals.json: $0.00
  203\sales.json: $99.00
  203\salestotals.json: $0.00
  204\sales.json: $88.88
  204\salestotals.json: $0.00
```
