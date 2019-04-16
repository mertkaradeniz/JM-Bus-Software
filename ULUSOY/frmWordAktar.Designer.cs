namespace ULUSOY
{
    partial class frmWordAktar
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
            this.button1 = new System.Windows.Forms.Button();
            this.plaka = new System.Windows.Forms.Label();
            this.zaman = new System.Windows.Forms.Label();
            this.host = new System.Windows.Forms.Label();
            this.kaptan = new System.Windows.Forms.Label();
            this.listView3 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.button1.Location = new System.Drawing.Point(12, 24);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(10, 10);
            this.button1.TabIndex = 0;
            this.button1.Text = "Dosyayı Oluştur";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // plaka
            // 
            this.plaka.AutoSize = true;
            this.plaka.Location = new System.Drawing.Point(36, 3);
            this.plaka.Name = "plaka";
            this.plaka.Size = new System.Drawing.Size(33, 13);
            this.plaka.TabIndex = 11;
            this.plaka.Text = "plaka";
            // 
            // zaman
            // 
            this.zaman.AutoSize = true;
            this.zaman.Location = new System.Drawing.Point(75, 3);
            this.zaman.Name = "zaman";
            this.zaman.Size = new System.Drawing.Size(38, 13);
            this.zaman.TabIndex = 12;
            this.zaman.Text = "zaman";
            // 
            // host
            // 
            this.host.AutoSize = true;
            this.host.Location = new System.Drawing.Point(119, 5);
            this.host.Name = "host";
            this.host.Size = new System.Drawing.Size(27, 13);
            this.host.TabIndex = 13;
            this.host.Text = "host";
            // 
            // kaptan
            // 
            this.kaptan.AutoSize = true;
            this.kaptan.Location = new System.Drawing.Point(152, 5);
            this.kaptan.Name = "kaptan";
            this.kaptan.Size = new System.Drawing.Size(40, 13);
            this.kaptan.TabIndex = 14;
            this.kaptan.Text = "kaptan";
            // 
            // listView3
            // 
            this.listView3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.listView3.CheckBoxes = true;
            this.listView3.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listView3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.listView3.GridLines = true;
            this.listView3.Location = new System.Drawing.Point(12, -429);
            this.listView3.Name = "listView3";
            this.listView3.Size = new System.Drawing.Size(10, 10);
            this.listView3.TabIndex = 44;
            this.listView3.UseCompatibleStateImageBehavior = false;
            this.listView3.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "KOLTUK NO";
            this.columnHeader1.Width = 123;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "AD SOYAD";
            this.columnHeader2.Width = 127;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "TEL";
            this.columnHeader3.Width = 82;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "İNDİ/BİNDİ";
            this.columnHeader4.Width = 105;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "BİLET İŞLEM/ÜCRET";
            this.columnHeader5.Width = 185;
            // 
            // frmWordAktar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(0, 0);
            this.Controls.Add(this.listView3);
            this.Controls.Add(this.kaptan);
            this.Controls.Add(this.host);
            this.Controls.Add(this.zaman);
            this.Controls.Add(this.plaka);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmWordAktar";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Worda Aktar";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.worda_aktar_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label plaka;
        private System.Windows.Forms.Label zaman;
        private System.Windows.Forms.Label host;
        private System.Windows.Forms.Label kaptan;
        private System.Windows.Forms.ListView listView3;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
    }
}