using System;
using System.Drawing;
using System.Windows.Forms;

namespace Interfaz
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // Paneles principales de navegación
        private Panel pnlPerfiles;
        private Panel pnlJuego;

        // Controles de Pantalla 1: Gestor de Perfiles
        private Label lblTituloNexus;
        private Label lblSubtituloNexus;
        private DataGridView dgvPerfiles;
        private TextBox txtNuevoNombre;
        private NumericUpDown numNuevaEdad;
        private Button btnCargarPerfil;
        private Button btnCrearPerfil;
        private Button btnPurgarPerfil;

        // Controles de Pantalla 2: HUD Superior
        private Panel pnlHud;
        private Label lblExploradorInfo;
        private Label lblRealidadInfo;
        private Label lblObjetivoInfo;
        private Label lblIrisEstado;
        private ProgressBar pbVida;
        private ProgressBar pbEnergia;
        private ProgressBar pbEstabilidad;
        private Label lblVidaVal;
        private Label lblEnergiaVal;
        private Label lblEstabilidadVal;
        private Label lblRecursosVal;

        // Lienzo Central del Mapa 40x20 (GDI+ DoubleBuffered)
        private BufferedCanvas canvasMapa;
        private Label lblLeyendaMapa;

        // Panel Lateral Derecho: Controles de Hub y Log
        private Panel pnlLateral;
        private Button btnNuevaIncursion;
        private Button btnExtraerSector;
        private Button btnInventario;
        private Button btnRecargarEnergia;
        private Button btnTienda;
        private Button btnAtlas;
        private Button btnManualEstado;
        private Button btnExtraerRealidad;
        private Button btnDesconectar;
        private RichTextBox rtbLog;

        // Timers
        private System.Windows.Forms.Timer timerIris;

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
            this.components = new System.ComponentModel.Container();
            this.KeyPreview = true;
            this.Text = "NEXUS TRAINING SYSTEM - Simulador Multiversal";
            this.ClientSize = new Size(1220, 760);
            this.MinimumSize = new Size(1220, 760);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(14, 16, 22);
            this.ForeColor = Color.FromArgb(220, 230, 240);
            this.Font = new Font("Consolas", 10F, FontStyle.Regular);

            // =========================================================
            // 1. PANEL GESTOR DE PERFILES (PANTALLA INICIAL)
            // =========================================================
            this.pnlPerfiles = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(14, 16, 22)
            };

            this.lblTituloNexus = new Label
            {
                Text = "╔═══ N E X U S   T R A I N I N G   S Y S T E M ═══╗",
                Font = new Font("Consolas", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 255, 255),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(100, 35, 1020, 45)
            };

            this.lblSubtituloNexus = new Label
            {
                Text = "SISTEMA DE ANCLAJE MULTIVERSAL // GESTIÓN BIOMÉTRICA DE EXPLORADORES",
                Font = new Font("Consolas", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 100, 255),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(100, 85, 1020, 30)
            };

            this.dgvPerfiles = new DataGridView
            {
                Bounds = new Rectangle(160, 135, 900, 340),
                BackgroundColor = Color.FromArgb(22, 26, 35),
                ForeColor = Color.FromArgb(0, 255, 140),
                GridColor = Color.FromArgb(0, 140, 160),
                BorderStyle = BorderStyle.FixedSingle,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false
            };
            this.dgvPerfiles.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 38, 52);
            this.dgvPerfiles.ColumnHeadersDefaultCellStyle.ForeColor = Color.Cyan;
            this.dgvPerfiles.ColumnHeadersDefaultCellStyle.Font = new Font("Consolas", 10.5F, FontStyle.Bold);
            this.dgvPerfiles.DefaultCellStyle.BackColor = Color.FromArgb(22, 26, 35);
            this.dgvPerfiles.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 100, 120);
            this.dgvPerfiles.DefaultCellStyle.SelectionForeColor = Color.White;

            // Grupo de creación de nuevo explorador
            GroupBox grpNuevo = new GroupBox
            {
                Text = " [ REGISTRAR NUEVO EXPLORADOR ] ",
                ForeColor = Color.Cyan,
                Bounds = new Rectangle(160, 495, 540, 110)
            };

            Label lblNom = new Label { Text = "Designación (Nombre):", Bounds = new Rectangle(20, 32, 180, 25), ForeColor = Color.White };
            this.txtNuevoNombre = new TextBox
            {
                Bounds = new Rectangle(205, 29, 175, 28),
                BackColor = Color.FromArgb(30, 35, 45),
                ForeColor = Color.SpringGreen,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblEd = new Label { Text = "Edad (Mín. 18):", Bounds = new Rectangle(20, 68, 180, 25), ForeColor = Color.White };
            this.numNuevaEdad = new NumericUpDown
            {
                Bounds = new Rectangle(205, 65, 90, 28),
                Minimum = 18,
                Maximum = 120,
                Value = 20,
                BackColor = Color.FromArgb(30, 35, 45),
                ForeColor = Color.SpringGreen
            };

            this.btnCrearPerfil = CrearBotonEstilizado("REGISTRAR Y ENTRAR", new Rectangle(395, 28, 130, 65), Color.FromArgb(0, 180, 100));
            grpNuevo.Controls.AddRange(new Control[] { lblNom, this.txtNuevoNombre, lblEd, this.numNuevaEdad, this.btnCrearPerfil });

            this.btnCargarPerfil = CrearBotonEstilizado("► CARGAR PERFIL SELECCIONADO", new Rectangle(720, 503, 340, 48), Color.FromArgb(0, 150, 190));
            this.btnPurgarPerfil = CrearBotonEstilizado("✖ PURGAR REGISTRO (BORRAR)", new Rectangle(720, 557, 340, 48), Color.FromArgb(180, 40, 50));

            this.pnlPerfiles.Controls.AddRange(new Control[]
            {
                this.lblTituloNexus, this.lblSubtituloNexus, this.dgvPerfiles,
                grpNuevo, this.btnCargarPerfil, this.btnPurgarPerfil
            });

            // =========================================================
            // 2. PANEL PRINCIPAL DE JUEGO (HUD + MAPA 40x20 + LATERAL)
            // =========================================================
            this.pnlJuego = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(14, 16, 22),
                Visible = false
            };

            // HUD Superior
            this.pnlHud = new Panel
            {
                Bounds = new Rectangle(15, 12, 1190, 110),
                BackColor = Color.FromArgb(22, 26, 36),
                BorderStyle = BorderStyle.FixedSingle
            };

            this.lblExploradorInfo = new Label
            {
                Text = "EXPLORADOR: --- | NIVEL: 1 (EXP: 0/100)",
                Font = new Font("Consolas", 11F, FontStyle.Bold),
                ForeColor = Color.SpringGreen,
                Bounds = new Rectangle(15, 10, 460, 24)
            };

            this.lblRealidadInfo = new Label
            {
                Text = "REALIDAD: ---",
                Font = new Font("Consolas", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 120, 255),
                Bounds = new Rectangle(480, 10, 380, 24)
            };

            this.lblIrisEstado = new Label
            {
                Text = "I.R.I.S.: DORMIDA",
                Font = new Font("Consolas", 10.5F, FontStyle.Bold),
                ForeColor = Color.Cyan,
                TextAlign = ContentAlignment.MiddleRight,
                Bounds = new Rectangle(865, 10, 305, 24)
            };

            // Barras de progreso (Vida, Energía, Estabilidad)
            Label lVida = new Label { Text = "VIDA:", Bounds = new Rectangle(15, 42, 60, 22), ForeColor = Color.LightCoral };
            this.pbVida = new ProgressBar { Bounds = new Rectangle(75, 42, 190, 20), Maximum = 100, Value = 100 };
            this.lblVidaVal = new Label { Text = "100/100", Bounds = new Rectangle(270, 42, 90, 22), ForeColor = Color.White };

            Label lEne = new Label { Text = "ENERGÍA:", Bounds = new Rectangle(375, 42, 80, 22), ForeColor = Color.Cyan };
            this.pbEnergia = new ProgressBar { Bounds = new Rectangle(460, 42, 190, 20), Maximum = 10, Value = 10 };
            this.lblEnergiaVal = new Label { Text = "10/10", Bounds = new Rectangle(655, 42, 80, 22), ForeColor = Color.White };

            Label lEst = new Label { Text = "ESTABILIDAD:", Bounds = new Rectangle(750, 42, 110, 22), ForeColor = Color.Violet };
            this.pbEstabilidad = new ProgressBar { Bounds = new Rectangle(865, 42, 210, 20), Maximum = 100, Value = 50 };
            this.lblEstabilidadVal = new Label { Text = "50%", Bounds = new Rectangle(1080, 42, 90, 22), ForeColor = Color.White };

            this.lblObjetivoInfo = new Label
            {
                Text = "OBJETIVO DE EXPEDICIÓN: [ En Campamento ]",
                ForeColor = Color.Gold,
                Bounds = new Rectangle(15, 74, 580, 24)
            };

            this.lblRecursosVal = new Label
            {
                Text = "FRAGMENTOS: 0  |  RESTOS (▤): 0  |  PROFICIENCIA: 0 PTS  |  MOCHILA: 1/3",
                ForeColor = Color.Orange,
                TextAlign = ContentAlignment.MiddleRight,
                Bounds = new Rectangle(600, 74, 570, 24)
            };

            this.pnlHud.Controls.AddRange(new Control[]
            {
                this.lblExploradorInfo, this.lblRealidadInfo, this.lblIrisEstado,
                lVida, this.pbVida, this.lblVidaVal,
                lEne, this.pbEnergia, this.lblEnergiaVal,
                lEst, this.pbEstabilidad, this.lblEstabilidadVal,
                this.lblObjetivoInfo, this.lblRecursosVal
            });

            // Lienzo del Mapa 40x20 (840x520 px -> Celdas de 21x26 px)
            this.canvasMapa = new BufferedCanvas
            {
                Bounds = new Rectangle(15, 132, 840, 520),
                BackColor = Color.FromArgb(10, 12, 18),
                BorderStyle = BorderStyle.FixedSingle
            };

            this.lblLeyendaMapa = new Label
            {
                Text = "[@] Cadete  [☗] Base  [☖] Campamento  [಄] Anomalía  [▤] Restos  [Ж] Enemigo  [Φ] Energía  [♡] Salud  [?] Evento\n" +
                       "CONTROLES: [W,A,S,D / Flechas / Clic Adyacente] Moverse (3 pasos = -1 E)  |  [ESPACIO] Volver al campamento (En ☗ o ☖)",
                ForeColor = Color.FromArgb(170, 185, 200),
                Font = new Font("Consolas", 9.2F),
                Bounds = new Rectangle(15, 660, 840, 85)
            };

            // Panel Lateral de Comandos y Log (SIN NUMERACIÓN)
            this.pnlLateral = new Panel
            {
                Bounds = new Rectangle(865, 132, 340, 610),
                BackColor = Color.FromArgb(20, 24, 34),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblMenuCmd = new Label
            {
                Text = "══ PANEL TÁCTICO NEXUS ══",
                ForeColor = Color.Cyan,
                Font = new Font("Consolas", 11F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(10, 10, 320, 25)
            };

            this.btnExtraerSector = CrearBotonEstilizado("Volver al campamento [ESPACIO]", new Rectangle(14, 42, 312, 36), Color.FromArgb(0, 185, 85));
            this.btnNuevaIncursion = CrearBotonEstilizado("Explorar realidad", new Rectangle(14, 82, 312, 36), Color.FromArgb(0, 120, 160));
            this.btnInventario = CrearBotonEstilizado("Inventario y síntesis", new Rectangle(14, 122, 312, 36), Color.FromArgb(60, 70, 110));
            this.btnRecargarEnergia = CrearBotonEstilizado("Recuperar energía [+5 Recarga]", new Rectangle(14, 162, 312, 36), Color.FromArgb(30, 130, 80));
            this.btnTienda = CrearBotonEstilizado("Tienda de proficiencia [Mejoras]", new Rectangle(14, 202, 312, 36), Color.FromArgb(120, 50, 140));
            this.btnManualEstado = CrearBotonEstilizado("Estado y manual", new Rectangle(14, 242, 312, 36), Color.FromArgb(55, 65, 85));
            this.btnAtlas = CrearBotonEstilizado("Atlas de realidades", new Rectangle(14, 282, 312, 36), Color.FromArgb(45, 95, 125));
            this.btnExtraerRealidad = CrearBotonEstilizado("¡Iniciar extracción total! (100%)", new Rectangle(14, 322, 312, 38), Color.FromArgb(180, 140, 0));
            this.btnDesconectar = CrearBotonEstilizado("Guardar y cambiar perfil", new Rectangle(14, 364, 312, 34), Color.FromArgb(150, 40, 45));

            Label lblLogTitle = new Label
            {
                Text = "── REGISTRO DE TELEMETRÍA (LOG) ──",
                ForeColor = Color.Orange,
                Font = new Font("Consolas", 9.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(10, 404, 320, 20)
            };

            this.rtbLog = new RichTextBox
            {
                Bounds = new Rectangle(14, 426, 312, 170),
                BackColor = Color.FromArgb(12, 14, 20),
                ForeColor = Color.SpringGreen,
                Font = new Font("Consolas", 9F),
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            this.pnlLateral.Controls.AddRange(new Control[]
            {
                lblMenuCmd, this.btnExtraerSector, this.btnNuevaIncursion,
                this.btnInventario, this.btnRecargarEnergia, this.btnTienda,
                this.btnManualEstado, this.btnAtlas, this.btnExtraerRealidad,
                this.btnDesconectar, lblLogTitle, this.rtbLog
            });

            this.pnlJuego.Controls.AddRange(new Control[]
            {
                this.pnlHud, this.canvasMapa, this.lblLeyendaMapa, this.pnlLateral
            });

            this.Controls.Add(this.pnlJuego);
            this.Controls.Add(this.pnlPerfiles);

            // Timer de I.R.I.S.
            this.timerIris = new System.Windows.Forms.Timer(this.components)
            {
                Interval = 1200
            };
        }

        private Button CrearBotonEstilizado(string texto, Rectangle bounds, Color colorBase)
        {
            Button btn = new Button
            {
                Text = texto,
                Bounds = bounds,
                BackColor = colorBase,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TabStop = false
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(100, 220, 255);
            btn.FlatAppearance.BorderSize = 1;
            return btn;
        }
    }

    /// <summary>
    /// Control de lienzo con DoubleBuffering activado para evitar parpadeos (Flickering) en GDI+.
    /// </summary>
    public class BufferedCanvas : Panel
    {
        public BufferedCanvas()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
        }
    }
}