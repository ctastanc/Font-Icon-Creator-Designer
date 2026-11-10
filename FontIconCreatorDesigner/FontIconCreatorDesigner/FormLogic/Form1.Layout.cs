using System;
using System.Drawing;
using System.Windows.Forms;

namespace FontIconCreatorDesigner
{
    public partial class Form1
    {
        #region Scrollable & Responsive Layout
        // Kontrollerdeki titremeyi (flicker) önlemek için dahili DoubleBuffered özelliğini etkinleştirir.
        private static void EnableDoubleBuffering(Control control)
        {
            if (control == null) return;
            typeof(Control)
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(control, true, null);
        }
        private void SetupScrollableLayout()
        {
            EnableDoubleBuffering(tlpGrid);
            EnableDoubleBuffering(pnlTopRuler);
            EnableDoubleBuffering(pnlLeftRuler);

            pnlTopRulerContainer.Location = new Point(0, 0);
            pnlTopRulerContainer.Size = new Size(TOTAL_COLS * (cellSize + 1), 25);
            EnableDoubleBuffering(pnlTopRulerContainer);

            pnlLeftRulerContainer.Location = new Point(0, 0);
            pnlLeftRulerContainer.Size = new Size(30, TOTAL_ROWS * (cellSize + 1));
            EnableDoubleBuffering(pnlLeftRulerContainer);
            EnableDoubleBuffering(pnlHeader);
            lblHeader.Location = new Point(34, 1);

            pnlTopRuler.Location = new Point(0, 0);
            pnlTopRuler.Height = 25;
            pnlTopRuler.Width = (TOTAL_COLS * (cellSize + 1)) + 10;
            pnlTopRulerContainer.Location = new Point(28 + 1, 2);

            pnlLeftRuler.Location = new Point(0, 0);
            pnlLeftRuler.Width = 30;
            pnlLeftRuler.Height = TOTAL_ROWS * (cellSize + 1);
            pnlLeftRulerContainer.Location = new Point(2, 28);

            pnlGridScroll.Location = new Point(0, 0); //44,90
            pnlGridScroll.Size = new Size(TOTAL_COLS * (cellSize + 1), TOTAL_ROWS * (cellSize + 1));
            pnlGridScroll.AutoScroll = true;
            pnlGridScroll.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            tlpGrid.Location = new Point(1, 1);

            lblRowValues.Location = new Point(4, 6);
            txtRowValues.Location = new Point(85, 7);

            rbFontMode.Location = new Point(0, 0);
            rbFontMode.Size = new Size(307, 31);
            rbIconMode.Location = new Point(307, 0);
            rbIconMode.Size = new Size(307, 31);
            pnlCommonControls.Size = new Size(613, 164);
            pnlFontControls.Size = new Size(613, 130);
            pnlCharBox.Location = new Point(7, 36);
            pnlCharBox.Size = new Size(599, 88);
            pnlFile.Size = new Size(613, 26);
            lblFile.Location = new Point(2, 6);

            // Kaydırma olaylarını cetvellerle eşitle
            pnlGridScroll.Scroll += (s, e) => SyncRulersWithScroll();
            tlpGrid.LocationChanged += (s, e) => SyncRulersWithScroll();

            // Grid ve kaydırma paneli üzerinde odak ve fare tekerleği desteği
            tlpGrid.MouseEnter += (s, e) => {
                if (!pnlGridScroll.Focused) pnlGridScroll.Focus();
            };
            pnlGridScroll.MouseEnter += (s, e) => {
                if (!pnlGridScroll.Focused) pnlGridScroll.Focus();
            };

            pnlGridScroll.MouseWheel += (s, e) => HandleGridMouseWheel(e);
            tlpGrid.MouseWheel += (s, e) => HandleGridMouseWheel(e);
            pnlTopRuler.MouseWheel += (s, e) => HandleGridMouseWheel(e);
            pnlLeftRuler.MouseWheel += (s, e) => HandleGridMouseWheel(e);

            rtxDataMonitor.MouseEnter += (s, e) => // rtxtDataMonitor fare tekerleği ve odak desteği (Ctrl+Wheel ile font büyütme/küçültme)
            {
                if (!rtxDataMonitor.Focused) rtxDataMonitor.Focus();
            };
            pnlDataMon.MouseEnter += (s, e) =>
            {
                if (!rtxDataMonitor.Focused) rtxDataMonitor.Focus();
            };
            rtxDataMonitor.MouseWheel += (s, e) => HandleRtxDataMonitorMouseWheel(e);
            this.Resize += (s, e) => PerformResponsiveLayout(); // Pencere yeniden boyutlandırma olayına bağla
        }

