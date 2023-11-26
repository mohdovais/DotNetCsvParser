# CSV Parser

A delimited text (file) parser in .Net heavily based on https://en.wikipedia.org/wiki/Comma-separated_values

The parser compliant to technical standard [RFC 4180](https://datatracker.ietf.org/doc/html/rfc4180) formalizes the CSV file format, except leading and trailing spaces and tabs are trimmed (ignored), whereas such trimming is forbidden by RFC 4180, which states "Spaces are considered part of a field and should not be ignored." According to RFC 4180, spaces outside quotes in a field are not allowed; however, the RFC also says that "Spaces are considered part of a field and should not be ignored." and "Implementers should 'be conservative in what you do, be liberal in what you accept from others' (RFC 793, section 2.10) when processing CSV files."

```
1997, Ford, E350
is same as
1997,Ford,E350

1997, "Ford" ,E350
is same as
1997,"Ford",E350
```

```C#
// File
string[][] parsedFile = CsvParser.ParseFile("Sample-Spreadsheet.csv");

// Text
string text = @"g520,Jim Radford,1344,-11.68,65.99,5.26,Nunavut,Telephones and Communication,0.59
LX 788,Jim Radford,1344,313.58,155.99,8.99,Nunavut,Telephones and Communication,0.58
Avery 52,Carlos Soltero,1412,26.92,3.69,0.5,Nunavut,Labels,0.38";

string[][] parsedText = CsvParser.ParseText(text);

```

It supports new line in cell:

```
Year,Make,Model,Description,Price
1997,Ford,E350,"ac, abs, moon",3000.00
1999,Chevy,"Venture ""Extended Edition""",,4900.00
1999,Chevy,"Venture ""Extended Edition, Very Large""","",5000.00
1996,Jeep,Grand Cherokee,"MUST SELL!
air, moon roof, loaded",4799.00
```

The above text will be parsed as:

| Year | Make  | Model                                  | Description                           | Price   |
| ---- | ----- | -------------------------------------- | ------------------------------------- | ------- |
| 1997 | Ford  | E350                                   | ac, abs, moon                         | 3000.00 |
| 1999 | Chevy | Venture "Extended Edition"             |                                       | 4900.00 |
| 1999 | Chevy | Venture "Extended Edition, Very Large" |                                       | 5000.00 |
| 1996 | Jeep  | Grand Cherokee                         | MUST SELL!<br/>air, moon roof, loaded | 4799.00 |

## CsvParserOptions.Separator

Example of an analogous European CSV/DSV file (where the decimal separator is a comma and the value separator is a semicolon):

```C#
string text = @"Year;Make;Model;Length
1997;Ford;E350;2,35
2000;Mercury;Cougar;2,38";

string[][] parsedText = CsvParser.ParseText(text, new CsvParserOptions(){ Separator = ';' });
/*
[
    [ "Year", "Make", "Model", "Length" ],
    [ "1997", "Ford", "E350", "2,35" ],
    [ "2000", "Mercury", "Cougar", "2,38" ]
]
*/

```

## CsvParserOptions.Normalization

There are three types on rows size normalization:

### NormalizeType.MatchMaxRow

`NormalizeType.MatchMaxRow` will make all rows same size if there are descrepencies in number of cells in rows. **This is default behaviour**.

```
a,b,c
d,e,f,g,h,i
k,l
```

```C#
string[][] parsedText = CsvParser.ParseText(text.Trim());

/*
will parse to:

[
    ["a", "b", "c", "", "", ""]
    ["d", "e", "f", "g", "h", "i"],
    ["k", "l", "","", "", ""]
]
*/
```

### NormalizeType.MatchFirstRow

`NormalizeType.MatchFirstRow` will make all rows same size of first rows.

```
a,b,c
d,e,f,g,h,i
k,l
```

```C#
string[][] parsedText = CsvParser.ParseText(text.Trim(),
    new CsvParserOptions()
    {
        Normalization = NormalizeType.MatchFirstRow
    });

/*
will parse to:

[
    ["a", "b", "c"]
    ["d", "e", "f"],
    ["k", "l", ""]
]
*/
```

### NormalizeType.None

`NormalizeType.None` will not modify the result if there is a size descrepency in rows.

```
a,b,c
d,e,f,g,h,i
k,l
```

```C#
string[][] parsedText = CsvParser.ParseText(text.Trim(),
    new CsvParserOptions()
    {
        Normalization = NormalizeType.None
    });

/*
will parse to:

[
    ["a", "b", "c"]
    ["d", "e", "f", "g", "h", "i"],
    ["k", "l"]
]
*/
```
