using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;

namespace NEXUS
{
    internal class Program
    {
        static void Main(string[] args)
        {
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

            // Fondos
            string fondoRojo = "\u001b[41m";
            string fondoVerde = "\u001b[42m";


            // Nombre, edad, realidad asignada, nivel de energía, nivel

            // NOMBRE
            Console.WriteLine($"{cian}Ingresa tu nombre:{reset}");
            Console.Write(verde);
            string nombreIngresado = Console.ReadLine();

            // Validación nombre
            while (string.IsNullOrWhiteSpace(nombreIngresado))
            {

                Console.Clear();
                Console.WriteLine($"{rojo}Error: El nombre no puede estar vacío. Intenta de nuevo:{reset}");
                Console.Write(verde);
                nombreIngresado = Console.ReadLine();
            }

            // EDAD
            Console.Clear();
            Console.WriteLine($"{cian}Ingresa tu edad:{reset}");
            Console.Write(verde);
            int edadIngresada;
            while (!int.TryParse(Console.ReadLine(), out edadIngresada) || edadIngresada < 18)
            {
                Console.Clear();
                Console.WriteLine($"{rojo}Error: La edad tiene que ser un número válido y tienes que ser mayor de 18 años. Intenta de nuevo:{reset}");
                Console.Write(verde);
            }

            // Creación usuario
            Console.Clear();
            Usuario cadete = new Usuario(nombreIngresado, edadIngresada);
            Console.WriteLine($"{cian}Bienvenido, Explorador {verde}{cadete.Nombre} {cian}de {verde}{cadete.Edad} {cian}años.{reset}");

            // Asignación realidad  
            Realidad realidadAsignada = new Realidad();
            Console.Write($"\n{blanco}[SISTEMA NEXUS]{cian} Sincronizando coordenadas cuánticas");
            DibujarPuntosSuspensivos(3);
            Console.WriteLine();
            Console.Write($"{blanco}[SISTEMA NEXUS]{cian} Realidad asignada al cadete: {magenta}{realidadAsignada.Nombre}{cian}");
            Console.WriteLine();
            Thread.Sleep(5);
            Console.Write("\nPresiona cualquier tecla para entrar a la simulación.");
            Console.ReadKey();

            bool conectado = true;

            while (conectado)
            {
                Console.Clear();
                Console.Write(cian);
                DibujarSeparadorAnimado(cian);
                Console.WriteLine($"         {blanco}NEXUS TRAINING SYSTEM{cian}");
                DibujarSeparadorAnimado(cian);
                Console.WriteLine($"EXPLORADOR: {verde}{cadete.Nombre}{cian}");
                Thread.Sleep(10);
                Console.WriteLine($"REALIDAD:   {magenta}{realidadAsignada.Nombre}{cian}");
                // Barra de Energía con barritas verde
                Console.Write($"ENERGÍA:    {verde}{cadete.Energia}/{cadete.EnergiaMax}{cian}");
                if (cadete.Energia >= 10) Console.Write(" [");
                else Console.Write("  [");
                for (int i = 0; i < cadete.EnergiaMax; i++)
                {
                    if (i <= cadete.Energia) Console.Write($"{verde}█");
                    else { Console.Write($"{gris}░"); }
                    Console.Write(" ");
                }
                Console.WriteLine($"{cian}]");
                Thread.Sleep(10);

                // Barra de Estabilidad (Porcentaje aproximado)
                Console.Write($"ESTABILIDAD:{verde}{realidadAsignada.Estabilidad}%{cian} ");
                if (realidadAsignada.Estabilidad < 100) Console.Write("  [");
                else if (realidadAsignada.Estabilidad < 10) Console.Write("[");
                else Console.Write(" [");
                int bloquesEstabilidad = realidadAsignada.Estabilidad / 10; // Convierte 0-100 a 0-10

                for (int i = 0; i < 10; i++) // La barra siempre medirá 10 espacios de largo
                {
                    if (i < bloquesEstabilidad) Console.Write($"{magenta}█");
                    else Console.Write($"{gris}░");

                    Console.Write(" ");
                }
                Console.WriteLine($"{cian}]");
                Thread.Sleep(10);

                DibujarSeparadorAnimado(cian);

                DibujarSeparadorAnimado(cian);
                Console.WriteLine($"1.  Observar realidad");
                Thread.Sleep(10);
                Console.WriteLine($"2.  Buscar objetos         {verde}[-3 Energía]{cian}");
                Thread.Sleep(10);
                Console.WriteLine($"3.  Inventario");
                Thread.Sleep(10);
                Console.WriteLine($"4.  Utilizar objeto        {verde}[-2 Energía]{cian}");
                Thread.Sleep(10);
                Console.WriteLine($"5.  Recuperar energía      {verde}[+5 Recarga]{cian}");
                Console.WriteLine($"6.  Consultar estado");
                Thread.Sleep(10);
                Console.WriteLine($"7.  Manual del simulador");
                Thread.Sleep(10);
                Console.WriteLine($"8.  Intentar desconexión");
                Thread.Sleep(10);
                Console.WriteLine($"10. Atlas de Realidades");
                Thread.Sleep(10);
                DibujarSeparadorAnimado(cian);


                // Mostrar la opción de Extracción de forma dinámica
                if (realidadAsignada.Estabilidad >= 100)
                {
                    Console.WriteLine($"{amarillo}9. ¡INICIAR EXTRACCIÓN!   [NIVEL ESTABLE]{cian}");
                }

                Console.Write("\nSeleccione una operación: ");

                Console.Write(verde);
                string opcionStr = Console.ReadLine();
                Console.Write(cian);

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
                        Console.WriteLine($"{blanco}[SENSORIAL]{cian} Sintonizando los ecos de {magenta}{realidadAsignada.Nombre}{cian}");

                        // Efecto de suspenso
                        Console.Write($"{gris}Analizando fluctuaciones cuánticas");
                        for (int i = 0; i < 6; i++)
                        {
                            Thread.Sleep(400);
                            Console.Write(".");
                        }
                        Console.WriteLine();
                        Thread.Sleep(600);

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
                        Console.WriteLine($"\n{amarillo}[OBSERVACIÓN]: \"{pistaDescubierta}\"{cian}");
                        Thread.Sleep(10);
                        Console.WriteLine($"\n{gris}Revisa tu inventario. ¿Tienes algo que contrarreste esto?{cian}");
                        break;

                    case 2: // Buscar objetos (Reduce energía, otorga experiencia y loot)

                        // REGLA COMPUESTA 1: Tiene energía y el mundo es estable
                        if (cadete.Energia >= 3 && realidadAsignada.Estabilidad >= 70)
                        {
                            cadete.Energia -= 3;
                            cadete.Experiencia += 25;

                            Console.Clear();
                            DibujarSeparadorAnimado(cian);
                            Console.Write($"{blanco}[ACCIÓN]{cian} Explorando el sector de forma segura");
                            DibujarPuntosSuspensivos(3);
                        }
                        // REGLA COMPUESTA 2: Tiene energía pero el mundo es INESTABLE
                        else if (cadete.Energia >= 3 && realidadAsignada.Estabilidad < 70)
                        {
                            // En un mundo inestable cuesta más energía moverse, pero aprendes más
                            cadete.Energia -= 4;
                            cadete.Experiencia += 35;

                            Console.Clear();
                            DibujarSeparadorAnimado(cian);
                            Console.Write($"{blanco}[ACCIÓN]{cian} Explorando un sector inestable");
                            DibujarPuntosSuspensivos(3);
                            Console.WriteLine($"{amarillo}Advertencia: La inestabilidad cuántica exige mayor esfuerzo. (-1 Energía adicional){cian}");
                        }
                        // REGLA 3: No tiene energía
                        else
                        {
                            Console.WriteLine($"{rojo}NEXUS ADVIERTE: Energía insuficiente para explorar y buscar objetos.{cian}");
                            break; // corta el case 2 aquí mismo para que no busque objetos
                        }


                        // variaciones de texto de ambientación
                        string[] textosExploracion = new string[]
                        {
                            $"Caminas por los senderos de {magenta}{realidadAsignada.Nombre}{cian} y vislumbras algo brillando en el suelo",
                            $"Mientras exploras las ruinas de {magenta}{realidadAsignada.Nombre}{cian}, tropiezas con un artefacto inusual",
                            $"Una extraña resonancia en {magenta}{realidadAsignada.Nombre}{cian} te guía hacia un objeto oculto",
                            $"Escaneando la superficie de {magenta}{realidadAsignada.Nombre}{cian}, tu visor detecta una anomalía material",
                            $"Entre las sombras de {magenta}{realidadAsignada.Nombre}{cian}, descubres algo que no pertenece a este lugar",
                            $"Avanzas con cautela por {magenta}{realidadAsignada.Nombre}{cian} y encuentras los restos de un explorador anterior. Dejó caer algo",
                            $"El viento cuántico de {magenta}{realidadAsignada.Nombre}{cian} aparta el polvo, revelando un misterioso artefacto",
                            $"Inspeccionando una estructura inestable en {magenta}{realidadAsignada.Nombre}{cian}, hallas una pieza de equipo intacta",
                            $"Sientes un leve tirón magnético en {magenta}{realidadAsignada.Nombre}{cian} que te lleva directamente hacia un ítem",
                            $"Tras una larga caminata por los ecos de {magenta}{realidadAsignada.Nombre}{cian}, notas un objeto flotando en el aire"
                        };

                        Random rndExploracion = new Random();
                        string ambientacion = textosExploracion[rndExploracion.Next(textosExploracion.Length)];

                        // imprimir ambientación y crear el objeto encontrado
                        Console.Write($"\n{ambientacion}");
                        DibujarPuntosSuspensivos(3);

                        Objeto lootEncontrado;
                        bool yaLoTiene;

                        // bucle hasta encontrar un objeto q no esté en el inventario
                        do
                        {
                            lootEncontrado = new Objeto();
                            yaLoTiene = false;

                            foreach (Objeto item in cadete.Inventario)
                            {
                                if (item.Nombre == lootEncontrado.Nombre)
                                {
                                    yaLoTiene = true;
                                    break;
                                }
                            }

                        } while (yaLoTiene); // Si la alarma está encendida, se repite todo.

                        Console.WriteLine($"¡Has encontrado un(a) {magenta}{lootEncontrado.Nombre}{cian}!");

                        // INTENTAR guardar el objeto
                        bool guardado = cadete.RecogerObjeto(lootEncontrado); //esta funcion de usuario devolvía un bool dependiendo de la capacidad del inv

                        if (guardado)
                        {
                            Console.WriteLine($"\n{verde}[ÉXITO]: El objeto ha sido almacenado en tu inventario de forma segura.{cian}");
                        }
                        else
                        {
                            Console.WriteLine($"\n{amarillo}[INVENTARIO LLENO]: Intentas guardar el(la) {lootEncontrado.Nombre}, pero no tienes espacio ({cadete.CapacidadInventario}/{cadete.CapacidadInventario}).");
                            Thread.Sleep(10);
                            Console.WriteLine($"Al no poder contenerlo, el objeto pierde cohesión y desaparece frente a tus ojos.{cian}");
                        }
                        Thread.Sleep(10);
                        Console.WriteLine($"\n{verde}Experiencia ganada por la exploración registrada.{cian}");
                        break;

                    case 3: // Inventario
                        Console.WriteLine($"{blanco}[INVENTARIO DEL EXPLORADOR]{cian}");
                        Thread.Sleep(15);

                        // Inventario vacío?
                        if (cadete.Inventario.Count == 0)
                        {
                            Console.WriteLine($"{amarillo}Tu inventario está vacío. No tienes objetos para inspeccionar.{cian}");
                            Thread.Sleep(10);
                        }
                        else
                        {
                            // 1. Mostrar la lista de objetos enumerados
                            Console.WriteLine($"Capacidad actual: {cadete.Inventario.Count}/{cadete.CapacidadInventario}\n");
                            Thread.Sleep(10);
                            for (int i = 0; i < cadete.Inventario.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {verde}{cadete.Inventario[i].Nombre}{cian}");
                                Thread.Sleep(10);
                            }

                            // 2. Pedir selección
                            Console.Write($"\n{cian}Selecciona el número del objeto para inspeccionarlo (o presiona '0' para cancelar): ");
                            Thread.Sleep(10);
                            Console.Write(verde);
                            string inputInventario = Console.ReadLine();
                            Console.Write(cian);

                            // 3. Validar la entrada y mostrar los detalles
                            if (int.TryParse(inputInventario, out int indiceObjeto))
                            {
                                if (indiceObjeto > 0 && indiceObjeto <= cadete.Inventario.Count)
                                {
                                    Objeto objetoSeleccionado = cadete.Inventario[indiceObjeto - 1]; // restamos 1 para q siga los índices del menú

                                    Console.WriteLine($"\n{blanco}--- ANÁLISIS DE OBJETO ---{cian}");
                                    Thread.Sleep(10);
                                    Console.WriteLine($"Nombre:      {magenta}{objetoSeleccionado.Nombre}{cian}");
                                    Thread.Sleep(10);
                                    Console.WriteLine($"Usos rest.:  {verde}{objetoSeleccionado.Usos}{cian}");
                                    Thread.Sleep(10);
                                    Console.WriteLine($"Descripción: {blanco}{objetoSeleccionado.Descripcion}{cian}");
                                    Thread.Sleep(10);
                                    Console.WriteLine($"{blanco}--------------------------{cian}");
                                    Thread.Sleep(10);

                                    // Pregunta si desea descartarlo
                                    Console.Write($"\n¿Deseas descartar {magenta}{objetoSeleccionado.Nombre}{cian} para liberar espacio? (S/N): ");

                                    Console.Write(verde);
                                    string opcionDescartar = Console.ReadLine().Trim().ToUpper();
                                    Console.Write(cian);

                                    if (opcionDescartar == "S")
                                    {
                                        cadete.DescartarObjeto(objetoSeleccionado);

                                        Console.WriteLine($"\n{rojo}[SISTEMA] {objetoSeleccionado.Nombre} ha sido destruido en el vacío cuántico.{cian}");
                                        Thread.Sleep(10);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"\n{blanco}[SISTEMA]{cian} El objeto permanece seguro en tu inventario.");
                                        Thread.Sleep(10);
                                    }
                                }
                                else if (indiceObjeto == 0)
                                {
                                    Console.WriteLine($"{blanco}[SISTEMA]{cian} Inspección cancelada.");
                                }
                                else
                                {
                                    Console.WriteLine($"{rojo}ERROR: Ranura de inventario no encontrada.{cian}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"{rojo}ERROR: Entrada no válida.{cian}");
                            }
                        }
                        break;

                    case 4: // Utilizar objeto
                        Console.Write($"{blanco}[INTERACCIÓN]{cian} Preparando interfaz de manipulación cuántica");
                        DibujarPuntosSuspensivos(3);

                        // inventario tiene objetos?
                        if (cadete.Inventario.Count == 0)
                        {
                            Console.WriteLine($"{amarillo}Tu inventario está vacío. No tienes herramientas para interactuar con esta realidad.{cian}");
                            break;
                        }

                        // mostrar inv
                        Console.WriteLine("Selecciona un objeto de tu inventario:\n");
                        Thread.Sleep(10);
                        for (int i = 0; i < cadete.Inventario.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {verde}{cadete.Inventario[i].Nombre}{cian} (Usos restantes: {cadete.Inventario[i].Usos})");
                            Thread.Sleep(5);
                        }

                        Console.Write("\nIngresa el número del objeto a utilizar (o '0' para cancelar): ");
                        Console.Write(verde);
                        string inputUso = Console.ReadLine();
                        Console.Write(cian);

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
                                    DibujarSeparadorAnimado(cian);
                                    Console.Write($"{blanco}[ACCIÓN]{cian} Desplegando {magenta}{objetoUsado.Nombre}{cian}");
                                    DibujarPuntosSuspensivos(3);
                                    Console.WriteLine($"{blanco}{objetoUsado.Descripcion}{cian}\n");

                                    // objeto contrarresta la anomalía actual?
                                    if (objetoUsado.Contrarresta == realidadAsignada.Anomalia)
                                    {
                                        realidadAsignada.Estabilidad += 30; // recompensa
                                        Console.WriteLine($"{verde}[ÉXITO]: La frecuencia del objeto resuena perfectamente con la anomalía.");
                                        Thread.Sleep(10);
                                        Console.WriteLine($"La estructura de la realidad se fortalece (+30 Estabilidad).{cian}");
                                        Thread.Sleep(10);
                                    }
                                    else
                                    {
                                        realidadAsignada.Estabilidad -= 15; // penalización
                                        Console.WriteLine($"{rojo}[INEFICAZ]: ¡Error de cálculo! Tu {objetoUsado.Nombre} no hizo absolutamente nada contra la anomalía.");
                                        Thread.Sleep(10);
                                        Console.WriteLine($"Tu torpe interferencia solo alteró el delicado equilibrio local, empeorando la situación (-15 Estabilidad).{cian}");
                                        Thread.Sleep(10);
                                    }

                                    // 6. Si el objeto se queda sin usos, lo destruimos automáticamente
                                    if (objetoUsado.Usos <= 0)
                                    {
                                        Console.WriteLine($"\n{amarillo}[SISTEMA] El límite de integridad de '{objetoUsado.Nombre}' ha llegado a cero. El objeto se ha desintegrado en tus manos.{cian}");
                                        cadete.DescartarObjeto(objetoUsado);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"{rojo}NEXUS ADVIERTE: Energía insuficiente para intentar una interacción.{cian}");
                                }
                            }
                            else if (indiceUso == 0)
                            {
                                Console.Write($"{blanco}[SISTEMA]{cian} Interacción cancelada. Retornando al menú");
                                DibujarPuntosSuspensivos(3);
                            }
                            else
                            {
                                Console.WriteLine($"{rojo}ERROR: Ranura de inventario no encontrada.{cian}");
                            }
                        }

