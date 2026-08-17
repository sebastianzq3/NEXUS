using System.Collections.Generic;

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

                Console.WriteLine("1. Observar realidad");
                Console.WriteLine("2. Buscar objetos");
                Console.WriteLine("3. Inventario");
                Console.WriteLine("4. Utilizar objeto (Interactuar)");
                Console.WriteLine("5. Recuperar energía");
                Console.WriteLine("6. Consultar estado");
                Console.WriteLine("7. Manual del simulador");
                Console.WriteLine("8. Intentar desconexión");
                // mostrar extracción solo cuando estabilidad es 100
                if (realidadAsignada.Estabilidad >= 100)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("9. ¡INICIAR EXTRACCIÓN! (Realidad Estabilizada)");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }

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
                    case 1: // Observar realidad (Pistas narrativas)
                        Console.Clear();
                        Console.WriteLine("========================================");
                        Console.WriteLine($"{blanco}[SENSORIAL]{cian} Sintonizando los ecos de {magenta}{realidadAsignada.Nombre}{cian}...");


                        string[] pistasTiempo =
                        {
                            "Miras tu reloj y las manecillas giran frenéticamente en sentido contrario.",
                            "Notas que la piel de tus manos envejece y rejuvenece en cuestión de segundos.",
                            "Una gota de lluvia grisácea se detiene en el aire frente a tus ojos, completamente congelada.",
                            "Escuchas tus propios pasos resonar un par de segundos ANTES de que tu bota toque el suelo.",
                            "Una planta a tus pies brota, florece, se marchita y se convierte en polvo en un solo parpadeo.",
                            "El sol parece cruzar el cielo a tirones, haciendo que las sombras de tu entorno bailen de forma errática.",
                            "Tiras una pequeña piedra y, antes de tocar el suelo, vuelve volando hacia la palma de tu mano.",
                            "Sientes un fuerte déjà vu; jurarías que ya caminaste por este mismo sendero hace exactamente un minuto.",
                            "Tu respiración suena desfasada, como si estuvieras inhalando ayer y exhalando mañana.",
                            "Ves el cadáver de un insecto en el suelo recomponerse y salir volando en reversa."
                        };

                        string[] pistasEspacio =
                        {
                            "Caminas diez metros en línea recta, pero al darte la vuelta, tu punto de origen está a kilómetros de distancia.",
                            "Las paredes de la estructura cercana no se unen en ángulos rectos, formando esquinas imposibles que marean tu vista.",
                            "Un pilar a lo lejos parece inmenso, pero al dar un paso hacia él, se encoge hasta caber en la palma de tu mano.",
                            "Miras a través del reflejo de un charco y te ves a ti mismo de espaldas, mirándote a ti mismo.",
                            "El horizonte parece curvarse hacia arriba, encerrándote en un valle que se siente como el interior de una esfera.",
                            "Intentas alcanzar un escombro cercano, pero tu brazo parece estirarse sin llegar nunca a tocarlo.",
                            "Dejas caer una moneda y, en lugar de chocar con el piso, cae infinitamente a través de un abismo que no estaba ahí.",
                            "El camino frente a ti se bifurca en tres direcciones, pero las tres parecen llevar exactamente a la misma roca.",
                            "La topografía del terreno cambia cada vez que parpadeas, alterando las distancias de forma indetectable.",
                            "El cielo y el suelo parecen intercambiar lugares bruscamente durante una fracción de segundo."
                        };

                        string[] pistasMente =
                        {
                            "Un recuerdo de tu infancia aflora, pero te das cuenta con terror de que le pertenece a otra persona.",
                            "Intentas recordar tu propio nombre por un segundo, pero tu cerebro se queda en un blanco absoluto.",
                            "Las sombras en el borde de tu visión toman formas humanoides que te observan con clara decepción.",
                            "Sientes la abrumadora certeza de que algo invisible está leyendo tus pensamientos en tiempo real.",
                            "Las letras del menú de tu traje parpadean y se transforman en símbolos incomprensibles que, extrañamente, puedes leer.",
                            "Sientes una profunda tristeza por la pérdida de un cadete compañero... un compañero que jamás existió.",
                            "Una voz idéntica a la tuya te susurra al oído que la única salida razonable es rendirse al vacío.",
                            "Cierras los ojos y, en lugar de oscuridad, ves un laberinto geométrico que pulsa al ritmo de tus latidos.",
                            "Comienzas a dudar si alguna vez entraste a la simulación NEXUS o si llevas toda tu vida atrapado aquí.",
                            "El miedo irracional de que tus propios brazos son sintéticos y no te pertenecen se apodera de tu razón."
                        };

                        string[] pistasSilencio =
                        {
                            "Pisas una rama seca. Se rompe en mil pedazos, pero el crujido es reemplazado por un vacío que lastima tus oídos.",
                            "Gritas con todas tus fuerzas, pero de tu garganta no sale absolutamente ningún sonido.",
                            "El aire es tan espeso y mudo que el latido de tu propio corazón se vuelve un tambor que te ensordece por completo.",
                            "Ves una enorme estructura colapsar a la distancia, cayendo en la más profunda y absoluta falta de ruido.",
                            "Chocas dos piezas de metal frente a tu rostro, pero el impacto no genera ni la más mínima vibración acústica.",
                            "El zumbido constante del sistema de tu traje de explorador se apaga; el vacío auditivo es casi asfixiante.",
                            "Sientes una presión enorme en los tímpanos, como si todo el sonido del mundo hubiera sido succionado hacia el cielo.",
                            "Intentas aplaudir, pero el impacto de tus palmas es absorbido por el ambiente como si golpearas bajo el agua.",
                            "La quietud es tan antinatural que sientes que hacer el más mínimo ruido podría quebrar la realidad como un cristal.",
                            "Escuchas un pitido agudo y constante dentro de tu cabeza, tu cerebro intentando compensar la muerte del sonido exterior."
                        };

                        Random rndPista = new Random();
                        string pistaDescubierta = "";

                        // pista en base a tipo de anomalía
                        switch (realidadAsignada.Anomalia)
                        {
                            case Anomalia.Tiempo:
                                pistaDescubierta = pistasTiempo[rndPista.Next(pistasTiempo.Length)];
                                break;
                            case Anomalia.Espacio:
                                pistaDescubierta = pistasEspacio[rndPista.Next(pistasEspacio.Length)];
                                break;
                            case Anomalia.Mente:
                                pistaDescubierta = pistasMente[rndPista.Next(pistasMente.Length)];
                                break;
                            case Anomalia.Silencio:
                                pistaDescubierta = pistasSilencio[rndPista.Next(pistasSilencio.Length)];
                                break;
                        }

                        // Mostramos la pista al jugador de forma misteriosa
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"\n[OBSERVACIÓN]: \"{pistaDescubierta}\"");

                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("\nRevisa tu inventario. ¿Tienes algo que contrarreste esto?");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;

                    case 2: // Buscar objetos (Reduce energía, otorga experiencia y loot)
                        if (cadete.Energia >= 3)
                        {
                            cadete.Energia -= 3;
                            cadete.Experiencia += 25;

                            Console.Clear();
                            Console.WriteLine("========================================");
                            Console.WriteLine($"{blanco}[ACCIÓN]{cian} Explorando el sector...");

                            // variaciones de texto de ambientación
                            string[] textosExploracion = new string[]
                            {
                                $"Caminas por los senderos de {magenta}{realidadAsignada.Nombre}{cian} y vislumbras algo brillando en el suelo...",
                                $"Mientras exploras las ruinas de {magenta}{realidadAsignada.Nombre}{cian}, tropiezas con un artefacto inusual...",
                                $"Una extraña resonancia en {magenta}{realidadAsignada.Nombre}{cian} te guía hacia un objeto oculto...",
                                $"Escaneando la superficie de {magenta}{realidadAsignada.Nombre}{cian}, tu visor detecta una anomalía material...",
                                $"Entre las sombras de {magenta}{realidadAsignada.Nombre}{cian}, descubres algo que no pertenece a este lugar...",
                                $"Avanzas con cautela por {magenta}{realidadAsignada.Nombre}{cian} y encuentras los restos de un explorador anterior. Dejó caer algo...",
                                $"El viento cuántico de {magenta}{realidadAsignada.Nombre}{cian} aparta el polvo, revelando un misterioso artefacto...",
                                $"Inspeccionando una estructura inestable en {magenta}{realidadAsignada.Nombre}{cian}, hallas una pieza de equipo intacta...",
                                $"Sientes un leve tirón magnético en {magenta}{realidadAsignada.Nombre}{cian} que te lleva directamente hacia un ítem...",
                                $"Tras una larga caminata por los ecos de {magenta}{realidadAsignada.Nombre}{cian}, notas un objeto flotando en el aire..."
                            };

                            Random rndExploracion = new Random();
                            string ambientacion = textosExploracion[rndExploracion.Next(textosExploracion.Length)];

                            // imprimir ambientación y crear el objeto encontrado
                            Console.WriteLine($"\n{ambientacion}");
                            Objeto lootEncontrado = new Objeto();
                            Console.WriteLine($"¡Has encontrado un(a) {magenta}{lootEncontrado.Nombre}{cian}!");

                            // INTENTAR guardar el objeto
                            bool guardado = cadete.RecogerObjeto(lootEncontrado); //esta funcion de usuario devolvía un bool dependiendo de la capacidad del inv

                            if (guardado)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"\n[ÉXITO]: El objeto ha sido almacenado en tu inventario de forma segura.");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.WriteLine($"\n[INVENTARIO LLENO]: Intentas guardar el(la) {lootEncontrado.Nombre}, pero no tienes espacio ({cadete.CapacidadInventario}/{cadete.CapacidadInventario}).");
                                Console.WriteLine("Al no poder contenerlo, el objeto pierde cohesión y desaparece frente a tus ojos.");
                            }

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"\n+25 EXP ganada por la exploración.");
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("NEXUS ADVIERTE: Energía insuficiente para explorar y buscar objetos.");
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        }
                        break;

                    case 3: // Inventario
                        Console.WriteLine($"{blanco}[INVENTARIO DEL EXPLORADOR]{cian}");

                        // Inventario vacío?
                        if (cadete.Inventario.Count == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine("Tu inventario está vacío. No tienes objetos para inspeccionar.");
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        }
                        else
                        {
                            // 1. Mostrar la lista de objetos enumerados
                            Console.WriteLine($"Capacidad actual: {cadete.Inventario.Count}/{cadete.CapacidadInventario}\n");
                            for (int i = 0; i < cadete.Inventario.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {verde}{cadete.Inventario[i].Nombre}{cian}");
                            }

                            // 2. Pedir selección
                            Console.Write("\nSelecciona el número del objeto para inspeccionarlo (o presiona '0' para cancelar): ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            string inputInventario = Console.ReadLine();
                            Console.ForegroundColor = ConsoleColor.Cyan;

                            // 3. Validar la entrada y mostrar los detalles
                            if (int.TryParse(inputInventario, out int indiceObjeto))
                            {
                                if (indiceObjeto > 0 && indiceObjeto <= cadete.Inventario.Count)
                                {
                                    Objeto objetoSeleccionado = cadete.Inventario[indiceObjeto - 1]; // restamos 1 para q siga los índices del menú

                                    Console.WriteLine($"\n{blanco}--- ANÁLISIS DE OBJETO ---{cian}");
                                    Console.WriteLine($"Nombre:      {magenta}{objetoSeleccionado.Nombre}{cian}");
                                    Console.WriteLine($"Usos rest.:  {verde}{objetoSeleccionado.Usos}{cian}");
                                    Console.WriteLine($"Descripción: {blanco}{objetoSeleccionado.Descripcion}{cian}");
                                    Console.WriteLine($"{blanco}--------------------------{cian}");

                                    // Pregunta si desea descartarlo
                                    Console.Write($"\n¿Deseas descartar {magenta}{objetoSeleccionado.Nombre}{cian} para liberar espacio? (S/N): ");

                                    Console.ForegroundColor = ConsoleColor.Green;
                                    string opcionDescartar = Console.ReadLine().Trim().ToUpper();
                                    Console.ForegroundColor = ConsoleColor.Cyan;

                                    if (opcionDescartar == "S")
                                    {
                                        cadete.DescartarObjeto(objetoSeleccionado);

                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine($"\n[SISTEMA] {objetoSeleccionado.Nombre} ha sido destruido en el vacío cuántico.");
                                        Console.ForegroundColor = ConsoleColor.Cyan;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"\n{blanco}[SISTEMA]{cian} El objeto permanece seguro en tu inventario.");
                                    }
                                }
                                else if (indiceObjeto == 0)
                                {
                                    Console.WriteLine($"{blanco}[SISTEMA]{cian} Inspección cancelada.");
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("ERROR: Ranura de inventario no encontrada.");
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("ERROR: Entrada no válida.");
                                Console.ForegroundColor = ConsoleColor.Cyan;
                            }
                        }
                        break;

                    case 4: // Utilizar objeto
                        Console.WriteLine($"{blanco}[INTERACCIÓN]{cian} Preparando interfaz de manipulación cuántica...");

                        // inventario tiene objetos?
                        if (cadete.Inventario.Count == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine("Tu inventario está vacío. No tienes herramientas para interactuar con esta realidad.");
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                        }

                        // mostrar inv
                        Console.WriteLine("Selecciona un objeto de tu inventario:\n");
                        for (int i = 0; i < cadete.Inventario.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {verde}{cadete.Inventario[i].Nombre}{cian} (Usos restantes: {cadete.Inventario[i].Usos})");
                        }

                        Console.Write("\nIngresa el número del objeto a utilizar (o '0' para cancelar): ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        string inputUso = Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.Cyan;

                        // validación
                        if (int.TryParse(inputUso, out int indiceUso))
                        {
                            if (indiceUso > 0 && indiceUso <= cadete.Inventario.Count)
                            {
                                // 4. Verificamos la energía ANTES de gastar el objeto
                                if (cadete.Energia >= 2)
                                {
                                    cadete.Energia -= 2;
                                    Objeto objetoUsado = cadete.Inventario[indiceUso - 1];

                                    // Restamos un uso al objeto
                                    objetoUsado.Usos--;

                                    Console.Clear();
                                    Console.WriteLine($"========================================");
                                    Console.WriteLine($"{blanco}[ACCIÓN]{cian} Desplegando {magenta}{objetoUsado.Nombre}{cian}...");
                                    Console.WriteLine($"{blanco}{objetoUsado.Descripcion}{cian}\n");

                                    // objeto contrarresta la anomalía actual?
                                    if (objetoUsado.Contrarresta == realidadAsignada.Anomalia)
                                    {
                                        realidadAsignada.Estabilidad += 30; // recompensa
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine($"[ÉXITO]: La frecuencia del objeto resuena perfectamente con la anomalía.");
                                        Console.WriteLine("La estructura de la realidad se fortalece (+30 Estabilidad).");
                                    }
                                    else
                                    {
                                        realidadAsignada.Estabilidad -= 15; // penalización
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine($"[INEFICAZ]: ¡Error de cálculo! Tu {objetoUsado.Nombre} no hizo absolutamente nada contra la anomalía.");
                                        Console.WriteLine("Tu torpe interferencia solo alteró el delicado equilibrio local, empeorando la situación (-15 Estabilidad).");
                                    }

                                    // 6. Si el objeto se queda sin usos, lo destruimos automáticamente
                                    if (objetoUsado.Usos <= 0)
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.WriteLine($"\n[SISTEMA] El límite de integridad de '{objetoUsado.Nombre}' ha llegado a cero. El objeto se ha desintegrado en tus manos.");
                                        cadete.DescartarObjeto(objetoUsado);
                                    }

                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("NEXUS ADVIERTE: Energía insuficiente para intentar una interacción.");
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                }
                            }
                            else if (indiceUso == 0)
                            {
                                Console.WriteLine($"{blanco}[SISTEMA]{cian} Interacción cancelada. Retornando al menú...");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("ERROR: Ranura de inventario no encontrada.");
                                Console.ForegroundColor = ConsoleColor.Cyan;
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("ERROR: Entrada no válida.");
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        }
                        break;

                    case 5: // Recuperar energía
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

                    case 6: // Consultar estado
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

                    case 7: // Manual del Simulador
                        Console.Clear();
                        Console.WriteLine("=======================================================");
                        Console.WriteLine($"{blanco}          [BASE DE DATOS: MANUAL DEL EXPLORADOR]       {cian}");
                        Console.WriteLine("=======================================================");

                        Console.WriteLine($"\n{magenta}1. OBJETIVO DE LA SIMULACIÓN:{cian}");
                        Console.WriteLine("Tu misión es adentrarte en simulaciones cuánticas inestables,");
                        Console.WriteLine("sobrevivir a sus peligros y mantener la ESTABILIDAD del mundo.");
                        Console.WriteLine("Si la Estabilidad cae a 20% o menos, la ANOMALÍA IRIS tomará el control.");

                        Console.WriteLine($"\n{verde}2. ENERGÍA Y RECURSOS:{cian}");
                        Console.WriteLine($"* {blanco}Energía:{cian} Necesaria para realizar acciones. Si se agota, quedarás");
                        Console.WriteLine("  indefenso. Usa la opción 'Recuperar energía' para recargarla.");
                        Console.WriteLine($"* {blanco}Experiencia:{cian} Sube tu Nivel de Cadete al explorar realidades.");

                        Console.WriteLine($"\n{magenta}3. LAS 4 ANOMALÍAS:{cian}");
                        Console.WriteLine("Cada mundo está corrompido por una anomalía oculta:");
                        Console.WriteLine($"{blanco}TIEMPO, ESPACIO, MENTE o SILENCIO.{cian}");
                        Console.WriteLine("Usa la opción 'Observar realidad' para recibir pistas sensoriales");
                        Console.WriteLine("del entorno y deducir a qué tipo de anomalía te estás enfrentando.");

                        Console.WriteLine($"\n{verde}4. INVENTARIO Y LOOT:{cian}");
                        Console.WriteLine($"* {blanco}Buscar objetos:{cian} Gasta energía, pero puedes encontrar Artefactos.");
                        Console.WriteLine("* Tu mochila tiene capacidad limitada. Deberás descartar objetos");
                        Console.WriteLine("  si quieres recoger equipo nuevo.");

                        Console.WriteLine($"\n{magenta}5. INTERACTUAR CON LA REALIDAD (SUPERVIVENCIA):{cian}");
                        Console.WriteLine("Una vez que deduzcas qué anomalía afecta al mundo, elige un objeto");
                        Console.WriteLine("de tu inventario (leyendo su descripción) e interactúa con la realidad.");
                        Console.WriteLine($"* {verde}Sinergia Correcta:{cian} La Estabilidad aumenta drásticamente.");
                        Console.WriteLine($"* {ConsoleColor.Red}Elección Incorrecta:{cian} La realidad empeora y pierdes Estabilidad.");

                        Console.WriteLine("\n=======================================================");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("FIN DEL ARCHIVO. Presiona cualquier tecla para volver al menú...");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;

                    case 8: // Desconexión
                        Console.WriteLine($"{blanco}[SISTEMA NEXUS]{cian} Iniciando protocolo de desconexión...");
                        Console.WriteLine("Guardando estado del cadete...");
                        Console.WriteLine($"\n{verde}Desconexión exitosa. Fin de la simulación.{cian}");
                        conectado = false;
                        break;

                    case 9: // extracción (solo aparece cuando Estabilidad es 100%)
                        if (realidadAsignada.Estabilidad >= 100)
                        {
                            Console.Clear();
                            Console.WriteLine("=======================================================");
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("           [PROTOCOLO DE EXTRACCIÓN INICIADO]          ");
                            Console.ResetColor();
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("=======================================================");

                            Console.WriteLine($"\n{blanco}[NEXUS]{cian} Sellando fisuras cuánticas en {magenta}{realidadAsignada.Nombre}{cian}...");
                            Console.WriteLine("La matriz espacial de este universo ha sido estabilizada por completo.");
                            Console.WriteLine($"La anomalía de tipo {magenta}{realidadAsignada.Anomalia}{cian} ha sido purgada. Has salvado esta realidad del colapso.");

                            // recompensa
                            cadete.Experiencia += 100;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"\n[RECOMPENSA DE EXTRACCIÓN]: {blanco}+100 EXP{verde} obtenida.");
                            Console.WriteLine($"Nivel actual del Cadete: {blanco}{cadete.Nivel}{verde}.");
                            Console.ForegroundColor = ConsoleColor.Cyan;

                            // nueva realidad
                            realidadAsignada = new Realidad();
                            Console.WriteLine($"\n{blanco}[SISTEMA NEXUS]{cian} Desconectando anclajes temporales...");
                            Console.WriteLine($"Buscando un nuevo mundo al borde del colapso...");
                            Console.WriteLine($"\nSincronizando nuevas coordenadas cuánticas...");
                            Console.WriteLine($"Nueva realidad asignada al cadete: {magenta}{realidadAsignada.Nombre}{cian}");
                            Console.WriteLine($"Nivel de amenaza inicial (Estabilidad): {verde}{realidadAsignada.Estabilidad}%{cian}");
                        }
                        else
                        {
                            // Protección por si el usuario presiona 9 cuando no debe
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("ERROR DE PROTOCOLO: Extracción denegada.");
                            Console.WriteLine($"Para sellar un universo se requiere un 100% de Estabilidad. (Actual: {realidadAsignada.Estabilidad}%)");
                            Console.WriteLine("Continúa purgando las anomalías del sector.");
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        }
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
            Random rndRealidad = new Random();
            this.Nombre = $"{nombresMundos[rndRealidad.Next(nombresMundos.Length)]} {adjetivos[rndRealidad.Next(adjetivos.Length)]}";
            this.Estabilidad = rndRealidad.Next(30, 70);
            this.Anomalia = (Anomalia)rndRealidad.Next(0, 4);
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
