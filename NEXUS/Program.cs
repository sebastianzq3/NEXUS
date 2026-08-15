namespace NEXUS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //aaaaaaa
            Console.ForegroundColor = ConsoleColor.Cyan;
            // Variables de colores en codigos ANSI
            string blanco = "\u001b[97m";
            string verde = "\x1b[32m";
            string cian = "\u001b[96m";
            string magenta = "\u001b[95m";

            // Nombre, edad, realidad asignada, nivel de energía, nivel

            // NOMBRE
            Console.WriteLine("Ingresa tu nombre:");
            Console.ForegroundColor = ConsoleColor.Green;
            string nombreIngresado = Console.ReadLine();

            // Validación nombre
            while (string.IsNullOrWhiteSpace(nombreIngresado))
            {

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: El nombre no puede estar vacío. Intenta de nuevo:");
                Console.ForegroundColor = ConsoleColor.Green;
                nombreIngresado = Console.ReadLine();
            }

            // EDAD
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Ingresa tu edad:");
            Console.ForegroundColor = ConsoleColor.Green;
            int edadIngresada;
            while (!int.TryParse(Console.ReadLine(), out edadIngresada) || edadIngresada < 18)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: La edad tiene que ser un número válido y tienes que ser mayor de 18 años. Intenta de nuevo:");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            // Creación usuario
            Console.Clear();
            Usuario cadete = new Usuario(nombreIngresado, edadIngresada);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Bienvenido, Explorador {verde}{cadete.Nombre} {cian}de {verde}{cadete.Edad} {cian}años.");

            // Asignación realidad
            Realidad realidadAsignada = new Realidad();
            Console.WriteLine($"\n{blanco}[SISTEMA NEXUS]{cian} Sincronizando coordenadas cuánticas...");
            Console.Write($"{blanco}[SISTEMA NEXUS]{cian} Realidad asignada al cadete: {magenta}{realidadAsignada.Nombre}{cian}");


            // (Tu código anterior termina en la asignación de la realidad)
            Console.WriteLine("\nPresiona cualquier tecla para entrar a la simulación...");
            Console.ReadKey();

            // ==========================================
            // EL BUCLE INFINITO (SESIÓN DE ENTRENAMIENTO)
            // ==========================================
            bool conectado = true;

            while (conectado)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("========================================");
                Console.WriteLine($"         {blanco}NEXUS TRAINING SYSTEM{cian}");
                Console.WriteLine("========================================");
                Console.WriteLine($"EXPLORADOR: {verde}{cadete.Nombre}{cian}");
                Console.WriteLine($"REALIDAD:   {magenta}{realidadAsignada.Nombre}{cian}");
                Console.WriteLine($"ENERGÍA:    {verde}{cadete.Energia}/{cadete.EnergiaMax}{cian}");
                Console.WriteLine($"ESTABILIDAD:{verde}{realidadAsignada.Estabilidad}%{cian}");
                Console.WriteLine("========================================\n");

                Console.WriteLine("1. Explorar realidad");
                Console.WriteLine("2. Analizar anomalía");
                Console.WriteLine("3. Consultar estado");
                Console.WriteLine("4. Recuperar energía");
                Console.WriteLine("5. Intentar desconexión");
                Console.Write("\nSeleccione una operación: ");

                Console.ForegroundColor = ConsoleColor.Green;
                string opcionStr = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Cyan;

                // Validación de entrada
                if (!int.TryParse(opcionStr, out int opcion))
                {
                    opcion = 0;
                }

                Console.Clear();
                // TOMA DE DECISIONES DE NEXUS
                switch (opcion)
                {
                    case 1: // Explorar realidad (Reduce energía, sube estabilidad)
                        if (cadete.Energia >= 3)
                        {
                            cadete.Energia -= 3;
                            realidadAsignada.Estabilidad += 15;
                            cadete.Experiencia += 25;
                            Console.WriteLine($"{blanco}[ACCIÓN]{cian} Explorando el sector...");
                            Console.WriteLine($"{verde}+15 Estabilidad{cian} | {verde}+25 EXP{cian}");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("NEXUS ADVIERTE: Energía insuficiente para explorar.");
                        }
                        break;

                    case 2: // Analizar anomalía (Consume energía, revela lore/afecta estabilidad)
                        if (cadete.Energia >= 2)
                        {
                            cadete.Energia -= 2;
                            Random eventoRandom = new Random();
                            int rng = eventoRandom.Next(0, 101);

                            Console.WriteLine($"{blanco}[ACCIÓN]{cian} Analizando fluctuaciones cuánticas...");

                            if (rng >= realidadAsignada.Estabilidad)
                            {
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("\n[ARCHIVO RECUPERADO]: 'La realidad que ustedes llaman simulación es solamente la realidad que todavía no comprenden.'");
                                Console.WriteLine("Has descubierto información clasificada sobre ORIGEN.");
                            }
                            else
                            {
                                realidadAsignada.Estabilidad -= 10;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n[PELIGRO]: La anomalía era inestable. Interferencia detectada.");
                                Console.WriteLine("La realidad pierde coherencia (-10 Estabilidad).");
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("NEXUS ADVIERTE: Energía insuficiente para analizar anomalías.");
                        }
                        break;

                    case 3: // Consultar estado
                        Console.WriteLine($"{blanco}[ESTADO DEL SISTEMA]{cian}");
                        Console.WriteLine($"Nivel del Cadete: {verde}{cadete.Nivel}{cian}");
                        Console.WriteLine($"Experiencia actual: {verde}{cadete.Experiencia}/100{cian}");
                        if (realidadAsignada.Estabilidad >= 80)
                        {
                            Console.WriteLine("Evaluación de la realidad: ESTABLE. Continúa el buen trabajo.");
                        }
                        else
                        {
                            Console.WriteLine("Evaluación de la realidad: INESTABLE. Requiere exploración urgente.");
                        }
                        break;

                    case 4: // Recuperar energía
                        if (cadete.Energia < cadete.EnergiaMax)
                        {
                            cadete.Energia += 4;
                            Console.WriteLine($"{blanco}[SOPORTE]{cian} Extrayendo energía del entorno...");
                            Console.WriteLine($"{verde}+4 Energía recuperada.{cian}");
                        }
                        else
                        {
                            Console.WriteLine($"{blanco}[SOPORTE]{cian} Los niveles de energía ya están al máximo.");
                        }
                        break;

                    case 5: // Desconexión
                        Console.WriteLine($"{blanco}[SISTEMA NEXUS]{cian} Iniciando protocolo de desconexión...");
                        Console.WriteLine("Guardando estado del cadete...");
                        Console.WriteLine($"\n{verde}Desconexión exitosa. Fin de la simulación.{cian}");
                        conectado = false;
                        break;

                    default: // Manejo de errores de entrada (Protocolo de seguridad)
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("ERROR: Operación no reconocida.");
                        Console.WriteLine("Penalización del sistema: -5 Estabilidad.");
                        realidadAsignada.Estabilidad -= 5;
                        break;
                }

                // EVENTO DE EMERGENCIA: ANOMALÍA IRIS
                if (conectado && realidadAsignada.Estabilidad <= 20)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\-----------------------------------------");
                    Console.WriteLine("       ALERTA DE INTERFERENCIA IRIS     ");
                    Console.WriteLine("----------------------------------------");
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("La estabilidad de la realidad está alcanzando niveles críticos.");
                    Console.WriteLine("NEXUS recomienda recuperación inmediata o desconexión.");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }

                if (conectado)
                {
                    Console.ResetColor();
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }
    }
    class Usuario
    {
        public string Nombre { get; private set; }
        public int Edad { get; private set; }
        public int EnergiaMax { get; private set; }
        private int _energia;
        public int Energia
        {
            get { return _energia; }
            set
            {
                if (value >= EnergiaMax)
                {
                    _energia = EnergiaMax;
                }
                else if (value < 0)
                {
                    _energia = 0;
                }
                else
                {
                    _energia = value;
                }
            }
        }
        public int Nivel { get; private set; }
        private int _experiencia;
        public int Experiencia
        {
            get { return _experiencia; }
            set
            {
                if (value >= 100)
                {
                    this.Nivel += (value / 100); // se suma uno de nivel por cada 100 de exp y el residuo queda en exp
                    _experiencia = (value % 100);
                }
                else
                {
                    _experiencia = value;
                }
            }
        }
        private int _estabilidad;
        public Usuario(string nombre, int edad)
        {
            Nombre = nombre;
            Edad = edad;
            EnergiaMax = 10;
            Energia = EnergiaMax;
            Nivel = 1;
            Experiencia = 0;
        }
    }
    class Realidad
    {
        private static string[] nombresMundos = new string[]
        {
            "Abismo", "Nether", "Limbo", "Horizonte", "Vacío", "Cosmos", "Nexo", "Núcleo", "Dominio", "Sector",
            "Sistema", "Anillo", "Bucle", "Ecosistema", "Refugio", "Santuario", "Origen", "Plano", "Vértice", "Edén",
            "Páramo", "Desierto", "Glaciar", "Purgatorio", "Inframundo", "Cúmulo", "Fragmento", "Vestigio", "Retazo", "Océano",
            "Continente", "Cráter", "Monolito", "Laberinto", "Portal", "Espejismo", "Eón", "Reino", "Imperio", "Paraíso",
            "Infierno", "Bastión", "Fuerte", "Castillo", "Palacio", "Templo", "Mausoleo", "Cementerio", "Bosque", "Pantano",
            "Archipiélago", "Satélite", "Asteroide", "Planeta", "Meteoro", "Cometa", "Sol", "Agujero", "Cénit", "Nadir",
            "Crepúsculo", "Ocaso", "Amanecer", "Multiverso", "Microcosmos", "Macrocosmos", "Holograma", "Simulador", "Servidor", "Nodo",
            "Puerto", "Enlace", "Espectro", "Fantasma", "Esqueleto", "Coliseo", "Engranaje", "Mecanismo", "Motor", "Reactor",
            "Generador", "Faro", "Centinela", "Guardián", "Vigilante", "Peregrino", "Exilio", "Destierro", "Umbral", "Precipicio",
            "Risco", "Cañón", "Valle", "Monte", "Pico", "Foso", "Pozo", "Letargo", "Cristal", "Prisma"
        };
        private static string[] adjetivos = new string[]
        {
            "Olvidado", "Sangriento", "Oscuro", "Luminoso", "Roto", "Eterno", "Infinito", "Fragmentado", "Perdido", "Oculto",
            "Silencioso", "Carmesí", "Dorado", "Metálico", "Cuántico", "Cibernético", "Arcano", "Místico", "Profundo", "Letal",
            "Tóxico", "Mutante", "Primigenio", "Desolado", "Sombrío", "Gélido", "Ardiente", "Ceniciento", "Corrupto", "Purificado",
            "Maldito", "Bendito", "Sagrado", "Profano", "Radiactivo", "Mecánico", "Orgánico", "Sintético", "Virtual", "Digital",
            "Analógico", "Astral", "Cósmico", "Estelar", "Solar", "Lunar", "Galáctico", "Dimensional", "Espectral", "Fantasmal",
            "Invisible", "Intangible", "Cristalino", "Vítreo", "Pétreo", "Férreo", "Óseo", "Carnoso", "Sanguinolento", "Putrefacto",
            "Marchito", "Floreciente", "Vívido", "Opaco", "Traslúcido", "Resplandeciente", "Cegador", "Tenebroso", "Lúgubre", "Macabro",
            "Siniestro", "Grotesco", "Sublime", "Majestuoso", "Imponente", "Colosal", "Titánico", "Enano", "Microscópico", "Infinitesimal",
            "Absoluto", "Relativo", "Paradójico", "Caótico", "Ordenado", "Lineal", "Cíclico", "Espiral", "Fracturado", "Intacto",
            "Virgen", "Inexplorado", "Conocido", "Desconocido", "Aislado", "Conectado", "Entrelazado", "Superpuesto", "Invertido", "Distorsionado"
        };
        public string Nombre { get; private set; }

        private int _estabilidad;
        public int Estabilidad
        {
            get { return _estabilidad; }
            set
            {
                if (value >= 100) _estabilidad = 100;
                else if (value <= 0) _estabilidad = 0;
                else _estabilidad = value;
            }
        }
        public Realidad()
        {
            Random rnd = new Random();
            this.Nombre = $"{nombresMundos[rnd.Next(nombresMundos.Length)]} {adjetivos[rnd.Next(adjetivos.Length)]}";
            this.Estabilidad = rnd.Next(30, 70);
        }
    }
}
