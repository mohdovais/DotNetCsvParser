using System.Text;

namespace CsvParser;

public class CsvParser
{
    private const int Quote = 34;
    //private const int Comma = 44;
    private const int CR = 13;
    private const int LF = 10;
    private const int Space = 32;

    public CsvParser()
    {
    }

    public static string[][] Parse(string file, char separator = ',')
    {
        var options = new FileStreamOptions()
        {
            Access = FileAccess.Read,
            Mode = FileMode.Open
        };
        using StreamReader stream = new(file, options);

        return Parse(stream, separator);

    }

    public static string[][] Parse(StreamReader streamReader, char separator = ',')
    {
        var isEscaped = false;
        var lines = new List<string[]>();
        var line = new List<string>();
        var segmentStringBuilder = new StringBuilder();
        var spaceStringBuilder = new StringBuilder();

        var read = streamReader.Read();
        var peak = streamReader.Peek();

        while (read != -1)
        {
            var current = (char)read;

            if (isEscaped)
            {
                if (read == Quote)
                {
                    if (peak == Quote)
                    {
                        segmentStringBuilder.Append(current);
                        streamReader.Read();
                    }
                    else
                    {
                        isEscaped = false;

                    }
                }
                else
                {
                    segmentStringBuilder.Append(current);
                }
            }
            else
            {
                if (read == Quote)
                {
                    isEscaped = true;
                }
                else if (read == separator)
                {
                    line.Add(segmentStringBuilder.ToString());
                    segmentStringBuilder.Clear();
                }
                else if (read == CR)
                {
                    // ignore
                }
                else if (read == LF)
                {
                    line.Add(segmentStringBuilder.ToString());
                    lines.Add(line.ToArray());
                    segmentStringBuilder.Clear();
                    line.Clear();
                }
                else if (read == Space)
                {
                    if (segmentStringBuilder.Length != 0 && ReadLeadingSpace(streamReader, separator, ref spaceStringBuilder))
                    {
                        segmentStringBuilder.Append(current).Append(spaceStringBuilder);
                    }
                }
                else
                {
                    segmentStringBuilder.Append(current);
                }
            }

            if(peak == -1 && read != LF)
            {
                line.Add(segmentStringBuilder.ToString());
                lines.Add(line.ToArray());
                segmentStringBuilder.Clear();
                line.Clear();
            }

            read = streamReader.Read();
            peak = streamReader.Peek();
        }

        

        segmentStringBuilder.Clear();
        spaceStringBuilder.Clear();

        return lines.ToArray();
    }


    private static bool ReadLeadingSpace(StreamReader stream, char separator, ref StringBuilder result)
    {
        var peek = stream.Peek();
        result.Clear();
        while (peek == Space)
        {
            result.Append(Space);
            stream.Read();
            peek = stream.Peek();
        }

        if (peek == separator || peek == CR || peek == LF || peek == -1)
        {
            result.Clear();
            return false;
        }

        return true;
    }

}