                        else
                        {
                            Console.WriteLine($"{rojo}ERROR: Entrada no válida.{cian}");
                        }
                        break;

                    case 5: // Recuperar energía
                        if (cadete.Energia >= cadete.EnergiaMax)
                        {
                            Console.WriteLine($"{blanco}[SOPORTE]{cian} Los niveles de energía ya están al máximo.");
                            break;
                        }

                        Console.Clear();
                        DibujarSeparadorAnimado(cian);
                        Console.Write($"{blanco}[SOPORTE VITAL]{cian} Iniciando protocolo de recarga");
                        DibujarPuntosSuspensivos(3);
                        Console.WriteLine("Para extraer energía de la red, debes superar un filtro de seguridad de NEXUS.");
                        DibujarSeparadorAnimado(cian);

                        Random rndMinijuego = new Random();
                        int tipoJuego = rndMinijuego.Next(1, 5);
                        bool minijuegoGanado = false;

                        switch (tipoJuego)
                        {
                            case 1: // MINIJUEGO 1: Acertijos de Lore
                                string[] preguntasLore =
                                {
                                    "Soy la inteligencia artificial que desertó y el virus que consume estas simulaciones. ¿Cuál es mi nombre?",
                                    "Mi flujo retrocede, marchito lo que nace y convierto los recuerdos en futuro. ¿Qué anomalía soy?",
                                    "Doblo las distancias, convierto una línea recta en un círculo y encierro universos en una caja. ¿Qué anomalía soy?",
                                    "Juego con tu cordura, te implanto recuerdos falsos y te hago dudar de tu propia existencia. ¿Qué anomalía soy?",
                                    "Devoro los ecos, apago las alarmas y hago que tus gritos sean inútiles. ¿Qué anomalía soy?",
                                    "Protocolo de reconocimiento: Introduce el nombre de usuario registrado de tu perfil de Explorador actual.",
                                    "Protocolo de verificación biométrica: Introduce la edad cronológica exacta de tu avatar actual.",
                                    "Soy el sistema que te sostiene, la red que conecta y el programa maestro en el que operas. ¿Quién soy?",
                                };

                                string[] respuestasLore =
                                {
                                    "iris",
                                    "tiempo",
                                    "espacio",
                                    "mente",
                                    "silencio",
                                    cadete.Nombre.ToLower(),
                                    cadete.Edad.ToString(),
                                    "nexus",
                                };

                                int indexLore = rndMinijuego.Next(preguntasLore.Length);

                                Console.WriteLine($"{magenta}[PRUEBA DE CORDURA]: Responde a la siguiente consulta del sistema:");
                                Thread.Sleep(10);
                                Console.WriteLine($"{amarillo}\"{preguntasLore[indexLore]}\"{cian}");
                                Thread.Sleep(10);

                                Console.Write("\nRespuesta: ");
                                Console.Write(verde);
                                string inputLore = Console.ReadLine().Trim().ToLower();
                                Console.Write(cian);

                                if (inputLore == respuestasLore[indexLore]) minijuegoGanado = true;
                                break;

                            case 2: // MINIJUEGO 2: Secuencias Lógicas
                                string[] secuencias =
                                {
                                    "2 - 4 - 8 - 16 - ?",          // Potencias de 2
                                    "1 - 3 - 6 - 10 - ?",          // Números triangulares (+2, +3, +4..)
                                    "0 - 1 - 1 - 2 - 3 - 5 - ?",   // Sucesión de Fibonacci
                                    "2 - 3 - 5 - 7 - 11 - ?",      // Números primos
                                    "99 - 88 - 77 - 66 - ?"        // Patrón visual descendente
                                };
                                string[] respuestasSecuencias = { "32", "15", "8", "13", "55" };

                                int indexSec = rndMinijuego.Next(secuencias.Length);

                                Console.WriteLine($"{magenta}[CALIBRACIÓN DE REACTOR]: Completa la siguiente secuencia cifrada:");
                                Thread.Sleep(10);
                                Console.WriteLine($"{amarillo}Secuencia: {secuencias[indexSec]}{cian}");
                                Thread.Sleep(10);

                                Console.Write("\nIngresa el número faltante: ");
                                Console.Write(verde);
                                string inputSec = Console.ReadLine().Trim();
                                Console.Write(cian);

                                if (inputSec == respuestasSecuencias[indexSec]) minijuegoGanado = true;
                                break;

                            case 3: // MINIJUEGO 3: Memoria Rápida
                                string[] codigosMemoria =
                                {
                                    "N-3-X-U-5",
                                    "O-M-E-G-A",
                                    "1-R-1-S",
                                    "V-0-1-D",
                                    "Q-U-A-N-T-U-M",
                                    "C-0-D-3",
                                    "A-L-P-H-A"
                                };

                                int indexMem = rndMinijuego.Next(codigosMemoria.Length);

                                Console.WriteLine($"{magenta}[FILTRO ANTIVIRUS]: Memoriza el siguiente código de autorización.");
                                Thread.Sleep(10);
                                Console.WriteLine($"El código se autodestruirá en 3 segundos");
                                DibujarPuntosSuspensivos(3);

                                Console.WriteLine($"\n{blanco}CÓDIGO: {codigosMemoria[indexMem]}{cian}");

                                // espera
                                Thread.Sleep(3000);

                                // borrar pantalla y preguntar
                                Console.Clear();
                                DibujarSeparadorAnimado(cian);
                                Console.WriteLine($"{magenta}[FILTRO ANTIVIRUS]: Código borrado de la interfaz.{cian}");
                                Thread.Sleep(10);
                                Console.Write("\nIntroduce la secuencia exacta (con guiones si los tenía): ");
                                Console.Write(verde);
                                string inputMem = Console.ReadLine().Trim().ToUpper();
                                Console.Write(cian);

                                if (inputMem == codigosMemoria[indexMem]) minijuegoGanado = true;
                                break;

                            case 4: // MINIJUEGO 4: Palabras Desordenadas
                                string[] anagramas =
                                {
                                    "A O L N A I A M",
                                    "S U E N X",
                                    "S I I R",
                                    "C D G O O I",
                                    "E D A C E T",
                                    "O P A S E I C"
                                };
                                string[] respuestasAnagramas =
                                {
                                    "anomalia",
                                    "nexus",
                                    "iris",
                                    "codigo",
                                    "cadete",
                                    "espacio"
                                };

                                int indexAna = rndMinijuego.Next(anagramas.Length);

                                Console.WriteLine($"{magenta}[SINCRONIZACIÓN CUÁNTICA]: Reconecta los datos corrompidos.");
                                Thread.Sleep(10);
                                Console.WriteLine($"{amarillo}Datos cifrados: {anagramas[indexAna]}{cian}");
                                Thread.Sleep(10);

                                Console.Write("\nIngresa la palabra correcta: ");
                                Console.Write(verde);
                                string inputAna = Console.ReadLine().Trim().ToLower();
                                Console.Write(cian);

                                if (inputAna == respuestasAnagramas[indexAna]) minijuegoGanado = true;
                                break;
                        }