        // Grid kaydırıldığında üst ve sol cetvellerin konumunu kaydırma miktarıyla senkronize eder.
        private void SyncRulersWithScroll()
        {
            Point scrollPos = tlpGrid.Location;// : pnlGridScroll.AutoScrollPosition;
            pnlTopRuler.Location = new Point(scrollPos.X, 0);
            pnlTopRuler.Invalidate();
            pnlLeftRuler.Location = new Point(0, scrollPos.Y);
            pnlLeftRuler.Invalidate();
            tlpGrid.Invalidate();
            tlpGrid.Update();
            pnlHeader.Invalidate();
            pnlDataMon.Invalidate();
            pnlRowVal.Invalidate();
            pnlLeft.Invalidate();
        }

        // Pencere boyutu değiştiğinde sağ panellerin, durum çubuğunun ve kaydırılabilir gridin konumlarını düzenler.
        private void PerformResponsiveLayout()
        {
            if (isPerformingLayout) return;
            if (this.WindowState == FormWindowState.Minimized) return;
            isPerformingLayout = true;
            try
            {
                int leftMargin   = 30;
                int rightMargin  = 30;
                int toptMargin   = 80;
                int bottomMargin = 25;

                int pX = this.ClientRectangle.X + leftMargin;
                int pY = this.ClientRectangle.Y + toptMargin;
                int pW = this.ClientSize.Width  - rightMargin;
                int pH = this.ClientSize.Height - bottomMargin;
                
                if (pW < 500 || pH < 350) return;

                int rightPanelWidth = 614;
                int rightPanelX = Math.Max(150, pW - rightPanelWidth );
                // Header
                pnlHeader.Location = new Point(pX, 0);
                pnlHeader.Size = new Size(pW - leftMargin, 51);
                btnTheme.Location = new Point(pX+pnlHeader.Width-76, 27);
                // Sol kontrol panelleri_________________________________________________________
                pnlLeft.Location = new Point(pX, pY + 34);
                pnlLeft.Size = new Size(rightPanelX - pX - 5, pH - pY - 34 - 5);

                pnlRowVal.Location = new Point(28, pnlLeft.Height - 33);
                pnlRowVal.Size = new Size(pnlLeft.Width - 28 - 7, 27);
                txtRowValues.Width = pnlRowVal.Width - 92;

                pnlGridScroll.SetBounds(28, 28, pnlLeft.Width - 28 - 7, pnlLeft.Height - 28 - 5 - pnlRowVal.Height - 4);
                pnlTopRulerContainer.Size = new Size(pnlGridScroll.ClientSize.Width, 25);
                pnlLeftRulerContainer.Size = new Size(30, pnlGridScroll.ClientSize.Height);

                btnOpenSysFont.Location = new Point(pX, pY);
                btnOpenSysFont.Size = new Size(pnlLeft.Width/2 - 2, 30); //275
                btnOpenImage.Location = new Point(pX + (pnlLeft.Width/2 + 2), pY); //px+280
                btnOpenImage.Size = new Size(pnlLeft.Width/2 - 2, 30);
                // Sağ kontrol panelleri _____________________________________________________
                pnlMode.Location = new Point(rightPanelX, pY);
                pnlMode.Width = rightPanelWidth;

                pnlCommonControls.Location = new Point(rightPanelX, pnlMode.Bottom);
                pnlCommonControls.Width = rightPanelWidth;

                pnlFontControls.Location = new Point(rightPanelX, pnlCommonControls.Bottom + 5);
                pnlFontControls.Width = rightPanelWidth;

                int rtbTop = isFontMode ? (pnlFontControls.Bottom + 5) : (pnlCommonControls.Bottom + 5);
                int rtbHeight = Math.Max(80, pH - rtbTop);
                pnlDataMon.SetBounds(rightPanelX, rtbTop, rightPanelWidth, rtbHeight - pnlFile.Height-10);
                rtxDataMonitor.SetBounds(2, 2, pnlDataMon.Width-4, pnlDataMon.Height-6);

                pnlFile.Location = new Point(rightPanelX, pnlDataMon.Bottom + 5);
                pnlFile.Width = rightPanelWidth;

                SyncRulersWithScroll();
            }
            finally  { isPerformingLayout = false; }
        }
        
