# Arquitectura Propuesta: Evolución al Modelo N-Capas

## 1. Estado de Avance de la Migración
La evolución arquitectónica de **NEXUS** desde su versión monolítica de consola (H3) se ha estructurado en dos fases estratégicas:

* **Fase 1 (COMPLETADA - Proyecto `Interfaz`):** Migración total de la interfaz de consola hacia **Windows Forms**, reemplazando el bucle síncrono `while(conectado)` por una arquitectura **Orientada a Eventos (*Event-Driven*)**, renderizado matricial con doble búfer (`BufferedCanvas` en GDI+) y control de estados entre **Modo Campamento** y **Modo Exploración**.
* **Fase 2 (EN PROYECCIÓN - Desacoplamiento N-Tier y SQL Server):** Extracción de las clases de dominio actualmente alojadas en `Form1.cs` hacia bibliotecas de clases independientes (`NEXUS.BLL` y `NEXUS.DAL`) y sustitución del guardado en archivos `.json` por persistencia transaccional en **Microsoft SQL Server**.

---

## 2. Capas del Sistema (Arquitectura Objetivo 3-Tier)

### 2.1. Capa de Presentación (`Interfaz` / `NEXUS.GUI`) — *Implementada*
Proyecto de tipo *Windows Forms App*.
* **Responsabilidad:** Capturar la entrada del usuario (teclado `WASD`/Flechas, clics en grilla y botones tácticos) y renderizar el estado del simulador en pantalla.
* **Componentes actuales:**
  * `Form1` (Gestor de Perfiles Biométricos, HUD Superior, Lienzo GDI+ $40 \times 20$, Panel Táctico Contextual y Registro de Telemetría).
  * Diálogos Modales integrados: Combate Táctico, Inventario y Síntesis, Soporte Vital, Tienda de Proficiencia y Atlas Multiversal ($18 \times 12$).

### 2.2. Capa de Lógica de Negocio (`NEXUS.BLL`) — *Lógica validada lista para extracción*
Proyecto de tipo *Class Library* (Biblioteca de clases).
* **Responsabilidad:** Contener todas las reglas del universo de NEXUS sin referencias a `System.Windows.Forms`.
* **Componentes:**
  * **Entidades Core:** `Entidad`, `Usuario`, `Enemigo`, `Realidad`, `Objeto`, `EntidadIris`, `AtlasRealidades` y `DatosPartida`.
  * **Servicios de Dominio:** Validación de desplazamientos, cálculo de curvas de coste en la tienda (`[1, 1, 2, 3, 5]`), resolución de combates y evaluación de vulnerabilidad de I.R.I.S.

### 2.3. Capa de Acceso a Datos (`NEXUS.DAL`) — *Fase de Base de Datos*
Proyecto de tipo *Class Library*.
* **Responsabilidad:** Actuar como puente transaccional entre la Lógica de Negocio y **Microsoft SQL Server**, reemplazando la carpeta local `Saves/*.json`.
* **Componentes:**
  * **Conectividad / ORM:** Implementación mediante `Microsoft.Data.SqlClient` (ADO.NET) o `Entity Framework Core` (`NexusContext`).
  * **Repositorios:** Clases estandarizadas (`JugadorRepository`, `PartidaRepository`, `InventarioRepository`) con operaciones CRUD sobre las tablas `T_Jugadores`, `T_Partidas_Activas` y `T_Inventarios`.

---

## 3. Diagrama de Flujo y Dependencias

```text
[ INTERFAZ GRÁFICA WINDOWS FORMS ] (Proyecto: Interfaz)
       │
       │ (Dispara eventos KeyDown, MouseClick, Timer.Tick)
       ▼
[ LÓGICA DE NEGOCIO Y ENTIDADES ] (Clases de Dominio / NEXUS.BLL)
       │
       │ (Serialización JSON actual / Futura llamada a Repositorios)
       ▼
[ CAPA DE PERSISTENCIA DE DATOS ] (Saves/*.json -> NEXUS.DAL)
       │
       ▼
[ MOTOR RELACIONAL SQL SERVER ]
```

---

## 4. Consolidación del Bucle de Juego Orientado a Eventos
El cambio arquitectónico más importante ya operativo en el proyecto `Interfaz` es la sustitución del bucle infinito de consola por el despacho de mensajes nativo de Windows Forms:
1. **Estado en Reposo (Modo Campamento):** El mapa permanece congelado y el hilo principal espera eventos de clic sobre el Panel Táctico para abrir modales de gestión.
2. **Estado Activo (Modo Exploración):** Cada pulsación de tecla invoca `ProcesarMovimiento(nx, ny)`, actualiza las propiedades encapsuladas del objeto `Usuario`, invalida el `BufferedCanvas` (`canvasMapa.Invalidate()`) para forzar un repintado limpio en GDI+ y verifica el estado de `EntidadIris` mediante `timerIris`.