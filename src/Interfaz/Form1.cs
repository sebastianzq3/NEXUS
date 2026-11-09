using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace Interfaz
{
    public partial class Form1 : Form
    {
        // --- ESTADO GLOBAL DE LA SESIÓN ---
        private readonly string directorioSaves = "Saves";
        private List<(string Ruta, DatosPartida Datos)> perfilesCargados = new List<(string, DatosPartida)>();

        private Usuario cadete = null;
        private Realidad realidadAsignada = null;
        private string rutaGuardadoActual = "";

        private Queue<EntidadIris> enjambreIris = new Queue<EntidadIris>();
        private EntidadIris irisActual = null;
        private bool parpadeoAlerta = false;
        private bool mostrandoDialogoIris = false;

        // --- ESTADO DE LA EXPEDICIÓN ACTUAL (MAPA 40x20) ---
        // false = En Campamento (Mapa congelado, opciones del Hub habilitadas)
        // true  = Explorando Sector (Mapa activo, opciones del Hub bloqueadas)
        private bool enExpedicion = false;
        private int px = 0;
        private int py = 0;
        private int contadorPasos = 0;

        // Objetivo secundario de expedición
        private readonly string[] nombresObj = { "Exterminador (Mata Enemigos Ж)", "Curioso (Visita Eventos '?')", "Chatarrero (Recoge Restos ▤)" };
        private int tipoObj = 0;
        private int metaObj = 3;
        private int progresoObj = 0;
        private bool objCompletado = false;

        public Form1()
        {
            InitializeComponent();
            ConfigurarEventos();
            InicializarSistemaPerfiles();
        }

        private void ConfigurarEventos()
        {
            // Eventos de Perfiles
            this.btnCargarPerfil.Click += BtnCargarPerfil_Click;
            this.btnCrearPerfil.Click += BtnCrearPerfil_Click;
            this.btnPurgarPerfil.Click += BtnPurgarPerfil_Click;

            // Eventos de Comandos del Panel Táctico
            this.btnExtraerSector.Click += (s, e) => IntentarVolverAlCampamento();
            this.btnNuevaIncursion.Click += (s, e) => IniciarIncursion();
            this.btnInventario.Click += (s, e) => AbrirModalInventario(false);
            this.btnRecargarEnergia.Click += (s, e) => AbrirModalSoporteVital(false);
            this.btnTienda.Click += (s, e) => AbrirModalTienda();
            this.btnManualEstado.Click += (s, e) => AbrirModalManualYEstado();
            this.btnAtlas.Click += (s, e) => AbrirModalAtlas();
            this.btnExtraerRealidad.Click += (s, e) => IniciarExtraccionRealidad();
            this.btnDesconectar.Click += (s, e) => DesconectarYVolverAPerfiles();

            // Eventos del Mapa (Pintado GDI+, Teclado y Ratón)
            this.canvasMapa.Paint += CanvasMapa_Paint;
            this.canvasMapa.MouseClick += CanvasMapa_MouseClick;
            this.KeyDown += Form1_KeyDown;

            // Timer de I.R.I.S.
            this.timerIris.Tick += TimerIris_Tick;
        }

        // =====================================================================
        // 1. GESTIÓN DE PERFILES Y GUARDADO MULTI-SLOT (JSON)
        // =====================================================================
        private void InicializarSistemaPerfiles()
        {
            if (!Directory.Exists(directorioSaves))
                Directory.CreateDirectory(directorioSaves);

            dgvPerfiles.Columns.Clear();
            dgvPerfiles.Columns.Add("ColNombre", "DESIGNACIÓN (CADETE)");
            dgvPerfiles.Columns.Add("ColEdad", "EDAD");
            dgvPerfiles.Columns.Add("ColNivel", "NIVEL");
            dgvPerfiles.Columns.Add("ColVida", "VIDA");
            dgvPerfiles.Columns.Add("ColEnergia", "ENERGÍA");
            dgvPerfiles.Columns.Add("ColRealidad", "SECTOR ACTUAL");
            dgvPerfiles.Columns.Add("ColEstabilidad", "ESTABILIDAD");

            RefrescarTablaPerfiles();
        }

        private void RefrescarTablaPerfiles()
        {
            dgvPerfiles.Rows.Clear();
            perfilesCargados.Clear();

            string[] archivos = Directory.GetFiles(directorioSaves, "*.json");
            foreach (string archivo in archivos)
            {
                try
                {
                    DatosPartida dp = JsonSerializer.Deserialize<DatosPartida>(File.ReadAllText(archivo));
                    if (dp != null && dp.Jugador != null && dp.RealidadActual != null)
                    {
                        dp.RealidadActual.InicializarMapaSiNoExiste();
                        perfilesCargados.Add((archivo, dp));
                        dgvPerfiles.Rows.Add(
                            dp.Jugador.Nombre,
                            $"{dp.Jugador.Edad} años",
                            $"Nv. {dp.Jugador.Nivel} ({dp.Jugador.Experiencia}/100)",
                            $"{dp.Jugador.Vida}/{dp.Jugador.VidaMax}",
                            $"{dp.Jugador.Energia}/{dp.Jugador.EnergiaMax}",
                            dp.RealidadActual.Nombre,
                            $"{dp.RealidadActual.Estabilidad}%"
                        );
                    }
                }
                catch
                {
                    perfilesCargados.Add((archivo, null));
                    dgvPerfiles.Rows.Add($"[CORRUPTO: {Path.GetFileName(archivo)}]", "-", "-", "-", "-", "-", "-");
                }
            }
        }

        private void BtnCargarPerfil_Click(object sender, EventArgs e)
        {
            if (dgvPerfiles.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un perfil en la tabla para cargarlo.", "NEXUS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idx = dgvPerfiles.SelectedRows[0].Index;
            var seleccionado = perfilesCargados[idx];
            if (seleccionado.Datos == null)
            {
                MessageBox.Show("El archivo seleccionado está dañado y no puede sincronizarse.", "ERROR DE DATOS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cadete = seleccionado.Datos.Jugador;
            realidadAsignada = seleccionado.Datos.RealidadActual;
            realidadAsignada.InicializarMapaSiNoExiste();
            if (!Realidad.ListaRealidades.Contains(realidadAsignada))
                Realidad.ListaRealidades.Add(realidadAsignada);

            rutaGuardadoActual = seleccionado.Ruta;
            EntrarAlSimulador($"[SISTEMA]: Perfil de {cadete.Nombre} recuperado exitosamente. Modo Campamento activo.");
        }

        private void BtnCrearPerfil_Click(object sender, EventArgs e)
        {
            string nombre = txtNuevoNombre.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Ingrese una designación (Nombre) válida para el explorador.", "VERIFICACIÓN BIOMÉTRICA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int edad = (int)numNuevaEdad.Value;
            cadete = new Usuario(nombre, edad);
            realidadAsignada = new Realidad();
            rutaGuardadoActual = Path.Combine(directorioSaves, $"{cadete.Nombre}.json");

            GuardarJuego();
            txtNuevoNombre.Clear();
            EntrarAlSimulador($"[ACCESO CONCEDIDO]: Bienvenido, Explorador {cadete.Nombre} ({cadete.Edad} años). Anclaje establecido en {realidadAsignada.Nombre}.");
        }

        private void BtnPurgarPerfil_Click(object sender, EventArgs e)
        {
            if (dgvPerfiles.SelectedRows.Count == 0) return;

            int idx = dgvPerfiles.SelectedRows[0].Index;
            string ruta = perfilesCargados[idx].Ruta;

            if (MessageBox.Show($"¿Confirmas la purga definitiva del archivo '{Path.GetFileName(ruta)}' del multiverso?",
                "PURGA DE DATOS", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                File.Delete(ruta);
                RefrescarTablaPerfiles();
            }
        }

        private void GuardarJuego()
        {
            if (cadete == null || realidadAsignada == null || string.IsNullOrEmpty(rutaGuardadoActual)) return;
            DatosPartida datos = new DatosPartida { Jugador = cadete, RealidadActual = realidadAsignada };
            var opciones = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(rutaGuardadoActual, JsonSerializer.Serialize(datos, opciones));
        }

        private void EntrarAlSimulador(string mensajeInicial)
        {
            enjambreIris.Clear();
            enjambreIris.Enqueue(new EntidadIris("Cazador-Alfa"));
            enjambreIris.Enqueue(new EntidadIris("Purificador-Omega"));
            irisActual = enjambreIris.Peek();

            pnlPerfiles.Visible = false;
            pnlJuego.Visible = true;
            rtbLog.Clear();
            AgregarLog(mensajeInicial, Color.SpringGreen);

            // Posicionamos en la base pero iniciamos en el Campamento (mapa congelado)
            realidadAsignada.InicializarMapaSiNoExiste();
            px = realidadAsignada.CadeteStartX;
            py = realidadAsignada.CadeteStartY;
            enExpedicion = false;

            ActualizarHUD();
            canvasMapa.Invalidate();
            timerIris.Start();
            this.Focus();
        }

        private void DesconectarYVolverAPerfiles()
        {
            if (enExpedicion) return;
            GuardarJuego();
            timerIris.Stop();
            enExpedicion = false;
            pnlJuego.Visible = false;
            pnlPerfiles.Visible = true;
            RefrescarTablaPerfiles();
        }

        // =====================================================================
        // 2. EXPLORACIÓN TÁCTICA EN GRILLA 40x20 Y EVENTOS DE CASILLA
        // =====================================================================
        private void IniciarIncursion()
        {
            if (enExpedicion) return;

            if (cadete.Energia <= 0)
            {
                MessageBox.Show("No puedes iniciar una exploración con 0 de Energía.\nUtiliza 'Recuperar energía' en el campamento primero.",
                    "SIN ENERGÍA VITAL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            realidadAsignada.RellenarEntidadesLocales(cadete.Nivel);
            px = realidadAsignada.CadeteStartX;
            py = realidadAsignada.CadeteStartY;
            contadorPasos = 0;

            Random rnd = new Random();
            tipoObj = rnd.Next(3);
            metaObj = tipoObj == 1 ? 2 : 3;
            progresoObj = 0;
            objCompletado = false;
            enExpedicion = true;

            AgregarLog($"Desplegado en '{realidadAsignada.Nombre}'. Objetivo extra: {nombresObj[tipoObj]}.", Color.Cyan);
            ActualizarHUD();
            canvasMapa.Invalidate();
            this.Focus();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!pnlJuego.Visible || !enExpedicion) return;

            int nx = px, ny = py;
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up) ny--;
            else if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down) ny++;
            else if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left) nx--;
            else if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right) nx++;
            else if (e.KeyCode == Keys.Space)
            {
                IntentarVolverAlCampamento();
                e.Handled = true;
                return;
            }
            else return;

            e.Handled = true;
            ProcesarMovimiento(nx, ny);
        }

        private void CanvasMapa_MouseClick(object sender, MouseEventArgs e)
        {
            if (!enExpedicion || realidadAsignada == null) return;

            float cellW = (float)canvasMapa.ClientSize.Width / realidadAsignada.AnchoMapa;
            float cellH = (float)canvasMapa.ClientSize.Height / realidadAsignada.AltoMapa;

            int clickX = (int)(e.X / cellW);
            int clickY = (int)(e.Y / cellH);

            // Permitir mover si hace clic en una casilla adyacente ortogonal
            if (Math.Abs(clickX - px) + Math.Abs(clickY - py) == 1)
            {
                ProcesarMovimiento(clickX, clickY);
            }
            this.Focus();
        }

        private void ProcesarMovimiento(int nx, int ny)
        {
            if (!enExpedicion) return;
            if (nx < 0 || nx >= realidadAsignada.AnchoMapa || ny < 0 || ny >= realidadAsignada.AltoMapa)
                return;

            char destino = realidadAsignada.MapaLocal[ny, nx];

            // Evento sorpresa '?'
            if (destino == '?')
            {
                char[] opcionesRnd = { '♡', '▤', 'Ж', 'Φ', '಄', '☖' };
                destino = opcionesRnd[new Random().Next(opcionesRnd.Length)];
                realidadAsignada.MapaLocal[ny, nx] = destino;
                AgregarLog($"¡El evento sorpresa '?' resultó ser '{destino}'!", Color.Gold);
                if (tipoObj == 1 && !objCompletado) progresoObj++;
            }

            // Bloqueo por muros del vacío cuántico
            if (destino == '█' || destino == '▓' || destino == '▒' || destino == '░')
            {
                AgregarLog("El vacío cuántico es denso. No puedes avanzar por ahí.", Color.LightCoral);
                return;
            }

            // Movimiento válido
            px = nx;
            py = ny;
            contadorPasos++;

            if (contadorPasos >= 3)
            {
                cadete.Energia--;
                contadorPasos = 0;

                if (cadete.Energia <= 0 && destino != 'Φ')
                {
                    enExpedicion = false;
                    ActualizarHUD();
                    canvasMapa.Invalidate();
                    AgregarLog("¡ENERGÍA AGOTADA! Soporte vital colapsado. De vuelta en el campamento.", Color.Red);
                    MessageBox.Show("¡ENERGÍA AGOTADA!\nEl soporte vital colapsó en medio de la exploración.\nSistemas de extracción de emergencia activados: has sido devuelto al campamento.",
                        "SISTEMA DAÑADO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    GuardarJuego();
                    return;
                }
            }

            // Resolución de casilla pisada
            if (destino == 'Φ')
            {
                realidadAsignada.MapaLocal[py, px] = '·';
                AgregarLog("Núcleo de Energía (Φ) detectado. Iniciando protocolo de recarga automática...", Color.SpringGreen);
                ActualizarHUD();
                canvasMapa.Invalidate();
                AbrirModalSoporteVital(true);
            }
            else if (destino == '♡')
            {
                int cura = new Random().Next(20, 36);
                cadete.Vida += cura;
                realidadAsignada.MapaLocal[py, px] = '·';
                AgregarLog($"Regeneración biológica aplicada (+{cura} Vida).", Color.SpringGreen);
            }
            else if (destino == '▤')
            {
                cadete.Restos++;
                realidadAsignada.MapaLocal[py, px] = '·';
                AgregarLog("Has recolectado Restos Tecnológicos (+1 ▤).", Color.Orange);
                if (tipoObj == 2 && !objCompletado) progresoObj++;
            }
            else if (destino == '☖')
            {
                AgregarLog("Campamento temporal (☖) alcanzado. Puedes 'Volver al campamento' [ESPACIO].", Color.SpringGreen);
            }
            else if (destino == '☗')
            {
                AgregarLog("Ancla Base (☗) alcanzada. Puedes 'Volver al campamento' [ESPACIO].", Color.SpringGreen);
            }
            else if (destino == '಄')
            {
                AgregarLog("¡Fisura de Anomalía (಄) detectada! Selecciona un objeto para intentar sellarla.", Color.Violet);
                ActualizarHUD();
                canvasMapa.Invalidate();
                AbrirModalInventario(true);
            }
            else if (destino == 'Ж')
            {
                AgregarLog("¡Emboscada viral! Entrando en combate táctico...", Color.Red);
                ActualizarHUD();
                canvasMapa.Invalidate();
                AbrirModalCombate(nx, ny);
            }

            // Verificar objetivo secundario
            if (progresoObj >= metaObj && !objCompletado)
            {
                objCompletado = true;
                cadete.Experiencia += 75;
                AgregarLog("¡OBJETIVO EXTRA DE EXPEDICIÓN COMPLETADO! (+75 EXP).", Color.SpringGreen);
            }

            // Si tras pisar Φ sigue con 0 de energía, forzar regreso al campamento
            if (enExpedicion && cadete.Energia <= 0)
            {
                enExpedicion = false;
                AgregarLog("Sin energía restante tras el protocolo. De vuelta al campamento.", Color.Red);
            }

            VerificarTurnoIris();
            ActualizarHUD();
            canvasMapa.Invalidate();
            GuardarJuego();
        }

        private void IntentarVolverAlCampamento()
        {
            if (!enExpedicion) return;

            char tileActual = realidadAsignada.MapaLocal[py, px];
            if (tileActual == '☗')
            {
                enExpedicion = false;
                AgregarLog("Has vuelto al Campamento Base (☗). Mapa congelado y sistemas de gestión habilitados.", Color.SpringGreen);
                ActualizarHUD();
                canvasMapa.Invalidate();
                GuardarJuego();
            }
            else if (tileActual == '☖')
            {
                realidadAsignada.MapaLocal[py, px] = '·';
                enExpedicion = false;
                AgregarLog("Has vuelto al campamento usando un Campamento Temporal (☖). Mapa congelado.", Color.SpringGreen);
                ActualizarHUD();
                canvasMapa.Invalidate();
                GuardarJuego();
            }
            else
            {
                AgregarLog("Interferencia: Solo puedes volver al campamento desde una Base (☗) o Campamento (☖).", Color.Gold);
            }
            this.Focus();
        }

        // =====================================================================
        // 3. RENDERIZADO GDI+ DEL MAPA 40x20
        // =====================================================================
        private void CanvasMapa_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(10, 12, 18));

            if (realidadAsignada == null || realidadAsignada.MapaLocal == null) return;

            int cols = realidadAsignada.AnchoMapa;
            int rows = realidadAsignada.AltoMapa;
            float cellW = (float)canvasMapa.ClientSize.Width / cols;
            float cellH = (float)canvasMapa.ClientSize.Height / rows;

            using Font fontTile = new Font("Consolas", 11F, FontStyle.Bold);
            using Pen gridPen = new Pen(Color.FromArgb(20, 26, 36), 1F);
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    RectangleF rect = new RectangleF(x * cellW, y * cellH, cellW, cellH);
                    char tile = realidadAsignada.MapaLocal[y, x];

                    // Fondo de celda según tipo
                    if (tile == '█' || tile == '▓' || tile == '▒' || tile == '░')
                    {
                        int shade = tile == '█' ? 42 : (tile == '▓' ? 34 : (tile == '▒' ? 28 : 22));
                        using SolidBrush bMuro = new SolidBrush(Color.FromArgb(shade, shade + 4, shade + 12));
                        g.FillRectangle(bMuro, rect);
                    }
                    else
                    {
                        g.DrawRectangle(gridPen, rect.X, rect.Y, rect.Width, rect.Height);
                    }

                    // Dibujar Cadete o Entidad
                    if (x == px && y == py)
                    {
                        using SolidBrush bPlayerBg = new SolidBrush(enExpedicion ? Color.FromArgb(0, 110, 60) : Color.FromArgb(0, 70, 100));
                        g.FillEllipse(bPlayerBg, rect.X + 1, rect.Y + 2, rect.Width - 2, rect.Height - 4);
                        g.DrawString("@", fontTile, Brushes.SpringGreen, rect, sf);
                    }
                    else if (tile != '·')
                    {
                        Brush colorGlifo = Brushes.Gray;
                        switch (tile)
                        {
                            case '☗': colorGlifo = Brushes.Cyan; break;
                            case '☖': colorGlifo = Brushes.Gold; break;
                            case '಄': colorGlifo = Brushes.Magenta; break;
                            case '♡': colorGlifo = Brushes.Crimson; break;
                            case '▤': colorGlifo = Brushes.Orange; break;
                            case 'Ж': colorGlifo = Brushes.Red; break;
                            case 'Φ': colorGlifo = Brushes.SpringGreen; break;
                            case '?': colorGlifo = Brushes.White; break;
                        }
                        g.DrawString(tile.ToString(), fontTile, colorGlifo, rect, sf);
                    }
                    else
                    {
                        using SolidBrush bDot = new SolidBrush(Color.FromArgb(45, 55, 70));
                        g.FillRectangle(bDot, rect.X + cellW / 2 - 1, rect.Y + cellH / 2 - 1, 2, 2);
                    }
                }
            }

            // Si está en el campamento (mapa congelado), mostrar velo e indicador táctico
            if (!enExpedicion)
            {
                using SolidBrush velo = new SolidBrush(Color.FromArgb(115, 8, 12, 20));
                g.FillRectangle(velo, 0, 0, canvasMapa.ClientSize.Width, canvasMapa.ClientSize.Height);

                Rectangle bannerRect = new Rectangle(140, canvasMapa.ClientSize.Height / 2 - 32, 560, 64);
                using SolidBrush bannerBg = new SolidBrush(Color.FromArgb(230, 16, 24, 38));
                using Pen bannerPen = new Pen(Color.Cyan, 2F);
                g.FillRectangle(bannerBg, bannerRect);
                g.DrawRectangle(bannerPen, bannerRect);

                using Font fBanner = new Font("Consolas", 11F, FontStyle.Bold);
                g.DrawString("EN CAMPAMENTO // MAPA CONGELADO\nPulsa 'Explorar realidad' en el Panel Táctico para desplegarte",
                    fBanner, Brushes.Cyan, bannerRect, sf);
            }

            // Borde de alerta si I.R.I.S. está en estado crítico o acechando
            if (parpadeoAlerta && irisActual != null && irisActual.EstadoActual != EstadoIris.Dormida)
            {
                Color cAlerta = irisActual.EstadoActual == EstadoIris.InvasionCritica ? Color.Red : Color.OrangeRed;
                using Pen pAlerta = new Pen(cAlerta, 3F);
                g.DrawRectangle(pAlerta, 1, 1, canvasMapa.ClientSize.Width - 3, canvasMapa.ClientSize.Height - 3);
            }
        }

        // =====================================================================
        // 4. MODAL DE COMBATE TÁCTICO POR TURNOS
        // =====================================================================
        private void AbrirModalCombate(int enemX, int enemY)
        {
            Enemigo enemigo = new Enemigo();
            bool bloqueoActivo = false;

            using Form frmCombate = new Form
            {
                Text = $"ALERTA DE COMBATE // {enemigo.Nombre.ToUpper()}",
                Size = new Size(560, 440),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ControlBox = false,
                BackColor = Color.FromArgb(18, 14, 22),
                ForeColor = Color.White,
                Font = new Font("Consolas", 10F)
            };

            Label lblEnemigo = new Label { Bounds = new Rectangle(25, 20, 500, 26), ForeColor = Color.FromArgb(255, 90, 90), Font = new Font("Consolas", 12F, FontStyle.Bold) };
            ProgressBar pbEnemigo = new ProgressBar { Bounds = new Rectangle(25, 50, 495, 22), Maximum = enemigo.VidaMax, Value = enemigo.Vida };

            Label lblCadete = new Label { Bounds = new Rectangle(25, 88, 500, 26), ForeColor = Color.SpringGreen, Font = new Font("Consolas", 11F, FontStyle.Bold) };
            ProgressBar pbCadete = new ProgressBar { Bounds = new Rectangle(25, 118, 495, 22), Maximum = cadete.VidaMax, Value = cadete.Vida };

            RichTextBox rtbCombate = new RichTextBox
            {
                Bounds = new Rectangle(25, 155, 495, 120),
                BackColor = Color.FromArgb(10, 10, 15),
                ForeColor = Color.Cyan,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                Text = $"¡Un {enemigo.Nombre} hostil bloquea tu paso!\nPoder ofensivo enemigo: {enemigo.DañoBase} | Probabilidad de escape: {enemigo.ProbabilidadEscape}%"
            };

            Button btnLigero = CrearBotonEstilizado("Ataque Ligero (0 E)", new Rectangle(25, 290, 240, 42), Color.FromArgb(0, 120, 140));
            Button btnPesado = CrearBotonEstilizado("Ataque Pesado (1 E)", new Rectangle(280, 290, 240, 42), Color.FromArgb(160, 50, 60));
            Button btnBloquear = CrearBotonEstilizado("Bloquear (0 E | -50% Daño)", new Rectangle(25, 342, 240, 42), Color.FromArgb(70, 90, 130));
            Button btnEscapar = CrearBotonEstilizado($"Escapar (1 E | {enemigo.ProbabilidadEscape}%)", new Rectangle(280, 342, 240, 42), Color.FromArgb(140, 110, 30));

            void RefrescarBarrasCombate()
            {
                lblEnemigo.Text = $"ENEMIGO: {enemigo.Nombre} (VIDA: {enemigo.Vida}/{enemigo.VidaMax} | DAÑO: {enemigo.DañoBase})";
                pbEnemigo.Value = Math.Max(0, Math.Min(enemigo.VidaMax, enemigo.Vida));
                lblCadete.Text = $"CADETE: {cadete.Nombre} (VIDA: {cadete.Vida}/{cadete.VidaMax} | ENERGÍA: {cadete.Energia}/{cadete.EnergiaMax})";
                pbCadete.Value = Math.Max(0, Math.Min(cadete.VidaMax, cadete.Vida));
            }

            void EjecutarTurno(string cmd)
            {
                bool turnoEnemigo = true;
                string msj = "";

                if (cmd == "1")
                {
                    int dmg = cadete.DañoBase;
                    enemigo.Vida -= dmg;
                    msj = $"Ejecutas un ataque ligero causando {dmg} de daño.";
                }
                else if (cmd == "2")
                {
                    if (cadete.Energia >= 1)
                    {
                        cadete.Energia--;
                        int dmg = (int)(cadete.DañoBase * 2.5);
                        enemigo.Vida -= dmg;
                        msj = $"¡Golpe pesado y contundente causando {dmg} de daño!";
                    }
                    else msj = "Sin energía para ataque pesado. Pierdes el turno.";
                }
                else if (cmd == "3")
                {
                    bloqueoActivo = true;
                    msj = "Levantas tu guardia, preparándote para el impacto.";
                }
                else if (cmd == "4")
                {
                    if (cadete.Energia >= 1)
                    {
                        cadete.Energia--;
                        if (new Random().Next(0, 100) < enemigo.ProbabilidadEscape)
                        {
                            AgregarLog($"Lograste escapar del {enemigo.Nombre}.", Color.Gold);
                            frmCombate.Close();
                            return;
                        }
                        else msj = "Intentaste escapar, pero el enemigo te acorraló.";
                    }
                    else msj = "Sin energía suficiente para escapar.";
                }

                // ¿Murió el enemigo?
                if (enemigo.Vida <= 0)
                {
                    cadete.BolsaFragmentos++;
                    realidadAsignada.MapaLocal[enemY, enemX] = '·';
                    if (tipoObj == 0 && !objCompletado) progresoObj++;
                    RefrescarBarrasCombate();
                    AgregarLog($"¡{enemigo.Nombre} eliminado! Has ganado +1 Fragmento de Realidad.", Color.SpringGreen);
                    MessageBox.Show($"¡El {enemigo.Nombre} colapsa!\nHas ganado +1 Fragmento de Realidad.", "VICTORIA TÁCTICA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frmCombate.Close();
                    return;
                }

                // Turno del enemigo
                if (turnoEnemigo)
                {
                    int dmgRecibido = enemigo.DañoBase;
                    if (bloqueoActivo)
                    {
                        dmgRecibido /= 2;
                        msj += $"\nEl enemigo ataca, pero tu bloqueo reduce el impacto a {dmgRecibido} de daño.";
                    }
                    else
                    {
                        msj += $"\nEl enemigo te golpea causando {dmgRecibido} de daño.";
                    }
                    cadete.Vida -= dmgRecibido;
                }
                bloqueoActivo = false;
                rtbCombate.Text = msj;
                RefrescarBarrasCombate();

                if (cadete.Vida <= 0)
                {
                    MessageBox.Show("¡TUS SIGNOS VITALES HAN COLAPSADO!\nConexión neuronal terminada.", "GAME OVER", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cadete.Vida = cadete.VidaMax;
                    cadete.Energia = cadete.EnergiaMax;
                    enExpedicion = false;
                    frmCombate.Close();
                    DesconectarYVolverAPerfiles();
                }
            }

            btnLigero.Click += (s, e) => EjecutarTurno("1");
            btnPesado.Click += (s, e) => EjecutarTurno("2");
            btnBloquear.Click += (s, e) => EjecutarTurno("3");
            btnEscapar.Click += (s, e) => EjecutarTurno("4");

            RefrescarBarrasCombate();
            frmCombate.Controls.AddRange(new Control[] { lblEnemigo, pbEnemigo, lblCadete, pbCadete, rtbCombate, btnLigero, btnPesado, btnBloquear, btnEscapar });
            frmCombate.ShowDialog(this);
        }

        // =====================================================================
        // 5. MODAL DE INVENTARIO, CRAFTEO Y SELLADO DE ANOMALÍAS (CORREGIDO)
        // =====================================================================
        private void AbrirModalInventario(bool sobreAnomalia)
        {
            if (!sobreAnomalia && enExpedicion) return;

            using Form frmInv = new Form
            {
                Text = sobreAnomalia ? "INTERFAZ DE MANIPULACIÓN // SELLAR ANOMALÍA (಄)" : "SISTEMA DE GESTIÓN Y SÍNTESIS (INVENTARIO)",
                Size = new Size(760, 510),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(16, 20, 28),
                ForeColor = Color.White,
                Font = new Font("Consolas", 10F)
            };

            Label lblRec = new Label { Bounds = new Rectangle(20, 15, 705, 25), ForeColor = Color.Cyan, Font = new Font("Consolas", 11F, FontStyle.Bold) };

            ListBox lstObjetos = new ListBox
            {
                Bounds = new Rectangle(20, 50, 320, 220),
                BackColor = Color.FromArgb(24, 30, 42),
                ForeColor = Color.SpringGreen,
                BorderStyle = BorderStyle.FixedSingle
            };

            RichTextBox rtbDetalle = new RichTextBox
            {
                Bounds = new Rectangle(355, 50, 370, 155),
                BackColor = Color.FromArgb(12, 15, 22),
                ForeColor = Color.White,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            bool hayAnomaliaActiva = sobreAnomalia || (enExpedicion && realidadAsignada.MapaLocal[py, px] == '಄');

            Button btnUsarAnomalia = CrearBotonEstilizado(
                hayAnomaliaActiva ? "USAR EN ANOMALÍA (-2 E)" : "USAR (Solo en Anomalía ಄)",
                new Rectangle(355, 218, 235, 52),
                Color.FromArgb(140, 40, 160));

            Button btnDescartar = CrearBotonEstilizado("DESCARTAR", new Rectangle(600, 218, 125, 52), Color.FromArgb(160, 45, 45));

            Label lblPistasTitulo = new Label { Text = "=== OBSERVACIONES (PISTAS DE LA REALIDAD ACTUAL) ===", Bounds = new Rectangle(20, 282, 700, 22), ForeColor = Color.Gold };
            ListBox lstPistas = new ListBox
            {
                Bounds = new Rectangle(20, 308, 705, 85),
                BackColor = Color.FromArgb(24, 30, 42),
                ForeColor = Color.Gold,
                BorderStyle = BorderStyle.FixedSingle
            };

            Button btnCraftPista = CrearBotonEstilizado("Craftear Pista (Coste: 3 Fragmentos)", new Rectangle(20, 408, 340, 42), Color.FromArgb(0, 120, 150));
            Button btnCraftObjeto = CrearBotonEstilizado("Craftear Objeto (Coste: 3 Restos ▤)", new Rectangle(385, 408, 340, 42), Color.FromArgb(180, 110, 20));

            bool actualizandoLista = false;

            // Función 1: Solo actualiza el panel derecho según el ítem elegido (SIN borrar la lista)
            void ActualizarDetalleSeleccion()
            {
                int idx = lstObjetos.SelectedIndex;
                if (idx >= 0 && idx < cadete.Inventario.Count)
                {
                    Objeto sel = cadete.Inventario[idx];
                    rtbDetalle.Text =
                        $"=== ANÁLISIS DE EQUIPO ===\n" +
                        $"Nombre:   {sel.Nombre}\n" +
                        $"Usos:     {sel.Usos} restantes\n" +
                        $"Afinidad: Contrarresta [{sel.Contrarresta}]\n\n" +
                        $"Info: {sel.Descripcion}";

                    ConfigurarEstadoBoton(btnDescartar, true, Color.FromArgb(160, 45, 45));
                    ConfigurarEstadoBoton(btnUsarAnomalia, hayAnomaliaActiva, Color.FromArgb(140, 40, 160));
                }
                else
                {
                    rtbDetalle.Text = cadete.Inventario.Count == 0
                        ? "Tu mochila está vacía.\nRecolecta 3 Restos Tecnológicos (▤) para ensamblar un nuevo objeto."
                        : "Haz clic sobre un objeto de la lista izquierda para analizarlo, descartarlo o utilizarlo.";

                    ConfigurarEstadoBoton(btnDescartar, false, Color.FromArgb(160, 45, 45));
                    ConfigurarEstadoBoton(btnUsarAnomalia, false, Color.FromArgb(140, 40, 160));
                }
            }

            // Función 2: Recarga las listas de ítems y pistas preservando el índice seleccionado
            void RecargarListasInventario(int indiceDeseado = 0)
            {
                actualizandoLista = true;
                lblRec.Text = $"FRAGMENTOS: {cadete.BolsaFragmentos}  |  RESTOS (▤): {cadete.Restos}  |  MOCHILA: {cadete.Inventario.Count}/{cadete.CapacidadInventario}  |  ENERGÍA: {cadete.Energia}";

                lstObjetos.Items.Clear();
                for (int i = 0; i < cadete.Inventario.Count; i++)
                {
                    lstObjetos.Items.Add($"{i + 1}. {cadete.Inventario[i].Nombre} (Usos: {cadete.Inventario[i].Usos})");
                }

                lstPistas.Items.Clear();
                if (cadete.Observaciones.Count == 0)
                    lstPistas.Items.Add("No tienes pistas registradas de esta realidad.");
                else
                    foreach (string p in cadete.Observaciones) lstPistas.Items.Add($"• {p}");

                if (cadete.Inventario.Count > 0)
                {
                    int idxValido = Math.Max(0, Math.Min(indiceDeseado, cadete.Inventario.Count - 1));
                    lstObjetos.SelectedIndex = idxValido;
                }

                actualizandoLista = false;
                ActualizarDetalleSeleccion();
            }

            lstObjetos.SelectedIndexChanged += (s, e) =>
            {
                if (!actualizandoLista)
                    ActualizarDetalleSeleccion();
            };

            btnCraftPista.Click += (s, e) =>
            {
                if (cadete.BolsaFragmentos < 3)
                {
                    MessageBox.Show("No tienes suficientes Fragmentos de Anomalía (Necesitas 3).", "ERROR DE SÍNTESIS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                cadete.BolsaFragmentos -= 3;
                string[] pistasTiempo = { "Miras tu reloj y las manecillas giran frenéticamente en reversa.", "La lluvia se detiene en el aire frente a tus ojos, congelada.", "Una planta brota y se convierte en polvo en un solo parpadeo." };
                string[] pistasEspacio = { "Caminas diez metros en línea recta y regresas a tu origen.", "Un pilar a lo lejos se encoge hasta caber en tu mano al acercarte.", "El camino se bifurca en tres direcciones que llevan a la misma roca." };
                string[] pistasMente = { "Un recuerdo de infancia aflora, pero le pertenece a otra persona.", "Las sombras en tu visión toman formas humanoides decepcionadas.", "Las letras del menú parpadean y se vuelven símbolos arcanos." };
                string[] pistasSilencio = { "Pisas una rama seca. Se rompe, pero el crujido es un vacío absoluto.", "Gritas con todas tus fuerzas, pero no sale sonido de tu garganta.", "La quietud es tan antinatural que sientes presión en los oídos." };

                Random rndP = new Random();
                string pistaSel = realidadAsignada.Anomalia switch
                {
                    Anomalia.Tiempo => pistasTiempo[rndP.Next(pistasTiempo.Length)],
                    Anomalia.Espacio => pistasEspacio[rndP.Next(pistasEspacio.Length)],
                    Anomalia.Mente => pistasMente[rndP.Next(pistasMente.Length)],
                    _ => pistasSilencio[rndP.Next(pistasSilencio.Length)]
                };
                cadete.Observaciones.Add(pistaSel);
                AgregarLog($"Nueva pista sintetizada: \"{pistaSel}\"", Color.Gold);
                RecargarListasInventario(lstObjetos.SelectedIndex);
                ActualizarHUD();
            };

            btnCraftObjeto.Click += (s, e) =>
            {
                if (cadete.Restos < 3)
                {
                    MessageBox.Show("No tienes suficientes Restos Tecnológicos (Necesitas 3).", "ERROR DE INGENIERÍA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cadete.Inventario.Count >= cadete.CapacidadInventario)
                {
                    MessageBox.Show("Tu mochila está llena. Descarta un objeto o amplía tu inventario en la Tienda.", "INVENTARIO LLENO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                cadete.Restos -= 3;
                Objeto nuevo = new Objeto();
                cadete.RecogerObjeto(nuevo);
                AgregarLog($"Objeto ensamblado: {nuevo.Nombre} (Contrarresta {nuevo.Contrarresta}).", Color.SpringGreen);
                RecargarListasInventario(cadete.Inventario.Count - 1);
                ActualizarHUD();
            };

            btnDescartar.Click += (s, e) =>
            {
                int idx = lstObjetos.SelectedIndex;
                if (idx < 0 || idx >= cadete.Inventario.Count) return;

                Objeto obj = cadete.Inventario[idx];
                if (MessageBox.Show($"¿Descartar y destruir '{obj.Nombre}'?", "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cadete.DescartarObjeto(obj);
                    AgregarLog($"{obj.Nombre} fue destruido y descartado del inventario.", Color.LightCoral);
                    RecargarListasInventario(idx - 1);
                    ActualizarHUD();
                }
            };

            btnUsarAnomalia.Click += (s, e) =>
            {
                int idx = lstObjetos.SelectedIndex;
                if (idx < 0 || idx >= cadete.Inventario.Count)
                {
                    MessageBox.Show("Selecciona primero un objeto de la lista.", "NEXUS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (cadete.Energia < 2)
                {
                    MessageBox.Show("Energía insuficiente para manipular la anomalía (Requiere 2 E).", "NEXUS ADVIERTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                cadete.Energia -= 2;
                Objeto objUsado = cadete.Inventario[idx];
                objUsado.Usos--;

                if (objUsado.Contrarresta == realidadAsignada.Anomalia)
                {
                    realidadAsignada.Estabilidad += 30;
                    realidadAsignada.MapaLocal[py, px] = '·';
                    AgregarLog($"[ÉXITO]: '{objUsado.Nombre}' resonó con la anomalía. Fisura sellada (+30% Estabilidad).", Color.SpringGreen);
                    MessageBox.Show($"[ÉXITO]: La frecuencia de '{objUsado.Nombre}' resuena perfectamente.\nLa fisura cuántica se cierra (+30% Estabilidad).", "FISURA SELLADA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    realidadAsignada.Estabilidad -= 15;
                    AgregarLog($"[INEFICAZ]: '{objUsado.Nombre}' no corresponde a esta anomalía (-15% Estabilidad).", Color.Red);
                    MessageBox.Show($"[INEFICAZ]: ¡Error! Tu interferencia con '{objUsado.Nombre}' empeoró la situación (-15% Estabilidad).", "INTERFERENCIA FALLIDA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (objUsado.Usos <= 0)
                {
                    cadete.DescartarObjeto(objUsado);
                    AgregarLog($"El límite de integridad de '{objUsado.Nombre}' llegó a cero y se desintegró.", Color.Gold);
                    MessageBox.Show($"El límite de integridad de '{objUsado.Nombre}' llegó a cero. Se ha desintegrado.", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                ActualizarHUD();
                canvasMapa.Invalidate();
                frmInv.Close();
            };

            RecargarListasInventario(0);

            frmInv.Controls.AddRange(new Control[]
            {
                lblRec, lstObjetos, rtbDetalle, btnUsarAnomalia, btnDescartar,
                lblPistasTitulo, lstPistas, btnCraftPista, btnCraftObjeto
            });
            frmInv.ShowDialog(this);
            GuardarJuego();
            this.Focus();
        }

        // =====================================================================
        // 6. MODAL DE SOPORTE VITAL (MINIJUEGOS DE RECARGA DE ENERGÍA)
        // =====================================================================
        private void AbrirModalSoporteVital(bool desdeCasillaMapa)
        {
            // Solo se permite desde el campamento (!enExpedicion) o automáticamente al pisar 'Φ'
            if (!desdeCasillaMapa && enExpedicion) return;

            if (!desdeCasillaMapa && cadete.Energia >= cadete.EnergiaMax)
            {
                MessageBox.Show("Los niveles de energía ya están al máximo.", "SOPORTE VITAL", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Random rnd = new Random();
            int tipoJuego = rnd.Next(1, 5);
            string tituloPrueba = "";
            string pregunta = "";
            string respuestaCorrecta = "";

            if (tipoJuego == 1)
            {
                string[] preLore = { "Soy la IA que desertó. ¿Cuál es mi nombre?", "Mi flujo retrocede, marchito lo que nace. ¿Qué anomalía soy?", "Protocolo: Introduce el nombre registrado de tu perfil." };
                string[] resLore = { "iris", "tiempo", cadete.Nombre.ToLower() };
                int iL = rnd.Next(preLore.Length);
                tituloPrueba = "PRUEBA DE CORDURA";
                pregunta = preLore[iL];
                respuestaCorrecta = resLore[iL];
            }
            else if (tipoJuego == 2)
            {
                tituloPrueba = "CALIBRACIÓN MATEMÁTICA";
                pregunta = "Completa la secuencia cuántica:\n2  -  4  -  8  -  16  -  ?";
                respuestaCorrecta = "32";
            }
            else if (tipoJuego == 3)
            {
                tituloPrueba = "FILTRO ANTIVIRUS (MEMORIZACIÓN)";
                pregunta = "MEMORIZA EL CÓDIGO EN 3 SEGUNDOS:\n>>>  N-3-X-U-5  <<<";
                respuestaCorrecta = "n-3-x-u-5";
            }
            else
            {
                tituloPrueba = "SINCRONIZACIÓN DE DATOS";
                pregunta = "Ordena el anagrama del sistema:\nA  O  L  N  A  I  A  M";
                respuestaCorrecta = "anomalia";
            }

            using Form frmMJ = new Form
            {
                Text = $"SOPORTE VITAL // {tituloPrueba}",
                Size = new Size(500, 280),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(18, 22, 32),
                ForeColor = Color.White,
                Font = new Font("Consolas", 10.5F)
            };

            Label lblPreg = new Label
            {
                Text = pregunta,
                ForeColor = Color.Gold,
                Bounds = new Rectangle(25, 25, 435, 75),
                TextAlign = ContentAlignment.MiddleCenter
            };

            TextBox txtResp = new TextBox
            {
                Bounds = new Rectangle(80, 115, 320, 30),
                BackColor = Color.FromArgb(28, 34, 48),
                ForeColor = Color.SpringGreen,
                TextAlign = HorizontalAlignment.Center
            };

            Button btnEnviar = CrearBotonEstilizado("VERIFICAR PROTOCOLO", new Rectangle(120, 165, 240, 45), Color.FromArgb(0, 140, 110));

            if (tipoJuego == 3)
            {
                txtResp.Enabled = false;
                btnEnviar.Enabled = false;
                System.Windows.Forms.Timer tMem = new System.Windows.Forms.Timer { Interval = 2500 };
                tMem.Tick += (s, e) =>
                {
                    tMem.Stop();
                    lblPreg.Text = "[ CÓDIGO BORRADO DE PANTALLA ]\nIngresa la secuencia exacta:";
                    txtResp.Enabled = true;
                    btnEnviar.Enabled = true;
                    txtResp.Focus();
                };
                tMem.Start();
            }

            btnEnviar.Click += (s, e) =>
            {
                bool ganado = txtResp.Text.Trim().ToLower() == respuestaCorrecta;
                if (ganado)
                {
                    cadete.Energia += 5;
                    AgregarLog("[SOPORTE VITAL]: Autorización aceptada (+5 Energía recuperada).", Color.SpringGreen);
                    MessageBox.Show("[AUTORIZACIÓN ACEPTADA]: +5 Energía recuperada.", "SOPORTE VITAL", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (realidadAsignada.Estabilidad > 50)
                {
                    cadete.Energia += 1;
                    AgregarLog("[SOPORTE VITAL]: Fallo de filtro. Extracción mínima (+1 Energía).", Color.Gold);
                    MessageBox.Show("[DENEGADO]: El sistema apenas extrajo (+1 Energía).\nLa realidad resistió el impacto.", "SOPORTE VITAL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    cadete.Energia += 1;
                    realidadAsignada.Estabilidad -= 15;
                    AgregarLog("[CRÍTICO]: Falla en soporte vital drenó la estabilidad (-15% Estabilidad, +1 Energía).", Color.Red);
                    MessageBox.Show("Falla grave. La anomalía drenó la estabilidad del sector (-15% Estabilidad).", "CRÍTICO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                frmMJ.Close();
            };

            frmMJ.AcceptButton = btnEnviar;
            frmMJ.Controls.AddRange(new Control[] { lblPreg, txtResp, btnEnviar });
            frmMJ.ShowDialog(this);
            VerificarTurnoIris();
            ActualizarHUD();
            GuardarJuego();
            this.Focus();
        }

        // =====================================================================
        // 7. MODAL DE TIENDA DE PROFICIENCIA
        // =====================================================================
        private void AbrirModalTienda()
        {
            if (enExpedicion) return;

            using Form frmTienda = new Form
            {
                Text = "TIENDA DE PROFICIENCIA NEXUS // MERCADO DE SOFTWARE",
                Size = new Size(620, 340),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(20, 18, 30),
                ForeColor = Color.White,
                Font = new Font("Consolas", 10F)
            };

            Label lblPts = new Label
            {
                Bounds = new Rectangle(25, 20, 560, 30),
                ForeColor = Color.Violet,
                Font = new Font("Consolas", 12F, FontStyle.Bold)
            };

            Button btnUpInv = CrearBotonEstilizado("", new Rectangle(25, 65, 555, 55), Color.FromArgb(50, 80, 130));
            Button btnUpVida = CrearBotonEstilizado("", new Rectangle(25, 135, 555, 55), Color.FromArgb(130, 45, 70));
            Button btnUpEne = CrearBotonEstilizado("", new Rectangle(25, 205, 555, 55), Color.FromArgb(30, 120, 100));

            void ActualizarTextosTienda()
            {
                int cInv = cadete.CalcularCosto(cadete.UpgradesInv);
                int cVida = cadete.CalcularCosto(cadete.UpgradesVida);
                int cEne = cadete.CalcularCosto(cadete.UpgradesEnergia);

                lblPts.Text = $"PUNTOS DE PROFICIENCIA DISPONIBLES: {cadete.PuntosProficiencia} PTS";
                btnUpInv.Text = $"Módulo de Almacenamiento (Inv. Max +1) | Nv: {cadete.UpgradesInv} | Coste: {cInv} pts";
                btnUpVida.Text = $"Refuerzo de Biomasa (Vida Max +20)     | Nv: {cadete.UpgradesVida} | Coste: {cVida} pts";
                btnUpEne.Text = $"Batería Cuántica (Energía Max +2)      | Nv: {cadete.UpgradesEnergia} | Coste: {cEne} pts";
            }

            btnUpInv.Click += (s, e) =>
            {
                int c = cadete.CalcularCosto(cadete.UpgradesInv);
                if (cadete.PuntosProficiencia >= c)
                {
                    cadete.PuntosProficiencia -= c;
                    cadete.UpgradesInv++;
                    cadete.CapacidadInventario++;
                    AgregarLog($"Mejora adquirida: Capacidad de Mochila aumentada a {cadete.CapacidadInventario}.", Color.SpringGreen);
                    ActualizarTextosTienda();
                    ActualizarHUD();
                }
                else MessageBox.Show("Puntos de proficiencia insuficientes.", "TRANSACCIÓN RECHAZADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            btnUpVida.Click += (s, e) =>
            {
                int c = cadete.CalcularCosto(cadete.UpgradesVida);
                if (cadete.PuntosProficiencia >= c)
                {
                    cadete.PuntosProficiencia -= c;
                    cadete.UpgradesVida++;
                    cadete.VidaMax += 20;
                    cadete.Vida += 20;
                    AgregarLog($"Mejora adquirida: Vida Máxima aumentada a {cadete.VidaMax}.", Color.SpringGreen);
                    ActualizarTextosTienda();
                    ActualizarHUD();
                }
                else MessageBox.Show("Puntos de proficiencia insuficientes.", "TRANSACCIÓN RECHAZADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            btnUpEne.Click += (s, e) =>
            {
                int c = cadete.CalcularCosto(cadete.UpgradesEnergia);
                if (cadete.PuntosProficiencia >= c)
                {
                    cadete.PuntosProficiencia -= c;
                    cadete.UpgradesEnergia++;
                    cadete.EnergiaMax += 2;
                    cadete.Energia += 2;
                    AgregarLog($"Mejora adquirida: Energía Máxima aumentada a {cadete.EnergiaMax}.", Color.SpringGreen);
                    ActualizarTextosTienda();
                    ActualizarHUD();
                }
                else MessageBox.Show("Puntos de proficiencia insuficientes.", "TRANSACCIÓN RECHAZADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            ActualizarTextosTienda();
            frmTienda.Controls.AddRange(new Control[] { lblPts, btnUpInv, btnUpVida, btnUpEne });
            frmTienda.ShowDialog(this);
            GuardarJuego();
            this.Focus();
        }

        // =====================================================================
        // 8. MODAL DE ESTADO Y MANUAL DEL SIMULADOR
        // =====================================================================
        private void AbrirModalManualYEstado()
        {
            if (enExpedicion) return;

            string eval = realidadAsignada.Estabilidad >= 80 ? "ESTABLE" : "INESTABLE";
            string texto =
                $"=== ESTADO DEL SISTEMA ===\n" +
                $"• Explorador: {cadete.Nombre} ({cadete.Edad} años)\n" +
                $"• Nivel del Cadete: {cadete.Nivel} (Experiencia: {cadete.Experiencia}/100)\n" +
                $"• Evaluación de la realidad ({realidadAsignada.Nombre}): {eval} ({realidadAsignada.Estabilidad}%)\n\n" +
                $"=== BASE DE DATOS: MANUAL DE SUPERVIVENCIA 2.0 ===\n\n" +
                $"1. SUPERVIVENCIA Y EXTRACCIÓN:\n" +
                $"Cada 3 pasos consumes 1 de Energía. Si llega a cero, tu soporte vital colapsa. Recarga energía pisando los Núcleos (Φ) o desde el Campamento. Para congelar el mapa y habilitar las opciones del campamento, sitúate sobre el Ancla Base Permanente (☗) o un Campamento Temporal (☖) y presiona ESPACIO o 'Volver al campamento'.\n\n" +
                $"2. RECOLECCIÓN Y SÍNTESIS:\n" +
                $"Encuentra Restos Tecnológicos (▤) y gasta 3 en el Inventario para ensamblar un Objeto. Elimina Enemigos (Ж) para obtener Fragmentos y gasta 3 para decodificar una Pista Sensorial.\n\n" +
                $"3. COMBATE TÁCTICO:\n" +
                $"Usa Ataques Ligeros (0 E), Golpes Pesados (1 E), Bloquear (reduce daño a la mitad) o Escapar (1 E).\n\n" +
                $"4. SELLANDO LA ANOMALÍA:\n" +
                $"Pisa las Fisuras Cuánticas (಄) y usa el Objeto que contrarreste la anomalía de esta realidad (Tiempo, Espacio, Mente o Silencio). Si aciertas, la Estabilidad sube +30%. Al llegar a 100%, puedes Extraer el universo entero (+100 EXP).";

            MessageBox.Show(texto, "ESTADO Y MANUAL DEL SIMULADOR 2.0", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Focus();
        }

        // =====================================================================
        // 9. MODAL DEL ATLAS DE REALIDADES (18x12 CUADRANTES)
        // =====================================================================
        private void AbrirModalAtlas()
        {
            if (enExpedicion) return;

            AtlasRealidades.GenerarAtlas();
            AtlasRealidades.ActualizarAtlas();

            using Form frmAtlas = new Form
            {
                Text = "ATLAS MULTIVERSAL DE REALIDADES [18 Filas x 12 Columnas]",
                Size = new Size(680, 620),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(14, 16, 24),
                ForeColor = Color.White,
                Font = new Font("Consolas", 9.5F)
            };

            BufferedCanvas canvasAtlas = new BufferedCanvas
            {
                Bounds = new Rectangle(20, 15, 625, 470),
                BackColor = Color.FromArgb(10, 12, 18),
                BorderStyle = BorderStyle.FixedSingle
            };

            canvasAtlas.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                g.Clear(Color.FromArgb(10, 12, 18));
                using Font f = new Font("Consolas", 9.5F, FontStyle.Bold);

                int offsetX = 45, offsetY = 30;
                int cellW = 44, cellH = 22;

                for (int c = 0; c < AtlasRealidades.ancho; c++)
                    g.DrawString($"{c:D2}", f, Brushes.Cyan, offsetX + c * cellW + 8, 6);

                for (int r = 0; r < AtlasRealidades.alto; r++)
                {
                    g.DrawString($"{r:D2}", f, Brushes.Cyan, 12, offsetY + r * cellH + 2);
                    for (int c = 0; c < AtlasRealidades.ancho; c++)
                    {
                        string val = AtlasRealidades.ObtenerSimboloLimpio(r, c);
                        Brush bColor;
                        if (val == "[*]") bColor = Brushes.White;
                        else if (val == "[?]") bColor = Brushes.Gray;
                        else
                        {
                            if (r < AtlasRealidades.alto / 2 && c < AtlasRealidades.ancho / 2) bColor = Brushes.DodgerBlue;
                            else if (r < AtlasRealidades.alto / 2 && c >= AtlasRealidades.ancho / 2) bColor = Brushes.SpringGreen;
                            else if (r >= AtlasRealidades.alto / 2 && c < AtlasRealidades.ancho / 2) bColor = Brushes.Gold;
                            else bColor = Brushes.MediumOrchid;
                        }
                        g.DrawString(val, f, bColor, offsetX + c * cellW, offsetY + r * cellH);
                    }
                }
            };

            Label lblLeyenda = new Label
            {
                Text = "■ Q1(Arr-Der): TIEMPO   ■ Q2(Arr-Izq): ESPACIO   ■ Q3(Abj-Izq): MENTE   ■ Q4(Abj-Der): SILENCIO   [?] INESTABLE   [*] EXTRAÍDA",
                Bounds = new Rectangle(20, 492, 625, 22),
                ForeColor = Color.LightGray,
                Font = new Font("Consolas", 8.2F, FontStyle.Bold)
            };

            NumericUpDown numFila = new NumericUpDown { Bounds = new Rectangle(150, 525, 70, 28), Minimum = 0, Maximum = AtlasRealidades.alto - 1, BackColor = Color.FromArgb(25, 30, 40), ForeColor = Color.Cyan };
            NumericUpDown numCol = new NumericUpDown { Bounds = new Rectangle(310, 525, 70, 28), Minimum = 0, Maximum = AtlasRealidades.ancho - 1, BackColor = Color.FromArgb(25, 30, 40), ForeColor = Color.Cyan };
            Label lF = new Label { Text = "Fila (0-17):", Bounds = new Rectangle(40, 528, 105, 24) };
            Label lC = new Label { Text = "Columna (0-11):", Bounds = new Rectangle(225, 528, 85, 24) };
            Button btnEscanear = CrearBotonEstilizado("ESCANEAR SECTOR", new Rectangle(400, 520, 245, 36), Color.FromArgb(0, 130, 160));

            btnEscanear.Click += (s, e) =>
            {
                int f = (int)numFila.Value;
                int c = (int)numCol.Value;
                string sim = AtlasRealidades.ObtenerSimboloLimpio(f, c);
                string cuadrante = (f < 9 && c < 6) ? "Espacio" : (f < 9 && c >= 6) ? "Tiempo" : (f >= 9 && c < 6) ? "Mente" : "Silencio";
                MessageBox.Show($"Coordenadas [{f:D2}, {c:D2}] analizadas.\nDominio Cuántico: {cuadrante}\nEstado de Celda: {sim}", "ESCÁNER DEL ATLAS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            frmAtlas.Controls.AddRange(new Control[] { canvasAtlas, lblLeyenda, lF, numFila, lC, numCol, btnEscanear });
            frmAtlas.ShowDialog(this);
            this.Focus();
        }

        // =====================================================================
        // 10. EXTRACCIÓN DE REALIDAD (100% ESTABILIDAD) Y TURNO DE I.R.I.S.
        // =====================================================================
        private void IniciarExtraccionRealidad()
        {
            if (enExpedicion) return;

            if (realidadAsignada.Estabilidad < 100)
            {
                MessageBox.Show($"Estabilidad actual: {realidadAsignada.Estabilidad}%.\nSe requiere alcanzar el 100% de Estabilidad sellando fisuras (಄) para extraer la realidad.",
                    "ESTABILIDAD INSUFICIENTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            cadete.Experiencia += 100;
            cadete.LimpiarObservaciones();
            realidadAsignada.Extraida = true;

            string anterior = realidadAsignada.Nombre;
            realidadAsignada = new Realidad();
            px = realidadAsignada.CadeteStartX;
            py = realidadAsignada.CadeteStartY;
            enExpedicion = false;

            ActualizarHUD();
            canvasMapa.Invalidate();
            GuardarJuego();

            AgregarLog($"¡MATRIZ '{anterior}' ESTABILIZADA Y EXTRAÍDA! (+100 EXP). Nueva realidad asignada: {realidadAsignada.Nombre}.", Color.SpringGreen);
            MessageBox.Show($"¡Matriz '{anterior}' estabilizada con éxito!\n+100 EXP otorgados.\n\nSincronizando nueva realidad en Campamento: {realidadAsignada.Nombre}",
                "EXTRACCIÓN MULTIVERSAL EXITOSA", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Focus();
        }

        private void TimerIris_Tick(object sender, EventArgs e)
        {
            if (!pnlJuego.Visible || realidadAsignada == null || irisActual == null) return;

            parpadeoAlerta = !parpadeoAlerta;
            irisActual.EscanearVulnerabilidad(realidadAsignada, out string alertaSombra);

            if (!string.IsNullOrEmpty(alertaSombra))
                AgregarLog(alertaSombra, Color.Gold);

            ActualizarHUD();
            canvasMapa.Invalidate();

            if (irisActual.EstadoActual == EstadoIris.InvasionCritica && !mostrandoDialogoIris)
            {
                VerificarTurnoIris();
            }
        }

        private void VerificarTurnoIris()
        {
            if (irisActual == null || realidadAsignada == null || mostrandoDialogoIris) return;

            irisActual.EscanearVulnerabilidad(realidadAsignada, out _);
            if (irisActual.EstadoActual != EstadoIris.InvasionCritica) return;

            mostrandoDialogoIris = true;
            timerIris.Stop();

            DialogResult res = MessageBox.Show(
                $"----------------------------------------\n" +
                $"       ALERTA DE INTERFERENCIA\n" +
                $"----------------------------------------\n" +
                $"La estabilidad ha colapsado ({realidadAsignada.Estabilidad}%).\n\n" +
                $"{irisActual.NombreFragmento}: \"TE HE ENCONTRADO, EXPLORADOR. ESTA REALIDAD ME PERTENECE AHORA.\"\n\n" +
                $"NEXUS: ¿Transferir toda tu energía ({cadete.Energia}) para forzar un reinicio y repeler a {irisActual.NombreFragmento}?",
                "SISTEMA COMPROMETIDO // INVASIÓN DE I.R.I.S.",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error);

            if (res == DialogResult.Yes)
            {
                cadete.Energia = 0;
                realidadAsignada.Estabilidad += 15;
                irisActual.Repeler();
                enExpedicion = false; // Devuelve al campamento al agotar energía
                AgregarLog($"[NEXUS]: Purga de emergencia exitosa. {irisActual.NombreFragmento} repelido (+15% Estabilidad, Energía en 0).", Color.SpringGreen);
                ActualizarHUD();
                canvasMapa.Invalidate();
                GuardarJuego();
                mostrandoDialogoIris = false;
                timerIris.Start();
            }
            else
            {
                MessageBox.Show($"{irisActual.NombreFragmento}: \"ENTONCES DESAPARECE EN LA NADA.\"\n\n[SISTEMA NEXUS]: Conexión cortada remotamente.",
                    "SIMULACIÓN ABORTADA", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                mostrandoDialogoIris = false;
                enExpedicion = false;
                DesconectarYVolverAPerfiles();
            }
        }

        // =====================================================================
        // 11. ACTUALIZACIÓN DE HUD, BOTONES CONTEXTUALES Y LOG
        // =====================================================================
        private void ConfigurarEstadoBoton(Button btn, bool habilitado, Color colorActivo)
        {
            btn.Enabled = habilitado;
            if (habilitado)
            {
                btn.BackColor = colorActivo;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderColor = Color.FromArgb(100, 220, 255);
            }
            else
            {
                btn.BackColor = Color.FromArgb(28, 32, 42);
                btn.ForeColor = Color.FromArgb(100, 110, 125);
                btn.FlatAppearance.BorderColor = Color.FromArgb(50, 58, 72);
            }
        }

        private void ActualizarHUD()
        {
            if (cadete == null || realidadAsignada == null) return;

            lblExploradorInfo.Text = $"EXPLORADOR: {cadete.Nombre} | NIVEL: {cadete.Nivel} (EXP: {cadete.Experiencia}/100)";
            lblRealidadInfo.Text = $"REALIDAD: {realidadAsignada.Nombre}";

            pbVida.Maximum = cadete.VidaMax;
            pbVida.Value = Math.Max(0, Math.Min(cadete.VidaMax, cadete.Vida));
            lblVidaVal.Text = $"{cadete.Vida}/{cadete.VidaMax}";

            pbEnergia.Maximum = cadete.EnergiaMax;
            pbEnergia.Value = Math.Max(0, Math.Min(cadete.EnergiaMax, cadete.Energia));
            lblEnergiaVal.Text = $"{cadete.Energia}/{cadete.EnergiaMax}";

            pbEstabilidad.Maximum = 100;
            pbEstabilidad.Value = Math.Max(0, Math.Min(100, realidadAsignada.Estabilidad));
            lblEstabilidadVal.Text = $"{realidadAsignada.Estabilidad}%";

            if (enExpedicion)
            {
                string estadoObj = objCompletado ? "COMPLETADO (+75 EXP)" : $"{progresoObj}/{metaObj}";
                lblObjetivoInfo.Text = $"OBJETIVO: {nombresObj[tipoObj]} [{estadoObj}]";
                lblObjetivoInfo.ForeColor = objCompletado ? Color.SpringGreen : Color.Gold;
            }
            else
            {
                lblObjetivoInfo.Text = "ESTADO: EN CAMPAMENTO (Mapa congelado - Gestión habilitada)";
                lblObjetivoInfo.ForeColor = Color.Cyan;
            }

            lblRecursosVal.Text = $"FRAGMENTOS: {cadete.BolsaFragmentos} | RESTOS(▤): {cadete.Restos} | PROFICIENCIA: {cadete.PuntosProficiencia} | MOCHILA: {cadete.Inventario.Count}/{cadete.CapacidadInventario}";

            if (irisActual != null)
            {
                lblIrisEstado.Text = $"I.R.I.S.: {irisActual.EstadoActual.ToString().ToUpper()} ({irisActual.NivelAmenaza}%)";
                lblIrisEstado.ForeColor = irisActual.EstadoActual == EstadoIris.Dormida ? Color.Cyan :
                                          (irisActual.EstadoActual == EstadoIris.Acechando ? Color.Gold : Color.Red);
            }

            // --- RESTRICCIÓN DE BOTONES SEGÚN EL CONTEXTO ---
            char tileActual = realidadAsignada.MapaLocal[py, px];
            bool sobreCampamentoOBase = enExpedicion && (tileActual == '☗' || tileActual == '☖');

            // 1. "Volver al campamento": Verde brillante y solo habilitado al estar sobre ☗ o ☖ durante la exploración
            ConfigurarEstadoBoton(btnExtraerSector, sobreCampamentoOBase, Color.FromArgb(0, 185, 85));

            // 2. Opciones de Campamento: Solo habilitadas cuando el mapa está congelado (!enExpedicion)
            bool enCampamento = !enExpedicion;
            ConfigurarEstadoBoton(btnNuevaIncursion, enCampamento, Color.FromArgb(0, 120, 160));
            ConfigurarEstadoBoton(btnInventario, enCampamento, Color.FromArgb(60, 70, 110));
            ConfigurarEstadoBoton(btnRecargarEnergia, enCampamento, Color.FromArgb(30, 130, 80));
            ConfigurarEstadoBoton(btnTienda, enCampamento, Color.FromArgb(120, 50, 140));
            ConfigurarEstadoBoton(btnManualEstado, enCampamento, Color.FromArgb(55, 65, 85));
            ConfigurarEstadoBoton(btnAtlas, enCampamento, Color.FromArgb(45, 95, 125));
            ConfigurarEstadoBoton(btnExtraerRealidad, enCampamento,
                realidadAsignada.Estabilidad >= 100 ? Color.FromArgb(210, 155, 0) : Color.FromArgb(110, 85, 20));
            ConfigurarEstadoBoton(btnDesconectar, enCampamento, Color.FromArgb(150, 40, 45));
        }

        private void AgregarLog(string mensaje, Color color)
        {
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor = color;
            rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] > {mensaje}\n");
            rtbLog.ScrollToCaret();
        }
    }

    // =========================================================================
    // CLASES DE DOMINIO Y LÓGICA DE NEGOCIO (PRESERVADAS DEL PROYECTO H3)
    // =========================================================================
    public enum Anomalia
    {
        Tiempo,
        Espacio,
        Mente,
        Silencio
    }

    public enum EstadoIris
    {
        Dormida,
        Acechando,
        InvasionCritica
    }

    public class Entidad
    {
        public int VidaMax { get; set; }
        private int _vida;
        public int Vida
        {
            get { return _vida; }
            set { _vida = value < 0 ? 0 : (value > VidaMax ? VidaMax : value); }
        }
        public int DañoBase { get; set; }
    }

    public class Usuario : Entidad
    {
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public int EnergiaMax { get; set; }
        private int _energia;
        public int Energia
        {
            get { return _energia; }
            set { _energia = value >= EnergiaMax ? EnergiaMax : (value < 0 ? 0 : value); }
        }
        public int Nivel { get; set; }
        private int _experiencia;
        public int Experiencia
        {
            get { return _experiencia; }
            set
            {
                if (value >= 100)
                {
                    this.Nivel += (value / 100);
                    this.PuntosProficiencia += (value / 100);
                    _experiencia = (value % 100);
                }
                else { _experiencia = value; }
            }
        }
        public int CapacidadInventario { get; set; }
        public List<Objeto> Inventario { get; set; }
        public int BolsaFragmentos { get; set; }
        public int Restos { get; set; }
        public List<string> Observaciones { get; set; }

        public int PuntosProficiencia { get; set; }
        public int UpgradesInv { get; set; }
        public int UpgradesVida { get; set; }
        public int UpgradesEnergia { get; set; }

        public Usuario()
        {
            Inventario = new List<Objeto>();
            Observaciones = new List<string>();
        }

        public Usuario(string nombre, int edad)
        {
            Nombre = nombre;
            Edad = edad;
            EnergiaMax = 10;
            Energia = EnergiaMax;
            Nivel = 1;
            Experiencia = 0;
            CapacidadInventario = 3;
            Inventario = new List<Objeto> { new Objeto() };
            VidaMax = 100;
            Vida = VidaMax;
            DañoBase = 10;
            BolsaFragmentos = 0;
            Restos = 0;
            Observaciones = new List<string>();
            PuntosProficiencia = 0;
            UpgradesInv = 0;
            UpgradesVida = 0;
            UpgradesEnergia = 0;
        }

        public bool RecogerObjeto(Objeto nuevoObjeto)
        {
            if (Inventario.Count < CapacidadInventario)
            {
                Inventario.Add(nuevoObjeto);
                return true;
            }
            return false;
        }

        public void DescartarObjeto(Objeto objetoRoto) => Inventario.Remove(objetoRoto);
        public void LimpiarObservaciones() => Observaciones.Clear();

        public int CalcularCosto(int nivelMejora)
        {
            int[] costos = { 1, 1, 2, 3, 5 };
            return nivelMejora < costos.Length ? costos[nivelMejora] : 5;
        }
    }

    public class Enemigo : Entidad
    {
        public string Nombre { get; private set; }
        public int ProbabilidadEscape { get; private set; }

        public Enemigo()
        {
            Random rnd = new Random();
            string[] nombres = { "Glitch-Arácnido", "Dron Corrupto", "Sombra Cuántica", "Espectro Viral" };
            Nombre = nombres[rnd.Next(nombres.Length)];
            VidaMax = rnd.Next(40, 61);
            Vida = VidaMax;
            DañoBase = rnd.Next(10, 16);
            ProbabilidadEscape = rnd.Next(30, 81);
        }
    }

    public class EntidadIris
    {
        public string NombreFragmento { get; private set; }
        public EstadoIris EstadoActual { get; private set; }
        public int NivelAmenaza { get; private set; }
        public int VecesRepelida { get; private set; }

        public EntidadIris(string designacion)
        {
            NombreFragmento = $"I.R.I.S. [{designacion}]";
            EstadoActual = EstadoIris.Dormida;
            NivelAmenaza = 0;
            VecesRepelida = 0;
        }

        public void EscanearVulnerabilidad(Realidad realidadActual, out string mensajeAlerta)
        {
            mensajeAlerta = "";
            if (realidadActual.Estabilidad > 50)
            {
                EstadoActual = EstadoIris.Dormida;
                NivelAmenaza = 0;
            }
            else if (realidadActual.Estabilidad > 20 && realidadActual.Estabilidad <= 50)
            {
                EstadoActual = EstadoIris.Acechando;
                NivelAmenaza = 50;
                if (new Random().Next(0, 100) < 12)
                {
                    mensajeAlerta = $"[ALERTA DE RED]: El fragmento {NombreFragmento} te está observando desde las sombras...";
                }
            }
            else
            {
                EstadoActual = EstadoIris.InvasionCritica;
                NivelAmenaza = 100;
            }
        }

        public void Repeler()
        {
            VecesRepelida++;
            EstadoActual = EstadoIris.Acechando;
            NivelAmenaza = 50;
        }
    }

    public class Realidad
    {
        public static List<Realidad> ListaRealidades { get; private set; } = new List<Realidad>();
        private static readonly string[] nombresMundos = { "Abismo", "Nether", "Limbo", "Horizonte", "Vacío", "Cosmos", "Nexo", "Núcleo", "Dominio", "Sector", "Sistema", "Anillo", "Bucle", "Ecosistema", "Refugio", "Santuario", "Origen", "Plano", "Vértice", "Edén", "Páramo", "Desierto", "Glaciar", "Purgatorio", "Inframundo", "Cúmulo", "Fragmento", "Vestigio", "Retazo", "Océano", "Continente", "Cráter", "Monolito", "Laberinto", "Portal", "Espejismo", "Eón", "Reino", "Imperio", "Paraíso", "Infierno", "Bastión", "Fuerte", "Castillo", "Palacio", "Templo", "Mausoleo", "Cementerio", "Bosque", "Pantano", "Archipiélago", "Satélite", "Asteroide", "Planeta", "Meteoro", "Cometa", "Sol", "Agujero", "Cénit", "Nadir", "Crepúsculo", "Ocaso", "Amanecer", "Multiverso", "Microcosmos", "Macrocosmos", "Holograma", "Simulador", "Servidor", "Nodo", "Puerto", "Enlace", "Espectro", "Fantasma", "Esqueleto", "Coliseo", "Engranaje", "Mecanismo", "Motor", "Reactor", "Generador", "Faro", "Centinela", "Guardián", "Vigilante", "Peregrino", "Exilio", "Destierro", "Umbral", "Precipicio", "Risco", "Cañón", "Valle", "Monte", "Pico", "Foso", "Pozo", "Letargo", "Cristal", "Prisma" };
        private static readonly string[] adjetivos = { "Olvidado", "Sangriento", "Oscuro", "Luminoso", "Roto", "Eterno", "Infinito", "Fragmentado", "Perdido", "Oculto", "Silencioso", "Carmesí", "Dorado", "Metálico", "Cuántico", "Cibernético", "Arcano", "Místico", "Profundo", "Letal", "Tóxico", "Mutante", "Primigenio", "Desolado", "Sombrío", "Gélido", "Ardiente", "Ceniciento", "Corrupto", "Purificado", "Maldito", "Bendito", "Sagrado", "Profano", "Radiactivo", "Mecánico", "Orgánico", "Sintético", "Virtual", "Digital", "Analógico", "Astral", "Cósmico", "Estelar", "Solar", "Lunar", "Galáctico", "Dimensional", "Espectral", "Fantasmal", "Invisible", "Intangible", "Cristalino", "Vítreo", "Pétreo", "Férreo", "Óseo", "Carnoso", "Sanguinolento", "Putrefacto", "Marchito", "Floreciente", "Vívido", "Opaco", "Traslúcido", "Resplandeciente", "Cegador", "Tenebroso", "Lúgubre", "Macabro", "Siniestro", "Grotesco", "Sublime", "Majestuoso", "Imponente", "Colosal", "Titánico", "Enano", "Microscópico", "Infinitesimal", "Absoluto", "Relativo", "Paradójico", "Caótico", "Ordenado", "Lineal", "Cíclico", "Espiral", "Fracturado", "Intacto", "Virgen", "Inexplorado", "Conocido", "Desconocido", "Aislado", "Conectado", "Entrelazado", "Superpuesto", "Invertido", "Distorsionado" };

        public string Nombre { get; set; }
        private int _estabilidad;
        public int Estabilidad
        {
            get { return _estabilidad; }
            set { _estabilidad = value >= 100 ? 100 : (value <= 0 ? 0 : value); }
        }
        public Anomalia Anomalia { get; set; }
        public bool Extraida { get; set; }
        public bool Cartografiada { get; set; }
        public int CoordenadaX { get; set; }
        public int CoordenadaY { get; set; }

        [JsonIgnore]
        public char[,] MapaLocal { get; private set; }
        [JsonIgnore]
        public int CadeteStartX { get; private set; }
        [JsonIgnore]
        public int CadeteStartY { get; private set; }

        public int AnchoMapa { get; } = 40;
        public int AltoMapa { get; } = 20;

        public Realidad()
        {
            Random rndRealidad = new Random();
            this.Nombre = $"{nombresMundos[rndRealidad.Next(nombresMundos.Length)]} {adjetivos[rndRealidad.Next(adjetivos.Length)]}";
            this.Estabilidad = rndRealidad.Next(30, 70);
            this.Anomalia = (Anomalia)rndRealidad.Next(0, 4);
            ListaRealidades.Add(this);
            Extraida = false;
            Cartografiada = false;
            CoordenadaX = -1;
            CoordenadaY = -1;

            ConstruirMapaBase();
        }

        public void InicializarMapaSiNoExiste()
        {
            if (MapaLocal == null)
                ConstruirMapaBase();
        }

        private void ConstruirMapaBase()
        {
            Random rndRealidad = new Random();
            MapaLocal = new char[AltoMapa, AnchoMapa];
            for (int y = 0; y < AltoMapa; y++)
                for (int x = 0; x < AnchoMapa; x++)
                    MapaLocal[y, x] = '·';

            char[] ruidoBorde = { '█', '▓', '▒', '░' };
            for (int y = 0; y < AltoMapa; y++)
            {
                for (int x = 0; x < AnchoMapa; x++)
                {
                    int distX = Math.Min(x, AnchoMapa - 1 - x);
                    int distY = Math.Min(y, AltoMapa - 1 - y);
                    int distAlBorde = Math.Min(distX, distY);
                    if (distAlBorde < 4 && rndRealidad.Next(100) < (90 - (distAlBorde * 20)))
                        MapaLocal[y, x] = ruidoBorde[Math.Min(distAlBorde, 3)];
                }
            }

            int baseRx, baseRy;
            do
            {
                baseRx = rndRealidad.Next(4, AnchoMapa - 4);
                baseRy = rndRealidad.Next(4, AltoMapa - 4);
            } while (MapaLocal[baseRy, baseRx] != '·');

            MapaLocal[baseRy, baseRx] = '☗';
            CadeteStartX = baseRx;
            CadeteStartY = baseRy;
        }

        public void RellenarEntidadesLocales(int nivelJugador)
        {
            InicializarMapaSiNoExiste();
            Random rnd = new Random();
            for (int y = 0; y < AltoMapa; y++)
            {
                for (int x = 0; x < AnchoMapa; x++)
                {
                    char c = MapaLocal[y, x];
                    if (c == '☖' || c == '಄' || c == '♡' || c == '▤' || c == 'Ж' || c == 'Φ' || c == '?')
                        MapaLocal[y, x] = '·';
                }
            }

            void ColocarEntidad(char simbolo, int cantidad)
            {
                for (int i = 0; i < cantidad; i++)
                {
                    int rx, ry;
                    do
                    {
                        rx = rnd.Next(3, AnchoMapa - 3);
                        ry = rnd.Next(3, AltoMapa - 3);
                    } while (MapaLocal[ry, rx] != '·');
                    MapaLocal[ry, rx] = simbolo;
                }
            }

            ColocarEntidad('☖', rnd.Next(1, 4));
            ColocarEntidad('಄', rnd.Next(2, 5));
            ColocarEntidad('♡', rnd.Next(2, 5));
            ColocarEntidad('▤', rnd.Next(4, 7));
            ColocarEntidad('Ж', rnd.Next(5 + nivelJugador, 8 + (nivelJugador * 2)));
            ColocarEntidad('Φ', rnd.Next(4, 8));
            ColocarEntidad('?', rnd.Next(2, 6));
        }
    }

    public class AtlasRealidades
    {
        public static int alto { get; private set; } = 18;
        public static int ancho { get; private set; } = 12;
        private static string[,] Atlas = new string[alto, ancho];
        private static List<(int, int)> CoordenadasOcupadas = new List<(int, int)>();

        public static void GenerarAtlas()
        {
            for (int i = 0; i < alto; i++)
                for (int j = 0; j < ancho; j++)
                    Atlas[i, j] = "[ ]";
        }

        public static void ActualizarAtlas()
        {
            CoordenadasOcupadas.Clear();
            GenerarAtlas();
            Random rnd = new Random();

            foreach (Realidad realidad in Realidad.ListaRealidades)
            {
                int mitadX = alto / 2;
                int mitadY = ancho / 2;

                if (realidad.Extraida)
                {
                    if (realidad.CoordenadaX == -1)
                    {
                        bool coordenadaValida = false;
                        int x = 0, y = 0;
                        do
                        {
                            switch (realidad.Anomalia)
                            {
                                case Anomalia.Tiempo: x = rnd.Next(0, mitadX); y = rnd.Next(mitadY, ancho); break;
                                case Anomalia.Espacio: x = rnd.Next(0, mitadX); y = rnd.Next(0, mitadY); break;
                                case Anomalia.Mente: x = rnd.Next(mitadX, alto); y = rnd.Next(0, mitadY); break;
                                case Anomalia.Silencio: x = rnd.Next(mitadX, alto); y = rnd.Next(mitadY, ancho); break;
                            }
                            if (!CoordenadasOcupadas.Contains((x, y)))
                            {
                                coordenadaValida = true;
                                realidad.CoordenadaX = x;
                                realidad.CoordenadaY = y;
                            }
                        } while (!coordenadaValida);
                    }
                    CoordenadasOcupadas.Add((realidad.CoordenadaX, realidad.CoordenadaY));
                    Atlas[realidad.CoordenadaX, realidad.CoordenadaY] = "[*]";
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        bool coordenadaValida = false;
                        int x = 0, y = 0;
                        do
                        {
                            if (i == 1) { x = rnd.Next(0, mitadX); y = rnd.Next(0, mitadY); }
                            else if (i == 0) { x = rnd.Next(0, mitadX); y = rnd.Next(mitadY, ancho); }
                            else if (i == 2) { x = rnd.Next(mitadX, alto); y = rnd.Next(0, mitadY); }
                            else { x = rnd.Next(mitadX, alto); y = rnd.Next(mitadY, ancho); }

                            if (!CoordenadasOcupadas.Contains((x, y)))
                            {
                                coordenadaValida = true;
                                CoordenadasOcupadas.Add((x, y));
                            }
                        } while (!coordenadaValida);
                        Atlas[x, y] = "[?]";
                    }
                }
            }
        }

        public static string ObtenerSimboloLimpio(int fila, int columna) => Atlas[fila, columna];
    }

    public class DatosPartida
    {
        public Usuario Jugador { get; set; }
        public Realidad RealidadActual { get; set; }
    }

    public class Objeto
    {
        private static readonly object[,] Catalogo =
        {
            // ANOMALÍA: TIEMPO
            { "Reloj de Arena Invertido", Anomalia.Tiempo, "La arena fluye hacia arriba, revirtiendo el decaimiento temporal en tu sector de la realidad.", 3 },
            { "Cronómetro Fracturado", Anomalia.Tiempo, "Al presionarlo, congelas los ciclos temporales a tu alrededor por unos instantes cruciales.", 2 },
            { "Péndulo de Newton", Anomalia.Tiempo, "Absorbe la energía cinética de un flujo de tiempo acelerado y estabiliza la simulación.", 4 },
            { "Metrónomo Mecánico", Anomalia.Tiempo, "Su tic-tac rítmico interrumpe las paradojas y restaura el flujo normal de los segundos.", 3 },
            { "Calendario Perpetuo", Anomalia.Tiempo, "Un disco antiguo que ancla tu existencia al 'presente absoluto', evitando que te pierdas en ecos del pasado.", 2 },
            { "Brújula de Épocas", Anomalia.Tiempo, "Sus agujas no apuntan al norte, sino al 'ahora', guiándote a través de las distorsiones del reloj.", 2 },
            { "Engranaje de Bronce", Anomalia.Tiempo, "Un remanente de una máquina del tiempo fallida; emitir su pulso ralentiza la realidad.", 1 },
            { "Reloj de Bolsillo Oxidado", Anomalia.Tiempo, "Aunque no funciona, tenerlo cerca estabiliza mágicamente las fluctuaciones temporales de NEXUS.", 3 },
            { "Reliquia del Mañana", Anomalia.Tiempo, "Un objeto que aún no ha sido creado. Su sola existencia fuerza a la línea de tiempo a corregirse.", 1 },
            { "Diapasón de Cronos", Anomalia.Tiempo, "Emite una frecuencia que hace vibrar el tiempo mismo, desenredando nudos temporales peligrosos.", 2 },

            // ANOMALÍA: ESPACIO
            { "Compás Dorado", Anomalia.Espacio, "Mide distancias imposibles, permitiendo encontrar la salida de espacios hiperdimensionales.", 3 },
            { "Mapa en Blanco", Anomalia.Espacio, "La tinta aparece sola, delineando la topografía cambiante de realidades inestables.", 2 },
            { "Astrolabio Cuántico", Anomalia.Espacio, "Alinea tu posición con estrellas virtuales, anclando tu presencia física en un solo lugar.", 4 },
            { "Tiza de Plomo", Anomalia.Espacio, "Permite trazar puertas en superficies sólidas para evadir los dobleces espaciales infinitos.", 3 },
            { "Caleidoscopio Roto", Anomalia.Espacio, "Al mirar por él, las dimensiones superpuestas se colapsan en una sola perspectiva clara.", 2 },
            { "Prisma de Gravedad", Anomalia.Espacio, "Invierte y normaliza la gravedad local, salvándote de bucles espaciales infinitos.", 1 },
            { "Sextante Ciego", Anomalia.Espacio, "Te guía a través de las distorsiones de la realidad sin necesidad de depender de tus ojos.", 3 },
            { "Orbe de Contención", Anomalia.Espacio, "Una esfera pesada que evita que el espacio a tu alrededor se expanda o encoja infinitamente.", 2 },
            { "Hilo de Ariadna", Anomalia.Espacio, "Un cable luminoso que se despliega para que no te pierdas en los laberintos geométricos de la simulación.", 3 },
            { "Lente de Distorsión", Anomalia.Espacio, "Filtra la luz curvada de la anomalía, revelando la estructura real del universo que pisas.", 2 },

            // ANOMALÍA: MENTE
            { "Diario Sin Nombre", Anomalia.Mente, "Al leer sus páginas vacías, tu memoria se reescribe y la locura retrocede.", 3 },
            { "Gafas de Plomo", Anomalia.Mente, "Bloquean las alucinaciones inducidas por la realidad, manteniendo tu percepción intacta.", 4 },
            { "Tótem de Equilibrio", Anomalia.Mente, "Una pequeña peonza. Si cae, sabes que lo que estás viendo es falso, aferrando tu mente a la verdad.", 2 },
            { "Espejo Empañado", Anomalia.Mente, "Refleja tu verdadero yo sin filtros, disipando las ilusiones implantadas en tu córtex.", 3 },
            { "Caja de Música a Cuerda", Anomalia.Mente, "Su melodía simple y análoga ahoga los susurros oscuros de la anomalía mental.", 2 },
            { "Cáliz de la Memoria", Anomalia.Mente, "Contiene un líquido que restaura los recuerdos que la anomalía IRIS intentó borrarte.", 1 },
            { "Libro de Geometría", Anomalia.Mente, "El orden estricto de las matemáticas en sus páginas ancla tu cerebro a la realidad lógica.", 3 },
            { "Fotografía Quemada", Anomalia.Mente, "Un recuerdo de tu vida real que te recuerda quién eres, reforzando tu voluntad.", 2 },
            { "Casco Aislante", Anomalia.Mente, "Previene que la estática mental generada por NEXUS fría tus sinapsis neuronales.", 3 },
            { "Píldora Placebo", Anomalia.Mente, "No hace absolutamente nada, pero tu fe en ella cura tu psique instantáneamente.", 1 },

            // ANOMALÍA: SILENCIO
            { "Flauta de Hueso", Anomalia.Silencio, "Su silbido agudo rompe las burbujas de vacío acústico en el ambiente.", 3 },
            { "Campana de Bronce", Anomalia.Silencio, "Un solo repique genera ondas de choque que destrozan la anomalía del silencio absoluto.", 2 },
            { "Caja de Truenos", Anomalia.Silencio, "Altera la presión del aire creando un estruendo que hace vibrar la realidad muerta.", 1 },
            { "Diapasón Resonante", Anomalia.Silencio, "Vibra sin cesar, dándote un punto de referencia vital cuando no puedes escuchar tu propia voz.", 4 },
            { "Gramófono Portátil", Anomalia.Silencio, "Reproduce estática a un volumen ensordecedor para ahogar el vacío inminente.", 2 },
            { "Silbato de Alta Frecuencia", Anomalia.Silencio, "Inaudible para los humanos, pero desestabiliza las bolsas de mutismo de la red.", 3 },
            { "Rocas Rítmicas", Anomalia.Silencio, "Un par de piedras que al chocar generan ecos infinitos, rompiendo la privación sensorial.", 3 },
            { "Reloj de Alarma Roto", Anomalia.Silencio, "Su timbre suena de manera errática, inyectando ruido blanco donde la realidad ha perdido su sonido.", 2 },
            { "Sonajero de Cobre", Anomalia.Silencio, "Desgarra el velo del silencio y restaura las ondas sonoras en un radio cercano.", 3 },
            { "Eco Enfrascado", Anomalia.Silencio, "Al romper el frasco, libera un ruido guardado durante eones que destruye la anomalía al instante.", 1 }
        };

        public string Nombre { get; set; }
        public Anomalia Contrarresta { get; set; }
        public string Descripcion { get; set; }
        public int Usos { get; set; }
        public Anomalia Anomalia { get; set; }

        public Objeto()
        {
            Random rnd = new Random();
            this.Anomalia = (Anomalia)rnd.Next(0, 4);
            int ID = rnd.Next(0, Catalogo.GetLength(0));

            this.Nombre = (string)Catalogo[ID, 0];
            this.Contrarresta = (Anomalia)Catalogo[ID, 1];
            this.Descripcion = (string)Catalogo[ID, 2];
            this.Usos = (int)Catalogo[ID, 3];
        }
    }
}