namespace CsvParser.Tests;

public class CsvParse_ParseShould
{
    [Fact]
    public void CsvParse_Should_parse_file_ends_with_a_newline_character()
    {
        var parsed = CsvParser.Parse(Path.Join(Directory.GetCurrentDirectory(), "files", "Sample-Spreadsheet-1.csv"));

        Assert.Single(parsed);
    }

    [Fact]
    public void CsvParse_Should_parse_file_without_newline_character()
    {
        var parsed = CsvParser.Parse(Path.Join(Directory.GetCurrentDirectory(), "files", "Sample-Spreadsheet-2.csv"));

        Assert.Single(parsed);
    }

    [Fact]
    public void CsvParse_Should_parse_multiple_lines()
    {
        var parsed = CsvParser.Parse(Path.Join(Directory.GetCurrentDirectory(), "files", "Sample-Spreadsheet-59507-rows.csv"));

        Assert.Equal(59507, parsed.Length);
    }

    [Fact]
    public void CsvParse_Should_parse_quoted_columns()
    {
        var parsed = CsvParser.Parse(Path.Join(Directory.GetCurrentDirectory(), "files", "Sample-Spreadsheet-1.csv"));

        Assert.Equal("Eldon Base for stackable storage shelf, platinum", parsed[0][0]);
    }

    [Fact]
    public void CsvParse_Should_parse_space_between_unquoted_columns()
    {
        var parsed = CsvParser.Parse(Path.Join(Directory.GetCurrentDirectory(), "files", "Sample-Spreadsheet-1.csv"));

        Assert.Equal("Storage & Organization", parsed[0][7]);
    }

    [Fact]
    public void CsvParse_Should_parse_complex_csv()
    {
        var expected = new string[][]
        {
            new string[]{ "Year", "Make", "Model", "Description", "Price" },
            new string[]{ "1997", "Ford", "E350", "ac, abs, moon", "3000.00" },
            new string[]{ "1999", "Chevy", "Venture \"Extended Edition\"", "", "4900.00" },
            new string[]{ "1999", "Chevy", "Venture \"Extended Edition, Very Large\"", "", "5000.00" },
            new string[]{ "1996", "Jeep", "Grand Cherokee","MUST SELL!\nair, moon roof, loaded", "4799.00" }
        };

        var actual = CsvParser.Parse(Path.Join(Directory.GetCurrentDirectory(), "files", "Wikipedia-Example.csv"));

        Assert.Equal(actual, expected);
    }
}