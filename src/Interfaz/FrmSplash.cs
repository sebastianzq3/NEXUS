using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Interfaz
{
    public class FrmSplash : Form
    {
        private readonly System.Windows.Forms.Timer timerAnim;
        private Image logoImg;
        private int tickActual = 0;
        private const int TotalTicks = 78; // ~2.5 segundos a 30 FPS (33 ms)
        private float fase = 0f;
        private float scanY = 0f;

        private readonly string[] mensajes =
        {
            "Sincronizando coordenadas cuánticas...",
            "Cargando núcleo I.R.I.S...",
            "Inicializando módulo didáctico C#..."
        };

        public FrmSplash()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(700, 410);
            this.BackColor = Color.FromArgb(10, 12, 18);
            this.DoubleBuffered = true;
            this.Opacity = 0.0;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.KeyPreview = true;

            CargarRecursos();

            this.MouseClick += (s, e) => CerrarSplash();
            this.KeyDown += (s, e) => CerrarSplash();

            timerAnim = new System.Windows.Forms.Timer { Interval = 33 };
            timerAnim.Tick += TimerAnim_Tick;
            timerAnim.Start();
        }

        private void CargarRecursos()
        {
            try
            {
                string rutaLogo = Path.Combine(AppContext.BaseDirectory, "Assets", "nexus_logo.png");
                if (File.Exists(rutaLogo))
                {
                    using FileStream fs = new FileStream(rutaLogo, FileMode.Open, FileAccess.Read);
                    logoImg = Image.FromStream(fs);
                }

                string rutaIco = Path.Combine(AppContext.BaseDirectory, "Assets", "nexus_icon.ico");
                if (File.Exists(rutaIco))
                {
                    this.Icon = new Icon(rutaIco);
                }
            }
            catch
            {
                logoImg = null;
            }
        }

        private void TimerAnim_Tick(object sender, EventArgs e)
        {
            tickActual++;
            fase += 0.16f;
            scanY = (scanY + 6f) % this.ClientSize.Height;

            // Fade-in suave (0.0 -> 1.0)
            if (this.Opacity < 1.0)
            {
                this.Opacity = Math.Min(1.0, this.Opacity + 0.08);
            }

            this.Invalidate();

            if (tickActual >= TotalTicks)
            {
                CerrarSplash();
            }
        }

        private void CerrarSplash()
        {
            timerAnim.Stop();
            this.Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;

            // Fondo con sutil cuadrícula táctica
            using (SolidBrush bg = new SolidBrush(Color.FromArgb(10, 12, 18)))
                g.FillRectangle(bg, 0, 0, w, h);

            using (Pen gridPen = new Pen(Color.FromArgb(18, 0, 220, 255), 1f))
            {
                for (int x = 0; x < w; x += 35) g.DrawLine(gridPen, x, 0, x, h);
                for (int y = 0; y < h; y += 35) g.DrawLine(gridPen, 0, y, w, y);
            }

            // Pulso neón en el borde exterior
            int alphaPulso = 140 + (int)(Math.Sin(fase) * 95);
            alphaPulso = Math.Max(40, Math.Min(255, alphaPulso));
            using (Pen pOuter = new Pen(Color.FromArgb(alphaPulso, 0, 229, 255), 2.5f))
                g.DrawRectangle(pOuter, 2, 2, w - 5, h - 5);
            using (Pen pInner = new Pen(Color.FromArgb(80, 213, 0, 249), 1f))
                g.DrawRectangle(pInner, 8, 8, w - 17, h - 17);

            // Área del logo con borde neón pulsante
            Rectangle logoRect = new Rectangle(95, 30, w - 190, 225);
            if (logoImg != null)
            {
                g.DrawImage(logoImg, logoRect);
            }
            else
            {
                // Fallback dibujado en GDI+ si no se encuentra la imagen
                using Font fFallback = new Font("Consolas", 42f, FontStyle.Bold);
                using StringFormat sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("N E X U S", fFallback, Brushes.Cyan, logoRect, sfCenter);
            }

            using (Pen pLogoGlow = new Pen(Color.FromArgb(alphaPulso, 0, 229, 255), 1.5f))
                g.DrawRectangle(pLogoGlow, logoRect);

            // Línea scanline holográfica
            using (LinearGradientBrush scanBrush = new LinearGradientBrush(
                new RectangleF(0, scanY - 12, w, 24),
                Color.FromArgb(0, 0, 255, 255),
                Color.FromArgb(42, 0, 255, 255),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(scanBrush, 4, scanY - 12, w - 8, 24);
            }
            using (Pen scanLine = new Pen(Color.FromArgb(110, 0, 255, 255), 1.2f))
                g.DrawLine(scanLine, 4, scanY, w - 4, scanY);

            // Barra de carga cuántica
            float progreso = Math.Min(1f, (float)tickActual / TotalTicks);
            int idxMsg = Math.Min(mensajes.Length - 1, (int)(progreso * mensajes.Length));
            string mensajeActual = mensajes[idxMsg];

            Rectangle barBg = new Rectangle(75, 285, w - 150, 20);
            using (SolidBrush bBarBack = new SolidBrush(Color.FromArgb(20, 26, 40)))
                g.FillRectangle(bBarBack, barBg);

            int fillW = Math.Max(4, (int)(barBg.Width * progreso));
            Rectangle barFill = new Rectangle(barBg.X, barBg.Y, fillW, barBg.Height);
            using (LinearGradientBrush bGrad = new LinearGradientBrush(
                barBg,
                Color.FromArgb(0, 210, 255),
                Color.FromArgb(213, 0, 249),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(bGrad, barFill);
            }

            using (Pen pBar = new Pen(Color.Cyan, 1.5f))
                g.DrawRectangle(pBar, barBg);

            using (Font fMsg = new Font("Consolas", 10f, FontStyle.Bold))
            using (Font fFirma = new Font("Consolas", 9f, FontStyle.Regular))
            using (Font fSkip = new Font("Consolas", 8f, FontStyle.Italic))
            using (StringFormat sfCenter = new StringFormat { Alignment = StringAlignment.Center })
            {
                g.DrawString($"{mensajeActual}  [{(int)(progreso * 100)}%]", fMsg, Brushes.Cyan, new RectangleF(0, 315, w, 24), sfCenter);
                g.DrawString("NEXUS v1.0 — Desarrollado por Sebastián Zubieta Quiroga", fFirma, Brushes.LightGray, new RectangleF(0, 352, w, 22), sfCenter);
                g.DrawString("Haz clic o presiona cualquier tecla para omitir", fSkip, Brushes.DimGray, new RectangleF(0, 378, w, 18), sfCenter);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            timerAnim.Stop();
            timerAnim.Dispose();
            logoImg?.Dispose();
            base.OnFormClosed(e);
        }
    }
}