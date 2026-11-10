using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace FontIconCreatorDesigner
{
    /// <summary>
    /// Icon & Font Designer Ana Form Sınıfı.
    /// Modüler yapı:
    /// - Form1.cs: Ana yaşam döngüsü, alanlar ve araç çubuğu olayları.
    /// - Form1.Layout.cs: Duyarlı yerleşim (responsive layout), kaydırma ve cetveller.
    /// - Form1.ModeNavigation.cs: Font & İkon modları, karakter paleti ve boyut kontrolleri.
    /// - Form1.GridEditor.cs: Piksel çizimi, hücre boyama, Seç & Taşı aracı ve üst boşluk silme.
    /// - Form1.FileOperations.cs: Dosya Aç/Kaydet, anlık görüntüler (snapshot) ve değişiklik takibi.
    /// </summary>
    public partial class Form1 : Form
    {
        #region Application Directories & Configuration
        private static string GetAppDir()
        {
            try
            {
                string loc = System.Reflection.Assembly.GetExecutingAssembly().Location;
                if (!string.IsNullOrEmpty(loc)) return Path.GetDirectoryName(loc);
            }
            catch { }
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        private readonly IniFile _ini = new(Path.Combine(GetAppDir(), "settings.ini"));
        #endregion

        #region Core State & Dimensions
        public static int TOTAL_ROWS = 72;
        public static int TOTAL_COLS = 72;
        public static void EnsureGridCapacity(int neededRows, int neededCols)
        {
            if (neededRows > TOTAL_ROWS) TOTAL_ROWS = Math.Max(neededRows, TOTAL_ROWS);
            if (neededCols > TOTAL_COLS) TOTAL_COLS = Math.Max(neededCols, TOTAL_COLS);
        }
        public static int CalculateColBytes(int width) => Math.Max(1, (width + 7) / 8);
        public void ConfigureGridDimensions(int rows, int cols, bool updateRulers = true)
        {
            tlpGrid.SuspendLayout();
            tlpGrid.ColumnStyles.Clear();
            tlpGrid.RowStyles.Clear();
            for (int c = 0; c < cols; c++)
                tlpGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, cellSize));
            for (int r = 0; r < rows; r++)
                tlpGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, cellSize));
            tlpGrid.RowCount = rows;
            tlpGrid.ColumnCount = cols;
            tlpGrid.Width = cols * (cellSize + 1);
            tlpGrid.Height = rows * (cellSize + 1);
            tlpGrid.ResumeLayout(true);

            if (updateRulers)
            {
                UpdateRulerDimensions();
                SyncRulersWithScroll();
                tlpGrid.Invalidate();
            }
        }
        public void UpdateEditorView(int row = 0, bool refreshGrid = true)
        {
            if (refreshGrid) tlpGrid.Refresh();
            if (isFontMode) UpdateCodePreview();
            else
            {
                rtxDataMonitor.Text = PrepareString();
                iconText = rtxDataMonitor.Text;
            }
            PrepareLineStr(row);
        }
        private readonly bool FONT_MODE = true;
        private readonly bool ICON_MODE = false;
        private int cellSize = 15;
        private Color selectedColor = Color.Black;
        private Color[,,] commonMatrix;
        private Color[,] coppiedMatrix;
        private Point oldPoint = new(100, 100);

        private int selectedChar = 0;
        private byte selectedNum = 0;
        private byte format = 1; // 0: Dec, 1: Hex (0x..), 2: Bin (0b..)
        private int colBytes = 0;
        private int rowCount = 0;
        private int lastRow = 0;

        private string currentFontFile = "";
        private string currentIconFile = "";
        private string currentFile
        {
            get => isFontMode ? currentFontFile : currentIconFile;
            set
            {
                if (isFontMode) currentFontFile = value;
                else currentIconFile = value;
            }
        }

        private bool isUpdatingUi = false;
        private readonly ToolTip toolTip = new();
        #endregion

        #region Font Mode State & Snapshots
        private bool isFontMode = true;
        private int fontSelectedChar = 33; // 'A'
        private FontModel fontModel = new(30);
        private readonly Button[] charPaletteButtons = new Button[FontModel.CHAR_COUNT];

        private Color[,,] defaultFontMatrix;
        private int[] defaultCharWidths;
        private byte defaultFontHeight;
        private string defaultFontName = "";
        private byte defaultSpaceChar;
        private byte defaultLowerSpc;

        #endregion

        #region Single Icon Mode State & Snapshots
        private Color[,,] iconMatrix;
        private Color[,] defaultIconMatrix;
        private int defaultIconRowCount = 21;
        private int defaultIconColCount = 16;
        private string defaultIconName = "icon";
        private int iconRowCount = 21;
        private int iconColCount = 16;
        private int iconColBytes = 2;
        private string iconText = "";
        private string iconName = "icon";
        #endregion

        #region Select & Move Tool State
        private bool isSelectMoveMode = false;
        private bool isSelecting = false;
        private bool isMovingSelection = false;
        private Point selectStartCell;
        private Rectangle selectionBox = Rectangle.Empty;
        private Rectangle originalSelectionBox = Rectangle.Empty;
        private Color[,] selectionBuffer;
        private Point moveGrabOffset;
        #endregion

        #region Scroll & Layout Containers
        private bool isPerformingLayout = false;
        #endregion

        #region Constructor & Initialization
        public Form1()
        {
            InitializeComponent();
            
            this.Size = new Size(1250, 1000);
            this.MinimumSize = new Size(1000, 700);

            toolTip.SetToolTip(btnRevertDefault, "Restore the selected character to its default (saved) state");
            toolTip.SetToolTip(btnSelectMove, "Select & Move Mode (Use the mouse to select an area and move it within the character boundaries)");
            toolTip.SetToolTip(btnTrimTop, "Delete the 0x00 space above all characters and shift everything upward.");
            toolTip.SetToolTip(btnSyncUC, "Synchronize font height with uppercase letters and adjust ascenders/descenders.");

            float initialRtxFontSize = 8.25f;
            try
            {
                if (float.TryParse(_ini.Read("AppConfig", "RtxFontSize", "8.25"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float savedRtx) && savedRtx >= 5f && savedRtx <= 36f)
                    initialRtxFontSize = savedRtx;
            }
            catch { }
            rtxDataMonitor.Font = new Font("Consolas", initialRtxFontSize, FontStyle.Regular);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            try
            {
                if (int.TryParse(_ini.Read("AppConfig", "CellSize", "15"), out int savedCs) && savedCs >= 1 && savedCs <= 60)
                    cellSize = savedCs;
            }
            catch { }

            InitializeGrid();
            UpdateStyles();
            coppiedMatrix = new Color[TOTAL_ROWS, TOTAL_COLS];
            SetupScrollableLayout();
            InitilizePaints();
            InitializeCharacterPalette();
            PopulateCharacterComboBox();
            TryLoadDefaultIcon(); // Varsayılan ikon ve font dosyasını yükle
            TryLoadDefaultFont();
            ApplyMode(FONT_MODE); // Başlangıçta Font Modu aktif
        }
        
        protected override void OnShown(EventArgs e)
        {
            this.ActiveControl = null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ThemeEngine.Register(this);
            ThemeEngine.ThemeChanged += OnThemeChanged;
            if (isFontMode && fontModel != null && commonMatrix != null)
            {
                SelectCharacter(fontSelectedChar);
                tlpGrid.Refresh();
                if (string.IsNullOrEmpty(currentFontFile)) rtxDataMonitor.Clear();
            }
            PerformResponsiveLayout();
        }

        private void OnThemeChanged()
        {
            for (int i = 0; i < FontModel.CHAR_COUNT; i++)
                ThemeEngine.StylePaletteButton(charPaletteButtons[i], i == selectedChar);
        }

        private void btnTheme_Click(object sender, EventArgs e) => ThemeEngine.SetTheme(!(ThemeEngine.IsDarkMode));

        private void InitializeGrid()
        {
            numCharWidth.Maximum = 512;
            numFontHeight.Maximum = 512;
            ConfigureGridDimensions(TOTAL_ROWS, TOTAL_COLS, updateRulers: false);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.Cancel) return;
            bool fontChanged = fontUnsavedChanges();
            bool iconChanged = iconUnsavedChanges();

            if (fontChanged || iconChanged)
            {
                string msg;
                if (fontChanged && iconChanged)
                    msg = "There are unsaved changes to the font and icon. Do you want to save them before exiting?";
                else if (fontChanged)
                    msg = "There are unsaved changes to the font. Do you want to save them before exiting?";
                else
                    msg = "There are unsaved changes to the icon. Do you want to save them before exiting?";
                DialogResult dr = MessageBox.Show(this, msg, "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    if (fontChanged && !SaveFontFile()) { e.Cancel = true; return; }
                    if (iconChanged && !SaveIconFile()) { e.Cancel = true; return; }
                }
                else if (dr == DialogResult.Cancel) { e.Cancel = true; return; }
            }
        }

        #endregion

        #region Common Toolbar Buttons & Mode Controls

        private void rbFillWhite_CheckedChanged(object sender, EventArgs e)
        {
            selectedColor = Color.White;
            selectedNum = 1;
        }

        private void rbFillBlack_CheckedChanged(object sender, EventArgs e)
        {
            selectedColor = Color.Black;
            selectedNum = 0;
        }

        private void SetDataFormat(byte newFormat)
        {
            format = newFormat;
            UpdateEditorView(lastRow, refreshGrid: false);
        }

        private void rbDecimalData_CheckedChanged(object sender, EventArgs e) => SetDataFormat(0);

        private void rbHexdecimalData_CheckedChanged(object sender, EventArgs e) => SetDataFormat(1);

        private void rbBinaryData_CheckedChanged(object sender, EventArgs e) => SetDataFormat(2);

        private void CharEdit_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCharEdit.Checked)
            {
                cbCharEdit.Text = "Edit ON ";
                cbCharEdit.BackColor = Color.DodgerBlue;
                cbCharEdit.ForeColor = Color.White;
                txtFontIconName.Enabled = true;
                //txtFontIconName.BackColor = SystemColors.GradientInactiveCaption;
            }
            else
            {
                cbCharEdit.Text = "Edit OFF";
                cbCharEdit.UseVisualStyleBackColor = true;
                cbCharEdit.ForeColor = Color.Black;
                txtFontIconName.Enabled = false;
                if (isSelectMoveMode) DisableSelectMoveMode();
            }
        }

        private void btnCopyChar_Click(object sender, EventArgs e)
        {
            if (commonMatrix == null) return;
            int rCount = Math.Min(tlpGrid.RowCount, commonMatrix.GetLength(1));
            int cCount = Math.Min(tlpGrid.ColumnCount, commonMatrix.GetLength(2));
            if (coppiedMatrix == null || rCount > coppiedMatrix.GetLength(0) || cCount > coppiedMatrix.GetLength(1))
                coppiedMatrix = new Color[Math.Max(rCount, 72), Math.Max(cCount, 72)];
            for (int a = 0; a < rCount; a++)
                for (int b = 0; b < cCount; b++)
                    coppiedMatrix[a, b] = commonMatrix[selectedChar, a, b];
            btnPasteChar.Enabled = true;
        }

        private void btnPasteChar_Click(object sender, EventArgs e)
        {
            if (commonMatrix == null || coppiedMatrix == null) return;
            int rCount = Math.Min(tlpGrid.RowCount, Math.Min(commonMatrix.GetLength(1), coppiedMatrix.GetLength(0)));
            int cCount = Math.Min(tlpGrid.ColumnCount, Math.Min(commonMatrix.GetLength(2), coppiedMatrix.GetLength(1)));
            for (int a = 0; a < rCount; a++)
                for (int b = 0; b < cCount; b++)
                    commonMatrix[selectedChar, a, b] = coppiedMatrix[a, b];
            UpdateEditorView(lastRow);
        }

        private void btnInverseChar_Click(object sender, EventArgs e)
        {
            if (commonMatrix == null) return;
            int rCount = Math.Min(tlpGrid.RowCount, commonMatrix.GetLength(1));
            int cCount = Math.Min(tlpGrid.ColumnCount, commonMatrix.GetLength(2));
            for (int a = 0; a < rCount; a++)
                for (int b = 0; b < cCount; b++)
                    commonMatrix[selectedChar, a, b] = (commonMatrix[selectedChar, a, b] == Color.Black) ? Color.White : Color.Black;
            UpdateEditorView(lastRow);
        }

        private void btnClipboard_Click(object sender, EventArgs e)
        {
            if (isFontMode)
                System.Windows.Forms.Clipboard.SetText(GetCurrentFontCode());
            else
                System.Windows.Forms.Clipboard.SetText(PrepareString());
        }

        private void btnClearText_Click(object sender, EventArgs e) => rtxDataMonitor.Clear();

        private void btnFillAll_Click(object sender, EventArgs e)
        {
            FillArray(selectedChar);
            UpdateEditorView(0);
        }

        #endregion

        #region Matrix & String Conversion Helpers

        public void FillArray(int CharIndex) => FillMatrix(CharIndex, selectedColor);

        private void FillMatrix(int charIndex, Color color)
        {
            if (commonMatrix == null) return;
            int rCount = commonMatrix.GetLength(1);
            int cCount = commonMatrix.GetLength(2);
            for (int a = 0; a < rCount; a++)
                for (int b = 0; b < cCount; b++)
                    commonMatrix[charIndex, a, b] = color;
        }

        public bool NumberToColor(byte Num) => Num != 0;
        public bool ColorToNumber(Color color) => color == Color.Black;

        private void EnsureIconMatrixCapacity(int neededH, int neededW)
        {
            if (iconMatrix == null)
            {
                int initH = Math.Max(neededH + 8, 32);
                int initW = Math.Max(neededW + 8, 32);
                iconMatrix = new Color[1, initH, initW];
                for (int r = 0; r < initH; r++)
                    for (int c = 0; c < initW; c++)
                        iconMatrix[0, r, c] = Color.White;
                return;
            }

            int curH = iconMatrix.GetLength(1);
            int curW = iconMatrix.GetLength(2);
            if (neededH > curH || neededW > curW)
            {
                int newH = Math.Max(curH, neededH + 16);
                int newW = Math.Max(curW, neededW + 16);
                Color[,,] newMat = new Color[1, newH, newW];
                for (int r = 0; r < newH; r++)
                    for (int c = 0; c < newW; c++)
                        newMat[0, r, c] = (r < curH && c < curW) ? iconMatrix[0, r, c] : Color.White;

                iconMatrix = newMat;
                if (!isFontMode) commonMatrix = iconMatrix;
            }
        }

        private string GetIconIdentifier()
        {
            if (!isUpdatingUi && !isFontMode && txtFontIconName != null && !string.IsNullOrWhiteSpace(txtFontIconName.Text))
            {
                string sanitized = IconHeaderManager.SanitizeIdentifier(txtFontIconName.Text.Trim());
                if (!string.IsNullOrWhiteSpace(sanitized))
                {
                    iconName = sanitized;
                    return sanitized;
                }
            }
            if (!string.IsNullOrWhiteSpace(iconName) && !iconName.Equals("icon", StringComparison.OrdinalIgnoreCase))
                return IconHeaderManager.SanitizeIdentifier(iconName);
            if (!string.IsNullOrWhiteSpace(currentIconFile))
            {
                string raw = Path.GetFileNameWithoutExtension(currentIconFile);
                string sanitized = IconHeaderManager.SanitizeIdentifier(raw);
                if (!string.IsNullOrWhiteSpace(sanitized))
                {
                    iconName = sanitized;
                    return sanitized;
                }
            }
            if (!string.IsNullOrWhiteSpace(iconName)) return IconHeaderManager.SanitizeIdentifier(iconName);
            return "icon";
        }

        private string GenerateIconCode(Color[,,] sourceMatrix, int charIdx, int height, int width, string identifier = null)
        {
            int h = height > 0 ? height : (rowCount > 0 ? rowCount : TOTAL_ROWS);
            int w = width > 0 ? width : (colBytes > 0 ? colBytes * 8 : TOTAL_COLS);
            if (sourceMatrix != null) h = Math.Min(h, sourceMatrix.GetLength(1));
            identifier ??= GetIconIdentifier();

            Color[,] curMat = new Color[h, w];
            for (int r = 0; r < h; r++)
            {
                for (int c = 0; c < w; c++)
                {
                    curMat[r, c] = (sourceMatrix != null && charIdx >= 0 && charIdx < sourceMatrix.GetLength(0) &&
                                    r < sourceMatrix.GetLength(1) && c < sourceMatrix.GetLength(2))
                        ? sourceMatrix[charIdx, r, c]
                        : Color.White;
                }
            }
            return IconHeaderManager.GenerateIconFile(identifier, curMat, h, w, format);
        }

        public string PrepareString()
        {
            int h = !isFontMode ? tlpGrid.RowCount : iconRowCount;
            int w = !isFontMode ? tlpGrid.ColumnCount : iconColCount;
            colBytes = CalculateColBytes(w);
            int charIdx = (isFontMode && commonMatrix != null && selectedChar < commonMatrix.GetLength(0)) ? selectedChar : 0;
            return GenerateIconCode(commonMatrix, charIdx, h, w);
        }

        private string PrepareIconString()
        {
            if (!isFontMode) return PrepareString();
            return GenerateIconCode(iconMatrix, 0, iconRowCount, iconColCount, GetIconIdentifier());
        }

        #endregion
    }
}
