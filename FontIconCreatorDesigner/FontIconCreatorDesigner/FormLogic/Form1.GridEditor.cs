using System;
using System.Drawing;
using System.Windows.Forms;

namespace font_creator
{
    public partial class Form1
    {
        #region Grid Cell Painting

        private void tlpGrid_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (commonMatrix == null) return;
            if (selectedChar >= commonMatrix.GetLength(0) || e.Row >= commonMatrix.GetLength(1) || e.Column >= commonMatrix.GetLength(2)) return;

            Color cellColor;

            if (selectionBuffer != null)
            {
                if (e.Column >= selectionBox.Left && e.Column < selectionBox.Right &&
                    e.Row >= selectionBox.Top && e.Row < selectionBox.Bottom)
                {
                    int r = e.Row - selectionBox.Top;
                    int c = e.Column - selectionBox.Left;
                    cellColor = (r < selectionBuffer.GetLength(0) && c < selectionBuffer.GetLength(1))
                        ? selectionBuffer[r, c]
                        : commonMatrix[selectedChar, e.Row, e.Column];
                }
                else if (e.Column >= originalSelectionBox.Left && e.Column < originalSelectionBox.Right &&
                         e.Row >= originalSelectionBox.Top && e.Row < originalSelectionBox.Bottom)
                {
                    cellColor = Color.White;
                }
                else
                {
                    cellColor = commonMatrix[selectedChar, e.Row, e.Column];
                }
            }
            else
            {
                cellColor = commonMatrix[selectedChar, e.Row, e.Column];
            }

            using SolidBrush brush = new(cellColor);
            e.Graphics.FillRectangle(brush, e.CellBounds);

