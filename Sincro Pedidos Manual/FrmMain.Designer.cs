namespace Sincro_Pedidos_Manual
{
    partial class FrmMain
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.sincronizarAhoraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sincronizacionConMaestrosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sincronizarAhoraToolStripMenuItem,
            this.sincronizacionConMaestrosToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(775, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // sincronizarAhoraToolStripMenuItem
            // 
            this.sincronizarAhoraToolStripMenuItem.Name = "sincronizarAhoraToolStripMenuItem";
            this.sincronizarAhoraToolStripMenuItem.Size = new System.Drawing.Size(112, 20);
            this.sincronizarAhoraToolStripMenuItem.Text = "Sincronizar Ahora";
            this.sincronizarAhoraToolStripMenuItem.Visible = false;
            this.sincronizarAhoraToolStripMenuItem.Click += new System.EventHandler(this.sincronizarAhoraToolStripMenuItem_Click);
            // 
            // sincronizacionConMaestrosToolStripMenuItem
            // 
            this.sincronizacionConMaestrosToolStripMenuItem.Name = "sincronizacionConMaestrosToolStripMenuItem";
            this.sincronizacionConMaestrosToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
            this.sincronizacionConMaestrosToolStripMenuItem.Text = "Crear Maestros";
            this.sincronizacionConMaestrosToolStripMenuItem.Click += new System.EventHandler(this.sincronizacionConMaestrosToolStripMenuItem_Click);
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(0, 24);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(775, 365);
            this.textBox1.TabIndex = 1;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 389);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmMain";
            this.Text = "Sincro Pedidos Manual";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem sincronizarAhoraToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripMenuItem sincronizacionConMaestrosToolStripMenuItem;
    }
}

