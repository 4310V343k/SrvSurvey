﻿namespace SrvSurvey.forms
{
    partial class FormRoute
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            list = new ListView();
            colSystem = new ColumnHeader();
            colDistance = new ColumnHeader();
            btnImport = new Button();
            btnClose = new Button();
            menuImport = new ButtonContextMenuStrip(components);
            menuImportNames = new ToolStripMenuItem();
            menuSystemNamesFile = new ToolStripMenuItem();
            menuSpanshTourist = new ToolStripMenuItem();
            menuSpanshTouristFile = new ToolStripMenuItem();
            status = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            btnSave = new Button();
            btnClear = new Button();
            checkActive = new CheckBox();
            menuImport.SuspendLayout();
            status.SuspendLayout();
            SuspendLayout();
            // 
            // list
            // 
            list.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            list.Columns.AddRange(new ColumnHeader[] { colSystem, colDistance });
            list.Location = new Point(12, 41);
            list.Name = "list";
            list.Size = new Size(454, 355);
            list.TabIndex = 0;
            list.UseCompatibleStateImageBehavior = false;
            list.View = View.Details;
            // 
            // colSystem
            // 
            colSystem.Text = "System";
            colSystem.Width = 260;
            // 
            // colDistance
            // 
            colDistance.Text = "Distance";
            // 
            // btnImport
            // 
            btnImport.Location = new Point(12, 12);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(75, 23);
            btnImport.TabIndex = 1;
            btnImport.Text = "Import ...";
            btnImport.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.Location = new Point(391, 402);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(75, 23);
            btnClose.TabIndex = 2;
            btnClose.Text = "&Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // menuImport
            // 
            menuImport.Items.AddRange(new ToolStripItem[] { menuImportNames, menuSystemNamesFile, menuSpanshTourist, menuSpanshTouristFile });
            menuImport.Name = "menuImport";
            menuImport.Size = new Size(234, 92);
            menuImport.targetButton = btnImport;
            // 
            // menuImportNames
            // 
            menuImportNames.Name = "menuImportNames";
            menuImportNames.Size = new Size(233, 22);
            menuImportNames.Text = "Copied system names ...";
            menuImportNames.Click += menuImportNames_Click;
            // 
            // menuSystemNamesFile
            // 
            menuSystemNamesFile.Name = "menuSystemNamesFile";
            menuSystemNamesFile.Size = new Size(233, 22);
            menuSystemNamesFile.Text = "System names file ...";
            menuSystemNamesFile.Click += menuSystemNamesFile_Click;
            // 
            // menuSpanshTourist
            // 
            menuSpanshTourist.Name = "menuSpanshTourist";
            menuSpanshTourist.Size = new Size(233, 22);
            menuSpanshTourist.Text = "Copied Spansh tourist route ...";
            menuSpanshTourist.Click += menuSpanshTourist_Click;
            // 
            // menuSpanshTouristFile
            // 
            menuSpanshTouristFile.Name = "menuSpanshTouristFile";
            menuSpanshTouristFile.Size = new Size(233, 22);
            menuSpanshTouristFile.Text = "Spansh tourist file ...";
            menuSpanshTouristFile.Click += menuSpanshTouristFile_Click;
            // 
            // status
            // 
            status.Items.AddRange(new ToolStripItem[] { lblStatus });
            status.Location = new Point(0, 426);
            status.Name = "status";
            status.Size = new Size(478, 24);
            status.TabIndex = 3;
            status.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = false;
            lblStatus.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            lblStatus.BorderStyle = Border3DStyle.SunkenOuter;
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(463, 19);
            lblStatus.Spring = true;
            lblStatus.Text = "-";
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.Location = new Point(310, 402);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 4;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnClear
            // 
            btnClear.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnClear.Location = new Point(12, 402);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(75, 23);
            btnClear.TabIndex = 5;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // checkActive
            // 
            checkActive.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkActive.AutoSize = true;
            checkActive.Location = new Point(364, 12);
            checkActive.Name = "checkActive";
            checkActive.Size = new Size(102, 19);
            checkActive.TabIndex = 6;
            checkActive.Text = "Route is active";
            checkActive.UseVisualStyleBackColor = true;
            // 
            // FormRoute
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(478, 450);
            Controls.Add(checkActive);
            Controls.Add(btnClear);
            Controls.Add(btnSave);
            Controls.Add(status);
            Controls.Add(btnClose);
            Controls.Add(btnImport);
            Controls.Add(list);
            Name = "FormRoute";
            Text = "Follow a route";
            menuImport.ResumeLayout(false);
            status.ResumeLayout(false);
            status.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView list;
        private ColumnHeader colSystem;
        private ColumnHeader colDistance;
        private Button btnImport;
        private Button btnClose;
        private ButtonContextMenuStrip menuImport;
        private ToolStripMenuItem menuImportNames;
        private ToolStripMenuItem menuSpanshTourist;
        private StatusStrip status;
        private ToolStripStatusLabel lblStatus;
        private Button btnSave;
        private Button btnClear;
        private ToolStripMenuItem menuSystemNamesFile;
        private ToolStripMenuItem menuSpanshTouristFile;
        private CheckBox checkActive;
    }
}