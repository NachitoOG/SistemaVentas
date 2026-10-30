namespace Vista_usuario
{
    partial class frmHome
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
            lblPerfil = new Label();
            btnVentas = new Button();
            btnOtrosModulos = new Button();
            btnAdministrar = new Button();
            btnIrPoliticas = new Button();
            btnConfiguracion = new Button();
            btnCerrarSesion = new Button();
            SuspendLayout();
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(40, 40, 40);
            lblTitulo.Location = new Point(22, 20);
            lblTitulo.MaximumSize = new Size(340, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Text = "Hola";
            lblPerfil.AutoSize = true;
            lblPerfil.ForeColor = Color.DimGray;
            lblPerfil.Location = new Point(24, 56);
            lblPerfil.MaximumSize = new Size(340, 0);
            lblPerfil.Name = "lblPerfil";
            lblPerfil.Text = "";
            btnVentas.Font = new Font("Segoe UI", 10F);
            btnVentas.Location = new Point(24, 96);
            btnVentas.Name = "btnVentas";
            btnVentas.Padding = new Padding(10, 0, 0, 0);
            btnVentas.Size = new Size(332, 38);
            btnVentas.TabIndex = 0;
            btnVentas.Text = "Presupuestos y pedidos de venta";
            btnVentas.TextAlign = ContentAlignment.MiddleLeft;
            btnVentas.UseVisualStyleBackColor = true;
            btnVentas.Click += btnVentas_Click;
            btnOtrosModulos.Font = new Font("Segoe UI", 10F);
            btnOtrosModulos.Location = new Point(24, 142);
            btnOtrosModulos.Name = "btnOtrosModulos";
            btnOtrosModulos.Padding = new Padding(10, 0, 0, 0);
            btnOtrosModulos.Size = new Size(332, 38);
            btnOtrosModulos.TabIndex = 1;
            btnOtrosModulos.Text = "Otros módulos del sistema";
            btnOtrosModulos.TextAlign = ContentAlignment.MiddleLeft;
            btnOtrosModulos.UseVisualStyleBackColor = true;
            btnOtrosModulos.Click += btnOtrosModulos_Click;
            btnAdministrar.Font = new Font("Segoe UI", 10F);
            btnAdministrar.Location = new Point(24, 188);
            btnAdministrar.Name = "btnAdministrar";
            btnAdministrar.Padding = new Padding(10, 0, 0, 0);
            btnAdministrar.Size = new Size(332, 38);
            btnAdministrar.TabIndex = 2;
            btnAdministrar.Text = "Administrar usuarios";
            btnAdministrar.TextAlign = ContentAlignment.MiddleLeft;
            btnAdministrar.UseVisualStyleBackColor = true;
            btnAdministrar.Click += btnAdministrar_Click;
            btnIrPoliticas.Font = new Font("Segoe UI", 10F);
            btnIrPoliticas.Location = new Point(24, 234);
            btnIrPoliticas.Name = "btnIrPoliticas";
            btnIrPoliticas.Padding = new Padding(10, 0, 0, 0);
            btnIrPoliticas.Size = new Size(332, 38);
            btnIrPoliticas.TabIndex = 3;
            btnIrPoliticas.Text = "Políticas de seguridad";
            btnIrPoliticas.TextAlign = ContentAlignment.MiddleLeft;
            btnIrPoliticas.UseVisualStyleBackColor = true;
            btnIrPoliticas.Click += btnIrPoliticas_Click;
            btnConfiguracion.Font = new Font("Segoe UI", 10F);
            btnConfiguracion.Location = new Point(24, 280);
            btnConfiguracion.Name = "btnConfiguracion";
            btnConfiguracion.Padding = new Padding(10, 0, 0, 0);
            btnConfiguracion.Size = new Size(332, 38);
            btnConfiguracion.TabIndex = 4;
            btnConfiguracion.Text = "Mi cuenta (contraseña y preguntas)";
            btnConfiguracion.TextAlign = ContentAlignment.MiddleLeft;
            btnConfiguracion.UseVisualStyleBackColor = true;
            btnConfiguracion.Click += btnConfiguracion_Click;
            btnCerrarSesion.Font = new Font("Segoe UI", 10F);
            btnCerrarSesion.Location = new Point(24, 326);
            btnCerrarSesion.Name = "btnCerrarSesion";
            btnCerrarSesion.Padding = new Padding(10, 0, 0, 0);
            btnCerrarSesion.Size = new Size(332, 38);
            btnCerrarSesion.TabIndex = 5;
            btnCerrarSesion.Text = "Cerrar sesión";
            btnCerrarSesion.TextAlign = ContentAlignment.MiddleLeft;
            btnCerrarSesion.UseVisualStyleBackColor = true;
            btnCerrarSesion.Click += btnCerrarSesion_Click;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblTitulo);
            Controls.Add(lblPerfil);
            Controls.Add(btnVentas);
            Controls.Add(btnOtrosModulos);
            Controls.Add(btnAdministrar);
            Controls.Add(btnIrPoliticas);
            Controls.Add(btnConfiguracion);
            Controls.Add(btnCerrarSesion);
            ClientSize = new Size(380, 378);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sistema de Gestión Comercial";
            Name = "frmHome";
            Load += frmHome_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblTitulo;
        private Label lblPerfil;
        private Button btnVentas;
        private Button btnOtrosModulos;
        private Button btnAdministrar;
        private Button btnIrPoliticas;
        private Button btnConfiguracion;
        private Button btnCerrarSesion;
    }
}
