# 🌐 NEXUS - Sistema de Simulación y Gestión Multiversal

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![Windows Forms](https://img.shields.io/badge/Windows%20Forms-0078D6?style=for-the-badge&logo=windows&logoColor=white)
![Visual Studio](https://img.shields.io/badge/Visual%20Studio-5C2D91.svg?style=for-the-badge&logo=visual-studio&logoColor=white)
![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)

**NEXUS** es un proyecto de software y videojuego RPG táctico de supervivencia desarrollado en C# (.NET) como evidencia de aplicación avanzada de los paradigmas de la Programación Orientada a Objetos (POO) y programación orientada a eventos en interfaces gráficas (**Windows Forms**). Este repositorio sirve como entrega oficial y base de conocimiento para **Programación II**.

## 🎯 Propósito del Proyecto
El sistema NEXUS ha evolucionado desde un simulador secuencial en consola hacia una aplicación gráfica interactiva de escritorio basada en **Windows Forms y renderizado GDI+**. Combina encapsulamiento riguroso, herencia, colecciones dinámicas, persistencia multi-slot en JSON y una máquina de estados táctica que alterna entre la gestión en campamento y la exploración en cuadrícula $40 \times 20$.

## 🏗️ Arquitectura y Tecnologías
- **Lenguaje y Runtime:** C# (Microsoft .NET)
- **Interfaz Gráfica (GUI):** Windows Forms con renderizado 2D GDI+ (`BufferedCanvas` con `DoubleBuffered = true`)
- **Paradigma:** Programación Orientada a Objetos (POO) y Arquitectura Orientada a Eventos (*Event-Driven*)
- **Gestión de Datos:** Colecciones Genéricas (`List<T>`, `Queue<T>`), serialización `System.Text.Json` y diseño relacional para SQL Server
- **Control de Versiones:** Git (GitHub Desktop)
- **Entorno IDE:** Visual Studio Community

## 📂 Estructura del Repositorio
El proyecto mantiene una organización modular dentro de la solución principal, separando la documentación técnica, la versión original de consola (H3) y el proyecto gráfico en Windows Forms (`Interfaz`):

## 📂 Estructura del Repositorio
El proyecto sigue una estructura limpia, omitiendo binarios temporales (`bin`, `obj`, `.vs`, `*.user`) mediante un archivo `.gitignore` configurado específicamente para C# y Visual Studio.

```text
📦 NEXUS
 ┣ 📂 docs/                                # Documentación de diseño, arquitectura y manuales
 ┃ ┣ 📂 evidencias/                        # Bocetos de interfaces y wireframes
 ┃ ┃ ┗ 📜 bocetos-de-interfaces-graficas.md
 ┃ ┣ 📜 analisis-tecnico.md                # Alternativas evaluadas, decisiones y fuentes (IA)
 ┃ ┣ 📜 arquitectura-actual.md             # Arquitectura Event-Driven en Windows Forms y legado H3
 ┃ ┣ 📜 arquitectura-propuesta.md          # Separación en capas para la versión con UI
 ┃ ┣ 📜 diseno-del-juego.md                # Visión, ciclo principal y mecánicas del juego
 ┃ ┣ 📜 modelo-de-datos-propuesto.md       # Entidades y relaciones para SQL Server
 ┃ ┗ 📜 NEXUS_Manual_Tecnico               # Manual técnico, diagramas y guía de estudio H3 original
 ┣ 📂 src/                                 # Código fuente y solución principal
 ┃ ┣ 📂 Interfaz/                          # Proyecto gráfico en Windows Forms (GUI)
 ┃ ┃ ┣ 📜 Form1.cs                         # Lógica de eventos, renderizado GDI+, modales y dominio
 ┃ ┃ ┣ 📜 Form1.Designer.cs                # Diseño de controles UI, HUD, BufferedCanvas y Panel Táctico
 ┃ ┃ ┣ 📜 Form1.resx                       # Recursos del formulario principal
 ┃ ┃ ┣ 📜 Interfaz.csproj                  # Archivo de configuración del proyecto Windows Forms
 ┃ ┃ ┗ 📜 Program.cs                       # Punto de entrada [STAThread] de la aplicación gráfica
 ┃ ┣ 📂 NEXUS/                             # Proyecto original de consola (Hito 3)
 ┃ ┃ ┣ 📂 Properties/                      # Propiedades de ensamblado del proyecto
 ┃ ┃ ┣ 📜 NEXUS.csproj                     # Archivo de configuración del proyecto de consola
 ┃ ┃ ┗ 📜 Program.cs                       # Código fuente del simulador en consola ANSI
 ┃ ┗ 📜 NEXUS.sln                          # Solución principal de Visual Studio
 ┣ 📜 .gitignore                           # Reglas de exclusión (bin, obj, .vs)
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
3. **Configurar Proyecto de Inicio:**
   En el Explorador de Soluciones, haz clic derecho sobre el proyecto **`Interfaz`** y selecciona **"Establecer como proyecto de inicio"** (*Set as Startup Project*).
4. **Compilación y Ejecución:**
   Presiona `Ctrl+Shift+B` para compilar la solución y `F5` para iniciar la interfaz gráfica de NEXUS.

## 🧠 Documentación y Evolución del Proyecto
La carpeta `docs/` documenta tanto los fundamentos teóricos del Hito 3 como la transición arquitectónica hacia Windows Forms:

- `diseno-del-juego.md` y `evidencias/`: Ciclo principal de juego dividido en **Modo Campamento (Mapa Congelado)** y **Modo Exploración**, reglas contextuales del Panel Táctico y modales interactivos.
- `arquitectura-actual.md` y `arquitectura-propuesta.md`: Análisis de la migración desde el bucle síncrono de consola hacia el modelo orientado a eventos de Windows Forms (`KeyDown`, `PaintEventArgs`, `Timer`) y su proyección en capas hacia SQL Server.
- `modelo-de-datos-propuesto.md`: Diseño relacional de entidades (`T_Jugadores`, `T_Partidas_Activas`, `T_Inventarios`) para persistir el progreso en SQL Server.
- `analisis-tecnico.md`: Evaluación comparativa de tecnologías, mitigación de *flickering* en GDI+ mediante doble búfer y verificación técnica.

---
*Desarrollado para la asignatura de Programación II. Misión: De Código a Conocimiento.*