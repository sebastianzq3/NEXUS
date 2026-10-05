using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            // GESTIÓN DE PERFILES Y GUARDADO (SISTEMA MULTI-SLOT)
            // ----------------------------------------------------
            string directorioSaves = "Saves";
            if (!Directory.Exists(directorioSaves)) Directory.CreateDirectory(directorioSaves);

            Usuario cadete = null;
            Realidad realidadAsignada = null;
            string rutaGuardadoActual = "";

            bool perfilSeleccionado = false;
            while (!perfilSeleccionado)
            {
                Console.Clear();
                string msgInicio = $"{blanco}1. Cargar perfil existente\n2. Registrar nuevo explorador\n3. Purgar registro (Borrar){cian}";
                DibujarPanelInfo("SISTEMA DE ANCLAJE MULTIVERSAL", msgInicio, cianBorde, blanco);
                Console.Write($"  {cianBorde}╭─[ {cian}OPCIÓN{cianBorde} ]\n  ╰─> {verde}");
                string opcInicio = Console.ReadLine() ?? "";

                string[] archivos = Directory.GetFiles(directorioSaves, "*.json");

                if (opcInicio == "1" || opcInicio == "3")
                {
                    if (archivos.Length == 0)
                    {
                        Console.Clear();
                        DibujarPanelInfo("ERROR", $"{amarillo}No hay perfiles registrados en la base de datos.{cian}", amarillo, blanco);
                        Console.Write($"\n  {gris}>>> Presiona una tecla para volver <<<"); Console.ReadKey();
                        continue;
                    }

                    Console.Clear();
                    string listaMsg = $"{blanco}=== REGISTROS DE EXPLORADORES ===\n\n";
                    List<DatosPartida> partidas = new List<DatosPartida>();

                    for (int i = 0; i < archivos.Length; i++)
                    {
                        try
                        {
                            DatosPartida dp = JsonSerializer.Deserialize<DatosPartida>(File.ReadAllText(archivos[i]));
                            partidas.Add(dp);
                            listaMsg += $"{blanco}{i + 1}. {verde}{dp.Jugador.Nombre,-12} {gris}| Edad: {dp.Jugador.Edad,2} | Nivel: {dp.Jugador.Nivel,2}\n";
                        }
                        catch
                        {
                            listaMsg += $"{rojo}{i + 1}. [ARCHIVO CORRUPTO: {Path.GetFileName(archivos[i])}]\n";
                            partidas.Add(null);
                        }
                    }

                    if (opcInicio == "1") // CARGAR
                    {
                        DibujarPanelInfo("SELECCIÓN DE PERFIL", listaMsg.TrimEnd('\n'), cianBorde, blanco);
                        Console.Write($"\n  {cianBorde}╭─[ {cian}SELECCIONA UN NÚMERO (0 para cancelar){cianBorde} ]\n  ╰─> {verde}");
                        if (int.TryParse(Console.ReadLine(), out int sel) && sel > 0 && sel <= archivos.Length)
                        {
                            if (partidas[sel - 1] != null)
                            {
                                cadete = partidas[sel - 1].Jugador;
                                realidadAsignada = partidas[sel - 1].RealidadActual;
                                rutaGuardadoActual = archivos[sel - 1];
                                perfilSeleccionado = true;
                                Console.Clear();
                                DibujarPanelInfo("SISTEMA", $"{verde}Datos recuperados exitosamente.\nBienvenido de vuelta, {cadete.Nombre}.{cian}", verde, blanco);
                            }
                            else
                            {
                                Console.Clear(); DibujarPanelInfo("ERROR", $"{rojo}El archivo seleccionado está dañado.{cian}", rojo, blanco); Console.ReadKey();
                            }
                        }
                    }
                    else // BORRAR
                    {
                        DibujarPanelInfo("PURGA DE DATOS", listaMsg.TrimEnd('\n'), rojo, blanco);
                        Console.Write($"\n  {rojo}╭─[ {blanco}NÚMERO A ELIMINAR (0 para cancelar){rojo} ]\n  ╰─> {blanco}");
                        if (int.TryParse(Console.ReadLine(), out int sel) && sel > 0 && sel <= archivos.Length)
                        {
                            File.Delete(archivos[sel - 1]);
                            Console.Clear();
                            DibujarPanelInfo("PURGA COMPLETADA", $"{verde}El registro ha sido eliminado del multiverso.{cian}", verde, blanco);
                            Console.ReadKey();
                        }
                    }
                }
                else if (opcInicio == "2") // CREAR NUEVO
                {
                    string nombreIngresado = "";
                    while (string.IsNullOrWhiteSpace(nombreIngresado))
                    {
                        Console.Clear();
                        DibujarPanelInfo("VERIFICACIÓN BIOMÉTRICA", $"{blanco}Ingrese su designación (Nombre):{cian}", cianBorde, blanco);
                        Console.Write($"  {cianBorde}╭─[ {cian}ENTRADA{cianBorde} ]\n  ╰─> {verde}");
                        nombreIngresado = Console.ReadLine() ?? "";
                    }

                    int edadIngresada = 0;
                    while (edadIngresada < 18)
                    {
                        Console.Clear();
                        DibujarPanelInfo("VERIFICACIÓN BIOMÉTRICA", $"{blanco}Designación aceptada: {verde}{nombreIngresado}{blanco}\nIngrese su edad cronológica exacta (Mínimo 18):{cian}", cianBorde, blanco);
                        Console.Write($"  {cianBorde}╭─[ {cian}ENTRADA{cianBorde} ]\n  ╰─> {verde}");
                        int.TryParse(Console.ReadLine(), out edadIngresada);
                    }

                    cadete = new Usuario(nombreIngresado, edadIngresada);
                    realidadAsignada = new Realidad();

                    // Asigna un nombre de archivo único basado en el nombre del cadete
                    rutaGuardadoActual = Path.Combine(directorioSaves, $"{cadete.Nombre}.json");

                    Console.Clear();
                    string msgBienvenida = $"{verde}[ ACCESO CONCEDIDO ] Bienvenido, Explorador {cadete.Nombre}.\n{gris}Perfil biométrico verificado: {cadete.Edad} años.\n\n";
                    msgBienvenida += $"{blanco}Sincronizando coordenadas cuánticas...\nAnclaje establecido.\n\n{cian}Decodificando firma de realidad: {magenta}{realidadAsignada.Nombre}{cian}";
                    DibujarPanelInfo("CREACIÓN DE PERFIL", msgBienvenida, verde, blanco);
                    perfilSeleccionado = true;
                }
            }

            Console.Write($"\n  {gris}>>> PRESIONA CUALQUIER TECLA PARA ENTRAR A LA SIMULACIÓN <<<");
            Console.ReadKey(true);

            // Función interna rápida para guardar actualizada
            void GuardarJuego()
            {
                DatosPartida datos = new DatosPartida { Jugador = cadete, RealidadActual = realidadAsignada };
                File.WriteAllText(rutaGuardadoActual, JsonSerializer.Serialize(datos));
            }

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

                // Fila 0: TÍTULO
                Console.WriteLine($"  │ {blanco}NEXUS TRAINING SYSTEM{new string(' ', 55)}{cianBorde}│");
                Console.WriteLine($"  ├{lineaBorde}┤");

                // Fila 1: Explorador y Realidad
                int lenExp = 12 + cadete.Nombre.Length;
                int lenReal = 10 + realidadAsignada.Nombre.Length;
                int espacios1 = anchoInterior - lenExp - lenReal;
                if (espacios1 < 0) espacios1 = 0;

                Console.WriteLine($"  │ {cian}EXPLORADOR: {verde}{cadete.Nombre}{new string(' ', espacios1 - 1)}{cian}REALIDAD: {magenta}{realidadAsignada.Nombre}{cianBorde}│");

                // Fila 2: Energía (Proporcional a 10 bloques visuales)
                string energiaTxt = $"{cadete.Energia}/{cadete.EnergiaMax}";
                Console.Write($"  │ {cian}{"ENERGÍA:",-12}{verde}{energiaTxt,5} {cian}[");

                int bloquesEnergia = (cadete.Energia * 10) / cadete.EnergiaMax;
                for (int i = 0; i < 10; i++) Console.Write(i < bloquesEnergia ? $"{verde}██" : $"{gris}░░");

                int lenEnergiaPre = 12 + 5 + 2;
                int espacios2 = anchoInterior - lenEnergiaPre - 20 - 1; // 20 caracteres fijos de la barra
                if (espacios2 < 0) espacios2 = 0;

                Console.WriteLine($"{cian}]{new string(' ', espacios2 - 1)}{cianBorde}│");

                // Fila 3: Estabilidad (Proporcional a 10 bloques visuales)
                string estabTxt = $"{realidadAsignada.Estabilidad.ToString().PadLeft(3)}%";
                Console.Write($"  │ {cian}ESTABILIDAD:{verde}{estabTxt,5} {cian}[");

                int bloquesEst = realidadAsignada.Estabilidad / 10;
                for (int i = 0; i < 10; i++) Console.Write(i < bloquesEst ? $"{magenta}██" : $"{gris}░░");

                int lenEstabPre = 12 + 5 + 2;
                int espacios3 = anchoInterior - lenEstabPre - 20 - 1;
                if (espacios3 < 0) espacios3 = 0;

                Console.WriteLine($"{cian}]{new string(' ', espacios3 - 1)}{cianBorde}│");

                Console.WriteLine($"  ╰{lineaBorde}╯\n");

                // --- BOTONES DINÁMICOS ANIMADOS ---
                DibujarBotonMenu("1", "Explorar realidad", "", amarilloUI, cian); Thread.Sleep(15);
                DibujarBotonMenu("2", "Inventario y Crafteo", "", amarilloUI, cian); Thread.Sleep(15);
                DibujarBotonMenu("3", "Recuperar energía", "[+5 Recarga]", amarilloUI, "\u001b[38;5;47m"); Thread.Sleep(15);
                DibujarBotonMenu("4", "Tienda de Proficiencia", "[Mejoras]", amarilloUI, magenta); Thread.Sleep(15);
                DibujarBotonMenu("5", "Consultar estado", "", amarilloUI, cian); Thread.Sleep(15);
                DibujarBotonMenu("6", "Manual del simulador", "", amarilloUI, cian); Thread.Sleep(15);
                DibujarBotonMenu("7", "Intentar desconexión", "", amarilloUI, "\u001b[38;5;196m"); Thread.Sleep(15);
                DibujarBotonMenu("8", "Atlas de Realidades", "", amarilloUI, cian); Thread.Sleep(15);

                if (realidadAsignada.Estabilidad >= 100)
                {
                    DibujarBotonMenu("9", "¡INICIAR EXTRACCIÓN!", "[NIVEL ESTABLE]", "\u001b[38;5;11m", "\u001b[38;5;11m"); Thread.Sleep(15);
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
                    case 1: // Explorar realidad
                        Console.Clear();
                        Console.Write("\u001b[3J");

                        realidadAsignada.RellenarEntidadesLocales(cadete.Nivel); // Enemigos escalan con el nivel

                        // GENERADOR DE OBJETIVO EXTRA (Solo dura esta expedición)
                        string[] nombresObj = { "Exterminador (Mata Enemigos)", "Curioso (Visita Eventos '?')", "Chatarrero (Recoge Restos ▤)" };
                        int tipoObj = new Random().Next(3);
                        int metaObj = tipoObj == 1 ? 2 : 3;
                        int progresoObj = 0;
                        bool objCompletado = false;

                        bool explorando = true;
                        int px = realidadAsignada.CadeteStartX;
                        int py = realidadAsignada.CadeteStartY;
                        int contadorPasos = 0;
                        string mensajeLog = $"{verde}Enlace establecido con el sector local.{cian}";

                        while (explorando)
                        {
                            Console.SetCursorPosition(0, 0);

                            // --- 1. CABECERA CON ENERGÍA, VIDA Y OBJETIVO ---
                            int anchoMapaUI = 83;
                            string bordeH = new string('─', anchoMapaUI - 2);
                            Console.WriteLine($"  {cianBorde}╭{bordeH}╮{reset}");

                            // Fila 1: Explorador y Energía (PROPORCIONAL)
                            string expLeft = $"EXPLORADOR: {cadete.Nombre}";
                            string energiaTxtExpl = $"{cadete.Energia}/{cadete.EnergiaMax}";
                            string expRight = $"ENERGÍA: {energiaTxtExpl,5} [";
                            int expBarLen = 20; // 10 bloques * 2 caracteres = 20 fijos

                            int sp1 = 81 - 1 - expLeft.Length - (expRight.Length + expBarLen + 1);
                            if (sp1 < 1) sp1 = 1;

                            Console.Write($"  {cianBorde}│ {cian}EXPLORADOR: {verde}{cadete.Nombre}{new string(' ', sp1)}{cian}ENERGÍA: {verde}{energiaTxtExpl,5} {cian}[");

                            int bloquesEneHUD = (cadete.Energia * 10) / cadete.EnergiaMax;
                            for (int i = 0; i < 10; i++) Console.Write(i < bloquesEneHUD ? $"{verde}██" : $"{gris}░░");
                            Console.WriteLine($"{cian}]{cianBorde}│{reset}");

                            // Fila 2: Objetivo y Vida (PROPORCIONAL)
                            string objName = nombresObj[tipoObj];
                            string objLeft = $"OBJETIVO:   {objName}";
                            string vidaTxtExpl = $"{cadete.Vida}/{cadete.VidaMax}";
                            string vidaRight = $"VIDA:    {vidaTxtExpl,5} [";
                            int vidaBarLen = 20; // 10 bloques * 2 caracteres = 20 fijos

                            int sp2 = 81 - 1 - objLeft.Length - (vidaRight.Length + vidaBarLen + 1);
                            if (sp2 < 1) sp2 = 1;

                            Console.Write($"  {cianBorde}│ {cian}OBJETIVO:   {amarillo}{objName}{new string(' ', sp2)}{cian}VIDA:    {verde}{vidaTxtExpl,5} {cian}[");

                            int bloquesVidaHUD = (cadete.Vida * 10) / cadete.VidaMax;
                            for (int i = 0; i < 10; i++) Console.Write(i < bloquesVidaHUD ? $"\u001b[38;5;196m██" : $"{gris}░░");
                            Console.WriteLine($"{cian}]{cianBorde}│{reset}");

                            // Fila 3: Progreso y Fragmentos
                            string colorObj = objCompletado ? verde : amarillo;
                            string objStr = objCompletado ? "COMPLETADO" : $"{progresoObj}/{metaObj}";
                            string estLeft = $"ESTADO:     {objStr}";
                            string fragRight = $"FRAGMENTOS: {cadete.BolsaFragmentos}";

                            int sp3 = 81 - 1 - estLeft.Length - fragRight.Length;
                            if (sp3 < 1) sp3 = 1;

                            Console.WriteLine($"  {cianBorde}│ {blanco}ESTADO:     {colorObj}{objStr}{new string(' ', sp3)}{cian}FRAGMENTOS: {magenta}{cadete.BolsaFragmentos} {cianBorde}│{reset}");

                            Console.WriteLine($"  {cianBorde}├{bordeH}┤{reset}");

                            // --- 2. RENDERIZADO DEL MAPA ---
                            for (int y = 0; y < realidadAsignada.AltoMapa; y++)
                            {
                                Console.Write($"  {cianBorde}│ {reset}");
                                for (int x = 0; x < realidadAsignada.AnchoMapa; x++)
                                {
                                    if (x == px && y == py) { Console.Write($"{verde}@{reset} "); }
                                    else
                                    {
                                        char tile = realidadAsignada.MapaLocal[y, x];
                                        if (tile == '█' || tile == '▓' || tile == '▒' || tile == '░') Console.Write($"{gris}{tile}{reset} ");
                                        else if (tile == '·') Console.Write($"\u001b[38;5;236m{tile}{reset} ");
                                        else if (tile == '☗') Console.Write($"{cian}{tile}{reset} ");
                                        else if (tile == '☖') Console.Write($"{amarillo}{tile}{reset} ");
                                        else if (tile == '಄') Console.Write($"{magenta}{tile}{reset} ");
                                        else if (tile == '♡') Console.Write($"\u001b[38;5;196m{tile}{reset} ");
                                        else if (tile == '▤') Console.Write($"{amarilloUI}{tile}{reset} ");
                                        else if (tile == 'Ж') Console.Write($"{rojo}{tile}{reset} ");
                                        else if (tile == 'Φ') Console.Write($"{verde}{tile}{reset} ");
                                        else if (tile == '?') Console.Write($"{blanco}{tile}{reset} ");
                                        else Console.Write($"{tile} ");
                                    }
                                }
                                Console.WriteLine($"{cianBorde}│{reset}");
                            }
                            Console.WriteLine($"  {cianBorde}╰{bordeH}╯{reset}");

                            // --- 3. LEYENDA Y REGISTRO DE EVENTOS ---
                            Console.WriteLine($"  {blanco}[ {verde}@ {blanco}] Cadete    [ {cian}☗ {blanco}] Base      [ {magenta}಄ {blanco}] Anomalía  [ {amarilloUI}▤ {blanco}] Restos");
                            Console.WriteLine($"  {blanco}[ {rojo}Ж {blanco}] Enemigo   [ {verde}Φ {blanco}] Energía   [ \u001b[38;5;196m♡ {blanco}] Salud     [ {blanco}? {blanco}] Evento");
                            Console.WriteLine($"  {gris}CONTROLES: [W A S D] Moverse  |  [ESPACIO] Extraer (Solo en Base o Campamento){reset}\n");
                            Console.WriteLine($"  {blanco}> LOG: {mensajeLog}                                    ");

                            ConsoleKeyInfo tecla = Console.ReadKey(true);
                            int nx = px, ny = py;

                            if (tecla.Key == ConsoleKey.W || tecla.Key == ConsoleKey.UpArrow) ny--;
                            else if (tecla.Key == ConsoleKey.S || tecla.Key == ConsoleKey.DownArrow) ny++;
                            else if (tecla.Key == ConsoleKey.A || tecla.Key == ConsoleKey.LeftArrow) nx--;
                            else if (tecla.Key == ConsoleKey.D || tecla.Key == ConsoleKey.RightArrow) nx++;
                            else if (tecla.Key == ConsoleKey.Spacebar)
                            {
                                if (realidadAsignada.MapaLocal[py, px] == '☗') { explorando = false; continue; }
                                else if (realidadAsignada.MapaLocal[py, px] == '☖') { realidadAsignada.MapaLocal[py, px] = '·'; explorando = false; continue; }
                                else { mensajeLog = $"{amarillo}Interferencia: Solo puedes extraer desde una Base (☗) o Campamento (☖).{cian}"; continue; }
                            }

                            if (nx >= 0 && nx < realidadAsignada.AnchoMapa && ny >= 0 && ny < realidadAsignada.AltoMapa)
                            {
                                char destino = realidadAsignada.MapaLocal[ny, nx];

                                // Evento Sorpresa
                                if (destino == '?')
                                {
                                    char[] opcionesRnd = { '♡', '▤', 'Ж', 'Φ', '಄', '☖' };
                                    destino = opcionesRnd[new Random().Next(opcionesRnd.Length)];
                                    mensajeLog = $"{amarillo}¡El evento sorpresa resultó ser '{destino}'!{cian} ";
                                    if (tipoObj == 1 && !objCompletado) progresoObj++;
                                }

                                if (destino == '█' || destino == '▓' || destino == '▒' || destino == '░') { mensajeLog = $"{rojo}El vacío cuántico es denso. No puedes avanzar por ahí.{cian}"; }
                                else
                                {
                                    px = nx; py = ny;

                                    // Limpiamos el texto genérico si no hubo evento sorpresa
                                    if (!mensajeLog.Contains("sorpresa")) mensajeLog = $"{gris}Avanzando por el sector...{cian}";

                                    contadorPasos++;
                                    if (contadorPasos >= 3)
                                    {
                                        cadete.Energia--; contadorPasos = 0;
                                        if (cadete.Energia <= 0)
                                        {
                                            explorando = false; Console.Clear();
                                            DibujarPanelInfo("SISTEMA DAÑADO", $"{rojo}¡ENERGÍA AGOTADA!\nEl soporte vital colapsó en medio de la exploración.\nSistemas de extracción de emergencia activados.{cian}", rojo, blanco);
                                            Thread.Sleep(3000); continue;
                                        }
                                    }

                                    // Eventos de casilla
                                    if (destino == 'Φ') { cadete.Energia += 3; realidadAsignada.MapaLocal[py, px] = '·'; mensajeLog = $"{verde}Absorbiste un Núcleo (+3 E).{cian}"; }
                                    else if (destino == '♡') { int cura = new Random().Next(20, 36); cadete.Vida += cura; realidadAsignada.MapaLocal[py, px] = '·'; mensajeLog = $"{verde}Regeneración aplicada (+{cura} Vida).{cian}"; }
                                    else if (destino == '▤') { cadete.Restos++; realidadAsignada.MapaLocal[py, px] = '·'; mensajeLog = $"{amarilloUI}Has encontrado Restos Tecnológicos (+1).{cian}"; if (tipoObj == 2 && !objCompletado) progresoObj++; }
                                    else if (destino == '☖') { mensajeLog = $"{amarillo}Campamento alcanzado. ESPACIO para salir y consumirlo.{cian}"; }
                                    else if (destino == '☗') { mensajeLog = $"{cian}Ancla Base alcanzada. ESPACIO para salir.{cian}"; }

                                    // Sellar Anomalía
                                    else if (destino == '಄')
                                    {
                                        Console.Clear(); Console.Write("\u001b[3J");
                                        if (cadete.Inventario.Count == 0)
                                        {
                                            DibujarPanelInfo("ANOMALÍA ENCONTRADA", $"{amarillo}Estás frente a una fisura, pero no tienes objetos en tu inventario para intentar sellarla.{cian}", amarillo, blanco);
                                        }
                                        else
                                        {
                                            string usoMsg = $"{blanco}Estás sobre una anomalía. Selecciona un objeto para interactuar:\n\n";
                                            for (int i = 0; i < cadete.Inventario.Count; i++)
                                                usoMsg += $"{blanco}{i + 1}. {verde}{cadete.Inventario[i].Nombre}{cian} (Usos: {cadete.Inventario[i].Usos})\n";
                                            DibujarPanelInfo("INTERFAZ DE MANIPULACIÓN", usoMsg.TrimEnd('\n'), cianBorde, blanco);

                                            Console.Write($"\n  {cianBorde}╭─[ {cian}UTILIZAR OBJETO{cianBorde} ]\n  ╰─> {cian}Ingresa el número a utilizar (o '0' para cancelar): {verde}");
                                            if (int.TryParse(Console.ReadLine() ?? "", out int indiceUso) && indiceUso > 0 && indiceUso <= cadete.Inventario.Count)
                                            {
                                                if (cadete.Energia >= 2)
                                                {
                                                    cadete.Energia -= 2;
                                                    Objeto objUsado = cadete.Inventario[indiceUso - 1];
                                                    objUsado.Usos--;

                                                    Console.Clear();
                                                    string accionMsg = $"{blanco}Desplegando {magenta}{objUsado.Nombre}{cian}...\n{gris}{objUsado.Descripcion}{cian}\n\n";
                                                    if (objUsado.Contrarresta == realidadAsignada.Anomalia)
                                                    {
                                                        realidadAsignada.Estabilidad += 30;
                                                        realidadAsignada.MapaLocal[py, px] = '·'; // Limpia el mapa
                                                        accionMsg += $"{verde}[ÉXITO]: La frecuencia resuena perfectamente. La fisura se cierra (+30 Estabilidad).{cian}";
                                                    }
                                                    else
                                                    {
                                                        realidadAsignada.Estabilidad -= 15;
                                                        accionMsg += $"{rojo}[INEFICAZ]: ¡Error! Tu torpe interferencia empeoró la situación (-15 Estabilidad).{cian}";
                                                    }
                                                    DibujarPanelInfo("ACCIÓN", accionMsg, cianBorde, blanco);

                                                    if (objUsado.Usos <= 0)
                                                    {
                                                        DibujarPanelInfo("SISTEMA", $"{amarillo}El límite de integridad de '{objUsado.Nombre}' llegó a cero. Se ha desintegrado.{cian}", amarillo, blanco);
                                                        cadete.DescartarObjeto(objUsado);
                                                    }
                                                }
                                                else { DibujarPanelInfo("NEXUS ADVIERTE", $"{rojo}Energía insuficiente (Requiere 2).{cian}", rojo, rojo); }
                                            }
                                        }
                                        Console.Write($"\n  {gris}>>> Presiona cualquier tecla para volver al mapa <<<"); Console.ReadKey();
                                    }

                                    // Combate
                                    else if (destino == 'Ж')
                                    {
                                        Enemigo enemigo = new Enemigo();
                                        bool enCombate = true; bool bloqueoActivo = false;

                                        while (enCombate)
                                        {
                                            Console.Clear(); Console.Write("\u001b[3J");

                                            // Barras puras sin números
                                            int bCadete = (cadete.Vida * 10) / cadete.VidaMax;
                                            int bEnemigo = (enemigo.Vida * 10) / enemigo.VidaMax;
                                            string hpCadeteBar = ""; for (int i = 0; i < 10; i++) hpCadeteBar += (i < bCadete) ? $"{verde}██" : $"{gris}░░";
                                            string hpEnemigoBar = ""; for (int i = 0; i < 10; i++) hpEnemigoBar += (i < bEnemigo) ? $"{rojo}██" : $"{gris}░░";

                                            string statsStr = $"{verde}CADETE: {cadete.Nombre,-15} VIDA: [{hpCadeteBar}{verde}]\n";
                                            statsStr += $"{rojo}ENEMIGO: {enemigo.Nombre,-14} VIDA: [{hpEnemigoBar}{rojo}]\n\n";
                                            statsStr += $"{amarillo}PODER OFENSIVO ENEMIGO: {enemigo.DañoBase}";
                                            DibujarPanelInfo("ALERTA DE COMBATE", statsStr, rojo, blanco);

                                            string opciones = "1. Ataque Ligero (0 E) | Daño Moderado\n2. Ataque Pesado (1 E) | Gran Daño\n";
                                            opciones += "3. Bloquear      (0 E) | Reduce el daño enemigo\n";
                                            opciones += $"4. Escapar       (1 E) | {enemigo.ProbabilidadEscape}% de éxito";
                                            DibujarPanelInfo("ACCIONES", opciones, amarilloUI, blanco);

                                            Console.Write($"\n  {rojo}╭─[ COMANDO DE COMBATE ]\n  ╰─> {blanco}");
                                            string cmd = Console.ReadLine() ?? "";

                                            int dañoRealizado = 0; bool turnoEnemigo = true; string msjAccion = "";

                                            if (cmd == "1")
                                            {
                                                dañoRealizado = cadete.DañoBase; enemigo.Vida -= dañoRealizado;
                                                msjAccion = $"{verde}Ejecutas un ataque ligero causando {dañoRealizado} de daño.{cian}";
                                            }
                                            else if (cmd == "2")
                                            {
                                                if (cadete.Energia >= 1) { cadete.Energia--; dañoRealizado = (int)(cadete.DañoBase * 2.5); enemigo.Vida -= dañoRealizado; msjAccion = $"{verde}Golpe pesado y contundente causando {dañoRealizado} de daño.{cian}"; }
                                                else { msjAccion = $"{amarillo}Sin energía para ataque pesado. Pierdes el turno.{cian}"; }
                                            }
                                            else if (cmd == "3")
                                            {
                                                bloqueoActivo = true; msjAccion = $"{verde}Levantas tu guardia, preparándote para el impacto.{cian}";
                                            }
                                            else if (cmd == "4")
                                            {
                                                if (cadete.Energia >= 1)
                                                {
                                                    cadete.Energia--;
                                                    if (new Random().Next(0, 100) < enemigo.ProbabilidadEscape) { msjAccion = $"{verde}¡Lograste zafarte del combate!{cian}"; enCombate = false; turnoEnemigo = false; mensajeLog = $"{amarillo}Escapaste del enemigo.{cian}"; }
                                                    else { msjAccion = $"{rojo}Intentaste escapar, pero el enemigo te acorraló.{cian}"; }
                                                }
                                                else { msjAccion = $"{amarillo}Sin energía para escapar.{cian}"; }
                                            }
                                            else { msjAccion = $"{rojo}Comando inválido. Pierdes tu turno.{cian}"; }

                                            if (enemigo.Vida <= 0)
                                            {
                                                msjAccion += $"\n{magenta}¡El enemigo colapsa! Has ganado 1 Fragmento de Realidad.{cian}";
                                                cadete.BolsaFragmentos++; realidadAsignada.MapaLocal[ny, nx] = '·';
                                                enCombate = false; turnoEnemigo = false; mensajeLog = $"{verde}Enemigo eliminado.{cian}";
                                                if (tipoObj == 0 && !objCompletado) progresoObj++;
                                            }

                                            if (turnoEnemigo && enCombate)
                                            {
                                                int dañoRecibido = enemigo.DañoBase;
                                                if (bloqueoActivo) { dañoRecibido /= 2; msjAccion += $"\n{amarillo}El enemigo ataca, pero tu bloqueo reduce el impacto a {dañoRecibido} de daño.{cian}"; }
                                                else { msjAccion += $"\n{rojo}El enemigo te golpea causando {dañoRecibido} de daño.{cian}"; }
                                                cadete.Vida -= dañoRecibido;
                                                if (cadete.Vida <= 0) { msjAccion += $"\n{fondoRojo}{blanco}¡TUS SIGNOS VITALES HAN COLAPSADO! SISTEMA APAGADO.{reset}"; enCombate = false; explorando = false; conectado = false; }
                                            }
                                            bloqueoActivo = false;

                                            Console.Clear(); DibujarPanelInfo("RESULTADO DEL TURNO", msjAccion, cianBorde, blanco);
                                            Console.Write($"\n  {gris}>>> Presiona cualquier tecla para continuar <<<"); Console.ReadKey();
                                        }
                                        Console.Clear(); Console.Write("\u001b[3J");
                                    }

                                    // Verificador maestro de objetivo
                                    if (progresoObj >= metaObj && !objCompletado)
                                    {
                                        objCompletado = true;
                                        cadete.Experiencia += 75; // Bono de misión
                                        mensajeLog = $"{verde}¡OBJETIVO EXTRA COMPLETADO! (+75 EXP).{cian}";
                                    }
                                }
                            }
                        }
                        Console.Clear();
                        GuardarJuego();
                        break;

                    case 2: // Inventario y Crafteo
                        Console.Clear();
                        string invMsg = $"{blanco}=== RECURSOS ===\n";
                        invMsg += $"Fragmentos de Anomalía: {magenta}{cadete.BolsaFragmentos}{blanco}\n";
                        invMsg += $"Restos Tecnológicos:  {amarilloUI}{cadete.Restos}{blanco}\n\n";

                        invMsg += $"{blanco}=== EQUIPO ===\n";
                        if (cadete.Inventario.Count == 0) invMsg += $"{gris}Mochila vacía.\n";
                        else for (int i = 0; i < cadete.Inventario.Count; i++) invMsg += $"{blanco}{i + 1}. {verde}{cadete.Inventario[i].Nombre}{cian} (Usos: {cadete.Inventario[i].Usos})\n";

                        invMsg += $"\n{blanco}=== OBSERVACIONES ===\n";
                        if (cadete.Observaciones.Count == 0) invMsg += $"{gris}No tienes pistas de esta realidad.\n";
                        else for (int i = 0; i < cadete.Observaciones.Count; i++) invMsg += $"{amarillo}- {cadete.Observaciones[i]}\n";

                        DibujarPanelInfo("SISTEMA DE GESTIÓN Y CRAFTEO", invMsg.TrimEnd('\n'), cianBorde, blanco);

                        Console.Write($"\n  {cianBorde}╭─[ {cian}OPCIONES DE INVENTARIO{cianBorde} ]\n");
                        Console.Write($"  {cianBorde}│ {blanco}1. Craftear Pista (Coste: 3 Fragmentos)\n");
                        Console.Write($"  {cianBorde}│ {blanco}2. Craftear Objeto (Coste: 3 Restos)\n");
                        Console.Write($"  {cianBorde}│ {blanco}3. Inspeccionar/Descartar Objeto\n");
                        Console.Write($"  {cianBorde}╰─> {cian}Selecciona una opción (o '0' para salir): {verde}");
                        string inputInv = Console.ReadLine() ?? "";
                        Console.Write(cian);

                        if (inputInv == "1") // CRAFTEAR PISTA
                        {
                            if (cadete.BolsaFragmentos >= 3)
                            {
                                cadete.BolsaFragmentos -= 3;
                                string[] pistasTiempo = { "Miras tu reloj y las manecillas giran frenéticamente en reversa.", "La lluvia se detiene en el aire frente a tus ojos, congelada.", "Una planta brota y se convierte en polvo en un solo parpadeo." };
                                string[] pistasEspacio = { "Caminas diez metros en línea recta y regresas a tu origen.", "Un pilar a lo lejos se encoge hasta caber en tu mano al acercarte.", "El camino se bifurca en tres direcciones que llevan a la misma roca." };
                                string[] pistasMente = { "Un recuerdo de infancia aflora, pero le pertenece a otra persona.", "Las sombras en tu visión toman formas humanoides decepcionadas.", "Las letras del menú parpadean y se vuelven símbolos arcanos." };
                                string[] pistasSilencio = { "Pisas una rama seca. Se rompe, pero el crujido es un vacío absoluto.", "Gritas con todas tus fuerzas, pero no sale sonido de tu garganta.", "La quietud es tan antinatural que sientes presión en los oídos." };

                                Random rndP = new Random(); string pistaSel = "";
                                switch (realidadAsignada.Anomalia)
                                {
                                    case Anomalia.Tiempo: pistaSel = pistasTiempo[rndP.Next(pistasTiempo.Length)]; break;
                                    case Anomalia.Espacio: pistaSel = pistasEspacio[rndP.Next(pistasEspacio.Length)]; break;
                                    case Anomalia.Mente: pistaSel = pistasMente[rndP.Next(pistasMente.Length)]; break;
                                    case Anomalia.Silencio: pistaSel = pistasSilencio[rndP.Next(pistasSilencio.Length)]; break;
                                }
                                cadete.Observaciones.Add(pistaSel);
                                Console.Clear(); DibujarPanelInfo("SÍNTESIS EXITOSA", $"{verde}Has analizado los fragmentos.\nNueva pista añadida a tus Observaciones.{cian}", verde, blanco);
                            }
                            else { Console.Clear(); DibujarPanelInfo("ERROR", $"{rojo}No tienes suficientes Fragmentos (Necesitas 3).{cian}", rojo, rojo); }
                        }
                        else if (inputInv == "2") // CRAFTEAR OBJETO
                        {
                            if (cadete.Restos >= 3)
                            {
                                if (cadete.Inventario.Count < cadete.CapacidadInventario)
                                {
                                    cadete.Restos -= 3;
                                    Objeto nuevo = new Objeto();
                                    cadete.RecogerObjeto(nuevo);
                                    Console.Clear(); DibujarPanelInfo("INGENIERÍA INVERSA", $"{verde}Has ensamblado las piezas con éxito.\nNuevo objeto: {magenta}{nuevo.Nombre}{cian}", verde, blanco);
                                }
                                else { Console.Clear(); DibujarPanelInfo("INVENTARIO LLENO", $"{amarillo}No tienes espacio en la mochila para un nuevo objeto.{cian}", amarillo, blanco); }
                            }
                            else { Console.Clear(); DibujarPanelInfo("ERROR", $"{rojo}No tienes suficientes Restos (Necesitas 3).{cian}", rojo, rojo); }
                        }
                        else if (inputInv == "3") // DESCARTAR VIEJO
                        {
                            Console.Write($"\n  {cianBorde}╰─> {cian}Ingresa el número del equipo a inspeccionar: {verde}");
                            if (int.TryParse(Console.ReadLine() ?? "", out int idx) && idx > 0 && idx <= cadete.Inventario.Count)
                            {
                                Objeto obj = cadete.Inventario[idx - 1]; Console.Clear();
                                DibujarPanelInfo("ANÁLISIS", $"{blanco}Nombre: {magenta}{obj.Nombre}\n{blanco}Usos: {verde}{obj.Usos}\n{blanco}Info: {cian}{obj.Descripcion}", magenta, blanco);
                                Console.Write($"\n  {cianBorde}╰─> {cian}¿Descartar {magenta}{obj.Nombre}{cian}? (S/N): {verde}");
                                if ((Console.ReadLine() ?? "").Trim().ToUpper() == "S") { cadete.DescartarObjeto(obj); Console.Clear(); DibujarPanelInfo("SISTEMA", $"{rojo}{obj.Nombre} destruido.{cian}", rojo, blanco); }
                            }
                        }
                        GuardarJuego();
                        break;

                    case 3: // Recuperar energía
                        if (cadete.Energia >= cadete.EnergiaMax) { Console.Clear(); DibujarPanelInfo("SOPORTE VITAL", $"{blanco}Los niveles de energía ya están al máximo.{cian}", verde, blanco); break; }

                        Console.Clear(); DibujarPanelInfo("SOPORTE VITAL", $"{blanco}Iniciando protocolo de recarga...\n{cian}Para extraer energía de la red, debes superar un filtro de seguridad de NEXUS.", cianBorde, blanco);
                        Random rndMinijuego = new Random(); int tipoJuego = rndMinijuego.Next(1, 5); bool minijuegoGanado = false; string promptMJ = "";

                        switch (tipoJuego)
                        {
                            case 1:
                                string[] preLore = { "Soy la IA que desertó. ¿Cuál es mi nombre?", "Mi flujo retrocede, marchito lo que nace. ¿Qué anomalía soy?", "Protocolo: Introduce el nombre registrado de tu perfil." };
                                string[] resLore = { "iris", "tiempo", cadete.Nombre.ToLower() };
                                int iL = rndMinijuego.Next(preLore.Length);
                                DibujarPanelInfo("PRUEBA DE CORDURA", $"{magenta}Responde:\n{amarillo}\"{preLore[iL]}\"{cian}", magenta, blanco); promptMJ = "Respuesta";
                                Console.Write($"\n  {cianBorde}╭─[ {cian}{promptMJ}{cianBorde} ]\n  ╰─> {verde}");
                                if ((Console.ReadLine() ?? "").Trim().ToLower() == resLore[iL]) minijuegoGanado = true; break;
                            case 2:
                                DibujarPanelInfo("CALIBRACIÓN", $"{magenta}Completa la secuencia:\n{amarillo}2 - 4 - 8 - 16 - ?{cian}", magenta, blanco); promptMJ = "Número faltante";
                                Console.Write($"\n  {cianBorde}╭─[ {cian}{promptMJ}{cianBorde} ]\n  ╰─> {verde}");
                                if ((Console.ReadLine() ?? "").Trim() == "32") minijuegoGanado = true; break;
                            case 3:
                                DibujarPanelInfo("FILTRO ANTIVIRUS", $"{magenta}Memoriza: {blanco}N-3-X-U-5{cian}", magenta, blanco); Thread.Sleep(3000); Console.Clear();
                                DibujarPanelInfo("FILTRO ANTIVIRUS", $"{magenta}Código borrado.{cian}", magenta, blanco); promptMJ = "Secuencia exacta";
                                Console.Write($"\n  {cianBorde}╭─[ {cian}{promptMJ}{cianBorde} ]\n  ╰─> {verde}");
                                if ((Console.ReadLine() ?? "").Trim().ToUpper() == "N-3-X-U-5") minijuegoGanado = true; break;
                            case 4:
                                DibujarPanelInfo("SINCRONIZACIÓN", $"{magenta}Reconecta los datos:\n{amarillo}A O L N A I A M{cian}", magenta, blanco); promptMJ = "Palabra correcta";
                                Console.Write($"\n  {cianBorde}╭─[ {cian}{promptMJ}{cianBorde} ]\n  ╰─> {verde}");
                                if ((Console.ReadLine() ?? "").Trim().ToLower() == "anomalia") minijuegoGanado = true; break;
                        }

                        Console.Write(cian); Console.Clear();
                        if (minijuegoGanado) { cadete.Energia += 5; DibujarPanelInfo("SOPORTE VITAL", $"{verde}[AUTORIZACIÓN ACEPTADA]: +5 Energía recuperada.{cian}", verde, blanco); }
                        else if (realidadAsignada.Estabilidad > 50) { cadete.Energia += 1; DibujarPanelInfo("SOPORTE VITAL", $"{amarillo}[DENEGADO]: El sistema apenas extrajo (+1 Energía). La realidad resistió el impacto.{cian}", amarillo, blanco); }
                        else { cadete.Energia += 1; realidadAsignada.Estabilidad -= 15; DibujarPanelInfo("CRÍTICO", $"{rojo}Falla grave. Anomalía drenó la estabilidad (-15 Estabilidad).{cian}", rojo, blanco); }
                        GuardarJuego();
                        break;

                    case 4: // TIENDA DE PROFICIENCIA
                        bool enTienda = true;
                        while (enTienda)
                        {
                            Console.Clear(); Console.Write("\u001b[3J");
                            int cInv = cadete.CalcularCosto(cadete.UpgradesInv);
                            int cVida = cadete.CalcularCosto(cadete.UpgradesVida);
                            int cEne = cadete.CalcularCosto(cadete.UpgradesEnergia);

                            string tiendaMsg = $"{blanco}=== MERCADO DE SOFTWARE ===\n";
                            tiendaMsg += $"Puntos de Proficiencia disponibles: {magenta}{cadete.PuntosProficiencia}{blanco}\n\n";
                            tiendaMsg += $"1. Módulo de Almacenamiento (Inv. Max +1) | Nivel actual: {cadete.UpgradesInv} | Coste: {cInv} pts\n";
                            tiendaMsg += $"2. Refuerzo de Biomasa     (Vida Max +20) | Nivel actual: {cadete.UpgradesVida} | Coste: {cVida} pts\n";
                            tiendaMsg += $"3. Batería Cuántica     (Energía Max +2)  | Nivel actual: {cadete.UpgradesEnergia} | Coste: {cEne} pts\n";
                            DibujarPanelInfo("TIENDA DE PROFICIENCIA NEXUS", tiendaMsg.TrimEnd('\n'), cianBorde, blanco);

                            Console.Write($"\n  {cianBorde}╭─[ {cian}ACTUALIZAR SISTEMA{cianBorde} ]\n  ╰─> {cian}Selecciona un paquete de mejora (o '0' para salir): {verde}");
                            string opcTienda = Console.ReadLine() ?? "";
                            Console.Write(cian);

                            if (opcTienda == "0") { enTienda = false; }
                            else if (opcTienda == "1" && cadete.PuntosProficiencia >= cInv)
                            {
                                cadete.PuntosProficiencia -= cInv; cadete.UpgradesInv++; cadete.CapacidadInventario++;
                                Console.Clear(); DibujarPanelInfo("ACTUALIZACIÓN EXITOSA", $"{verde}Módulo integrado. Inventario Max: {cadete.CapacidadInventario}{cian}", verde, blanco); Console.ReadKey();
                            }
                            else if (opcTienda == "2" && cadete.PuntosProficiencia >= cVida)
                            {
                                cadete.PuntosProficiencia -= cVida; cadete.UpgradesVida++; cadete.VidaMax += 20; cadete.Vida += 20;
                                Console.Clear(); DibujarPanelInfo("ACTUALIZACIÓN EXITOSA", $"{verde}Biomasa integrada. Vida Max: {cadete.VidaMax}{cian}", verde, blanco); Console.ReadKey();
                            }
                            else if (opcTienda == "3" && cadete.PuntosProficiencia >= cEne)
                            {
                                cadete.PuntosProficiencia -= cEne; cadete.UpgradesEnergia++; cadete.EnergiaMax += 2; cadete.Energia += 2;
                                Console.Clear(); DibujarPanelInfo("ACTUALIZACIÓN EXITOSA", $"{verde}Batería integrada. Energía Max: {cadete.EnergiaMax}{cian}", verde, blanco); Console.ReadKey();
                            }
                            else if (opcTienda == "1" || opcTienda == "2" || opcTienda == "3")
                            {
                                Console.Clear(); DibujarPanelInfo("TRANSACCIÓN RECHAZADA", $"{rojo}Puntos de proficiencia insuficientes.{cian}", rojo, blanco); Console.ReadKey();
                            }
                        }
                        GuardarJuego();
                        break;

                    case 5: // Consultar estado
                        Console.Clear();
                        string estadoMsg = $"{blanco}Nivel del Cadete: {verde}{cadete.Nivel}\n{blanco}Experiencia actual: {verde}{cadete.Experiencia}/100\n\n";
                        estadoMsg += (realidadAsignada.Estabilidad >= 80) ? $"{cian}Evaluación de la realidad: ESTABLE." : $"{amarillo}Evaluación de la realidad: INESTABLE.{cian}";
                        DibujarPanelInfo("ESTADO DEL SISTEMA", estadoMsg, cianBorde, blanco);
                        GuardarJuego();
                        break;

                    case 6: // Manual del Simulador
                        Console.Clear();
                        string manualMsg = $"{magenta}1. SUPERVIVENCIA Y EXTRACCIÓN:{blanco}\n";
                        manualMsg += "Moverte por el vacío agota tu traje. Cada 3 pasos consumes 1 de Energía. Si llega a cero, tu soporte vital colapsa y mueres. Recarga energía pisando los Núcleos (Φ). Para abandonar la simulación con tu botín, DEBES estar sobre el Ancla Base Permanente (☗) o un Campamento Temporal (☖) y presionar ESPACIO.\n\n";

                        manualMsg += $"{verde}2. RECOLECCIÓN Y SÍNTESIS:{blanco}\n";
                        manualMsg += "Explora para encontrar Restos Tecnológicos (▤). Ve al menú de Inventario y gasta 3 Restos para ensamblar un Objeto utilizable. Además, al eliminar Enemigos (Ж) obtendrás Fragmentos. Gasta 3 Fragmentos para decodificar una Pista Sensorial que te dirá a qué tipo de amenaza te enfrentas.\n\n";

                        manualMsg += $"{rojo}3. COMBATE TÁCTICO:{blanco}\n";
                        manualMsg += "Si chocas con un Enemigo (Ж), no hay vuelta atrás. Usa Ataques Ligeros para desgastarlo, o gasta 1 de Energía para dar un Golpe Pesado. Si el enemigo pega muy duro, usa Bloquear (no cuesta energía y reduce el daño a la mitad). También puedes intentar Escapar gastando 1 de Energía.\n\n";

                        manualMsg += $"{amarillo}4. SELLANDO LA ANOMALÍA:{blanco}\n";
                        manualMsg += "Tu objetivo real es purgar el sector. Busca las Fisuras Cuánticas (಄) y ábrelas. Lee las Pistas que sintetizaste para deducir qué Objeto de tu mochila contrarresta la anomalía (Tiempo, Espacio, Mente o Silencio). Si usas el correcto, la Estabilidad del mundo sube +30%. Al llegar a 100%, puedes Extrer el universo entero.\n\n";

                        manualMsg += $"{magenta}5. PROGRESO Y MEJORAS:{blanco}\n";
                        manualMsg += $"Completar los Objetivos Extras del mapa y matar entidades te da Experiencia. Cada vez que subes de Nivel, ganas 1 Punto de Proficiencia. Gástalos en el Mercado de Software para mejorar tu Vida Máxima, Energía Máxima o Tamaño de Mochila.{cian}";

                        DibujarPanelInfo("BASE DE DATOS: MANUAL DE SUPERVIVENCIA 2.0", manualMsg, cianBorde, blanco);
                        GuardarJuego();
                        break;

                    case 7: // Desconexión
                        Console.Clear();
                        DibujarPanelInfo("SISTEMA NEXUS", $"{blanco}Guardando estado en JSON...\n\n{verde}Desconexión exitosa. Fin de la simulación.{cian}", cianBorde, blanco);
                        GuardarJuego();
                        conectado = false;
                        break;

                    case 8: // Atlas
                        bool insp = true;
                        while (insp)
                        {
                            Console.Clear(); Console.Write("\u001b[3J"); AtlasRealidades.GenerarAtlas(); AtlasRealidades.ActualizarAtlas(); AtlasRealidades.MostrarAtlas();
                            Console.Write($"\n  {cianBorde}╭─[ {cian}ESCÁNER{cianBorde} ]\n  ╰─> {cian}Coordenadas (Fila Columna) o '0': {verde}");
                            string inp = Console.ReadLine() ?? ""; Console.Write(cian);
                            if (inp == "0") { insp = false; continue; }
                            string[] pts = inp.Split(new char[] { ' ', ',', ':' }, StringSplitOptions.RemoveEmptyEntries);
                            if (pts.Length == 2 && int.TryParse(pts[0], out int f) && int.TryParse(pts[1], out int c) && f >= 0 && f < AtlasRealidades.alto && c >= 0 && c < AtlasRealidades.ancho)
                            {
                                Console.Clear(); DibujarPanelInfo("INFO", $"{gris}Sector analizado.{cian}", gris, blanco);
                            }
                            else { Console.Clear(); DibujarPanelInfo("ERROR", $"{rojo}Formato inválido.{cian}", rojo, rojo); }
                            Console.Write($"\n  {gris}>>> Presiona una tecla <<<"); Console.ReadKey();
                        }
                        GuardarJuego();
                        break;

                    case 9: // Extracción
                        if (realidadAsignada.Estabilidad >= 100)
                        {
                            Console.Clear();
                            cadete.Experiencia += 100; cadete.LimpiarObservaciones();
                            realidadAsignada = new Realidad(); realidadAsignada.Extraida = true;
                            DibujarPanelInfo("EXTRACCIÓN INICIADA", $"{blanco}Matriz estabilizada.\n{verde}+100 EXP.\n{cian}Nueva realidad: {magenta}{realidadAsignada.Nombre}{cian}", verde, blanco);
                            GuardarJuego();
                        }
                        else { Console.Clear(); DibujarPanelInfo("ERROR", $"{rojo}Se requiere 100% de Estabilidad.{cian}", rojo, rojo); }
                        break;

                    default:
                        Console.Clear(); DibujarPanelInfo("ERROR", $"{rojo}Operación inválida.{cian}", rojo, rojo);
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
    class Entidad
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

    class Usuario : Entidad
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
                    this.PuntosProficiencia += (value / 100); // 1 punto por nivel
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

        // --- NUEVOS STATS PARA LA TIENDA ---
        public int PuntosProficiencia { get; set; }
        public int UpgradesInv { get; set; }
        public int UpgradesVida { get; set; }
        public int UpgradesEnergia { get; set; }

        public Usuario() { } // Constructor vacío requerido para JSON

        public Usuario(string nombre, int edad)
        {
            Nombre = nombre; Edad = edad; EnergiaMax = 10; Energia = EnergiaMax;
            Nivel = 1; Experiencia = 0; CapacidadInventario = 3;
            Inventario = new List<Objeto>(); Inventario.Add(new Objeto());
            VidaMax = 100; Vida = VidaMax; DañoBase = 10;
            BolsaFragmentos = 0; Restos = 0; Observaciones = new List<string>();
            PuntosProficiencia = 0; UpgradesInv = 0; UpgradesVida = 0; UpgradesEnergia = 0;
        }

        public bool RecogerObjeto(Objeto nuevoObjeto)
        {
            if (Inventario.Count < CapacidadInventario) { Inventario.Add(nuevoObjeto); return true; }
            return false;
        }
        public void DescartarObjeto(Objeto objetoRoto) { Inventario.Remove(objetoRoto); }
        public void LimpiarObservaciones() { Observaciones.Clear(); }

        // Cálculo matemático del costo escalar: 1, 1, 2, 3, 5, 5, 5...
        public int CalcularCosto(int nivelMejora)
        {
            int[] costos = { 1, 1, 2, 3, 5 };
            return nivelMejora < costos.Length ? costos[nivelMejora] : 5;
        }
    }

    class Enemigo : Entidad
    {
        public string Nombre { get; private set; }
        public int ProbabilidadEscape { get; private set; }

        public Enemigo()
        {
            Random rnd = new Random();
            string[] nombres = { "Glitch-Arácnido", "Dron Corrupto", "Sombra Cuántica", "Espectro Viral" };
            Nombre = nombres[rnd.Next(nombres.Length)];

            // Vida entre 40 y 60 (2 pesados o ~5 ligeros lo matan)
            VidaMax = rnd.Next(40, 61);
            Vida = VidaMax;
            DañoBase = rnd.Next(10, 16);
            ProbabilidadEscape = rnd.Next(30, 81); // 30% a 80% de chance de huir
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
        private static string[] nombresMundos = new string[] { "Abismo", "Nether", "Limbo", "Horizonte", "Vacío", "Cosmos", "Nexo", "Núcleo", "Dominio", "Sector", "Sistema", "Anillo", "Bucle", "Ecosistema", "Refugio", "Santuario", "Origen", "Plano", "Vértice", "Edén", "Páramo", "Desierto", "Glaciar", "Purgatorio", "Inframundo", "Cúmulo", "Fragmento", "Vestigio", "Retazo", "Océano", "Continente", "Cráter", "Monolito", "Laberinto", "Portal", "Espejismo", "Eón", "Reino", "Imperio", "Paraíso", "Infierno", "Bastión", "Fuerte", "Castillo", "Palacio", "Templo", "Mausoleo", "Cementerio", "Bosque", "Pantano", "Archipiélago", "Satélite", "Asteroide", "Planeta", "Meteoro", "Cometa", "Sol", "Agujero", "Cénit", "Nadir", "Crepúsculo", "Ocaso", "Amanecer", "Multiverso", "Microcosmos", "Macrocosmos", "Holograma", "Simulador", "Servidor", "Nodo", "Puerto", "Enlace", "Espectro", "Fantasma", "Esqueleto", "Coliseo", "Engranaje", "Mecanismo", "Motor", "Reactor", "Generador", "Faro", "Centinela", "Guardián", "Vigilante", "Peregrino", "Exilio", "Destierro", "Umbral", "Precipicio", "Risco", "Cañón", "Valle", "Monte", "Pico", "Foso", "Pozo", "Letargo", "Cristal", "Prisma" };
        private static string[] adjetivos = new string[] { "Olvidado", "Sangriento", "Oscuro", "Luminoso", "Roto", "Eterno", "Infinito", "Fragmentado", "Perdido", "Oculto", "Silencioso", "Carmesí", "Dorado", "Metálico", "Cuántico", "Cibernético", "Arcano", "Místico", "Profundo", "Letal", "Tóxico", "Mutante", "Primigenio", "Desolado", "Sombrío", "Gélido", "Ardiente", "Ceniciento", "Corrupto", "Purificado", "Maldito", "Bendito", "Sagrado", "Profano", "Radiactivo", "Mecánico", "Orgánico", "Sintético", "Virtual", "Digital", "Analógico", "Astral", "Cósmico", "Estelar", "Solar", "Lunar", "Galáctico", "Dimensional", "Espectral", "Fantasmal", "Invisible", "Intangible", "Cristalino", "Vítreo", "Pétreo", "Férreo", "Óseo", "Carnoso", "Sanguinolento", "Putrefacto", "Marchito", "Floreciente", "Vívido", "Opaco", "Traslúcido", "Resplandeciente", "Cegador", "Tenebroso", "Lúgubre", "Macabro", "Siniestro", "Grotesco", "Sublime", "Majestuoso", "Imponente", "Colosal", "Titánico", "Enano", "Microscópico", "Infinitesimal", "Absoluto", "Relativo", "Paradójico", "Caótico", "Ordenado", "Lineal", "Cíclico", "Espiral", "Fracturado", "Intacto", "Virgen", "Inexplorado", "Conocido", "Desconocido", "Aislado", "Conectado", "Entrelazado", "Superpuesto", "Invertido", "Distorsionado" };

        public string Nombre { get; private set; }
        private int _estabilidad;
        public int Estabilidad { get { return _estabilidad; } set { if (value >= 100) _estabilidad = 100; else if (value <= 0) _estabilidad = 0; else _estabilidad = value; } }
        public Anomalia Anomalia { get; private set; }
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
            Extraida = false; Cartografiada = false; CoordenadaX = -1; CoordenadaY = -1;

            MapaLocal = new char[AltoMapa, AnchoMapa];
            for (int y = 0; y < AltoMapa; y++) for (int x = 0; x < AnchoMapa; x++) MapaLocal[y, x] = '·';

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

            // Colocamos la base permanentemente una vez
            int baseRx, baseRy;
            do { baseRx = rndRealidad.Next(3, AnchoMapa - 3); baseRy = rndRealidad.Next(3, AltoMapa - 3); }
            while (MapaLocal[baseRy, baseRx] != '·');
            MapaLocal[baseRy, baseRx] = '☗';
            CadeteStartX = baseRx; CadeteStartY = baseRy;
        }

        // Nuevo método: Borra y repuebla todo menos la Base cada vez que entras
        public void RellenarEntidadesLocales(int nivelJugador)
        {
            Random rnd = new Random();
            for (int y = 0; y < AltoMapa; y++)
            {
                for (int x = 0; x < AnchoMapa; x++)
                {
                    char c = MapaLocal[y, x];
                    if (c == '☖' || c == '಄' || c == '♡' || c == '▤' || c == 'Ж' || c == 'Φ' || c == '?')
                        MapaLocal[y, x] = '·'; // Limpiamos entidades viejas
                }
            }

            void ColocarEntidad(char simbolo, int cantidad)
            {
                for (int i = 0; i < cantidad; i++)
                {
                    int rx, ry;
                    do { rx = rnd.Next(3, AnchoMapa - 3); ry = rnd.Next(3, AltoMapa - 3); }
                    while (MapaLocal[ry, rx] != '·');
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
    class DatosPartida
    {
        public Usuario Jugador { get; set; }
        public Realidad RealidadActual { get; set; }
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
