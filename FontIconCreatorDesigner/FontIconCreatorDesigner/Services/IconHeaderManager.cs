using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FontIconCreatorDesigner
{
    /// Tekil ikon / karakter veri modelini temsil eder.
    public class IconModel
    {
        public string Name { get; set; } = "icon";
        public int Height { get; set; } = 21;
        public int Width { get; set; } = 16;
        public Color[,] Matrix { get; set; }

        public IconModel(int height = 21, int width = 16, string name = "icon")
        {
            Height = Math.Max(1, height);
            Width = Math.Max(1, width);
            Name = IconHeaderManager.SanitizeIdentifier(name);
            Matrix = new Color[Height, Width];
            for (int r = 0; r < Height; r++)
                for (int c = 0; c < Width; c++)
                    Matrix[r, c] = Color.White;
        }
    }

    /// .ic uzantılı tekil ikon/karakter dosyalarını ayrıştıran ve C dizisi olarak üreten yönetici sınıfı.
    /// Format: const uint8_t icon_name[] = { Height, Width, // (height H width W) ... };
    public static class IconHeaderManager
    {
        #region Identifier Sanitization & File Utilities
        /// C dili için geçerli bir değişken/dizi adı üretir.
        public static string SanitizeIdentifier(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "icon";
            var sb = new StringBuilder();
            foreach (char ch in name)
            {
                if (char.IsLetterOrDigit(ch) || ch == '_') sb.Append(ch);
                else sb.Append('_');
            }
            string result = sb.ToString();
            if (result.Length > 0 && char.IsDigit(result[0]))
                result = "_" + result;
            return string.IsNullOrWhiteSpace(result) ? "icon" : result;
        }

        /// Belirtilen dizinde çakışma durumunda numara ekleyerek benzersiz bir dosya yolu ve adı üretir.
        public static (string filePath, string finalName) GetUniqueFilePath(string outputDirectory, string baseName, string extension)
        {
            if (string.IsNullOrWhiteSpace(outputDirectory))
                outputDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            string safeBase = string.IsNullOrWhiteSpace(baseName) ? "file" : baseName;
            string candidateName = safeBase;
            string ext = extension.StartsWith(".") ? extension : "." + extension;
            string filePath = Path.Combine(outputDirectory, $"{candidateName}{ext}");

            int counter = 1;
            while (File.Exists(filePath))
            {
                candidateName = $"{safeBase}_{counter}";
                filePath = Path.Combine(outputDirectory, $"{candidateName}{ext}");
                counter++;
            }

            return (filePath, candidateName);
        }

        /// Bayt değerini seçilen C formatına (0: Dec, 1: Hex, 2: Bin) dönüştürür.
        public static string FormatByte(byte val, byte format, bool trailingCommaSpace = true)
        {
            string suffix = trailingCommaSpace ? ", " : ",";
            return format switch
            {
                0 => $"{val}{suffix}",
                2 => $"0B{Convert.ToString(val, 2).PadLeft(8, '0')}{suffix}",
                _ => $"0x{val:X2}{suffix}"
            };
        }
        #endregion

        #region Byte Parsing & Zero Trimming
        /// Onaltılık (0x..), ikilik (0b..) veya onluk formatındaki bayt metnini sayıya dönüştürür.
        public static int ParseByteValue(string text)
        {
            try
            {
                text = text.Trim();
                if (text.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
                    return Convert.ToInt32(text.Substring(2), 2);
                if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    return Convert.ToInt32(text.Substring(2), 16);
                return Convert.ToInt32(text, 10);
            }
            catch { return -1; }
        }

        private class ByteItem
        {
            public byte Value { get; set; }
            public string RawText { get; set; }
        }

        private class LineInfo
        {
            public List<ByteItem> Items { get; set; }
            public string Comment { get; set; }
        }

        /// Sağ taraftaki tamamen sıfır olan bayt sütunlarını kırpar.
        public static string TrimRightZeroColumns(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            var linesData = new List<LineInfo>();
            int maxCols = 0;
            using (var reader = new StringReader(input))
            {
                string rawLine;
                while ((rawLine = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(rawLine)) continue;
                    int commentIdx = rawLine.IndexOf("//");
                    string dataPart = commentIdx >= 0 ? rawLine.Substring(0, commentIdx) : rawLine;
                    string comment = commentIdx >= 0 ? rawLine.Substring(commentIdx) : "";
                    var parts = dataPart.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var bytes = new List<ByteItem>();
                    foreach (var part in parts)
                    {
                        string clean = part.Trim().Trim('{', '}', ';');
                        if (string.IsNullOrWhiteSpace(clean)) continue;
                        int val = ParseByteValue(clean);
                        if (val >= 0) bytes.Add(new ByteItem { Value = (byte)val, RawText = clean });
                    }
                    if (bytes.Count > 0)
                    {
                        if (bytes.Count > maxCols) maxCols = bytes.Count;
                        linesData.Add(new LineInfo { Items = bytes, Comment = comment });
                    }
                }
            }
            if (linesData.Count == 0 || maxCols == 0) return input;
            int keepColumns = maxCols;
            for (int col = maxCols - 1; col >= 0; col--)
            {
                bool allZero = true;
                foreach (var line in linesData)
                {
                    if (col < line.Items.Count && line.Items[col].Value != 0)
                    {
                        allZero = false;
                        break;
                    }
                }
                if (allZero) keepColumns = col;
                else break;
            }
            var resultLines = new List<string>();
            foreach (var line in linesData)
            {
                var kept = line.Items.Take(keepColumns).Select(x => x.RawText);
                string rowText = string.Join(", ", kept);
                if (!string.IsNullOrEmpty(rowText)) rowText += ",";
                if (!string.IsNullOrWhiteSpace(line.Comment))
                    rowText += "\t" + line.Comment;
                resultLines.Add(rowText);
            }
            return string.Join(Environment.NewLine, resultLines);
        }
        #endregion

        #region File Parsing & Generation
        /// .ic dosya içeriğini ayrıştırıp IconModel nesnesi olarak döndürür.
        public static IconModel ParseIconFile(string inputString, string defaultName = "icon")
        {
            if (string.IsNullOrWhiteSpace(inputString)) return new IconModel(21, 16, defaultName);
            string icon_name = defaultName;
            var identMatch = Regex.Match(inputString, @"(?:const\s+)?uint8_t\s+([A-Za-z_][A-Za-z0-9_]*)\s*\[\s*\]");
            if (identMatch.Success) icon_name = identMatch.Groups[1].Value;
            bool isNewFormat = identMatch.Success || Regex.IsMatch(inputString, @"height\s+\d+\s+width\s+\d+", RegexOptions.IgnoreCase);
            if (!isNewFormat) inputString = TrimRightZeroColumns(inputString);
            int parsedH = -1;
            int parsedW = -1;
            var dataRows = new List<List<byte>>();
            using (var reader = new StringReader(inputString))
            {
                string rawLine;
                while ((rawLine = reader.ReadLine()) != null)
                {
                    string line = rawLine.Trim();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (line.StartsWith("const", StringComparison.OrdinalIgnoreCase) ||
                        line.StartsWith("uint8_t", StringComparison.OrdinalIgnoreCase) ||
                        line.StartsWith("};") || line == "{" || line == "}")
                        continue;
                    int commentIdx = line.IndexOf("//");
                    string comment = commentIdx >= 0 ? line.Substring(commentIdx) : "";
                    string dataPart = commentIdx >= 0 ? line.Substring(0, commentIdx) : line;
                    // Başlık satırı: e.g. 21, 16, // (height 21 width 16)
                    if (parsedH < 0 && parsedW < 0)
                    {
                        var dimMatch = Regex.Match(comment, @"height\s+(\d+)\s+width\s+(\d+)", RegexOptions.IgnoreCase);
                        if (dimMatch.Success)
                        {
                            parsedH = int.Parse(dimMatch.Groups[1].Value);
                            parsedW = int.Parse(dimMatch.Groups[2].Value);
                            continue;
                        }
                        if (identMatch.Success)
                        {
                            var nums = Regex.Matches(dataPart, @"\b\d+\b");
                            if (nums.Count == 2)
                            {
                                parsedH = int.Parse(nums[0].Value);
                                parsedW = int.Parse(nums[1].Value);
                                continue;
                            }
                        }
                    }
                    // Satırdaki bayt değerlerini oku
                    var byteMatches = Regex.Matches(dataPart, @"\b(0[xX][0-9A-Fa-f]+|0[bB][01]+|\d+)\b");
                    if (byteMatches.Count > 0)
                    {
                        var rowBytes = new List<byte>();
                        foreach (Match m in byteMatches)
                        {
                            int val = ParseByteValue(m.Value);
                            if (val >= 0) rowBytes.Add((byte)val);
                        }
                        dataRows.Add(rowBytes);
                    }
                }
            }
            // Geriye dönük uyumluluk (boyut satırı yoksa)
            if (parsedH <= 0) parsedH = dataRows.Count;
            if (parsedW <= 0)
            {
                int maxBytes = 0;
                foreach (var r in dataRows) if (r.Count > maxBytes) maxBytes = r.Count;
                parsedW = maxBytes * 8;
            }
            if (parsedH <= 0) parsedH = 21;
            if (parsedW <= 0) parsedW = 16;
            var model = new IconModel(parsedH, parsedW, icon_name);
            for (int r = 0; r < Math.Min(parsedH, dataRows.Count); r++)
            {
                var rowBytes = dataRows[r];
                int col = 0;
                for (int i = 0; i < rowBytes.Count; i++)
                {
                    byte bVal = rowBytes[i];
                    for (int b = 7; b >= 0; b--)
                    {
                        if (col < parsedW)
                        {
                            bool isSet = ((bVal >> b) & 1) != 0;
                            model.Matrix[r, col] = isSet ? Color.Black : Color.White;
                        }
                        col++;
                    }
                }
            }
            return model;
        }

        /// İkon matrisini C dizisi (.ic dosya formatı) metnine dönüştürür.
        public static string GenerateIconFile(string name, Color[,] matrix, int height, int width, byte format)
        {
            int h = Math.Max(1, height);
            int w = Math.Max(1, width);
            int bytesPerRow = (w + 7) / 8;
            string ident = SanitizeIdentifier(name);
            var sb = new StringBuilder();
            sb.AppendLine($"const uint8_t {ident}[] = {{");
            sb.AppendLine($"\t{h}, {w}, // (height {h} width {w})");
            for (int r = 0; r < h; r++)
            {
                sb.Append("\t");
                var commentSb = new StringBuilder("// ");
                int col = 0;
                for (int i = 0; i < bytesPerRow; i++)
                {
                    byte tempByte = 0;
                    for (int b = 7; b >= 0; b--)
                    {
                        bool isBlack = false;
                        if (matrix != null && r < matrix.GetLength(0) && col < matrix.GetLength(1) && col < w)
                            isBlack = (matrix[r, col] == Color.Black);
                        if (isBlack) tempByte |= (byte)(1 << b);
                        if (col < w) commentSb.Append(isBlack ? '#' : ' ');
                        col++;
                    }
                    sb.Append(FormatByte(tempByte, format));
                }
                sb.Append(commentSb.ToString());
                sb.AppendLine();
            }
            sb.AppendLine("};");
            return sb.ToString();
        }
        #endregion
    }
}
