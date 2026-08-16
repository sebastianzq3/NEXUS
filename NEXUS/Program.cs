using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NEXUS
{
    internal class Program
    {
        static void Main(string[] args)
        {
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



            Console.WriteLine("\nPresiona cualquier tecla para entrar a la simulación...");
            Console.ReadKey();

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
                    Console.WriteLine("----------------------------------------");
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
    public enum Anomalia
    {
        Tiempo, //0
        Espacio,//1
        Mente,  //2
        Silencio//3

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
        public Anomalia Anomalia { get; private set; }
        public Realidad()
        {
            Random rnd = new Random();
            this.Nombre = $"{nombresMundos[rnd.Next(nombresMundos.Length)]} {adjetivos[rnd.Next(adjetivos.Length)]}";
            this.Estabilidad = rnd.Next(30, 70);
            this.Anomalia = (Anomalia)rnd.Next(0, 4);
        }
    }
    class Objeto
    {
        static object[,] Catalogo =
        {

            // ANOMALÍA: TIEMPO (Objetos para manipular o anclar el flujo temporal)

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

            // ANOMALÍA: ESPACIO (Objetos para orientación y topografía no euclidiana)

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

            // ANOMALÍA: MENTE (Objetos psicológicos, memorias y anclajes de cordura)

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

            // ANOMALÍA: SILENCIO (Objetos acústicos, generadores de ruido y frecuencias)

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

        public string Nombre { get; private set; }
        public Anomalia Contrarresta { get; private set; }
        public string Descripcion { get; private set; }
        public int Usos { get; set; } //public set para que podamos cambiarlo

        public Anomalia Anomalia { get; private set; }
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
