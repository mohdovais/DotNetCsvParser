namespace CsvParser;

public class CsvParserException : Exception
{
    public CsvParserException()
    {

    }

    public CsvParserException(string message) : base(message)
    {

    }

    public CsvParserException(string message, Exception inner) : base(message, inner)
    {

    }
}