using System;
using System.Drawing;
using System.Windows.Forms;

namespace ComestiblesDanLogistica
{
    public class Form1 : Form
    {
        private GestorCavaDan gestor = new GestorCavaDan();
        private ErrorProvider errorProvider = new ErrorProvider();
        private TabControl tabControl;

        // Pestaña 1: Recepción en Cava
        private TextBox txtCodigoLote;
        private ComboBox cmbProductoIngreso, cmbEstiba, cmbBahia;
        private NumericUpDown numUnidadesIngreso, numPesoUnitario;
        private DateTimePicker dtpVencimiento;
        private TableLayoutPanel panelMatrizVisual;

        // Pestaña 2: Inventario y Vida Útil
        private ComboBox cmbFiltroProducto;
        private Label lblTotalProducto;
        private DataGridView dgvInventario;

        // Pestaña 3: Pedidos y Despacho
        private TextBox txtIdPedido, txtCliente, txtDestino;
        private ComboBox cmbCanal, cmbLoteDisponible;
        private NumericUpDown numCantidadPedido, numHorasViaje;
        private Label lblHorasViaje;
        private DataGridView dgvPedidos;

        // Pestaña 4: Informe Cierre de Turno
        private TextBox txtInformeCierre;

        public Form1()
        {
            ConfigurarVentanaPrincipal();
            ConstruirPestanas();
            gestor.CargarDatosTxt();
            ActualizarTodasLasVistas();
        }

        private void ConfigurarVentanaPrincipal()
        {
            this.Text = "Comestibles Dan S.A. - Sistema de Control de Cava y Distribución Refrigerada";
            this.Size = new Size(1050, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9.5f);
            this.BackColor = Color.FromArgb(245, 247, 250);
            errorProvider.ContainerControl = this;

            Panel header = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = Color.FromArgb(153, 27, 27) };
            Label lblTitulo = new Label
            {
                Text = "COMESTIBLES DAN | Gestión de Cava de Alistamiento (4x5) y Rutas",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 16)
            };
            header.Controls.Add(lblTitulo);
            this.Controls.Add(header);

