using AccesoDatos.Data;
using AccesoDatos.Models;
using AccesoDatos.Repositories;

namespace AppWinForms;

// Pantalla de ABM de Socios: alta, baja (lógica), modificación y consulta.
// El formulario NO arma consultas: le pide todo al repositorio.
public class FormSocios : Form
{
    private readonly SocioRepository _repositorio;

    // Id del socio elegido en la grilla. null = se está cargando un socio nuevo.
    private int? _socioId = null;

    // ---------- Controles ----------
    private readonly GroupBox grpDatos = new()
    {
        Text = "Datos del socio",
        Location = new Point(12, 12),
        Size = new Size(880, 120)
    };

    private readonly TextBox txtNombre = new() { Location = new Point(100, 27), Width = 200, MaxLength = 50 };
    private readonly TextBox txtApellido = new() { Location = new Point(410, 27), Width = 200, MaxLength = 50 };
    private readonly TextBox txtDni = new() { Location = new Point(100, 69), Width = 200, MaxLength = 8 };
    private readonly TextBox txtTelefono = new() { Location = new Point(410, 69), Width = 200, MaxLength = 20 };

    private readonly Button btnGuardar = new() { Text = "Guardar", Location = new Point(650, 24), Size = new Size(100, 32) };
    private readonly Button btnNuevo = new() { Text = "Nuevo", Location = new Point(760, 24), Size = new Size(100, 32) };
    private readonly Button btnCambiarEstado = new() { Text = "Dar de baja", Location = new Point(650, 66), Size = new Size(210, 32) };

    private readonly TextBox txtBuscar = new() { Location = new Point(170, 147), Width = 250 };
    private readonly CheckBox chkSoloActivos = new()
    {
        Text = "Solo socios activos",
        Location = new Point(440, 149),
        AutoSize = true,
        Checked = true
    };

    private readonly DataGridView dgvSocios = new();

    public FormSocios(AplicacionDbContext context)
    {
        _repositorio = new SocioRepository(context);

        Text = "Gimnasio - Socios";
        ClientSize = new Size(904, 530);
        StartPosition = FormStartPosition.CenterScreen;

        ConfigurarGrilla();

        // Todo lo del cuadro "Datos del socio"
        grpDatos.Controls.AddRange(new Control[]
        {
            Etiqueta("Nombre:", 20, 30), txtNombre,
            Etiqueta("Apellido:", 330, 30), txtApellido,
            Etiqueta("DNI:", 20, 72), txtDni,
            Etiqueta("Teléfono:", 330, 72), txtTelefono,
            btnGuardar, btnNuevo, btnCambiarEstado
        });

        Controls.AddRange(new Control[]
        {
            grpDatos,
            Etiqueta("Buscar (DNI o apellido):", 12, 150), txtBuscar, chkSoloActivos,
            dgvSocios
        });

        // Eventos (se conectan después de armar todo para que no se disparen antes de tiempo)
        btnGuardar.Click += btnGuardar_Click;
        btnNuevo.Click += (_, _) => LimpiarFormulario();
        btnCambiarEstado.Click += btnCambiarEstado_Click;
        txtBuscar.TextChanged += (_, _) => CargarGrilla();
        chkSoloActivos.CheckedChanged += (_, _) => CargarGrilla();
        dgvSocios.CellClick += dgvSocios_CellClick;

        Load += (_, _) => CargarGrilla();
    }

    // ---------- Armado de controles ----------
    private static Label Etiqueta(string texto, int x, int y)
    {
        return new Label { Text = texto, Location = new Point(x, y), AutoSize = true };
    }

    private void ConfigurarGrilla()
    {
        dgvSocios.Location = new Point(12, 185);
        dgvSocios.Size = new Size(880, 330);
        dgvSocios.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvSocios.ReadOnly = true;
        dgvSocios.AllowUserToAddRows = false;
        dgvSocios.AllowUserToDeleteRows = false;
        dgvSocios.MultiSelect = false;
        dgvSocios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvSocios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvSocios.RowHeadersVisible = false;
    }

    // ---------- Consulta ----------
    private void CargarGrilla()
    {
        try
        {
            var socios = _repositorio.Buscar(txtBuscar.Text.Trim(), chkSoloActivos.Checked);

            // Mostramos solo las columnas que interesan (sin las listas de navegación)
            dgvSocios.DataSource = socios
                .Select(s => new
                {
                    s.Id,
                    s.Nombre,
                    s.Apellido,
                    s.Dni,
                    s.Telefono,
                    Alta = s.FechaAlta.ToString("dd/MM/yyyy"),
                    Estado = s.Activo ? "Activo" : "Inactivo"
                })
                .ToList();

            var columnaId = dgvSocios.Columns["Id"];
            if (columnaId != null)
            {
                columnaId.Visible = false;
            }

            dgvSocios.ClearSelection();
        }
        catch (Exception ex)
        {
            MostrarError($"No se pudo cargar la lista de socios: {ex.Message}");
        }
    }

