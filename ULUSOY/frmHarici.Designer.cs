namespace JMOtobusYazilimi
{
    partial class frmHarici
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Toplam_tutar",
            "Otobüsün Rezerve ve Ücretli yolcu olmak üzere toplam ücreti veren değişkendir.",
            "Sayısal"}, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "Toplam_yolcu",
            "Otobüste bulunan toplam dolu koltuk sayısını veren değikendir."}, -1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "Otobüs_kapasite",
            "Otobüsün kapasitesini veren değişkendir."}, -1);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "Bos_koltuk",
            "Otobüsteki boş koltuk sayısını veren değişkendir."}, -1);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
            "Ucretsiz_yolcu",
            "Otobüsün sadece Ücretsiz yolcu sayısını veren değişkendir."}, -1);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new string[] {
            "Ucretli_yolcu",
            "Otobüsün sadece Ücret ödeyenlerin yolcu sayısını veren değişkendir."}, -1);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem(new string[] {
            "Rezerve_yolcu",
            "Otobüsün sadece Rezerve  yolcu sayısını veren değişkendir."}, -1);
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem(new string[] {
            "T_ucretli_yolcu_ucreti",
            "(Para) Otobüste ücretini ödeyen yolcuların totalini veren değişkendir."}, -1);
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem(new string[] {
            "T_rezerve_yolcu_ucret",
            "(Para) Otobüste rezerve olan yolcuların toplam ücret totalini gösteren değişkendi" +
                "r."}, -1);
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtEtiketAdi = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtDegeri = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDegiskenAdi = new System.Windows.Forms.TextBox();
            this.listView5 = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label16 = new System.Windows.Forms.Label();
            this.button17 = new System.Windows.Forms.Button();
            this.button18 = new System.Windows.Forms.Button();
            this.listView4 = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Controls.Add(this.textBox1);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.txtEtiketAdi);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.txtDegeri);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.txtDegiskenAdi);
            this.groupBox5.Controls.Add(this.listView5);
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Controls.Add(this.button17);
            this.groupBox5.Controls.Add(this.button18);
            this.groupBox5.Controls.Add(this.listView4);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.groupBox5.Location = new System.Drawing.Point(11, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(833, 423);
            this.groupBox5.TabIndex = 269;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Sistem Hesap Formülü";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label13.Location = new System.Drawing.Point(621, 120);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(97, 18);
            this.label13.TabIndex = 286;
            this.label13.Text = "Değişken Tipi";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(624, 141);
            this.textBox1.Name = "textBox1";
            this.textBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.textBox1.Size = new System.Drawing.Size(135, 24);
            this.textBox1.TabIndex = 285;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label12.Location = new System.Drawing.Point(433, 117);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(69, 18);
            this.label12.TabIndex = 284;
            this.label12.Text = "Etiket Adı";
            // 
            // txtEtiketAdi
            // 
            this.txtEtiketAdi.Location = new System.Drawing.Point(436, 141);
            this.txtEtiketAdi.Name = "txtEtiketAdi";
            this.txtEtiketAdi.Size = new System.Drawing.Size(162, 24);
            this.txtEtiketAdi.TabIndex = 283;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label11.Location = new System.Drawing.Point(621, 54);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(51, 18);
            this.label11.TabIndex = 282;
            this.label11.Text = "Değeri";
            // 
            // txtDegeri
            // 
            this.txtDegeri.Location = new System.Drawing.Point(624, 75);
            this.txtDegeri.Name = "txtDegeri";
            this.txtDegeri.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtDegeri.Size = new System.Drawing.Size(135, 24);
            this.txtDegeri.TabIndex = 281;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label10.ForeColor = System.Drawing.Color.Firebrick;
            this.label10.Location = new System.Drawing.Point(8, 369);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(385, 36);
            this.label10.TabIndex = 280;
            this.label10.Text = "Yukarıda tabloda ki sistem tarafından verilen değişkenleri,\r\nformül içinde kullan" +
    "abilir , tekrar tekrar yapılandırabilirsiniz.\r\n";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label7.Location = new System.Drawing.Point(433, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(94, 18);
            this.label7.TabIndex = 279;
            this.label7.Text = "Değişken Adı";
            // 
            // txtDegiskenAdi
            // 
            this.txtDegiskenAdi.Location = new System.Drawing.Point(436, 79);
            this.txtDegiskenAdi.Name = "txtDegiskenAdi";
            this.txtDegiskenAdi.Size = new System.Drawing.Size(162, 24);
            this.txtDegiskenAdi.TabIndex = 278;
            // 
            // listView5
            // 
            this.listView5.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader5,
            this.columnHeader8,
            this.columnHeader6});
            this.listView5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.listView5.FullRowSelect = true;
            this.listView5.GridLines = true;
            this.listView5.HoverSelection = true;
            this.listView5.Location = new System.Drawing.Point(436, 172);
            this.listView5.Name = "listView5";
            this.listView5.Size = new System.Drawing.Size(323, 233);
            this.listView5.TabIndex = 277;
            this.listView5.UseCompatibleStateImageBehavior = false;
            this.listView5.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "ID";
            this.columnHeader7.Width = 0;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Değişken Adı";
            this.columnHeader5.Width = 119;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Etiket";
            this.columnHeader8.Width = 100;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Değeri";
            this.columnHeader6.Width = 141;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.White;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.label16.ForeColor = System.Drawing.Color.Firebrick;
            this.label16.Location = new System.Drawing.Point(433, 26);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(165, 18);
            this.label16.TabIndex = 276;
            this.label16.Text = "Değişken Tanımlama";
            // 
            // button17
            // 
            this.button17.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.button17.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.button17.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.button17.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.button17.Location = new System.Drawing.Point(765, 204);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(59, 26);
            this.button17.TabIndex = 275;
            this.button17.Text = "Sil";
            this.button17.UseVisualStyleBackColor = true;
            // 
            // button18
            // 
            this.button18.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.button18.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.button18.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.button18.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button18.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.button18.Location = new System.Drawing.Point(765, 172);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(59, 26);
            this.button18.TabIndex = 274;
            this.button18.Text = "Ekle";
            this.button18.UseVisualStyleBackColor = true;
            // 
            // listView4
            // 
            this.listView4.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.listView4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.listView4.FullRowSelect = true;
            this.listView4.GridLines = true;
            this.listView4.HoverSelection = true;
            this.listView4.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9});
            this.listView4.Location = new System.Drawing.Point(9, 54);
            this.listView4.Name = "listView4";
            this.listView4.Size = new System.Drawing.Size(406, 301);
            this.listView4.TabIndex = 273;
            this.listView4.UseCompatibleStateImageBehavior = false;
            this.listView4.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Değişken Adı";
            this.columnHeader3.Width = 141;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Açıklaması";
            this.columnHeader4.Width = 514;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.ForeColor = System.Drawing.Color.Firebrick;
            this.label3.Location = new System.Drawing.Point(6, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(238, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "Sistemden Gelen Parametreler";
            // 
            // frmHarici
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(851, 460);
            this.Controls.Add(this.groupBox5);
            this.Name = "frmHarici";
            this.Text = "frmHarici";
            this.Load += new System.EventHandler(this.frmHarici_Load);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtEtiketAdi;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtDegeri;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtDegiskenAdi;
        public System.Windows.Forms.ListView listView5;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button button17;
        private System.Windows.Forms.Button button18;
        public System.Windows.Forms.ListView listView4;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label label3;
    }
}