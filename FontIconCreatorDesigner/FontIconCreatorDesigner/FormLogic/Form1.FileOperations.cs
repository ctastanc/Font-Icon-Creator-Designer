using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FontIconCreatorDesigner
{
    public partial class Form1
    {
        #region Unified File Operations (Open / Save / Save As)
        private async void btnOpen_Click(object sender, EventArgs e)
        {
            string path = Directory.GetCurrentDirectory();
            if (!string.IsNullOrWhiteSpace(currentFile) && File.Exists(currentFile))
                path = Path.GetDirectoryName(currentFile);
            OpenFileDialog1.InitialDirectory = path;
            OpenFileDialog1.Filter = isFontMode
                ? "Font / Image Files (*.h;*.df)|*.h;*.df|C Font Header (*.h;*.df)|*.h;*.df|All Files (*.*)|*.*"
                : "Image Files (*.ic)|*.ic|C Font Header (*.h;*.df)|*.h;*.df|All Files (*.*)|*.*";
            DialogResult result = OpenFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                System.Windows.Forms.Application.DoEvents(); 
                await System.Threading.Tasks.Task.Delay(100);
                string fileName = OpenFileDialog1.FileName;
                if (fileName.EndsWith(".h", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".df", StringComparison.OrdinalIgnoreCase))
                {
                    if (!isFontMode)
                    {
                        rbFontMode.Checked = true;
                        ApplyMode(FONT_MODE);
                    }
                    LoadFontFile(fileName);
                }
                else
                {
                    if (isFontMode) // .ic dosyası seçildiğinde ikon moduna geç ve ikonu yükle
                    {
                        rbIconMode.Checked = true;
                        ApplyMode(ICON_MODE);
                    }
                    LoadIconFile(fileName);
                }
            }
        }

        private void LoadFontFile(string filePath)
        {
            try
            {
                FontModel parsed = FontHeaderManager.ParseFontFile(filePath);
                LoadFontFromModel(parsed, filePath);
                tlpGrid.Refresh();
                GC.Collect(2, GCCollectionMode.Optimized); // Explicitly collect generation 2 to keep memory low after loading a file
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading the font file: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (isFontMode)
            {
                if (SaveFontFile())
                {
                    UpdateCodePreview();
                    UpdateFileLabel();
                    MessageBox.Show("Font header saved successfully:\n" + currentFontFile, "Saved", MessageBoxButtons.OK);
                }
            }
            else
            {
                if (SaveIconFile())
                {
                    UpdateEditorView(0, refreshGrid: false);
                    MessageBox.Show("Icon/Character successfully saved:\n" + currentIconFile, "Saved", MessageBoxButtons.OK);
                }
            }
        }

        private bool WriteFontToFile(string filePath)
        {
            try
            {
                SetCurrentFontFile(filePath);
                UpdateFontNumericLimits();
                string code = FontHeaderManager.GenerateHeaderFile(fontModel, format);
                File.WriteAllText(currentFontFile, code);
                UpdateCodePreview();
                SaveFontDefaultSnapshot();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Font saving error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool WriteIconToFile(string filePath)
        {
            try
            {
                SetCurrentIconFile(filePath);
                string content = PrepareIconString();
                File.WriteAllText(currentIconFile, content);
                UpdateFileLabel();
                SaveIconDefaultSnapshot();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error saving icon: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            string path = Directory.GetCurrentDirectory();
            if (!string.IsNullOrWhiteSpace(currentFile)) path = Path.GetDirectoryName(currentFile);
            SaveFileDialog1.InitialDirectory = path;
            if (isFontMode)
            {
                SaveFileDialog1.Filter = "C Font Header Files (*.h)|*.h|Image Files (*.ic)|*.ic|All Files (*.*)|*.*";
                SaveFileDialog1.FileName = fontModel.FontName + ".h";
            }
            else
            {
                SaveFileDialog1.Filter = "Image Files (*.ic)|*.ic|C Font Header Files (*.h)|*.h|All Files (*.*)|*.*";
                SaveFileDialog1.FileName = GetIconIdentifier() + ".ic";
            }
            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = SaveFileDialog1.FileName;
                if (fileName.EndsWith(".h", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".df", StringComparison.OrdinalIgnoreCase))
                {
                    if (WriteFontToFile(fileName))
                        MessageBox.Show("Font header saved:\n" + currentFontFile, "Saved", MessageBoxButtons.OK);
                }
                else
                {
                    if (WriteIconToFile(fileName))
                    {
                        UpdateEditorView(0, refreshGrid: false);
                        MessageBox.Show("Icon/Character saved:\n" + currentIconFile, "Saved", MessageBoxButtons.OK);
                    }
                }
            }
        }

        private bool SaveFontFile()
        {
            if (string.IsNullOrWhiteSpace(currentFontFile) || !File.Exists(currentFontFile))
            {
                SaveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
                SaveFileDialog1.Filter = "C Font Header Files (*.h)|*.h|All Files (*.*)|*.*";
                SaveFileDialog1.FileName = (fontModel?.FontName ?? "Font") + ".h";
                if (SaveFileDialog1.ShowDialog(this) != DialogResult.OK) return false;
                return WriteFontToFile(SaveFileDialog1.FileName);
            }

            return WriteFontToFile(currentFontFile);
        }

        private bool SaveIconFile()
        {
            if (string.IsNullOrWhiteSpace(currentIconFile) || !File.Exists(currentIconFile))
            {
                SaveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
                SaveFileDialog1.Filter = "Image Files (*.ic)|*.ic|All Files (*.*)|*.*";
                SaveFileDialog1.FileName = GetIconIdentifier() + ".ic";
                if (SaveFileDialog1.ShowDialog(this) != DialogResult.OK) return false;
                return WriteIconToFile(SaveFileDialog1.FileName);
            }

            return WriteIconToFile(currentIconFile);
        }

        private void SetCurrentFontFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) { currentFontFile = ""; }
            else
            {
                try
                {
                    currentFontFile = filePath.EndsWith(".df", StringComparison.OrdinalIgnoreCase)
                        ? Path.ChangeExtension(Path.GetFullPath(filePath), ".h") : Path.GetFullPath(filePath);
                    _ini.Write("AppConfig", "LastFontFile", currentFontFile);
                }
                catch { currentFontFile = ""; }
            }
            UpdateFileLabel();
        }

        private void SetCurrentIconFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) { currentIconFile = ""; }
            else
            {
                try
                {
                    currentIconFile = Path.GetFullPath(filePath);
                    _ini.Write("AppConfig", "LastIconFile", currentIconFile);
                }
                catch { currentIconFile = ""; }
            }
            UpdateFileLabel();
        }

        private void UpdateFontNumericLimits()
        {
            if (fontModel == null) return;
            int maxW = 16;
            if (fontModel.CharWidths != null)
            {
                foreach (int w in fontModel.CharWidths) if (w > maxW) maxW = w;
            }
            numFontHeight?.Maximum = Math.Max(512, fontModel.Height + 64);
            numCharWidth?.Maximum = Math.Max(512, maxW + 64);
        }

        private void UpdateFileLabel()
        {
            string f = isFontMode ? currentFontFile : currentIconFile;
            lblFile.Text = string.IsNullOrEmpty(f) ? "File: " : "File: " + f;
        }
        #endregion

        #region Default File Loading & Snapshots (Revert & Dirty Checks)
        private void setIconVals(string n, int rc, int cc, int cb)
        {
            iconName = n; iconRowCount = rc; iconColCount = cc; iconColBytes = cb;
        }

        private void ApplyIconModelData(IconModel icModel)
        {
            if (icModel == null) return;
            setIconVals(icModel.Name, icModel.Height, icModel.Width, CalculateColBytes(icModel.Width));
            Form1.EnsureGridCapacity(iconRowCount, iconColCount);
            EnsureIconMatrixCapacity(iconRowCount, iconColCount);
            int copyH = Math.Min(iconRowCount, icModel.Height);
            int copyW = Math.Min(iconColCount, icModel.Width);
            for (int r = 0; r < copyH; r++)
                for (int c = 0; c < copyW; c++)
                    iconMatrix[0, r, c] = icModel.Matrix[r, c];
        }

        private void SyncIconControlsFromModel()
        {
            bool wasUpdating = isUpdatingUi;
            isUpdatingUi = true;
            try
            {
                Form1.EnsureGridCapacity(iconRowCount, iconColCount);
                numFontHeight?.Maximum = Math.Max(512, iconRowCount + 64);
                numCharWidth?.Maximum = Math.Max(512, iconColCount + 64);
                numFontHeight?.Value = Math.Max(1, Math.Min(numFontHeight.Maximum, iconRowCount));
                numCharWidth?.Value = Math.Max(1, Math.Min(numCharWidth.Maximum, iconColCount));
                txtFontIconName?.Text = GetIconIdentifier();
            }
            finally
            {
                isUpdatingUi = wasUpdating;
            }
        }

        private void SyncFontControlsFromModel()
        {
            if (fontModel == null) return;
            bool wasUpdating = isUpdatingUi;
            isUpdatingUi = true;
            try
            {
                UpdateFontNumericLimits();
                txtFontIconName.Text = fontModel.FontName;
                numFontHeight.Value = Math.Max(1, Math.Min((int)numFontHeight.Maximum, (int)fontModel.Height));
                numSpaceChar.Value = fontModel.SpaceChar;
                numLowerSpc.Value = fontModel.UpperCaseBottomSpc;
            }
            finally
            {
                isUpdatingUi = wasUpdating;
            }
        }

        private void LoadFontFromModel(FontModel model, string filePath = "", int selectCharIndex = 33)
        {
            if (model == null) return;
            defaultFontMatrix = null;
            fontModel = model;
            commonMatrix = fontModel.Matrix;
            SetCurrentFontFile(filePath);
            SaveFontDefaultSnapshot();

            SyncFontControlsFromModel();
            PopulateCharacterComboBox();
            int idx = (selectCharIndex >= 0 && selectCharIndex < FontModel.CHAR_COUNT) ? selectCharIndex : 33;
            SelectCharacter(idx);
            cbCharEdit.Checked = false;
        }

        private void TryLoadDefaultIcon()
        {
            string lastIcon = _ini.Read("AppConfig", "LastIconFile", "");
            if (!string.IsNullOrWhiteSpace(lastIcon) && File.Exists(lastIcon))
            {
                try
                {
                    string content = File.ReadAllText(lastIcon);
                    SetCurrentIconFile(lastIcon);
                    var icModel = IconHeaderManager.ParseIconFile(content, Path.GetFileNameWithoutExtension(lastIcon));
                    ApplyIconModelData(icModel);
                    iconText = content;
                    SaveIconDefaultSnapshot();
                    return;
                }
                catch { }
            }
            SetCurrentIconFile(""); // Dosya yoksa veya geçersizse: İkonu tamamen BOŞ aç
            setIconVals("icon", 21, 16, 2);
            iconText = "";
            int initIconH = Math.Max(iconRowCount, 32);
            int initIconW = Math.Max(iconColCount, 32);
            iconMatrix = new Color[1, initIconH, initIconW];
            for (int r = 0; r < initIconH; r++)
                for (int c = 0; c < initIconW; c++)
                    iconMatrix[0, r, c] = Color.White;
            SaveIconDefaultSnapshot();
        }

        private void TryLoadDefaultFont()
        {
            string lastFont = _ini.Read("AppConfig", "LastFontFile", "");
            if (!string.IsNullOrWhiteSpace(lastFont) && File.Exists(lastFont))
            {
                try
                {
                    FontModel parsed = FontHeaderManager.ParseFontFile(lastFont);
                    LoadFontFromModel(parsed, lastFont);
                    return;
                }
                catch { }
            }
            SetCurrentFontFile(""); // Dosya yoksa veya açılamazsa: Ekranı tamamen BOŞ aç!
            FontModel defaultModel = new(30)
            {
                FontName = "Font",
                Height = 30,
                SpaceChar = 2,
                LowerCaseBottomSpc = 7
            };
            LoadFontFromModel(defaultModel, "");
        }

        private void SaveFontDefaultSnapshot()
        {
            if (fontModel != null && fontModel.Matrix != null)
            {
                defaultFontMatrix = (Color[,,])fontModel.Matrix.Clone();
                defaultCharWidths = (int[])fontModel.CharWidths.Clone();
                defaultFontHeight = fontModel.Height;
                defaultFontName = fontModel.FontName ?? "";
                defaultSpaceChar = fontModel.SpaceChar;
                defaultLowerSpc = fontModel.LowerCaseBottomSpc;
            }
        }

        private void SaveIconDefaultSnapshot()
        {
            defaultIconRowCount = iconRowCount;
            defaultIconColCount = iconColCount;
            defaultIconName = iconName ?? "icon";
            if (iconMatrix != null)
            {
                int rCount = iconMatrix.GetLength(1);
                int cCount = iconMatrix.GetLength(2);
                defaultIconMatrix = new Color[rCount, cCount];
                for (int r = 0; r < rCount; r++)
                    for (int c = 0; c < cCount; c++) defaultIconMatrix[r, c] = iconMatrix[0, r, c];
            }
        }

        public bool fontUnsavedChanges()
        {
            if (fontModel == null || fontModel.Matrix == null || defaultFontMatrix == null || defaultCharWidths == null) return false;
            if (fontModel.Height != defaultFontHeight) return true;
            if (fontModel.FontName != defaultFontName) return true;
            if (fontModel.SpaceChar != defaultSpaceChar) return true;
            if (fontModel.LowerCaseBottomSpc != defaultLowerSpc) return true;
            for (int i = 0; i < FontModel.CHAR_COUNT; i++)
                if (i < fontModel.CharWidths.Length && i < defaultCharWidths.Length)
                    if (fontModel.CharWidths[i] != defaultCharWidths[i]) return true;
            int chars = Math.Min(fontModel.Matrix.GetLength(0), defaultFontMatrix.GetLength(0));
            int rCount = Math.Min(fontModel.Matrix.GetLength(1), defaultFontMatrix.GetLength(1));
            int cCount = Math.Min(fontModel.Matrix.GetLength(2), defaultFontMatrix.GetLength(2));
            for (int i = 0; i < chars; i++)
                for (int r = 0; r < rCount; r++)
                    for (int c = 0; c < cCount; c++)
                        if (fontModel.Matrix[i, r, c] != defaultFontMatrix[i, r, c]) return true;
            return false;
        }

        public bool iconUnsavedChanges()
        {
            if (iconMatrix == null || defaultIconMatrix == null) return false;
            if (iconRowCount != defaultIconRowCount) return true;
            if (iconColCount != defaultIconColCount) return true;
            if (iconName != defaultIconName) return true;
            if (iconMatrix.GetLength(1) != defaultIconMatrix.GetLength(0) ||
                iconMatrix.GetLength(2) != defaultIconMatrix.GetLength(1)) return true;
            int rCount = Math.Min(iconRowCount, Math.Min(iconMatrix.GetLength(1), defaultIconMatrix.GetLength(0)));
            int cCount = Math.Min(iconColCount, Math.Min(iconMatrix.GetLength(2), defaultIconMatrix.GetLength(1)));
            for (int r = 0; r < rCount; r++)
                for (int c = 0; c < cCount; c++)
                    if (iconMatrix[0, r, c] != defaultIconMatrix[r, c]) return true;
            return false;
        }

        private void btnRevertDefault_Click(object sender, EventArgs e)
        {
            CancelSelection();
            if (isFontMode)
            {
                if (defaultFontMatrix != null && defaultCharWidths != null &&
                    selectedChar >= 0 && selectedChar < FontModel.CHAR_COUNT)
                {
                    int maxR = Math.Min(commonMatrix.GetLength(1), defaultFontMatrix.GetLength(1));
                    int maxC = Math.Min(commonMatrix.GetLength(2), defaultFontMatrix.GetLength(2));
                    for (int r = 0; r < maxR; r++)
                        for (int c = 0; c < maxC; c++) commonMatrix[selectedChar, r, c] = defaultFontMatrix[selectedChar, r, c];
                    int defWidth = defaultCharWidths[selectedChar];
                    fontModel.CharWidths[selectedChar] = defWidth;
                    isUpdatingUi = true;
                    numCharWidth.Value = Math.Max(1, Math.Min(numCharWidth.Maximum, defWidth));
                    isUpdatingUi = false;
                    UpdateCharacterComboItem(selectedChar, defWidth);
                    UpdateGridForSelectedChar();
                    tlpGrid.Refresh();
                    UpdateCodePreview();
                    PrepareLineStr(0);
                }
            }
            else
            {
                if (defaultIconMatrix != null && iconMatrix != null)
                {
                    int maxR = Math.Min(iconMatrix.GetLength(1), defaultIconMatrix.GetLength(0));
                    int maxC = Math.Min(iconMatrix.GetLength(2), defaultIconMatrix.GetLength(1));
                    for (int r = 0; r < maxR; r++)
                        for (int c = 0; c < maxC; c++) iconMatrix[0, r, c] = defaultIconMatrix[r, c];
                    tlpGrid.Refresh();
                    rtxDataMonitor.Text = PrepareString();
                    iconText = rtxDataMonitor.Text;
                    PrepareLineStr(0);
                }
            }
        }
        #endregion

        #region System Font & Image Import Operations
        private Font GetLastSystemFont()
        {
            try
            {
                string family = _ini.Read("AppConfig", "LastSysFontFamily", "Segoe UI");
                string sizeStr = _ini.Read("AppConfig", "LastSysFontSize", "24");
                string styleStr = _ini.Read("AppConfig", "LastSysFontStyle", "Regular");
                if (!float.TryParse(sizeStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float size) || size < 4 || size > 140)
                    size = 24f;
                if (!Enum.TryParse(styleStr, out FontStyle style)) style = FontStyle.Regular;
                return new Font(family, size, style);
            }
            catch { return new Font("Segoe UI", 24f, FontStyle.Regular); }
        }

        private void SaveLastSystemFont(Font font)
        {
            if (font == null) return;
            try
            {
                _ini.Write("AppConfig", "LastSysFontFamily", font.FontFamily.Name);
                _ini.Write("AppConfig", "LastSysFontSize", font.Size.ToString(System.Globalization.CultureInfo.InvariantCulture));
                _ini.Write("AppConfig", "LastSysFontStyle", font.Style.ToString());
            }
            catch {  }
        }

        private void btnOpenSysFont_Click(object sender, EventArgs e)
        {
            using var fontDialog = new FontDialog();
            fontDialog.ShowEffects = false;
            fontDialog.FontMustExist = true;
            fontDialog.AllowVerticalFonts = false;
            fontDialog.MinSize = 6;
            fontDialog.MaxSize = 140;
            Font initialFont = GetLastSystemFont();
            fontDialog.Font = initialFont;
            if (fontDialog.ShowDialog(this) != DialogResult.OK)
            {
                initialFont?.Dispose(); 
                return;
            }

            try
            {
                Font selectedFont = fontDialog.Font;
                SaveLastSystemFont(selectedFont);
                FontModel model = SystemFontConverter.ConvertFont(selectedFont); // 1. Sistem fontunu 95 karakterli FontModel nesnesine dönüştür
                string debugDir = GetAppDir(); // 2. bin/Debug dizinine .h formatında kaydet
                string savedPath = SystemFontConverter.SaveFontToHeader(model, debugDir, format);
                if (!isFontMode) // 3. Sağdaki rbFontMode aktif değilse aktif et
                {
                    rbFontMode.Checked = true;
                    ApplyMode(FONT_MODE);
                }
                LoadFontFile(savedPath); // 4. Hazırlanan dosyayı yükle ve çalışmaya hazır hale getir
                UpdateCodePreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "An error occurred while converting the system font:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenImage_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Title = "Select Image File to Convert into Icon (.ic)";
            ofd.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff;*.ico)|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff;*.ico|All Files (*.*)|*.*";
            string lastDir = Directory.GetCurrentDirectory();
            if (!string.IsNullOrWhiteSpace(currentIconFile) && File.Exists(currentIconFile))
                lastDir = Path.GetDirectoryName(currentIconFile);
            ofd.InitialDirectory = lastDir;
            if (ofd.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                using var importDlg = new ImageImportDialog(ofd.FileName); // Önizleme ve ayar iletişim kutusunu aç
                if (importDlg.ShowDialog(this) != DialogResult.OK) return;
                IconModel iconModel = importDlg.ResultIconModel;
                if (iconModel == null) return;
                string debugDir = GetAppDir(); // 1. bin/Debug dizinine .ic formatında kaydet
                string savedPath = ImageToIconConverter.SaveIconToFile(iconModel, debugDir, format);
                if (isFontMode)  // 2. Sağdaki rbIconMode aktif değilse aktif et
                {
                    rbIconMode.Checked = true;
                    ApplyMode(ICON_MODE);
                }
                LoadIconFromModel(iconModel, savedPath); // 3. Hazırlanan ikonu yükle ve çalışmaya hazır hale getir
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "An error occurred while converting the image:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadIconFromModel(IconModel iconModel, string savedPath)
        {
            if (iconModel == null) return;
            defaultIconMatrix = null;
            SetCurrentIconFile(savedPath);
            ApplyIconModelData(iconModel);
            SyncIconControlsFromModel();

            commonMatrix = iconMatrix;
            selectedChar = 0;
            lastRow = 0;
            rowCount = iconRowCount;
            colBytes = iconColBytes;
            UpdateGridForIcon();
            SaveIconDefaultSnapshot();
            cbCharEdit.Checked = false;
        }

        private void LoadIconFile(string filePath)
        {
            try
            {
                string content = File.ReadAllText(filePath);
                string defaultName = Path.GetFileNameWithoutExtension(filePath);
                var icModel = IconHeaderManager.ParseIconFile(content, defaultName);
                LoadIconFromModel(icModel, filePath);
                rtxDataMonitor.Text = content;
                iconText = content;
                PrepareLineStr(0);
                UpdateRulerDimensions();
                tlpGrid.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading the icon file:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