    // Al hacer clic en una fila, se cargan sus datos en el formulario para modificarlos.
    private void dgvSocios_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return; // clic en el encabezado
        }

        int id = Convert.ToInt32(dgvSocios.Rows[e.RowIndex].Cells["Id"].Value);
        var socio = _repositorio.ObtenerPorId(id);
        if (socio == null)
        {
            MostrarError("El socio seleccionado ya no existe.");
            CargarGrilla();
            return;
        }

        _socioId = socio.Id;
        txtNombre.Text = socio.Nombre;
        txtApellido.Text = socio.Apellido;
        txtDni.Text = socio.Dni;
        txtTelefono.Text = socio.Telefono;

        btnGuardar.Text = "Guardar cambios";
        btnCambiarEstado.Text = socio.Activo ? "Dar de baja" : "Reactivar";
    }

    // ---------- Alta y modificación ----------
    private void btnGuardar_Click(object? sender, EventArgs e)
    {
        string nombre = txtNombre.Text.Trim();
        string apellido = txtApellido.Text.Trim();
        string dni = txtDni.Text.Trim();
        string telefono = txtTelefono.Text.Trim();

        // Validaciones básicas
        if (nombre == "" || apellido == "" || dni == "")
        {
            MostrarError("Nombre, apellido y DNI son obligatorios.");
            return;
        }

        if (!dni.All(char.IsDigit) || dni.Length < 7 || dni.Length > 8)
        {
            MostrarError("El DNI debe tener 7 u 8 números, sin puntos.");
            return;
        }

        if (telefono != "" && !telefono.All(char.IsDigit))
        {
            MostrarError("El teléfono solo puede tener números.");
            return;
        }

        try
        {
            // No se permiten dos socios con el mismo DNI
            var existente = _repositorio.ObtenerPorDni(dni);
            if (existente != null && existente.Id != _socioId)
            {
                MostrarError("Ya existe un socio con ese DNI.");
                return;
            }

            if (_socioId == null)
            {
                // ALTA
                var nuevo = new Socio
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    Dni = dni,
                    Telefono = telefono,
                    FechaAlta = DateTime.Today,
                    Activo = true
                };
                _repositorio.agregar(nuevo);
                MostrarAviso("Socio registrado correctamente.");
            }
            else
            {
                // MODIFICACIÓN: se busca el socio, se cambian sus datos y se guarda
                var socio = _repositorio.ObtenerPorId(_socioId.Value);
                if (socio == null)
                {
                    MostrarError("El socio ya no existe.");
                    CargarGrilla();
                    LimpiarFormulario();
                    return;
                }

                socio.Nombre = nombre;
                socio.Apellido = apellido;
                socio.Dni = dni;
                socio.Telefono = telefono;
                _repositorio.modificar(socio);
                MostrarAviso("Datos actualizados correctamente.");
            }

            CargarGrilla();
            LimpiarFormulario();
        }
        catch (Exception ex)
        {
            MostrarError($"No se pudo guardar: {ex.Message}");
        }
    }

    // ---------- Baja (lógica) y reactivación ----------
    // No se borra el registro: se marca Activo = false para conservar su historial.
    private void btnCambiarEstado_Click(object? sender, EventArgs e)
    {
        if (_socioId == null)
        {
            MostrarAviso("Primero seleccioná un socio de la lista.");
            return;
        }

        try
        {
            var socio = _repositorio.ObtenerPorId(_socioId.Value);
            if (socio == null)
            {
                MostrarError("El socio ya no existe.");
                CargarGrilla();
                LimpiarFormulario();
                return;
            }

            string accion = socio.Activo ? "dar de baja" : "reactivar";
            var respuesta = MessageBox.Show(
                $"¿Querés {accion} a {socio.Nombre} {socio.Apellido}?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
            {
                return;
            }

            socio.Activo = !socio.Activo;
            _repositorio.modificar(socio);

            CargarGrilla();
            LimpiarFormulario();
        }
        catch (Exception ex)
        {
            MostrarError($"No se pudo cambiar el estado: {ex.Message}");
        }
    }

    // ---------- Utilidades ----------
    private void LimpiarFormulario()
    {
        _socioId = null;
        txtNombre.Clear();
        txtApellido.Clear();
        txtDni.Clear();
        txtTelefono.Clear();
        btnGuardar.Text = "Guardar";
        btnCambiarEstado.Text = "Dar de baja";
        dgvSocios.ClearSelection();
        txtNombre.Focus();
    }

    private static void MostrarAviso(string mensaje)
    {
        MessageBox.Show(mensaje, "Gimnasio", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private static void MostrarError(string mensaje)
    {
        MessageBox.Show(mensaje, "Gimnasio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}