using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace font_creator
{
    /// <summary>
    /// Resim dosyasını .ic formatına dönüştürmeden önce kullanıcıya boyut, eşik ve negatiflik önizlemesi sunan iletişim kutusu.
    /// TheDotFactory tarzında boş kenar kırpma (tightest crop) ve sınırsız/ayarlanabilir boyutlandırma içerir.
    /// </summary>
    public class ImageImportDialog : Form
    {
        private readonly string _filePath;
        private readonly Bitmap _sourceBitmap;
        private bool _isUpdatingUi = false;

        private PictureBox pbOriginal;
        private PictureBox pbPreview;
        private NumericUpDown numWidth;
        private NumericUpDown numHeight;
        private CheckBox cbAspectRatio;
        private CheckBox cbCropBorders;
        private TrackBar tbThreshold;
        private Label lblThresholdVal;
        private CheckBox cbInvert;
        private CheckBox cbTransparentWhite;
        private Label lblInfo;
        private Label lblStatus;
        private Button btnOk;
        private Button btnCancel;
        private Button btnResetOriginal;

        public IconModel ResultIconModel { get; private set; }

        public ImageImportDialog(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var img = Image.FromStream(fs))
            {
                _sourceBitmap = new Bitmap(img);
            }

            InitializeComponent();
            ThemeEngine.Register(this);
            CalculateInitialDimensions();
            UpdatePreview();
        }

        private void InitializeComponent()
        {
            this.Text = "Import Image to Icon (.ic) - Preview & Settings";
            this.Size = new Size(740, 590);
            this.MinimumSize = new Size(700, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.BackColor = Color.WhiteSmoke;

            // Top Info Label
            lblInfo = new Label
            {
                Location = new Point(16, 12),
                Size = new Size(690, 24),
                Text = $"File: {Path.GetFileName(_filePath)}  |  Original: {_sourceBitmap.Width} x {_sourceBitmap.Height} px",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.DarkSlateGray
            };
            this.Controls.Add(lblInfo);

            // Preview Group: Original
            var grpOriginal = new GroupBox
            {
                Text = "Original Image",
                Location = new Point(16, 40),
                Size = new Size(340, 240)
            };
            pbOriginal = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = _sourceBitmap,
                BackColor = Color.FromArgb(240, 240, 240)
            };
            grpOriginal.Controls.Add(pbOriginal);
            this.Controls.Add(grpOriginal);

            // Preview Group: Converted
            var grpPreview = new GroupBox
            {
                Text = "1-Bit Monochrome Preview (Output)",
                Location = new Point(366, 40),
                Size = new Size(340, 240)
            };
            pbPreview = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.CenterImage,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            grpPreview.Controls.Add(pbPreview);
            this.Controls.Add(grpPreview);

            // Settings Group
            var grpSettings = new GroupBox
            {
                Text = "Conversion Settings",
                Location = new Point(16, 285),
                Size = new Size(690, 195)
            };

            int maxDim = Math.Max(512, Math.Max(_sourceBitmap.Width, _sourceBitmap.Height));

            // Width
            var lblW = new Label { Text = "Width (px):", Location = new Point(16, 26), AutoSize = true };
            numWidth = new NumericUpDown
            {
                Location = new Point(95, 23),
                Size = new Size(75, 23),
                Minimum = 1,
                Maximum = maxDim,
                Value = Math.Max(1, Math.Min(maxDim, _sourceBitmap.Width))
            };
            numWidth.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                if (cbAspectRatio.Checked && _sourceBitmap.Width > 0)
                {
                    _isUpdatingUi = true;
                    int newH = Math.Max(1, Math.Min(maxDim, (int)Math.Round((double)numWidth.Value * _sourceBitmap.Height / _sourceBitmap.Width)));
                    numHeight.Value = newH;
                    _isUpdatingUi = false;
                }
                UpdatePreview();
            };

            // Height
            var lblH = new Label { Text = "Height (px):", Location = new Point(185, 26), AutoSize = true };
            numHeight = new NumericUpDown
            {
                Location = new Point(265, 23),
                Size = new Size(75, 23),
                Minimum = 1,
                Maximum = maxDim,
                Value = Math.Max(1, Math.Min(maxDim, _sourceBitmap.Height))
            };
            numHeight.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                if (cbAspectRatio.Checked && _sourceBitmap.Height > 0)
                {
                    _isUpdatingUi = true;
                    int newW = Math.Max(1, Math.Min(maxDim, (int)Math.Round((double)numHeight.Value * _sourceBitmap.Width / _sourceBitmap.Height)));
                    numWidth.Value = newW;
                    _isUpdatingUi = false;
                }
                UpdatePreview();
            };

            // Aspect Ratio Checkbox
            cbAspectRatio = new CheckBox
            {
                Text = "Keep Aspect Ratio",
                Location = new Point(360, 25),
                AutoSize = true,
                Checked = true
            };

            // Reset to Original Size Button
            btnResetOriginal = new Button
            {
                Text = "↺ Original Size",
                Location = new Point(510, 22),
                Size = new Size(110, 26),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnResetOriginal.Click += (s, e) =>
            {
                _isUpdatingUi = true;
                numWidth.Value = Math.Max(1, Math.Min(maxDim, _sourceBitmap.Width));
                numHeight.Value = Math.Max(1, Math.Min(maxDim, _sourceBitmap.Height));
                _isUpdatingUi = false;
                UpdatePreview();
            };

            // Auto-Crop Empty Borders (TheDotFactory style)
            cbCropBorders = new CheckBox
            {
                Text = "Auto-Crop Empty Borders (TheDotFactory Tightest Crop)",
                Location = new Point(16, 58),
                AutoSize = true,
                Checked = false
            };
            cbCropBorders.CheckedChanged += (s, e) => UpdatePreview();

            // Threshold Slider
            var lblTh = new Label { Text = "Threshold (B/W):", Location = new Point(16, 92), AutoSize = true };
            tbThreshold = new TrackBar
            {
                Location = new Point(125, 86),
                Size = new Size(240, 45),
                Minimum = 0,
                Maximum = 255,
                Value = 128,
                TickFrequency = 25
            };
            lblThresholdVal = new Label { Text = "128", Location = new Point(375, 92), Size = new Size(40, 20) };
            tbThreshold.Scroll += (s, e) =>
            {
                lblThresholdVal.Text = tbThreshold.Value.ToString();
                UpdatePreview();
            };

            // Invert Checkbox
            cbInvert = new CheckBox
            {
                Text = "Invert Colors (Negative)",
                Location = new Point(16, 145),
                AutoSize = true,
                Checked = false
            };
            cbInvert.CheckedChanged += (s, e) => UpdatePreview();

            // Transparent as White Checkbox
            cbTransparentWhite = new CheckBox
            {
                Text = "Treat Alpha Transparency as Background (White)",
                Location = new Point(200, 145),
                AutoSize = true,
                Checked = true
            };
            cbTransparentWhite.CheckedChanged += (s, e) => UpdatePreview();

            grpSettings.Controls.Add(lblW);
            grpSettings.Controls.Add(numWidth);
            grpSettings.Controls.Add(lblH);
            grpSettings.Controls.Add(numHeight);
            grpSettings.Controls.Add(cbAspectRatio);
            grpSettings.Controls.Add(btnResetOriginal);
            grpSettings.Controls.Add(cbCropBorders);
            grpSettings.Controls.Add(lblTh);
            grpSettings.Controls.Add(tbThreshold);
            grpSettings.Controls.Add(lblThresholdVal);
            grpSettings.Controls.Add(cbInvert);
            grpSettings.Controls.Add(cbTransparentWhite);
            this.Controls.Add(grpSettings);

            // Bottom Status
            lblStatus = new Label
            {
                Location = new Point(16, 495),
                Size = new Size(380, 24),
                Text = "Status: Ready",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.DarkSlateGray
            };
            this.Controls.Add(lblStatus);

            // OK & Cancel Buttons
            btnOk = new Button
            {
                Text = "✔ Convert & Import",
                Location = new Point(440, 490),
                Size = new Size(150, 36),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.OK
            };
            btnOk.Click += (s, e) =>
            {
                BuildResult();
                this.Close();
            };

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(600, 490),
                Size = new Size(100, 36),
                FlatStyle = FlatStyle.Standard,
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        private void CalculateInitialDimensions()
        {
            _isUpdatingUi = true;
            numWidth.Value = Math.Max(1, _sourceBitmap.Width);
            numHeight.Value = Math.Max(1, _sourceBitmap.Height);
            _isUpdatingUi = false;
        }

        private void UpdatePreview()
        {
            try
            {
                int targetW = (int)numWidth.Value;
                int targetH = (int)numHeight.Value;
                int threshold = tbThreshold.Value;
                bool invert = cbInvert.Checked;
                bool transWhite = cbTransparentWhite.Checked;
                bool cropBorders = cbCropBorders.Checked;
                string iconName = Path.GetFileNameWithoutExtension(_filePath);

                var model = ImageToIconConverter.ConvertImage(
                    _sourceBitmap,
                    iconName,
                    targetW,
                    targetH,
                    threshold,
                    invert,
                    transWhite,
                    cropBorders);

                // Önizleme kutusuna sığacak şekilde dinamik ölçekle
                int maxDim = Math.Max(model.Width, model.Height);
                int scale = Math.Max(1, Math.Min(6, 200 / Math.Max(1, maxDim)));
                var oldImg = pbPreview.Image;
                pbPreview.Image = ImageToIconConverter.CreatePreviewBitmap(model, scale);
                oldImg?.Dispose();

                int bytesPerRow = (model.Width + 7) / 8;
                lblStatus.Text = $"Dimensions: {model.Width} x {model.Height} px  |  Bytes/Row: {bytesPerRow}  |  Total Bytes: {bytesPerRow * model.Height}";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Preview error: " + ex.Message;
            }
        }

        private void BuildResult()
        {
            int targetW = (int)numWidth.Value;
            int targetH = (int)numHeight.Value;
            int threshold = tbThreshold.Value;
            bool invert = cbInvert.Checked;
            bool transWhite = cbTransparentWhite.Checked;
            bool cropBorders = cbCropBorders.Checked;
            string iconName = Path.GetFileNameWithoutExtension(_filePath);

            ResultIconModel = ImageToIconConverter.ConvertImage(
                _sourceBitmap,
                iconName,
                targetW,
                targetH,
                threshold,
                invert,
                transWhite,
                cropBorders);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _sourceBitmap?.Dispose();
                pbPreview.Image?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
