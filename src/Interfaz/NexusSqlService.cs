using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.Data.SqlClient;

namespace Interfaz
{
    /// <summary>
    /// Capa de Acceso a Datos (DAL) para Microsoft SQL Server.
    /// Encapsula la persistencia relacional en NexusDB mediante ADO.NET puro,
    /// garantizando transacciones ACID (SqlTransaction) y consultas parametrizadas.
    /// </summary>
    public static class NexusSqlService
    {
        public const string CadenaPorDefecto = @"Server=.\SQLEXPRESS;Database=NexusDB;Trusted_Connection=True;TrustServerCertificate=True;";

        private static readonly string RutaConfigConexion = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "NEXUS",
            "sql_connection.txt"
        );

        /// <summary>
        /// Obtiene la cadena de conexión configurada o la predeterminada.
        /// </summary>
        public static string ObtenerCadenaConexion()
        {
            try
            {
                if (File.Exists(RutaConfigConexion))
                {
                    string leida = File.ReadAllText(RutaConfigConexion).Trim();
                    if (!string.IsNullOrWhiteSpace(leida))
                        return leida;
                }
            }
            catch { }

            return CadenaPorDefecto;
        }

        /// <summary>
        /// Guarda una cadena de conexión personalizada de forma persistente.
        /// </summary>
        public static void GuardarCadenaConexion(string cadena)
        {
            try
            {
                string carpeta = Path.GetDirectoryName(RutaConfigConexion);
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                File.WriteAllText(RutaConfigConexion, cadena.Trim());
            }
            catch { }
        }

