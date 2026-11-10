using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FontIconCreatorDesigner
{
    public class FontModel
    {
        public const int CHAR_COUNT = 95;
        public const int FIRST_ASCII = 32;  // ' '
        public const int LAST_ASCII = 126;  // '~'

        public string FontName { get; set; } = "font30";
        public string DataArrayName { get; set; } = "SegoeUI30Data";
        public string IncludeFile { get; set; } = "fontStruct.h";
        public string CommentTitle { get; set; } = "Font data for Segoe UI 24pt";
        public string BitmapsCommentName { get; set; }
        public string DescriptorsCommentName { get; set; }
        public string OffsetCommentLine { get; set; }

        public byte Height { get; set; } = 30;
        public byte SpaceChar { get; set; } = 2;
        public byte LowerCaseBottomSpc { get; set; } = 7;

        public byte UpperCaseBottomSpc
        {
            get => LowerCaseBottomSpc;
            set => LowerCaseBottomSpc = value;
        }

        public int[] CharWidths { get; set; } = new int[CHAR_COUNT];
        public Color[,,] Matrix { get; set; } // [95, Height, MaxCols]

        public FontModel(int height = 30, int maxCols = 32, bool allocateMatrix = true)
        {
            Height = (byte)Math.Max(1, height);
            SpaceChar = 2;
            LowerCaseBottomSpc = 7;
            FontName = "Font";
            CharWidths = new int[CHAR_COUNT];
            for (int i = 0; i < CHAR_COUNT; i++) CharWidths[i] = 16;

            if (allocateMatrix)
            {
                int allocRows = Math.Max((int)Height, 32);
                int allocCols = Math.Max(maxCols, 32);
                InitMatrix(allocRows, allocCols);
            }
        }

        public void InitMatrix(int rows, int cols)
        {
            int r = Math.Max(rows, 32);
            int c = Math.Max(cols, 32);
            Matrix = new Color[CHAR_COUNT, r, c];
            for (int i = 0; i < CHAR_COUNT; i++)
            {
                for (int row = 0; row < r; row++)
                {
                    for (int col = 0; col < c; col++)
                    {
                        Matrix[i, row, col] = Color.White;
                    }
                }
            }
        }

        public void EnsureCapacity(int neededRows, int neededCols)
        {
            if (Matrix == null)
            {
                int r = Math.Max(neededRows + 8, 32);
                int c = Math.Max(neededCols + 8, 32);
                InitMatrix(r, c);
                return;
            }

            int curRows = Matrix.GetLength(1);
            int curCols = Matrix.GetLength(2);
            if (neededRows > curRows || neededCols > curCols)
            {
                int newRows = Math.Max(curRows, Math.Max(neededRows + 8, 32));
                int newCols = Math.Max(curCols, Math.Max(neededCols + 8, 32));
                Color[,,] newMat = new Color[CHAR_COUNT, newRows, newCols];
                for (int i = 0; i < CHAR_COUNT; i++)
                {
                    for (int r = 0; r < newRows; r++)
                    {
                        for (int c = 0; c < newCols; c++)
                        {
                            newMat[i, r, c] = (r < curRows && c < curCols)
                                ? Matrix[i, r, c]
                                : Color.White;
                        }
                    }
                }
                Matrix = newMat;
            }
        }

        /// Karakterin kullanıcıya gösterilecek metnini döner (' ' -> "Space", diğerleri -> 'A', '1' vb.)
        public static string GetCharDisplayName(int asciiOrIndex, bool isIndex = true)
        {
            int ascii = isIndex ? (FIRST_ASCII + asciiOrIndex) : asciiOrIndex;
            char ch = (char)ascii;
            return ch == ' ' ? "Space" : ch.ToString();
        }

        /// Karakter ComboBox öğesi için standart metin formatı üretir: [65] 'A' (16 px)
        public static string FormatCharItemText(int charIndex, int width)
        {
            int ascii = FIRST_ASCII + charIndex;
            return $"[{ascii}] '{GetCharDisplayName(charIndex)}' ({width} px)";
        }

        /// Belirtilen karakterdeki siyah piksellerin en üst ve en alt satır indekslerini bulur.
        public bool TryGetCharVerticalBounds(int charIndex, out int top, out int bottom)
        {
            top = -1;
            bottom = -1;
            if (Matrix == null || charIndex < 0 || charIndex >= CHAR_COUNT) return false;
            int w = charIndex < CharWidths.Length ? CharWidths[charIndex] : 0;
            int limitR = Math.Min((int)Height, Matrix.GetLength(1));
            int limitC = Math.Min(w, Matrix.GetLength(2));
            for (int r = 0; r < limitR; r++)
            {
                for (int c = 0; c < limitC; c++)
                {
                    if (Matrix[charIndex, r, c] == Color.Black)
                    {
                        if (top == -1) top = r;
                        bottom = r;
                        break;
                    }
                }
            }
            return top != -1;
        }

        /// Belirtilen karakterin satırlarını beyaza boyayarak temizler.
        public void ClearRows(int charIndex, int startRow, int count)
        {
            if (Matrix == null || charIndex < 0 || charIndex >= CHAR_COUNT || count <= 0) return;
            int endRow = Math.Min(startRow + count, Matrix.GetLength(1));
            int cols = Matrix.GetLength(2);
            for (int r = Math.Max(0, startRow); r < endRow; r++)
                for (int c = 0; c < cols; c++)
                    Matrix[charIndex, r, c] = Color.White;
        }

        /// Belirtilen karakterin satırlarını yukarı veya aşağı öteler.
        public void ShiftRows(int charIndex, int destRow, int srcRow, int count)
        {
            if (Matrix == null || charIndex < 0 || charIndex >= CHAR_COUNT || count <= 0) return;
            int cols = Matrix.GetLength(2);
            int maxR = Matrix.GetLength(1);
            if (destRow < srcRow)
            {
                for (int i = 0; i < count; i++)
                {
                    int d = destRow + i;
                    int s = srcRow + i;
                    if (d >= 0 && d < maxR && s >= 0 && s < maxR)
                        for (int c = 0; c < cols; c++)
                            Matrix[charIndex, d, c] = Matrix[charIndex, s, c];
                }
            }
            else if (destRow > srcRow)
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    int d = destRow + i;
                    int s = srcRow + i;
                    if (d >= 0 && d < maxR && s >= 0 && s < maxR)
                        for (int c = 0; c < cols; c++)
                            Matrix[charIndex, d, c] = Matrix[charIndex, s, c];
                }
            }
        }
    }

    public static class FontHeaderManager
    {
        public static FontModel ParseFontContent(string content, string filePath = "")
        {
            if (!string.IsNullOrEmpty(filePath) && filePath.EndsWith(".df", StringComparison.OrdinalIgnoreCase))
                return ParseDotFactoryFile(content, filePath);
            return ParseHeaderFile(content, filePath);
        }

        public static FontModel ParseFontFile(string filePath)
        {
            string content = File.ReadAllText(filePath);
            return ParseFontContent(content, filePath);
        }

        public static FontModel ParseHeaderFile(string content, string filePath = "")
        {
            if (content.Contains("FONT_INFO") || content.Contains("FONT_CHAR_INFO"))
            {
                return ParseDotFactoryFile(content, filePath);
            }

            FontModel model = new ();

            // 1. Match tFont descriptor struct:
            // Example:
            // const tFont font30 =
            // {
            // 30, 2, 7,
            //    {
            // {3, 0},         /*   */
            // ...
            //    },
            // 	SegoeUI30Data
            // };
            var tFontRegex = new Regex(
                @"const\s+(?:tFont|struct\s+\w+)\s+(?<name>\w+)\s*=\s*\{\s*(?<height>\d+)\s*,\s*(?<space>\d+)\s*,\s*(?<lower>\d+)\s*,\s*\{\s*(?<offsets>.*?)\s*\}\s*,\s*(?<dataName>\w+)\s*\};",
                RegexOptions.Singleline);

            var tMatch = tFontRegex.Match(content);

            int parsedHeight = 30;
            int parsedSpace = 2;
            int parsedLower = 7;
            string fontName = "font30";
            string dataArrayName = "SegoeUI30Data";
            var offsetsList = new List<(int width, int offset)>();

            if (tMatch.Success)
            {
                fontName = tMatch.Groups["name"].Value;
                int.TryParse(tMatch.Groups["height"].Value, out parsedHeight);
                int.TryParse(tMatch.Groups["space"].Value, out parsedSpace);
                int.TryParse(tMatch.Groups["lower"].Value, out parsedLower);
                dataArrayName = tMatch.Groups["dataName"].Value;

                string offsetsBlock = tMatch.Groups["offsets"].Value;
                // Strip comments from offsets block
                offsetsBlock = Regex.Replace(offsetsBlock, @"/\*.*?\*/", "", RegexOptions.Singleline);
                offsetsBlock = Regex.Replace(offsetsBlock, @"//[^\r\n]*", "");

                var pairMatches = Regex.Matches(offsetsBlock, @"\{\s*(\d+)\s*,\s*(\d+)\s*\}");
                foreach (Match pm in pairMatches)
                {
                    int w = int.Parse(pm.Groups[1].Value);
                    int off = int.Parse(pm.Groups[2].Value);
                    offsetsList.Add((w, off));
                }
            }

            // Extract include file if present
            var incMatch = Regex.Match(content, @"#include\s*[<""](?<inc>[^>""]+)[>""]");
            if (incMatch.Success)
            {
                model.IncludeFile = incMatch.Groups["inc"].Value;
            }

            // Extract comment title if present
            var titleMatch = Regex.Match(content, @"/\*\s*\r?\n\s*\*\*\s*(?<title>.*?)\r?\n\s*\*/", RegexOptions.Singleline);
            if (titleMatch.Success)
            {
                model.CommentTitle = titleMatch.Groups["title"].Value.Trim();
            }

            var bmpCommentMatch = Regex.Match(content, @"/\*\s*Character bitmaps for\s+(?<name>[^*]+?)\s*\*/");
            if (bmpCommentMatch.Success)
            {
                model.BitmapsCommentName = bmpCommentMatch.Groups["name"].Value.Trim();
            }

            var descCommentMatch = Regex.Match(content, @"/\*\s*Character descriptors for\s+(?<name>[^*]+?)\s*\*/");
            if (descCommentMatch.Success)
            {
                model.DescriptorsCommentName = descCommentMatch.Groups["name"].Value.Trim();
            }

            var offsetMatch = Regex.Match(content, @"/\*\s*\{\s*\[Char width in bits\],\s*\[Offset into[^*]+?in bytes\]\s*\}\s*\*/");
            if (offsetMatch.Success)
            {
                model.OffsetCommentLine = offsetMatch.Value;
            }

            model.FontName = fontName;
            model.DataArrayName = dataArrayName;
            model.Height = (byte)Math.Max(1, parsedHeight);
            model.SpaceChar = (byte)parsedSpace;
            model.LowerCaseBottomSpc = (byte)parsedLower;

            // 2. Match data array
            // const uint8_t SegoeUI30Data[] = //6304
            // { ... };
            var dataRegex = new Regex(
                @"const\s+uint8_t\s+(?<name>\w+)\s*\[\s*\]\s*=\s*(?://[^\r\n]*)?\s*\{\s*(?<bytes>.*?)\s*\};",
                RegexOptions.Singleline);

            var dMatch = dataRegex.Match(content);
            if (dMatch.Success && !tMatch.Success)
            {
                model.DataArrayName = dMatch.Groups["name"].Value;
            }

            byte[] dataBytes = null;
            if (dMatch.Success)
            {
                string rawBytes = dMatch.Groups["bytes"].Value;
                // Strip block comments
                rawBytes = Regex.Replace(rawBytes, @"/\*.*?\*/", "", RegexOptions.Singleline);
                // Strip line comments
                rawBytes = Regex.Replace(rawBytes, @"//[^\r\n]*", "");

                dataBytes = ExtractBytesFromBlock(rawBytes);
            }

            // If we have offsets and data bytes, populate characters
            if (offsetsList.Count > 0 && dataBytes != null)
            {
                PopulateFontMatrixFromOffsets(model, offsetsList, dataBytes);
            }
            else
            {
                // Fallback: Try parsing from individual character comment blocks:
                // /* @offset 'char' (width pixels wide) */
                PopulateFontMatrixFromOffsets(model, offsetsList, null);
                ParseFromCommentsFallback(content, model);
            }

            return model;
        }

        public static FontModel ParseDotFactoryFile(string content, string filePath = "")
        {
            FontModel model = new ();

            string cleanText = Regex.Replace(content, @"/\*.*?\*/", "", RegexOptions.Singleline);
            cleanText = Regex.Replace(cleanText, @"//[^\r\n]*", "");

            var titleMatch = Regex.Match(content, @"/\*\s*\r?\n\s*\*\*\s*(?<title>.*?)\r?\n\s*\*/", RegexOptions.Singleline);
            if (titleMatch.Success)
            {
                model.CommentTitle = titleMatch.Groups["title"].Value.Trim();
            }
            else
            {
                model.CommentTitle = "Font data";
            }

            var infoMatch = Regex.Match(cleanText,
                @"const\s+FONT_INFO\s+(?<name>\w+)\s*=\s*\{\s*(?<h>\d+)\s*,\s*(?:'(?<start>[^']*)'|(?<startNum>\d+))\s*,\s*(?:'(?<end>[^']*)'|(?<endNum>\d+))\s*,\s*(?<desc>\w+)\s*,\s*(?<bmp>\w+)\s*,?\s*\};",
                RegexOptions.Singleline);

            int parsedHeight = 0;
            string descArrayName = "";
            string bmpArrayName = "";

            if (infoMatch.Success)
            {
                int.TryParse(infoMatch.Groups["h"].Value, out parsedHeight);
                descArrayName = infoMatch.Groups["desc"].Value;
                bmpArrayName = infoMatch.Groups["bmp"].Value;
            }

            var descMatch = Regex.Match(cleanText,
                @"const\s+(?:FONT_CHAR_INFO|tChar|struct\s+\w+)\s+(?<name>\w+)\s*\[\s*\]\s*=\s*\{\s*(?<pairs>.*?)\s*\};",
                RegexOptions.Singleline);

            var offsetsList = new List<(int width, int offset)>();
            if (descMatch.Success)
            {
                var pairs = Regex.Matches(descMatch.Groups["pairs"].Value, @"\{\s*(\d+)\s*,\s*(\d+)\s*\}");
                foreach (Match pm in pairs)
                {
                    offsetsList.Add((int.Parse(pm.Groups[1].Value), int.Parse(pm.Groups[2].Value)));
                }
            }

            if (parsedHeight <= 0 && offsetsList.Count >= 2)
            {
                for (int i = 0; i < offsetsList.Count - 1; i++)
                {
                    if (offsetsList[i].width <= 8 && offsetsList[i].width > 0)
                    {
                        int diff = offsetsList[i + 1].offset - offsetsList[i].offset;
                        if (diff > 0)
                        {
                            parsedHeight = diff;
                            break;
                        }
                    }
                }
            }
            if (parsedHeight <= 0) parsedHeight = 30;

            var bmpMatch = Regex.Match(cleanText,
                @"const\s+(?:uint_8|uint8_t|unsigned\s+char|char)\s+(?<name>\w+)\s*\[\s*\]\s*=\s*\{\s*(?<bytes>.*?)\s*\};",
                RegexOptions.Singleline);

            byte[] dataBytes = null;
            if (bmpMatch.Success)
            {
                string rawArrayName = bmpMatch.Groups["name"].Value;
                if (string.IsNullOrEmpty(bmpArrayName)) bmpArrayName = rawArrayName;

                dataBytes = ExtractBytesFromBlock(bmpMatch.Groups["bytes"].Value);
            }

            model.Height = (byte)Math.Max(1, parsedHeight);
            model.IncludeFile = "fontStruct.h";

            model.FontName = "font" + model.Height;
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                string fName = Path.GetFileNameWithoutExtension(filePath);
                if (!string.Equals(fName, "font", StringComparison.OrdinalIgnoreCase))
                {
                    model.FontName = fName;
                }
            }

            if (!string.IsNullOrEmpty(bmpArrayName))
            {
                if (bmpArrayName.EndsWith("Bitmaps", StringComparison.OrdinalIgnoreCase))
                    model.DataArrayName = bmpArrayName.Substring(0, bmpArrayName.Length - 7) + "Data";
                else if (bmpArrayName.EndsWith("CharBitmaps", StringComparison.OrdinalIgnoreCase))
                    model.DataArrayName = bmpArrayName.Substring(0, bmpArrayName.Length - 11) + "Data";
                else
                    model.DataArrayName = bmpArrayName + "Data";
            }
            else
            {
                model.DataArrayName = model.FontName + "Data";
            }

            if (offsetsList.Count > 0 && offsetsList[0].width > 0)
                model.SpaceChar = (byte)offsetsList[0].width;
            else
                model.SpaceChar = 2;

            PopulateFontMatrixFromOffsets(model, offsetsList, dataBytes);

            model.UpperCaseBottomSpc = CalculateUpperCaseBottomSpc(model);
            return model;
        }

        private static void PopulateFontMatrixFromOffsets(FontModel model, List<(int width, int offset)> offsetsList, byte[] dataBytes)
        {
            int maxCharWidth = 16;
            foreach (var off in offsetsList)
            {
                if (off.width > maxCharWidth) maxCharWidth = off.width;
            }

            int maxRows = Math.Max((int)model.Height, 32);
            int maxCols = Math.Max(maxCharWidth, 32);
            model.InitMatrix(maxRows, maxCols);

            if (offsetsList.Count > 0 && dataBytes != null)
            {
                for (int i = 0; i < Math.Min(FontModel.CHAR_COUNT, offsetsList.Count); i++)
                {
                    int charWidth = offsetsList[i].width;
                    int offset = offsetsList[i].offset;
                    model.CharWidths[i] = charWidth;

                    model.EnsureCapacity(model.Height, charWidth);

                    int bytesPerRow = Form1.CalculateColBytes(charWidth);
                    for (int r = 0; r < model.Height; r++)
                    {
                        int rowStart = offset + (r * bytesPerRow);
                        for (int c = 0; c < charWidth; c++)
                        {
                            int byteIdx = rowStart + (c / 8);
                            if (byteIdx < dataBytes.Length)
                            {
                                int bitIdx = 7 - (c % 8);
                                bool isSet = ((dataBytes[byteIdx] >> bitIdx) & 1) == 1;
                                if (r < model.Matrix.GetLength(1) && c < model.Matrix.GetLength(2))
                                {
                                    model.Matrix[i, r, c] = isSet ? Color.Black : Color.White;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static byte CalculateUpperCaseBottomSpc(FontModel model)
        {
            if (model == null) return 0;

            int aIndex = 'A' - FontModel.FIRST_ASCII;
            if (model.TryGetCharVerticalBounds(aIndex, out _, out int aBottom))
            {
                return (byte)Math.Max(0, (model.Height - 1) - aBottom);
            }

            int maxBottom = -1;
            for (int i = 'A' - FontModel.FIRST_ASCII; i <= 'Z' - FontModel.FIRST_ASCII && i < FontModel.CHAR_COUNT; i++)
            {
                char ch = (char)(FontModel.FIRST_ASCII + i);
                if (ch == 'Q') continue;
                if (model.TryGetCharVerticalBounds(i, out _, out int b))
                {
                    if (b > maxBottom) maxBottom = b;
                }
            }

            if (maxBottom >= 0)
            {
                return (byte)Math.Max(0, (model.Height - 1) - maxBottom);
            }

            return 0;
        }

        private static void ParseFromCommentsFallback(string content, FontModel model)
        {
            var charBlockRegex = new Regex(
                @"/\*\s*@(?<off>\d+)\s*'(?<char>.)'\s*\((?<w>\d+)\s*pixels\s*wide\)\s*\*/(?<rows>.*?)(?=(?:/\*\s*@\d+)|(?:const\s+tFont)|\z)",
                RegexOptions.Singleline);

            var matches = charBlockRegex.Matches(content);
            foreach (Match m in matches)
            {
                char ch = m.Groups["char"].Value[0];
                int ascii = (int)ch;
                int charIdx = ascii - FontModel.FIRST_ASCII;
                if (charIdx < 0 || charIdx >= FontModel.CHAR_COUNT) continue;

                int width = int.Parse(m.Groups["w"].Value);
                model.CharWidths[charIdx] = width;
                model.EnsureCapacity(model.Height, width);

                string rowsStr = m.Groups["rows"].Value;
                using var reader = new StringReader(rowsStr);
                string line;
                int r = 0;
                while ((line = reader.ReadLine()) != null && r < model.Height)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    int commentIdx = line.IndexOf("//");
                    string dataPart = commentIdx >= 0 ? line.Substring(0, commentIdx) : line;
                    var numMatches = Regex.Matches(dataPart, @"\b(0[xX][0-9A-Fa-f]+|0[bB][01]+|\d+)\b");
                    int c = 0;
                    foreach (Match nm in numMatches)
                    {
                        byte val = ParseByte(nm.Value);
                        for (int b = 7; b >= 0 && c < width; b--)
                        {
                            bool isSet = ((val >> b) & 1) == 1;
                            if (r < model.Matrix.GetLength(1) && c < model.Matrix.GetLength(2))
                            {
                                model.Matrix[charIdx, r, c] = isSet ? Color.Black : Color.White;
                            }
                            c++;
                        }
                    }
                    r++;
                }
            }
        }

        private static byte ParseByte(string text)
        {
            int val = IconHeaderManager.ParseByteValue(text);
            return val >= 0 ? (byte)val : (byte)0;
        }

        public static byte[] ExtractBytesFromBlock(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return Array.Empty<byte>();
            var matches = Regex.Matches(text, @"\b(0[xX][0-9A-Fa-f]+|0[bB][01]+|\d+)\b");
            var list = new List<byte>(matches.Count);
            foreach (Match m in matches)
            {
                list.Add(ParseByte(m.Value));
            }
            return list.ToArray();
        }

        /// Font karakterinin tek bir satırını C dizisi baytları ve görsel açıklama yorumu olarak StringBuilder'a ekler.
        public static void AppendCharRow(StringBuilder sb, FontModel model, int charIndex, int r, int w, int bpr, byte format, string indent = "")
        {
            sb.Append(indent);
            for (int b = 0; b < bpr; b++)
            {
                byte val = 0;
                for (int k = 0; k < 8; k++)
                {
                    int c = b * 8 + k;
                    if (c < w && model.Matrix != null && r < model.Matrix.GetLength(1) && c < model.Matrix.GetLength(2) &&
                        model.Matrix[charIndex, r, c] == Color.Black)
                    {
                        val |= (byte)(1 << (7 - k));
                    }
                }
                sb.Append(IconHeaderManager.FormatByte(val, format));
            }

            sb.Append("// ");
            for (int c = 0; c < w; c++)
            {
                bool isBlack = model.Matrix != null && r < model.Matrix.GetLength(1) && c < model.Matrix.GetLength(2) &&
                               model.Matrix[charIndex, r, c] == Color.Black;
                sb.Append(isBlack ? "#" : " ");
            }
            sb.AppendLine();
        }

        public static string GenerateHeaderFile(FontModel model, byte format = 1)
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* ");
            sb.AppendLine($"**  {model.CommentTitle}");
            sb.AppendLine("*/");
            sb.AppendLine();
            string bmpName = !string.IsNullOrWhiteSpace(model.BitmapsCommentName) ? model.BitmapsCommentName : model.FontName;
            sb.AppendLine($"/* Character bitmaps for {bmpName} */");
            sb.AppendLine("#pragma once");
            sb.AppendLine($"#include <{model.IncludeFile}>");
            sb.AppendLine();

            // Compute offsets and total data size
            int[] offsets = new int[FontModel.CHAR_COUNT];
            int currentOffset = 0;
            for (int i = 0; i < FontModel.CHAR_COUNT; i++)
            {
                offsets[i] = currentOffset;
                int w = model.CharWidths[i];
                int bpr = (w + 7) / 8;
                currentOffset += bpr * model.Height;
            }
            int totalBytes = currentOffset;

            sb.AppendLine($"const uint8_t {model.DataArrayName}[] = //{totalBytes}");
            sb.AppendLine("{");

            for (int i = 0; i < FontModel.CHAR_COUNT; i++)
            {
                char ch = (char)(FontModel.FIRST_ASCII + i);
                int w = model.CharWidths[i];
                int bpr = (w + 7) / 8;
                int off = offsets[i];

                string charDisplay = ch == '\\' ? "\\\\" : ch == '\'' ? "\\'" : ch.ToString();
                sb.AppendLine($"\t/* @{off} '{charDisplay}' ({w} pixels wide) */");

                for (int r = 0; r < model.Height; r++)
                {
                    AppendCharRow(sb, model, i, r, w, bpr, format, "\t");
                }
                sb.AppendLine();
            }

            sb.AppendLine("};");
            sb.AppendLine();

            // Descriptor struct
            string descName = !string.IsNullOrWhiteSpace(model.DescriptorsCommentName) ? model.DescriptorsCommentName : model.FontName;
            sb.AppendLine($"/* Character descriptors for {descName} */");
            if (!string.IsNullOrWhiteSpace(model.OffsetCommentLine))
                sb.AppendLine(model.OffsetCommentLine);
            else
                sb.AppendLine("/* { [Char width in bits], [Offset into segoeUI_24ptCharBitmaps in bytes] } */");
            sb.AppendLine($"const tFont {model.FontName} =");
            sb.AppendLine("{");
            sb.AppendLine($"{model.Height}, {model.SpaceChar}, {model.LowerCaseBottomSpc},");
            sb.AppendLine("   {");

            for (int i = 0; i < FontModel.CHAR_COUNT; i++)
            {
                char ch = (char)(FontModel.FIRST_ASCII + i);
                string charDisplay = ch == '\\' ? "\\\\" : ch.ToString();
                sb.AppendLine($"{{{model.CharWidths[i]}, {offsets[i]}}},\t\t/* {charDisplay} */");
            }

            sb.AppendLine("   },");
            sb.AppendLine($"\t{model.DataArrayName}");
            sb.AppendLine("};");

            return sb.ToString();
        }

        public static string GenerateCharString(FontModel model, int charIndex, byte format = 1)
        {
            if (charIndex < 0 || charIndex >= FontModel.CHAR_COUNT) return "";
            var sb = new StringBuilder();
            char ch = (char)(FontModel.FIRST_ASCII + charIndex);
            int w = model.CharWidths[charIndex];
            int bpr = (w + 7) / 8;

            string charDisplay = FontModel.GetCharDisplayName(charIndex);
            sb.AppendLine($"/* Character: '{charDisplay}' (ASCII: {FontModel.FIRST_ASCII + charIndex}) | Width: {w} px | Height: {model.Height} px */");

            for (int r = 0; r < model.Height; r++)
            {
                AppendCharRow(sb, model, charIndex, r, w, bpr, format, "");
            }

            return sb.ToString();
        }
    }
}
