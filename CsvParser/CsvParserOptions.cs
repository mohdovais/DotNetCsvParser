using System.Text;

namespace CsvParser;

public enum NormalizeType
{
	None,
	MatchFirstRow,
	MatchMaxRow
}

public class CsvParserOptions
{
	public Encoding Encoding { get; set; } = Encoding.UTF8;

	public char Separator { get; set; } = ',';

	public NormalizeType Normalization { get; set; } = NormalizeType.MatchMaxRow;
}

