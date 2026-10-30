namespace Vista_usuario
{
    partial class Login
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
            lblSubtitulo = new Label();
            lblUsuario = new Label();
            txtUsuario = new TextBox();
            lblContraseña = new Label();
            txtPassword = new TextBox();
            chkMostrar = new CheckBox();
            btnIngresar = new Button();
            lblMensaje = new Label();
            LlblRecuperar = new LinkLabel();
            SuspendLayout();
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(40, 40, 40);
            lblTitulo.Location = new Point(32, 28);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Text = "Sistema de Gestión Comercial";
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.ForeColor = Color.DimGray;
            lblSubtitulo.Location = new Point(34, 62);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Text = "Iniciá sesión para continuar";
            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(34, 104);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Text = "Usuario";
            txtUsuario.Font = new Font("Segoe UI", 10F);
            txtUsuario.Location = new Point(34, 124);
            txtUsuario.MaxLength = 20;
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(312, 23);
            txtUsuario.TabIndex = 0;
            lblContraseña.AutoSize = true;
            lblContraseña.Location = new Point(34, 164);
            lblContraseña.Name = "lblContraseña";
            lblContraseña.Text = "Contraseña";
            txtPassword.Font = new Font("Segoe UI", 10F);
            txtPassword.Location = new Point(34, 184);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(312, 23);
            txtPassword.TabIndex = 1;
            txtPassword.UseSystemPasswordChar = true;
            chkMostrar.AutoSize = true;
            chkMostrar.Location = new Point(34, 218);
            chkMostrar.Name = "chkMostrar";
            chkMostrar.TabIndex = 2;
            chkMostrar.Text = "Mostrar contraseña";
            chkMostrar.UseVisualStyleBackColor = true;
            chkMostrar.CheckedChanged += chkMostrar_CheckedChanged;
            btnIngresar.Font = new Font("Segoe UI", 10F);
            btnIngresar.Location = new Point(34, 252);
            btnIngresar.Name = "btnIngresar";
            btnIngresar.Size = new Size(312, 36);
            btnIngresar.TabIndex = 3;
            btnIngresar.Text = "Ingresar";
            btnIngresar.UseVisualStyleBackColor = true;
            btnIngresar.Click += btnIngresar_Click;
            lblMensaje.AutoSize = true;
            lblMensaje.ForeColor = Color.Firebrick;
            lblMensaje.Location = new Point(34, 298);
            lblMensaje.MaximumSize = new Size(312, 0);
            lblMensaje.Name = "lblMensaje";
            lblMensaje.Text = "";
            LlblRecuperar.AutoSize = true;
            LlblRecuperar.Location = new Point(34, 350);
            LlblRecuperar.Name = "LlblRecuperar";
            LlblRecuperar.TabIndex = 4;
            LlblRecuperar.TabStop = true;
            LlblRecuperar.Text = "¿Olvidaste tu contraseña?";
            LlblRecuperar.LinkClicked += LlblRecuperar_LinkClicked;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblTitulo);
            Controls.Add(lblSubtitulo);
            Controls.Add(lblUsuario);
            Controls.Add(txtUsuario);
            Controls.Add(lblContraseña);
            Controls.Add(txtPassword);
            Controls.Add(chkMostrar);
            Controls.Add(btnIngresar);
            Controls.Add(lblMensaje);
            Controls.Add(LlblRecuperar);
            ClientSize = new Size(380, 392);
            AcceptButton = btnIngresar;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Iniciar sesión";
            Name = "Login";
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblTitulo;
        private Label lblSubtitulo;
        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblContraseña;
        private TextBox txtPassword;
        private CheckBox chkMostrar;
        private Button btnIngresar;
        private Label lblMensaje;
        private LinkLabel LlblRecuperar;
    }
}
