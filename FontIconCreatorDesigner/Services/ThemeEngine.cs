using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace font_creator
{
    internal class ControlSnapshot
    {
        public Color BackColor;
        public Color ForeColor;
        public Font Font;
        public FlatStyle FlatStyle;
        public bool UseVisualStyleBackColor;
        public Color BorderColor;
        public int BorderSize;
        public Color MouseOverBackColor;
        public Color MouseDownBackColor;
        public Color CheckedBackColor;
    }

    public static class ThemeEngine
    {
        #region 1. RENK PALETI
        public static bool IsDarkMode { get; private set; }
        public static readonly Color AccentColor = Color.DodgerBlue;
        // Koyu Mod Sabitleri
        public static readonly Color DarkBackground = Color.FromArgb(30, 30, 30);
        public static readonly Color DarkInputBox = Color.FromArgb(24, 24, 24);
        public static readonly Color DarkButton = Color.FromArgb(45, 45, 48);
        public static readonly Color DarkButtonHover =  Color.FromArgb(62, 62, 66);
        public static readonly Color DarkButtonPressed = Color.FromArgb(40, 40, 40);
        public static readonly Color DarkButtonBorder = Color.Gray;
        // Dinamik Degerler
        public static Color BackgroundColor => IsDarkMode ? DarkBackground : SystemColors.Control;
        public static Color TextColor => IsDarkMode ? Color.White : Color.Black;
        public static Color ButtonPressedColor => IsDarkMode ? DarkButtonPressed : Color.LightSteelBlue;
        public static Color ButtonBorderColor => IsDarkMode ? DarkButtonBorder : Color.Silver;
        public static readonly Color DarkBorder = Color.DimGray;
        public static Color HeaderBorderColor => IsDarkMode ? DarkBorder : Color.Silver;
        public static Color InputBackgroundColor => IsDarkMode ? DarkInputBox : Color.White;
        // Karakter Paleti
        public static readonly Font PaletteRegularFont = new Font("Segoe UI", 8.5f, FontStyle.Regular);
        public static readonly Font PaletteBoldFont = new Font("Segoe UI", 9f, FontStyle.Bold);
        public static Color PaletteButtonBg => IsDarkMode ? DarkButton : Color.White;
        public static Color PaletteButtonHover => IsDarkMode ? DarkButtonHover : Color.FromArgb(224, 238, 255);
        public static Color PaletteSelectedBg => AccentColor;
        public static Color PaletteSelectedHover => ControlPaint.Light(AccentColor);
        // Cetvel
        public static Brush RulerTextBrush => IsDarkMode ? Brushes.White : Brushes.Black;
        public static event Action ThemeChanged;
        #endregion

        #region 2. WIN32 DWM VE UXTHEME
        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        public static void ApplyTitleBar(Form form)
        {
            if (form == null || !form.IsHandleCreated) return;
            try
            {
                IntPtr handle = form.Handle;
                int useImmersive = IsDarkMode ? 1 : 0;
                int hr = DwmSetWindowAttribute(handle, 20, ref useImmersive, sizeof(int));
                if (hr != 0) DwmSetWindowAttribute(handle, 19, ref useImmersive, sizeof(int));
                int captionColor = IsDarkMode ? 0x001E1E1E : unchecked((int)0xFFFFFFFF);
                int textColor    = IsDarkMode ? 0x00FFFFFF : unchecked((int)0xFFFFFFFF);
                int borderColor  = IsDarkMode ? 0x00463F3F : unchecked((int)0xFFFFFFFF);
                DwmSetWindowAttribute(handle, 35, ref captionColor, sizeof(int));
                DwmSetWindowAttribute(handle, 36, ref textColor, sizeof(int));
                DwmSetWindowAttribute(handle, 34, ref borderColor, sizeof(int));
            }
            catch { }
        }

        public static void ApplyScrollbar(Control ctrl)
        {
            if (ctrl == null) return;
            Action apply = () =>
            {
                if (!ctrl.IsHandleCreated) return;
                try
                {
                    string subApp = IsDarkMode ? "DarkMode_Explorer" : "Explorer";
                    SetWindowTheme(ctrl.Handle, subApp, null);
                }
                catch { }
            };
            if (ctrl.IsHandleCreated) apply();
            else ctrl.HandleCreated += (s, e) => apply();
        }
        #endregion

        #region 3. BASLATMA VE TEMA DEGISTIRME
        static ThemeEngine() => Initialize();

        public static void Initialize()
        {
            try
            {
                var ini = new IniFile();
                string saved = ini.Read("AppConfig", "Theme", "Light");
                IsDarkMode = string.Equals(saved, "Dark", StringComparison.OrdinalIgnoreCase);
            }
            catch { IsDarkMode = false; }
        }

        public static void SetTheme(bool darkMode)
        {
            if (IsDarkMode == darkMode) return;
            IsDarkMode = darkMode;
            try
            {
                new IniFile().Write("AppConfig", "Theme", IsDarkMode ? "Dark" : "Light");
            }
            catch { }
            RefreshAllRegisteredForms();
            ThemeChanged?.Invoke();
        }
        #endregion

        #region 4. FORM VE KONTROL KAYDI
        private static readonly List<WeakReference<Form>> _registeredForms = [];
        private static readonly Dictionary<Control, ControlSnapshot> _snapshots = [];

        public static void SnapshotControlTree(Control root)
        {
            if (!_snapshots.ContainsKey(root))
            {
                var snap = new ControlSnapshot
                {
                    BackColor = root.BackColor,
                    ForeColor = root.ForeColor,
                    Font = root.Font
                };
                if (root is ButtonBase b)
                {
                    snap.FlatStyle = b.FlatStyle;
                    snap.UseVisualStyleBackColor = b.UseVisualStyleBackColor;
                    snap.BorderColor = b.FlatAppearance.BorderColor;
                    snap.BorderSize = b.FlatAppearance.BorderSize;
                    snap.MouseOverBackColor = b.FlatAppearance.MouseOverBackColor;
                    snap.MouseDownBackColor = b.FlatAppearance.MouseDownBackColor;
                    snap.CheckedBackColor = b.FlatAppearance.CheckedBackColor;
                }
                _snapshots[root] = snap;
            }
            foreach (Control child in root.Controls) SnapshotControlTree(child);
        }

        public static void Register(Form form)
        {
            if (form == null) return;
            _registeredForms.Add(new WeakReference<Form>(form));
            form.HandleCreated += (s, e) => ApplyTitleBar(form);
            form.Shown += (s, e) => ApplyTitleBar(form);
            SnapshotControlTree(form);
            ApplyToForm(form);
            WatchContainerRecursively(form);
        }

        public static void RefreshAllRegisteredForms()
        {
            for (int i = _registeredForms.Count - 1; i >= 0; i--)
            {
                if (_registeredForms[i].TryGetTarget(out Form form) && !form.IsDisposed)
                {
                    if (form.InvokeRequired) form.BeginInvoke(new Action(() => ApplyToForm(form)));
                    else ApplyToForm(form);
                }
                else _registeredForms.RemoveAt(i);
            }
        }

        public static void ApplyToForm(Form form)
        {
            if (form == null || form.IsDisposed) return;
            ApplyTitleBar(form);
            form.BackColor = BackgroundColor;
            form.ForeColor = TextColor;
            ApplyToControlTree(form);
            form.Invalidate(true);
            form.Refresh();
        }

        public static void WatchContainerRecursively(Control container)
        {
            container.ControlAdded -= Container_ControlAdded;
            container.ControlAdded += Container_ControlAdded;
            foreach (Control child in container.Controls)
            {
                if (child.HasChildren && child is not NumericUpDown) WatchContainerRecursively(child);
            }
        }

        private static void Container_ControlAdded(object sender, ControlEventArgs e)
        {
            SnapshotControlTree(e.Control);
            ApplyToControl(e.Control);
            if (e.Control.HasChildren && e.Control is not NumericUpDown) WatchContainerRecursively(e.Control);
        }

        public static void ApplyToControlTree(Control root)
        {
            if (root == null || root.Name == "pnlCharBox") return;
            foreach (Control child in root.Controls)
            {
                ApplyToControl(child);
                if (child.HasChildren && child is not NumericUpDown) ApplyToControlTree(child);
            }
        }
        #endregion

        #region 5. KONTROL TEMALANDIRMA (GENEL VE TASARIM UYUMLU)
        public static void ApplyToControl(Control ctrl)
        {
            if (ctrl == null || ctrl.IsDisposed) return;
            if (!_snapshots.TryGetValue(ctrl, out ControlSnapshot snap))
            {
                SnapshotControlTree(ctrl);
                _snapshots.TryGetValue(ctrl, out snap);
            }
            if (ctrl is Panel panel)
            {
                ApplyScrollbar(panel);
                panel.BackColor = IsDarkMode ? DarkBackground : (snap != null ? snap.BackColor : BackgroundColor);
                panel.ForeColor = IsDarkMode ? TextColor : (snap != null ? snap.ForeColor : TextColor);
            }
            else if (ctrl is RadioButton rb)
            {
                if (rb.Appearance == Appearance.Button)
                {
                    SyncButton(rb);
                    rb.CheckedChanged -= RadioButton_CheckedChanged;
                    rb.CheckedChanged += RadioButton_CheckedChanged;
                }
                else rb.ForeColor = IsDarkMode ? TextColor : (snap != null ? snap.ForeColor : Color.Black);
            }
            else if (ctrl is CheckBox cb && cb.Appearance != Appearance.Button)
            {
                cb.ForeColor = IsDarkMode ? TextColor : (snap != null ? snap.ForeColor : Color.Black);
            }
            else if (ctrl is ButtonBase btn)
            {
                btn.MouseUp -= Button_FocusEvents;
                btn.MouseUp += Button_FocusEvents;
                btn.Click -= Button_FocusEvents;
                btn.Click += Button_FocusEvents;
                btn.KeyUp -= Button_FocusEvents;
                btn.KeyUp += Button_FocusEvents;
                btn.TextChanged -= Button_SyncEvents;
                btn.TextChanged += Button_SyncEvents;
                btn.BackColorChanged -= Button_SyncEvents;
                btn.BackColorChanged += Button_SyncEvents;
                btn.ForeColorChanged -= Button_SyncEvents;
                btn.ForeColorChanged += Button_SyncEvents;
                btn.Paint -= Button_PaintHandler;
                btn.Paint += Button_PaintHandler;
                btn.EnabledChanged -= Button_EnabledChanged;
                btn.EnabledChanged += Button_EnabledChanged;

                if (btn is CheckBox chk)
                {
                    chk.CheckedChanged -= Button_SyncEvents;
                    chk.CheckedChanged += Button_SyncEvents;
                }
                SyncButton(btn);
            }
            else if (ctrl is TextBox txt)
            {
                txt.BackColor = IsDarkMode ? InputBackgroundColor : (snap != null ? snap.BackColor : Color.White);
                txt.ForeColor = IsDarkMode ? TextColor : (snap != null ? snap.ForeColor : Color.Black);
            }
            else if (ctrl is RichTextBox rtx)
            {
                rtx.BackColor = IsDarkMode ? InputBackgroundColor : (snap != null ? snap.BackColor : Color.White);
                rtx.ForeColor = IsDarkMode ? TextColor : (snap != null ? snap.ForeColor : Color.Black);
                ApplyScrollbar(rtx);
            }
            else if (ctrl is NumericUpDown num)
            {
                num.BackColor = IsDarkMode ? InputBackgroundColor : (snap != null ? snap.BackColor : Color.White);
                num.ForeColor = IsDarkMode ? TextColor : (snap != null ? snap.ForeColor : Color.Black);
            }
            else if (ctrl is ComboBox cmb)
            {
                cmb.BackColor = IsDarkMode ? InputBackgroundColor : (snap != null ? snap.BackColor : Color.White);
                cmb.ForeColor = IsDarkMode ? TextColor : (snap != null ? snap.ForeColor : Color.Black);
                cmb.FlatStyle = IsDarkMode ? FlatStyle.Flat : (snap != null ? snap.FlatStyle : FlatStyle.Standard);
            }
            else if (ctrl is GroupBox grp)
            {
                grp.BackColor = IsDarkMode ? DarkBackground : (snap != null ? snap.BackColor : BackgroundColor);
                grp.ForeColor = IsDarkMode ? TextColor : (snap != null ? snap.ForeColor : Color.Black);
            }
            else if (ctrl is Label lbl)
            {
                lbl.ForeColor = IsDarkMode ? TextColor : (snap != null ? snap.ForeColor : Color.Black);
                lbl.Paint -= Generic_PaintDisabled;
                lbl.Paint += Generic_PaintDisabled;
            }
        }

        private static bool _isSyncing = false;

        internal static void SyncButtonHelper(ButtonBase btn, ControlSnapshot snap)
        {
            btn.BackColor = snap.BackColor;
            btn.ForeColor = snap.ForeColor;
            btn.FlatStyle = snap.FlatStyle;
            btn.FlatAppearance.BorderSize = snap.BorderSize;
            btn.FlatAppearance.BorderColor = snap.BorderColor;
            btn.FlatAppearance.CheckedBackColor = snap.CheckedBackColor;
            btn.FlatAppearance.MouseOverBackColor = snap.MouseOverBackColor;
            btn.FlatAppearance.MouseDownBackColor = snap.MouseDownBackColor;
            btn.Font = snap.Font;
        }

        internal static void SyncButtonHelper2(ButtonBase btn, ControlSnapshot snap, int size)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = size;
            btn.FlatAppearance.BorderColor = DarkButtonBorder;
            btn.BackColor = DarkButton;
            btn.ForeColor = TextColor;
            btn.Font = snap?.Font ?? btn.Font;
            btn.FlatAppearance.MouseOverBackColor = DarkButtonHover;
            btn.FlatAppearance.MouseDownBackColor = DarkButtonPressed;
        }

        internal static void RestoreLightButton(ButtonBase btn, ControlSnapshot snap)
        {
            if (snap == null)
            {
                btn.ResetBackColor();
                btn.ResetForeColor();
                btn.UseVisualStyleBackColor = true;
                btn.FlatStyle = FlatStyle.Standard;
                return;
            }
            SyncButtonHelper(btn, snap); // 1. Snapshot'taki tüm özellikleri (MouseOverBackColor, MouseDownBackColor,  BorderColor, BorderSize, FlatStyle, Font) istisnasız geri yükle
            if (snap.UseVisualStyleBackColor) // 2. Sistem temalı butonlar için görsel stil ve renk sıfırlaması
            {
                btn.ResetBackColor();
                btn.ResetForeColor();
                btn.UseVisualStyleBackColor = true;
            }
            else btn.UseVisualStyleBackColor = false;
        }

        public static void SyncButton(ButtonBase btn)
        {
            if (_isSyncing || btn == null || btn.IsDisposed) return;
            _snapshots.TryGetValue(btn, out ControlSnapshot snap);
            try
            {
                _isSyncing = true;
                // Buton görünümlü toggle kontrolü (Appearance.Button olan RadioButton veya CheckBox)
                bool isToggleButton = (btn is RadioButton rb && rb.Appearance == Appearance.Button)
                                   || (btn is CheckBox cb && cb.Appearance == Appearance.Button);
                int borderSize = (btn is RadioButton) ? 0 : 1;

                if (isToggleButton)
                {
                    bool isChecked = (btn is RadioButton r && r.Checked) || (btn is CheckBox c && c.Checked);
                    if (isChecked)
                    {
                        // 1. SEÇİLİ (CHECKED) TOGGLE BUTON DURUMU
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.FlatAppearance.BorderSize = borderSize;
                        Color activeColor = IsDarkMode
                            ? AccentColor : ((snap != null && snap.CheckedBackColor != Color.Empty && snap.CheckedBackColor != Color.Transparent)
                                ? snap.CheckedBackColor : AccentColor);

                        btn.BackColor = activeColor;
                        btn.ForeColor = Color.White;
                        btn.Font = snap?.Font != null ? new Font(snap.Font, FontStyle.Bold) : new Font(btn.Font, FontStyle.Bold);
                        btn.FlatAppearance.BorderColor = ControlPaint.Dark(activeColor);
                        btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(activeColor);
                        btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(activeColor);
                    }
                    else
                    {
                        // 2. SEÇİLİ OLMAYAN (UNCHECKED) TOGGLE BUTON DURUMU
                        if (IsDarkMode) SyncButtonHelper2(btn, snap, borderSize);
                        else RestoreLightButton(btn, snap);
                    }
                }
                else
                {
                    // 3. NORMAL STANDART BUTON (BUTTON)
                    bool isCustomActive = !btn.UseVisualStyleBackColor
                        && btn.BackColor != DarkButton
                        && (snap == null || btn.BackColor != snap.BackColor)
                        && btn.BackColor != SystemColors.Control;
                    if (isCustomActive)
                    {
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.FlatAppearance.BorderSize = 1;
                        btn.FlatAppearance.BorderColor = ControlPaint.Dark(btn.BackColor);
                        btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(btn.BackColor);
                        btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(btn.BackColor);
                        btn.ForeColor = (btn.BackColor.GetBrightness() < 0.65f) ? Color.White : Color.Black;
                    }
                    else
                    {
                        if (IsDarkMode) SyncButtonHelper2(btn, snap, 1);
                        else RestoreLightButton(btn, snap);
                    }
                }
                if (btn is CheckBox chk) chk.FlatAppearance.CheckedBackColor = btn.BackColor;
            }
            finally { _isSyncing = false; }
        }

        private static void Button_FocusEvents(object sender, EventArgs e)
        {
            if (sender is ButtonBase btn)
            {
                SyncButton(btn);
                Form f = btn.FindForm();
                if (f != null && !f.IsDisposed) { f.ActiveControl = null; }
            }
        }

        private static void Button_SyncEvents(object sender, EventArgs e)
        {
            if (sender is ButtonBase btn) SyncButton(btn);
        }

        private static void Button_PaintHandler(object sender, PaintEventArgs e)
        {
            if (sender is ButtonBase btn)
            {
                Generic_PaintDisabled(btn, e);
                CheckAndSyncIfNeeded(btn);
            }
        }

        private static void CheckAndSyncIfNeeded(ButtonBase btn)
        {
            if (_isSyncing || btn == null || btn.IsDisposed) return;
            _snapshots.TryGetValue(btn, out ControlSnapshot snap);
            if (IsDarkMode)
            {
                if (btn.UseVisualStyleBackColor) SyncButton(btn);
            }
            else
            {
                if (snap != null && snap.UseVisualStyleBackColor && btn.UseVisualStyleBackColor && btn.FlatStyle != snap.FlatStyle)
                    SyncButton(btn);
            }
        }

        private static void Button_EnabledChanged(object sender, EventArgs e)
        {
            if (sender is Control ctrl) ctrl.Invalidate();
        }
        #endregion

        #region 5.1 GENEL DISABLED PAINT (TUM KONTROLLER ICIN)
        private static TextFormatFlags GetAlignmentFlags(ContentAlignment align)
        {
            switch (align)
            {
                case ContentAlignment.TopLeft: return TextFormatFlags.Top | TextFormatFlags.Left;
                case ContentAlignment.TopCenter: return TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
                case ContentAlignment.TopRight: return TextFormatFlags.Top | TextFormatFlags.Right;
                case ContentAlignment.MiddleLeft: return TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
                case ContentAlignment.MiddleCenter: return TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                case ContentAlignment.MiddleRight: return TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
                case ContentAlignment.BottomLeft: return TextFormatFlags.Bottom | TextFormatFlags.Left;
                case ContentAlignment.BottomCenter: return TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
                case ContentAlignment.BottomRight: return TextFormatFlags.Bottom | TextFormatFlags.Right;
                default: return TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
            }
        }

        private static void Generic_PaintDisabled(object sender, PaintEventArgs e)
        {
            if (sender is Control ctrl && !ctrl.Enabled && IsDarkMode)
            {
                Rectangle textRect = ctrl.ClientRectangle;
                if (ctrl is ButtonBase btn)
                {
                    using (var brush = new SolidBrush(DarkButton)) e.Graphics.FillRectangle(brush, textRect);
                    using (var pen = new Pen(ButtonBorderColor)) e.Graphics.DrawRectangle(pen, 0, 0, btn.Width - 1, btn.Height - 1);
                    textRect.Inflate(0, 0);
                }
                else
                {
                    Color bg = (ctrl.BackColor == Color.Transparent || ctrl.BackColor.A == 0)
                        ? (ctrl.Parent?.BackColor ?? DarkBackground) : ctrl.BackColor;
                    e.Graphics.Clear(bg);
                }
                ContentAlignment align = (ctrl is ButtonBase b) ? b.TextAlign : (ctrl is Label l) ? l.TextAlign : ContentAlignment.MiddleCenter;
                bool autoEllipsis = (ctrl is ButtonBase b1 && b1.AutoEllipsis) || (ctrl is Label l1 && l1.AutoEllipsis);
                TextFormatFlags flags = GetAlignmentFlags(align) | TextFormatFlags.WordBreak;
                if (autoEllipsis) flags |= TextFormatFlags.EndEllipsis;
                TextRenderer.DrawText(e.Graphics, ctrl.Text, ctrl.Font, textRect, Color.Gray, flags);
            }
        }
        #endregion

        #region 6. RADYO BUTONLARI
        private static void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton rb && rb.Parent != null)
                foreach (Control sibling in rb.Parent.Controls)
                    if (sibling is RadioButton sibRb && sibRb.Appearance == Appearance.Button) SyncButton(sibRb);
        }
        #endregion

        #region 7. KARAKTER PALETI BUTON YONETIMI
        public static void StylePaletteButton(Button btn, bool isSelected)
        {
            if (isSelected)
            {
                btn.BackColor = PaletteSelectedBg;
                btn.ForeColor = Color.White;
                btn.Font = PaletteBoldFont;
                btn.FlatAppearance.BorderColor = ControlPaint.Dark(PaletteSelectedBg);
                btn.FlatAppearance.MouseOverBackColor = PaletteSelectedHover;
                btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(PaletteSelectedBg);
            }
            else
            {
                btn.BackColor = PaletteButtonBg;
                btn.ForeColor = TextColor;
                btn.Font = PaletteRegularFont;
                btn.FlatAppearance.BorderColor = ButtonBorderColor;
                btn.FlatAppearance.MouseOverBackColor = PaletteButtonHover;
                btn.FlatAppearance.MouseDownBackColor = ButtonPressedColor;
            }
        }
        #endregion
    }
}