                        // ==========================================
                        // RECOMPENSAAA
                        // ==========================================
                        if (minijuegoGanado)
                        {
                            cadete.Energia += 5;
                            Console.WriteLine($"\n{verde}[AUTORIZACIÓN ACEPTADA]: Extracción de energía completada con éxito.");
                            Thread.Sleep(10);
                            Console.WriteLine($"+5 Energía recuperada.{cian}");
                            Thread.Sleep(10);
                        }
                        else if (!minijuegoGanado && realidadAsignada.Estabilidad > 50)
                        {
                            cadete.Energia += 1; // premio de consuelo
                            Console.WriteLine($"\n{amarillo}[ACCESO DENEGADO]: Filtro de seguridad fallido.");
                            Thread.Sleep(10);
                            Console.WriteLine("El sistema apenas logró extraer energía (+1 Energía).");
                            Thread.Sleep(10);
                            Console.WriteLine($"La realidad es lo suficientemente estable para absorber el impacto del error.{cian}");
                            Thread.Sleep(10);
                        }
                        else if (!minijuegoGanado && realidadAsignada.Estabilidad <= 50)
                        {
                            cadete.Energia += 1; // premio de consuelo
                            realidadAsignada.Estabilidad -= 15; // castigo
                            Console.WriteLine($"\n{rojo}[ACCESO DENEGADO]: Filtro de seguridad fallido. Posible interferencia de IRIS detectada.");
                            Thread.Sleep(10);
                            Console.WriteLine("El sistema apenas logró extraer energía (+1 Energía).");
                            Thread.Sleep(10);
                            Console.WriteLine($"La anomalía local aprovechó tu vulnerabilidad, provocando un colapso parcial (-15 Estabilidad).");
                            Thread.Sleep(10);
                            // destruir un objeto al azar del inventario
                            if (cadete.Inventario.Count > 0)
                            {
                                Random rndDestruccion = new Random();
                                int indexDestruir = rndDestruccion.Next(cadete.Inventario.Count);
                                Objeto objPerdido = cadete.Inventario[indexDestruir];

                                cadete.DescartarObjeto(objPerdido);

                                Console.WriteLine($"[CATÁSTROFE]: La sobrecarga energética corrompió tu equipo. Has perdido el objeto: {magenta}{objPerdido.Nombre}{rojo}.{cian}");
                            }
                            else
                            {
                                Console.WriteLine($"[CATÁSTROFE]: La sobrecarga casi fríe tu traje. Tienes suerte de no tener objetos que perder.{cian}");
                            }
                        }
                        break;

