namespace FontIconCreatorDesigner
{
    partial class Form1
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod
        
        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.tlpGrid = new System.Windows.Forms.TableLayoutPanel();
            this.rtxDataMonitor = new System.Windows.Forms.RichTextBox();
            this.btnRevertDefault = new System.Windows.Forms.Button();
            this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SaveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.pnlTopRuler = new System.Windows.Forms.Panel();
            this.pnlLeftRuler = new System.Windows.Forms.Panel();
            this.txtRowValues = new System.Windows.Forms.TextBox();
            this.lblRowValues = new System.Windows.Forms.Label();
            this.lblFile = new System.Windows.Forms.Label();
            this.pnlCommonControls = new System.Windows.Forms.Panel();
            this.btnSyncUC = new System.Windows.Forms.Button();
            this.txtFontIconName = new System.Windows.Forms.TextBox();
            this.btnTrimTop = new System.Windows.Forms.Button();
            this.lblCharWidth = new System.Windows.Forms.Label();
            this.btnSelectMove = new System.Windows.Forms.Button();
            this.numCharWidth = new System.Windows.Forms.NumericUpDown();
            this.lblFontName = new System.Windows.Forms.Label();
            this.cbCharEdit = new System.Windows.Forms.CheckBox();
            this.numLowerSpc = new System.Windows.Forms.NumericUpDown();
            this.lblLowerSpc = new System.Windows.Forms.Label();
            this.btnOpen = new System.Windows.Forms.Button();
            this.numSpaceChar = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblSpace = new System.Windows.Forms.Label();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.lblHeight = new System.Windows.Forms.Label();
            this.btnClearText = new System.Windows.Forms.Button();
            this.numFontHeight = new System.Windows.Forms.NumericUpDown();
            this.btnClipboard = new System.Windows.Forms.Button();
            this.grpWith = new System.Windows.Forms.GroupBox();
            this.rbFillBlack = new System.Windows.Forms.RadioButton();
            this.rbFillWhite = new System.Windows.Forms.RadioButton();
            this.btnFillAll = new System.Windows.Forms.Button();
            this.btnInverseChar = new System.Windows.Forms.Button();
            this.btnCopyChar = new System.Windows.Forms.Button();
            this.btnPasteChar = new System.Windows.Forms.Button();
            this.grpFormat = new System.Windows.Forms.GroupBox();
            this.rbBinaryData = new System.Windows.Forms.RadioButton();
            this.rbHexdecimalData = new System.Windows.Forms.RadioButton();
            this.rbDecimalData = new System.Windows.Forms.RadioButton();
            this.pnlFontControls = new System.Windows.Forms.Panel();
            this.rbViewCurrentChar = new System.Windows.Forms.RadioButton();
            this.rbViewFullHeader = new System.Windows.Forms.RadioButton();
            this.lblCharSelect = new System.Windows.Forms.Label();
            this.btnPrevChar = new System.Windows.Forms.Button();
            this.cmbCharacters = new System.Windows.Forms.ComboBox();
            this.btnNextChar = new System.Windows.Forms.Button();
            this.pnlCharBox = new System.Windows.Forms.Panel();
            this.lblHeader = new System.Windows.Forms.Label();
            this.rbFontMode = new System.Windows.Forms.RadioButton();
            this.rbIconMode = new System.Windows.Forms.RadioButton();
            this.pnlMode = new System.Windows.Forms.Panel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pnlDataMon = new System.Windows.Forms.Panel();
            this.pnlFile = new System.Windows.Forms.Panel();
            this.pnlRowVal = new System.Windows.Forms.Panel();
            this.pnlTopRulerContainer = new System.Windows.Forms.Panel();
            this.pnlLeftRulerContainer = new System.Windows.Forms.Panel();
            this.pnlGridScroll = new System.Windows.Forms.Panel();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.btnOpenSysFont = new System.Windows.Forms.Button();
            this.btnOpenImage = new System.Windows.Forms.Button();
            this.btnTheme = new System.Windows.Forms.Button();
            this.pnlCommonControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCharWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLowerSpc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpaceChar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFontHeight)).BeginInit();
            this.grpWith.SuspendLayout();
            this.grpFormat.SuspendLayout();
            this.pnlFontControls.SuspendLayout();
            this.pnlMode.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlDataMon.SuspendLayout();
            this.pnlFile.SuspendLayout();
            this.pnlRowVal.SuspendLayout();
            this.pnlTopRulerContainer.SuspendLayout();
            this.pnlLeftRulerContainer.SuspendLayout();
            this.pnlGridScroll.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpGrid
            // 
            this.tlpGrid.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlpGrid.ColumnCount = 1;
            this.tlpGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1354F));
            this.tlpGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpGrid.Location = new System.Drawing.Point(8, 7);
            this.tlpGrid.Name = "tlpGrid";
            this.tlpGrid.RowCount = 1;
            this.tlpGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1283F));
            this.tlpGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1283F));
            this.tlpGrid.Size = new System.Drawing.Size(221, 137);
            this.tlpGrid.TabIndex = 0;
            this.tlpGrid.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.tlpGrid_CellPaint);
            this.tlpGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tlpGrid_MouseDown);
            this.tlpGrid.MouseLeave += new System.EventHandler(this.tlpGrid_MouseLeave);
            this.tlpGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tlpGrid_MouseMove);
            this.tlpGrid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tlpGrid_MouseUp);
            // 
            // rtxDataMonitor
            // 
            this.rtxDataMonitor.BackColor = System.Drawing.SystemColors.Control;
            this.rtxDataMonitor.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxDataMonitor.Location = new System.Drawing.Point(6, 7);
            this.rtxDataMonitor.Name = "rtxDataMonitor";
            this.rtxDataMonitor.Size = new System.Drawing.Size(599, 103);
            this.rtxDataMonitor.TabIndex = 33;
            this.rtxDataMonitor.Text = "";
            this.rtxDataMonitor.WordWrap = false;
            // 
            // btnRevertDefault
            // 
            this.btnRevertDefault.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnRevertDefault.Location = new System.Drawing.Point(6, 49);
            this.btnRevertDefault.Name = "btnRevertDefault";
            this.btnRevertDefault.Size = new System.Drawing.Size(57, 64);
            this.btnRevertDefault.TabIndex = 8;
            this.btnRevertDefault.Text = "Revert Default Char";
            this.btnRevertDefault.UseVisualStyleBackColor = true;
            this.btnRevertDefault.Click += new System.EventHandler(this.btnRevertDefault_Click);
            // 
            // pnlTopRuler
            // 
            this.pnlTopRuler.Location = new System.Drawing.Point(8, 3);
            this.pnlTopRuler.Name = "pnlTopRuler";
            this.pnlTopRuler.Size = new System.Drawing.Size(227, 18);
            this.pnlTopRuler.TabIndex = 0;
            // 
            // pnlLeftRuler
            // 
            this.pnlLeftRuler.Location = new System.Drawing.Point(3, 7);
            this.pnlLeftRuler.Name = "pnlLeftRuler";
            this.pnlLeftRuler.Size = new System.Drawing.Size(21, 140);
            this.pnlLeftRuler.TabIndex = 0;
            // 
            // txtRowValues
            // 
            this.txtRowValues.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRowValues.Location = new System.Drawing.Point(85, 3);
            this.txtRowValues.Name = "txtRowValues";
            this.txtRowValues.Size = new System.Drawing.Size(103, 13);
            this.txtRowValues.TabIndex = 26;
            // 
            // lblRowValues
            // 
            this.lblRowValues.AutoSize = true;
            this.lblRowValues.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblRowValues.Location = new System.Drawing.Point(2, 5);
            this.lblRowValues.Name = "lblRowValues";
            this.lblRowValues.Size = new System.Drawing.Size(72, 13);
            this.lblRowValues.TabIndex = 27;
            this.lblRowValues.Text = "Row Values :";
            // 
            // lblFile
            // 
            this.lblFile.AutoEllipsis = true;
            this.lblFile.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblFile.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblFile.Location = new System.Drawing.Point(25, 7);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(572, 13);
            this.lblFile.TabIndex = 34;
            this.lblFile.Text = "File :";
            this.lblFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlCommonControls
            // 
            this.pnlCommonControls.Controls.Add(this.btnSyncUC);
            this.pnlCommonControls.Controls.Add(this.txtFontIconName);
            this.pnlCommonControls.Controls.Add(this.btnTrimTop);
            this.pnlCommonControls.Controls.Add(this.lblCharWidth);
            this.pnlCommonControls.Controls.Add(this.btnSelectMove);
            this.pnlCommonControls.Controls.Add(this.numCharWidth);
            this.pnlCommonControls.Controls.Add(this.btnRevertDefault);
            this.pnlCommonControls.Controls.Add(this.lblFontName);
            this.pnlCommonControls.Controls.Add(this.cbCharEdit);
            this.pnlCommonControls.Controls.Add(this.numLowerSpc);
            this.pnlCommonControls.Controls.Add(this.lblLowerSpc);
            this.pnlCommonControls.Controls.Add(this.btnOpen);
            this.pnlCommonControls.Controls.Add(this.numSpaceChar);
            this.pnlCommonControls.Controls.Add(this.btnSave);
            this.pnlCommonControls.Controls.Add(this.lblSpace);
            this.pnlCommonControls.Controls.Add(this.btnSaveAs);
            this.pnlCommonControls.Controls.Add(this.lblHeight);
            this.pnlCommonControls.Controls.Add(this.btnClearText);
            this.pnlCommonControls.Controls.Add(this.numFontHeight);
            this.pnlCommonControls.Controls.Add(this.btnClipboard);
            this.pnlCommonControls.Controls.Add(this.grpWith);
            this.pnlCommonControls.Controls.Add(this.btnFillAll);
            this.pnlCommonControls.Controls.Add(this.btnInverseChar);
            this.pnlCommonControls.Controls.Add(this.btnCopyChar);
            this.pnlCommonControls.Controls.Add(this.btnPasteChar);
            this.pnlCommonControls.Controls.Add(this.grpFormat);
            this.pnlCommonControls.Location = new System.Drawing.Point(335, 106);
            this.pnlCommonControls.Name = "pnlCommonControls";
            this.pnlCommonControls.Size = new System.Drawing.Size(613, 164);
            this.pnlCommonControls.TabIndex = 31;
            // 
            // btnSyncUC
            // 
            this.btnSyncUC.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnSyncUC.Location = new System.Drawing.Point(189, 49);
            this.btnSyncUC.Name = "btnSyncUC";
            this.btnSyncUC.Size = new System.Drawing.Size(57, 64);
            this.btnSyncUC.TabIndex = 39;
            this.btnSyncUC.Text = "Sync Upper Case";
            this.btnSyncUC.UseVisualStyleBackColor = true;
            this.btnSyncUC.Click += new System.EventHandler(this.btnSyncUC_Click);
            // 
            // txtFontIconName
            // 
            this.txtFontIconName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFontIconName.Enabled = false;
            this.txtFontIconName.Location = new System.Drawing.Point(51, 130);
            this.txtFontIconName.Name = "txtFontIconName";
            this.txtFontIconName.Size = new System.Drawing.Size(80, 20);
            this.txtFontIconName.TabIndex = 1;
            this.txtFontIconName.Text = "font30";
            this.txtFontIconName.TextChanged += new System.EventHandler(this.txtFontName_TextChanged);
            // 
            // btnTrimTop
            // 
            this.btnTrimTop.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnTrimTop.Location = new System.Drawing.Point(128, 49);
            this.btnTrimTop.Name = "btnTrimTop";
            this.btnTrimTop.Size = new System.Drawing.Size(57, 64);
            this.btnTrimTop.TabIndex = 36;
            this.btnTrimTop.Text = "Trim Top";
            this.btnTrimTop.UseVisualStyleBackColor = true;
            this.btnTrimTop.Click += new System.EventHandler(this.btnTrimTop_Click);
            // 
            // lblCharWidth
            // 
            this.lblCharWidth.AutoSize = true;
            this.lblCharWidth.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCharWidth.Location = new System.Drawing.Point(277, 132);
            this.lblCharWidth.Name = "lblCharWidth";
            this.lblCharWidth.Size = new System.Drawing.Size(45, 13);
            this.lblCharWidth.TabIndex = 14;
            this.lblCharWidth.Text = "Width :";
            // 
            // btnSelectMove
            // 
            this.btnSelectMove.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnSelectMove.Location = new System.Drawing.Point(67, 49);
            this.btnSelectMove.Name = "btnSelectMove";
            this.btnSelectMove.Size = new System.Drawing.Size(57, 64);
            this.btnSelectMove.TabIndex = 35;
            this.btnSelectMove.Text = "Select And Move";
            this.btnSelectMove.UseVisualStyleBackColor = true;
            this.btnSelectMove.Click += new System.EventHandler(this.btnSelectMove_Click);
            // 
            // numCharWidth
            // 
            this.numCharWidth.Location = new System.Drawing.Point(324, 129);
            this.numCharWidth.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.numCharWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCharWidth.Name = "numCharWidth";
            this.numCharWidth.Size = new System.Drawing.Size(48, 20);
            this.numCharWidth.TabIndex = 15;
            this.numCharWidth.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numCharWidth.ValueChanged += new System.EventHandler(this.numCharWidth_ValueChanged);
            // 
            // lblFontName
            // 
            this.lblFontName.AutoSize = true;
            this.lblFontName.Enabled = false;
            this.lblFontName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblFontName.Location = new System.Drawing.Point(8, 133);
            this.lblFontName.Name = "lblFontName";
            this.lblFontName.Size = new System.Drawing.Size(42, 13);
            this.lblFontName.TabIndex = 0;
            this.lblFontName.Text = "Name :";
            // 
            // cbCharEdit
            // 
            this.cbCharEdit.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbCharEdit.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.cbCharEdit.Location = new System.Drawing.Point(6, 9);
            this.cbCharEdit.Name = "cbCharEdit";
            this.cbCharEdit.Size = new System.Drawing.Size(95, 28);
            this.cbCharEdit.TabIndex = 0;
            this.cbCharEdit.Text = "Edit OFF";
            this.cbCharEdit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbCharEdit.UseVisualStyleBackColor = true;
            this.cbCharEdit.CheckedChanged += new System.EventHandler(this.CharEdit_CheckedChanged);
            // 
            // numLowerSpc
            // 
            this.numLowerSpc.Location = new System.Drawing.Point(547, 129);
            this.numLowerSpc.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.numLowerSpc.Name = "numLowerSpc";
            this.numLowerSpc.Size = new System.Drawing.Size(40, 20);
            this.numLowerSpc.TabIndex = 7;
            this.numLowerSpc.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numLowerSpc.ValueChanged += new System.EventHandler(this.numLowerSpc_ValueChanged);
            // 
            // lblLowerSpc
            // 
            this.lblLowerSpc.AutoSize = true;
            this.lblLowerSpc.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblLowerSpc.Location = new System.Drawing.Point(504, 133);
            this.lblLowerSpc.Name = "lblLowerSpc";
            this.lblLowerSpc.Size = new System.Drawing.Size(37, 13);
            this.lblLowerSpc.TabIndex = 6;
            this.lblLowerSpc.Text = "Base :";
            // 
            // btnOpen
            // 
            this.btnOpen.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnOpen.Location = new System.Drawing.Point(106, 9);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(95, 28);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // numSpaceChar
            // 
            this.numSpaceChar.Location = new System.Drawing.Point(439, 129);
            this.numSpaceChar.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.numSpaceChar.Name = "numSpaceChar";
            this.numSpaceChar.Size = new System.Drawing.Size(40, 20);
            this.numSpaceChar.TabIndex = 5;
            this.numSpaceChar.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numSpaceChar.ValueChanged += new System.EventHandler(this.numSpaceChar_ValueChanged);
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnSave.Location = new System.Drawing.Point(206, 9);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(95, 28);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblSpace
            // 
            this.lblSpace.AutoSize = true;
            this.lblSpace.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblSpace.Location = new System.Drawing.Point(392, 132);
            this.lblSpace.Name = "lblSpace";
            this.lblSpace.Size = new System.Drawing.Size(43, 13);
            this.lblSpace.TabIndex = 4;
            this.lblSpace.Text = "Space :";
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnSaveAs.Location = new System.Drawing.Point(306, 9);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(95, 28);
            this.btnSaveAs.TabIndex = 3;
            this.btnSaveAs.Text = "Save As";
            this.btnSaveAs.UseVisualStyleBackColor = true;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblHeight.Location = new System.Drawing.Point(149, 132);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(48, 13);
            this.lblHeight.TabIndex = 2;
            this.lblHeight.Text = "Height :";
            // 
            // btnClearText
            // 
            this.btnClearText.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnClearText.Location = new System.Drawing.Point(406, 9);
            this.btnClearText.Name = "btnClearText";
            this.btnClearText.Size = new System.Drawing.Size(95, 28);
            this.btnClearText.TabIndex = 4;
            this.btnClearText.Text = "Clear";
            this.btnClearText.UseVisualStyleBackColor = true;
            this.btnClearText.Click += new System.EventHandler(this.btnClearText_Click);
            // 
            // numFontHeight
            // 
            this.numFontHeight.Location = new System.Drawing.Point(199, 129);
            this.numFontHeight.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.numFontHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numFontHeight.Name = "numFontHeight";
            this.numFontHeight.Size = new System.Drawing.Size(44, 20);
            this.numFontHeight.TabIndex = 3;
            this.numFontHeight.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numFontHeight.ValueChanged += new System.EventHandler(this.numFontHeight_ValueChanged);
            // 
            // btnClipboard
            // 
            this.btnClipboard.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnClipboard.Location = new System.Drawing.Point(506, 9);
            this.btnClipboard.Name = "btnClipboard";
            this.btnClipboard.Size = new System.Drawing.Size(101, 28);
            this.btnClipboard.TabIndex = 5;
            this.btnClipboard.Text = "Copy Clipboard";
            this.btnClipboard.UseVisualStyleBackColor = true;
            this.btnClipboard.Click += new System.EventHandler(this.btnClipboard_Click);
            // 
            // grpWith
            // 
            this.grpWith.Controls.Add(this.rbFillBlack);
            this.grpWith.Controls.Add(this.rbFillWhite);
            this.grpWith.Location = new System.Drawing.Point(254, 50);
            this.grpWith.Name = "grpWith";
            this.grpWith.Size = new System.Drawing.Size(69, 64);
            this.grpWith.TabIndex = 6;
            this.grpWith.TabStop = false;
            this.grpWith.Text = "Fill With";
            // 
            // rbFillBlack
            // 
            this.rbFillBlack.AutoSize = true;
            this.rbFillBlack.Checked = true;
            this.rbFillBlack.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.rbFillBlack.Location = new System.Drawing.Point(6, 40);
            this.rbFillBlack.Name = "rbFillBlack";
            this.rbFillBlack.Size = new System.Drawing.Size(52, 17);
            this.rbFillBlack.TabIndex = 1;
            this.rbFillBlack.TabStop = true;
            this.rbFillBlack.Text = "Black";
            this.rbFillBlack.UseVisualStyleBackColor = true;
            this.rbFillBlack.CheckedChanged += new System.EventHandler(this.rbFillBlack_CheckedChanged);
            // 
            // rbFillWhite
            // 
            this.rbFillWhite.AutoSize = true;
            this.rbFillWhite.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.rbFillWhite.Location = new System.Drawing.Point(6, 18);
            this.rbFillWhite.Name = "rbFillWhite";
            this.rbFillWhite.Size = new System.Drawing.Size(56, 17);
            this.rbFillWhite.TabIndex = 0;
            this.rbFillWhite.Text = "White";
            this.rbFillWhite.UseVisualStyleBackColor = true;
            this.rbFillWhite.CheckedChanged += new System.EventHandler(this.rbFillWhite_CheckedChanged);
            // 
            // btnFillAll
            // 
            this.btnFillAll.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnFillAll.Location = new System.Drawing.Point(330, 57);
            this.btnFillAll.Name = "btnFillAll";
            this.btnFillAll.Size = new System.Drawing.Size(54, 24);
            this.btnFillAll.TabIndex = 7;
            this.btnFillAll.Text = "Fill All";
            this.btnFillAll.UseVisualStyleBackColor = true;
            this.btnFillAll.Click += new System.EventHandler(this.btnFillAll_Click);
            // 
            // btnInverseChar
            // 
            this.btnInverseChar.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnInverseChar.Location = new System.Drawing.Point(330, 85);
            this.btnInverseChar.Name = "btnInverseChar";
            this.btnInverseChar.Size = new System.Drawing.Size(54, 24);
            this.btnInverseChar.TabIndex = 8;
            this.btnInverseChar.Text = "Inverse";
            this.btnInverseChar.UseVisualStyleBackColor = true;
            this.btnInverseChar.Click += new System.EventHandler(this.btnInverseChar_Click);
            // 
            // btnCopyChar
            // 
            this.btnCopyChar.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnCopyChar.Location = new System.Drawing.Point(390, 57);
            this.btnCopyChar.Name = "btnCopyChar";
            this.btnCopyChar.Size = new System.Drawing.Size(60, 24);
            this.btnCopyChar.TabIndex = 9;
            this.btnCopyChar.Text = "Copy";
            this.btnCopyChar.UseVisualStyleBackColor = true;
            this.btnCopyChar.Click += new System.EventHandler(this.btnCopyChar_Click);
            // 
            // btnPasteChar
            // 
            this.btnPasteChar.Enabled = false;
            this.btnPasteChar.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnPasteChar.Location = new System.Drawing.Point(390, 85);
            this.btnPasteChar.Name = "btnPasteChar";
            this.btnPasteChar.Size = new System.Drawing.Size(60, 24);
            this.btnPasteChar.TabIndex = 10;
            this.btnPasteChar.Text = "Paste";
            this.btnPasteChar.UseVisualStyleBackColor = true;
            this.btnPasteChar.Click += new System.EventHandler(this.btnPasteChar_Click);
            // 
            // grpFormat
            // 
            this.grpFormat.Controls.Add(this.rbBinaryData);
            this.grpFormat.Controls.Add(this.rbHexdecimalData);
            this.grpFormat.Controls.Add(this.rbDecimalData);
            this.grpFormat.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.grpFormat.Location = new System.Drawing.Point(457, 49);
            this.grpFormat.Name = "grpFormat";
            this.grpFormat.Size = new System.Drawing.Size(150, 64);
            this.grpFormat.TabIndex = 11;
            this.grpFormat.TabStop = false;
            this.grpFormat.Text = "Format";
            // 
            // rbBinaryData
            // 
            this.rbBinaryData.AutoSize = true;
            this.rbBinaryData.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.rbBinaryData.Location = new System.Drawing.Point(102, 26);
            this.rbBinaryData.Name = "rbBinaryData";
            this.rbBinaryData.Size = new System.Drawing.Size(42, 17);
            this.rbBinaryData.TabIndex = 2;
            this.rbBinaryData.Text = "Bin";
            this.rbBinaryData.UseVisualStyleBackColor = true;
            this.rbBinaryData.CheckedChanged += new System.EventHandler(this.rbBinaryData_CheckedChanged);
            // 
            // rbHexdecimalData
            // 
            this.rbHexdecimalData.AutoSize = true;
            this.rbHexdecimalData.Checked = true;
            this.rbHexdecimalData.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.rbHexdecimalData.Location = new System.Drawing.Point(54, 27);
            this.rbHexdecimalData.Name = "rbHexdecimalData";
            this.rbHexdecimalData.Size = new System.Drawing.Size(44, 17);
            this.rbHexdecimalData.TabIndex = 1;
            this.rbHexdecimalData.TabStop = true;
            this.rbHexdecimalData.Text = "Hex";
            this.rbHexdecimalData.UseVisualStyleBackColor = true;
            this.rbHexdecimalData.CheckedChanged += new System.EventHandler(this.rbHexdecimalData_CheckedChanged);
            // 
            // rbDecimalData
            // 
            this.rbDecimalData.AutoSize = true;
            this.rbDecimalData.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.rbDecimalData.Location = new System.Drawing.Point(6, 27);
            this.rbDecimalData.Name = "rbDecimalData";
            this.rbDecimalData.Size = new System.Drawing.Size(44, 17);
            this.rbDecimalData.TabIndex = 0;
            this.rbDecimalData.Text = "Dec";
            this.rbDecimalData.UseVisualStyleBackColor = true;
            this.rbDecimalData.CheckedChanged += new System.EventHandler(this.rbDecimalData_CheckedChanged);
            // 
            // pnlFontControls
            // 
            this.pnlFontControls.Controls.Add(this.rbViewCurrentChar);
            this.pnlFontControls.Controls.Add(this.rbViewFullHeader);
            this.pnlFontControls.Controls.Add(this.lblCharSelect);
            this.pnlFontControls.Controls.Add(this.btnPrevChar);
            this.pnlFontControls.Controls.Add(this.cmbCharacters);
            this.pnlFontControls.Controls.Add(this.btnNextChar);
            this.pnlFontControls.Controls.Add(this.pnlCharBox);
            this.pnlFontControls.Location = new System.Drawing.Point(335, 276);
            this.pnlFontControls.Name = "pnlFontControls";
            this.pnlFontControls.Size = new System.Drawing.Size(613, 140);
            this.pnlFontControls.TabIndex = 32;
            // 
            // rbViewCurrentChar
            // 
            this.rbViewCurrentChar.AutoSize = true;
            this.rbViewCurrentChar.Checked = true;
            this.rbViewCurrentChar.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.rbViewCurrentChar.Location = new System.Drawing.Point(356, 9);
            this.rbViewCurrentChar.Name = "rbViewCurrentChar";
            this.rbViewCurrentChar.Size = new System.Drawing.Size(104, 17);
            this.rbViewCurrentChar.TabIndex = 8;
            this.rbViewCurrentChar.TabStop = true;
            this.rbViewCurrentChar.Text = "Character Code";
            this.rbViewCurrentChar.UseVisualStyleBackColor = true;
            this.rbViewCurrentChar.CheckedChanged += new System.EventHandler(this.rbViewMode_CheckedChanged);
            // 
            // rbViewFullHeader
            // 
            this.rbViewFullHeader.AutoSize = true;
            this.rbViewFullHeader.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.rbViewFullHeader.Location = new System.Drawing.Point(480, 9);
            this.rbViewFullHeader.Name = "rbViewFullHeader";
            this.rbViewFullHeader.Size = new System.Drawing.Size(84, 17);
            this.rbViewFullHeader.TabIndex = 9;
            this.rbViewFullHeader.Text = "All Font (.h)";
            this.rbViewFullHeader.UseVisualStyleBackColor = true;
            this.rbViewFullHeader.CheckedChanged += new System.EventHandler(this.rbViewMode_CheckedChanged);
            // 
            // lblCharSelect
            // 
            this.lblCharSelect.AutoSize = true;
            this.lblCharSelect.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCharSelect.Location = new System.Drawing.Point(6, 10);
            this.lblCharSelect.Name = "lblCharSelect";
            this.lblCharSelect.Size = new System.Drawing.Size(62, 13);
            this.lblCharSelect.TabIndex = 10;
            this.lblCharSelect.Text = "Character :";
            // 
            // btnPrevChar
            // 
            this.btnPrevChar.Location = new System.Drawing.Point(78, 6);
            this.btnPrevChar.Name = "btnPrevChar";
            this.btnPrevChar.Size = new System.Drawing.Size(28, 23);
            this.btnPrevChar.TabIndex = 11;
            this.btnPrevChar.Text = "◀";
            this.btnPrevChar.UseVisualStyleBackColor = true;
            this.btnPrevChar.Click += new System.EventHandler(this.btnPrevChar_Click);
            // 
            // cmbCharacters
            // 
            this.cmbCharacters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCharacters.FormattingEnabled = true;
            this.cmbCharacters.Location = new System.Drawing.Point(108, 7);
            this.cmbCharacters.Name = "cmbCharacters";
            this.cmbCharacters.Size = new System.Drawing.Size(200, 21);
            this.cmbCharacters.TabIndex = 12;
            this.cmbCharacters.SelectedIndexChanged += new System.EventHandler(this.cmbCharacters_SelectedIndexChanged);
            // 
            // btnNextChar
            // 
            this.btnNextChar.Location = new System.Drawing.Point(312, 6);
            this.btnNextChar.Name = "btnNextChar";
            this.btnNextChar.Size = new System.Drawing.Size(28, 23);
            this.btnNextChar.TabIndex = 13;
            this.btnNextChar.Text = "▶";
            this.btnNextChar.UseVisualStyleBackColor = true;
            this.btnNextChar.Click += new System.EventHandler(this.btnNextChar_Click);
            // 
            // pnlCharBox
            // 
            this.pnlCharBox.AutoScroll = true;
            this.pnlCharBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCharBox.Location = new System.Drawing.Point(6, 34);
            this.pnlCharBox.Name = "pnlCharBox";
            this.pnlCharBox.Size = new System.Drawing.Size(599, 93);
            this.pnlCharBox.TabIndex = 16;
            // 
            // lblHeader
            // 
            this.lblHeader.Font = new System.Drawing.Font("Vladimir Script", 36F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.Black;
            this.lblHeader.Location = new System.Drawing.Point(34, 1);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(407, 48);
            this.lblHeader.TabIndex = 35;
            this.lblHeader.Text = "Font & Icon Creator";
            this.lblHeader.UseMnemonic = false;
            // 
            // rbFontMode
            // 
            this.rbFontMode.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbFontMode.BackColor = System.Drawing.SystemColors.ControlLight;
            this.rbFontMode.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbFontMode.Checked = true;
            this.rbFontMode.FlatAppearance.BorderSize = 0;
            this.rbFontMode.FlatAppearance.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.rbFontMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbFontMode.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.rbFontMode.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbFontMode.Location = new System.Drawing.Point(0, 0);
            this.rbFontMode.Name = "rbFontMode";
            this.rbFontMode.Size = new System.Drawing.Size(307, 31);
            this.rbFontMode.TabIndex = 1;
            this.rbFontMode.TabStop = true;
            this.rbFontMode.Text = "FONT";
            this.rbFontMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbFontMode.UseVisualStyleBackColor = false;
            this.rbFontMode.CheckedChanged += new System.EventHandler(this.rbMode_CheckedChanged);
            // 
            // rbIconMode
            // 
            this.rbIconMode.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbIconMode.BackColor = System.Drawing.SystemColors.ControlLight;
            this.rbIconMode.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.rbIconMode.FlatAppearance.BorderSize = 0;
            this.rbIconMode.FlatAppearance.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.rbIconMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbIconMode.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.rbIconMode.Location = new System.Drawing.Point(307, 0);
            this.rbIconMode.Name = "rbIconMode";
            this.rbIconMode.Size = new System.Drawing.Size(307, 31);
            this.rbIconMode.TabIndex = 0;
            this.rbIconMode.Text = "ICON / IMAGE";
            this.rbIconMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbIconMode.UseVisualStyleBackColor = false;
            this.rbIconMode.CheckedChanged += new System.EventHandler(this.rbMode_CheckedChanged);
            // 
            // pnlMode
            // 
            this.pnlMode.Controls.Add(this.rbFontMode);
            this.pnlMode.Controls.Add(this.rbIconMode);
            this.pnlMode.Location = new System.Drawing.Point(335, 73);
            this.pnlMode.Name = "pnlMode";
            this.pnlMode.Size = new System.Drawing.Size(614, 31);
            this.pnlMode.TabIndex = 36;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlHeader.Controls.Add(this.lblHeader);
            this.pnlHeader.Location = new System.Drawing.Point(12, 2);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(566, 51);
            this.pnlHeader.TabIndex = 2;
            // 
            // pnlDataMon
            // 
            this.pnlDataMon.Controls.Add(this.rtxDataMonitor);
            this.pnlDataMon.Location = new System.Drawing.Point(335, 409);
            this.pnlDataMon.Name = "pnlDataMon";
            this.pnlDataMon.Size = new System.Drawing.Size(613, 120);
            this.pnlDataMon.TabIndex = 37;
            // 
            // pnlFile
            // 
            this.pnlFile.Controls.Add(this.lblFile);
            this.pnlFile.Location = new System.Drawing.Point(335, 535);
            this.pnlFile.Name = "pnlFile";
            this.pnlFile.Size = new System.Drawing.Size(613, 26);
            this.pnlFile.TabIndex = 38;
            // 
            // pnlRowVal
            // 
            this.pnlRowVal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlRowVal.Controls.Add(this.lblRowValues);
            this.pnlRowVal.Controls.Add(this.txtRowValues);
            this.pnlRowVal.Location = new System.Drawing.Point(44, 209);
            this.pnlRowVal.Name = "pnlRowVal";
            this.pnlRowVal.Size = new System.Drawing.Size(247, 26);
            this.pnlRowVal.TabIndex = 35;
            // 
            // pnlTopRulerContainer
            // 
            this.pnlTopRulerContainer.Controls.Add(this.pnlTopRuler);
            this.pnlTopRulerContainer.Location = new System.Drawing.Point(39, 6);
            this.pnlTopRulerContainer.Name = "pnlTopRulerContainer";
            this.pnlTopRulerContainer.Size = new System.Drawing.Size(252, 25);
            this.pnlTopRulerContainer.TabIndex = 39;
            // 
            // pnlLeftRulerContainer
            // 
            this.pnlLeftRulerContainer.Controls.Add(this.pnlLeftRuler);
            this.pnlLeftRulerContainer.Location = new System.Drawing.Point(8, 31);
            this.pnlLeftRulerContainer.Name = "pnlLeftRulerContainer";
            this.pnlLeftRulerContainer.Size = new System.Drawing.Size(30, 166);
            this.pnlLeftRulerContainer.TabIndex = 40;
            // 
            // pnlGridScroll
            // 
            this.pnlGridScroll.AutoScroll = true;
            this.pnlGridScroll.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlGridScroll.Controls.Add(this.tlpGrid);
            this.pnlGridScroll.Location = new System.Drawing.Point(44, 33);
            this.pnlGridScroll.Name = "pnlGridScroll";
            this.pnlGridScroll.Size = new System.Drawing.Size(247, 164);
            this.pnlGridScroll.TabIndex = 41;
            // 
            // pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.pnlTopRulerContainer);
            this.pnlLeft.Controls.Add(this.pnlRowVal);
            this.pnlLeft.Controls.Add(this.pnlGridScroll);
            this.pnlLeft.Controls.Add(this.pnlLeftRulerContainer);
            this.pnlLeft.Location = new System.Drawing.Point(12, 106);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(307, 253);
            this.pnlLeft.TabIndex = 0;
            // 
            // btnOpenSysFont
            // 
            this.btnOpenSysFont.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnOpenSysFont.Location = new System.Drawing.Point(12, 73);
            this.btnOpenSysFont.Name = "btnOpenSysFont";
            this.btnOpenSysFont.Size = new System.Drawing.Size(163, 27);
            this.btnOpenSysFont.TabIndex = 39;
            this.btnOpenSysFont.Text = "Open System Font";
            this.btnOpenSysFont.UseVisualStyleBackColor = true;
            this.btnOpenSysFont.Click += new System.EventHandler(this.btnOpenSysFont_Click);
            // 
            // btnOpenImage
            // 
            this.btnOpenImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnOpenImage.Location = new System.Drawing.Point(181, 73);
            this.btnOpenImage.Name = "btnOpenImage";
            this.btnOpenImage.Size = new System.Drawing.Size(138, 27);
            this.btnOpenImage.TabIndex = 40;
            this.btnOpenImage.Text = "Open Image";
            this.btnOpenImage.UseVisualStyleBackColor = true;
            this.btnOpenImage.Click += new System.EventHandler(this.btnOpenImage_Click);
            // 
            // btnTheme
            // 
            this.btnTheme.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnTheme.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btnTheme.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTheme.Location = new System.Drawing.Point(752, 20);
            this.btnTheme.Name = "btnTheme";
            this.btnTheme.Size = new System.Drawing.Size(75, 23);
            this.btnTheme.TabIndex = 41;
            this.btnTheme.Text = "Theme";
            this.btnTheme.UseVisualStyleBackColor = true;
            this.btnTheme.Click += new System.EventHandler(this.btnTheme_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(998, 581);
            this.Controls.Add(this.btnTheme);
            this.Controls.Add(this.btnOpenImage);
            this.Controls.Add(this.btnOpenSysFont);
            this.Controls.Add(this.pnlLeft);
            this.Controls.Add(this.pnlFile);
            this.Controls.Add(this.pnlDataMon);
            this.Controls.Add(this.pnlMode);
            this.Controls.Add(this.pnlFontControls);
            this.Controls.Add(this.pnlCommonControls);
            this.Controls.Add(this.pnlHeader);
            this.MinimumSize = new System.Drawing.Size(1000, 620);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.pnlCommonControls.ResumeLayout(false);
            this.pnlCommonControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCharWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLowerSpc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpaceChar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFontHeight)).EndInit();
            this.grpWith.ResumeLayout(false);
            this.grpWith.PerformLayout();
            this.grpFormat.ResumeLayout(false);
            this.grpFormat.PerformLayout();
            this.pnlFontControls.ResumeLayout(false);
            this.pnlFontControls.PerformLayout();
            this.pnlMode.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlDataMon.ResumeLayout(false);
            this.pnlFile.ResumeLayout(false);
            this.pnlRowVal.ResumeLayout(false);
            this.pnlRowVal.PerformLayout();
            this.pnlTopRulerContainer.ResumeLayout(false);
            this.pnlLeftRulerContainer.ResumeLayout(false);
            this.pnlGridScroll.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
       
        private System.Windows.Forms.TableLayoutPanel tlpGrid;
        private System.Windows.Forms.RichTextBox rtxDataMonitor;
        private System.Windows.Forms.Button btnRevertDefault;
        private System.Windows.Forms.OpenFileDialog OpenFileDialog1;
        private System.Windows.Forms.SaveFileDialog SaveFileDialog1;
        private System.Windows.Forms.Panel pnlTopRuler;
        private System.Windows.Forms.Panel pnlLeftRuler;
        private System.Windows.Forms.TextBox txtRowValues;
        private System.Windows.Forms.Label lblRowValues;
        private System.Windows.Forms.Label lblFile;

        // Common Controls Panel
        private System.Windows.Forms.Panel pnlCommonControls;
        private System.Windows.Forms.CheckBox cbCharEdit;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.Button btnClearText;
        private System.Windows.Forms.Button btnClipboard;
        private System.Windows.Forms.GroupBox grpWith;
        private System.Windows.Forms.RadioButton rbFillBlack;
        private System.Windows.Forms.RadioButton rbFillWhite;
        private System.Windows.Forms.Button btnFillAll;
        private System.Windows.Forms.Button btnInverseChar;
        private System.Windows.Forms.Button btnCopyChar;
        private System.Windows.Forms.Button btnPasteChar;
        private System.Windows.Forms.GroupBox grpFormat;
        private System.Windows.Forms.RadioButton rbBinaryData;
        private System.Windows.Forms.RadioButton rbHexdecimalData;
        private System.Windows.Forms.RadioButton rbDecimalData;

        // Font Controls Panel
        private System.Windows.Forms.Panel pnlFontControls;
        private System.Windows.Forms.RadioButton rbViewCurrentChar;
        private System.Windows.Forms.RadioButton rbViewFullHeader;
        private System.Windows.Forms.Label lblFontName;
        private System.Windows.Forms.TextBox txtFontIconName;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.NumericUpDown numFontHeight;
        private System.Windows.Forms.Label lblSpace;
        private System.Windows.Forms.NumericUpDown numSpaceChar;
        private System.Windows.Forms.Label lblLowerSpc;
        private System.Windows.Forms.NumericUpDown numLowerSpc;
        private System.Windows.Forms.Label lblCharSelect;
        private System.Windows.Forms.Button btnPrevChar;
        private System.Windows.Forms.ComboBox cmbCharacters;
        private System.Windows.Forms.Button btnNextChar;
        private System.Windows.Forms.Label lblCharWidth;
        private System.Windows.Forms.NumericUpDown numCharWidth;
        private System.Windows.Forms.Panel pnlCharBox;
        private System.Windows.Forms.Button btnTrimTop;
        private System.Windows.Forms.Button btnSelectMove;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.RadioButton rbFontMode;
        private System.Windows.Forms.RadioButton rbIconMode;
        private System.Windows.Forms.Panel pnlMode;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlDataMon;
        private System.Windows.Forms.Panel pnlFile;
        private System.Windows.Forms.Panel pnlRowVal;
        private System.Windows.Forms.Panel pnlTopRulerContainer;
        private System.Windows.Forms.Panel pnlLeftRulerContainer;
        private System.Windows.Forms.Panel pnlGridScroll;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Button btnSyncUC;
        private System.Windows.Forms.Button btnOpenSysFont;
        private System.Windows.Forms.Button btnOpenImage;
        private System.Windows.Forms.Button btnTheme;
    }
}