        /// <summary>
        /// Comprueba la conectividad con el servidor y la existencia de la base de datos NexusDB.
        /// </summary>
        public static bool ProbarConexion(string cadena, out string mensaje)
        {
            try
            {
                using var conn = new SqlConnection(cadena);
                conn.Open();

                using var cmd = new SqlCommand("SELECT @@VERSION AS Version, DB_NAME() AS BaseDatos;", conn);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string bd = reader["BaseDatos"]?.ToString() ?? "Desconocida";
                    mensaje = $"Conexión exitosa a '{bd}'.";
                    return true;
                }

                mensaje = "Conexión establecida pero no se pudo verificar el contexto de la base de datos.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtiene el resumen de todos los perfiles persistidos en SQL Server para el DataGridView.
        /// </summary>
        public static List<(string Clave, DatosPartida Datos)> ListarPerfiles(string cadena)
        {
            var resultado = new List<(string Clave, DatosPartida Datos)>();

            using var conn = new SqlConnection(cadena);
            conn.Open();

            using var cmd = new SqlCommand("dbo.sp_ListarPerfiles", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string nombre = reader["Nombre"].ToString();
                int edad = Convert.ToInt32(reader["Edad"]);
                int nivel = Convert.ToInt32(reader["Nivel"]);
                int exp = Convert.ToInt32(reader["Experiencia"]);
                int vidaActual = Convert.ToInt32(reader["VidaActual"]);
                int vidaMax = Convert.ToInt32(reader["VidaMax"]);
                int energiaActual = Convert.ToInt32(reader["EnergiaActual"]);
                int energiaMax = Convert.ToInt32(reader["EnergiaMax"]);
                string nombreRealidad = reader["NombreRealidad"].ToString();
                int estabilidad = Convert.ToInt32(reader["NivelEstabilidad"]);
                int prof = Convert.ToInt32(reader["PuntosProficiencia"]);
                int pResp = Convert.ToInt32(reader["PreguntasCSharpRespondidas"]);
                int pAcert = Convert.ToInt32(reader["PreguntasCSharpAcertadas"]);

                Usuario u = new Usuario
                {
                    Nombre = nombre,
                    Edad = edad,
                    Nivel = nivel,
                    Experiencia = exp,
                    VidaMax = vidaMax,
                    Vida = vidaActual,
                    EnergiaMax = energiaMax,
                    Energia = energiaActual,
                    PuntosProficiencia = prof,
                    PreguntasCSharpRespondidas = pResp,
                    PreguntasCSharpAcertadas = pAcert
                };

                Realidad r = new Realidad
                {
                    Nombre = nombreRealidad,
                    Estabilidad = estabilidad
                };

                DatosPartida dp = new DatosPartida
                {
                    Jugador = u,
                    RealidadActual = r
                };

                resultado.Add((nombre, dp));
            }

            return resultado;
        }

        /// <summary>
        /// Carga una partida completa desde SQL Server (Jugador + Realidad + Inventario + Observaciones).
        /// </summary>
        public static DatosPartida CargarPartidaCompleta(string cadena, string nombre)
        {
            using var conn = new SqlConnection(cadena);
            conn.Open();

            using var cmd = new SqlCommand("dbo.sp_CargarPartidaPorNombre", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 50) { Value = nombre });

            using var reader = cmd.ExecuteReader();

            // 1. Result Set 1: Jugador y Realidad
            if (!reader.Read())
                return null;

            Usuario cadete = new Usuario
            {
                Nombre = reader["Nombre"].ToString(),
                Edad = Convert.ToInt32(reader["Edad"]),
                Nivel = Convert.ToInt32(reader["Nivel"]),
                Experiencia = Convert.ToInt32(reader["Experiencia"]),
                VidaMax = Convert.ToInt32(reader["VidaMax"]),
                Vida = reader["VidaActual"] != DBNull.Value ? Convert.ToInt32(reader["VidaActual"]) : Convert.ToInt32(reader["VidaMax"]),
                EnergiaMax = Convert.ToInt32(reader["EnergiaMax"]),
                Energia = reader["EnergiaActual"] != DBNull.Value ? Convert.ToInt32(reader["EnergiaActual"]) : Convert.ToInt32(reader["EnergiaMax"]),
                DañoBase = Convert.ToInt32(reader["DanoBase"]),
                CapacidadInventario = Convert.ToInt32(reader["CapacidadInventario"]),
                PuntosProficiencia = Convert.ToInt32(reader["PuntosProficiencia"]),
                UpgradesInv = Convert.ToInt32(reader["UpgradesInv"]),
                UpgradesVida = Convert.ToInt32(reader["UpgradesVida"]),
                UpgradesEnergia = Convert.ToInt32(reader["UpgradesEnergia"]),
                PreguntasCSharpRespondidas = Convert.ToInt32(reader["PreguntasCSharpRespondidas"]),
                PreguntasCSharpAcertadas = Convert.ToInt32(reader["PreguntasCSharpAcertadas"]),
                BolsaFragmentos = reader["Fragmentos"] != DBNull.Value ? Convert.ToInt32(reader["Fragmentos"]) : 0,
                Restos = reader["Restos"] != DBNull.Value ? Convert.ToInt32(reader["Restos"]) : 0,
                Inventario = new List<Objeto>(),
                Observaciones = new List<string>()
            };

            Realidad realidad = new Realidad
            {
                Nombre = reader["NombreRealidad"]?.ToString() ?? "Sector Desconocido",
                Estabilidad = reader["NivelEstabilidad"] != DBNull.Value ? Convert.ToInt32(reader["NivelEstabilidad"]) : 50,
                Anomalia = reader["TipoAnomalia"] != DBNull.Value ? (Anomalia)Convert.ToByte(reader["TipoAnomalia"]) : Anomalia.Tiempo,
                CoordenadaX = reader["CoordX"] != DBNull.Value ? Convert.ToInt32(reader["CoordX"]) : -1,
                CoordenadaY = reader["CoordY"] != DBNull.Value ? Convert.ToInt32(reader["CoordY"]) : -1,
                Extraida = reader["Extraida"] != DBNull.Value && Convert.ToBoolean(reader["Extraida"]),
                Cartografiada = reader["Cartografiada"] != DBNull.Value && Convert.ToBoolean(reader["Cartografiada"])
            };

            // 2. Result Set 2: Inventario
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    string nomItem = reader["NombreObjeto"].ToString();
                    Anomalia tipoAnom = (Anomalia)Convert.ToByte(reader["TipoAnomalia"]);
                    string desc = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : "";
                    int usos = Convert.ToInt32(reader["UsosRestantes"]);

                    Objeto obj = new Objeto
                    {
                        Nombre = nomItem,
                        Contrarresta = tipoAnom,
                        Anomalia = tipoAnom,
                        Descripcion = desc,
                        Usos = usos
                    };
                    cadete.Inventario.Add(obj);
                }
            }

            // 3. Result Set 3: Observaciones
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    string textoPista = reader["TextoPista"].ToString();
                    cadete.Observaciones.Add(textoPista);
                }
            }

            realidad.InicializarMapaSiNoExiste();
            return new DatosPartida { Jugador = cadete, RealidadActual = realidad };
        }

        /// <summary>
        /// Guarda atómicamente la partida en SQL Server usando una transacción explícita (SqlTransaction).
        /// Garantiza consistencia ACID de Jugador, Partida Activa, Inventario y Observaciones.
        /// </summary>
        public static void GuardarPartidaCompleta(string cadena, Usuario jugador, Realidad realidad)
        {
            if (jugador == null || realidad == null)
                throw new ArgumentNullException("Los datos del jugador o la realidad no pueden ser nulos.");

            using var conn = new SqlConnection(cadena);
            conn.Open();

            using var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                // 1. Guardar cabecera mediante el procedimiento almacenado
                int jugadorId;
                using (var cmd = new SqlCommand("dbo.sp_GuardarPartidaCabecera", conn, trans))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 50) { Value = jugador.Nombre });
                    cmd.Parameters.Add(new SqlParameter("@Edad", SqlDbType.Int) { Value = jugador.Edad });
                    cmd.Parameters.Add(new SqlParameter("@Nivel", SqlDbType.Int) { Value = jugador.Nivel });
                    cmd.Parameters.Add(new SqlParameter("@Experiencia", SqlDbType.Int) { Value = jugador.Experiencia });
                    cmd.Parameters.Add(new SqlParameter("@VidaMax", SqlDbType.Int) { Value = jugador.VidaMax });
                    cmd.Parameters.Add(new SqlParameter("@EnergiaMax", SqlDbType.Int) { Value = jugador.EnergiaMax });
                    cmd.Parameters.Add(new SqlParameter("@DanoBase", SqlDbType.Int) { Value = jugador.DañoBase });
                    cmd.Parameters.Add(new SqlParameter("@CapacidadInventario", SqlDbType.Int) { Value = Math.Max(1, jugador.CapacidadInventario) });
                    cmd.Parameters.Add(new SqlParameter("@PuntosProficiencia", SqlDbType.Int) { Value = jugador.PuntosProficiencia });
                    cmd.Parameters.Add(new SqlParameter("@UpgradesInv", SqlDbType.Int) { Value = jugador.UpgradesInv });
                    cmd.Parameters.Add(new SqlParameter("@UpgradesVida", SqlDbType.Int) { Value = jugador.UpgradesVida });
                    cmd.Parameters.Add(new SqlParameter("@UpgradesEnergia", SqlDbType.Int) { Value = jugador.UpgradesEnergia });
                    cmd.Parameters.Add(new SqlParameter("@PreguntasRespondidas", SqlDbType.Int) { Value = jugador.PreguntasCSharpRespondidas });
                    cmd.Parameters.Add(new SqlParameter("@PreguntasAcertadas", SqlDbType.Int) { Value = jugador.PreguntasCSharpAcertadas });

                    // Datos de Partida Activa
                    cmd.Parameters.Add(new SqlParameter("@VidaActual", SqlDbType.Int) { Value = jugador.Vida });
                    cmd.Parameters.Add(new SqlParameter("@EnergiaActual", SqlDbType.Int) { Value = jugador.Energia });
                    cmd.Parameters.Add(new SqlParameter("@Fragmentos", SqlDbType.Int) { Value = jugador.BolsaFragmentos });
                    cmd.Parameters.Add(new SqlParameter("@Restos", SqlDbType.Int) { Value = jugador.Restos });
                    cmd.Parameters.Add(new SqlParameter("@NombreRealidad", SqlDbType.NVarChar, 100) { Value = realidad.Nombre });
                    cmd.Parameters.Add(new SqlParameter("@TipoAnomalia", SqlDbType.TinyInt) { Value = (byte)realidad.Anomalia });
                    cmd.Parameters.Add(new SqlParameter("@NivelEstabilidad", SqlDbType.Int) { Value = Math.Clamp(realidad.Estabilidad, 0, 100) });
                    cmd.Parameters.Add(new SqlParameter("@CoordX", SqlDbType.Int) { Value = Math.Clamp(realidad.CoordenadaX, -1, 40) });
                    cmd.Parameters.Add(new SqlParameter("@CoordY", SqlDbType.Int) { Value = Math.Clamp(realidad.CoordenadaY, -1, 20) });
                    cmd.Parameters.Add(new SqlParameter("@Extraida", SqlDbType.Bit) { Value = realidad.Extraida });
                    cmd.Parameters.Add(new SqlParameter("@Cartografiada", SqlDbType.Bit) { Value = realidad.Cartografiada });

                    var pOut = new SqlParameter("@JugadorIdOutput", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(pOut);

                    cmd.ExecuteNonQuery();
                    jugadorId = Convert.ToInt32(pOut.Value);
                }

                // 2. Sincronizar Inventario (Reemplazo atómico dentro de la misma transacción)
                using (var cmdDelInv = new SqlCommand("DELETE FROM dbo.T_Inventarios WHERE FK_JugadorId = @JId;", conn, trans))
                {
                    cmdDelInv.Parameters.Add(new SqlParameter("@JId", SqlDbType.Int) { Value = jugadorId });
                    cmdDelInv.ExecuteNonQuery();
                }

                if (jugador.Inventario != null && jugador.Inventario.Count > 0)
                {
                    foreach (var item in jugador.Inventario)
                    {
                        using var cmdInsInv = new SqlCommand(@"
                            INSERT INTO dbo.T_Inventarios (FK_JugadorId, NombreObjeto, TipoAnomalia, Descripcion, UsosRestantes)
                            VALUES (@JId, @Nom, @Anom, @Desc, @Usos);", conn, trans);

                        cmdInsInv.Parameters.Add(new SqlParameter("@JId", SqlDbType.Int) { Value = jugadorId });
                        cmdInsInv.Parameters.Add(new SqlParameter("@Nom", SqlDbType.NVarChar, 100) { Value = item.Nombre });
                        cmdInsInv.Parameters.Add(new SqlParameter("@Anom", SqlDbType.TinyInt) { Value = (byte)item.Contrarresta });
                        cmdInsInv.Parameters.Add(new SqlParameter("@Desc", SqlDbType.NVarChar, 300) { Value = (object)item.Descripcion ?? DBNull.Value });
                        cmdInsInv.Parameters.Add(new SqlParameter("@Usos", SqlDbType.Int) { Value = Math.Max(1, item.Usos) });

                        cmdInsInv.ExecuteNonQuery();
                    }
                }

                // 3. Sincronizar Observaciones
                using (var cmdDelObs = new SqlCommand("DELETE FROM dbo.T_Observaciones WHERE FK_JugadorId = @JId;", conn, trans))
                {
                    cmdDelObs.Parameters.Add(new SqlParameter("@JId", SqlDbType.Int) { Value = jugadorId });
                    cmdDelObs.ExecuteNonQuery();
                }

                if (jugador.Observaciones != null && jugador.Observaciones.Count > 0)
                {
                    foreach (var obs in jugador.Observaciones)
                    {
                        using var cmdInsObs = new SqlCommand(@"
                            INSERT INTO dbo.T_Observaciones (FK_JugadorId, TextoPista)
                            VALUES (@JId, @Pista);", conn, trans);

                        cmdInsObs.Parameters.Add(new SqlParameter("@JId", SqlDbType.Int) { Value = jugadorId });
                        cmdInsObs.Parameters.Add(new SqlParameter("@Pista", SqlDbType.NVarChar, 300) { Value = obs });
                        cmdInsObs.ExecuteNonQuery();
                    }
                }

                trans.Commit();
            }
            catch
            {
                try { trans.Rollback(); } catch { }
                throw;
            }
        }

        /// <summary>
        /// Elimina definitivamente el perfil y todos sus registros asociados en cascada.
        /// </summary>
        public static bool EliminarPerfil(string cadena, string nombre)
        {
            using var conn = new SqlConnection(cadena);
            conn.Open();

            using var cmd = new SqlCommand("dbo.sp_EliminarPerfil", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 50) { Value = nombre });

            int filas = Convert.ToInt32(cmd.ExecuteScalar());
            return filas > 0;
        }

        /// <summary>
        /// Carga el catálogo de preguntas didácticas de C# almacenadas en SQL Server.
        /// </summary>
        public static List<PreguntaCSharp> CargarPreguntasDesdeSql(string cadena)
        {
            var preguntas = new List<PreguntaCSharp>();

            using var conn = new SqlConnection(cadena);
            conn.Open();

            using var cmd = new SqlCommand("dbo.sp_ObtenerPreguntasCSharp", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                preguntas.Add(new PreguntaCSharp
                {
                    Categoria = reader["Categoria"].ToString(),
                    Dificultad = reader["Dificultad"].ToString(),
                    Enunciado = reader["Enunciado"].ToString(),
                    CodigoEjemplo = reader["CodigoEjemplo"] != DBNull.Value ? reader["CodigoEjemplo"].ToString() : null,
                    Opciones = new[]
                    {
                        reader["OpcionA"].ToString(),
                        reader["OpcionB"].ToString(),
                        reader["OpcionC"].ToString(),
                        reader["OpcionD"].ToString()
                    },
                    IndiceCorrecto = Convert.ToInt32(reader["IndiceCorrecto"]),
                    Explicacion = reader["Explicacion"].ToString()
                });
            }

            return preguntas;
        }
    }
}