            // Seçim kutusu ve şeffaf mavi katman çizimi
            if (isSelectMoveMode && (isSelecting || selectionBuffer != null) &&
                selectionBox.Width > 0 && selectionBox.Height > 0 &&
                e.Column >= selectionBox.Left && e.Column < selectionBox.Right &&
                e.Row >= selectionBox.Top && e.Row < selectionBox.Bottom)
            {
                using SolidBrush selOverlay = new(Color.FromArgb(45, Color.DodgerBlue));
                e.Graphics.FillRectangle(selOverlay, e.CellBounds);

                using Pen pen = new(Color.DodgerBlue, 2);
                if (e.Row == selectionBox.Top)
                    e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Right, e.CellBounds.Top);
                if (e.Row == selectionBox.Bottom - 1)
                    e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
                if (e.Column == selectionBox.Left)
                    e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Left, e.CellBounds.Bottom);
                if (e.Column == selectionBox.Right - 1)
                    e.Graphics.DrawLine(pen, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom);
            }
        }

        #endregion

        #region Mouse Events for Grid Pixel Editing

        private void tlpGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (isSelectMoveMode && cbCharEdit.Checked)
            {
                HandleSelectMoveMouseDown(e);
                return;
            }
            HandleMouseDrawing(e);
        }

        private void tlpGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelectMoveMode && cbCharEdit.Checked)
            {
                HandleSelectMoveMouseMove(e);
                return;
            }
            HandleMouseDrawing(e);
        }

        private void tlpGrid_MouseUp(object sender, MouseEventArgs e)
        {
            if (isSelectMoveMode && cbCharEdit.Checked)
            {
                HandleSelectMoveMouseUp(e);
                return;
            }
            oldPoint = new Point(100, 100);
        }

        private void tlpGrid_MouseLeave(object sender, EventArgs e) => oldPoint = new Point(100, 100);

        private void HandleMouseDrawing(MouseEventArgs e)
        {
            if (commonMatrix == null) return;

            Color? drawColor = null;
            if (Control.MouseButtons == MouseButtons.Left)
                drawColor = selectedColor;
            else if (Control.MouseButtons == MouseButtons.Right)
                drawColor = NumberToColor(selectedNum) ? Color.Black : Color.White;

            if (!drawColor.HasValue) return;

            int maxWidth = isFontMode ? fontModel.CharWidths[selectedChar] : tlpGrid.ColumnCount;
            int maxHeight = isFontMode ? fontModel.Height : tlpGrid.RowCount;

            var cellPos = GetRowColIndex(tlpGrid, tlpGrid.PointToClient(Cursor.Position));
            if (cellPos != null)
            {
                Point boxPoint = (Point)cellPos;
                if (boxPoint.X < maxWidth && boxPoint.Y < maxHeight)
                {
                    if (boxPoint != oldPoint && cbCharEdit.Checked)
                    {
                        if (boxPoint.Y < commonMatrix.GetLength(1) && boxPoint.X < commonMatrix.GetLength(2))
                        {
                            commonMatrix[selectedChar, boxPoint.Y, boxPoint.X] = drawColor.Value;
                            oldPoint = boxPoint;
                            tlpGrid.Refresh();
                            if (isFontMode && rbViewCurrentChar.Checked) UpdateCodePreview();
                        }
                    }
                    PrepareLineStr(e);
                }
            }
        }

        public static Point? GetRowColIndex(TableLayoutPanel tlp, Point point)
        {
            if (point.X > tlp.Width || point.Y > tlp.Height) return null;
            int w = tlp.Width;
            int h = tlp.Height;
            int[] widths = tlp.GetColumnWidths();
            int i;
            for (i = widths.Length - 1; i >= 0 && point.X < w; i--) w -= widths[i];
            int col = i + 1;
            int[] heights = tlp.GetRowHeights();
            for (i = heights.Length - 1; i >= 0 && point.Y < h; i--) h -= heights[i];
            int row = i + 1;
            if (col >= tlp.ColumnCount || row >= tlp.RowCount) return null;
            return new Point(col, row);
        }

        private void PrepareLineStr(int row)
        {
            if (commonMatrix == null) return;
            if (row >= commonMatrix.GetLength(1)) return;

            int charIdx = (commonMatrix != null && selectedChar >= 0 && selectedChar < commonMatrix.GetLength(0)) ? selectedChar : 0;
            int charW = isFontMode ? fontModel.CharWidths[charIdx] : tlpGrid.ColumnCount;
            colBytes = (charW + 7) / 8;
            if (colBytes < 1) colBytes = 1;

            byte tempByte = 0;
            string lineString = "";
            int col = 0;
            for (int i = 0; i < colBytes; i++)
            {
                for (int b = 7; b >= 0; b--)
                {
                    if (col < charW && col < commonMatrix.GetLength(2))
                    {
                        tempByte = (byte)(tempByte | (ColorToNumber(commonMatrix[charIdx, row, col]) ? 0x01 << b : 0x00));
                    }
                    col++;
                }
                if (format == 0) lineString += tempByte.ToString() + " , ";
                else if (format == 1) lineString += "0X" + tempByte.ToString("X2") + " , ";
                else if (format == 2) lineString += "0B" + Convert.ToString(tempByte, 2).PadLeft(8, '0') + " , ";
                tempByte = 0;
            }
            lblRowValues.Text = "Row " + (row + 1) + " values :";
            txtRowValues.Text = lineString;
            lastRow = row;
        }

        private void PrepareLineStr(MouseEventArgs e)
        {
            int clickedRow = e.Y / (CELL_SIZE + 1);
            if (clickedRow < tlpGrid.RowCount) PrepareLineStr(clickedRow);
        }

        #endregion

        #region Select & Move Tool

        public void DisableSelectMoveMode()
        {
            CommitSelection();
            isSelectMoveMode = false;
            if (btnSelectMove != null)
            {
                btnSelectMove.UseVisualStyleBackColor = true;
                btnSelectMove.ForeColor = Color.Black;
                btnSelectMove.Font = new Font(btnSelectMove.Font.FontFamily, btnSelectMove.Font.Size, FontStyle.Regular);
                btnSelectMove.Text = "Select And Move";
            }
            tlpGrid.Cursor = Cursors.Default;
            tlpGrid.Refresh();
        }

        private void btnSelectMove_Click(object sender, EventArgs e)
        {
            if (isSelectMoveMode) { DisableSelectMoveMode(); }
            else
            {
                if (!cbCharEdit.Checked) cbCharEdit.Checked = true;
                isSelectMoveMode = true;
                btnSelectMove.BackColor = Color.DodgerBlue;
                btnSelectMove.ForeColor = Color.White;
                btnSelectMove.Font = new Font(btnSelectMove.Font.FontFamily, btnSelectMove.Font.Size, FontStyle.Bold);
                btnSelectMove.Text = "Select And Move ON";
                tlpGrid.Cursor = Cursors.Cross;
                tlpGrid.Refresh();
            }
        }

        private void HandleSelectMoveMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                CancelSelection();
                return;
            }
            if (e.Button != MouseButtons.Left) return;
            var cellPos = GetRowColIndex(tlpGrid, tlpGrid.PointToClient(Cursor.Position));
            if (cellPos == null) return;
            Point pt = (Point)cellPos;
            int maxWidth = isFontMode ? fontModel.CharWidths[selectedChar] : tlpGrid.ColumnCount;
            int maxHeight = isFontMode ? fontModel.Height : tlpGrid.RowCount;
            if (pt.X >= maxWidth || pt.Y >= maxHeight) return;
            if (selectionBuffer != null && selectionBox.Contains(pt))
            {
                isMovingSelection = true;
                moveGrabOffset = new Point(pt.X - selectionBox.X, pt.Y - selectionBox.Y);
            }
            else
            {
                CommitSelection();
                isSelecting = true;
                selectStartCell = pt;
                selectionBox = new Rectangle(pt.X, pt.Y, 1, 1);
                tlpGrid.Refresh();
            }
        }

        private void HandleSelectMoveMouseMove(MouseEventArgs e)
        {
            var cellPos = GetRowColIndex(tlpGrid, tlpGrid.PointToClient(Cursor.Position));

            if (selectionBuffer != null && cellPos != null && selectionBox.Contains((Point)cellPos))
            {
                tlpGrid.Cursor = Cursors.SizeAll;
            }
            else if (isSelectMoveMode)
            {
                tlpGrid.Cursor = Cursors.Cross;
            }

            if (cellPos == null) return;
            Point pt = (Point)cellPos;
            int maxWidth = isFontMode ? fontModel.CharWidths[selectedChar] : tlpGrid.ColumnCount;
            int maxHeight = isFontMode ? fontModel.Height : tlpGrid.RowCount;
            if (isSelecting)
            {
                int curX = Math.Max(0, Math.Min(maxWidth - 1, pt.X));
                int curY = Math.Max(0, Math.Min(maxHeight - 1, pt.Y));
                int x1 = Math.Min(selectStartCell.X, curX);
                int y1 = Math.Min(selectStartCell.Y, curY);
                int x2 = Math.Max(selectStartCell.X, curX);
                int y2 = Math.Max(selectStartCell.Y, curY);
                selectionBox = new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);
                tlpGrid.Refresh();
            }
            else if (isMovingSelection)
            {
                int newX = pt.X - moveGrabOffset.X;
                int newY = pt.Y - moveGrabOffset.Y;
                newX = Math.Max(0, Math.Min(maxWidth - selectionBox.Width, newX));
                newY = Math.Max(0, Math.Min(maxHeight - selectionBox.Height, newY));
                if (newX != selectionBox.X || newY != selectionBox.Y)
                {
                    selectionBox = new Rectangle(newX, newY, selectionBox.Width, selectionBox.Height);
                    tlpGrid.Refresh();
                }
            }
        }

        private void HandleSelectMoveMouseUp(MouseEventArgs e)
        {
            if (isSelecting)
            {
                isSelecting = false;
                if (selectionBox.Width > 0 && selectionBox.Height > 0)
                {
                    int w = selectionBox.Width;
                    int h = selectionBox.Height;
                    selectionBuffer = new Color[h, w];
                    for (int r = 0; r < h; r++)
                        for (int c = 0; c < w; c++) selectionBuffer[r, c] = commonMatrix[selectedChar, selectionBox.Y + r, selectionBox.X + c];
                    
                    originalSelectionBox = selectionBox;
                    tlpGrid.Refresh();
                }
            }
            else if (isMovingSelection)
            {
                isMovingSelection = false;
                tlpGrid.Refresh();
            }
        }

        public void CommitSelection()
        {
            if (selectionBuffer == null) return;
            int maxWidth = isFontMode ? fontModel.CharWidths[selectedChar] : tlpGrid.ColumnCount;
            int maxHeight = isFontMode ? fontModel.Height : tlpGrid.RowCount;
            // 1. Orijinal alanı temizle (Beyaz)
            for (int r = originalSelectionBox.Top; r < originalSelectionBox.Bottom; r++)
                for (int c = originalSelectionBox.Left; c < originalSelectionBox.Right; c++)
                    if (r < commonMatrix.GetLength(1) && c < commonMatrix.GetLength(2)) commonMatrix[selectedChar, r, c] = Color.White;
            // 2. Taşınan pikselleri hedef konuma yaz
            for (int r = 0; r < selectionBox.Height; r++)
            {
                for (int c = 0; c < selectionBox.Width; c++)
                {
                    int destR = selectionBox.Y + r;
                    int destC = selectionBox.X + c;
                    if (destR < commonMatrix.GetLength(1) && destC < commonMatrix.GetLength(2) &&
                        destR < maxHeight && destC < maxWidth)
                    {
                        commonMatrix[selectedChar, destR, destC] = selectionBuffer[r, c];
                    }
                }
            }

            CancelSelection();
            UpdateEditorView(lastRow);
        }

        public void CancelSelection()
        {
            selectionBuffer = null;
            isMovingSelection = false;
            isSelecting = false;
            selectionBox = Rectangle.Empty;
            originalSelectionBox = Rectangle.Empty;
            tlpGrid.Refresh();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (isSelectMoveMode && selectionBuffer != null)
            {
                int maxWidth = isFontMode ? fontModel.CharWidths[selectedChar] : tlpGrid.ColumnCount;
                int maxHeight = isFontMode ? fontModel.Height : tlpGrid.RowCount;
                int dx = 0, dy = 0;
                if (keyData == Keys.Left) dx = -1;
                else if (keyData == Keys.Right) dx = 1;
                else if (keyData == Keys.Up) dy = -1;
                else if (keyData == Keys.Down) dy = 1;
                else if (keyData == Keys.Enter)
                {
                    CommitSelection();
                    return true;
                }
                else if (keyData == Keys.Escape)
                {
                    CancelSelection();
                    return true;
                }

                if (dx != 0 || dy != 0)
                {
                    int newX = Math.Max(0, Math.Min(maxWidth - selectionBox.Width, selectionBox.X + dx));
                    int newY = Math.Max(0, Math.Min(maxHeight - selectionBox.Height, selectionBox.Y + dy));
                    selectionBox = new Rectangle(newX, newY, selectionBox.Width, selectionBox.Height);
                    tlpGrid.Refresh();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion
        #region Trim Blank Top Rows & Shift Up

        private void btnTrimTop_Click(object sender, EventArgs e) => TrimTopBlankRowsAndShiftUp();

        /// Karakterlerin üst kısmındaki 0x00 boşluk satırlarını siler ve tüm pikselleri yukarı öteler.
        public void TrimTopBlankRowsAndShiftUp()
        {
            CommitSelection();
            if (commonMatrix == null) return;
            if (isFontMode)
            {
                int h = fontModel.Height;
                int commonBlankRows = 0;
                for (int r = 0; r < h; r++)
                {
                    bool isRowAllZeroAcrossFont = true;
                    for (int i = 0; i < FontModel.CHAR_COUNT; i++)
                    {
                        int charW = fontModel.CharWidths[i];
                        for (int c = 0; c < charW; c++)
                        {
                            if (fontModel.Matrix[i, r, c] == Color.Black)
                            {
                                isRowAllZeroAcrossFont = false;
                                break;
                            }
                        }
                        if (!isRowAllZeroAcrossFont) break;
                    }

                    if (isRowAllZeroAcrossFont) commonBlankRows++;
                    else break;
                }

                if (commonBlankRows > 0)
                {
                    DialogResult dr = MessageBox.Show(this,
                        $"Above all 95 characters {commonBlankRows} common row space (0x00) detected.\n\n" +
                        $"Should all characters be shifted up by {commonBlankRows} rows, and the font height updated from {h} to {h - commonBlankRows}?\n\n" +
                        "[Yes]: Shift the characters upward and reduce the font height.\n" +
                        "[No]: Shift upward only, without changing the font height (leaving a gap at the bottom).\n" +
                        "[Cancel]: Do not perform any action.",
                        "Delete Top Space and Shift Up",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (dr == DialogResult.Cancel) return;
                    bool reduceHeight = (dr == DialogResult.Yes);
                    // 95 karakterin tamamını commonBlankRows kadar yukarı kaydır
                    for (int i = 0; i < FontModel.CHAR_COUNT; i++)
                    {
                        for (int r = 0; r < h - commonBlankRows; r++)
                            for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                                fontModel.Matrix[i, r, c] = fontModel.Matrix[i, r + commonBlankRows, c];
                        for (int r = h - commonBlankRows; r < h; r++)
                            for (int c = 0; c < fontModel.Matrix.GetLength(2); c++) fontModel.Matrix[i, r, c] = Color.White;
                    }

                    if (reduceHeight)
                    {
                        byte newH = (byte)Math.Max(1, h - commonBlankRows);
                        fontModel.Height = newH;
                        isUpdatingUi = true;
                        numFontHeight.Value = newH;
                        isUpdatingUi = false;
                    }

                    UpdateGridForSelectedChar();
                    UpdateEditorView(0);
                    MessageBox.Show(this,
                        $"All characters have been successfully shifted up by {commonBlankRows} rows." +
                        (reduceHeight ? $"\nNew Font Height: {fontModel.Height}" : ""),
                        "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    DialogResult dr = MessageBox.Show(this,
                        "No common top margin could be found for all characters (at least one character uses the topmost row).\n\n" +
                        "Do you still want to shift all characters up by 1 row?\n(The pixels in the top row will be deleted)",
                        "Top Space Not Found",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (dr == DialogResult.Yes)
                    {
                        for (int i = 0; i < FontModel.CHAR_COUNT; i++)
                        {
                            for (int r = 0; r < h - 1; r++)
                            {
                                for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                                {
                                    fontModel.Matrix[i, r, c] = fontModel.Matrix[i, r + 1, c];
                                }
                            }
                            for (int c = 0; c < fontModel.Matrix.GetLength(2); c++) fontModel.Matrix[i, h - 1, c] = Color.White;
                        }

                        UpdateGridForSelectedChar();
                        UpdateEditorView(0);

                        MessageBox.Show(this, "All characters were shifted up by one line.", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else // İkon Modu
            {
                int h = tlpGrid.RowCount;
                int w = tlpGrid.ColumnCount;
                int blankRows = 0;
                for (int r = 0; r < h; r++)
                {
                    bool isRowZero = true;
                    for (int c = 0; c < w; c++)
                    {
                        if (iconMatrix[0, r, c] == Color.Black)
                        {
                            isRowZero = false;
                            break;
                        }
                    }
                    if (isRowZero) blankRows++;
                    else break;
                }

                if (blankRows > 0)
                {
                    DialogResult dr = MessageBox.Show(this,
                        $"{blankRows} rows of empty space were detected above the character.\n\n" +
                        $"Should the character be shifted up by {blankRows} rows?",
                        "Scroll Up", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                    {
                        for (int r = 0; r < h - blankRows; r++)
                            for (int c = 0; c < w; c++) iconMatrix[0, r, c] = iconMatrix[0, r + blankRows, c];
                        for (int r = h - blankRows; r < h; r++)
                            for (int c = 0; c < w; c++) iconMatrix[0, r, c] = Color.White;

                        UpdateEditorView(0);
                        MessageBox.Show(this, $"The character was shifted up by {blankRows} rows.", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show(this, "No blank lines were found above the character (line 1 was filled).", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        #endregion

        #region Sync Upper Case

        /// <summary>
        /// Font yüksekliğini büyük harf (uppercase) yüksekliği ile senkronize eder.
        /// 1- b, d, h, k, l, t karakterlerinin üstünden fazlalık silinir.
        /// 2- i, j karakterlerinin nokta kısmı aşağı kaydırılır.
        /// 3- f karakterinin üst eğimi aşağı kaydırılarak gövdeden eksiltilir.
        /// 4- p, q karakterlerinin altından eksiltilir.
        /// 5- y, g, j karakterlerinde harfin bozulmaması adına eğimin başladığı yerin üstündeki düz kısımdan eksiltilir.
        /// 6- Diğer karakterler üst sınırın üstüne taşıyorsa taştığı miktar kadar aşağı kaydırılır.
        /// 7- Üstteki ve alttaki boşluklar silinerek font yüksekliği güncellenir.
        /// 8- Farklı font yükseklikleri ve harf yapıları (örn. c43.h g harfi) için dinamik algılama kullanılır.
        /// </summary>
        private void btnSyncUC_Click(object sender, EventArgs e)
        {
            if (!isFontMode || fontModel == null || commonMatrix == null)
            {
                MessageBox.Show(this, "Sync Upper Case is only available in Font mode with an active font.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            CommitSelection();
            // 1. Büyük harf referansını ('A') tespit et
            int aIdx = 'A' - FontModel.FIRST_ASCII;
            int ucTop = -1;
            int ucBottom = -1;

            if (aIdx >= 0 && aIdx < FontModel.CHAR_COUNT)
            {
                for (int r = 0; r < fontModel.Height; r++)
                {
                    for (int c = 0; c < fontModel.CharWidths[aIdx]; c++)
                    {
                        if (fontModel.Matrix[aIdx, r, c] == Color.Black)
                        {
                            if (ucTop == -1) ucTop = r;
                            ucBottom = r;
                            break;
                        }
                    }
                }
            }

            // 'A' boşsa diğer büyük harflerden (B..Z, Q hariç) tespit et
            if (ucTop == -1)
            {
                for (char ch = 'B'; ch <= 'Z'; ch++)
                {
                    if (ch == 'Q') continue;
                    int idx = ch - FontModel.FIRST_ASCII;
                    if (idx < 0 || idx >= FontModel.CHAR_COUNT) continue;
                    int t = -1, b = -1;
                    for (int r = 0; r < fontModel.Height; r++)
                    {
                        for (int c = 0; c < fontModel.CharWidths[idx]; c++)
                        {
                            if (fontModel.Matrix[idx, r, c] == Color.Black)
                            {
                                if (t == -1) t = r;
                                b = r;
                                break;
                            }
                        }
                    }
                    if (t != -1)
                    {
                        ucTop = t;
                        ucBottom = b;
                        break;
                    }
                }
            }

            if (ucTop == -1)
            {
                MessageBox.Show(this, "Could not detect uppercase character height. Please ensure uppercase letters ('A'..'Z') have bitmap data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int ucHeight = ucBottom - ucTop + 1;

            // 2. Uzantısı olmayan küçük harf referansını ('a') tespit et
            int aLowerIdx = 'a' - FontModel.FIRST_ASCII;
            int lcTop = -1;
            int lcBottom = -1;

            if (aLowerIdx >= 0 && aLowerIdx < FontModel.CHAR_COUNT)
            {
                for (int r = 0; r < fontModel.Height; r++)
                {
                    for (int c = 0; c < fontModel.CharWidths[aLowerIdx]; c++)
                    {
                        if (fontModel.Matrix[aLowerIdx, r, c] == Color.Black)
                        {
                            if (lcTop == -1) lcTop = r;
                            lcBottom = r;
                            break;
                        }
                    }
                }
            }

            // 'a' boşsa diğer uzantısız küçük harflerden tespit et
            if (lcTop == -1)
            {
                char[] nonDesc = { 'c', 'e', 'm', 'n', 'o', 'r', 's', 'u', 'v', 'w', 'x', 'z' };
                foreach (char ch in nonDesc)
                {
                    int idx = ch - FontModel.FIRST_ASCII;
                    if (idx < 0 || idx >= FontModel.CHAR_COUNT) continue;
                    int t = -1, b = -1;
                    for (int r = 0; r < fontModel.Height; r++)
                    {
                        for (int c = 0; c < fontModel.CharWidths[idx]; c++)
                        {
                            if (fontModel.Matrix[idx, r, c] == Color.Black)
                            {
                                if (t == -1) t = r;
                                b = r;
                                break;
                            }
                        }
                    }
                    if (t != -1)
                    {
                        lcTop = t;
                        lcBottom = b;
                        break;
                    }
                }
            }

            int diff_A_a = (lcTop != -1 && lcTop > ucTop) ? (lcTop - ucTop) : (int)fontModel.LowerCaseBottomSpc;
            if (diff_A_a <= 0) diff_A_a = Math.Max(1, ucHeight / 3);

            int currentHeight = fontModel.Height;
            int targetHeight = ucHeight + diff_A_a;
            int cutTop = ucTop;
            int cutBottom = (currentHeight - 1 - ucBottom) - diff_A_a;
            if (cutBottom < 0) cutBottom = 0;
            int bottomLimit = cutTop + targetHeight - 1;

            if (cutTop == 0 && cutBottom == 0 && currentHeight == targetHeight)
            {
                MessageBox.Show(this, $"Font is already synchronized with uppercase height ({targetHeight} px).", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult dr = MessageBox.Show(this,
                $"Uppercase height: {ucHeight} px (rows {ucTop}..{ucBottom})\n" +
                $"Lowercase gap (A - a): {diff_A_a} px\n" +
                $"Top space to remove: {cutTop} rows\n" +
                $"Bottom space to remove: {cutBottom} rows\n" +
                $"Target font height: {targetHeight} px ({ucHeight} + {diff_A_a})\n\n" +
                "Do you want to proceed with synchronizing the font height?",
                "Sync Upper Case",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            // 1- b, d, h, k, l, t karakterlerinin üstünden cutTop kadar sil
            char[] stemAscenders = { 'b', 'd', 'h', 'k', 'l', 't' };
            if (cutTop > 0)
            {
                foreach (char ch in stemAscenders)
                {
                    int idx = ch - FontModel.FIRST_ASCII;
                    if (idx < 0 || idx >= FontModel.CHAR_COUNT) continue;
                    for (int r = 0; r < cutTop; r++)
                    {
                        for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                        {
                            fontModel.Matrix[idx, r, c] = Color.White;
                        }
                    }
                }
            }

            // 2- i, j için nokta kısmı aşağı kaydırılır
            char[] dotChars = { 'i', 'j' };
            foreach (char ch in dotChars)
            {
                int idx = ch - FontModel.FIRST_ASCII;
                if (idx < 0 || idx >= FontModel.CHAR_COUNT) continue;

                int dotStart = -1, dotEnd = -1;
                for (int r = 0; r < lcTop; r++)
                {
                    bool hasPx = false;
                    for (int c = 0; c < fontModel.CharWidths[idx]; c++)
                    {
                        if (fontModel.Matrix[idx, r, c] == Color.Black)
                        {
                            hasPx = true;
                            break;
                        }
                    }
                    if (hasPx)
                    {
                        if (dotStart == -1) dotStart = r;
                        dotEnd = r;
                    }
                }

                if (dotStart != -1 && dotStart < cutTop)
                {
                    int shift = cutTop - dotStart;
                    if (shift > 0 && dotEnd + shift < lcTop)
                    {
                        for (int r = dotEnd; r >= dotStart; r--)
                        {
                            for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                            {
                                fontModel.Matrix[idx, r + shift, c] = fontModel.Matrix[idx, r, c];
                            }
                        }
                        for (int r = dotStart; r < dotStart + shift; r++)
                        {
                            for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                            {
                                fontModel.Matrix[idx, r, c] = Color.White;
                            }
                        }
                    }
                }
            }

            // 3- f için eğimin başladığı yerden aşağıya doğru silinir (üst eğim aşağı kaydırılır)
            int fIdx = 'f' - FontModel.FIRST_ASCII;
            if (fIdx >= 0 && fIdx < FontModel.CHAR_COUNT)
            {
                int fStart = -1;
                for (int r = 0; r < lcTop; r++)
                {
                    for (int c = 0; c < fontModel.CharWidths[fIdx]; c++)
                    {
                        if (fontModel.Matrix[fIdx, r, c] == Color.Black)
                        {
                            fStart = r;
                            break;
                        }
                    }
                    if (fStart != -1) break;
                }

                if (fStart != -1 && fStart < cutTop)
                {
                    int shift = cutTop - fStart;
                    if (shift > 0)
                    {
                        int hookEnd = -1;
                        for (int r = lcTop - 1; r > fStart; r--)
                        {
                            bool sameAsAbove = true;
                            for (int c = 0; c < fontModel.CharWidths[fIdx]; c++)
                            {
                                if (fontModel.Matrix[fIdx, r, c] != fontModel.Matrix[fIdx, r - 1, c])
                                {
                                    sameAsAbove = false;
                                    break;
                                }
                            }
                            if (!sameAsAbove)
                            {
                                hookEnd = r - 1;
                                break;
                            }
                        }

                        if (hookEnd == -1 || hookEnd < fStart)
                        {
                            hookEnd = fStart + Math.Max(1, (lcTop - fStart) / 2);
                        }

                        if (hookEnd + shift < lcTop)
                        {
                            for (int r = hookEnd; r >= fStart; r--)
                            {
                                for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                                {
                                    fontModel.Matrix[fIdx, r + shift, c] = fontModel.Matrix[fIdx, r, c];
                                }
                            }
                            for (int r = fStart; r < fStart + shift; r++)
                            {
                                for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                                {
                                    fontModel.Matrix[fIdx, r, c] = Color.White;
                                }
                            }
                        }
                        else
                        {
                            for (int r = 0; r < cutTop; r++)
                                for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                                    fontModel.Matrix[fIdx, r, c] = Color.White;
                        }
                    }
                }
            }

            // 4- p, q için alttan cutBottom kadar silinir
            if (cutBottom > 0)
            {
                char[] stemDescenders = { 'p', 'q' };
                foreach (char ch in stemDescenders)
                {
                    int idx = ch - FontModel.FIRST_ASCII;
                    if (idx < 0 || idx >= FontModel.CHAR_COUNT) continue;
                    for (int r = currentHeight - cutBottom; r < currentHeight; r++)
                    {
                        for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                        {
                            fontModel.Matrix[idx, r, c] = Color.White;
                        }
                    }
                }
            }

            // 5 & 8- y, g, j için harfin bozulmaması adına eğimin başladığı yerin üstünden / alt eğimler arasından cutBottom kadar atılır
            if (cutBottom > 0)
            {
                char[] curveDescenders = { 'y', 'g', 'j' };
                foreach (char ch in curveDescenders)
                {
                    int idx = ch - FontModel.FIRST_ASCII;
                    if (idx < 0 || idx >= FontModel.CHAR_COUNT) continue;

                    bool hasBottomPixels = false;
                    for (int r = currentHeight - cutBottom; r < currentHeight; r++)
                    {
                        for (int c = 0; c < fontModel.CharWidths[idx]; c++)
                        {
                            if (fontModel.Matrix[idx, r, c] == Color.Black)
                            {
                                hasBottomPixels = true;
                                break;
                            }
                        }
                        if (hasBottomPixels) break;
                    }

                    if (!hasBottomPixels) continue;

                    // ucBottom'dan aşağıda, harfin en az değiştiği (düz/eğimin başladığı yerin üstündeki) satırları bul
                    int bestCutRow = -1;
                    int minDiff = int.MaxValue;
                    int searchStart = ucBottom + 1;
                    int searchEnd = currentHeight - cutBottom - 1;

                    for (int r = searchStart; r <= searchEnd; r++)
                    {
                        int diff = 0;
                        for (int c = 0; c < fontModel.CharWidths[idx]; c++)
                        {
                            if (fontModel.Matrix[idx, r, c] != fontModel.Matrix[idx, r + cutBottom, c])
                                diff++;
                        }
                        if (diff < minDiff)
                        {
                            minDiff = diff;
                            bestCutRow = r;
                            if (minDiff == 0) break;
                        }
                    }

                    if (bestCutRow != -1)
                    {
                        for (int r = bestCutRow + cutBottom; r < currentHeight; r++)
                        {
                            for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                            {
                                fontModel.Matrix[idx, r - cutBottom, c] = fontModel.Matrix[idx, r, c];
                            }
                        }
                        for (int r = currentHeight - cutBottom; r < currentHeight; r++)
                        {
                            for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                            {
                                fontModel.Matrix[idx, r, c] = Color.White;
                            }
                        }
                    }
                    else
                    {
                        for (int r = currentHeight - cutBottom; r < currentHeight; r++)
                        {
                            for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                            {
                                fontModel.Matrix[idx, r, c] = Color.White;
                            }
                        }
                    }
                }
            }

            // 6- Süslü Parantezler: { }
            // Ortadaki çıkıntı dikkate alınarak alt ve üst kıvrımlardan sonra kırpmalar yarı yarıya yapılır.
            // Örneğin 4 için 2 üstten 2 alttan. 3 için 1 üstten 1 alttan ve kalan 1 ortadaki kıvrımın tam ortasından silinir.
            char[] curlyBraces = { '{', '}' };
            foreach (char ch in curlyBraces)
            {
                int idx = ch - FontModel.FIRST_ASCII;
                if (idx < 0 || idx >= FontModel.CHAR_COUNT) continue;

                int minRow = -1;
                int maxRow = -1;
                for (int r = 0; r < currentHeight; r++)
                {
                    for (int c = 0; c < fontModel.CharWidths[idx]; c++)
                    {
                        if (fontModel.Matrix[idx, r, c] == Color.Black)
                        {
                            if (minRow == -1) minRow = r;
                            maxRow = r;
                            break;
                        }
                    }
                }
                if (minRow == -1) continue;

                int charH = maxRow - minRow + 1;
                int totalShorten = charH - targetHeight;
                if (totalShorten > 0)
                {
                    int cutUpper = totalShorten / 2;
                    int cutLower = totalShorten / 2;
                    int cutMiddle = totalShorten % 2;

                    // Ortadaki çıkıntının merkezini (apeks) tespit et
                    int midRow = (minRow + maxRow) / 2;
                    int bestApexRow = midRow;
                    int bestColScore = (ch == '{') ? int.MaxValue : int.MinValue;
                    int bestDist = int.MaxValue;

                    for (int r = minRow + 4; r <= maxRow - 4; r++)
                    {
                        int minCol = int.MaxValue;
                        int maxCol = int.MinValue;
                        for (int c = 0; c < fontModel.CharWidths[idx]; c++)
                        {
                            if (fontModel.Matrix[idx, r, c] == Color.Black)
                            {
                                if (c < minCol) minCol = c;
                                if (c > maxCol) maxCol = c;
                            }
                        }

                        if (minCol != int.MaxValue)
                        {
                            int dist = Math.Abs(r - midRow);
                            if (ch == '{')
                            {
                                if (minCol < bestColScore || (minCol == bestColScore && dist < bestDist))
                                {
                                    bestColScore = minCol;
                                    bestDist = dist;
                                    bestApexRow = r;
                                }
                            }
                            else // '}'
                            {
                                if (maxCol > bestColScore || (maxCol == bestColScore && dist < bestDist))
                                {
                                    bestColScore = maxCol;
                                    bestDist = dist;
                                    bestApexRow = r;
                                }
                            }
                        }
                    }

                    var rowsToRemove = new System.Collections.Generic.HashSet<int>();

                    // 1. Kalan 1 satır ortadaki kıvrımın tam ortasından silinir
                    if (cutMiddle > 0) rowsToRemove.Add(bestApexRow);
                    // 2. Üst kıvrımdan sonraki düz kısımdan cutUpper kadar silinir
                    var upperStem = new System.Collections.Generic.List<int>();
                    for (int r = minRow + 5; r <= bestApexRow - 3; r++) upperStem.Add(r);
                    if (upperStem.Count > 0)
                    {
                        for (int i = 0; i < cutUpper; i++)
                        {
                            int pick = upperStem.Count / 2 + i - (cutUpper / 2);
                            pick = Math.Max(0, Math.Min(upperStem.Count - 1, pick));
                            rowsToRemove.Add(upperStem[pick]);
                        }
                    }
                    // 3. Alt kıvrımdan önceki düz kısımdan cutLower kadar silinir
                    var lowerStem = new System.Collections.Generic.List<int>();
                    for (int r = bestApexRow + 4; r <= maxRow - 5; r++) lowerStem.Add(r);
                    if (lowerStem.Count > 0)
                    {
                        for (int i = 0; i < cutLower; i++)
                        {
                            int pick = lowerStem.Count / 2 + i - (cutLower / 2);
                            pick = Math.Max(0, Math.Min(lowerStem.Count - 1, pick));
                            rowsToRemove.Add(lowerStem[pick]);
                        }
                    }
                    // Kalan satırları topla ve cutTop'tan başlayarak yerleştir
                    var keptRows = new System.Collections.Generic.List<Color[]>();
                    for (int r = minRow; r <= maxRow; r++)
                    {
                        if (!rowsToRemove.Contains(r))
                        {
                            var rowColors = new Color[fontModel.Matrix.GetLength(2)];
                            for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                                rowColors[c] = fontModel.Matrix[idx, r, c];
                            keptRows.Add(rowColors);
                        }
                    }

                    // Matrise yaz
                    for (int r = 0; r < currentHeight; r++)
                    {
                        for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                        {
                            int keptIdx = r - cutTop;
                            if (keptIdx >= 0 && keptIdx < keptRows.Count) fontModel.Matrix[idx, r, c] = keptRows[keptIdx][c];
                            else fontModel.Matrix[idx, r, c] = Color.White;
                        }
                    }
                }
                else
                {
                    int excessTop = Math.Max(0, cutTop - minRow);
                    if (excessTop > 0)
                    {
                        for (int r = maxRow; r >= minRow; r--)
                        {
                            if (r + excessTop < currentHeight)
                            {
                                for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                                    fontModel.Matrix[idx, r + excessTop, c] = fontModel.Matrix[idx, r, c];
                            }
                        }
                        for (int r = minRow; r < minRow + excessTop; r++)
                             for (int c = 0; c < fontModel.Matrix.GetLength(2); c++) fontModel.Matrix[idx, r, c] = Color.White;
                    }
                }
            }

            // 7- Köşeli Parantezler ve Yay Parantezler: [ ] ( )
            // Üst ve alt köşeler/kıvrımlar korunarak aradaki düz gövde kısmından eksiltilir
            char[] otherBrackets = { '[', ']', '(', ')' };
            foreach (char ch in otherBrackets)
            {
                int idx = ch - FontModel.FIRST_ASCII;
                if (idx < 0 || idx >= FontModel.CHAR_COUNT) continue;

                int minRow = -1;
                int maxRow = -1;
                for (int r = 0; r < currentHeight; r++)
                {
                    for (int c = 0; c < fontModel.CharWidths[idx]; c++)
                    {
                        if (fontModel.Matrix[idx, r, c] == Color.Black)
                        {
                            if (minRow == -1) minRow = r;
                            maxRow = r;
                            break;
                        }
                    }
                }
                if (minRow == -1) continue;

                int excessTop = Math.Max(0, cutTop - minRow);
                int excessBottom = Math.Max(0, maxRow - bottomLimit);

                if (excessTop > 0)
                {
                    int topEnd = minRow;
                    if (ch == '[' || ch == ']')
                    {
                        int stemW = 0;
                        int midRow = (minRow + maxRow) / 2;
                        for (int c = 0; c < fontModel.CharWidths[idx]; c++)
                            if (fontModel.Matrix[idx, midRow, c] == Color.Black) stemW++;

                        for (int r = minRow; r < midRow; r++)
                        {
                            int rw = 0;
                            for (int c = 0; c < fontModel.CharWidths[idx]; c++)
                                if (fontModel.Matrix[idx, r, c] == Color.Black) rw++;
                            if (rw > stemW) topEnd = r;
                            else break;
                        }
                    }
                    else // '(' or ')'
                    {
                        topEnd = minRow + (maxRow - minRow) / 4;
                    }

                    for (int r = topEnd; r >= minRow; r--)
                    {
                        if (r + excessTop < currentHeight)
                        {
                            for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                                fontModel.Matrix[idx, r + excessTop, c] = fontModel.Matrix[idx, r, c];
                        }
                    }
                    for (int r = minRow; r < minRow + excessTop; r++)
                        for (int c = 0; c < fontModel.Matrix.GetLength(2); c++) fontModel.Matrix[idx, r, c] = Color.White;
                }

                if (excessBottom > 0)
                {
                    int botStart = maxRow;
                    if (ch == '[' || ch == ']')
                    {
                        int stemW = 0;
                        int midRow = (minRow + maxRow) / 2;
                        for (int c = 0; c < fontModel.CharWidths[idx]; c++)
                            if (fontModel.Matrix[idx, midRow, c] == Color.Black) stemW++;

                        for (int r = maxRow; r > midRow; r--)
                        {
                            int rw = 0;
                            for (int c = 0; c < fontModel.CharWidths[idx]; c++)
                                if (fontModel.Matrix[idx, r, c] == Color.Black) rw++;
                            if (rw > stemW) botStart = r;
                            else break;
                        }
                    }
                    else // '(' or ')'
                    {
                        botStart = maxRow - (maxRow - minRow) / 4;
                    }

                    for (int r = botStart; r <= maxRow; r++)
                    {
                        if (r - excessBottom >= 0)
                        {
                            for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                                fontModel.Matrix[idx, r - excessBottom, c] = fontModel.Matrix[idx, r, c];
                        }
                    }
                    for (int r = maxRow - excessBottom + 1; r <= maxRow; r++)
                        for (int c = 0; c < fontModel.Matrix.GetLength(2); c++) fontModel.Matrix[idx, r, c] = Color.White;
                }
            }

            // 7.5- $ Karakteri: Üst ucu A'nın üst ucuna (ucTop) denk gelecek şekilde hizalanır.
            // Kesilecek miktar üstten ve alttan orantılı olarak kesilir (örn. 4 ise 2 üstten, 2 alttan).
            // Bölümden 1 artarsa (tek sayı ise) kalan 1 satır alttaki uzantıdan kesilir ve üst ucu A hizasına indirilir.
            int dollarIdx = '$' - FontModel.FIRST_ASCII;
            if (dollarIdx >= 0 && dollarIdx < FontModel.CHAR_COUNT)
            {
                int dlMinRow = -1;
                int dlMaxRow = -1;
                for (int r = 0; r < currentHeight; r++)
                {
                    for (int c = 0; c < fontModel.CharWidths[dollarIdx]; c++)
                    {
                        if (fontModel.Matrix[dollarIdx, r, c] == Color.Black)
                        {
                            if (dlMinRow == -1) dlMinRow = r;
                            dlMaxRow = r;
                            break;
                        }
                    }
                }

                if (dlMinRow != -1 && dlMaxRow >= dlMinRow)
                {
                    int excessTop = Math.Max(0, cutTop - dlMinRow);
                    if (excessTop > 0)
                    {
                        int cutFromTop = excessTop / 2;
                        int cutFromBottom = excessTop - cutFromTop; // Tek sayı durumunda artan 1 satır alttan kesilir

                        int keptStart = dlMinRow + cutFromTop;
                        int keptEnd = dlMaxRow - cutFromBottom;

                        var keptRows = new System.Collections.Generic.List<Color[]>();
                        for (int r = keptStart; r <= keptEnd; r++)
                        {
                            var rowColors = new Color[fontModel.Matrix.GetLength(2)];
                            for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                                rowColors[c] = fontModel.Matrix[dollarIdx, r, c];
                            keptRows.Add(rowColors);
                        }

                        // Altta bottomLimit sınırını aşma durumuna karşı güvenlik kontrolü
                        while (ucTop + keptRows.Count - 1 > bottomLimit)
                        {
                            keptRows.RemoveAt(keptRows.Count - 1);
                        }

                        // Matrisi temizle ve üst ucu A'nın üst ucuna (ucTop) denk gelecek şekilde yerleştir
                        for (int r = 0; r < currentHeight; r++)
                        {
                            for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                            {
                                int keptIdx = r - ucTop;
                                if (keptIdx >= 0 && keptIdx < keptRows.Count)
                                    fontModel.Matrix[dollarIdx, r, c] = keptRows[keptIdx][c];
                                else
                                    fontModel.Matrix[dollarIdx, r, c] = Color.White;
                            }
                        }
                    }
                    else
                    {
                        // excessTop == 0 durumunda, eğer alttan taşıyorsa altı temizle
                        for (int r = bottomLimit + 1; r < currentHeight; r++)
                            for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                                fontModel.Matrix[dollarIdx, r, c] = Color.White;
                    }
                }
            }

            // 7.6- |, /, \ karakterlerinin sınırları taşan kısımları kırpılır
            char[] verticalTrimChars = { '|', '/', '\\' };
            foreach (char ch in verticalTrimChars)
            {
                int idx = ch - FontModel.FIRST_ASCII;
                if (idx < 0 || idx >= FontModel.CHAR_COUNT) continue;
                for (int r = 0; r < cutTop; r++)
                    for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                        fontModel.Matrix[idx, r, c] = Color.White;

                for (int r = bottomLimit + 1; r < currentHeight; r++)
                    for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                        fontModel.Matrix[idx, r, c] = Color.White;
            }

            // 8- !, ? karakterleri
            int exclIdx = '!' - FontModel.FIRST_ASCII;
            if (exclIdx >= 0 && exclIdx < FontModel.CHAR_COUNT)
            {
                for (int r = 0; r < cutTop; r++)
                    for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                        fontModel.Matrix[exclIdx, r, c] = Color.White;
            }

            int questIdx = '?' - FontModel.FIRST_ASCII;
            if (questIdx >= 0 && questIdx < FontModel.CHAR_COUNT)
            {
                int qMin = -1;
                for (int r = 0; r < cutTop; r++)
                {
                    for (int c = 0; c < fontModel.CharWidths[questIdx]; c++)
                    {
                        if (fontModel.Matrix[questIdx, r, c] == Color.Black)
                        {
                            qMin = r;
                            break;
                        }
                    }
                    if (qMin != -1) break;
                }
                if (qMin != -1)
                {
                    int shift = cutTop - qMin;
                    if (shift > 0)
                    {
                        for (int r = 19; r >= qMin; r--)
                            if (r + shift < currentHeight)
                                for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                                    fontModel.Matrix[questIdx, r + shift, c] = fontModel.Matrix[questIdx, r, c];
                        for (int r = qMin; r < qMin + shift; r++)
                            for (int c = 0; c < fontModel.Matrix.GetLength(2); c++) fontModel.Matrix[questIdx, r, c] = Color.White;
                    }
                }
            }

            // 9- Diğer karakterler için üstten cutTop sınırını aşanlar güvenli miktarda aşağı kaydırılır
            var specialChars = new System.Collections.Generic.HashSet<char> {
                'A', 'a', 'b', 'd', 'h', 'k', 'l', 't', 'i', 'j', 'f', 'p', 'q', 'y', 'g',
                '[', ']', '{', '}', '(', ')', '$', '|', '/', '\\', '!', '?'
            };

            if (cutTop > 0)
            {
                for (int i = 0; i < FontModel.CHAR_COUNT; i++)
                {
                    char ch = (char)(FontModel.FIRST_ASCII + i);
                    if (specialChars.Contains(ch)) continue;
                    int minRow = -1;
                    int maxRow = -1;
                    for (int r = 0; r < currentHeight; r++)
                    {
                        for (int c = 0; c < fontModel.CharWidths[i]; c++)
                        {
                            if (fontModel.Matrix[i, r, c] == Color.Black)
                            {
                                if (minRow == -1) minRow = r;
                                maxRow = r;
                                break;
                            }
                        }
                    }

                    if (minRow != -1 && minRow < cutTop)
                    {
                        int shift = cutTop - minRow;
                        int maxShift = Math.Max(0, bottomLimit - maxRow);
                        int actShift = Math.Min(shift, maxShift);

                        if (actShift > 0)
                        {
                            for (int r = currentHeight - 1; r >= actShift; r--)
                                for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                                     fontModel.Matrix[i, r, c] = fontModel.Matrix[i, r - actShift, c];
                            for (int r = 0; r < actShift; r++)
                                for (int c = 0; c < fontModel.Matrix.GetLength(2); c++) fontModel.Matrix[i, r, c] = Color.White;
                        }
                        for (int r = 0; r < cutTop; r++)
                            for (int c = 0; c < fontModel.Matrix.GetLength(2); c++) fontModel.Matrix[i, r, c] = Color.White;
                    }
                }
            }

            // 7- Üstte cutTop, altta cutBottom boşlukları silinerek font yüksekliği targetHeight yapılır
            for (int i = 0; i < FontModel.CHAR_COUNT; i++)
            {
                for (int r = 0; r < targetHeight; r++)
                    for (int c = 0; c < fontModel.Matrix.GetLength(2); c++)
                        fontModel.Matrix[i, r, c] = fontModel.Matrix[i, cutTop + r, c];
                for (int r = targetHeight; r < fontModel.Matrix.GetLength(1); r++)
                    for (int c = 0; c < fontModel.Matrix.GetLength(2); c++) fontModel.Matrix[i, r, c] = Color.White;
            }

            fontModel.Height = (byte)targetHeight;
            fontModel.LowerCaseBottomSpc = (byte)diff_A_a;
            isUpdatingUi = true;
            numFontHeight.Value = targetHeight;
            numLowerSpc.Value = diff_A_a;
            isUpdatingUi = false;
            UpdateGridForSelectedChar();
            UpdateEditorView(0);
        }
        #endregion
    }
}