            tabControl = new TabControl { Dock = DockStyle.Fill, Padding = new Point(12, 6) };
            this.Controls.Add(tabControl);
            tabControl.BringToFront();
        }

        private void ConstruirPestanas()
        {
            TabPage tab1 = new TabPage("1. Recepción en Cava (Matriz 4x5)");
            ConstruirTabRecepcion(tab1);

            TabPage tab2 = new TabPage("2. Inventario y Vida Útil");
            ConstruirTabInventario(tab2);

            TabPage tab3 = new TabPage("3. Pedidos y Distribución");
            ConstruirTabPedidos(tab3);

            TabPage tab4 = new TabPage("4. Informe Cierre de Turno");
            ConstruirTabInforme(tab4);

            tabControl.TabPages.AddRange(new[] { tab1, tab2, tab3, tab4 });
        }

        private void ConstruirTabRecepcion(TabPage tab)
        {
            GroupBox grpForm = new GroupBox { Text = "Ingreso de Lote Terminado desde Planta", Location = new Point(15, 15), Size = new Size(360, 520) };

            grpForm.Controls.Add(new Label { Text = "Código de Lote:", Location = new Point(15, 30), AutoSize = true });
            txtCodigoLote = new TextBox { Location = new Point(15, 52), Width = 320, PlaceholderText = "Ej: L-2026-001" };

            grpForm.Controls.Add(new Label { Text = "Línea de Producto:", Location = new Point(15, 88), AutoSize = true });
            cmbProductoIngreso = new ComboBox { Location = new Point(15, 110), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbProductoIngreso.Items.AddRange(new[] { "Salchichas", "Mortadelas", "Jamones", "Chorizos", "Salchichones", "Tocinetas" });
            cmbProductoIngreso.SelectedIndex = 0;

            grpForm.Controls.Add(new Label { Text = "Cantidad de Unidades:", Location = new Point(15, 146), AutoSize = true });
            numUnidadesIngreso = new NumericUpDown { Location = new Point(15, 168), Width = 150, Minimum = 1, Maximum = 5000, Value = 100 };

            grpForm.Controls.Add(new Label { Text = "Peso Unitario (kg):", Location = new Point(185, 146), AutoSize = true });
            numPesoUnitario = new NumericUpDown { Location = new Point(185, 168), Width = 150, DecimalPlaces = 2, Increment = 0.25m, Minimum = 0.10m, Maximum = 50m, Value = 1.50m };

            grpForm.Controls.Add(new Label { Text = "Fecha de Vencimiento:", Location = new Point(15, 206), AutoSize = true });
            dtpVencimiento = new DateTimePicker { Location = new Point(15, 228), Width = 320, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(30) };

            grpForm.Controls.Add(new Label { Text = "Estiba (Fila 1-4):", Location = new Point(15, 268), AutoSize = true });
            cmbEstiba = new ComboBox { Location = new Point(15, 290), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbEstiba.Items.AddRange(new[] { "Estiba 1", "Estiba 2", "Estiba 3", "Estiba 4" });
            cmbEstiba.SelectedIndex = 0;

            grpForm.Controls.Add(new Label { Text = "Bahía de Carga (Col 1-5):", Location = new Point(185, 268), AutoSize = true });
            cmbBahia = new ComboBox { Location = new Point(185, 290), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbBahia.Items.AddRange(new[] { "Bahía 1", "Bahía 2", "Bahía 3", "Bahía 4", "Bahía 5" });
            cmbBahia.SelectedIndex = 0;

            Button btnRegistrar = new Button
            {
                Text = "Almacenar Lote en Cava",
                Location = new Point(15, 345),
                Size = new Size(320, 42),
                BackColor = Color.FromArgb(153, 27, 27),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };
            btnRegistrar.Click += BtnRegistrarLote_Click;

            grpForm.Controls.AddRange(new Control[] { txtCodigoLote, cmbProductoIngreso, numUnidadesIngreso, numPesoUnitario, dtpVencimiento, cmbEstiba, cmbBahia, btnRegistrar });

            GroupBox grpMatriz = new GroupBox { Text = "Mapa Físico de la Cava de Alistamiento (4 Estibas x 5 Bahías)", Location = new Point(390, 15), Size = new Size(620, 520) };
            panelMatrizVisual = new TableLayoutPanel { Location = new Point(15, 30), Size = new Size(590, 470), RowCount = 4, ColumnCount = 5 };
            for (int i = 0; i < 4; i++) panelMatrizVisual.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));
            for (int j = 0; j < 5; j++) panelMatrizVisual.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
            grpMatriz.Controls.Add(panelMatrizVisual);

            tab.Controls.AddRange(new Control[] { grpForm, grpMatriz });
        }

        private void ConstruirTabInventario(TabPage tab)
        {
            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 65 };
            topPanel.Controls.Add(new Label { Text = "Consultar stock por producto:", Location = new Point(15, 22), AutoSize = true });

            cmbFiltroProducto = new ComboBox { Location = new Point(200, 19), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFiltroProducto.Items.AddRange(new[] { "Todos", "Salchichas", "Mortadelas", "Jamones", "Chorizos", "Salchichones", "Tocinetas" });
            cmbFiltroProducto.SelectedIndex = 0;
            cmbFiltroProducto.SelectedIndexChanged += (s, e) => ActualizarTablaInventario();

            lblTotalProducto = new Label { Text = "Total disponible: 0 unidades", Location = new Point(425, 22), AutoSize = true, Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = Color.FromArgb(153, 27, 27) };
            topPanel.Controls.AddRange(new Control[] { cmbFiltroProducto, lblTotalProducto });

            dgvInventario = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White
            };
            dgvInventario.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "Lote" },
                new DataGridViewTextBoxColumn { HeaderText = "Producto" },
                new DataGridViewTextBoxColumn { HeaderText = "Ubicación Cava" },
                new DataGridViewTextBoxColumn { HeaderText = "Unidades" },
                new DataGridViewTextBoxColumn { HeaderText = "Peso Total (kg)" },
                new DataGridViewTextBoxColumn { HeaderText = "Vencimiento" },
                new DataGridViewTextBoxColumn { HeaderText = "Vida Útil (Días)" },
                new DataGridViewTextBoxColumn { HeaderText = "Estado Rotación" }
            );

            tab.Controls.Add(dgvInventario);
            tab.Controls.Add(topPanel);
        }

        private void ConstruirTabPedidos(TabPage tab)
        {
            GroupBox grpPedido = new GroupBox { Text = "Recepción y Validación Polimórfica de Pedidos", Location = new Point(15, 15), Size = new Size(350, 520) };

            grpPedido.Controls.Add(new Label { Text = "ID Pedido:", Location = new Point(15, 28), AutoSize = true });
            txtIdPedido = new TextBox { Location = new Point(15, 48), Width = 315, PlaceholderText = "Ej: PED-101" };

            grpPedido.Controls.Add(new Label { Text = "Cliente / Punto de Venta:", Location = new Point(15, 82), AutoSize = true });
            txtCliente = new TextBox { Location = new Point(15, 102), Width = 315, PlaceholderText = "Ej: Supermercado La Vaquita" };

            grpPedido.Controls.Add(new Label { Text = "Canal Comercial (Polimorfismo):", Location = new Point(15, 136), AutoSize = true });
            cmbCanal = new ComboBox { Location = new Point(15, 156), Width = 315, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCanal.Items.AddRange(new[] { "Tienda Urbana (Mín. 12 días)", "Población Intermunicipal (Mín. 28 días)" });
            cmbCanal.SelectedIndex = 0;
            cmbCanal.SelectedIndexChanged += (s, e) =>
            {
                bool esPoblacion = cmbCanal.SelectedIndex == 1;
                lblHorasViaje.Visible = esPoblacion;
                numHorasViaje.Visible = esPoblacion;
            };

            grpPedido.Controls.Add(new Label { Text = "Lote en Cava a Despachar:", Location = new Point(15, 190), AutoSize = true });
            cmbLoteDisponible = new ComboBox { Location = new Point(15, 210), Width = 315, DropDownStyle = ComboBoxStyle.DropDownList };

            grpPedido.Controls.Add(new Label { Text = "Unidades Solicitadas:", Location = new Point(15, 244), AutoSize = true });
            numCantidadPedido = new NumericUpDown { Location = new Point(15, 264), Width = 315, Minimum = 1, Maximum = 5000, Value = 20 };

            grpPedido.Controls.Add(new Label { Text = "Zona Urbana / Municipio:", Location = new Point(15, 298), AutoSize = true });
            txtDestino = new TextBox { Location = new Point(15, 318), Width = 315, PlaceholderText = "Ej: Envigado / Rionegro" };

            lblHorasViaje = new Label { Text = "Horas de Viaje Refrigerado:", Location = new Point(15, 352), AutoSize = true, Visible = false };
            numHorasViaje = new NumericUpDown { Location = new Point(15, 372), Width = 315, DecimalPlaces = 1, Increment = 0.5m, Minimum = 0.5m, Maximum = 24m, Value = 2.5m, Visible = false };

            Button btnProcesarPedido = new Button
            {
                Text = "Validar y Despachar Pedido",
                Location = new Point(15, 420),
                Size = new Size(315, 42),
                BackColor = Color.FromArgb(21, 128, 61),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };
            btnProcesarPedido.Click += BtnProcesarPedido_Click;

            grpPedido.Controls.AddRange(new Control[] { txtIdPedido, txtCliente, cmbCanal, cmbLoteDisponible, numCantidadPedido, txtDestino, lblHorasViaje, numHorasViaje, btnProcesarPedido });

            dgvPedidos = new DataGridView
            {
                Location = new Point(380, 22),
                Size = new Size(630, 513),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            dgvPedidos.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID" },
                new DataGridViewTextBoxColumn { HeaderText = "Canal" },
                new DataGridViewTextBoxColumn { HeaderText = "Cliente" },
                new DataGridViewTextBoxColumn { HeaderText = "Producto" },
                new DataGridViewTextBoxColumn { HeaderText = "Unds" },
                new DataGridViewTextBoxColumn { HeaderText = "Peso (kg)" },
                new DataGridViewTextBoxColumn { HeaderText = "Flete COP" },
                new DataGridViewTextBoxColumn { HeaderText = "Estado / Motivo" }
            );

            tab.Controls.AddRange(new Control[] { grpPedido, dgvPedidos });
        }

        private void ConstruirTabInforme(TabPage tab)
        {
            Button btnGenerar = new Button
            {
                Text = "Actualizar Informe de Cierre de Turno y Alertas",
                Location = new Point(15, 15),
                Size = new Size(340, 38),
                BackColor = Color.FromArgb(30, 58, 138),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
            btnGenerar.Click += (s, e) => txtInformeCierre.Text = gestor.GenerarInformeCierreTurno();

            txtInformeCierre = new TextBox
            {
                Location = new Point(15, 65),
                Size = new Size(995, 470),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 10.5f),
                BackColor = Color.White
            };

            tab.Controls.AddRange(new Control[] { btnGenerar, txtInformeCierre });
        }

        private void BtnRegistrarLote_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            if (string.IsNullOrWhiteSpace(txtCodigoLote.Text))
            {
                errorProvider.SetError(txtCodigoLote, "Ingrese el código del lote.");
                MessageBox.Show("Debe ingresar un código de lote válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dtpVencimiento.Value.Date <= DateTime.Today)
            {
                errorProvider.SetError(dtpVencimiento, "La fecha debe ser futura.");
                MessageBox.Show("No se permite ingresar producto vencido o con vencimiento hoy.", "Control de Calidad", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoteInventario lote = new LoteInventario(
                txtCodigoLote.Text.Trim(),
                cmbProductoIngreso.SelectedItem.ToString(),
                (int)numUnidadesIngreso.Value,
                (double)numPesoUnitario.Value,
                dtpVencimiento.Value,
                cmbEstiba.SelectedIndex,
                cmbBahia.SelectedIndex
            );

            if (gestor.RegistrarLote(lote, out string msg))
            {
                MessageBox.Show(msg, "Ingreso Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtCodigoLote.Clear();
                ActualizarTodasLasVistas();
            }
            else
            {
                MessageBox.Show(msg, "Ubicación Ocupada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnProcesarPedido_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            if (string.IsNullOrWhiteSpace(txtIdPedido.Text) || string.IsNullOrWhiteSpace(txtCliente.Text) || string.IsNullOrWhiteSpace(txtDestino.Text))
            {
                MessageBox.Show("Complete todos los campos del pedido (ID, Cliente y Destino).", "Campos Requeridos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmbLoteDisponible.SelectedItem == null)
            {
                MessageBox.Show("No hay lotes disponibles en la cava para despachar.", "Sin Inventario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ItemComboLote itemSeleccionado = (ItemComboLote)cmbLoteDisponible.SelectedItem;
            LoteInventario lote = itemSeleccionado.Lote;

            PedidoDistribucion pedido;
            switch (cmbCanal.SelectedIndex)
            {
                case 0:
                    pedido = new PedidoTiendaUrbana(txtIdPedido.Text.Trim(), txtCliente.Text.Trim(), lote.Producto, (int)numCantidadPedido.Value, 0, "", 0, txtDestino.Text.Trim());
                    break;
                default:
                    pedido = new PedidoPoblacion(txtIdPedido.Text.Trim(), txtCliente.Text.Trim(), lote.Producto, (int)numCantidadPedido.Value, 0, "", 0, txtDestino.Text.Trim(), (double)numHorasViaje.Value);
                    break;
            }

            bool aprobado = gestor.ProcesarPedido(pedido, lote.Estiba, lote.Bahia, out string mensaje);
            ActualizarTodasLasVistas();

            if (aprobado)
            {
                MessageBox.Show($"{mensaje}\nCosto logístico calculado: ${pedido.CalcularCostoLogistico():N0} COP", "Pedido Aprobado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtIdPedido.Clear();
                txtCliente.Clear();
                txtDestino.Clear();
            }
            else
            {
                MessageBox.Show($"PEDIDO RECHAZADO:\n{mensaje}", "Alerta de Rotación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarTodasLasVistas()
        {
            panelMatrizVisual.Controls.Clear();
            LoteInventario[,] matriz = gestor.ObtenerMatrizCava();
            for (int f = 0; f < 4; f++)
            {
                for (int c = 0; c < 5; c++)
                {
                    LoteInventario l = matriz[f, c];
                    Panel celda = new Panel { Dock = DockStyle.Fill, Margin = new Padding(4), BorderStyle = BorderStyle.FixedSingle };
                    Label lbl = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 8f) };

                    if (l == null || l.Unidades == 0)
                    {
                        celda.BackColor = Color.FromArgb(241, 245, 249);
                        lbl.Text = $"E{f + 1}-B{c + 1}\n[LIBRE]";
                        lbl.ForeColor = Color.Gray;
                    }
                    else
                    {
                        int dias = l.DiasVidaUtil();
                        celda.BackColor = dias < 12 ? Color.FromArgb(254, 202, 202) : (dias < 28 ? Color.FromArgb(254, 240, 138) : Color.FromArgb(187, 247, 208));
                        lbl.Text = $"E{f + 1}-B{c + 1}\n{l.Producto}\n{l.Unidades} und ({dias}d)";
                    }
                    celda.Controls.Add(lbl);
                    panelMatrizVisual.Controls.Add(celda, c, f);
                }
            }

            ActualizarTablaInventario();
            cmbLoteDisponible.Items.Clear();
            foreach (LoteInventario l in gestor.ObtenerLotesActivos())
            {
                cmbLoteDisponible.Items.Add(new ItemComboLote(l));
            }
            if (cmbLoteDisponible.Items.Count > 0) cmbLoteDisponible.SelectedIndex = 0;

            dgvPedidos.Rows.Clear();
            foreach (PedidoDistribucion p in gestor.ObtenerPedidos())
            {
                dgvPedidos.Rows.Add(
                    p.IdPedido,
                    p.TipoCanal(),
                    p.Cliente,
                    p.Producto,
                    p.CantidadUnidades,
                    p.PesoTotalKg.ToString("N1"),
                    p.Aprobado ? $"${p.CalcularCostoLogistico():N0}" : "$0",
                    p.MotivoRechazo
                );
            }

            txtInformeCierre.Text = gestor.GenerarInformeCierreTurno();
        }

        private void ActualizarTablaInventario()
        {
            dgvInventario.Rows.Clear();
            string filtro = cmbFiltroProducto.SelectedItem?.ToString() ?? "Todos";

            foreach (LoteInventario l in gestor.ObtenerLotesActivos())
            {
                if (filtro == "Todos" || l.Producto == filtro)
                {
                    dgvInventario.Rows.Add(
                        l.CodigoLote,
                        l.Producto,
                        $"Estiba {l.Estiba + 1} - Bahía {l.Bahia + 1}",
                        l.Unidades,
                        l.PesoTotalKg().ToString("N2"),
                        l.FechaVencimiento.ToString("yyyy-MM-dd"),
                        l.DiasVidaUtil(),
                        l.EstadoFrescura()
                    );
                }
            }

            if (filtro == "Todos")
            {
                int totalGeneral = 0;
                foreach (LoteInventario l in gestor.ObtenerLotesActivos()) totalGeneral += l.Unidades;
                lblTotalProducto.Text = $"Total general en cava: {totalGeneral:N0} unidades";
            }
            else
            {
                lblTotalProducto.Text = $"Total {filtro}: {gestor.ConsultarUnidadesPorProducto(filtro):N0} unidades";
            }
        }

        private class ItemComboLote
        {
            public LoteInventario Lote { get; }
            public ItemComboLote(LoteInventario lote) { Lote = lote; }
            public override string ToString() => $"[E{Lote.Estiba + 1}-B{Lote.Bahia + 1}] {Lote.Producto} - {Lote.Unidades} und ({Lote.DiasVidaUtil()}d)";
        }
    }
}