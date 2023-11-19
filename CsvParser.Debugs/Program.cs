// See https://aka.ms/new-console-template for more information

var file = Path.Join(Directory.GetCurrentDirectory(), "files", "Sample-Spreadsheet-2.csv");
var parsed = CsvParser.CsvParser.Parse(file);
