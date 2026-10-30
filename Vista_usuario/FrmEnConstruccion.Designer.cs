namespace Vista_usuario
{
    partial class FrmEnConstruccion
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
            lblIcono = new Label();
            lblTitulo = new Label();
            lblTexto = new Label();
            btnVolver = new Button();
            SuspendLayout();
            lblIcono.AutoSize = true;
            lblIcono.Font = new Font("Segoe MDL2 Assets", 30F);
            lblIcono.ForeColor = Color.FromArgb(202, 80, 16);
            lblIcono.Location = new Point(22, 22);
            lblIcono.Name = "lblIcono";
            lblIcono.Text = "\uE7BA";
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(40, 40, 40);
            lblTitulo.Location = new Point(82, 30);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Text = "Módulo en construcción";
            lblTexto.AutoSize = true;
            lblTexto.Location = new Point(24, 92);
            lblTexto.MaximumSize = new Size(372, 0);
            lblTexto.Name = "lblTexto";
            lblTexto.Text = "Acá se van a conectar los módulos de los otros grupos del sistema: productos y stock, proveedores, compras, clientes y facturación. Todavía no están integrados.";
            btnVolver.Location = new Point(300, 170);
            btnVolver.Name = "btnVolver";
            btnVolver.Size = new Size(100, 32);
            btnVolver.TabIndex = 0;
            btnVolver.Text = "Volver";
            btnVolver.UseVisualStyleBackColor = true;
            btnVolver.Click += btnVolver_Click;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblIcono);
            Controls.Add(lblTitulo);
            Controls.Add(lblTexto);
            Controls.Add(btnVolver);
            ClientSize = new Size(420, 216);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Otros módulos";
            CancelButton = btnVolver;
            Name = "FrmEnConstruccion";
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblIcono;
        private Label lblTitulo;
        private Label lblTexto;
        private Button btnVolver;
    }
}
