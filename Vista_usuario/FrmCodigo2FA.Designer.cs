namespace Vista_usuario
{
    partial class FrmCodigo2FA
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
            lblTitulo = new Label();
            lblExplicacion = new Label();
            txtCodigo = new TextBox();
            lblMensaje = new Label();
            btnConfirmar = new Button();
            btnCancelar = new Button();
            SuspendLayout();
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(40, 40, 40);
            lblTitulo.Location = new Point(18, 16);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Text = "Verificación en dos pasos";
            lblExplicacion.AutoSize = true;
            lblExplicacion.ForeColor = Color.DimGray;
            lblExplicacion.Location = new Point(20, 54);
            lblExplicacion.MaximumSize = new Size(360, 0);
            lblExplicacion.Name = "lblExplicacion";
            lblExplicacion.Text = "";
            txtCodigo.Font = new Font("Segoe UI", 14F);
            txtCodigo.Location = new Point(20, 100);
            txtCodigo.MaxLength = 6;
            txtCodigo.Name = "txtCodigo";
            txtCodigo.Size = new Size(160, 23);
            txtCodigo.TabIndex = 0;
            txtCodigo.TextAlign = HorizontalAlignment.Center;
            txtCodigo.KeyPress += txtCodigo_KeyPress;
            lblMensaje.AutoSize = true;
            lblMensaje.ForeColor = Color.Firebrick;
            lblMensaje.Location = new Point(20, 148);
            lblMensaje.MaximumSize = new Size(360, 0);
            lblMensaje.Name = "lblMensaje";
            lblMensaje.Text = "";
            btnConfirmar.Location = new Point(176, 196);
            btnConfirmar.Name = "btnConfirmar";
            btnConfirmar.Size = new Size(100, 32);
            btnConfirmar.TabIndex = 1;
            btnConfirmar.Text = "Confirmar";
            btnConfirmar.UseVisualStyleBackColor = true;
            btnConfirmar.Click += btnConfirmar_Click;
            btnCancelar.Location = new Point(284, 196);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(100, 32);
            btnCancelar.TabIndex = 2;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblTitulo);
            Controls.Add(lblExplicacion);
            Controls.Add(txtCodigo);
            Controls.Add(lblMensaje);
            Controls.Add(btnConfirmar);
            Controls.Add(btnCancelar);
            ClientSize = new Size(400, 244);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Código de ingreso";
            AcceptButton = btnConfirmar;
            CancelButton = btnCancelar;
            Name = "FrmCodigo2FA";
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblTitulo;
        private Label lblExplicacion;
        private TextBox txtCodigo;
        private Label lblMensaje;
        private Button btnConfirmar;
        private Button btnCancelar;
    }
}