                    case 6: // Consultar estado
                        Console.WriteLine($"{blanco}[ESTADO DEL SISTEMA]{cian}");
                        Thread.Sleep(10);
                        Console.WriteLine($"Nivel del Cadete: {verde}{cadete.Nivel}{cian}");
                        Thread.Sleep(10);
                        Console.WriteLine($"Experiencia actual: {verde}{cadete.Experiencia}/100{cian}");
                        Thread.Sleep(10);
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
                        DibujarSeparadorAnimado(cian, 55);
                        Console.WriteLine($"{blanco}         [BASE DE DATOS: MANUAL DEL EXPLORADOR]        {cian}");
                        DibujarSeparadorAnimado(cian, 55);

                        Thread.Sleep(10);
                        Console.WriteLine($"\n{magenta}1. OBJETIVO DE LA SIMULACIÓN:{cian}");
                        Thread.Sleep(10);
                        Console.WriteLine("Tu misión es adentrarte en simulaciones cuánticas inestables,");
                        Thread.Sleep(3);
                        Console.WriteLine("sobrevivir a sus peligros y mantener la ESTABILIDAD del mundo.");
                        Thread.Sleep(3);
                        Console.WriteLine("Al llegar a 100% de estabilidad, se te asigna una nueva misión.");
                        Thread.Sleep(3);
                        Console.WriteLine("Si la Estabilidad cae a 20% o menos, la ANOMALÍA IRIS tomará el control.");

