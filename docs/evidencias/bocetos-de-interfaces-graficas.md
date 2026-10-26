# Bocetos de Interfaces Gráficas (Wireframes) - NEXUS WinForms

A continuación se presentan los esquemas de las pantallas principales del juego utilizando la distribución de controles estándar de Windows Forms (Paneles, DataGridViews, ProgressBars, Botones y PictureBoxes).

---

## 1. FrmMain: Menú Principal y Gestor de Perfiles

```text
+-----------------------------------------------------------------------------+
| [X] NEXUS - Sistema de Anclaje Multiversal                            [-] [X]|
+-----------------------------------------------------------------------------+
|                                                                             |
|                                                                             |
|                              [ N E X U S ]                                  |
|                          Training System v2.0                               |
|                                                                             |
|   +---------------------------------------------------------------------+   |
|   | DataGridView: Perfiles Registrados                                  |   |
|   |---------------------------------------------------------------------|   |
|   | Nombre        | Nivel | Experiencia | Pts. Proficiencia |           |   |
|   |---------------+-------+-------------+-------------------|           |   |
|   | Cadete Alpha  |   5   |     80/100  |         2         |           |   |
|   | Explorador Z  |   2   |     15/100  |         0         |           |   |
|   | ...           |       |             |                   |           |   |
|   +---------------------------------------------------------------------+   |
|                                                                             |
|         [ CREAR NUEVO PERFIL ]   [ CARGAR SELECCIÓN ]   [ ELIMINAR ]        |
|                                                                             |
+-----------------------------------------------------------------------------+
```
**Descripción de UX:** Interfaz limpia y directa. El `DataGridView` central permite seleccionar fácilmente la partida. Los botones inferiores activan los eventos CRUD sobre la base de datos SQL Server.

---

## 2. FrmExploracion: Mapa Táctico de Incursión

```text
+-----------------------------------------------------------------------------+
| [X] NEXUS - Sector: Abismo Oscuro (Estabilidad: 45%)                  [-] [X]|
+-----------------------------------------------------------------------------+
| [HUD]                                                                       |
| Vida:     [██████████░░] 80/100     Estabilidad: [████░░░░░░] 45%           |
| Energía:  [██████░░░░░░] 5/10       Objetivo:    [ Chatarrero (1/3) ]       |
+-----------------------------------------------------------------------------+
| [ MAPA TÁCTICO - TableLayoutPanel / GDI+ ]  | [ REGISTRO DEL SISTEMA ]      |
|                                             | RichTextBox                   |
|  [▓] [▓] [▓] [▓] [▓] [▓] [▓] [▓] [▓] [▓]    | > Enlace establecido...       |
|  [▓] [·] [·] [·] [·] [·] [·] [?] [·] [▓]    | > Avanzando por el sector...  |
|  [▓] [·] [☗] [·] [·] [·] [·] [·] [·] [▓]    | > Absorbiste un Núcleo (+3 E) |
|  [▓] [·] [·] [@] [·] [·] [·] [Ж] [·] [▓]    | > ¡Anomalía detectada!        |
|  [▓] [·] [·] [·] [·] [಄] [·] [·] [·] [▓]    |                               |
|  [▓] [·] [▤] [·] [·] [·] [·] [·] [·] [▓]    |                               |
|  [▓] [·] [·] [·] [·] [♡] [·] [·] [·] [▓]    |                               |
|  [▓] [▓] [▓] [▓] [▓] [▓] [▓] [▓] [▓] [▓]    |-------------------------------|
|                                             | [ACCIONES]                    |
|                                             | [ INVENTARIO Y CRAFTEO (I) ]  |
|  Controles: (W,A,S,D) o Clic en casilla     | [   INTENTAR EXTRACCIÓN    ]  |
+-----------------------------------------------------------------------------+
```
**Descripción de UX:** El área izquierda renderiza la cuadrícula de juego (ya sea usando botones, imágenes (PictureBox) o dibujando con GDI+ en un evento Paint). El panel derecho mantiene un registro constante para que el jugador nunca pierda el hilo de los eventos.

