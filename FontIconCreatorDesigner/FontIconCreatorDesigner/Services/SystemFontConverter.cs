using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace font_creator
{
    /// TheDotFactory algoritmasını kullanarak sistem fontlarını gömülü grafik ekranlar için
    /// 1-bit bitmap (.h) C başlık dosyasına dönüştüren servis sınıfı.
    public static class SystemFontConverter
    {
        /// Seçilen System.Drawing.Font nesnesini TheDotFactory algoritmasıyla 95 ASCII karakteri (32..126)
        /// içeren ve taban çizgisi (baseline) kusursuz hizalanmış FontModel nesnesine dönüştürür.
        public static FontModel ConvertFont(Font font, int spacePixels = -1)
        {
            if (font == null) throw new ArgumentNullException(nameof(font));

            // 1. ADIM (TheDotFactory - getLargestBitmapFromCharInfo):
            // Tüm 95 karakterin çizim alanını ölçerek en büyük genişlik ve yüksekliği belirle
            int largestW = 0;
            int largestH = 0;
            for (int i = 0; i < FontModel.CHAR_COUNT; i++)
            {
                char ch = (char)(FontModel.FIRST_ASCII + i);
                Size sz = TextRenderer.MeasureText(ch.ToString(), font);
                if (sz.Width > largestW) largestW = sz.Width;
                if (sz.Height > largestH) largestH = sz.Height;
            }
            largestW += 16;
            largestH += 16;

            // 2. ADIM (TheDotFactory - convertCharacterToBitmap):
            // Her karakteri yatayda ortalanmış ve SingleBitPerPixelGridFit modunda çiz
            Bitmap[] charBitmaps = new Bitmap[FontModel.CHAR_COUNT];
            int[] minX = new int[FontModel.CHAR_COUNT];
            int[] maxX = new int[FontModel.CHAR_COUNT];
            int[] minY = new int[FontModel.CHAR_COUNT];
            int[] maxY = new int[FontModel.CHAR_COUNT];
            bool[] hasBlack = new bool[FontModel.CHAR_COUNT];

            int tightestTopY = int.MaxValue;
            int tightestBottomY = -1;

            for (int i = 0; i < FontModel.CHAR_COUNT; i++)
            {
                char ch = (char)(FontModel.FIRST_ASCII + i);
                Bitmap bmp = new Bitmap(largestW, largestH);
                using (Graphics gfx = Graphics.FromImage(bmp))
                {
                    gfx.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                    gfx.PixelOffsetMode = PixelOffsetMode.None;
                    gfx.SmoothingMode = SmoothingMode.None;

                    RectangleF rect = new RectangleF(0, 0, largestW, largestH);
                    using (StringFormat sf = new StringFormat())
                    {
                        // TheDotFactory'deki gibi yatayda ortala, dikeyde üstten hizala
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Near;
                        gfx.FillRectangle(Brushes.White, rect);
                        gfx.DrawString(ch.ToString(), font, Brushes.Black, rect, sf);
                    }
                }
                charBitmaps[i] = bmp;

                // Siyah piksellerin sınırlarını tara
                int cMinX = int.MaxValue, cMaxX = -1, cMinY = int.MaxValue, cMaxY = -1;
                for (int r = 0; r < largestH; r++)
                {
                    for (int c = 0; c < largestW; c++)
                    {
                        Color p = bmp.GetPixel(c, r);
                        if (p.R == 0 && p.G == 0 && p.B == 0) // Siyah piksel
                        {
                            if (r < cMinY) cMinY = r;
                            if (r > cMaxY) cMaxY = r;
                            if (c < cMinX) cMinX = c;
                            if (c > cMaxX) cMaxX = c;
                        }
                    }
                }

                if (cMaxX >= 0)
                {
                    hasBlack[i] = true;
                    minX[i] = cMinX;
                    maxX[i] = cMaxX;
                    minY[i] = cMinY;
                    maxY[i] = cMaxY;

                    // Tüm font için en üst ve en alt siyah piksel sınırını güncelle
                    if (cMinY < tightestTopY) tightestTopY = cMinY;
                    if (cMaxY > tightestBottomY) tightestBottomY = cMaxY;
                }
                else
                {
                    hasBlack[i] = false;
                    minX[i] = 0;
                    maxX[i] = -1;
                    minY[i] = 0;
                    maxY[i] = -1;
                }
            }

            // 3. ADIM (TheDotFactory - findTightestCommonBitmapBorder & manipulateBitmap):
            // Fontun gerçek yüksekliği en üst ve en alt ortak sınırların farkıdır
            if (tightestBottomY < 0)
            {
                tightestTopY = 0;
                tightestBottomY = Math.Max(12, font.Height) - 1;
            }

            int finalHeight = tightestBottomY - tightestTopY + 1;
            if (finalHeight > 255) finalHeight = 255;
            if (finalHeight < 1) finalHeight = 1;

            // Boşluk karakteri genişliğini hesapla
            int spaceWidth = spacePixels > 0 ? spacePixels : Math.Max(2, (int)Math.Round(font.SizeInPoints * 0.25f));

            FontModel model = new(finalHeight, largestW, allocateMatrix: true);

            string safeName = BuildFontIdentifier(font, finalHeight);
            model.FontName = safeName;
            model.DataArrayName = safeName + "Data";
            model.BitmapsCommentName = safeName;
            model.DescriptorsCommentName = safeName;
            model.IncludeFile = "fontStruct.h";
            model.CommentTitle = $"Font data for {font.FontFamily.Name} {font.SizeInPoints:0.#}pt {font.Style}";
            model.SpaceChar = (byte)spaceWidth;
            model.CharWidths[0] = spaceWidth;

            // 4. ADIM: Her karakterin piksellerini tam hizalı olarak FontModel matrisine aktar
            for (int i = 0; i < FontModel.CHAR_COUNT; i++)
            {
                Bitmap bmp = charBitmaps[i];

                if (i == 0 || !hasBlack[i])
                {
                    // Boşluk veya boş karakter
                    model.CharWidths[i] = spaceWidth;
                    for (int r = 0; r < finalHeight; r++)
                        for (int c = 0; c < spaceWidth; c++)
                            model.Matrix[i, r, c] = Color.White;
                }
                else
                {
                    // TheDotFactory sıkı kırpma (tightest horizontal crop):
                    int charW = maxX[i] - minX[i] + 1;
                    if (charW < 1) charW = 1;

                    model.CharWidths[i] = charW;

                    // Pikselleri ortak dikey yükseklik [tightestTopY .. tightestBottomY] boyunca aktar
                    for (int r = 0; r < finalHeight; r++)
                    {
                        int srcY = tightestTopY + r;
                        if (srcY >= 0 && srcY < bmp.Height)
                        {
                            for (int c = 0; c < charW; c++)
                            {
                                int srcX = minX[i] + c;
                                if (srcX >= 0 && srcX < bmp.Width)
                                {
                                    Color p = bmp.GetPixel(srcX, srcY);
                                    bool isBlack = (p.R == 0 && p.G == 0 && p.B == 0);
                                    model.Matrix[i, r, c] = isBlack ? Color.Black : Color.White;
                                }
                                else
                                {
                                    model.Matrix[i, r, c] = Color.White;
                                }
                            }
                        }
                    }
                }

                bmp.Dispose();
            }

            // Alt boşluk / taban çizgisi (baseline) mesafesini hesapla
            byte bottomSpc = FontHeaderManager.CalculateUpperCaseBottomSpc(model);
            model.UpperCaseBottomSpc = bottomSpc;
            model.LowerCaseBottomSpc = bottomSpc;

            return model;
        }

        /// FontModel verisini C başlık (.h) dosyası olarak belirtilen dizine kaydeder.
        /// Aynı isimli dosya varsa sonuna _1, _2, _3 ekleyerek üzerine yazılmasını engeller.
        public static string SaveFontToHeader(FontModel model, string outputDirectory, byte format = 1)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrWhiteSpace(outputDirectory))
                outputDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            string baseFontName = string.IsNullOrWhiteSpace(model.FontName) ? "font" : model.FontName;
            string candidateName = baseFontName;
            string filePath = Path.Combine(outputDirectory, $"{candidateName}.h");

            int counter = 1;
            while (File.Exists(filePath))
            {
                candidateName = $"{baseFontName}_{counter}";
                filePath = Path.Combine(outputDirectory, $"{candidateName}.h");
                counter++;
            }

            if (!string.Equals(candidateName, baseFontName, StringComparison.OrdinalIgnoreCase))
            {
                model.FontName = candidateName;
                model.DataArrayName = candidateName + "Data";
                model.BitmapsCommentName = candidateName;
                model.DescriptorsCommentName = candidateName;
            }

            string headerContent = FontHeaderManager.GenerateHeaderFile(model, format);
            File.WriteAllText(filePath, headerContent);
            return filePath;
        }

        /// Font nesnesi için C dili uyumlu bir değişken/dosya adı üretir (örn: segoeUI_24pt).
        public static string BuildFontIdentifier(Font font, int height)
        {
            string family = IconHeaderManager.SanitizeIdentifier(font.FontFamily.Name.ToLowerInvariant());
            int pt = (int)Math.Round(font.SizeInPoints);
            string styleSuffix = "";
            if (font.Bold && font.Italic) styleSuffix = "_bi";
            else if (font.Bold) styleSuffix = "_b";
            else if (font.Italic) styleSuffix = "_i";

            string name = $"{family}_{pt}pt{styleSuffix}";
            return IconHeaderManager.SanitizeIdentifier(name);
        }
    }
}
