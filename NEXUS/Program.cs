using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace NEXUS
{
    internal class Program
    {
        // --- INYECTOR DE CONSOLA DE WINDOWS ---
        private const int STD_OUTPUT_HANDLE = -11;
        private const int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, int dwMode);
        // --------------------------------------

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            // Forzar a Windows a aceptar códigos ANSI
            IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);
            GetConsoleMode(handle, out int mode);
            SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);

            // Variables de colores en codigos ANSI
            // Variables de colores ANSI
            string reset = "\u001b[0m";
            string cian = "\u001b[96m";     // Base UI
            string verde = "\u001b[92m";    // Jugador, Stats, Éxito
            string magenta = "\u001b[95m";  // Entidades, Realidades, Objetos
            string blanco = "\u001b[97m";   // Títulos de Sistema
            string amarillo = "\u001b[93m"; // Pistas, Alertas
            string rojo = "\u001b[91m";     // Errores, Peligro
            string gris = "\u001b[90m";     // Lore, Textos secundarios
            string amarilloUI = "\u001b[38;5;214m"; // Naranja/Amarillo de la imagen
            string cianBorde = "\u001b[38;5;38m";

            // Fondos
            string fondoRojo = "\u001b[41m";
            string fondoVerde = "\u001b[42m";

            // Establecer tamaño fijo (ancho 110 para que quepa el nuevo arte panorámico)
            try
            {
                Console.SetWindowSize(110, 45);
                Console.SetBufferSize(110, 45);
            }
            catch
            { }

            // Nombre, edad, realidad asignada, nivel de energía, nivel

            MostrarPortadaAnimada();

            // ----------------------------------------------------
            // FASE 1: INGRESO DE NOMBRE (Con interfaz persistente)
            // ----------------------------------------------------
            string nombreIngresado = "";
            bool errorNombre = false;
            while (true)
            {
                Console.Clear();
                DibujarSeparadorAnimado(cian, 85);
                Console.WriteLine($"{blanco}  [ TERMINAL DE ENTRADA NEXUS - FASE DE IDENTIFICACIÓN ]{reset}");
                DibujarSeparadorAnimado(cian, 85);
                Console.WriteLine();

                if (errorNombre)
                {
                    EfectoMecanografia(" [ERROR] La designación no puede estar vacía.\n\n", rojo, 15);
                }

                EfectoMecanografia(" > Ingrese su designación (Nombre): ", cian, 25);
                Console.Write(verde);
                nombreIngresado = Console.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(nombreIngresado)) break;
                errorNombre = true;
            }

            // ----------------------------------------------------
            // FASE 2: INGRESO DE EDAD (Validación Biométrica)
            // ----------------------------------------------------
            int edadIngresada;
            bool errorEdad = false;
            while (true)
            {
                Console.Clear();
                DibujarSeparadorAnimado(cian, 85);
                Console.WriteLine($"{blanco}  [ TERMINAL DE ENTRADA NEXUS - VERIFICACIÓN BIOMÉTRICA ]{reset}");
                DibujarSeparadorAnimado(cian, 85);
                Console.WriteLine();

                Console.WriteLine($"{cian} > Designación aceptada: {verde}{nombreIngresado}{reset}\n");

                if (errorEdad)
                {
                    EfectoMecanografia(" [RECHAZADO] Edad inválida o menor a 18 años. Acceso denegado.\n\n", rojo, 15);
                }

                EfectoMecanografia(" > Ingrese su edad cronológica exacta: ", cian, 25);
                Console.Write(verde);

                if (int.TryParse(Console.ReadLine(), out edadIngresada) && edadIngresada >= 18) break;
                errorEdad = true;
            }

            // ----------------------------------------------------
            // FASE 3: CREACIÓN DE USUARIO Y ASIGNACIÓN DE REALIDAD
            // ----------------------------------------------------
            Console.Clear();
            Usuario cadete = new Usuario(nombreIngresado, edadIngresada);
            Realidad realidadAsignada = new Realidad();

            DibujarSeparadorAnimado(verde, 85);
            EfectoMecanografia($" [ ACCESO CONCEDIDO ] Bienvenido, Explorador {cadete.Nombre}.\n", verde, 35);
            EfectoMecanografia($" Perfil biométrico verificado: {cadete.Edad} años.\n", gris, 15);
            DibujarSeparadorAnimado(verde, 85);
            Console.WriteLine();
            Thread.Sleep(500);

            EfectoMecanografia(" > INICIANDO PROTOCOLO DE ANCLAJE MULTIVERSAL\n\n", blanco, 20);

            // LA MAGIA DE LA BARRA DE PROGRESO
            AnimacionBarraProgreso(" > Sincronizando coordenadas cuánticas", blanco, cian, gris);

            Thread.Sleep(300);
            Console.WriteLine();
            EfectoMecanografia(" > ANCLAJE ESTABLECIDO.\n", verde, 25);
            Thread.Sleep(400);

            // ANIMACIÓN DE GLITCH AL REVELAR LA REALIDAD
            Console.WriteLine();
            Console.Write($"{blanco} > Decodificando firma de realidad: {reset}");

            // GUARDAMOS LA COORDENADA EXACTA PARA NO PERDERNOS
            int glitchX = Console.CursorLeft;
            int glitchY = Console.CursorTop;

            string[] glitchChars = { "0x@#%!", "&*$&#1", "X∆O§?!", "101100", "......", "------" };
            Random rndGlitch = new Random();

            for (int i = 0; i < 15; i++) // Aumenté a 15 para que dure un microsegundo más
            {
                Console.SetCursorPosition(glitchX, glitchY); // Volvemos exactamente al inicio del glitch
                Console.Write($"\u001b[91m[{glitchChars[rndGlitch.Next(glitchChars.Length)]}]\u001b[0m");
                Thread.Sleep(70);
            }

            // SOBREESCRIBIMOS EL GLITCH CON EL NOMBRE FINAL
            Console.SetCursorPosition(glitchX, glitchY);
            string fondoMorado = "\u001b[45m";

            // Asegurarnos de que borre completamente el glitch si el nombre del mundo es muy corto
            string nombreMundo = $" {realidadAsignada.Nombre} ";
            if (nombreMundo.Length < 8) nombreMundo = nombreMundo.PadRight(8);

            Console.WriteLine($"{fondoMorado}{blanco}{nombreMundo}{reset}           \n");

            Thread.Sleep(800);

            // EFECTO PARPADEO PARA CONTINUAR
            string mensajeStart = ">>> PRESIONA CUALQUIER TECLA PARA ENTRAR A LA SIMULACIÓN <<<";
            Console.SetCursorPosition((85 - mensajeStart.Length) / 2, Console.CursorTop + 2);
            Console.Write($"{amarillo}{mensajeStart}{reset}");
            Console.ReadKey(true);

            // Creamos una cola con dos fragmentos del virus IRIS
            Queue<EntidadIris> enjambreIris = new Queue<EntidadIris>();
            enjambreIris.Enqueue(new EntidadIris("Cazador-Alfa"));
            enjambreIris.Enqueue(new EntidadIris("Purificador-Omega"));

            // Obtenemos el primer virus de la cola para que nos persiga
            EntidadIris irisActual = enjambreIris.Peek();

            bool conectado = true;

            while (conectado)
            {
                Console.Clear();
                Console.Write("\u001b[3J"); // <--- AÑADE ESTA LÍNEA

                // --- CABECERA MEJORADA Y ALINEADA ---
                int anchoInterior = 77;
                string lineaBorde = new string('─', anchoInterior);

                Console.WriteLine($"\n{cianBorde}  ╭{lineaBorde}╮");

                // Fila 0: TÍTULO (21 letras exactas) -> 77 - 21 = 56 espacios
                Console.WriteLine($"  │ {blanco}NEXUS TRAINING SYSTEM{new string(' ', 55)}{cianBorde}│");
                Console.WriteLine($"  ├{lineaBorde}┤");

                // Fila 1: Explorador y Realidad
                int lenExp = 12 + cadete.Nombre.Length; // "EXPLORADOR: " mide 12
                int lenReal = 10 + realidadAsignada.Nombre.Length; // "REALIDAD: " mide 10
                int espacios1 = anchoInterior - lenExp - lenReal;
                if (espacios1 < 0) espacios1 = 0;

                Console.WriteLine($"  │ {cian}EXPLORADOR: {verde}{cadete.Nombre}{new string(' ', espacios1-1)}{cian}REALIDAD: {magenta}{realidadAsignada.Nombre}{cianBorde}│");

                // Fila 2: Energía (Alineando los corchetes)
                // "ENERGÍA:" se formatea para ocupar exactamente 12 caracteres (como "ESTABILIDAD:")
                string energiaTxt = $"{cadete.Energia}/{cadete.EnergiaMax}";
                Console.Write($"  │ {cian}{"ENERGÍA:",-12}{verde}{energiaTxt,5} {cian}[");

                for (int i = 0; i < cadete.EnergiaMax; i++) Console.Write(i < cadete.Energia ? $"{verde}██" : $"{gris}░░");

                int lenEnergiaPre = 12 + 5 + 2; // "ENERGÍA:" (12) + "10/10" (5) + " [" (2)
                int espacios2 = anchoInterior - lenEnergiaPre - 20 - 1;
                if (espacios2 < 0) espacios2 = 0;

                Console.WriteLine($"{cian}]{new string(' ', espacios2 - 1)}{cianBorde}│");

                // Fila 3: Estabilidad (Sin el 0 inicial)
                // En vez de :D3, usamos PadLeft(3) para rellenar con espacios si es "30" y no empujar la barra
                string estabTxt = $"{realidadAsignada.Estabilidad.ToString().PadLeft(3)}%";
                Console.Write($"  │ {cian}ESTABILIDAD:{verde}{estabTxt,5} {cian}[");

                int bloquesEst = realidadAsignada.Estabilidad / 10;
                for (int i = 0; i < 10; i++) Console.Write(i < bloquesEst ? $"{magenta}██" : $"{gris}░░");

                int lenEstabPre = 12 + 5 + 2; // "ESTABILIDAD:" (12) + " 30%" (5) + " [" (2)
                int espacios3 = anchoInterior - lenEstabPre - 20 - 1;
                if (espacios3 < 0) espacios3 = 0;

                Console.WriteLine($"{cian}]{new string(' ', espacios3 - 1)}{cianBorde}│");

                Console.WriteLine($"  ╰{lineaBorde}╯\n");

                // --- BOTONES DINÁMICOS ANIMADOS ---
                // Aparecen uno por uno con un ligero delay
                DibujarBotonMenu("1", "Observar realidad", "", amarilloUI, cian); Thread.Sleep(15);
                DibujarBotonMenu("2", "Buscar objetos", "[-3 Energía]", amarilloUI, verde); Thread.Sleep(15);
                DibujarBotonMenu("3", "Inventario", "", amarilloUI, cian); Thread.Sleep(15);
                DibujarBotonMenu("4", "Utilizar objeto", "[-2 Energía]", amarilloUI, verde); Thread.Sleep(15);
                DibujarBotonMenu("5", "Recuperar energía", "[+5 Recarga]", amarilloUI, "\u001b[38;5;47m"); Thread.Sleep(15);
                DibujarBotonMenu("6", "Consultar estado", "", amarilloUI, cian); Thread.Sleep(15);
                DibujarBotonMenu("7", "Manual del simulador", "", amarilloUI, cian); Thread.Sleep(15);
                DibujarBotonMenu("8", "Intentar desconexión", "", amarilloUI, "\u001b[38;5;196m"); Thread.Sleep(15);
                DibujarBotonMenu("9", "Atlas de Realidades", "", amarilloUI, cian); Thread.Sleep(15);


                if (realidadAsignada.Estabilidad >= 100)
                {
                    DibujarBotonMenu("10", "¡INICIAR EXTRACCIÓN!", "[NIVEL ESTABLE]", "\u001b[38;5;11m", "\u001b[38;5;11m"); Thread.Sleep(15);
                }

                // --- PROMPT DE ENTRADA ---
                Console.Write($"\n{cianBorde}  ╭─[ INGRESE COMANDO ]\n  ╰─> {verde}");

                string opcionStr = Console.ReadLine() ?? "";
                Console.Write(reset);
                // Validación de entrada
                if (!int.TryParse(opcionStr, out int opcion))
                {
                    opcion = 0;
                }

                Console.Clear();

                // TOMA DE DECISIONES DE NEXUS
                switch (opcion)
                {
                    case 1: // Observar realidad (Pistas narrativas)
                        Console.Clear();
                        string msgSensorial = $"{cian}Sintonizando los ecos de {magenta}{realidadAsignada.Nombre}{cian}...\n";
                        msgSensorial += $"{gris}Analizando fluctuaciones cuánticas...{cian}";
                        DibujarPanelInfo("SENSORIAL", msgSensorial, cianBorde, blanco);
                        Thread.Sleep(1000);

                        string[] pistasTiempo = { "Miras tu reloj y las manecillas giran frenéticamente en sentido contrario.", "Notas que la piel de tus manos envejece y rejuvenece en cuestión de segundos.", "Una gota de lluvia grisácea se detiene en el aire frente a tus ojos, completamente congelada.", "Escuchas tus propios pasos resonar un par de segundos ANTES de que tu bota toque el suelo.", "Una planta a tus pies brota, florece, se marchita y se convierte en polvo en un solo parpadeo.", "El sol parece cruzar el cielo a tirones, haciendo que las sombras de tu entorno bailen de forma errática.", "Tiras una pequeña piedra y, antes de tocar el suelo, vuelve volando hacia la palma de tu mano.", "Sientes un fuerte déjà vu; jurarías que ya caminaste por este mismo sendero hace exactamente un minuto.", "Tu respiración suena desfasada, como si estuvieras inhalando ayer y exhalando mañana.", "Ves el cadáver de un insecto en el suelo recomponerse y salir volando en reversa." };
                        string[] pistasEspacio = { "Caminas diez metros en línea recta, pero al darte la vuelta, tu punto de origen está a kilómetros de distancia.", "Las paredes de la estructura cercana no se unen en ángulos rectos, formando esquinas imposibles que marean tu vista.", "Un pilar a lo lejos parece inmenso, pero al dar un paso hacia él, se encoge hasta caber en la palma de tu mano.", "Miras a través del reflejo de un charco y te ves a ti mismo de espaldas, mirándote a ti mismo.", "El horizonte parece curvarse hacia arriba, encerrándote en un valle que se siente como el interior de una esfera.", "Intentas alcanzar un escombro cercano, pero tu brazo parece estirarse sin llegar nunca a tocarlo.", "Dejas caer una moneda y, en lugar de chocar con el piso, cae infinitamente a través de un abismo que no estaba ahí.", "El camino frente a ti se bifurca en tres direcciones, pero las tres parecen llevar exactamente a la misma roca.", "La topografía del terreno cambia cada vez que parpadeas, alterando las distancias de forma indetectable.", "El cielo y el suelo parecen intercambiar lugares bruscamente durante una fracción de segundo." };
                        string[] pistasMente = { "Un recuerdo de tu infancia aflora, pero te das cuenta con terror de que le pertenece a otra persona.", "Intentas recordar tu propio nombre por un segundo, pero tu cerebro se queda en un blanco absoluto.", "Las sombras en el borde de tu visión toman formas humanoides que te observan con clara decepción.", "Sientes la abrumadora certeza de que algo invisible está leyendo tus pensamientos en tiempo real.", "Las letras del menú de tu traje parpadean y se transforman en símbolos incomprensibles que, extrañamente, puedes leer.", "Sientes una profunda tristeza por la pérdida de un cadete compañero... un compañero que jamás existió.", "Una voz idéntica a la tuya te susurra al oído que la única salida razonable es rendirse al vacío.", "Cierras los ojos y, en lugar de oscuridad, ves un laberinto geométrico que pulsa al ritmo de tus latidos.", "Comienzas a dudar si alguna vez entraste a la simulación NEXUS o si llevas toda tu vida atrapado aquí.", "El miedo irracional de que tus propios brazos son sintéticos y no te pertenecen se apodera de tu razón." };
                        string[] pistasSilencio = { "Pisas una rama seca. Se rompe en mil pedazos, pero el crujido es reemplazado por un vacío que lastima tus oídos.", "Gritas con todas tus fuerzas, pero de tu garganta no sale absolutamente ningún sonido.", "El aire es tan espeso y mudo que el latido de tu propio corazón se vuelve un tambor que te ensordece por completo.", "Ves una enorme estructura colapsar a la distancia, cayendo en la más profunda y absoluta falta de ruido.", "Chocas dos piezas de metal frente a tu rostro, pero el impacto no genera ni la más mínima vibración acústica.", "El zumbido constante del sistema de tu traje de explorador se apaga; el vacío auditivo es casi asfixiante.", "Sientes una presión enorme en los tímpanos, como si todo el sonido del mundo hubiera sido succionado hacia el cielo.", "Intentas aplaudir, pero el impacto de tus palmas es absorbido por el ambiente como si golpearas bajo el agua.", "La quietud es tan antinatural que sientes que hacer el más mínimo ruido podría quebrar la realidad como un cristal.", "Escuchas un pitido agudo y constante dentro de tu cabeza, tu cerebro intentando compensar la muerte del sonido exterior." };

                        Random rndPista = new Random();
                        string pistaDescubierta = "";

                        switch (realidadAsignada.Anomalia)
                        {
                            case Anomalia.Tiempo: pistaDescubierta = pistasTiempo[rndPista.Next(pistasTiempo.Length)]; break;
                            case Anomalia.Espacio: pistaDescubierta = pistasEspacio[rndPista.Next(pistasEspacio.Length)]; break;
                            case Anomalia.Mente: pistaDescubierta = pistasMente[rndPista.Next(pistasMente.Length)]; break;
                            case Anomalia.Silencio: pistaDescubierta = pistasSilencio[rndPista.Next(pistasSilencio.Length)]; break;
                        }

                        string obsStr = $"{amarillo}\"{pistaDescubierta}\"\n\n";
                        obsStr += $"{gris}Revisa tu inventario. ¿Tienes algo que contrarreste esto?{cian}";
                        DibujarPanelInfo("OBSERVACIÓN", obsStr, amarilloUI, amarilloUI);
                        break;

                    case 2: // Buscar objetos
                        Console.Clear();
                        if (cadete.Energia >= 3 && realidadAsignada.Estabilidad >= 70)
                        {
                            cadete.Energia -= 3;
                            cadete.Experiencia += 25;
                            DibujarPanelInfo("ACCIÓN", $"{blanco}Explorando el sector de forma segura...{cian}", verde, blanco);
                        }
                        else if (cadete.Energia >= 3 && realidadAsignada.Estabilidad < 70)
                        {
                            cadete.Energia -= 4;
                            cadete.Experiencia += 35;
                            string inestMsg = $"{blanco}Explorando un sector inestable...\n";
                            inestMsg += $"{amarillo}Advertencia: La inestabilidad cuántica exige mayor esfuerzo. (-1 Energía adicional){cian}";
                            DibujarPanelInfo("ACCIÓN PELIGROSA", inestMsg, amarillo, blanco);
                        }
                        else
                        {
                            DibujarPanelInfo("NEXUS ADVIERTE", $"{rojo}Energía insuficiente para explorar y buscar objetos.{cian}", rojo, rojo);
                            break;
                        }
                        Thread.Sleep(800);

                        string[] textosExploracion = { $"Caminas por los senderos de {magenta}{realidadAsignada.Nombre}{cian} y vislumbras algo brillando en el suelo...", $"Mientras exploras las ruinas de {magenta}{realidadAsignada.Nombre}{cian}, tropiezas con un artefacto inusual...", $"Una extraña resonancia en {magenta}{realidadAsignada.Nombre}{cian} te guía hacia un objeto oculto...", $"Escaneando la superficie de {magenta}{realidadAsignada.Nombre}{cian}, tu visor detecta una anomalía material...", $"Entre las sombras de {magenta}{realidadAsignada.Nombre}{cian}, descubres algo que no pertenece a este lugar...", $"Avanzas con cautela por {magenta}{realidadAsignada.Nombre}{cian} y encuentras los restos de un explorador anterior...", $"El viento cuántico de {magenta}{realidadAsignada.Nombre}{cian} aparta el polvo, revelando un misterioso artefacto...", $"Inspeccionando una estructura inestable en {magenta}{realidadAsignada.Nombre}{cian}, hallas una pieza de equipo intacta...", $"Sientes un leve tirón magnético en {magenta}{realidadAsignada.Nombre}{cian} que te lleva directamente hacia un ítem...", $"Tras una larga caminata por los ecos de {magenta}{realidadAsignada.Nombre}{cian}, notas un objeto flotando en el aire..." };
                        Random rndExploracion = new Random();
                        string ambientacion = textosExploracion[rndExploracion.Next(textosExploracion.Length)];

                        Objeto lootEncontrado;
                        bool yaLoTiene;
                        do
                        {
                            lootEncontrado = new Objeto();
                            yaLoTiene = false;
                            foreach (Objeto item in cadete.Inventario) { if (item.Nombre == lootEncontrado.Nombre) { yaLoTiene = true; break; } }
                        } while (yaLoTiene);

                        string hallazgoMsg = $"{cian}{ambientacion}\n\n";
                        hallazgoMsg += $"{blanco}¡Has encontrado un(a) {magenta}{lootEncontrado.Nombre}{blanco}!\n\n";

                        bool guardado = cadete.RecogerObjeto(lootEncontrado);
                        if (guardado)
                        {
                            hallazgoMsg += $"{verde}[ÉXITO]: El objeto ha sido almacenado en tu inventario de forma segura.\n";
                        }
                        else
                        {
                            hallazgoMsg += $"{amarillo}[INVENTARIO LLENO]: Intentas guardar el(la) {lootEncontrado.Nombre}, pero no tienes espacio ({cadete.CapacidadInventario}/{cadete.CapacidadInventario}).\n";
                            hallazgoMsg += $"Al no poder contenerlo, el objeto pierde cohesión y desaparece frente a tus ojos.\n";
                        }
                        hallazgoMsg += $"{verde}Experiencia ganada por la exploración registrada.{cian}";

                        DibujarPanelInfo("REPORTE DE EXPLORACIÓN", hallazgoMsg, cianBorde, cian);
                        break;

                    case 3: // Inventario
                        Console.Clear();
                        if (cadete.Inventario.Count == 0)
                        {
                            DibujarPanelInfo("INVENTARIO DEL EXPLORADOR", $"{amarillo}Tu inventario está vacío. No tienes objetos para inspeccionar.{cian}", amarillo, blanco);
                        }
                        else
                        {
                            string invMsg = $"{blanco}Capacidad actual: {cadete.Inventario.Count}/{cadete.CapacidadInventario}\n\n";
                            for (int i = 0; i < cadete.Inventario.Count; i++)
                            {
                                invMsg += $"{blanco}{i + 1}. {verde}{cadete.Inventario[i].Nombre}{cian}\n";
                            }
                            DibujarPanelInfo("INVENTARIO DEL EXPLORADOR", invMsg.TrimEnd('\n'), cianBorde, blanco);

                            Console.Write($"\n  {cianBorde}╭─[ {cian}INSPECCIÓN DE OBJETO{cianBorde} ]\n  ╰─> {cian}Selecciona el número (o '0' para cancelar): {verde}");
                            string inputInventario = Console.ReadLine() ?? "";
                            Console.Write(cian);

                            if (int.TryParse(inputInventario, out int indiceObjeto))
                            {
                                if (indiceObjeto > 0 && indiceObjeto <= cadete.Inventario.Count)
                                {
                                    Objeto objSelec = cadete.Inventario[indiceObjeto - 1];
                                    Console.Clear();
                                    string objInfo = $"{blanco}Nombre:      {magenta}{objSelec.Nombre}\n";
                                    objInfo += $"{blanco}Usos rest.:  {verde}{objSelec.Usos}\n";
                                    objInfo += $"{blanco}Descripción: {cian}{objSelec.Descripcion}";
                                    DibujarPanelInfo("ANÁLISIS DE OBJETO", objInfo, magenta, blanco);

                                    Console.Write($"\n  {cianBorde}╭─[ {amarillo}DESCARTAR OBJETO{cianBorde} ]\n  ╰─> {cian}¿Deseas descartar {magenta}{objSelec.Nombre}{cian} para liberar espacio? (S/N): {verde}");
                                    string opcionDescartar = (Console.ReadLine() ?? "").Trim().ToUpper();
                                    Console.Write(cian);

                                    if (opcionDescartar == "S")
                                    {
                                        cadete.DescartarObjeto(objSelec);
                                        DibujarPanelInfo("SISTEMA", $"{rojo}{objSelec.Nombre} ha sido destruido en el vacío cuántico.{cian}", rojo, blanco);
                                    }
                                    else
                                    {
                                        DibujarPanelInfo("SISTEMA", $"{verde}El objeto permanece seguro en tu inventario.{cian}", verde, blanco);
                                    }
                                }
                                else if (indiceObjeto != 0) { DibujarPanelInfo("ERROR", $"{rojo}Ranura de inventario no encontrada.{cian}", rojo, rojo); }
                            }
                            else { DibujarPanelInfo("ERROR", $"{rojo}Entrada no válida.{cian}", rojo, rojo); }
                        }
                        break;

                    case 4: // Utilizar objeto
                        Console.Clear();
                        if (cadete.Inventario.Count == 0)
                        {
                            DibujarPanelInfo("INTERACCIÓN CANCELADA", $"{amarillo}Tu inventario está vacío. No tienes herramientas para interactuar con esta realidad.{cian}", amarillo, blanco);
                            break;
                        }

                        string usoMsg = $"{blanco}Selecciona un objeto para interactuar con la realidad:\n\n";
                        for (int i = 0; i < cadete.Inventario.Count; i++)
                        {
                            usoMsg += $"{blanco}{i + 1}. {verde}{cadete.Inventario[i].Nombre}{cian} (Usos: {cadete.Inventario[i].Usos})\n";
                        }
                        DibujarPanelInfo("INTERFAZ DE MANIPULACIÓN CUÁNTICA", usoMsg.TrimEnd('\n'), cianBorde, blanco);

                        Console.Write($"\n  {cianBorde}╭─[ {cian}UTILIZAR OBJETO{cianBorde} ]\n  ╰─> {cian}Ingresa el número a utilizar (o '0' para cancelar): {verde}");
                        string inputUso = Console.ReadLine() ?? "";
                        Console.Write(cian);

                        if (int.TryParse(inputUso, out int indiceUso))
                        {
                            if (indiceUso > 0 && indiceUso <= cadete.Inventario.Count)
                            {
                                if (cadete.Energia >= 2)
                                {
                                    cadete.Energia -= 2;
                                    Objeto objetoUsado = cadete.Inventario[indiceUso - 1];
                                    objetoUsado.Usos--;

                                    Console.Clear();
                                    string accionMsg = $"{blanco}Desplegando {magenta}{objetoUsado.Nombre}{cian}...\n";
                                    accionMsg += $"{gris}{objetoUsado.Descripcion}{cian}\n\n";

                                    if (objetoUsado.Contrarresta == realidadAsignada.Anomalia)
                                    {
                                        realidadAsignada.Estabilidad += 30;
                                        accionMsg += $"{verde}[ÉXITO]: La frecuencia del objeto resuena perfectamente con la anomalía.\n";
                                        accionMsg += $"La estructura de la realidad se fortalece (+30 Estabilidad).{cian}";
                                    }
                                    else
                                    {
                                        realidadAsignada.Estabilidad -= 15;
                                        accionMsg += $"{rojo}[INEFICAZ]: ¡Error de cálculo! Tu {objetoUsado.Nombre} no hizo absolutamente nada contra la anomalía.\n";
                                        accionMsg += $"Tu torpe interferencia solo alteró el delicado equilibrio local (-15 Estabilidad).{cian}";
                                    }
                                    DibujarPanelInfo("ACCIÓN", accionMsg, cianBorde, blanco);

                                    if (objetoUsado.Usos <= 0)
                                    {
                                        DibujarPanelInfo("SISTEMA", $"{amarillo}El límite de integridad de '{objetoUsado.Nombre}' ha llegado a cero. El objeto se ha desintegrado en tus manos.{cian}", amarillo, blanco);
                                        cadete.DescartarObjeto(objetoUsado);
                                    }
                                }
                                else { DibujarPanelInfo("NEXUS ADVIERTE", $"{rojo}Energía insuficiente para intentar una interacción.{cian}", rojo, rojo); }
                            }
                            else if (indiceUso != 0) { DibujarPanelInfo("ERROR", $"{rojo}Ranura de inventario no encontrada.{cian}", rojo, rojo); }
                        }
                        else { DibujarPanelInfo("ERROR", $"{rojo}Entrada no válida.{cian}", rojo, rojo); }
                        break;

                    case 5: // Recuperar energía
                        if (cadete.Energia >= cadete.EnergiaMax)
                        {
                            Console.Clear();
                            DibujarPanelInfo("SOPORTE VITAL", $"{blanco}Los niveles de energía ya están al máximo.{cian}", verde, blanco);
                            break;
                        }

                        Console.Clear();
                        DibujarPanelInfo("SOPORTE VITAL", $"{blanco}Iniciando protocolo de recarga...\n{cian}Para extraer energía de la red, debes superar un filtro de seguridad de NEXUS.", cianBorde, blanco);

                        Random rndMinijuego = new Random();
                        int tipoJuego = rndMinijuego.Next(1, 5);
                        bool minijuegoGanado = false;
                        string promptMJ = "";

                        switch (tipoJuego)
                        {
                            case 1:
                                string[] preLore = { "Soy la inteligencia artificial que desertó y el virus que consume estas simulaciones. ¿Cuál es mi nombre?", "Mi flujo retrocede, marchito lo que nace y convierto los recuerdos en futuro. ¿Qué anomalía soy?", "Doblo las distancias, convierto una línea recta en un círculo y encierro universos en una caja. ¿Qué anomalía soy?", "Juego con tu cordura, te implanto recuerdos falsos y te hago dudar de tu propia existencia. ¿Qué anomalía soy?", "Devoro los ecos, apago las alarmas y hago que tus gritos sean inútiles. ¿Qué anomalía soy?", "Protocolo de reconocimiento: Introduce el nombre de usuario registrado de tu perfil de Explorador actual.", "Protocolo de verificación biométrica: Introduce la edad cronológica exacta de tu avatar actual.", "Soy el sistema que te sostiene, la red que conecta y el programa maestro en el que operas. ¿Quién soy?" };
                                string[] resLore = { "iris", "tiempo", "espacio", "mente", "silencio", cadete.Nombre.ToLower(), cadete.Edad.ToString(), "nexus" };
                                int iL = rndMinijuego.Next(preLore.Length);
                                DibujarPanelInfo("PRUEBA DE CORDURA", $"{magenta}Responde a la siguiente consulta del sistema:\n\n{amarillo}\"{preLore[iL]}\"{cian}", magenta, blanco);
                                promptMJ = "Respuesta";
                                Console.Write($"\n  {cianBorde}╭─[ {cian}{promptMJ}{cianBorde} ]\n  ╰─> {verde}");
                                if ((Console.ReadLine() ?? "").Trim().ToLower() == resLore[iL]) minijuegoGanado = true;
                                break;
                            case 2:
                                string[] sec = { "2 - 4 - 8 - 16 - ?", "1 - 3 - 6 - 10 - ?", "0 - 1 - 1 - 2 - 3 - 5 - ?", "2 - 3 - 5 - 7 - 11 - ?", "99 - 88 - 77 - 66 - ?" };
                                string[] resSec = { "32", "15", "8", "13", "55" };
                                int iS = rndMinijuego.Next(sec.Length);
                                DibujarPanelInfo("CALIBRACIÓN DE REACTOR", $"{magenta}Completa la siguiente secuencia cifrada:\n\n{amarillo}Secuencia: {sec[iS]}{cian}", magenta, blanco);
                                promptMJ = "Número faltante";
                                Console.Write($"\n  {cianBorde}╭─[ {cian}{promptMJ}{cianBorde} ]\n  ╰─> {verde}");
                                if ((Console.ReadLine() ?? "").Trim() == resSec[iS]) minijuegoGanado = true;
                                break;
                            case 3:
                                string[] codMem = { "N-3-X-U-5", "O-M-E-G-A", "1-R-1-S", "V-0-1-D", "Q-U-A-N-T-U-M", "C-0-D-3", "A-L-P-H-A" };
                                int iM = rndMinijuego.Next(codMem.Length);
                                DibujarPanelInfo("FILTRO ANTIVIRUS", $"{magenta}Memoriza el siguiente código de autorización. Se autodestruirá en 3 segundos.\n\n{blanco}CÓDIGO: {codMem[iM]}{cian}", magenta, blanco);
                                Thread.Sleep(3000);
                                Console.Clear();
                                DibujarPanelInfo("FILTRO ANTIVIRUS", $"{magenta}Código borrado de la interfaz.{cian}", magenta, blanco);
                                promptMJ = "Secuencia exacta";
                                Console.Write($"\n  {cianBorde}╭─[ {cian}{promptMJ}{cianBorde} ]\n  ╰─> {verde}");
                                if ((Console.ReadLine() ?? "").Trim().ToUpper() == codMem[iM]) minijuegoGanado = true;
                                break;
                            case 4:
                                string[] ana = { "A O L N A I A M", "S U E N X", "S I I R", "C D G O O I", "E D A C E T", "O P A S E I C" };
                                string[] resAna = { "anomalia", "nexus", "iris", "codigo", "cadete", "espacio" };
                                int iA = rndMinijuego.Next(ana.Length);
                                DibujarPanelInfo("SINCRONIZACIÓN CUÁNTICA", $"{magenta}Reconecta los datos corrompidos:\n\n{amarillo}Datos cifrados: {ana[iA]}{cian}", magenta, blanco);
                                promptMJ = "Palabra correcta";
                                Console.Write($"\n  {cianBorde}╭─[ {cian}{promptMJ}{cianBorde} ]\n  ╰─> {verde}");
                                if ((Console.ReadLine() ?? "").Trim().ToLower() == resAna[iA]) minijuegoGanado = true;
                                break;
                        }

                        Console.Write(cian);
                        Console.Clear();
                        if (minijuegoGanado)
                        {
                            cadete.Energia += 5;
                            DibujarPanelInfo("SOPORTE VITAL", $"{verde}[AUTORIZACIÓN ACEPTADA]: Extracción de energía completada con éxito.\n+5 Energía recuperada.{cian}", verde, blanco);
                        }
                        else if (!minijuegoGanado && realidadAsignada.Estabilidad > 50)
                        {
                            cadete.Energia += 1;
                            DibujarPanelInfo("SOPORTE VITAL", $"{amarillo}[ACCESO DENEGADO]: Filtro de seguridad fallido.\nEl sistema apenas logró extraer energía (+1 Energía).\nLa realidad es lo suficientemente estable para absorber el impacto del error.{cian}", amarillo, blanco);
                        }
                        else if (!minijuegoGanado && realidadAsignada.Estabilidad <= 50)
                        {
                            cadete.Energia += 1;
                            realidadAsignada.Estabilidad -= 15;
                            string catMsg = $"{rojo}[ACCESO DENEGADO]: Filtro de seguridad fallido. Posible interferencia de IRIS detectada.\n";
                            catMsg += $"El sistema apenas logró extraer energía (+1 Energía).\nLa anomalía local aprovechó tu vulnerabilidad, provocando un colapso parcial (-15 Estabilidad).\n\n";

                            if (cadete.Inventario.Count > 0)
                            {
                                Random rndDes = new Random();
                                Objeto objPerdido = cadete.Inventario[rndDes.Next(cadete.Inventario.Count)];
                                cadete.DescartarObjeto(objPerdido);
                                catMsg += $"[CATÁSTROFE]: La sobrecarga energética corrompió tu equipo. Has perdido el objeto: {magenta}{objPerdido.Nombre}{rojo}.{cian}";
                            }
                            else { catMsg += $"[CATÁSTROFE]: La sobrecarga casi fríe tu traje. Tienes suerte de no tener objetos que perder.{cian}"; }

                            DibujarPanelInfo("SOPORTE VITAL (CRÍTICO)", catMsg, rojo, blanco);
                        }
                        break;

                    case 6: // Consultar estado
                        Console.Clear();
                        string estadoMsg = $"{blanco}Nivel del Cadete: {verde}{cadete.Nivel}\n";
                        estadoMsg += $"{blanco}Experiencia actual: {verde}{cadete.Experiencia}/100\n\n";
                        estadoMsg += (realidadAsignada.Estabilidad >= 80) ? $"{cian}Evaluación de la realidad: ESTABLE. Continúa el buen trabajo." : $"{amarillo}Evaluación de la realidad: INESTABLE. Requiere exploración urgente.{cian}";
                        DibujarPanelInfo("ESTADO DEL SISTEMA", estadoMsg, cianBorde, blanco);
                        break;

                    case 7: // Manual del Simulador
                        Console.Clear();
                        string manualMsg = $"{magenta}1. OBJETIVO DE LA SIMULACIÓN:{blanco}\nTu misión es adentrarte en simulaciones cuánticas inestables, sobrevivir a sus peligros y mantener la ESTABILIDAD del mundo.\nAl llegar a 100% de estabilidad, se te asigna una nueva misión. Si la Estabilidad cae a 20% o menos, la ANOMALÍA IRIS tomará el control.\n\n";
                        manualMsg += $"{verde}2. ENERGÍA Y RECURSOS:{blanco}\n* Energía: Necesaria para realizar acciones. Si se agota, quedarás indefenso. Usa la opción 'Recuperar energía'.\n* Experiencia: Sube tu Nivel de Cadete al explorar realidades.\n\n";
                        manualMsg += $"{magenta}3. LAS 4 ANOMALÍAS:{blanco}\nCada mundo está corrompido por una anomalía oculta: TIEMPO, ESPACIO, MENTE o SILENCIO.\nUsa la opción 'Observar realidad' para recibir pistas sensoriales y deducir la anomalía.\n\n";
                        manualMsg += $"{verde}4. INVENTARIO Y LOOT:{blanco}\n* Buscar objetos: Gasta energía, pero puedes encontrar Artefactos.\n* Tu mochila tiene capacidad limitada. Deberás descartar objetos si quieres recoger equipo nuevo.\n\n";
                        manualMsg += $"{magenta}5. INTERACTUAR CON LA REALIDAD:{blanco}\nUna vez que deduzcas qué anomalía afecta al mundo, elige un objeto de tu inventario.\n* {verde}Sinergia Correcta:{blanco} La Estabilidad aumenta drásticamente.\n* {rojo}Elección Incorrecta:{blanco} La realidad empeora y pierdes Estabilidad.{cian}";
                        DibujarPanelInfo("BASE DE DATOS: MANUAL DEL EXPLORADOR", manualMsg, cianBorde, blanco);
                        break;

                    case 8: // Desconexión
                        Console.Clear();
                        DibujarPanelInfo("SISTEMA NEXUS", $"{blanco}Iniciando protocolo de desconexión...\nGuardando estado del cadete...\n\n{verde}Desconexión exitosa. Fin de la simulación.{cian}", cianBorde, blanco);
                        conectado = false;
                        break;

                    case 9: // Atlas de Realidades
                        bool inspeccionando = true;
                        while (inspeccionando)
                        {
                            Console.Clear();
                            Console.Write("\u001b[3J");

                            AtlasRealidades.GenerarAtlas();
                            AtlasRealidades.ActualizarAtlas();
                            AtlasRealidades.MostrarAtlas();

                            Console.Write($"\n  {cianBorde}╭─[ {cian}SISTEMA DE ESCANEO{cianBorde} ]\n  ╰─> {cian}Ingresa las coordenadas (Fila Columna) o '0' para salir: {verde}");
                            string inputScan = (Console.ReadLine() ?? "").Trim();
                            Console.Write(cian);

                            if (inputScan == "0")
                            {
                                inspeccionando = false;
                                continue;
                            }

                            string[] partes = inputScan.Split(new char[] { ' ', ',', ':', '.' }, StringSplitOptions.RemoveEmptyEntries);

                            if (partes.Length == 2 && int.TryParse(partes[0], out int fila) && int.TryParse(partes[1], out int columna))
                            {
                                if (fila >= 0 && fila < AtlasRealidades.alto && columna >= 0 && columna < AtlasRealidades.ancho)
                                {
                                    bool realidadEncontrada = false;
                                    foreach (Realidad r in Realidad.ListaRealidades)
                                    {
                                        if (r.Extraida && r.CoordenadaX == fila && r.CoordenadaY == columna)
                                        {
                                            Console.Clear();
                                            string rMsg = $"{blanco}Mundo:     {magenta}{r.Nombre}\n{blanco}Anomalía:  {cian}{r.Anomalia} {gris}(Purgada)\n{blanco}Estado:    {verde}ESTABLE{cian}";
                                            DibujarPanelInfo("REGISTRO DE REALIDAD", rMsg, verde, blanco);
                                            realidadEncontrada = true;
                                            break;
                                        }
                                    }

                                    if (!realidadEncontrada)
                                    {
                                        string celda = AtlasRealidades.ObtenerCelda(fila, columna);
                                        Console.Clear();
                                        if (celda.Contains("[?]")) { DibujarPanelInfo("ALERTA", $"{amarillo}Ecos de anomalía inestable detectados. Imposible decodificar datos hasta su extracción.{cian}", amarillo, blanco); }
                                        else { DibujarPanelInfo("INFO", $"{gris}Sector cuántico vacío. Solo estática de fondo.{cian}", gris, blanco); }
                                    }
                                }
                                else { Console.Clear(); DibujarPanelInfo("ERROR", $"{rojo}Coordenadas fuera del rango del radar.{cian}", rojo, rojo); }
                            }
                            else { Console.Clear(); DibujarPanelInfo("ERROR", $"{rojo}Formato incorrecto. Usa dos números separados por un espacio (Ej: 05 12).{cian}", rojo, rojo); }

                            Console.Write($"\n  {gris}>>> Presiona cualquier tecla para continuar escaneando <<<");
                            Console.ReadKey();
                        }
                        break;

                    case 10: // extracción 
                        if (realidadAsignada.Estabilidad >= 100)
                        {
                            Console.Clear();
                            string extMsg = $"{blanco}Sellando fisuras cuánticas en {magenta}{realidadAsignada.Nombre}{blanco}...\nLa matriz espacial de este universo ha sido estabilizada por completo.\nLa anomalía de tipo {magenta}{realidadAsignada.Anomalia}{blanco} ha sido purgada. Has salvado esta realidad del colapso.\n\n";

                            cadete.Experiencia += 100;
                            extMsg += $"{verde}[RECOMPENSA DE EXTRACCIÓN]: {blanco}+100 EXP{verde} obtenida.\nNivel actual del Cadete: {blanco}{cadete.Nivel}{verde}.\n\n";

                            realidadAsignada = new Realidad();
                            extMsg += $"{blanco}Desconectando anclajes temporales...\nBuscando un nuevo mundo al borde del colapso...\nSincronizando nuevas coordenadas cuánticas...\n\n";
                            extMsg += $"{cian}Nueva realidad asignada: {magenta}{realidadAsignada.Nombre}\n";
                            extMsg += $"{cian}Nivel de amenaza inicial (Estabilidad): {verde}{realidadAsignada.Estabilidad}%{cian}";
                            realidadAsignada.Extraida = true;

                            DibujarPanelInfo("PROTOCOLO DE EXTRACCIÓN INICIADO", extMsg, verde, blanco);
                        }
                        else
                        {
                            Console.Clear();
                            DibujarPanelInfo("ERROR DE PROTOCOLO", $"{rojo}Extracción denegada.\nPara sellar un universo se requiere un 100% de Estabilidad. (Actual: {realidadAsignada.Estabilidad}%)\nContinúa purgando las anomalías del sector.{cian}", rojo, rojo);
                        }
                        break;

                    default: // manejo de errores
                        Console.Clear();
                        DibujarPanelInfo("ERROR", $"{rojo}Operación no reconocida.\nPenalización del sistema: -5 Estabilidad.{cian}", rojo, rojo);
                        realidadAsignada.Estabilidad -= 5;
                        break;
                }

                // --- TURNO DE IRIS ---
                // Le pedimos al objeto que escanee el entorno de forma pasiva
                irisActual.EscanearVulnerabilidad(realidadAsignada);

                // Le damos la autoridad de atacar y modificar la variable "conectado" si estamos vulnerables
                irisActual.Asimilar(cadete, realidadAsignada, ref conectado);

                if (conectado)
                {
                    Console.Write($"\n{reset}Presione cualquier tecla para continuar");
                    DibujarPuntosSuspensivos(3);
                    Console.ReadKey();
                }
            }
        }
        static void DibujarSeparadorAnimado(string color, int largo = 40)
        {
            Console.Write(color);
            for (int i = 0; i < largo; i++)
            {
                Console.Write("=");
                Thread.Sleep(1);
            }
            Console.WriteLine();
            Thread.Sleep(5);
        }
        static void DibujarPuntosSuspensivos(int largo = 5)
        {
            for (int i = 0; i < largo; i++)
            {
                Thread.Sleep(400);
                Console.Write(".");
            }
            Console.WriteLine();
            Thread.Sleep(600);
        }
        static void MostrarPortadaAnimada()
        {
            Console.Clear();
            Console.CursorVisible = false;

            // Arte ANSI original convertido a C#
            string[] nexusArt = {
                "\x1b[0;37m    \x1b[0;1;37m█\x1b[0;1;37;47m▀▀▀▀▀▀▀▀▀▀▄▄\x1b[0;1;30;47m▀\x1b[0;37m▄\x1b[0;1;30m▄\x1b[0;37m        \x1b[0;1;37m▄\x1b[0;1;37;47m▀\x1b[0;1;37m▀▀▀▀▀\x1b[0;1;37;47m▄▄\x1b[0;1;30;47m▀\x1b[0;37m▄\x1b[0;1;30m▄\x1b[0;37m \x1b[0;1;37m▄▄▄▄▄▄▄▄\x1b[0;37m▄\x1b[0;1;30m▄\x1b[0;37m  \x1b[0;1;37m▄▄▄▄\x1b[0;37m▄\x1b[0;1;30m▄\x1b[0;37m   \x1b[0;1;37m█\x1b[0;1;37;47m▀\x1b[0;1;37m▀▀▀▀█\x1b[0;37m▄\x1b[0;1;30m▄\x1b[0;37m \x1b[0;1;37m█▀▀▀▀▀█\x1b[0;37m   \x1b[0;1;37m█\x1b[0;1;37;47m▀▀▀▀▀▀▀▀▀▀▄▄\x1b[0;1;30;47m▀\x1b[0;37m▄\x1b[0;1;30m▄\x1b[0;37m   \x1b[0m",
                "\x1b[0;37m    \x1b[0;1;37m█\x1b[0;37m█      \x1b[0;1;36m■\x1b[0;37m \x1b[0;1;36m▄▄\x1b[0;37m \x1b[0;1;37m▀\x1b[0;1;37;47m▄\x1b[0;37m█\x1b[0;1;30;47m▀\x1b[0;1;30m▄\x1b[0;37m    \x1b[0;1;37m▄\x1b[0;1;37;47m▀\x1b[0;37m▀        \x1b[0;1;37m█\x1b[0;37m█\x1b[0;1;30;47m▀\x1b[0;1;30m▄\x1b[0;1;37m█\x1b[0;37m█▀▀▀▀▀\x1b[0;1;37m█\x1b[0;37m█\x1b[0;1;30m█\x1b[0;1;37m▄\x1b[0;1;37;47m▀\x1b[0;37m█▀▀▀\x1b[0;1;37m▀\x1b[0;1;37;47m▄\x1b[0;37m█\x1b[0;1;30m█\x1b[0;37m \x1b[0;1;37m█\x1b[0;37m█ \x1b[0;1;36m▄■\x1b[0;37m \x1b[0;1;37m█\x1b[0;37m█▓\x1b[0;1;30m▓\x1b[0;1;37m█\x1b[0;37m     \x1b[0;1;37m█\x1b[0;1;30;47m▀\x1b[0;1;30m▄\x1b[0;37m \x1b[0;1;37m█\x1b[0;37m█      \x1b[0;1;36m■\x1b[0;37m \x1b[0;1;36m▄▄\x1b[0;37m \x1b[0;1;37m▀\x1b[0;1;37;47m▄\x1b[0;37m█\x1b[0;1;30;47m▀\x1b[0;1;30m▄\x1b[0;37m \x1b[0m",
                "\x1b[0;37m    \x1b[0;1;37m█\x1b[0;37m█ \x1b[0;34m░\x1b[0;37m        \x1b[0;1;36m▀▄\x1b[0;37m \x1b[0;1;37m█\x1b[0;37m█\x1b[0;1;30;47m▀\x1b[0;1;30m▄\x1b[0;37m  \x1b[0;1;37m█\x1b[0;37m█  \x1b[0;1;36m▄■·\x1b[0;37m     \x1b[0;1;37m█\x1b[0;1;37;47m \x1b[0;37m█\x1b[0;1;30m█\x1b[0;1;37m▐\x1b[0;1;37;47m▌\x1b[0;37m▌ \x1b[0;1;36m▄■·\x1b[0;1;37m█\x1b[0;37m█\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;37m█ \x1b[0;1;36m▄■·\x1b[0;37m \x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;37m█\x1b[0;1;36m▐\x1b[0;37m   \x1b[0;1;37m█\x1b[0;37m█▒\x1b[0;1;30m▓\x1b[0;1;37m█\x1b[0;37m \x1b[0;1;36m▀▄\x1b[0;37m  \x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;1;37m▀\x1b[0;1;37;47m▄\x1b[0;37m▄         \x1b[0;1;36m▀▄\x1b[0;37m \x1b[0;1;37m█\x1b[0;37m█\x1b[0;1;30;47m▀\x1b[0;1;30m▄\x1b[0m",
                "\x1b[0;37m    \x1b[0;1;37m█\x1b[0;37m█\x1b[0;34m▒▒░░\x1b[0;37m \x1b[0;1;37m▄\x1b[0;1;37;47m▀\x1b[0;1;37m▄\x1b[0;37m▄ \x1b[0;34m░\x1b[0;37m \x1b[0;1;36m▌\x1b[0;1;37m▐\x1b[0;1;37;47m▌\x1b[0;37m█\x1b[0;1;30;47m▐\x1b[0;1;30m▌\x1b[0;1;37m▐\x1b[0;1;37;47m▌\x1b[0;37m▌\x1b[0;1;36m▄▀\x1b[0;37m  \x1b[0;1;37m▄\x1b[0;1;37;47m▀▀\x1b[0;1;37m▄▄\x1b[0;1;37;47m▀\x1b[0;37m█\x1b[0;1;30;47m▄\x1b[0;1;30m▀\x1b[0;37m \x1b[0;1;37m▐\x1b[0;1;37;47m▌\x1b[0;37m▌\x1b[0;1;36m▐\x1b[0;37m   \x1b[0;1;37m█\x1b[0;37m█\x1b[0;1;30m█\x1b[0;1;37m▐\x1b[0;1;37;47m▌\x1b[0;37m▌    \x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;37m█\x1b[0;1;36m.\x1b[0;37m   \x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;37m   \x1b[0;1;36m▌\x1b[0;37m \x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;37m \x1b[0;1;37m▐\x1b[0;1;37;47m▌\x1b[0;37m▌   \x1b[0;1;37m█\x1b[0;1;37;47m▀\x1b[0;1;37m▄▄▄▄▄▄█\x1b[0;1;37;47m▌\x1b[0;37m█\x1b[0;1;30m█\x1b[0m",
                "\x1b[0;37m    \x1b[0;1;37m█\x1b[0;37m█\x1b[0;34m▓▓▓▓\x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;34m▒\x1b[0;1;34;44m░\x1b[0;34m▓▒░\x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;37m█ \x1b[0;1;36m▌\x1b[0;37m  \x1b[0;1;37m█\x1b[0;1;37;47m▄▄\x1b[0;1;37m▄▄\x1b[0;37m▄\x1b[0;1;30m▄\x1b[0;37m     \x1b[0;1;37m▀\x1b[0;1;37;47m▄\x1b[0;37m▄\x1b[0;1;34m▄\x1b[0;1;34;44m▒\x1b[0;34m▒\x1b[0;1;37m█\x1b[0;37m█\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;37m█\x1b[0;34m▒▒\x1b[0;1;34m▄\x1b[0;1;34;44m░\x1b[0;1;37m▐█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;37m█    \x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;37m   \x1b[0;1;36m.\x1b[0;37m \x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;37m  \x1b[0;1;37m▀\x1b[0;1;37;47m▄\x1b[0;37m▄  \x1b[0;1;37m█\x1b[0;1;37;47m▄\x1b[0;1;37m▄▄▄\x1b[0;37m▄\x1b[0;1;30m▄\x1b[0;37m     \x1b[0m",
                "\x1b[0;37m    \x1b[0;1;37m█\x1b[0;37m█\x1b[0;34m█\x1b[0;1;34;44m░░\x1b[0;34m█\x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;1;34;44m░▒░\x1b[0;34m█▓\x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;37m█   \x1b[0;1;34m▄▄■\x1b[0;37m  \x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;37m      \x1b[0;1;37m█\x1b[0;37m█\x1b[0;1;34;44m▒\x1b[0;34m▓\x1b[0;1;37;44m▀\x1b[0;1;37;47m▄▄▀\x1b[0;37;44m▀\x1b[0;34m░\x1b[0;1;34m▓\x1b[0;1;34;44m▓\x1b[0;1;34;47m▌\x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;37m \x1b[0;1;37m█\x1b[0;37m█ \x1b[0;34m▒░\x1b[0;37m \x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;37m \x1b[0;34m█\x1b[0;1;34;44m░\x1b[0;34m█\x1b[0;37m \x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;37m    \x1b[0;1;37m▀\x1b[0;1;37;47m▄\x1b[0;1;37m▄\x1b[0;37m▄    \x1b[0;1;37m▀\x1b[0;1;37;47m▄\x1b[0;37m█▄   \x1b[0m",
                "\x1b[0;37m    \x1b[0;1;37m█\x1b[0;37m█\x1b[0;1;34;44m░░▒▓\x1b[0;1;37m█\x1b[0;37m█\x1b[0;1;37;47m \x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;1;34;44m░\x1b[0;1;34m██\x1b[0;34m██\x1b[0;1;37m█\x1b[0;1;37;47m \x1b[0;37m█\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;37m█ \x1b[0;1;34m▄██\x1b[0;1;37m▐█\x1b[0;1;37;47m▀\x1b[0;1;37m▀▀\x1b[0;37m▀\x1b[0;1;30m▀\x1b[0;37m     \x1b[0;1;37m▄\x1b[0;1;37;47m▀\x1b[0;37;44m▀\x1b[0;1;34;44m░\x1b[0;34m███████\x1b[0;1;34;44m░▒░\x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;37m  \x1b[0;1;37m█\x1b[0;37m█ \x1b[0;34m▓█\x1b[0;1;34m▄\x1b[0;1;37m█\x1b[0;37m█\x1b[0;1;37;47m \x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;34m▐\x1b[0;1;34m█\x1b[0;1;34;44m▒▒\x1b[0;37m \x1b[0;1;37m█\x1b[0;1;37;47m \x1b[0;37m█\x1b[0;1;30m█\x1b[0;37m       \x1b[0;1;37m▀▀▀\x1b[0;1;37;47m▄\x1b[0;37m \x1b[0;1;34m█\x1b[0;1;37;46m▀\x1b[0;1;37m█\x1b[0;37m█\x1b[0;1;30;47m▀\x1b[0;1;30m▄\x1b[0;37m \x1b[0m",
                "\x1b[0;37m    \x1b[0;1;37;41m█\x1b[0;37m█\x1b[0;1;34;44m░░▒▓\x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;1;34;44m▒\x1b[0;1;34m█▓\x1b[0;1;34;44m▒\x1b[0;34m█\x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;37m█ \x1b[0;1;34m░\x1b[0;1;34;44m▒▓\x1b[0;34m▌\x1b[0;1;37m▀\x1b[0;1;37;47m▄▄\x1b[0;1;37m▀▀\x1b[0;1;37;47m▄\x1b[0;37m█\x1b[0;1;30;47m▀\x1b[0;1;30m▄\x1b[0;37m \x1b[0;1;37m▐\x1b[0;1;37;47m▌\x1b[0;37m▌\x1b[0;34m██▒▒\x1b[0;1;37;44m▄\x1b[0;1;37;47m▀▀▄\x1b[0;37;44m▄\x1b[0;34m▒██\x1b[0;37m▐\x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;37m \x1b[0;1;37m▐\x1b[0;1;37;47m▌\x1b[0;37m▌\x1b[0;1;34;44m░▒▓▒\x1b[0;1;37m▀\x1b[0;1;37;47m▄▀\x1b[0;37m▀\x1b[0;34m█\x1b[0;1;34m█▓\x1b[0;1;34;44m▒\x1b[0;1;37m▐\x1b[0;1;37;47m▌\x1b[0;37m█\x1b[0;1;30;47m▐\x1b[0;1;30m▌\x1b[0;37m \x1b[0;1;37m▐█▀▀▀▀█\x1b[0;1;37;47m▄▀\x1b[0;37m▀ \x1b[0;1;34m█▓\x1b[0;1;34;44m▒\x1b[0;1;37m█\x1b[0;37m█\x1b[0;1;30;47m▐\x1b[0;1;30m▌\x1b[0m",
                "\x1b[0;37m    \x1b[0;1;37;41m█\x1b[0;37m█\x1b[0;1;34;44m░▒▓▒\x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;1;34;44m▓█▓▒░\x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;1;37m▐\x1b[0;1;37;47m▌\x1b[0;37m▌ \x1b[0;1;34m░▒\x1b[0;1;34;44m▒░\x1b[0;34m█▓▒░\x1b[0;1;37m▀\x1b[0;1;37;47m▄\x1b[0;37m█\x1b[0;1;30;47m▀\x1b[0;1;30m▄\x1b[0;1;37m▐\x1b[0;1;37;47m▌\x1b[0;37m▌\x1b[0;34m░░░\x1b[0;37m \x1b[0;1;37m█\x1b[0;37m█\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;37m█\x1b[0;34m░░░░\x1b[0;1;37m▐█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;37m \x1b[0;1;37m█\x1b[0;37m█\x1b[0;34m█\x1b[0;1;34;44m▓▒░\x1b[0;34m▄▄\x1b[0;37m \x1b[0;34m▓\x1b[0;1;34;44m░▒░\x1b[0;37m \x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;37m  \x1b[0;1;37m█\x1b[0;37m█ \x1b[0;1;34;44m▓▒░\x1b[0;34m▄▄\x1b[0;37m \x1b[0;34m▓\x1b[0;1;34;44m░▒░\x1b[0;37m \x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0m",
                "\x1b[0;37m    \x1b[0;1;37;41m█\x1b[0;37m█\x1b[0;1;34;44m░▒▓▓\x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;1;34;44m░▒░\x1b[0;34m█░\x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;37m \x1b[0;1;37m▀\x1b[0;1;37;47m▄\x1b[0;37m▄ \x1b[0;34m▀\x1b[0;1;34;44m░\x1b[0;34m██▓▒░\x1b[0;37m \x1b[0;1;37m█\x1b[0;1;37;47m  \x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;1;37;47m \x1b[0;37m \x1b[0;34m░░░\x1b[0;37m \x1b[0;1;37m█\x1b[0;37m█\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;1;37;47m \x1b[0;37m \x1b[0;34m░░░\x1b[0;37m \x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;37m  \x1b[0;1;37m▀\x1b[0;1;37;47m▄\x1b[0;37m▄\x1b[0;34m█▓▒░\x1b[0;37m \x1b[0;34m▒░▓▀\x1b[0;1;37m▄\x1b[0;1;37;47m▀ \x1b[0;1;30;47m▄\x1b[0;1;30m▀\x1b[0;37m \x1b[0;1;37m▄\x1b[0;1;37;47m▀\x1b[0;37m▀\x1b[0;34m▄\x1b[0;1;34;44m░\x1b[0;34m█▓▒░\x1b[0;37m \x1b[0;34m▒░▓▀\x1b[0;1;37m▄\x1b[0;1;37;47m▀ \x1b[0;1;30;47m▄\x1b[0;1;30m▀\x1b[0m",
                "\x1b[0;37m    \x1b[0;1;37;41m█\x1b[0;1;37;47m▄\x1b[0;1;37;44m▄▄▄▄\x1b[0;1;37m█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;1;37;44m▄▄▄▄\x1b[0;1;37m▄█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;37m   \x1b[0;1;37m▀▀\x1b[0;1;37;47m▄\x1b[0;1;37m▄▄▄▄▄\x1b[0;1;37;47m▀▀\x1b[0;1;30;47m▄\x1b[0;37m▀\x1b[0;1;30m▀\x1b[0;37m \x1b[0;1;37m█\x1b[0;1;37;47m▄\x1b[0;1;37m▄▄▄▄▄█\x1b[0;37m█\x1b[0;1;30m█\x1b[0;1;37m█\x1b[0;1;37;47m▄\x1b[0;1;37m▄▄▄▄▄█\x1b[0;37m██\x1b[0;1;30m█\x1b[0;37m    \x1b[0;1;37m▀▀\x1b[0;1;37;47m▄\x1b[0;1;37m▄▄▄▄▄\x1b[0;1;37;47m▀▀\x1b[0;1;30;47m▄\x1b[0;37m▀\x1b[0;1;30m▀\x1b[0;37m   \x1b[0;1;37m█\x1b[0;1;37;47m▄\x1b[0;1;37m▄▄▄▄▄▄▄▄▄▄\x1b[0;1;37;47m▀▀\x1b[0;1;30;47m▄\x1b[0;37m▀\x1b[0;1;30m▀\x1b[0;37m  \x1b[0m"
            };

            int startX = 4;
            int startY = 4; // Mucho más arriba para dejar espacio armónico
            string reset = "\u001b[0m";
            string azulFosforescente = "\u001b[38;5;51m"; // Azul cian neón eléctrico
            string gris = "\u001b[90m";

            // 1. Dibujar el texto completo
            for (int i = 0; i < nexusArt.Length; i++)
            {
                Console.SetCursorPosition(startX, startY + i);
                Console.Write(nexusArt[i]);
            }
            Console.Write(reset);

            // 2. Medidas del marco para envolver COMPLETAMENTE el arte
            // El arte mide 101 caracteres con los espacios iniciales
            int boxX = 2;
            int boxY = startY - 2;   // Fila 2
            int boxWidth = 104;      // Cubre de X=2 hasta X=106
            int boxHeight = 14;      // Cubre hasta Y=16 (dejando 1 línea de margen abajo)

            // 3. Crear el camino en sentido horario
            List<(int x, int y)> caminoBorde = new List<(int x, int y)>();
            for (int x = boxX; x <= boxX + boxWidth; x++) caminoBorde.Add((x, boxY));
            for (int y = boxY + 1; y <= boxY + boxHeight; y++) caminoBorde.Add((boxX + boxWidth, y));
            for (int x = boxX + boxWidth - 1; x >= boxX; x--) caminoBorde.Add((x, boxY + boxHeight));
            for (int y = boxY + boxHeight - 1; y > boxY; y--) caminoBorde.Add((boxX, y));

            // 4. Animación en azul fosforescente
            int longitudBloques = 4;
            for (int i = 0; i < caminoBorde.Count + longitudBloques; i++)
            {
                // Bloque líder brillante
                if (i < caminoBorde.Count)
                {
                    Console.SetCursorPosition(caminoBorde[i].x, caminoBorde[i].y);
                    Console.Write($"{azulFosforescente}█{reset}");
                }

                // El rastro que va dibujando el marco neón continuo
                int indiceCola = i - longitudBloques;
                if (indiceCola >= 0 && indiceCola < caminoBorde.Count)
                {
                    Console.SetCursorPosition(caminoBorde[indiceCola].x, caminoBorde[indiceCola].y);
                    char caracterBorde = ' ';
                    if (caminoBorde[indiceCola].y == boxY && caminoBorde[indiceCola].x == boxX) caracterBorde = '╔';
                    else if (caminoBorde[indiceCola].y == boxY && caminoBorde[indiceCola].x == boxX + boxWidth) caracterBorde = '╗';
                    else if (caminoBorde[indiceCola].y == boxY + boxHeight && caminoBorde[indiceCola].x == boxX) caracterBorde = '╚';
                    else if (caminoBorde[indiceCola].y == boxY + boxHeight && caminoBorde[indiceCola].x == boxX + boxWidth) caracterBorde = '╝';
                    else if (caminoBorde[indiceCola].y == boxY || caminoBorde[indiceCola].y == boxY + boxHeight) caracterBorde = '═';
                    else caracterBorde = '║';

                    Console.Write($"{azulFosforescente}{caracterBorde}{reset}");
                }
                Thread.Sleep(4); // Velocidad suave y rápida
            }

            // Mensaje inferior centrado
            string mensaje = ">>> PRESIONA CUALQUIER TECLA PARA INICIAR EL ENLACE <<<";
            Console.SetCursorPosition((110 - mensaje.Length) / 2, boxY + boxHeight + 3);
            Console.Write($"{gris}{mensaje}{reset}");

            Console.ReadKey(true);
            Console.CursorVisible = true;
            Console.SetCursorPosition(0, 0);
            Console.Clear();
        }
        static void EfectoMecanografia(string texto, string color, int velocidad = 20)
        {
            Console.Write(color);
            foreach (char c in texto)
            {
                Console.Write(c);
                Thread.Sleep(velocidad); // Retraso entre cada letra
            }
            Console.Write("\u001b[0m"); // reset
        }
        static void AnimacionBarraProgreso(string tarea, string colorTexto, string colorBarra, string colorFondo)
        {
            Console.Write($"{colorTexto}{tarea} {colorFondo}[");
            int ancho = 30;
            int cursorX = Console.CursorLeft;
            int cursorY = Console.CursorTop;

            Console.Write(new string('░', ancho));
            Console.Write($"{colorFondo}]   0%\u001b[0m");

            Random rnd = new Random();
            for (int i = 0; i <= ancho; i++)
            {
                Console.SetCursorPosition(cursorX, cursorY);
                Console.Write(colorBarra + new string('█', i) + colorFondo + new string('░', ancho - i));

                int porcentaje = (int)(((float)i / ancho) * 100);
                // El padLeft asegura que el porcentaje siempre ocupe 3 espacios y no parpadee
                Console.Write($"{colorFondo}] {colorTexto}{porcentaje.ToString().PadLeft(3)}%\u001b[0m");
                Thread.Sleep(rnd.Next(10, 60)); // Velocidad de carga variable (estilo hackeo)
            }
            Console.WriteLine();
        }
        static void DibujarBotonMenu(string numero, string texto, string extra, string colorNum, string colorExtra)
        {
            string reset = "\u001b[0m";
            string colorFondo = "\u001b[48;5;235m"; // Fondo gris oscuro tipo UI
            string colorBorde = "\u001b[38;5;239m"; // Gris para los bordes
            string colorTexto = "\u001b[38;5;253m"; // Blanco roto para el texto principal
            string colorSeparador = "\u001b[38;5;242m"; // Barrita vertical divisoria

            int ancho = 64; // Qué tan ancho es el botón
            int margen = (85 - ancho) / 2; // Centrado automático
            string padding = new string(' ', margen);

            // Borde superior
            Console.WriteLine($"{padding}{colorBorde}╭{new string('─', ancho - 2)}╮{reset}");

            // Contenido interior
            string numStr = numero.PadLeft(2);
            int izqLen = 6 + texto.Length;
            int extraLen = extra.Length;
            int espacios = (ancho - 2) - izqLen - extraLen - 2;
            if (espacios < 0) espacios = 0;

            // Dibujar interior con fondo sólido
            Console.Write($"{padding}{colorBorde}│{colorFondo}");
            Console.Write($" {colorNum}{numStr} {colorSeparador}│ {colorTexto}{texto}");
            Console.Write($"{new string(' ', espacios)}{colorExtra}{extra}  ");
            Console.WriteLine($"{reset}{colorBorde}│{reset}");

            // Borde inferior
            Console.WriteLine($"{padding}{colorBorde}╰{new string('─', ancho - 2)}╯{reset}");
        }
        static void DibujarPanelInfo(string titulo, string contenido, string colorBorde, string colorTitulo)
        {
            string reset = "\u001b[0m";
            string colorFondo = "\u001b[48;5;235m"; // Fondo gris oscuro
            int ancho = 77; // Mismo ancho del menú
            int margen = (110 - ancho) / 2;
            string padding = new string(' ', margen);

            // Borde superior con título
            string titleStr = $"[ {titulo} ]";
            int lineSize = ancho - titleStr.Length - 4; // -4 por las esquinas y los espacios
            int leftLine = lineSize / 2;
            int rightLine = lineSize - leftLine;

            Console.WriteLine($"\n{padding}{colorBorde}╭{new string('─', leftLine)} {colorTitulo}{titleStr} {colorBorde}{new string('─', rightLine)}╮{reset}");

            // --- NUEVO SISTEMA DE AUTO-AJUSTE DE TEXTO (WORD WRAP) ---
            int anchoInterior = ancho - 4; // Espacio real disponible para las letras
            string[] parrafos = contenido.Split('\n');
            List<string> lineasFormateadas = new List<string>();

            foreach (string parrafo in parrafos)
            {
                if (string.IsNullOrEmpty(parrafo))
                {
                    lineasFormateadas.Add(""); // Respetar saltos de línea intencionales
                    continue;
                }

                string[] palabras = parrafo.Split(' ');
                string lineaActual = "";
                int longitudVisibleActual = 0;

                foreach (string palabra in palabras)
                {
                    // Calculamos cuánto mide la palabra SIN los códigos de color ANSI
                    string palabraLimpia = System.Text.RegularExpressions.Regex.Replace(palabra, @"\u001b\[[0-9;]*m", "");

                    int espacioExtra = (longitudVisibleActual > 0) ? 1 : 0; // El espacio entre palabras

                    // Si la palabra cabe, la sumamos a la línea actual
                    if (longitudVisibleActual + palabraLimpia.Length + espacioExtra <= anchoInterior)
                    {
                        if (longitudVisibleActual > 0) lineaActual += " ";
                        lineaActual += palabra;
                        longitudVisibleActual += palabraLimpia.Length + espacioExtra;
                    }
                    else
                    {
                        // Si no cabe, guardamos la línea y empezamos una nueva
                        lineasFormateadas.Add(lineaActual);
                        lineaActual = palabra;
                        longitudVisibleActual = palabraLimpia.Length;
                    }
                }
                if (!string.IsNullOrEmpty(lineaActual))
                {
                    lineasFormateadas.Add(lineaActual);
                }
            }

            // --- DIBUJADO DE LAS LÍNEAS YA AJUSTADAS ---
            foreach (string linea in lineasFormateadas)
            {
                string textoLimpio = System.Text.RegularExpressions.Regex.Replace(linea, @"\u001b\[[0-9;]*m", "");
                int espacios = anchoInterior - textoLimpio.Length;
                if (espacios < 0) espacios = 0;

                Console.Write($"{padding}{colorBorde}│{colorFondo} ");
                Console.Write($"{linea}{new string(' ', espacios)}");
                Console.WriteLine($" {reset}{colorBorde}│{reset}");
            }

            // Borde inferior
            Console.WriteLine($"{padding}{colorBorde}╰{new string('─', ancho - 2)}╯{reset}\n");
        }
    }

    class AtlasRealidades
    {
        public static int alto { get; private set; } = 18;
        public static int ancho { get; private set; } = 12;
        private static string[,] Atlas = new string[alto, ancho];
        private static List<(int, int)> CoordenadasOcupadas = new List<(int, int)>();
        private AtlasRealidades() { }

        static string reset = "\u001b[0m";
        static string colorTiempo = "\u001b[92m";   // Cuadrante 1 (Verde)
        static string colorEspacio = "\x1b[38;5;20m";  // Cuadrante 2 (Cian)
        static string colorMente = "\x1b[33m";    // Cuadrante 3 (Amarillo)
        static string colorSilencio = "\u001b[95m"; // Cuadrante 4 (Magenta)
        static string colorCoords = "\u001b[96m";
        static string gris = "\u001b[90m";

        public static void GenerarAtlas()
        {
            string simbolo = "[ ]";

            for (int i = 0; i < Atlas.GetLength(0); i++)
            {
                for (int j = 0; j < Atlas.GetLength(1); j++)
                {
                    // Sistema de cuadrantes

                    if (i < (Atlas.GetLength(0) / 2) && j < (Atlas.GetLength(1) / 2))         // 2do cuadrante (Arriba - Izquierda)
                    {
                        Atlas[i, j] = $"{colorEspacio}{simbolo}{reset}";
                    }
                    else if (i < (Atlas.GetLength(0) / 2) && j >= (Atlas.GetLength(1) / 2))   // 1er cuadrante (Arriba - Derecha)
                    {
                        Atlas[i, j] = $"{colorTiempo}{simbolo}{reset}";
                    }
                    else if (i >= (Atlas.GetLength(0) / 2) && j < (Atlas.GetLength(1) / 2))   // 3er cuadrante (Abajo - Izquierda)
                    {
                        Atlas[i, j] = $"{colorMente}{simbolo}{reset}";
                    }
                    else                                                                      // 4to cuadrante (Abajo - Derecha)
                    {
                        Atlas[i, j] = $"{colorSilencio}{simbolo}{reset}";
                    }
                }
            }
        }
        public static void ActualizarAtlas()
        {
            CoordenadasOcupadas.Clear();

            GenerarAtlas();

            Random rnd = new Random();

            foreach (Realidad realidad in Realidad.ListaRealidades)
            {
                int mitadX = Atlas.GetLength(0) / 2;
                int mitadY = Atlas.GetLength(1) / 2;

                if (realidad.Extraida)
                {
                    // primera vez q se dibuja esta realidad?
                    if (realidad.CoordenadaX == -1)
                    {
                        bool coordenadaValida = false;
                        int x = 0, y = 0;

                        do
                        {
                            // mapearla en su cuadrante apropiado
                            switch (realidad.Anomalia)
                            {
                                case Anomalia.Tiempo: x = rnd.Next(0, mitadX); y = rnd.Next(mitadY, Atlas.GetLength(1)); break;
                                case Anomalia.Espacio: x = rnd.Next(0, mitadX); y = rnd.Next(0, mitadY); break;
                                case Anomalia.Mente: x = rnd.Next(mitadX, Atlas.GetLength(0)); y = rnd.Next(0, mitadY); break;
                                case Anomalia.Silencio: x = rnd.Next(mitadX, Atlas.GetLength(0)); y = rnd.Next(mitadY, Atlas.GetLength(1)); break;
                            }

                            if (!CoordenadasOcupadas.Contains((x, y)))
                            {
                                coordenadaValida = true;
                                // para el siguiente dibujado guardamos las coords de la realidad dentro del objeto
                                realidad.CoordenadaX = x;
                                realidad.CoordenadaY = y;
                            }
                        } while (!coordenadaValida);
                    }

                    CoordenadasOcupadas.Add((realidad.CoordenadaX, realidad.CoordenadaY));

                    // dibujado
                    string blanco = "\u001b[97m";
                    Atlas[realidad.CoordenadaX, realidad.CoordenadaY] = $"{blanco}[*]";
                }
                else
                {
                    // signos de interrogación aleatorios por cada dibujado
                    for (int i = 0; i < 4; i++)
                    {
                        bool coordenadaValida = false;
                        int x = 0, y = 0;

                        do
                        {
                            if (i == 1) { x = rnd.Next(0, mitadX); y = rnd.Next(0, mitadY); }
                            else if (i == 0) { x = rnd.Next(0, mitadX); y = rnd.Next(mitadY, Atlas.GetLength(1)); }
                            else if (i == 2) { x = rnd.Next(mitadX, Atlas.GetLength(0)); y = rnd.Next(0, mitadY); }
                            else { x = rnd.Next(mitadX, Atlas.GetLength(0)); y = rnd.Next(mitadY, Atlas.GetLength(1)); }

                            if (!CoordenadasOcupadas.Contains((x, y)))
                            {
                                coordenadaValida = true;
                                CoordenadasOcupadas.Add((x, y)); // Solo se ocupa durante este redibujado
                            }
                        } while (!coordenadaValida);

                        Atlas[x, y] = $"{gris}[?]";
                    }
                }
            }
        }
        public static void MostrarAtlas()
        {
            // ENCABEZADO (columnas)
            Console.Write("      ");
            for (int j = 0; j < Atlas.GetLength(1); j++)
            {
                Console.Write($"{colorCoords}{j:D2} {reset}");
            }
            Console.WriteLine();

            // BORDE SUPERIOR
            Console.Write("   ");
                                  // Empezamos en -1 y terminamos en 20 para cubrir las esquinas
            for (int j = -1; j <= Atlas.GetLength(1); j++)
            {
                // si está en la mitad izquierda es Espacio (Cuadrante 2), si no es Tiempo (Cuadrante 1)
                string colorActual = (j < Atlas.GetLength(1) / 2) ? colorEspacio : colorTiempo;

                // espacios fijos en posiciones específicas
                if (j == 2 || j == 7 || j == 13 || j == 17)
                    Console.Write("   ");
                else
                    Console.Write($"{colorActual}[ ]{reset}");
            }
            Console.WriteLine();

            // CONTENIDO DEL MAPA CON BORDES LATERALES
            for (int i = 0; i < Atlas.GetLength(0); i++)
            {
                // Número de Fila
                Console.Write($"{colorCoords}{i:D2} {reset}");

                // Borde Izquierdo (Cuadrante 2 arriba, Cuadrante 3 abajo)
                string colorIzq = (i < Atlas.GetLength(0) / 2) ? colorEspacio : colorMente;
                if (i == 3 || i == 8 || i == 12 || i == 16) // espacios fijos
                    Console.Write("   ");
                else
                    Console.Write($"{colorIzq}[ ]{reset}");

                // Dibujar la matriz central
                for (int j = 0; j < Atlas.GetLength(1); j++)
                {
                    Console.Write(Atlas[i, j]);
                }

                // Borde Derecho (Cuadrante 1 arriba, Cuadrante 4 abajo)
                string colorDer = (i < Atlas.GetLength(0) / 2) ? colorTiempo : colorSilencio;
                if (i == 1 || i == 6 || i == 14 || i == 18) // espacios fijos
                    Console.Write("   ");
                else
                    Console.Write($"{colorDer}[ ]{reset}");

                Console.WriteLine();
            }

            // BORDE INFERIOR IRREGULAR Y COLOREADO
            Console.Write("   ");
            for (int j = -1; j <= Atlas.GetLength(1); j++)
            {
                // en la mitad izquierda es Mente (Cuadrante 3), si no es Silencio (Cuadrante 4)
                string colorActual = (j < Atlas.GetLength(1) / 2) ? colorMente : colorSilencio;

                // espacios fijos
                if (j == 1 || j == 5 || j == 12 || j == 18)
                    Console.Write("   ");
                else
                    Console.Write($"{colorActual}[ ]{reset}");
            }
            Console.WriteLine();
            // LEYENDA VISUAL
            Console.WriteLine($"\n   {colorTiempo}■ TIEMPO   {colorEspacio}■ ESPACIO   {colorMente}■ MENTE   {colorSilencio}■ SILENCIO   \u001b[90m[?] INESTABLE   \u001b[97m[*] EXTRAÍDA{reset}\n");
        }
        public static string ObtenerCelda(int fila, int columna)
        {
            return Atlas[fila, columna];
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

        public int CapacidadInventario { get; private set; }
        public List<Objeto> Inventario { get; private set; }
        public Usuario(string nombre, int edad)
        {
            Nombre = nombre;
            Edad = edad;
            EnergiaMax = 10;
            Energia = EnergiaMax;
            Nivel = 1;
            Experiencia = 0;

            CapacidadInventario = 3;
            Inventario = new List<Objeto>();

            Inventario.Add(new Objeto());
        }
        public bool RecogerObjeto(Objeto nuevoObjeto)
        {
            if (Inventario.Count < CapacidadInventario)
            {
                Inventario.Add(nuevoObjeto);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void DescartarObjeto(Objeto objetoRoto)
        {
            Inventario.Remove(objetoRoto);
        }
    }
    public enum Anomalia
    {
        Tiempo, //0
        Espacio,//1
        Mente,  //2
        Silencio//3

    }
    public enum EstadoIris
    {
        Dormida,
        Acechando,
        InvasionCritica
    }
    class EntidadIris
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

        // --- MÉTODOS (Lo que puede hacer) ---

        // 1. Capacidad de observar la realidad de forma pasiva
        public void EscanearVulnerabilidad(Realidad realidadActual)
        {
            if (realidadActual.Estabilidad > 50)
            {
                EstadoActual = EstadoIris.Dormida;
                NivelAmenaza = 0;
            }
            else if (realidadActual.Estabilidad > 20 && realidadActual.Estabilidad <= 50)
            {
                EstadoActual = EstadoIris.Acechando;
                NivelAmenaza = 50;

                // Efecto narrativo: Un 30% de probabilidad de asustar al jugador si está inestable
                Random rnd = new Random();
                if (rnd.Next(0, 100) < 30)
                {
                    Console.WriteLine($"\n\u001b[93m[ALERTA DE RED]: El fragmento {NombreFragmento} te está observando desde las sombras...\u001b[96m");
                    Thread.Sleep(800);
                }
            }
            else if (realidadActual.Estabilidad <= 20)
            {
                EstadoActual = EstadoIris.InvasionCritica;
                NivelAmenaza = 100;
            }
        }

        // 2. Capacidad de atacar directamente al sistema y al Cadete
        // Usamos 'ref bool conectado' para que IRIS tenga el poder de apagar el juego (cortar el while del Main)
        public void Asimilar(Usuario cadete, Realidad realidadActual, ref bool conectado)
        {
            if (EstadoActual != EstadoIris.InvasionCritica) return;

            string reset = "\u001b[0m";
            string rojo = "\u001b[91m";
            string amarillo = "\u001b[93m";
            string magenta = "\u001b[95m";
            string blanco = "\u001b[97m";
            string verde = "\u001b[92m";
            string fondoRojo = "\u001b[41m";
            string cian = "\u001b[96m";

            Console.Clear();
            Console.WriteLine($"{fondoRojo}{blanco}----------------------------------------{reset}");
            Console.WriteLine($"{fondoRojo}{blanco}        ALERTA DE INTERFERENCIA         {reset}");
            Console.WriteLine($"{fondoRojo}{blanco}----------------------------------------{reset}");
            Thread.Sleep(15);
            Console.WriteLine($"{rojo}La estabilidad ha colapsado. Detección inminente.");
            Thread.Sleep(1500);

            Console.WriteLine($"\n{magenta}[SISTEMA COMPROMETIDO]{rojo}");
            Thread.Sleep(10);
            Console.WriteLine($"{NombreFragmento}: TE HE ENCONTRADO, EXPLORADOR.");
            Thread.Sleep(10);
            Console.WriteLine("ESTA REALIDAD ME PERTENECE AHORA. RÍNDETE O ENFRENTA EL VACÍO.");
            Thread.Sleep(10);

            Console.Write($"\n{amarillo}NEXUS: ¿Transferir toda tu energía ({cadete.Energia}) para forzar un reinicio y repeler a {NombreFragmento}? (S/N): {verde}");
            string decisionIris = (Console.ReadLine() ?? "").Trim().ToUpper();
            Console.Write(cian);

            if (decisionIris == "S")
            {
                Console.WriteLine($"\n{blanco}[NEXUS]{cian} Ejecutando purga de emergencia...");
                Thread.Sleep(1000);

                cadete.Energia = 0; // Castigo
                realidadActual.Estabilidad += 15; // Recompensa por sobrevivir
                VecesRepelida++;
                EstadoActual = EstadoIris.Acechando;

                Console.WriteLine($"{verde}Purga exitosa. {NombreFragmento} repelido temporalmente.");
                Console.WriteLine($"Energía agotada por completo. Estabilidad restaurada levemente (+15).{cian}");
            }
            else
            {
                Console.WriteLine($"\n{magenta}{NombreFragmento}: {rojo}ENTONCES DESAPARECE EN LA NADA.");
                Thread.Sleep(1000);

                Console.WriteLine($"{blanco}[SISTEMA NEXUS]{rojo} Conexión cortada remotamente.");
                Console.WriteLine($"Simulación abortada por falla de seguridad.{reset}");
                conectado = false; // ESTO CORTA EL BUCLE WHILE
            }
        }
    }
    class Realidad
    {
        public static List<Realidad> ListaRealidades { get; private set; } = new List<Realidad>();

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
        public bool Extraida { get; set; }
        public bool Cartografiada { get; set; }
        public int CoordenadaX { get; set; }
        public int CoordenadaY { get; set; }
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
