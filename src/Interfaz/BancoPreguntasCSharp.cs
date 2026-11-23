using System;
using System.Collections.Generic;

namespace Interfaz
{
    public class PreguntaCSharp
    {
        public string Categoria { get; set; }
        public string Enunciado { get; set; }
        public string CodigoEjemplo { get; set; }
        public string[] Opciones { get; set; }
        public int IndiceCorrecto { get; set; }
        public string Explicacion { get; set; }
        public string Dificultad { get; set; }
    }

    public class BancoPreguntasCSharp
    {
        private readonly List<PreguntaCSharp> todasLasPreguntas;
        private readonly Queue<PreguntaCSharp> colaBarajada = new Queue<PreguntaCSharp>();
        private readonly Random rnd = new Random();
        private PreguntaCSharp ultimaPregunta = null;

        public BancoPreguntasCSharp()
        {
            todasLasPreguntas = CrearBancoInicial();
            RellenarYBarajarCola();
        }

        public void CargarPreguntasDesdeLista(List<PreguntaCSharp> nuevas)
        {
            if (nuevas != null && nuevas.Count > 0)
            {
                todasLasPreguntas.Clear();
                todasLasPreguntas.AddRange(nuevas);
                RellenarYBarajarCola();
            }
        }

        public PreguntaCSharp ObtenerSiguientePregunta()
        {
            if (colaBarajada.Count == 0)
                RellenarYBarajarCola();

            PreguntaCSharp siguiente = colaBarajada.Dequeue();
            ultimaPregunta = siguiente;
            return siguiente;
        }

        private void RellenarYBarajarCola()
        {
            List<PreguntaCSharp> copia = new List<PreguntaCSharp>(todasLasPreguntas);

            // Algoritmo Fisher-Yates
            for (int i = copia.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                PreguntaCSharp temp = copia[i];
                copia[i] = copia[j];
                copia[j] = temp;
            }

            // Evitar repetición inmediata entre el final de una ronda y el inicio de la siguiente
            if (ultimaPregunta != null && copia.Count > 1 && ReferenceEquals(copia[0], ultimaPregunta))
            {
                PreguntaCSharp tmp = copia[0];
                copia[0] = copia[copia.Count - 1];
                copia[copia.Count - 1] = tmp;
            }

            colaBarajada.Clear();
            foreach (PreguntaCSharp p in copia)
                colaBarajada.Enqueue(p);
        }