---

## 3. FrmInventario: Gestión y Síntesis (Ventana Modal)

```text
+-----------------------------------------------------------------------------+
| [X] NEXUS - Interfaz de Manipulación                                  [-] [X]|
+-----------------------------------------------------------------------------+
| [ RECURSOS ]  Fragmentos: 4   |   Restos Tecnológicos: 7                    |
+-----------------------------------------------------------------------------+
| LISTA DE EQUIPO (ListBox)      | DETALLES DEL OBJETO SELECCIONADO           |
|                                |                                            |
| > Reloj de Arena Invertido     | [Picture/Icon] Nombre: Reloj Invertido     |
|   Gafas de Plomo               |                Usos: 2/3                   |
|                                |                                            |
|                                | Descripción: La arena fluye hacia arriba,  |
|                                | revirtiendo el decaimiento temporal en tu  |
|                                | sector de la realidad.                     |
|                                |                                            |
|                                |            [ EQUIPAR / USAR ]              |
|                                |            [ DESCARTAR ITEM ]              |
+-----------------------------------------------------------------------------+
| [ PISTAS SENSORIALES (Observaciones) ]                                      |
| - Miras tu reloj y las manecillas giran frenéticamente en reversa.          |
+-----------------------------------------------------------------------------+
| [ CRAFTEO ]                                                                 |
| [ DECODIFICAR PISTA (Costo: 3 Frag) ]   [ ENSAMBLAR OBJETO (Costo: 3 Rest) ]|
+-----------------------------------------------------------------------------+
```
**Descripción de UX:** Pantalla dividida lógicamente. Arriba la "moneda", en medio la gestión de la mochila con información detallada a la derecha, y en la zona inferior el sistema de crafteo y las pistas deducidas.

---

## 4. FrmCombate: Encuentro Táctico (Ventana Modal)

```text
+-----------------------------------------------------------------------------+
| [X] ALERTA: CONTACTO HOSTIL                                           [-] [X]|
+-----------------------------------------------------------------------------+
|                                                                             |
|                      [ PICTURE BOX: Enemigo Sprite ]                        |
|                        GLITCH-ARÁCNIDO (Daño: 15)                           |
|                      HP: [████████████░░░] 45/60                            |
|                                                                             |
|-----------------------------------------------------------------------------|
|                                                                             |
|                      [ PICTURE BOX: Jugador Sprite ]                        |
|                          EXPLORADOR (Energía: 4)                            |
|                      HP: [████████████████] 80/100                          |
|                                                                             |
|-----------------------------------------------------------------------------|
| [ LOG DE COMBATE ]                                                          |
| > El enemigo te golpea causando 12 de daño.                                 |
| > Tu turno...                                                               |
|-----------------------------------------------------------------------------|
| [ COMANDOS TÁCTICOS ]                                                       |
|                                                                             |
|  +------------------------+      +------------------------+                 |
|  | [1] ATAQUE LIGERO (0E) |      | [2] ATAQUE PESADO (1E) |                 |
|  | Daño moderado, seguro  |      | Gran daño, consume E   |                 |
|  +------------------------+      +------------------------+                 |
|  +------------------------+      +------------------------+                 |
|  | [3] GUARDIA / BLOQUEO  |      | [4] INTENTAR ESCAPE    |                 |
|  | Mitiga el daño al 50%  |      | Consume 1E (45% éxito) |                 |
|  +------------------------+      +------------------------+                 |
|                                                                             |
+-----------------------------------------------------------------------------+
```
**Descripción de UX:** Esta ventana detiene el tiempo del juego (modal). Toda la atención se centra en la decisión táctica mediante botones amplios (*hitboxes* generosos) fáciles de hacer clic. Las barras de HP garantizan un entendimiento inmediato del estado del combate.