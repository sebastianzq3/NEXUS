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
 ┣ 📂 docs/               # Manual técnico, diagramas y guía de estudio H3 (PDF/HTML)
 ┣ 📂 src/                # Código fuente del proyecto
 ┃ ┣ 📂 Models/           # Entidades del sistema (Clases base, interfaces, derivadas)
 ┃ ┣ 📂 Managers/         # Lógica de negocio y repositorios de listas
 ┃ ┣ 📜 Program.cs        # Punto de entrada de la aplicación
 ┃ ┗ 📜 NEXUS.csproj      # Archivo de configuración del proyecto C#
 ┣ 📜 .gitignore          # Reglas de exclusión (bin, obj, .vs)
 ┣ 📜 NEXUS.sln           # Solución principal de Visual Studio
 ┗ 📜 README.md           # Documentación principal del repositorio
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

## 🧠 Documentación y Preparación (Hito 3)
La carpeta `docs/` contiene el **Manual Técnico Oficial de NEXUS**. Este documento no solo explica el funcionamiento de este código, sino que actúa como una guía de estudio intensiva para la evaluación del Hito 3, abordando:
- Diferencias entre POO y Programación Estructurada.
- Manejo de memoria (Objetos vs Clases, Instanciación).
- Herencia múltiple de interfaces vs Herencia simple de clases.
- Ejecución diferida en colecciones con LINQ.
- Banco de preguntas universitarias resueltas.

---
*Desarrollado para la asignatura de Programación II. Misión: De Código a Conocimiento.*