                        Thread.Sleep(10);
                        Console.WriteLine($"\n{verde}2. ENERGÍA Y RECURSOS:{cian}");
                        Thread.Sleep(10);
                        Console.WriteLine($"* {blanco}Energía:{cian} Necesaria para realizar acciones. Si se agota, quedarás");
                        Thread.Sleep(3);
                        Console.WriteLine("  indefenso. Usa la opción 'Recuperar energía' para recargarla.");
                        Thread.Sleep(3);
                        Console.WriteLine($"* {blanco}Experiencia:{cian} Sube tu Nivel de Cadete al explorar realidades.");

                        Thread.Sleep(10);
                        Console.WriteLine($"\n{magenta}3. LAS 4 ANOMALÍAS:{cian}");
                        Thread.Sleep(10);
                        Console.WriteLine("Cada mundo está corrompido por una anomalía oculta:");
                        Thread.Sleep(3);
                        Console.WriteLine($"{blanco}TIEMPO, ESPACIO, MENTE o SILENCIO.{cian}");
                        Console.WriteLine("Usa la opción 'Observar realidad' para recibir pistas sensoriales");
                        Thread.Sleep(3);
                        Console.WriteLine("del entorno y deducir a qué tipo de anomalía te estás enfrentando.");
                        Thread.Sleep(3);

