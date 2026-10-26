# 🌐 NEXUS - Sistema de Gestión Centralizada

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![Visual Studio](https://img.shields.io/badge/Visual%20Studio-5C2D91.svg?style=for-the-badge&logo=visual-studio&logoColor=white)
![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)

**NEXUS** es un proyecto de software desarrollado en C# (.NET) como evidencia de dominio absoluto de los paradigmas de la Programación Orientada a Objetos (POO). Este repositorio sirve como entrega oficial y base de conocimiento para el **Hito 3 de Programación II**.

## 🎯 Propósito del Proyecto
El sistema NEXUS ha sido diseñado para abandonar la secuencialidad de la programación estructurada y adoptar una arquitectura basada en objetos, donde la información y los comportamientos están rigurosamente encapsulados. El sistema permite la creación, gestión, filtrado y polimorfismo de entidades de negocio utilizando colecciones dinámicas y consultas en memoria.

## 🏗️ Arquitectura y Tecnologías
- **Lenguaje:** C# (Microsoft .NET)
- **Paradigma:** Programación Orientada a Objetos (POO)
- **Gestión de Datos:** Colecciones Genéricas (`List<T>`) y LINQ (Language Integrated Query)
- **Control de Versiones:** Git (GitHub Desktop)
- **Entorno IDE:** Visual Studio Community

## 📂 Estructura del Repositorio
El proyecto sigue una estructura limpia, omitiendo binarios temporales mediante un archivo `.gitignore` configurado específicamente para C# y Visual Studio.

```text
📦 NEXUS
 ┣ 📂 docs/                                # Documentación de diseño, arquitectura y manuales
 ┃ ┣ 📂 evidencias/                        # Bocetos de interfaces y wireframes
 ┃ ┃ ┗ 📜bocetos-de-interfaces-graficas.md
 ┃ ┣ 📜 analisis-tecnico.md                # Alternativas evaluadas, decisiones y fuentes (IA)
 ┃ ┣ 📜 arquitectura-actual.md             # Estructura, funcionalidades y limitaciones de H3
 ┃ ┣ 📜 arquitectura-propuesta.md          # Separación en capas para la versión con UI
 ┃ ┣ 📜 diseno-del-juego.md                # Visión, ciclo principal y mecánicas del juego
 ┃ ┣ 📜 modelo-de-datos-propuesto.md       # Entidades y relaciones para SQL Server
 ┃ ┗ 📜 NEXUS_Manual_Tecnico               # Manual técnico, diagramas y guía de estudio H3 original
 ┣ 📂 src/                                 # Código fuente del proyecto
 ┃ ┣ 📂 Models/                            # Entidades del sistema (Clases base, interfaces, derivadas)
 ┃ ┣ 📂 Managers/                          # Lógica de negocio y repositorios de listas
 ┃ ┣ 📜 Program.cs                         # Punto de entrada de la aplicación
 ┃ ┗ 📜 NEXUS.csproj                       # Archivo de configuración del proyecto C#
 ┣ 📜 .gitignore                           # Reglas de exclusión (bin, obj, .vs)
 ┣ 📜 NEXUS.sln                            # Solución principal de Visual Studio
 ┗ 📜 README.md                            # Documentación principal del repositorio
```

## 🚀 Guía de Ejecución

1. **Clonar el repositorio:**
   Abre tu terminal o GitHub Desktop y ejecuta:
   ```bash
   git clone https://github.com/sebastianzq3/NEXUS.git
   ```
2. **Abrir la Solución:**
   Navega a la carpeta clonada y abre el archivo `NEXUS.sln` utilizando **Visual Studio Community**.
3. **Compilación:**
   Selecciona `Compilar > Compilar solución` (o presiona `Ctrl+Shift+B`) para restaurar paquetes y verificar referencias.
4. **Ejecución:**
   Presiona `F5` o haz clic en "Iniciar" para desplegar la consola/interfaz de NEXUS.

## 🧠 Documentación y Evolución del Proyecto (Hito 3 y Final)
La carpeta `docs/` contiene el **Manual Técnico Oficial de NEXUS** junto con la **nueva documentación de diseño y arquitectura** exigida para la evolución gráfica del proyecto.

**Fase de Consola y Guía de Estudio (Hito 3)**
El manual original no solo explica el funcionamiento de este código, sino que actúa como una guía de estudio intensiva abordando:
- Diferencias entre POO y Programación Estructurada.
- Manejo de memoria (Objetos vs Clases, Instanciación).
- Herencia múltiple de interfaces vs Herencia simple de clases.
- Ejecución diferida en colecciones con LINQ.
- Banco de preguntas universitarias resueltas.

**Fase de Expansión Gráfica (CONSTRAINTS - Evaluación Final)**
Los nuevos documentos Markdown establecen la ruta técnica para migrar NEXUS hacia **Windows Forms** y **SQL Server**:
- `diseno-del-juego.md` y `evidencias/`: Visión de la nueva experiencia, mecánicas, ciclo principal de juego y bocetos (wireframes) de las futuras pantallas.
- `arquitectura-actual.md` y `arquitectura-propuesta.md`: Diagnóstico del monolitismo en consola y propuesta estructural separada en capas (UI, Lógica, Datos).
- `modelo-de-datos-propuesto.md`: Diseño relacional de entidades para persistir el progreso multiverso de los cadetes de forma segura.
- `analisis-tecnico.md`: Evaluación de alternativas, ventajas/desventajas, decisiones justificadas y verificación de información obtenida mediante IA.

---
*Desarrollado para la asignatura de Programación II. Misión: De Código a Conocimiento.*