        private List<PreguntaCSharp> CrearBancoInicial()
        {
            return new List<PreguntaCSharp>
            {
                // =============================================================
                // CATEGORÍA 1: SINTAXIS Y TIPOS DE DATOS (7 preguntas)
                // =============================================================
                new PreguntaCSharp
                {
                    Categoria = "Sintaxis y Tipos de Datos",
                    Dificultad = "Básico",
                    Enunciado = "¿Qué tipo de dato infiere el compilador para la variable 'energia' al usar 'var'?",
                    CodigoEjemplo = "var energia = 10;",
                    Opciones = new[] { "object (tipado dinámico)", "int (tipado estático en compilación)", "dynamic (resuelto en ejecución)", "double por defecto" },
                    IndiceCorrecto = 1,
                    Explicacion = "'var' realiza inferencia de tipos en tiempo de compilación: 'energia' queda fuertemente tipada como int y no puede cambiar de tipo después."
                },
                new PreguntaCSharp
                {
                    Categoria = "Sintaxis y Tipos de Datos",
                    Dificultad = "Básico",
                    Enunciado = "¿Qué valor queda almacenado en 'resultado' tras ejecutar este fragmento?",
                    CodigoEjemplo = "int a = 7;\nint b = 2;\ndouble resultado = a / b;",
                    Opciones = new[] { "3.0", "3.5", "4.0", "Error de compilación" },
                    IndiceCorrecto = 0,
                    Explicacion = "Como 'a' y 'b' son int, C# ejecuta primero una división entera (7 / 2 = 3) y recién después convierte ese 3 a double (3.0). Para obtener 3.5 habría que hacer (double)a / b."
                },
                new PreguntaCSharp
                {
                    Categoria = "Sintaxis y Tipos de Datos",
                    Dificultad = "Básico",
                    Enunciado = "¿Qué hace el operador de coalescencia nula '??' en C#?",
                    CodigoEjemplo = "string alias = nombreIngresado ?? \"Cadete\";",
                    Opciones = new[] { "Compara si dos cadenas son iguales ignorando mayúsculas", "Lanza una excepción si la variable es nula", "Devuelve el operando derecho cuando el izquierdo es null", "Convierte cualquier objeto a string" },
                    IndiceCorrecto = 2,
                    Explicacion = "El operador '??' evalúa el operando izquierdo; si no es null lo devuelve, y si es null retorna el valor alternativo de la derecha."
                },
                new PreguntaCSharp
                {
                    Categoria = "Sintaxis y Tipos de Datos",
                    Dificultad = "Básico",
                    Enunciado = "¿Cuál es la forma correcta de usar interpolación de cadenas en C#?",
                    CodigoEjemplo = "int nivel = 5;",
                    Opciones = new[] { "\"Nivel: {nivel}\"", "$\"Nivel: {nivel}\"", "@\"Nivel: {nivel}\"", "#\"Nivel: {nivel}\"" },
                    IndiceCorrecto = 1,
                    Explicacion = "El prefijo '$' delante de las comillas habilita la interpolación de cadenas, permitiendo evaluar expresiones dentro de llaves {}."
                },
                new PreguntaCSharp
                {
                    Categoria = "Sintaxis y Tipos de Datos",
                    Dificultad = "Intermedio",
                    Enunciado = "¿Cuál es la diferencia clave entre 'const' y 'readonly' en C#?",
                    CodigoEjemplo = "const int MAX = 100;\nreadonly string id = Guid.NewGuid().ToString();",
                    Opciones = new[] { "No existe diferencia, son sinónimos", "'const' solo funciona con strings y 'readonly' con enteros", "'readonly' puede modificarse en cualquier método de la clase", "'const' se define en compilación; 'readonly' puede asignarse en el constructor" },
                    IndiceCorrecto = 3,
                    Explicacion = "Un campo 'const' debe conocerse en tiempo de compilación, mientras que 'readonly' puede recibir un valor en tiempo de ejecución dentro del constructor."
                },
                new PreguntaCSharp
                {
                    Categoria = "Sintaxis y Tipos de Datos",
                    Dificultad = "Básico",
                    Enunciado = "¿Qué ocurre al ejecutar 'int.TryParse' con una cadena no numérica?",
                    CodigoEjemplo = "bool ok = int.TryParse(\"NEXUS\", out int valor);",
                    Opciones = new[] { "ok es false y valor queda en 0, sin lanzar excepción", "Se lanza FormatException inmediatamente", "ok es true y valor toma el código ASCII", "El programa se detiene por desbordamiento" },
                    IndiceCorrecto = 0,
                    Explicacion = "TryParse intenta convertir de forma segura: si falla, devuelve false y asigna el valor por defecto del tipo (0 para int) sin lanzar excepciones."
                },
                new PreguntaCSharp
                {
                    Categoria = "Sintaxis y Tipos de Datos",
                    Dificultad = "Intermedio",
                    Enunciado = "¿Qué tipo numérico de C# es el recomendado para cálculos financieros o monetarios exactos?",
                    CodigoEjemplo = "",
                    Opciones = new[] { "float", "double", "decimal", "long" },
                    IndiceCorrecto = 2,
                    Explicacion = "'decimal' usa representación en base 10 de 128 bits, evitando los pequeños errores de redondeo binario propios de float y double."
                },

                // =============================================================
                // CATEGORÍA 2: PROGRAMACIÓN ORIENTADA A OBJETOS (7 preguntas)
                // =============================================================
                new PreguntaCSharp
                {
                    Categoria = "Programación Orientada a Objetos",
                    Dificultad = "Básico",
                    Enunciado = "¿Cómo funciona la herencia de clases e interfaces en C#?",
                    CodigoEjemplo = "class Usuario : Entidad, IGuardable, ICombate { }",
                    Opciones = new[] { "Una clase puede heredar de múltiples clases base simultáneamente", "Herencia simple de clase base, pero múltiple implementación de interfaces", "Una clase solo puede implementar una interfaz a la vez", "Las interfaces no pueden ser implementadas junto a una clase base" },
                    IndiceCorrecto = 1,
                    Explicacion = "C# restringe la herencia a una sola clase base para evitar ambigüedades (problema del diamante), pero permite implementar tantas interfaces como se necesiten."
                },
                new PreguntaCSharp
                {
                    Categoria = "Programación Orientada a Objetos",
                    Dificultad = "Intermedio",
                    Enunciado = "¿Para qué se combinan las palabras clave 'virtual' y 'override'?",
                    CodigoEjemplo = "public virtual void Atacar() { }\npublic override void Atacar() { }",
                    Opciones = new[] { "Para permitir que una clase derivada redefina un método (polimorfismo)", "Para ocultar una clase del ensamblado", "Para impedir que una clase sea heredada", "Para ejecutar métodos en paralelo en otro hilo" },
                    IndiceCorrecto = 0,
                    Explicacion = "Marcar un método como 'virtual' en la clase base autoriza a las subclases a proporcionar su propia implementación mediante 'override', habilitando el polimorfismo."
                },
                new PreguntaCSharp
                {
                    Categoria = "Programación Orientada a Objetos",
                    Dificultad = "Básico",
                    Enunciado = "¿Qué efecto tiene declarar una propiedad con 'public string Nombre { get; private set; }'?",
                    CodigoEjemplo = "public string Nombre { get; private set; }",
                    Opciones = new[] { "Nadie puede leer ni escribir la propiedad", "Cualquier clase externa puede modificar Nombre libremente", "Cualquiera puede leer Nombre, pero solo la propia clase puede modificarlo", "La propiedad se vuelve estática automáticamente" },
                    IndiceCorrecto = 2,
                    Explicacion = "Es un patrón estándar de encapsulamiento: expone el valor para lectura pública mientras protege su modificación interna dentro de la clase."
                },
                new PreguntaCSharp
                {
                    Categoria = "Programación Orientada a Objetos",
                    Dificultad = "Básico",
                    Enunciado = "¿Qué nivel de acceso otorga el modificador 'protected' a un miembro de clase?",
                    CodigoEjemplo = "protected int energiaBase = 10;",
                    Opciones = new[] { "Solo visible dentro de la misma clase exacta", "Visible en la misma clase y en sus clases derivadas", "Visible para cualquier proyecto externo", "Solo visible dentro de métodos static" },
                    IndiceCorrecto = 1,
                    Explicacion = "'protected' permite que las subclases accedan a atributos o métodos heredados sin exponerlos públicamente al resto del programa."
                },
                new PreguntaCSharp
                {
                    Categoria = "Programación Orientada a Objetos",
                    Dificultad = "Básico",
                    Enunciado = "¿Qué característica define a un método o propiedad declarada como 'static'?",
                    CodigoEjemplo = "AtlasRealidades.GenerarAtlas();",
                    Opciones = new[] { "Solo puede llamarse una vez durante la ejecución", "Requiere crear una instancia con 'new' antes de usarse", "Se guarda automáticamente en disco duro", "Pertenece al tipo/clase en sí y no a una instancia específica" },
                    IndiceCorrecto = 3,
                    Explicacion = "Los miembros 'static' se invocan directamente sobre el nombre de la clase (ej. AtlasRealidades.GenerarAtlas()) y se comparten en toda la aplicación."
                },
                new PreguntaCSharp
                {
                    Categoria = "Programación Orientada a Objetos",
                    Dificultad = "Intermedio",
                    Enunciado = "¿Cuál afirmación es correcta respecto a una 'abstract class' en C#?",
                    CodigoEjemplo = "public abstract class EntidadBase { public abstract void Actuar(); }",
                    Opciones = new[] { "No puede instanciarse con 'new' y puede combinar métodos abstractos y concretos", "No puede contener campos ni constructores", "Todos sus métodos deben ser privados", "Es idéntica a una clase 'sealed'" },
                    IndiceCorrecto = 0,
                    Explicacion = "Una clase abstracta actúa como plantilla base: no se instancia directamente, pero sí puede tener estado, constructores y lógica compartida."
                },
                new PreguntaCSharp
                {
                    Categoria = "Programación Orientada a Objetos",
                    Dificultad = "Intermedio",
                    Enunciado = "¿Por qué suele añadirse un constructor vacío 'public Usuario() { }' al usar System.Text.Json?",
                    CodigoEjemplo = "public Usuario() { Inventario = new List<Objeto>(); }",
                    Opciones = new[] { "Porque C# prohíbe tener constructores con parámetros", "Para que el programa compile más rápido", "Para que el deserializador JSON pueda instanciar el objeto y llenar sus propiedades", "Para eliminar la necesidad de usar propiedades públicas" },
                    IndiceCorrecto = 2,
                    Explicacion = "Por defecto, JsonSerializer.Deserialize busca un constructor público sin parámetros para crear el objeto y luego asignar cada propiedad desde el JSON."
                },

                // =============================================================
                // CATEGORÍA 3: COLECCIONES Y LINQ (7 preguntas)
                // =============================================================
                new PreguntaCSharp
                {
                    Categoria = "Colecciones y LINQ",
                    Dificultad = "Básico",
                    Enunciado = "En 'Queue<EntidadIris>', ¿qué orden sigue la colección y qué hace el método 'Peek()'?",
                    CodigoEjemplo = "Queue<EntidadIris> cola = new Queue<EntidadIris>();\nvar actual = cola.Peek();",
                    Opciones = new[] { "LIFO; Peek() elimina el último elemento", "FIFO; Peek() devuelve el primer elemento sin quitarlo de la cola", "Orden aleatorio; Peek() vacía la cola", "FIFO; Peek() extrae y borra el primer elemento" },
                    IndiceCorrecto = 1,
                    Explicacion = "Queue<T> es FIFO (First In, First Out). Peek() inspecciona el elemento al frente sin retirarlo, a diferencia de Dequeue() que sí lo extrae."
                },
                new PreguntaCSharp
                {
                    Categoria = "Colecciones y LINQ",
                    Dificultad = "Básico",
                    Enunciado = "¿Cuál es la principal ventaja de 'Dictionary<TKey, TValue>' frente a buscar en una 'List<T>'?",
                    CodigoEjemplo = "Dictionary<string, int> puntajes = new Dictionary<string, int>();",
                    Opciones = new[] { "Acceso promedio O(1) mediante clave única hash", "Permite almacenar claves duplicadas sin restricción", "Consume siempre menos memoria que un arreglo simple", "Ordena automáticamente las claves alfabéticamente" },
                    IndiceCorrecto = 0,
                    Explicacion = "Dictionary<TKey, TValue> usa una tabla hash interna, permitiendo búsquedas por clave en tiempo promedio constante O(1) en vez de recorrer toda la lista O(n)."
                },
                new PreguntaCSharp
                {
                    Categoria = "Colecciones y LINQ",
                    Dificultad = "Básico",
                    Enunciado = "¿Qué realiza el operador LINQ 'Where' sobre una colección?",
                    CodigoEjemplo = "var fuertes = enemigos.Where(e => e.DañoBase >= 13);",
                    Opciones = new[] { "Ordena la colección de mayor a menor", "Modifica directamente la lista original borrando elementos", "Filtra y devuelve los elementos que cumplen la condición booleana", "Cuenta cuántos elementos existen y devuelve un int" },
                    IndiceCorrecto = 2,
                    Explicacion = "Where recibe un predicado (expresión lambda que retorna bool) y produce una secuencia filtrada sin mutar la colección original."
                },
                new PreguntaCSharp
                {
                    Categoria = "Colecciones y LINQ",
                    Dificultad = "Intermedio",
                    Enunciado = "¿Cuándo se ejecuta realmente una consulta LINQ con ejecución diferida como 'Where' o 'Select'?",
                    CodigoEjemplo = "var consulta = objetos.Where(o => o.Usos > 1);",
                    Opciones = new[] { "Exactamente en la línea donde se declara la variable 'consulta'", "Al compilar el proyecto en Visual Studio", "Solo cuando se cierra el programa", "Cuando se itera (foreach) o se materializa (ToList(), Count(), First())" },
                    IndiceCorrecto = 3,
                    Explicacion = "La ejecución diferida (deferred execution) posterga la evaluación hasta que los datos son efectivamente recorridos o convertidos con ToList()/ToArray()."
                },
                new PreguntaCSharp
                {
                    Categoria = "Colecciones y LINQ",
                    Dificultad = "Básico",
                    Enunciado = "¿Qué devuelve el método LINQ 'Select' en el siguiente ejemplo?",
                    CodigoEjemplo = "var nombres = inventario.Select(obj => obj.Nombre);",
                    Opciones = new[] { "El primer objeto del inventario", "Una secuencia proyectada con los nombres (IEnumerable<string>)", "Un booleano indicando si el inventario tiene elementos", "La cantidad total de usos disponibles" },
                    IndiceCorrecto = 1,
                    Explicacion = "Select transforma (proyecta) cada elemento de entrada en un nuevo valor o tipo; en este caso extrae solo la propiedad Nombre de cada Objeto."
                },
                new PreguntaCSharp
                {
                    Categoria = "Colecciones y LINQ",
                    Dificultad = "Intermedio",
                    Enunciado = "¿Qué devuelve 'FirstOrDefault()' si ningún elemento de una 'List<Objeto>' cumple la condición?",
                    CodigoEjemplo = "Objeto hallado = inventario.FirstOrDefault(o => o.Usos > 99);",
                    Opciones = new[] { "null (valor por defecto para tipos por referencia)", "Lanza InvalidOperationException", "Crea un Objeto nuevo automáticamente", "Devuelve el último elemento de la lista" },
                    IndiceCorrecto = 0,
                    Explicacion = "A diferencia de First() que lanza excepción si no hay coincidencias, FirstOrDefault() devuelve el valor por defecto del tipo (null para clases, 0 para int)."
                },
                new PreguntaCSharp
                {
                    Categoria = "Colecciones y LINQ",
                    Dificultad = "Básico",
                    Enunciado = "¿Cuál es la diferencia principal entre un arreglo 'Objeto[]' y una 'List<Objeto>'?",
                    CodigoEjemplo = "",
                    Opciones = new[] { "Los arreglos no permiten acceso por índice", "List<T> solo puede guardar enteros y cadenas", "El arreglo tiene tamaño fijo; List<T> crece y decrece dinámicamente", "List<T> no puede recorrerse con foreach" },
                    IndiceCorrecto = 2,
                    Explicacion = "Un arreglo en C# tiene longitud fija al crearse, mientras que List<T> administra un arreglo interno que se redimensiona automáticamente al usar Add/Remove."
                },

                // =============================================================
                // CATEGORÍA 4: EXCEPCIONES, MEMORIA Y .NET (7 preguntas)
                // =============================================================
                new PreguntaCSharp
                {
                    Categoria = "Excepciones, Memoria y .NET",
                    Dificultad = "Básico",
                    Enunciado = "En un bloque 'try / catch / finally', ¿cuándo se ejecuta el bloque 'finally'?",
                    CodigoEjemplo = "try { Abrir(); } catch { Log(); } finally { Cerrar(); }",
                    Opciones = new[] { "Únicamente si ocurrió una excepción", "Siempre, tanto si hubo excepción como si todo terminó bien", "Solo cuando no ocurre ningún error", "Únicamente en compilación Debug" },
                    IndiceCorrecto = 1,
                    Explicacion = "El bloque 'finally' se ejecuta incondicionalmente al salir del try/catch, siendo ideal para cerrar archivos, conexiones o liberar recursos."
                },
                new PreguntaCSharp
                {
                    Categoria = "Excepciones, Memoria y .NET",
                    Dificultad = "Intermedio",
                    Enunciado = "¿Por qué es buena práctica usar 'using' al crear objetos GDI+ como 'Pen', 'Brush' o 'Font'?",
                    CodigoEjemplo = "using Pen lapiz = new Pen(Color.Cyan, 2f);",
                    Opciones = new[] { "Garantiza llamar a Dispose() automáticamente y evita fugas de memoria", "Hace que el dibujo se ejecute en la GPU de forma directa", "Aumenta la resolución del monitor", "Evita tener que importar System.Drawing" },
                    IndiceCorrecto = 0,
                    Explicacion = "Pen, Brush y Font envuelven recursos nativos de Windows (GDI+) e implementan IDisposable; 'using' asegura su liberación inmediata al salir del ámbito."
                },
                new PreguntaCSharp
                {
                    Categoria = "Excepciones, Memoria y .NET",
                    Dificultad = "Intermedio",
                    Enunciado = "¿Cómo se comporta la asignación entre variables de un tipo 'struct' frente a un tipo 'class'?",
                    CodigoEjemplo = "Point p1 = new Point(1, 2); Point p2 = p1;\nUsuario u1 = new Usuario(); Usuario u2 = u1;",
                    Opciones = new[] { "Ambos copian solo la dirección de memoria", "Ambos clonan todos los datos de forma independiente", "struct copia el valor completo; class copia la referencia al mismo objeto", "struct siempre vive en disco y class en RAM" },
                    IndiceCorrecto = 2,
                    Explicacion = "struct es tipo por valor (modificar p2 no afecta a p1), mientras que class es tipo por referencia (u1 y u2 apuntan al mismo objeto en el heap)."
                },
                new PreguntaCSharp
                {
                    Categoria = "Excepciones, Memoria y .NET",
                    Dificultad = "Básico",
                    Enunciado = "¿Qué causa una 'NullReferenceException' en tiempo de ejecución?",
                    CodigoEjemplo = "Usuario cadete = null;\nstring n = cadete.Nombre;",
                    Opciones = new[] { "Dividir un entero entre cero", "Acceder a un índice negativo de un arreglo", "Escribir un nombre de variable mal escrito", "Intentar acceder a una propiedad o método de una variable que vale null" },
                    IndiceCorrecto = 3,
                    Explicacion = "NullReferenceException ocurre al desreferenciar una variable de tipo referencia que no apunta a ninguna instancia válida en memoria."
                },
                new PreguntaCSharp
                {
                    Categoria = "Excepciones, Memoria y .NET",
                    Dificultad = "Intermedio",
                    Enunciado = "¿Para qué sirve el atributo '[JsonIgnore]' sobre la propiedad 'MapaLocal' en NEXUS?",
                    CodigoEjemplo = "[JsonIgnore]\npublic char[,] MapaLocal { get; private set; }",
                    Opciones = new[] { "Para cifrar la propiedad dentro del archivo JSON", "Para excluir esa propiedad al serializar y deserializar con System.Text.Json", "Para convertir la matriz 2D en texto plano automáticamente", "Para evitar que el compilador verifique errores de tipo" },
                    IndiceCorrecto = 1,
                    Explicacion = "System.Text.Json no serializa arreglos multidimensionales (char[,]) directamente; [JsonIgnore] indica que esa propiedad debe omitirse en el JSON."
                },
                new PreguntaCSharp
                {
                    Categoria = "Excepciones, Memoria y .NET",
                    Dificultad = "Básico",
                    Enunciado = "¿Cuál es la función principal del Garbage Collector (GC) en .NET?",
                    CodigoEjemplo = "",
                    Opciones = new[] { "Liberar automáticamente la memoria administrada de objetos que ya no tienen referencias", "Eliminar archivos temporales del escritorio de Windows", "Detectar errores de sintaxis antes de compilar", "Comprimir imágenes PNG dentro de la carpeta Assets" },
                    IndiceCorrecto = 0,
                    Explicacion = "El GC rastrea objetos en el heap administrado y recupera la memoria de aquellos que ya no son alcanzables desde el código activo."
                },
                new PreguntaCSharp
                {
                    Categoria = "Excepciones, Memoria y .NET",
                    Dificultad = "Básico",
                    Enunciado = "¿Por qué conviene usar 'Path.Combine(carpeta, archivo)' en lugar de concatenar cadenas con '+'?",
                    CodigoEjemplo = "string ruta = Path.Combine(directorioSaves, \"perfil.json\");",
                    Opciones = new[] { "Porque crea automáticamente el archivo en el disco", "Porque valida que el JSON no esté corrupto", "Porque gestiona separadores de directorio de forma segura y evita barras mal formadas", "Porque otorga permisos de administrador en Program Files" },
                    IndiceCorrecto = 2,
                    Explicacion = "Path.Combine inserta el separador correcto del sistema operativo y evita errores comunes por barras faltantes o duplicadas."
                }
            };
        }
    }
}