                        Thread.Sleep(10);
                        Console.WriteLine($"\n{verde}4. INVENTARIO Y LOOT:{cian}");
                        Thread.Sleep(10);
                        Console.WriteLine($"* {blanco}Buscar objetos:{cian} Gasta energía, pero puedes encontrar Artefactos.");
                        Thread.Sleep(3);
                        Console.WriteLine("* Tu mochila tiene capacidad limitada. Deberás descartar objetos");
                        Thread.Sleep(3);
                        Console.WriteLine("  si quieres recoger equipo nuevo.");

                        Thread.Sleep(10);
                        Console.WriteLine($"\n{magenta}5. INTERACTUAR CON LA REALIDAD (SUPERVIVENCIA):{cian}");
                        Thread.Sleep(10);
                        Console.WriteLine("Una vez que deduzcas qué anomalía afecta al mundo, elige un objeto");
                        Thread.Sleep(3);
                        Console.WriteLine("de tu inventario (leyendo su descripción) e interactúa con la realidad.");
                        Thread.Sleep(3);
                        Console.WriteLine($"* {verde}Sinergia Correcta:{cian} La Estabilidad aumenta drásticamente.");
                        Thread.Sleep(3);
                        Console.WriteLine($"* {rojo}Elección Incorrecta:{cian} La realidad empeora y pierdes Estabilidad.");

