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
            string nombreIngresado = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Green;

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
            string realidadAsignada;
            // Generación random de nombre de la realidad
            {
                Random rnd = new Random();
                // 100 sustantivos generados por IA
                string[] nombresMundos = new string[]
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

                // 100 adjetivos generados por IA
                string[] adjetivos = new string[]
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

                realidadAsignada = $"{nombresMundos[rnd.Next(nombresMundos.Length)]} {adjetivos[rnd.Next(adjetivos.Length)]}";
            }
            Console.WriteLine($"\n{blanco}[SISTEMA NEXUS]{cian} Sincronizando coordenadas cuánticas...");
            Console.Write($"{blanco}[SISTEMA NEXUS]{cian} Realidad asignada al cadete: {magenta}{realidadAsignada}{cian}");



            Console.ReadKey();
        }
    }
    class Usuario
    {
        public string Nombre { get; private set; }
        public int Edad { get; private set; }
        public Usuario(string nombre, int edad)
        {
            Nombre = nombre;
            Edad = edad;
        }
    }
}
