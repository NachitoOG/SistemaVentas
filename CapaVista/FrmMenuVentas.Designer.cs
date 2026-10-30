namespace CapaVista
{
    partial class FrmMenuVentas
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.btnPresupuestos = new System.Windows.Forms.Button();
            this.btnNotasPedido = new System.Windows.Forms.Button();
            this.btnAvisos = new System.Windows.Forms.Button();
            this.btnReportes = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.SuspendLayout();

            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(24, 20);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Text = "Sistema de Gestión Comercial";

            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Location = new System.Drawing.Point(26, 54);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Text = "Grupo 7 - Presupuestos y Pedidos de Venta";

            this.btnPresupuestos.Location = new System.Drawing.Point(26, 92);
            this.btnPresupuestos.Name = "btnPresupuestos";
            this.btnPresupuestos.Size = new System.Drawing.Size(300, 40);
            this.btnPresupuestos.TabIndex = 0;
            this.btnPresupuestos.Text = "&Presupuestos de venta";
            this.btnPresupuestos.UseVisualStyleBackColor = true;
            this.btnPresupuestos.Click += new System.EventHandler(this.btnPresupuestos_Click);

            this.btnNotasPedido.Location = new System.Drawing.Point(26, 140);
            this.btnNotasPedido.Name = "btnNotasPedido";
            this.btnNotasPedido.Size = new System.Drawing.Size(300, 40);
            this.btnNotasPedido.TabIndex = 1;
            this.btnNotasPedido.Text = "&Notas de pedido";
            this.btnNotasPedido.UseVisualStyleBackColor = true;
            this.btnNotasPedido.Click += new System.EventHandler(this.btnNotasPedido_Click);

            this.btnAvisos.Location = new System.Drawing.Point(26, 188);
            this.btnAvisos.Name = "btnAvisos";
            this.btnAvisos.Size = new System.Drawing.Size(300, 40);
            this.btnAvisos.TabIndex = 2;
            this.btnAvisos.Text = "&Avisos de mercadería preparada";
            this.btnAvisos.UseVisualStyleBackColor = true;
            this.btnAvisos.Click += new System.EventHandler(this.btnAvisos_Click);

            this.btnReportes.Location = new System.Drawing.Point(26, 236);
            this.btnReportes.Name = "btnReportes";
            this.btnReportes.Size = new System.Drawing.Size(300, 40);
            this.btnReportes.TabIndex = 3;
            this.btnReportes.Text = "&Reportes";
            this.btnReportes.UseVisualStyleBackColor = true;
            this.btnReportes.Click += new System.EventHandler(this.btnReportes_Click);

            this.btnSalir.Location = new System.Drawing.Point(226, 296);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(100, 30);
            this.btnSalir.TabIndex = 4;
            this.btnSalir.Text = "&Salir";
            this.btnSalir.UseVisualStyleBackColor = true;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);

            this.lblUsuario.AutoEllipsis = true;
            this.lblUsuario.ForeColor = System.Drawing.Color.DimGray;
            this.lblUsuario.Location = new System.Drawing.Point(26, 303);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(190, 18);
            this.lblUsuario.Text = "";

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 344);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblSubtitulo);
            this.Controls.Add(this.btnPresupuestos);
            this.Controls.Add(this.btnNotasPedido);
            this.Controls.Add(this.btnAvisos);
            this.Controls.Add(this.btnReportes);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblUsuario);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmMenuVentas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Menú principal";
            this.Load += new System.EventHandler(this.FrmMenuVentas_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmMenuVentas_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Button btnPresupuestos;
        private System.Windows.Forms.Button btnNotasPedido;
        private System.Windows.Forms.Button btnAvisos;
        private System.Windows.Forms.Button btnReportes;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Label lblUsuario;
    }
}
