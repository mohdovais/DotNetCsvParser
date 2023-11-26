namespace CsvParser.Tests;

public class CsvParse_ParseTextShould
{

    [Fact]
    public void CsvParseText_Should_parse_complex_csv()
    {
        var text = @"Year,Make,Model,Description,Price
1997,Ford,E350,""ac, abs, moon"",3000.00
1999,Chevy,""Venture """"Extended Edition"""""","""",4900.00
1999,Chevy,""Venture """"Extended Edition, Very Large"""""",,5000.00
1996,Jeep,Grand Cherokee,""MUST SELL!
air, moon roof, loaded"",4799.00
";
        var expected = new string[][]
        {
            new string[]{ "Year", "Make", "Model", "Description", "Price" },
            new string[]{ "1997", "Ford", "E350", "ac, abs, moon", "3000.00" },
            new string[]{ "1999", "Chevy", "Venture \"Extended Edition\"", "", "4900.00" },
            new string[]{ "1999", "Chevy", "Venture \"Extended Edition, Very Large\"", "", "5000.00" },
            new string[]{ "1996", "Jeep", "Grand Cherokee","MUST SELL!\r\nair, moon roof, loaded", "4799.00" }
        };

        var actual = CsvParser.ParseText(text);

        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void CsvParseText_Should_Have_UniformRowLength_ByDefault()
    {
        var text = @"
a, b, c
a, b, c, d, e
a, b";
        var expected = new string[][]
        {
            new string[]{ "a", "b", "c", "", "" },
            new string[]{ "a", "b", "c", "d", "e" },
            new string[]{ "a", "b", "", "", "" }
        };

        var actual = CsvParser.ParseText(text.Trim());

        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void CsvParseText_Should_Have_SameRowLength_IfMatchFirstRow()
    {
        var text = @"
a, b, c
a, b, c, d, e
a, b";
        var expected = new string[][]
        {
            new string[]{ "a", "b", "c" },
            new string[]{ "a", "b", "c" },
            new string[]{ "a", "b", "" }
        };

        var actual = CsvParser.ParseText(text.Trim(), new()
        {
            Normalization = NormalizeType.MatchFirstRow
        });

        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void CsvParseText_Should_Have_DifferentRowLength_IfMatchFirstRow()
    {
        var text = @"
a, b, c
a, b, c, d, e
a, b";
        var expected = new string[][]
        {
            new string[]{ "a", "b", "c" },
            new string[]{ "a", "b", "c", "d", "e" },
            new string[]{ "a", "b" }
        };

        var actual = CsvParser.ParseText(text.Trim(), new()
        {
            Normalization = NormalizeType.None
        });

        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [InlineData(@"a, b, c
""a"" ""b"", c, d, e
a, b")]
    [InlineData(@"a, b, c
""a"" b, c, d, e
a, b")]
    public void CsvParseText_Should_Throw_Parsing_Exception(string text)
    {
        Assert.Throws<CsvParserException>(() => CsvParser.ParseText(text));
    }

    [Theory]
    [InlineData("  a,b,c\n d , e , f\n  g  ,  h  ,  i  ")]
    [InlineData("\"a\",\"b\",\"c\"\n \"d\" , \"e\" , \"f\"\n  \"g\"  ,  \"h\"  ,  \"i\"  ")]
    public void CsvParseText_Should_Ignore_SpaceWrapping(string text)
    {
        var expected = new string[][]
        {
            new string[]{ "a", "b", "c" },
            new string[]{ "d", "e", "f" },
            new string[]{ "g", "h", "i" },
        };

        var actual = CsvParser.ParseText(text);

        Assert.Equivalent(expected, actual);
    }
}