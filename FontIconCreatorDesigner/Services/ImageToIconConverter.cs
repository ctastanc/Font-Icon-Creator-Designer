using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace font_creator
{
    /// Resim dosyalarını (bmp, jpeg, png, gif, ico vb.) gömülü grafik ekranlar için
    /// 1-bit bitmap (.ic) formatına TheDotFactory standartlarında dönüştüren servis sınıfı.
    /// 72x72 kısıtlaması kaldırılmış olup, dinamik boyutlandırma destekler.
    public static class ImageToIconConverter
    {
        /// Resmi verilen parametreler doğrultusunda IconModel nesnesine dönüştürür.
        public static IconModel ConvertImage(
            Bitmap sourceImage,
            string iconName = "icon",
            int targetWidth = -1,
            int targetHeight = -1,
            int threshold = 128,
            bool invert = false,
            bool transparentAsWhite = true,
            bool cropEmptyBorders = false)
        {
            if (sourceImage == null) throw new ArgumentNullException(nameof(sourceImage));
            Bitmap workingBitmap = sourceImage;
            bool shouldDisposeWorking = false;
            // 1. İsteğe bağlı boş kenar boşluklarını kırpma (TheDotFactory PaddingRemoval.Tightest)
            if (cropEmptyBorders)
            {
                Rectangle bounds = FindContentBounds(workingBitmap, threshold, invert, transparentAsWhite);
                if (bounds.Width > 0 && bounds.Height > 0 &&
                    (bounds.Width < workingBitmap.Width || bounds.Height < workingBitmap.Height))
                {
                    Bitmap cropped = workingBitmap.Clone(bounds, workingBitmap.PixelFormat);
                    workingBitmap = cropped;
                    shouldDisposeWorking = true;
                }
            }
            int origW = workingBitmap.Width;
            int origH = workingBitmap.Height;
            int finalW = targetWidth > 0 ? targetWidth : origW;
            int finalH = targetHeight > 0 ? targetHeight : origH;
            // Form1 grid kapasitesini resim boyutuna göre gerekirse genişlet
            Form1.EnsureGridCapacity(finalH, finalW);
            string sanitizedName = IconHeaderManager.SanitizeIdentifier(iconName);
            IconModel model = new IconModel(finalH, finalW, sanitizedName);
            // 2. Yeniden boyutlandırma gerekiyorsa yüksek kaliteli ölçekleme yap
            Bitmap processedBmp = workingBitmap;
            bool shouldDisposeProcessed = false;
            if (finalW != origW || finalH != origH)
            {
                processedBmp = new Bitmap(finalW, finalH, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(processedBmp))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.Clear(Color.Transparent);
                    g.DrawImage(workingBitmap, new Rectangle(0, 0, finalW, finalH));
                }
                shouldDisposeProcessed = true;
            }
            // 3. Pikselleri 1-bit siyah/beyaz matrisine aktar
            for (int r = 0; r < finalH; r++)
            {
                for (int c = 0; c < finalW; c++)
                {
                    Color pixel = processedBmp.GetPixel(c, r);
                    bool isBlack;
                    if (pixel.A < 128 && transparentAsWhite)
                    {
                        isBlack = invert;
                    }
                    else
                    {
                        int lum = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                        isBlack = lum < threshold;
                        if (invert) isBlack = !isBlack;
                    }
                    model.Matrix[r, c] = isBlack ? Color.Black : Color.White;
                }
            }
            if (shouldDisposeProcessed) processedBmp.Dispose();
            if (shouldDisposeWorking) workingBitmap.Dispose();
            return model;
        }

        /// Resimdeki dolu (siyah) piksellerin en dar çevreleyen dikdörtgenini (bounding box) bulur.
        private static Rectangle FindContentBounds(Bitmap bmp, int threshold, bool invert, bool transparentAsWhite)
        {
            int minX = bmp.Width, maxX = -1, minY = bmp.Height, maxY = -1;
            for (int r = 0; r < bmp.Height; r++)
            {
                for (int c = 0; c < bmp.Width; c++)
                {
                    Color p = bmp.GetPixel(c, r);
                    bool isBlack;
                    if (p.A < 128 && transparentAsWhite)
                    {
                        isBlack = invert;
                    }
                    else
                    {
                        int lum = (int)(0.299 * p.R + 0.587 * p.G + 0.114 * p.B);
                        isBlack = lum < threshold;
                        if (invert) isBlack = !isBlack;
                    }

                    if (isBlack)
                    {
                        if (c < minX) minX = c;
                        if (c > maxX) maxX = c;
                        if (r < minY) minY = r;
                        if (r > maxY) maxY = r;
                    }
                }
            }
            if (maxX >= minX && maxY >= minY)
                return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
            return new Rectangle(0, 0, bmp.Width, bmp.Height);
        }

        /// Dosya yolundan resmi kilit bırakmadan yükleyip IconModel nesnesine dönüştürür.
        public static IconModel ConvertImageFile(
            string filePath,
            int targetWidth = -1,
            int targetHeight = -1,
            int threshold = 128,
            bool invert = false,
            bool transparentAsWhite = true,
            bool cropEmptyBorders = false)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new FileNotFoundException("Image file not found.", filePath);
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var img = Image.FromStream(fs);
            using var bmp = new Bitmap(img);
            string iconName = Path.GetFileNameWithoutExtension(filePath);
            return ConvertImage(bmp, iconName, targetWidth, targetHeight, threshold, invert, transparentAsWhite, cropEmptyBorders);
        }

        /// IconModel verisini C dizisi (.ic) dosyası olarak belirtilen dizine kaydeder.
        /// Aynı isimli dosya varsa sonuna _1, _2, _3 ekleyerek üzerine yazılmasını engeller.
        public static string SaveIconToFile(IconModel model, string outputDirectory, byte format = 1)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrWhiteSpace(outputDirectory))
                outputDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);
            string baseName = string.IsNullOrWhiteSpace(model.Name) ? "icon" : model.Name;
            string candidateName = baseName;
            string filePath = Path.Combine(outputDirectory, $"{candidateName}.ic");
            int counter = 1;
            while (File.Exists(filePath))
            {
                candidateName = $"{baseName}_{counter}";
                filePath = Path.Combine(outputDirectory, $"{candidateName}.ic");
                counter++;
            }
            if (!string.Equals(candidateName, baseName, StringComparison.OrdinalIgnoreCase)) model.Name = candidateName;
            string content = IconHeaderManager.GenerateIconFile(model.Name, model.Matrix, model.Height, model.Width, format);
            File.WriteAllText(filePath, content);
            return filePath;
        }

        /// IconModel matrisinden büyütülmüş bir önizleme Bitmap'i oluşturur.
        public static Bitmap CreatePreviewBitmap(IconModel model, int scale = 4)
        {
            if (model == null || model.Matrix == null) return null;
            int scaleFactor = Math.Max(1, scale);
            int bmpW = model.Width * scaleFactor;
            int bmpH = model.Height * scaleFactor;
            Bitmap bmp = new Bitmap(bmpW, bmpH);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                using var brushBlack = new SolidBrush(Color.Black);
                for (int r = 0; r < model.Height; r++)
                {
                    for (int c = 0; c < model.Width; c++)
                    {
                        if (model.Matrix[r, c] == Color.Black)
                            g.FillRectangle(brushBlack, c * scaleFactor, r * scaleFactor, scaleFactor, scaleFactor);
                    }
                }
            }
            return bmp;
        }
    }
}
