using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FontIconCreatorDesigner
{
    public partial class Form1
    {
        #region Paint Events
        private Color borderColor = Color.DodgerBlue; // ThemeEngine.PanelBorderColor; //ThemeEngine.AccentColor;
        private void InitilizePaints()
        {
            pnlHeader.Paint += pnlHeader_Paint;
            pnlTopRuler.Paint += pnlTopRuler_Paint;
            pnlLeftRuler.Paint += pnlLeftRuler_Paint;
            string[] Panels = { "pnlLeft", "pnlCommonControls", "pnlFontControls", "pnlDataMon", "pnlFile" };
            foreach (Control c in this.Controls)
                if (Panels.Contains(c.Name)) c.Paint +=common_Paint;
        }

        private void common_Paint(object sender, PaintEventArgs e)
        {
            Control c = (Control)sender;
            int borderWidth = 2;
            ControlPaint.DrawBorder(
                e.Graphics, c.ClientRectangle,
                borderColor, borderWidth, ButtonBorderStyle.Solid, // Sol
                borderColor, borderWidth, ButtonBorderStyle.Solid, // Üst
                borderColor, borderWidth, ButtonBorderStyle.Solid, // Sağ
                borderColor, borderWidth, ButtonBorderStyle.Solid  // Alt
            );
        }

        private void pnlHeader_Paint(object sender, PaintEventArgs e)
        {
            Color borderColor1 = ThemeEngine.HeaderBorderColor; //Color.Silver;
            int borderWidth = 2;
            ControlPaint.DrawBorder(
                e.Graphics, pnlHeader.ClientRectangle,
                borderColor1, borderWidth, ButtonBorderStyle.None, // Sol
                borderColor1, borderWidth, ButtonBorderStyle.None, // Üst
                borderColor1, borderWidth, ButtonBorderStyle.None, // Sağ
                borderColor1, borderWidth, ButtonBorderStyle.Solid  // Alt
            );
        }

        private void pnlTopRuler_Paint(object sender, PaintEventArgs e)
        {
            using Font font = new("Segoe UI", 7.0f, FontStyle.Regular);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int rulerHeight = pnlTopRuler.Height;
            int maxCols = tlpGrid.ColumnCount;
            int cellPitch = cellSize + 1;
            int fullWidth = maxCols * cellPitch;
            using (var bgBrush = new SolidBrush(ThemeEngine.BackgroundColor)) // Arka planı temizle
                g.FillRectangle(bgBrush, pnlTopRuler.ClientRectangle);
            int colStep = 8;
            if (colStep * cellPitch < 26) // Metinlerin üst üste binmemesi için dinamik sütun adımı (en az 26px mesafe)
            {
                if (16 * cellPitch >= 26) colStep = 16;
                else if (32 * cellPitch >= 26) colStep = 32;
                else colStep = 64;
            }
            int medStep = colStep / 2;
            int minStep = (cellPitch >= 8) ? 1 : (cellPitch >= 4 ? Math.Max(1, medStep / 2) : 0);
            Brush textBrush = ThemeEngine.RulerTextBrush;
            using Pen rPen = new(ThemeEngine.TextColor, 1);
            for (int col = 0; col <= maxCols; col++)
            {
                int xPos = col * cellPitch;
                if (col % colStep == 0)
                {
                    g.DrawLine(rPen, xPos, rulerHeight - 10, xPos, rulerHeight - 1);
                    string text = col.ToString();
                    SizeF textSize = g.MeasureString(text, font);
                    float textX = (col == 0) ? xPos + 1 : xPos - (textSize.Width / 2f);
                    g.DrawString(text, font, textBrush, textX, 2);
                }
                else if (medStep > 0 && col % medStep == 0)
                    g.DrawLine(rPen, xPos, rulerHeight - 6, xPos, rulerHeight - 1);
                else if (minStep > 0 && col % minStep == 0)
                    g.DrawLine(rPen, xPos, rulerHeight - 4, xPos, rulerHeight - 1);
            }
        }

        private void pnlLeftRuler_Paint(object sender, PaintEventArgs e)
        {
            using Font font = new("Segoe UI", 7.0f, FontStyle.Regular);
            Control panel = (Control)sender;
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (var bgBrush = new SolidBrush(ThemeEngine.BackgroundColor))
                g.FillRectangle(bgBrush, panel.ClientRectangle);
            int panelWidth = panel.Width;
            int cellPitch = cellSize + 1;
            int maxRows = tlpGrid.RowCount;
            int step = 1;
            if (cellPitch >= 14) step = 1; // Dikeyde metin yüksekliği ~13px. Metinlerin çakışmaması için dinamik adım:
            else if (cellPitch * 2 >= 14) step = 2;
            else if (cellPitch * 5 >= 14) step = 5;
            else step = 10; // Grid 1 olduğunda cellPitch=2, 10'ar 10'ar artar (20px aralık)
            Brush textBrush = ThemeEngine.RulerTextBrush;
            using Pen rPen = new(ThemeEngine.TextColor, 1);
            StringFormat sf = new()
            {
                Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center
            };
            for (int row = 1; row <= maxRows; row++)
            {
                int yTop = (row - 1) * cellPitch;
                int yCenter = yTop + (cellPitch / 2);
                if (row == 1 || row % step == 0)
                {
                    g.DrawLine(rPen, panelWidth - 6, yCenter, panelWidth - 1, yCenter);
                    RectangleF textRect = new(0, yCenter - 7, panelWidth - 8, 14);
                    g.DrawString(row.ToString(), font, textBrush, textRect, sf);
                }
                else if (step == 10 && row % 5 == 0)
                    g.DrawLine(rPen, panelWidth - 4, yCenter, panelWidth - 1, yCenter);
                else if (step == 5 && cellPitch >= 3)
                    g.DrawLine(rPen, panelWidth - 3, yCenter, panelWidth - 1, yCenter);
                else if (step == 2 && cellPitch >= 6)
                    g.DrawLine(rPen, panelWidth - 3, yCenter, panelWidth - 1, yCenter);
            }
        }
        #endregion
    }
}