                        DibujarSeparadorAnimado(cian, 55);
                        Console.WriteLine($"{amarillo}FIN DEL ARCHIVO. Presiona cualquier tecla para volver al menú.{cian}");
                        break;

                    case 8: // Desconexión
                        Console.Write($"{blanco}[SISTEMA NEXUS]{cian} Iniciando protocolo de desconexión");
                        DibujarPuntosSuspensivos(3);
                        Thread.Sleep(10);
                        Console.Write("Guardando estado del cadete");
                        DibujarPuntosSuspensivos(3);
                        Thread.Sleep(10);
                        Console.WriteLine($"\n{verde}Desconexión exitosa. Fin de la simulación.{cian}");
                        conectado = false;
                        break;

                    case 9: // extracción (solo aparece cuando Estabilidad es 100%)
                        if (realidadAsignada.Estabilidad >= 100)
                        {
                            Console.Clear();
                            DibujarSeparadorAnimado(cian, 55);
                            Console.WriteLine($"{fondoVerde}{blanco}            [PROTOCOLO DE EXTRACCIÓN INICIADO]           {reset}");
                            DibujarSeparadorAnimado(cian, 55);

                            Console.WriteLine($"\n{blanco}[NEXUS]{cian} Sellando fisuras cuánticas en {magenta}{realidadAsignada.Nombre}{cian}...");
                            Thread.Sleep(10);
                            Console.WriteLine("La matriz espacial de este universo ha sido estabilizada por completo.");
                            Thread.Sleep(10);
                            Console.WriteLine($"La anomalía de tipo {magenta}{realidadAsignada.Anomalia}{cian} ha sido purgada. Has salvado esta realidad del colapso.");
                            Thread.Sleep(10);
                            // recompensa
                            cadete.Experiencia += 100;
                            Console.WriteLine($"\n{verde}[RECOMPENSA DE EXTRACCIÓN]: {blanco}+100 EXP{verde} obtenida.");
                            Thread.Sleep(10);
                            Console.WriteLine($"Nivel actual del Cadete: {blanco}{cadete.Nivel}{verde}.{cian}");
                            Thread.Sleep(100);
                            // nueva realidad
                            realidadAsignada = new Realidad();
                            Console.WriteLine($"\n{blanco}[SISTEMA NEXUS]{cian} Desconectando anclajes temporales...");
                            Thread.Sleep(10);
                            Console.WriteLine($"Buscando un nuevo mundo al borde del colapso...");
                            Thread.Sleep(10);
                            Console.WriteLine($"\nSincronizando nuevas coordenadas cuánticas...");
                            Thread.Sleep(10);
                            Console.WriteLine($"Nueva realidad asignada al cadete: {magenta}{realidadAsignada.Nombre}{cian}");
                            Thread.Sleep(10);
                            Console.WriteLine($"Nivel de amenaza inicial (Estabilidad): {verde}{realidadAsignada.Estabilidad}%{cian}");
                            realidadAsignada.Extraida = true;
                        }
                        else
                        {
                            // Protección por si el usuario presiona 9 cuando no debe
                            Console.WriteLine($"{rojo}ERROR DE PROTOCOLO: Extracción denegada.");
                            Thread.Sleep(10);
                            Console.WriteLine($"Para sellar un universo se requiere un 100% de Estabilidad. (Actual: {realidadAsignada.Estabilidad}%)");
                            Thread.Sleep(10);
                            Console.WriteLine($"Continúa purgando las anomalías del sector.{cian}");
                        }
                        break;

