using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace FontIconCreatorDesigner
{
    public partial class Form1
    {
        #region Character Palette & Selection
        /// ASCII 32 - 126 arasındaki tüm 95 karakter için alt panelde seçim butonlarını oluşturur.
        private void InitializeCharacterPalette()
        {
            pnlCharBox.Controls.Clear();
            pnlCharBox.SuspendLayout();

            int btnW = 28;
            int btnH = 26;
            int spacing = 2;
            int vScrollBarWidth = SystemInformation.VerticalScrollBarWidth;
            int availableWidth = Math.Max(200, pnlCharBox.ClientSize.Width - vScrollBarWidth - 10);
            int colsPerRow = Math.Max(1, availableWidth / (btnW + spacing));

            for (int i = 0; i < FontModel.CHAR_COUNT; i++)
            {
                char ch = (char)(FontModel.FIRST_ASCII + i);
                Button btn = new()
                {
                    UseMnemonic = false,
                    Size = new Size(btnW, btnH),
                    Text = ch.ToString(), 
                    Tag = i,
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                    Margin = new Padding(1),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.White,
                };

                int r = i / colsPerRow;
                int c = i % colsPerRow;
                btn.Location = new Point(c * (btnW + spacing) + 2, r * (btnH + spacing) + 2);
                int charIdx = i;
                btn.Click += (s, e) => SelectCharacter(charIdx);
                string displayChar = FontModel.GetCharDisplayName(i);
                toolTip.SetToolTip(btn, $"ASCII: {FontModel.FIRST_ASCII + i} ('{displayChar}')");
                charPaletteButtons[i] = btn;
                pnlCharBox.Controls.Add(btn);
            }
            pnlCharBox.ResumeLayout(true);
        }

        /// Karakter açılır kutusunu (ComboBox) güncel karakter listesi ve piksel genişlikleriyle doldurur.
        private void PopulateCharacterComboBox()
        {
            isUpdatingUi = true;
            cmbCharacters.Items.Clear();
            for (int i = 0; i < FontModel.CHAR_COUNT; i++)
            {
                cmbCharacters.Items.Add(FontModel.FormatCharItemText(i, fontModel.CharWidths[i]));
            }
            if (selectedChar >= 0 && selectedChar < FontModel.CHAR_COUNT) cmbCharacters.SelectedIndex = selectedChar;
            isUpdatingUi = false;
        }

        /// Belirtilen karakterin ComboBox listesindeki genişlik metnini günceller.
        private void UpdateCharacterComboItem(int charIndex, int width)
        {
            if (charIndex >= 0 && charIndex < cmbCharacters.Items.Count)
            {
                bool wasUpdating = isUpdatingUi;
                isUpdatingUi = true;
                cmbCharacters.Items[charIndex] = FontModel.FormatCharItemText(charIndex, width);
                isUpdatingUi = wasUpdating;
            }
        }

        /// Font modunda belirtilen indeksteki karakteri seçer, gridi ve arayüzü günceller.
        private void SelectCharacter(int index)
        {
            CommitSelection();
            if (index < 0 || index >= FontModel.CHAR_COUNT) return;
            selectedChar = index;
            if (isFontMode) fontSelectedChar = index;
            isUpdatingUi = true;
            if (cmbCharacters.SelectedIndex != selectedChar) cmbCharacters.SelectedIndex = selectedChar;
            int currentWidth = Math.Max(1, fontModel.CharWidths[selectedChar]);
            if (numCharWidth != null && numCharWidth.Maximum < currentWidth)  numCharWidth.Maximum = Math.Max(512, currentWidth + 64);
            fontModel.EnsureCapacity(fontModel.Height, currentWidth);
            commonMatrix = fontModel.Matrix;
            numCharWidth.Value = currentWidth;

            OnThemeChanged();

            pnlCharBox.ScrollControlIntoView(charPaletteButtons[selectedChar]);
            UpdateGridForSelectedChar();
            isUpdatingUi = false;
            UpdateEditorView(0, refreshGrid: false);
        }

        /// Seçili karakterin genişliğine göre grid (TableLayoutPanel) sütun ve satırlarını 1'er piksel hassasiyetle ayarlar.
        private void UpdateGridForSelectedChar()
        {
            if (commonMatrix == null) return;
            rowCount = fontModel.Height;
            int charWidth = Math.Max(1, fontModel.CharWidths[selectedChar]);
            fontModel.EnsureCapacity(rowCount, charWidth);
            commonMatrix = fontModel.Matrix;
            colBytes = CalculateColBytes(charWidth);
            ConfigureGridDimensions(rowCount, charWidth);
        }

        /// İkon boyutlarına (iconRowCount x iconColCount) göre grid sütun ve satırlarını tam olarak ayarlar.
        public void UpdateGridForIcon()
        {
            if (iconRowCount <= 0) iconRowCount = 21;
            if (iconColCount <= 0) iconColCount = 16;
            rowCount = iconRowCount;
            colBytes = CalculateColBytes(iconColCount);
            iconColBytes = colBytes;
            Form1.EnsureGridCapacity(iconRowCount, iconColCount);
            ConfigureGridDimensions(iconRowCount, iconColCount);
        }

        #endregion
        #region Mode Switching (Font Mode vs Icon Mode)
        private void rbMode_CheckedChanged(object sender, EventArgs e)
        {
            if (isUpdatingUi) return;
            ApplyMode(rbFontMode.Checked);
        }

        private void controlsSet(bool set)
        {
            int bottomLimit = (lblFile.Visible) ? (lblFile.Top - 6) : (this.ClientSize.Height - 35);
            lblSpace.Enabled = set;
            numSpaceChar.Enabled = set;
            lblLowerSpc.Enabled = set;
            numLowerSpc.Enabled = set;
            pnlFontControls.Visible = set;
            btnSyncUC?.Enabled = set;
            int targetTop = pnlFontControls.Bottom + 5;
            int targetHeight = Math.Max(80, bottomLimit - targetTop);
            rtxDataMonitor.SetBounds(rtxDataMonitor.Left, targetTop, rtxDataMonitor.Width, targetHeight);
        }

        /// Font Modu ile Tekil İkon Modu arasındaki geçişi yönetir.
        private void ApplyMode(bool mode)
        {
            CommitSelection();
            if (commonMatrix != null && mode == isFontMode) return;
            if (isFontMode && !mode) fontSelectedChar = selectedChar; // Font modundan çıkarken aktif karakter indeksini kaydet
            else if (!isFontMode && mode) // İkon modundan çıkarken ikon durumunu kaydet
            {
                iconRowCount = tlpGrid.RowCount;
                iconColCount = tlpGrid.ColumnCount;
                iconColBytes = colBytes;
                iconText = PrepareString();
            }
            isFontMode = mode;
            isUpdatingUi = true;

            if (mode) // Font
            {
                rbFontMode.Checked = true;
                SyncFontControlsFromModel();
                controlsSet(true);
                commonMatrix = fontModel.Matrix;
                UpdateFileLabel();
                PopulateCharacterComboBox();
                SelectCharacter(fontSelectedChar);
            }
            else // Icon
            {
                rbIconMode.Checked = true;
                txtFontIconName.Text = GetIconIdentifier();
                controlsSet(false);
                // Eğer mevcut geçerli bir ikon dosyası varsa boyutları ve matrisi doğrula
                if (!string.IsNullOrEmpty(currentIconFile) && File.Exists(currentIconFile))
                {
                    try
                    {
                        string content = File.ReadAllText(currentIconFile);
                        var icModel = IconHeaderManager.ParseIconFile(content, Path.GetFileNameWithoutExtension(currentIconFile));
                        ApplyIconModelData(icModel);
                        iconText = content;
                    }
                    catch { }
                }

                SyncIconControlsFromModel();
                selectedChar = 0;
                EnsureIconMatrixCapacity(iconRowCount, iconColCount);
                commonMatrix = iconMatrix;
                UpdateFileLabel();
                UpdateGridForIcon();

                if (string.IsNullOrEmpty(currentIconFile)) rtxDataMonitor.Text = "";
                else if (!string.IsNullOrEmpty(iconText)) rtxDataMonitor.Text = iconText;
                else rtxDataMonitor.Text = PrepareString();
                PrepareLineStr(0);
            }
            isUpdatingUi = false;
            PerformResponsiveLayout();
        }
        #endregion

        #region Navigation & Dimension Controls
        private void btnPrevChar_Click(object sender, EventArgs e)
        {
            if (selectedChar > 0) SelectCharacter(selectedChar - 1);
        }

        private void btnNextChar_Click(object sender, EventArgs e)
        {
            if (selectedChar < FontModel.CHAR_COUNT - 1) SelectCharacter(selectedChar + 1);
        }

        private void cmbCharacters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isUpdatingUi) return;
            if (cmbCharacters.SelectedIndex >= 0) SelectCharacter(cmbCharacters.SelectedIndex);
        }

        private void numCharWidth_ValueChanged(object sender, EventArgs e)
        {
            if (isUpdatingUi) return;
            int newW = Math.Max(1, (int)numCharWidth.Value);
            if (isFontMode)
            {
                fontModel.EnsureCapacity(fontModel.Height, newW);
                commonMatrix = fontModel.Matrix;
                fontModel.CharWidths[selectedChar] = newW;

                UpdateCharacterComboItem(selectedChar, newW);
                UpdateGridForSelectedChar();
                UpdateCodePreview();
            }
            else
            {
                iconColCount = newW;
                Form1.EnsureGridCapacity(iconRowCount, iconColCount);
                EnsureIconMatrixCapacity(iconRowCount, iconColCount);
                UpdateGridForIcon();
                UpdateEditorView(0, refreshGrid: false);
            }
        }

        private void numFontHeight_ValueChanged(object sender, EventArgs e)
        {
            if (isUpdatingUi) return;
            int newH = Math.Max(1, (int)numFontHeight.Value);
            if (isFontMode)
            {
                fontModel.EnsureCapacity(newH, fontModel.Matrix.GetLength(2));
                commonMatrix = fontModel.Matrix;
                fontModel.Height = (byte)newH;
                UpdateGridForSelectedChar();
                UpdateCodePreview();
            }
            else
            {
                iconRowCount = newH;
                Form1.EnsureGridCapacity(iconRowCount, iconColCount);
                EnsureIconMatrixCapacity(iconRowCount, iconColCount);
                UpdateGridForIcon();
                UpdateEditorView(0, refreshGrid: false);
            }
        }

        private void numSpaceChar_ValueChanged(object sender, EventArgs e)
        {
            if (isUpdatingUi) return;
            fontModel.SpaceChar = (byte)numSpaceChar.Value;
            UpdateCodePreview();
        }

        private void numLowerSpc_ValueChanged(object sender, EventArgs e)
        {
            if (isUpdatingUi) return;
            fontModel.LowerCaseBottomSpc = (byte)numLowerSpc.Value;
            UpdateCodePreview();
        }

        private void txtFontName_TextChanged(object sender, EventArgs e)
        {
            if (isUpdatingUi) return;
            if (isFontMode)
            {
                if (!string.IsNullOrWhiteSpace(txtFontIconName.Text))
                {
                    fontModel.FontName = IconHeaderManager.SanitizeIdentifier(txtFontIconName.Text.Trim());
                    if (string.IsNullOrWhiteSpace(fontModel.DataArrayName))
                        fontModel.DataArrayName = fontModel.FontName + "Data";
                    UpdateCodePreview();
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(txtFontIconName.Text))
                {
                    iconName = IconHeaderManager.SanitizeIdentifier(txtFontIconName.Text.Trim());
                    rtxDataMonitor.Text = PrepareString();
                    iconText = rtxDataMonitor.Text;
                }
            }
        }

        private void rbViewMode_CheckedChanged(object sender, EventArgs e) => UpdateCodePreview();

        private string GetCurrentFontCode()
        {
            return rbViewFullHeader.Checked
                ? FontHeaderManager.GenerateHeaderFile(fontModel, format)
                : FontHeaderManager.GenerateCharString(fontModel, selectedChar, format);
        }

        private void UpdateCodePreview() // Font modunda RichTextBox1 kod önizleme metnini günceller.
        {
            if (!isFontMode) return;
            rtxDataMonitor.Text = GetCurrentFontCode();
        }
        #endregion
    }
}
