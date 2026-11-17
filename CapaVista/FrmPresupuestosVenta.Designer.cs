namespace CapaVista
{
    partial class FrmPresupuestosVenta
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
            DataGridViewCellStyle dataGridViewCellStyle10 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle11 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle12 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle13 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle14 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle15 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle16 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle17 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle18 = new DataGridViewCellStyle();
            lblTitulo = new Label();
            btnNuevo = new Button();
            grpFiltros = new GroupBox();
            lblFiltroCliente = new Label();
            cboFiltroCliente = new ComboBox();
            lblFiltroEstado = new Label();
            cboFiltroEstado = new ComboBox();
            chkIncluirBajas = new CheckBox();
            colNumero = new DataGridViewTextBoxColumn();
            colCliente = new DataGridViewTextBoxColumn();
            colFecha = new DataGridViewTextBoxColumn();
            colEstado = new DataGridViewTextBoxColumn();
            colTotal = new DataGridViewTextBoxColumn();
            icoDetalles = new DataGridViewTextBoxColumn();
            icoEditar = new DataGridViewTextBoxColumn();
            icoAprobar = new DataGridViewTextBoxColumn();
            icoRechazar = new DataGridViewTextBoxColumn();
            icoBaja = new DataGridViewTextBoxColumn();
            dgvPresupuestos = new DataGridView();
            lblCantidadRegistros = new Label();
            btnCerrar = new Button();
            grpFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPresupuestos).BeginInit();
            SuspendLayout();
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(40, 40, 40);
            lblTitulo.Location = new Point(12, 14);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(204, 25);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Presupuestos de venta";
            // 
            // btnNuevo
            // 
            btnNuevo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNuevo.Location = new Point(718, 12);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(170, 34);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "+  Nuevo presupuesto";
            btnNuevo.UseVisualStyleBackColor = true;
            btnNuevo.Click += btnNuevo_Click;
            // 
            // grpFiltros
            // 
            grpFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpFiltros.Controls.Add(lblFiltroCliente);
            grpFiltros.Controls.Add(cboFiltroCliente);
            grpFiltros.Controls.Add(lblFiltroEstado);
            grpFiltros.Controls.Add(cboFiltroEstado);
            grpFiltros.Controls.Add(chkIncluirBajas);
            grpFiltros.Location = new Point(12, 58);
            grpFiltros.Name = "grpFiltros";
            grpFiltros.Size = new Size(876, 62);
            grpFiltros.TabIndex = 1;
            grpFiltros.TabStop = false;
            grpFiltros.Text = "Filtros";
            // 
            // lblFiltroCliente
            // 
            lblFiltroCliente.AutoSize = true;
            lblFiltroCliente.Location = new Point(14, 28);
            lblFiltroCliente.Name = "lblFiltroCliente";
            lblFiltroCliente.Size = new Size(47, 15);
            lblFiltroCliente.TabIndex = 0;
            lblFiltroCliente.Text = "Cliente:";
            // 
            // cboFiltroCliente
            // 
            cboFiltroCliente.AutoCompleteCustomSource.AddRange(new string[] { "SuggestAppend" });
            cboFiltroCliente.AutoCompleteSource = AutoCompleteSource.ListItems;
            cboFiltroCliente.FormattingEnabled = true;
            cboFiltroCliente.Location = new Point(68, 25);
            cboFiltroCliente.Name = "cboFiltroCliente";
            cboFiltroCliente.Size = new Size(260, 23);
            cboFiltroCliente.TabIndex = 0;
            cboFiltroCliente.SelectedIndexChanged += Filtros_Cambiaron;
            // 
            // lblFiltroEstado
            // 
            lblFiltroEstado.AutoSize = true;
            lblFiltroEstado.Location = new Point(348, 28);
            lblFiltroEstado.Name = "lblFiltroEstado";
            lblFiltroEstado.Size = new Size(45, 15);
            lblFiltroEstado.TabIndex = 1;
            lblFiltroEstado.Text = "Estado:";
            // 
            // cboFiltroEstado
            // 
            cboFiltroEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFiltroEstado.FormattingEnabled = true;
            cboFiltroEstado.Location = new Point(400, 25);
            cboFiltroEstado.Name = "cboFiltroEstado";
            cboFiltroEstado.Size = new Size(150, 23);
            cboFiltroEstado.TabIndex = 1;
            cboFiltroEstado.SelectedIndexChanged += Filtros_Cambiaron;
            // 
            // chkIncluirBajas
            // 
            chkIncluirBajas.AutoSize = true;
            chkIncluirBajas.Location = new Point(574, 27);
            chkIncluirBajas.Name = "chkIncluirBajas";
            chkIncluirBajas.Size = new Size(161, 19);
            chkIncluirBajas.TabIndex = 2;
            chkIncluirBajas.Text = "Mostrar los dados de baja";
            chkIncluirBajas.UseVisualStyleBackColor = true;
            chkIncluirBajas.CheckedChanged += Filtros_Cambiaron;
            // 
            // colNumero
            // 
            colNumero.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colNumero.DataPropertyName = "IdPresupuesto";
            colNumero.HeaderText = "N°";
            colNumero.Name = "colNumero";
            colNumero.ReadOnly = true;
            colNumero.SortMode = DataGridViewColumnSortMode.NotSortable;
            colNumero.Width = 60;
            // 
            // colCliente
            // 
            colCliente.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colCliente.DataPropertyName = "NombreCliente";
            colCliente.HeaderText = "Cliente";
            colCliente.Name = "colCliente";
            colCliente.ReadOnly = true;
            colCliente.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colFecha
            // 
            colFecha.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colFecha.DataPropertyName = "FechaEmision";
            dataGridViewCellStyle10.Format = "dd/MM/yyyy";
            colFecha.DefaultCellStyle = dataGridViewCellStyle10;
            colFecha.HeaderText = "Fecha";
            colFecha.Name = "colFecha";
            colFecha.ReadOnly = true;
            colFecha.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colEstado
            // 
            colEstado.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colEstado.DataPropertyName = "EstadoTexto";
            colEstado.HeaderText = "Estado";
            colEstado.Name = "colEstado";
            colEstado.ReadOnly = true;
            colEstado.SortMode = DataGridViewColumnSortMode.NotSortable;
            colEstado.Width = 110;
            // 
            // colTotal
            // 
            colTotal.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colTotal.DataPropertyName = "Total";
            dataGridViewCellStyle11.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle11.Format = "N2";
            colTotal.DefaultCellStyle = dataGridViewCellStyle11;
            colTotal.HeaderText = "Total";
            colTotal.Name = "colTotal";
            colTotal.ReadOnly = true;
            colTotal.SortMode = DataGridViewColumnSortMode.NotSortable;
            colTotal.Width = 120;
            // 
            // icoDetalles
            // 
            icoDetalles.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle12.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.Font = new Font("Segoe MDL2 Assets", 11F);
            dataGridViewCellStyle12.ForeColor = Color.FromArgb(90, 90, 90);
            dataGridViewCellStyle12.SelectionForeColor = Color.FromArgb(90, 90, 90);
            icoDetalles.DefaultCellStyle = dataGridViewCellStyle12;
            icoDetalles.HeaderText = "";
            icoDetalles.Name = "icoDetalles";
            icoDetalles.ReadOnly = true;
            icoDetalles.Resizable = DataGridViewTriState.False;
            icoDetalles.SortMode = DataGridViewColumnSortMode.NotSortable;
            icoDetalles.ToolTipText = "Ver detalles";
            icoDetalles.Width = 36;
            // 
            // icoEditar
            // 
            icoEditar.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle13.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle13.Font = new Font("Segoe MDL2 Assets", 11F);
            dataGridViewCellStyle13.ForeColor = Color.FromArgb(0, 99, 177);
            dataGridViewCellStyle13.SelectionForeColor = Color.FromArgb(0, 99, 177);
            icoEditar.DefaultCellStyle = dataGridViewCellStyle13;
            icoEditar.HeaderText = "";
            icoEditar.Name = "icoEditar";
            icoEditar.ReadOnly = true;
            icoEditar.Resizable = DataGridViewTriState.False;
            icoEditar.SortMode = DataGridViewColumnSortMode.NotSortable;
            icoEditar.ToolTipText = "Editar";
            icoEditar.Width = 36;
            // 
            // icoAprobar
            // 
            icoAprobar.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle14.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle14.Font = new Font("Segoe MDL2 Assets", 11F);
            dataGridViewCellStyle14.ForeColor = Color.FromArgb(16, 124, 16);
            dataGridViewCellStyle14.SelectionForeColor = Color.FromArgb(16, 124, 16);
            icoAprobar.DefaultCellStyle = dataGridViewCellStyle14;
            icoAprobar.HeaderText = "";
            icoAprobar.Name = "icoAprobar";
            icoAprobar.ReadOnly = true;
            icoAprobar.Resizable = DataGridViewTriState.False;
            icoAprobar.SortMode = DataGridViewColumnSortMode.NotSortable;
            icoAprobar.ToolTipText = "Aprobar";
            icoAprobar.Width = 36;
            // 
            // icoRechazar
            // 
            icoRechazar.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle15.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle15.Font = new Font("Segoe MDL2 Assets", 11F);
            dataGridViewCellStyle15.ForeColor = Color.FromArgb(202, 80, 16);
            dataGridViewCellStyle15.SelectionForeColor = Color.FromArgb(202, 80, 16);
            icoRechazar.DefaultCellStyle = dataGridViewCellStyle15;
            icoRechazar.HeaderText = "";
            icoRechazar.Name = "icoRechazar";
            icoRechazar.ReadOnly = true;
            icoRechazar.Resizable = DataGridViewTriState.False;
            icoRechazar.SortMode = DataGridViewColumnSortMode.NotSortable;
            icoRechazar.ToolTipText = "Rechazar";
            icoRechazar.Width = 36;
            // 
            // icoBaja
            // 
            icoBaja.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle16.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle16.Font = new Font("Segoe MDL2 Assets", 11F);
            dataGridViewCellStyle16.ForeColor = Color.FromArgb(196, 43, 28);
            dataGridViewCellStyle16.SelectionForeColor = Color.FromArgb(196, 43, 28);
            icoBaja.DefaultCellStyle = dataGridViewCellStyle16;
            icoBaja.HeaderText = "";
            icoBaja.Name = "icoBaja";
            icoBaja.ReadOnly = true;
            icoBaja.Resizable = DataGridViewTriState.False;
            icoBaja.SortMode = DataGridViewColumnSortMode.NotSortable;
            icoBaja.ToolTipText = "Dar de baja";
            icoBaja.Width = 36;
            // 
            // dgvPresupuestos
            // 
            dgvPresupuestos.AllowUserToAddRows = false;
            dgvPresupuestos.AllowUserToDeleteRows = false;
            dgvPresupuestos.AllowUserToResizeRows = false;
            dgvPresupuestos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvPresupuestos.BackgroundColor = Color.White;
            dgvPresupuestos.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle17.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle17.BackColor = Color.FromArgb(242, 242, 242);
            dataGridViewCellStyle17.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle17.ForeColor = Color.FromArgb(64, 64, 64);
            dataGridViewCellStyle17.Padding = new Padding(4, 0, 0, 0);
            dataGridViewCellStyle17.SelectionBackColor = Color.FromArgb(242, 242, 242);
            dataGridViewCellStyle17.SelectionForeColor = Color.FromArgb(64, 64, 64);
            dataGridViewCellStyle17.WrapMode = DataGridViewTriState.True;
            dgvPresupuestos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle17;
            dgvPresupuestos.ColumnHeadersHeight = 32;
            dgvPresupuestos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvPresupuestos.Columns.AddRange(new DataGridViewColumn[] { colNumero, colCliente, colFecha, colEstado, colTotal, icoDetalles, icoEditar, icoAprobar, icoRechazar, icoBaja });
            dataGridViewCellStyle18.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle18.BackColor = Color.White;
            dataGridViewCellStyle18.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle18.ForeColor = Color.Black;
            dataGridViewCellStyle18.Padding = new Padding(4, 0, 4, 0);
            dataGridViewCellStyle18.SelectionBackColor = Color.FromArgb(221, 235, 247);
            dataGridViewCellStyle18.SelectionForeColor = Color.Black;
            dataGridViewCellStyle18.WrapMode = DataGridViewTriState.False;
            dgvPresupuestos.DefaultCellStyle = dataGridViewCellStyle18;
            dgvPresupuestos.EnableHeadersVisualStyles = false;
            dgvPresupuestos.GridColor = Color.Gainsboro;
            dgvPresupuestos.Location = new Point(12, 130);
            dgvPresupuestos.MultiSelect = false;
            dgvPresupuestos.Name = "dgvPresupuestos";
            dgvPresupuestos.ReadOnly = true;
            dgvPresupuestos.RowHeadersVisible = false;
            dgvPresupuestos.RowTemplate.Height = 30;
            dgvPresupuestos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPresupuestos.Size = new Size(876, 378);
            dgvPresupuestos.TabIndex = 2;
            dgvPresupuestos.CellClick += dgvPresupuestos_CellClick;
            dgvPresupuestos.CellFormatting += dgvPresupuestos_CellFormatting;
            dgvPresupuestos.CellMouseEnter += dgvPresupuestos_CellMouseEnter;
            dgvPresupuestos.CellMouseLeave += dgvPresupuestos_CellMouseLeave;
            dgvPresupuestos.CellToolTipTextNeeded += dgvPresupuestos_CellToolTipTextNeeded;
            // 
            // lblCantidadRegistros
            // 
            lblCantidadRegistros.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblCantidadRegistros.AutoSize = true;
            lblCantidadRegistros.Location = new Point(12, 524);
            lblCantidadRegistros.Name = "lblCantidadRegistros";
            lblCantidadRegistros.Size = new Size(67, 15);
            lblCantidadRegistros.TabIndex = 3;
            lblCantidadRegistros.Text = "Registros: 0";
            // 
            // btnCerrar
            // 
            btnCerrar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCerrar.Location = new Point(788, 518);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(100, 30);
            btnCerrar.TabIndex = 3;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = true;
            btnCerrar.Click += btnCerrar_Click;
            // 
            // FrmPresupuestosVenta
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 560);
            Controls.Add(lblTitulo);
            Controls.Add(btnNuevo);
            Controls.Add(grpFiltros);
            Controls.Add(dgvPresupuestos);
            Controls.Add(lblCantidadRegistros);
            Controls.Add(btnCerrar);
            MinimumSize = new Size(916, 599);
            Name = "FrmPresupuestosVenta";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Presupuestos de Venta";
            Load += FrmPresupuestosVenta_Load;
            grpFiltros.ResumeLayout(false);
            grpFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPresupuestos).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblTitulo;
        private Button btnNuevo;
        private GroupBox grpFiltros;
        private Label lblFiltroCliente;
        private ComboBox cboFiltroCliente;
        private Label lblFiltroEstado;
        private ComboBox cboFiltroEstado;
        private CheckBox chkIncluirBajas;
        private DataGridViewTextBoxColumn colNumero;
        private DataGridViewTextBoxColumn colCliente;
        private DataGridViewTextBoxColumn colFecha;
        private DataGridViewTextBoxColumn colEstado;
        private DataGridViewTextBoxColumn colTotal;
        private DataGridViewTextBoxColumn icoDetalles;
        private DataGridViewTextBoxColumn icoEditar;
        private DataGridViewTextBoxColumn icoAprobar;
        private DataGridViewTextBoxColumn icoRechazar;
        private DataGridViewTextBoxColumn icoBaja;
        private DataGridView dgvPresupuestos;
        private Label lblCantidadRegistros;
        private Button btnCerrar;
    }
}