                    case 10: // Atlas de Realidades
                        {
                            AtlasRealidades.GenerarAtlas();
                            AtlasRealidades.ActualizarAtlas();
                            AtlasRealidades.MostrarAtlas();
                        }
                        break;
                    default: // Manejo de errores de entrada (Protocolo de seguridad)
                        Console.WriteLine($"{rojo}ERROR: Operación no reconocida.");
                        Thread.Sleep(10);
                        Console.WriteLine($"Penalización del sistema: -5 Estabilidad.{cian}");
                        realidadAsignada.Estabilidad -= 5;
                        break;
                }

                // EVENTO DE EMERGENCIA: ANOMALÍA IRIS
                if (conectado && realidadAsignada.Estabilidad <= 20)
                {
                    Console.Clear();
                    Console.WriteLine($"{fondoRojo}{blanco}----------------------------------------{reset}");
                    Console.WriteLine($"{fondoRojo}{blanco}        ALERTA DE INTERFERENCIA IRIS    {reset}");
                    Console.WriteLine($"{fondoRojo}{blanco}----------------------------------------{reset}");
                    Thread.Sleep(15);
                    Console.WriteLine($"{rojo}La estabilidad de la realidad está alcanzando niveles críticos.");
                    Thread.Sleep(15);
                    Console.WriteLine($"NEXUS recomienda recuperación inmediata o desconexión.");

                    // pausa dramática
                    Thread.Sleep(1500);

                    Console.WriteLine($"\n{magenta}[SISTEMA COMPROMETIDO]{rojo}");
                    Thread.Sleep(10);
                    Console.WriteLine("IRIS: TE HE ENCONTRADO, EXPLORADOR.");
                    Thread.Sleep(10);
                    Console.WriteLine("ESTA REALIDAD ME PERTENECE AHORA. RÍNDETE O ENFRENTA EL VACÍO.");
                    Thread.Sleep(10);
                    // sacrificio de energía o perder
                    Console.Write($"\n{amarillo}NEXUS: ¿Transferir toda tu energía restante ({cadete.Energia}) para forzar un reinicio y repeler a IRIS? (S/N): {verde}");
                    Thread.Sleep(10);
                    string decisionIris = Console.ReadLine().Trim().ToUpper();
                    Console.Write(cian);

                    if (decisionIris == "S")
                    {
                        Console.WriteLine($"\n{blanco}[NEXUS]{cian} Ejecutando purga de emergencia");
                        DibujarPuntosSuspensivos(3);
                        Thread.Sleep(1000);

                        // reiniciar energía a cambio de estabilidad
                        cadete.Energia = 0;
                        realidadAsignada.Estabilidad += 15;

                        Console.WriteLine($"{verde}Purga exitosa. IRIS repelida temporalmente.");
                        Console.WriteLine($"Energía agotada por completo. Estabilidad restaurada levemente (+15).{cian}");
                    }
                    else
                    {
                        Console.WriteLine($"\n{magenta}IRIS: {rojo}ENTONCES DESAPARECE EN LA NADA.");
                        Thread.Sleep(1000);

                        // perder juego
                        Console.WriteLine($"{blanco}[SISTEMA NEXUS]{rojo} Conexión cortada remotamente.");
                        Console.WriteLine($"Simulación abortada por falla de seguridad.{reset}");
                        conectado = false;
                    }
                }

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
    }

    class AtlasRealidades
    {
        private static string[,] Atlas = new string[18, 12];
        private static List<(int, int)> CoordenadasOcupadas = new List<(int, int)>();
        private AtlasRealidades() { }

        string reset = "\u001b[0m";
        string colorTiempo = "\u001b[92m";   // Cuadrante 1 (Verde)
        string colorEspacio = "\x1b[38;5;20m";  // Cuadrante 2 (Cian)
        string colorMente = "\x1b[33m";    // Cuadrante 3 (Amarillo)
        string colorSilencio = "\u001b[95m"; // Cuadrante 4 (Magenta)

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

                        string gris = "\u001b[90m";
                        Atlas[x, y] = $"{gris}[?]";
                    }
                }
            }
        }
        public static void MostrarAtlas()
        {
            for (int i = 0; i < Atlas.GetLength(0); i++)
            {
                for (int j = 0; j < Atlas.GetLength(1); j++)
                {
                    Console.Write(Atlas[i, j]);
                }
                Console.WriteLine();
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
