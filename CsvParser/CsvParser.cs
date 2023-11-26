using System.Text;

namespace CsvParser;

public class CsvParser
{
    private const int Quote = 34;
    private const int CR = 13;
    private const int LF = 10;
    private const int Space = 32;

    public static string[][] ParseText(string text, CsvParserOptions? options = null)
    {
        options ??= new CsvParserOptions();

        using var streamReader = new StringReader(text);
        return ParseStreamReader(streamReader, options);
    }

    public static string[][] ParseFile(string file, CsvParserOptions? options = null)
    {
        options ??= new CsvParserOptions();

        var streamOptions = new FileStreamOptions()
        {
            Access = FileAccess.Read,
            Mode = FileMode.Open,
        };
        using StreamReader streamReader = new(file, options.Encoding, false, streamOptions);

        return ParseStreamReader(streamReader, options);
    }

    private static string[][] ParseStreamReader(TextReader streamReader, CsvParserOptions options)
    {

        var isEscaped = false;
        var isExpectingSeparator = false;
        var result = new List<string[]>();
        var currentRow = new List<string>();
        var currentCell = new StringBuilder();
        var spaceBuilder = new StringBuilder();

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
                        _ = currentCell.Append(current);
                        _ = streamReader.Read();
                    }
                    else
                    {
                        isEscaped = false;
                        isExpectingSeparator = true;

                    }
                }
                else
                {
                    _ = currentCell.Append(current);
                }
            }
            else
            {
                if (read == Quote)
                {
                    if (isExpectingSeparator)
                    {
                        throw new CsvParserException(
                            GetExceptionMessage(streamReader, current, options.Separator)
                        );
                    }

                    isEscaped = true;
                }
                else if (read == options.Separator)
                {
                    isExpectingSeparator = false;
                    currentRow.Add(currentCell.ToString());
                    _ = currentCell.Clear();
                }
                else if (read == CR)
                {
                    // ignore
                }
                else if (read == LF)
                {
                    isExpectingSeparator = false;
                    currentRow.Add(currentCell.ToString());
                    result.Add(currentRow.ToArray());
                    _ = currentCell.Clear();
                    currentRow.Clear();
                }
                else if (read == Space)
                {
                    if (currentCell.Length != 0 && ReadLeadingSpace(streamReader, options.Separator, ref spaceBuilder))
                    {
                        _ = currentCell.Append(current).Append(spaceBuilder);
                    }

                    peak = streamReader.Peek();
                }
                else
                {
                    if (isExpectingSeparator)
                    {
                        throw new CsvParserException(
                            GetExceptionMessage(streamReader, current, options.Separator)
                        );
                    }

                    _ = currentCell.Append(current);
                }
            }

            if (peak == -1 && read != LF)
            {
                currentRow.Add(currentCell.ToString());
                result.Add(currentRow.ToArray());
                _ = currentCell.Clear();
                currentRow.Clear();
            }

            read = streamReader.Read();
            peak = streamReader.Peek();
        }

        _ = currentCell.Clear();
        _ = spaceBuilder.Clear();

        return Normalize(result, options.Normalization);
    }

    private static bool ReadLeadingSpace(TextReader streamReader, char separator, ref StringBuilder result)
    {
        var peek = streamReader.Peek();
        _ = result.Clear();
        while (peek == Space)
        {
            _ = result.Append(Space);
            _ = streamReader.Read();
            peek = streamReader.Peek();
        }

        if (peek == separator || peek == CR || peek == LF || peek == -1)
        {
            _ = result.Clear();
            return false;
        }

        return true;
    }

    private static string[][] Normalize(List<string[]> list, NormalizeType type)
    {
        if (type != NormalizeType.None && list.Count > 1)
        {
            var min = int.MaxValue;
            var max = -1;

            list.ForEach(x =>
            {
                min = min < x.Length ? min : x.Length;
                max = max < x.Length ? x.Length : max;
            });

            var requiredLength = type == NormalizeType.MatchFirstRow ? list[0].Length : max;

            if (requiredLength != min || requiredLength != max)
            {
                return list.ConvertAll(row =>
                {
                    if (row.Length != requiredLength)
                    {
                        var oldLength = row.Length;
                        Array.Resize(ref row, requiredLength);
                        if (oldLength < requiredLength)
                        {
                            for (var i = oldLength; i < requiredLength; i++)
                            {
                                row[i] = string.Empty;
                            }
                        }
                    }
                    return row;
                }).ToArray();
            }

        }

        return list.ToArray();
    }

    private static string GetExceptionMessage(TextReader streamReader, char actual, char separator)
    {
        var message = $"Expecting '{separator}' but found '{actual}{streamReader.ReadLine()}'";
        streamReader.Dispose();
        return message;
    }

}