        private void UpdateRulerDimensions() // Karakter genişliği veya yüksekliği değiştiğinde cetvel boyutlarını günceller.
        {
            pnlTopRuler.Width = (tlpGrid.ColumnCount * (cellSize + 1)) + 10;
            pnlLeftRuler.Height = (tlpGrid.RowCount * (cellSize + 1));
            pnlTopRuler?.Invalidate();
            pnlLeftRuler?.Invalidate();
            pnlTopRulerContainer.Width = pnlGridScroll.ClientSize.Width;
            pnlLeftRulerContainer.Height = pnlGridScroll.ClientSize.Height;
            SyncRulersWithScroll();
        }

        /// Grid(Zoom) seviyesini dinamik olarak ayarlar.
        /// Hücre boyutu 1 ile 60 piksel arasÄ±nda değişebilir.
        /// Cetveller ve grid otomatik olarak orantılanır.
        public void SetCellSize(int newSize)
        {
            newSize = Math.Max(1, Math.Min(60, newSize));
            if (newSize == cellSize) return;
            int oldPitch = cellSize + 1;
            int newPitch = newSize + 1;
            int scrollX = Math.Abs(pnlGridScroll.AutoScrollPosition.X);
            int scrollY = Math.Abs(pnlGridScroll.AutoScrollPosition.Y);
            float ratio = (float)newPitch / oldPitch;
            cellSize = newSize;
            try { _ini.Write("AppConfig", "CellSize", cellSize.ToString()); } catch { }
            tlpGrid.SuspendLayout();
            while (tlpGrid.ColumnStyles.Count < tlpGrid.ColumnCount)
                tlpGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, cellSize));
            for (int c = 0; c < tlpGrid.ColumnCount; c++)
            {
                tlpGrid.ColumnStyles[c].SizeType = SizeType.Absolute;
                tlpGrid.ColumnStyles[c].Width = cellSize;
            }
            while (tlpGrid.RowStyles.Count < tlpGrid.RowCount)
                tlpGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, cellSize));
            for (int r = 0; r < tlpGrid.RowCount; r++)
            {
                tlpGrid.RowStyles[r].SizeType = SizeType.Absolute;
                tlpGrid.RowStyles[r].Height = cellSize;
            }
            tlpGrid.Width = tlpGrid.ColumnCount * (cellSize + 1);
            tlpGrid.Height = tlpGrid.RowCount * (cellSize + 1);
            tlpGrid.ResumeLayout(true);
            UpdateRulerDimensions();
            int targetScrollX = (int)Math.Round(scrollX * ratio);
            int targetScrollY = (int)Math.Round(scrollY * ratio);
            pnlGridScroll.AutoScrollPosition = new Point(targetScrollX, targetScrollY);
            SyncRulersWithScroll();
            tlpGrid.Invalidate();
            toolTip.Show($"Grid Zoom: {cellSize}px (Ctrl+Wheel)", tlpGrid, 15, 15, 600); // Kullanıcıya anlık yakınlaştırma seviyesini bildir
        }

        /// Fare tekerleği olaylarını yönetir:
        /// - Ctrl basılı ise: Hücre boyutunu büyütür veya 1'e kadar küçültür (Zoom).
        /// - Ctrl basılı değilse: Sayfayı normal dikey kaydırır.
        private void HandleGridMouseWheel(MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (e is HandledMouseEventArgs handled) handled.Handled = true;
                int deltaSteps = Math.Max(1, Math.Abs(e.Delta) / 120);
                if (e.Delta > 0)
                {
                    SetCellSize(cellSize + deltaSteps); // Ctrl + Mouse Wheel Up: Büyüt
                }
                else if (e.Delta < 0)
                     SetCellSize(cellSize - deltaSteps); // Ctrl + Mouse Wheel Down: 1'e kadar küçült
            }
            else
            {
                if (pnlGridScroll.VerticalScroll.Visible)
                {
                    int currentScroll = Math.Abs(pnlGridScroll.AutoScrollPosition.Y);
                    int change = (e.Delta / 120) * Math.Max(16, cellSize * 3);
                    int newScroll = Math.Max(0, currentScroll - change);
                    pnlGridScroll.AutoScrollPosition = new Point(Math.Abs(pnlGridScroll.AutoScrollPosition.X), newScroll);
                    SyncRulersWithScroll();
                }
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            Point mousePos = this.PointToClient(Cursor.Position);
            if (pnlLeft != null && pnlLeft.Bounds.Contains(mousePos))
            {
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    HandleGridMouseWheel(e);
                    if (e is HandledMouseEventArgs handled) handled.Handled = true;
                    return;
                }
            }
            else if (rtxDataMonitor != null && rtxDataMonitor.Visible &&
                     rtxDataMonitor.ClientRectangle.Contains(rtxDataMonitor.PointToClient(Cursor.Position)))
            {
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    HandleRtxDataMonitorMouseWheel(e);
                    if (e is HandledMouseEventArgs handled) handled.Handled = true;
                    return;
                }
            }
            base.OnMouseWheel(e);
        }

        /// rtxDataMonitor fare tekerleği olaylarını yönetir:
        /// - Ctrl basılı ise: Font boyutunu büyütür veya 5pt'ye kadar küçültür (Zoom).
        /// - Ctrl basılı değilse: Metin kutusu normal dikey kaydırma yapar.
        private void HandleRtxDataMonitorMouseWheel(MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (e is HandledMouseEventArgs handled) handled.Handled = true;
                float currentSize = rtxDataMonitor.Font.Size;
                int deltaSteps = Math.Max(1, Math.Abs(e.Delta) / 120);
                float step = 1.0f * deltaSteps;
                float newSize = (e.Delta > 0)
                    ? Math.Min(36.0f, currentSize + step)
                    : Math.Max(5.0f, currentSize - step);
                if (Math.Abs(newSize - currentSize) > 0.01f) ChangeRtxFontSize(newSize);
            }
        }

        public void ChangeRtxFontSize(float newSize)
        {
            try
            {
                newSize = (float)Math.Round(newSize, 2);
                newSize = Math.Max(5.0f, Math.Min(36.0f, newSize));
                Font newFont = new Font(rtxDataMonitor.Font.FontFamily, newSize, rtxDataMonitor.Font.Style);
                rtxDataMonitor.Font = newFont;
                rtxDataMonitor.ZoomFactor = 1.0f;
                int selStart = rtxDataMonitor.SelectionStart;
                int selLength = rtxDataMonitor.SelectionLength;
                rtxDataMonitor.SelectAll();
                rtxDataMonitor.SelectionFont = newFont;
                rtxDataMonitor.Select(selStart, selLength);
                try
                {
                    _ini.Write("AppConfig", "RtxFontSize", newSize.ToString(System.Globalization.CultureInfo.InvariantCulture));
                }
                catch { }
                toolTip.Show($"Text Font: {newSize:0.##}pt (Ctrl+Wheel)", rtxDataMonitor, 15, 15, 600);
            }
            catch { }
        }
        #endregion
        
    }
}