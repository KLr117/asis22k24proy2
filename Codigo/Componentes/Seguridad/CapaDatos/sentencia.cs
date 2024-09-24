﻿using System;
using System.Data.Odbc;
using System.Windows.Forms;
using System.Net;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions; // Para expresiones regulares

namespace CapaDatos
{
    public class sentencia
    {
        conexion cn = new conexion();
        private string idUsuario;

        public sentencia(string idUsuario)
        {
            this.idUsuario = idUsuario;
        }

        public sentencia()
        {

        }
        //Kateryn De Leon
        //buscar usuarios
        public OdbcDataAdapter consultarUsuarios()
        {
            cn.conectar();
            string sqlUsuarios = "SELECT Pk_id_usuario as Usuario, nombre_usuario as Nombre, apellido_usuario as Apellido, username_usuario as Username, password_usuario as Password, email_usuario as Email, ultima_conexion_usuario as Ultima_Conexion, estado_usuario as Estado, pregunta as Pregunta, respuesta as Respuesta FROM tbl_usuarios";
            OdbcDataAdapter dataUsuarios = new OdbcDataAdapter(sqlUsuarios, cn.conectar());
            insertarBitacora(idUsuario, "Realizo una consulta a usuarios", "tbl_usuarios", "1001");
            return dataUsuarios;
        }

        //****************************************Kevin López***************************************************
        public OdbcDataAdapter consultarModulos()
        {
            cn.conectar();
            string sqlModulos = "SELECT nombre_modulo FROM Tbl_modulos WHERE estado_modulo = 1";
            OdbcDataAdapter dataModulos = new OdbcDataAdapter(sqlModulos, cn.conectar());
            insertarBitacora(idUsuario, "Realizo una consulta a modulos", "Tbl_modulos", "1003");
            return dataModulos;
        }
        //****************************************FIN Kevin López***************************************************

        //****************************************Kevin López***************************************************
        public OdbcDataAdapter consultarPerfiles()
        {
            cn.conectar();
            string sqlPerfiles = "SELECT nombre_perfil FROM Tbl_perfiles WHERE estado_perfil = 1";
            OdbcDataAdapter dataPerfiles = new OdbcDataAdapter(sqlPerfiles, cn.conectar());
            insertarBitacora(idUsuario, "Realizo una consulta a perfiles", "Tbl_perfiles", "1004");
            return dataPerfiles;
        }
        //****************************************Kevin López***************************************************

        //#############INICIO ALYSON RODRIGUEZ 9959-21-829
        public OdbcDataAdapter consultarAplicaciones(string nombreModulo)
        {
            cn.conectar();
            OdbcDataAdapter dataAplicaciones = null;

            try
            {
                string sqlAplicaciones = @"
                SELECT a.Pk_id_aplicacion, a.nombre_aplicacion 
                FROM tbl_aplicaciones a
                JOIN tbl_asignacion_modulo_aplicacion ama ON a.pk_id_aplicacion = ama.fk_id_aplicacion
                JOIN tbl_modulos m ON m.pk_id_modulos = ama.fk_id_modulos
                WHERE m.nombre_modulo = ?";

                dataAplicaciones = new OdbcDataAdapter(sqlAplicaciones, cn.conectar());
                dataAplicaciones.SelectCommand.Parameters.AddWithValue("?", nombreModulo);

                // Registro de la bitacora
                insertarBitacora(idUsuario, "Realizó una consulta a aplicaciones", "tbl_aplicacion", "1002");

                return dataAplicaciones;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            finally
            {
                // Asegúrate de cerrar la conexión si es necesario
                // cn.desconectar(); // Descomenta si tienes un método para cerrar la conexión
            }
        }

        //#############FINALIZA ALYSON RODRIGUEZ 9959-21-829


        //Trabajado María José Véliz
        public OdbcDataAdapter insertarPermisosUA(string codigoUsuario, string nombreAplicacion, string ingresar, string consulta, string modificar, string eliminar, string imprimir)
        {
            string sCodigoAplicacion = " ";
            string sCodigoUsuario = " ";

            try
            {
                // Obtén el código de la aplicación
                OdbcCommand sqlCodigoModulo = new OdbcCommand("SELECT Pk_id_aplicacion FROM Tbl_aplicaciones WHERE nombre_aplicacion = '" + nombreAplicacion + "' ", cn.conectar());
                OdbcDataReader almacena = sqlCodigoModulo.ExecuteReader();

                while (almacena.Read() == true)
                {
                    sCodigoAplicacion = almacena.GetString(0);
                }


                // Obtén el código del usuario
                OdbcCommand sqlCodigoUsuario = new OdbcCommand("SELECT Pk_id_usuario FROM Tbl_usuarios WHERE nombre_usuario = '" + codigoUsuario + "' ", cn.conectar());
                OdbcDataReader almacenaUsuario = sqlCodigoUsuario.ExecuteReader();

                while (almacenaUsuario.Read() == true)
                {
                    sCodigoUsuario = almacenaUsuario.GetString(0);
                }
                almacenaUsuario.Close();
                sqlCodigoUsuario.Connection.Close();

                // Inserta los permisos usando el código de la aplicación y el código del usuario
                string sqlInsertarPermisosUA = "INSERT INTO Tbl_permisos_aplicaciones_usuario(Fk_id_usuario, Fk_id_aplicacion, guardar_permiso, buscar_permiso, modificar_permiso, eliminar_permiso, imprimir_permiso) VALUES ('" + sCodigoUsuario + "','" + sCodigoAplicacion + "', '" + ingresar + "', '" + consulta + "', '" + modificar + "', '" + eliminar + "', '" + imprimir + "');";
                // Ejecuta el comando de inserción
                OdbcDataAdapter dataPermisosUA = new OdbcDataAdapter(sqlInsertarPermisosUA, cn.conectar());
                // Inserta en la bitácora
                insertarBitacora(idUsuario, "Asignó aplicación: " + nombreAplicacion + " a usuario: " + codigoUsuario, "Tbl_permisos_aplicaciones_usuario", "1103");


                almacena.Close();
                sqlCodigoModulo.Connection.Close();

                return dataPermisosUA;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        //Termina

        //###################  lo que hizo Karla  Sofia Gómez Tobar #######################
        public OdbcDataAdapter mostrarPerfilesDeUsuario(string TablaPerfilUsuario)
        {
            string sql = "SELECT * FROM " + TablaPerfilUsuario + ";";
            OdbcDataAdapter dataTable = new OdbcDataAdapter(sql, cn.conectar());
            return dataTable;
        }

        public bool eliminarPerfilUsuario(string Id_Perfil_Usuario)
        {

            try
            {

                cn.conectar();
                string sqlEliminarPerfilUsuario = "DELETE FROM Tbl_asignaciones_perfils_usuario WHERE PK_id_Perfil_Usuario = ?";
                using (OdbcCommand cmd = new OdbcCommand(sqlEliminarPerfilUsuario, cn.conectar()))
                {

                    cmd.Parameters.AddWithValue("@Id_Perfil_Usuario", Id_Perfil_Usuario);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        insertarBitacora(idUsuario, "Eliminó un perfil: " + Id_Perfil_Usuario, "Tbl_asignaciones_perfils_usuario", "1103");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }


        }

        public void insertarPerfilUsuario(string codigoUsuario, string codigoPerfil)
        {


            try
            {
                using (OdbcConnection connection = cn.conectar())

                {
                    //
                    string query = "INSERT INTO Tbl_asignaciones_perfils_usuario(" +
                                                     "Fk_id_usuario," +
                                                     "Fk_id_perfil)" +
                                                     "VALUES (?, ?) ";

                    using (OdbcCommand cmd = new OdbcCommand(query, connection))
                    {
                        // Agregar los parámetros al comando
                        cmd.Parameters.AddWithValue("@Fk_id_usuario", codigoUsuario);
                        cmd.Parameters.AddWithValue("@Fk_id_perfil", codigoPerfil);

                        // Ejecutar el comando
                        cmd.ExecuteNonQuery();
                        insertarBitacora(idUsuario, "Inserto un nuevo modulo: " + codigoUsuario + " - " + codigoPerfil, "Tbl_asignaciones_perfils_usuario", "1103");
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar la asignacion: " + ex.Message);
            }
        }
        //###################  termina lo que hizo  Karla  Sofia Gómez Tobar #######################




        //###################  lo que hizo Karla  Sofia Gómez Tobar #######################
        public OdbcDataAdapter validarIDAplicacion()
        {
            try
            {

                string sqlIDAplicacion = "SELECT MAX(Pk_id_aplicacion)+1 FROM tbl_aplicaciones";
                OdbcDataAdapter dataIDAplicacion = new OdbcDataAdapter(sqlIDAplicacion, cn.conectar());
                return dataIDAplicacion;
                insertarBitacora(idUsuario, "Se selecciono una aplicación", "tbl_aplicaciones", "1101");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        //###################  termina lo que hizo  Karla  Sofia Gómez Tobar #######################

        //---------------------------------------------------- Inicio: GABRIELA SUC ---------------------------------------------------
        public bool ModificarUsuario(string Id_Usuario, string nombre, string apellido, string correo, int estado_usuario, string pregunta, string respuesta)
        {
            try
            {
                // Comenzar a construir la consulta SQL
                string query = "UPDATE Tbl_usuarios SET ";

                // Lista de parámetros a incluir en la consulta
                List<OdbcParameter> parameters = new List<OdbcParameter>();

                // Agregar solo los campos que no estén vacíos
                if (!string.IsNullOrEmpty(nombre))
                {
                    query += "nombre_usuario = ?, ";
                    parameters.Add(new OdbcParameter("nombre_usuario", nombre));
                }
                if (!string.IsNullOrEmpty(apellido))
                {
                    query += "apellido_usuario = ?, ";
                    parameters.Add(new OdbcParameter("apellido_usuario", apellido));
                }
                if (!string.IsNullOrEmpty(correo))
                {
                    query += "email_usuario = ?, ";
                    parameters.Add(new OdbcParameter("email_usuario", correo));
                }
                if (!string.IsNullOrEmpty(respuesta))
                {
                    query += "respuesta = ?, ";
                    parameters.Add(new OdbcParameter("respuesta", respuesta));
                }

                // Asegurarse de agregar la pregunta seleccionada
                if (!string.IsNullOrEmpty(pregunta))
                {
                    query += "pregunta = ?, "; // Asegúrate de que el campo en la BD sea correcto
                    parameters.Add(new OdbcParameter("pregunta", pregunta));
                }

                // El estado siempre se modifica (0 o 1)
                query += "estado_usuario = ? ";
                parameters.Add(new OdbcParameter("estado_usuario", estado_usuario));

                // Completar la consulta SQL con la condición WHERE
                query += "WHERE Pk_id_usuario = ?;";
                parameters.Add(new OdbcParameter("id_usuario", Id_Usuario));

                // Ejecutar la consulta SQL
                using (OdbcCommand command = new OdbcCommand(query, cn.conectar()))
                {
                    // Agregar los parámetros al comando
                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(param);
                    }
                    insertarBitacora(idUsuario, "Se modificó el usuario: " + Id_Usuario, "Tbl_usuarios", "1001");

                    // Ejecutar la consulta
                    int result = command.ExecuteNonQuery();

                    // Verificar si se modificó algún registro
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error al intentar modificar el registro: " + ex.Message);
                insertarBitacora(idUsuario, "Ocurrio un error al modificar el usuario: " + Id_Usuario, "Tbl_usuarios", "1001");
                return false;
            }
        }


        //---------------------------------------------------- Fin: GABRIELA SUC ----------------------------------------------------

        /* Creado por Emerzon Garcia */

        public bool EliminarPerfil1(string ID_perfil)
        {
            try
            {
                // Conectar a la base de datos
                cn.conectar();

                // Crear la consulta SQL para eliminar
                string sqlEliminarPerfil = "DELETE FROM Tbl_perfiles WHERE PK_id_perfil = ?";

                // Usar OdbcCommand para ejecutar el DELETE
                using (OdbcCommand cmd = new OdbcCommand(sqlEliminarPerfil, cn.conectar()))
                {
                    // Agregar parámetro para evitar inyecciones SQL
                    cmd.Parameters.AddWithValue("@ID_perfil", ID_perfil);

                    // Ejecutar la consulta
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Insertar en bitácora si la eliminación fue exitosa
                    if (rowsAffected > 0)
                    {
                        insertarBitacora(idUsuario, "Eliminó un perfil: " + ID_perfil, "tbl_perfil", "1004");
                        return true; // Indica que la eliminación fue exitosa
                    }
                    else
                    {
                        return false; // No se afectó ninguna fila
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        /*-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/



        //AGREGAR
        //KATERYN DE LEON

        //--------------------------------------------------------------- Inicio: Marco Monroy ---------------------------------------------------------------
        public OdbcDataAdapter insertarusuario(string nombre, string apellido, string id, string clave, string correo, string fecha, string estadousuario, string pregunta, string respuesta)
        {
            // Consulta SQL con concatenación de valores (esto es menos seguro, pero funciona en tu caso)
            string sqlusuarios = "INSERT INTO tbl_usuarios (nombre_usuario, apellido_usuario, username_usuario, password_usuario, email_usuario, ultima_conexion_usuario, estado_usuario, pregunta, respuesta) " +
                                 "VALUES ('" + nombre + "', '" + apellido + "', '" + id + "', '" + clave + "', '" + correo + "', '" + fecha + "', '" + estadousuario + "', '" + pregunta + "', '" + respuesta + "')";

            // Crear el comando y asignar la conexión
            OdbcDataAdapter datausuarios = new OdbcDataAdapter(sqlusuarios, cn.conectar());

            try
            {
                insertarBitacora(idUsuario, "Se inserto el usuario con nombre: " + id, "tbl_usuarios", "1101");
                return datausuarios; // Retorna si la inserción fue exitosa
            }
            catch (Exception ex)
            {
                // Capturar cualquier excepción y manejarla
                Console.WriteLine("Error al insertar el usuario: " + ex.Message);

                // Retornar null o manejar de acuerdo a tus necesidades
                return null;
            }
        }


        //--------------------------------------------------------------- Fin: Marco Monroy ---------------------------------------------------------------


        public OdbcDataAdapter insertarclaves(string id, string nombre, string apellido, string clave)
        {
            cn.conectar();
            MessageBox.Show("Contraseña Actualizada");
            string sqlconsulta = "UPDATE tbl_usuarios set PK_id_usuario='" + id + "',nombre_usuario='" + apellido + "',apellido_usuarios='" + nombre + "',password_usuario='" + clave + "',estado_usuario='1' where PK_id_usuario='" + id + "'";
            OdbcDataAdapter dataconsulta = new OdbcDataAdapter(sqlconsulta, cn.conectar());
            insertarBitacora(idUsuario, "Se inserto el usuario con id: " + id, "tbl_usuarios", "1201");
            return dataconsulta;
        }

        public OdbcDataAdapter update(string usuario)
        {
            cn.conectar();
            string sqlconsulta = "select PK_id_perfil FROM tbl_usuario_perfil where PK_id_usuario = '" + usuario + "'";
            OdbcDataAdapter dataconsulta = new OdbcDataAdapter(sqlconsulta, cn.conectar());
            return dataconsulta;

        }


        public OdbcDataAdapter clienteupdate(string clave, string usuario)
        {
            cn.conectar();
            MessageBox.Show("Contraseña Actualizada");
            string sqlconsulta = "UPDATE tbl_usuario set password_usuario='" + clave + "' where PK_id_usuario='" + usuario + "'";
            OdbcDataAdapter dataconsulta = new OdbcDataAdapter(sqlconsulta, cn.conectar());
            return dataconsulta;
        }


        //###################  lo que hizo Karla  Sofia Gómez Tobar #######################
        public void insertaraplicacion(string codigo, string nombre, string descripcion, string estado)
        {
            try
            {
                // Crear la conexión y el comando
                using (OdbcConnection connection = cn.conectar())
                {
                    string query = "INSERT INTO tbl_aplicaciones (" +
                                   "Pk_id_aplicacion, " +
                                   "nombre_aplicacion, " +
                                   "descripcion_aplicacion, " +
                                   "estado_aplicacion) " +
                                   "VALUES (?, ?, ?, ?)";

                    using (OdbcCommand cmd = new OdbcCommand(query, connection))
                    {
                        // Agregar los parámetros al comando
                        cmd.Parameters.AddWithValue("@Pk_id_modulo", codigo);
                        cmd.Parameters.AddWithValue("@nombre_modulo", nombre);
                        cmd.Parameters.AddWithValue("@descripcion_modulo", descripcion);
                        cmd.Parameters.AddWithValue("@estado_modulo", estado);

                        // Ejecutar el comando
                        cmd.ExecuteNonQuery();

                        insertarBitacora(idUsuario, "Inserto un nuevo modulo: " + codigo + " - " + nombre, "tbl_aplicaciones", "1002");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar la aplicación: " + ex.Message);
            }
        }
        //###################  termina lo que hizo  Karla  Sofia Gómez Tobar #######################

        //---------------------------------------------------- Inicio: GABRIELA SUC ----------------------------------------------------

        // Método para cambiar el estado del usuario en la base de datos
        public bool CambiarEstadoUsuario(string LlaveUsuario, int nuevoEstado)
        {
            try
            {
                // Consulta SQL para actualizar el estado del usuario
                string query = "UPDATE Tbl_usuarios SET estado_usuario = ? WHERE Pk_id_usuario = ?";

                using (OdbcCommand command = new OdbcCommand(query, cn.conectar()))
                {
                    command.Parameters.AddWithValue("estado_usuario", nuevoEstado);
                    command.Parameters.AddWithValue("id_usuario", LlaveUsuario);

                    insertarBitacora(idUsuario, "Se desactivo el usuario: " + LlaveUsuario, "tbl_usuarios", "1001");
                    int result = command.ExecuteNonQuery();
     
                    // Verifica si se actualizó algún registro
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar el estado del usuario: " + ex.Message);
                insertarBitacora(idUsuario, "Ocurrio un error al desactivar el usuario: " + LlaveUsuario, "tbl_usuarios", "1001");
                return false;
            }
        }

        //---------------------------------------------------- Fin: GABRIELA SUC ----------------------------------------------------


        // Esta parte fue echa por Carlos Hernandez
        public OdbcDataAdapter actualizaraplicacion(string codigo, string nombre, string descripcion, string estado)
        {
            Console.WriteLine("ESTO SE INGRESA EN LA SENTENCIA: " + codigo + ", " + nombre + ", " + descripcion + ", " + estado);
            try
            {
                cn.conectar();
                string sqlactualizaraplicacion = "UPDATE tbl_aplicaciones SET nombre_aplicacion = '" + nombre + "', descripcion_aplicacion = '" + descripcion + "', estado_aplicacion = '" + estado + "' WHERE Pk_id_aplicacion ='" + codigo + "'";
                OdbcDataAdapter dataTable = new OdbcDataAdapter(sqlactualizaraplicacion, cn.conectar());
                insertarBitacora(idUsuario, "Actualizo una aplicacion: " + codigo + " - " + nombre, "tbl_aplicaciones", "1003");
                return dataTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

        }
        //termina lo que hizo carlos hernandez 


        public bool eliminaraplicacion(string codigo)
        {
            try
            {

                cn.conectar();


                string sqlEliminarAplicacion = "DELETE FROM tbl_aplicaciones WHERE Pk_id_aplicacion = ?";

                using (OdbcCommand cmd = new OdbcCommand(sqlEliminarAplicacion, cn.conectar()))
                {

                    cmd.Parameters.AddWithValue("@codigo", codigo);


                    int rowsAffected = cmd.ExecuteNonQuery();


                    if (rowsAffected > 0)
                    {
                        insertarBitacora(idUsuario, "Eliminó una aplicacion: " + codigo, "tbl_aplicaciones", "1002");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

        }

        //Kateryn De León
        //MOSTRAR PARA  EL BOTON BUSCAR 
        public OdbcDataAdapter mostrar(string id)
        {
            OdbcConnection connection = cn.conectar();

            string sqlusuarios = "SELECT Pk_id_usuario as Usuario, nombre_usuario as Nombre, apellido_usuario as Apellido, username_usuario as Username, password_usuario as Password, email_usuario as Email, ultima_conexion_usuario as Ultima_Conexion, estado_usuario as Estado, pregunta as Pregunta, respuesta as Respuesta FROM tbl_usuarios where pk_id_usuario = '" + id + "'";

            OdbcCommand command = new OdbcCommand(sqlusuarios, connection);

            try
            {
                // Ejecutar la consulta de inserción
                command.ExecuteNonQuery();
                insertarBitacora(idUsuario, "Se buscaron los datos del usuario con id: " + id, "tbl_usuarios", "1001");
            }
            catch (Exception ex)
            {
                // Manejo de errores
                MessageBox.Show("Error al insertar usuario: " + ex.Message);
            }
            finally
            {
                // Cerrar la conexión
                connection.Close();
            }

            OdbcDataAdapter datausuarios = new OdbcDataAdapter(sqlusuarios, cn.conectar());

            if (datausuarios == null)
            {
                return null;
            }
            else
                return datausuarios;
        }
        public OdbcDataAdapter mostraraplicacion(string id)
        {

            cn.conectar();
            string sqlusuarios = "SELECT PK_id_aplicacion,PK_id_modulo,nombre_aplicacion,descripcion_aplicacion from tbl_aplicacion WHERE PK_id_aplicacion ='" + id + "'";
            OdbcDataAdapter datausuarios = new OdbcDataAdapter(sqlusuarios, cn.conectar());

            if (datausuarios == null)
            {
                return null;
            }
            else
                return datausuarios;
        }

        //****************************************Kevin López***************************************************
        public OdbcDataAdapter consultaraplicaciones(string aplicacion)
        {
            cn.conectar();
            string sqlAplicaciones = "SELECT Pk_id_aplicacion, nombre_aplicacion, descripcion_aplicacion FROM tbl_aplicaciones WHERE Pk_id_aplicacion = ?";

            // Registro de la bitacora
            insertarBitacora(idUsuario, "Realizó una consulta a aplicaciones", "tbl_aplicaciones", "1002");

            OdbcDataAdapter dataTable = new OdbcDataAdapter(sqlAplicaciones, cn.conectar());
            dataTable.SelectCommand.Parameters.AddWithValue("?", aplicacion);

            return dataTable;
        }


        //**************************************** FIN Kevin López***************************************************

        /*-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/


        //Trabajado por María José Véliz Ochoa, 9959-21-5909
        public OdbcDataAdapter validarIDModulos()
        {
            try
            {

                string sqlIDmodulo = "SELECT MAX(Pk_id_modulo)+1 FROM tbl_modulos";
                OdbcDataAdapter dataIDmodulo = new OdbcDataAdapter(sqlIDmodulo, cn.conectar());
                return dataIDmodulo;
                insertarBitacora(idUsuario, "Se mostraron los modulos", "tbl_modulos", "1003");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        // termina
        public OdbcDataAdapter validarIDperfiles()
        {
            try
            {

                string sqlIDperfil = "SELECT MAX(Pk_id_perfil)+1 FROM Tbl_perfiles";
                OdbcDataAdapter dataIDperfil = new OdbcDataAdapter(sqlIDperfil, cn.conectar());
                return dataIDperfil;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }




        public OdbcDataAdapter insertarPerfil(string codigo, string nombre, string descripcion, string estado)
        {
            cn.conectar();
            try
            {
                string sqlPerfil = "INSERT INTO Tbl_perfiles (Pk_id_Perfil, nombre_perfil, descripcion_perfil, estado_perfil) VALUES ('" + codigo + "','" + nombre + "', '" + descripcion + "', " + estado + ");";
                OdbcDataAdapter datainsertarperfil = new OdbcDataAdapter(sqlPerfil, cn.conectar());
                insertarBitacora(idUsuario, "Inserto un nuevo perfil: " + codigo + " - " + nombre, "Tbl_perfiles", "1004");
                return datainsertarperfil;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        //*****************ACA TERMINA LA PRIMERA PARTE ACTUALIZADA POR JOSUÉ DAVID PAZ GÓMEZ*************************


        public OdbcDataAdapter ConsultarPerfil(string perfil)
        {
            cn.conectar();
            string sqlPerfil = "SELECT * FROM Tbl_perfiles WHERE Pk_id_perfil = " + perfil;
            OdbcDataAdapter dataTable = new OdbcDataAdapter(sqlPerfil, cn.conectar());
            insertarBitacora(idUsuario, "Realizo una consulta a perfiles ", "Tbl_perfiles", "1004");

            return dataTable;
        }

        public OdbcDataAdapter ActualizarPerfil(string ID_perfil, string nombre, string descripcion, string estado)
        {
            try
            {
                cn.conectar();
                string sqlactualizarperfil = "UPDATE Tbl_perfiles SET nombre_perfil = '" + nombre + "', descripcion_perfil = '" + descripcion + "', estado_perfil = '" + estado + "' WHERE Pk_id_perfil ='" + ID_perfil + "'";
                OdbcDataAdapter dataTable = new OdbcDataAdapter(sqlactualizarperfil, cn.conectar());
                insertarBitacora(idUsuario, "Actualizo un perfil: " + ID_perfil + " - " + nombre, "Tbl_perfiles", "1004");

                return dataTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }


        //Trabajado por María José Véliz Ochoa 9959-21-5909
        //se optó por usar OdbcCommand en lugar de OdbcDataAdapter, cambió estructura
        public void insertarModulo(string codigo, string nombre, string descripcion, string estado)
        {
            try
            {
                // Crear la conexión y el comando
                using (OdbcConnection connection = cn.conectar())
                {
                    string query = "INSERT INTO tbl_modulos (" +
                                   "Pk_id_modulos, " +
                                   "nombre_modulo, " +
                                   "descripcion_modulo, " +
                                   "estado_modulo) " +
                                   "VALUES (?, ?, ?, ?)";

                    using (OdbcCommand cmd = new OdbcCommand(query, connection))
                    {
                        // Agregar los parámetros al comando
                        cmd.Parameters.AddWithValue("@Pk_id_modulo", codigo);
                        cmd.Parameters.AddWithValue("@nombre_modulo", nombre);
                        cmd.Parameters.AddWithValue("@descripcion_modulo", descripcion);
                        cmd.Parameters.AddWithValue("@estado_modulo", estado);

                        // Ejecutar el comando
                        cmd.ExecuteNonQuery();

                        insertarBitacora(idUsuario, "Inserto un nuevo modulo: " + codigo + " - " + nombre, "tbl_modulos", "1003");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar módulo: " + ex.Message);
            }
        }
        // termina
        //------------

        //****************************************************MODIFICADO POR JOSUE PAZ***********************************************
        public OdbcDataAdapter insertarPermisosPerfilA(string codigoperfil, string nombreaplicacion, string ingresar, string modificar, string eliminar, string consulta, string imprimir)
        {
            string sCodigoAplicacion = " ";
            string sCodigoPerfil = "";

            try
            {
                OdbcCommand sqlCodigoModulo = new OdbcCommand("SELECT Pk_id_aplicacion FROM Tbl_aplicaciones WHERE nombre_aplicacion = '" + nombreaplicacion + "' ", cn.conectar());
                OdbcDataReader almacena = sqlCodigoModulo.ExecuteReader();

                while (almacena.Read() == true)
                {
                    sCodigoAplicacion = almacena.GetString(0);
                }

                OdbcCommand sqlCodigoPerfil = new OdbcCommand("SELECT Pk_id_perfil FROM Tbl_perfiles WHERE nombre_perfil = '" + codigoperfil + "' ", cn.conectar());
                OdbcDataReader almacenaPerfil = sqlCodigoPerfil.ExecuteReader();

                while (almacenaPerfil.Read() == true)
                {
                    sCodigoPerfil = almacenaPerfil.GetString(0);
                }

                string sqlInsertarPermisosPerfilApp = "INSERT INTO Tbl_permisos_aplicacion_perfil(Fk_id_perfil, Fk_id_aplicacion, guardar_permiso, modificar_permiso, eliminar_permiso, buscar_permiso, imprimir_permiso) VALUES ('" + sCodigoPerfil + "', '" + sCodigoAplicacion + "', '" + ingresar + "', '" + modificar + "', '" + eliminar + "', '" + consulta + "', '" + imprimir + "');";
                OdbcDataAdapter dataPermisosPerfilAplicacion = new OdbcDataAdapter(sqlInsertarPermisosPerfilApp, cn.conectar());
                insertarBitacora(idUsuario, "Asigno permiso: " + nombreaplicacion + " a perfil: " + codigoperfil, "Tbl_permisos_aplicacion_perfil", "1103");


                almacena.Close();
                sqlCodigoModulo.Connection.Close();

                return dataPermisosPerfilAplicacion;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

        }

        //****************************ACA TERMINA******************************************************************


        //--------------------------------- Emerzon Garcia -----------------------------------------------------------

        public OdbcDataAdapter mostrarPerfilesYPermisos(string TablaPerfilUsuario)
        {
            try
            {
                // Consulta para obtener perfiles y permisos junto con el nombre de la aplicación
                string sql = @"
            SELECT p.Fk_id_perfil, a.nombre_aplicacion, 
                   p.guardar_permiso, p.buscar_permiso, p.modificar_permiso, 
                   p.eliminar_permiso, p.imprimir_permiso
            FROM Tbl_permisos_aplicacion_perfil p
            JOIN Tbl_aplicaciones a ON p.Fk_id_aplicacion = a.Pk_id_aplicacion
            JOIN Tbl_perfiles pf ON p.Fk_id_perfil = pf.Pk_id_perfil;
        ";

                OdbcDataAdapter dataTable = new OdbcDataAdapter(sql, cn.conectar());
                return dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ejecutar la consulta: " + ex.Message);
                return null;
            }
        }

        //---------------------------------------------------------------------------------------------------------------



        //------------Para formulario Mantenimiento Modulos---------

        //Trabajado por María José Véliz Ochoa, 9959-21-5909
        public OdbcDataAdapter ConsultarModulos(string modulo)
        {
            cn.conectar();
            string sqlModulos = "SELECT * FROM tbl_modulos WHERE Pk_id_modulos = " + modulo;
            insertarBitacora(idUsuario, "Realizo una consulta a modulos", "tbl_modulos", "1003");
            OdbcDataAdapter dataTable = new OdbcDataAdapter(sqlModulos, cn.conectar());
            return dataTable;
        } //termina

        //###############ALYSON RODRIGUEZ BOTON ACTUALIZAR : creo que solo cambie que el nombre de la tabla estuviera bien
        public OdbcDataAdapter ActualizarModulo(string ID_modulo, string nombre, string descripcion, string estado)
        {
            Console.WriteLine("ESTO SE INGRESA EN LA SENTENCIA: " + ID_modulo + ", " + nombre + ", " + descripcion + ", " + estado);
            try
            {
                cn.conectar();
                string sqlactualizarmodulo = "UPDATE tbl_modulos SET nombre_modulo = '" + nombre + "', descripcion_modulo = '" + descripcion + "', estado_modulo = '" + estado + "' WHERE PK_id_modulos ='" + ID_modulo + "'";
                OdbcDataAdapter dataTable = new OdbcDataAdapter(sqlactualizarmodulo, cn.conectar());
                insertarBitacora(idUsuario, "Actualizo un modulo: " + ID_modulo + " - " + nombre, "tbl_modulos", "1003");
                return dataTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        // ########### FIN  ##########################################



        // Inicio Fernando Garcia - Brandon Boch

        public DataSet consultarBitacora()
        {
            OdbcDataAdapter dat = null;
            DataSet ds = null;
            try
            {
                ds = new DataSet();
                dat = new OdbcDataAdapter("SELECT PK_id_bitacora as Id, FK_id_usuario as Usuario, fecha_bitacora as Fecha, hora_bitacora as Hora, host_bitacora as Host, ip_bitacora as IP, accion_bitacora as Accion, tabla as Tabla, aplicacion as Aplicacion from tbl_bitacora"
                , cn.conectar());
                dat.Fill(ds);
            }
            catch (OdbcException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return ds;
        }

        // Nuevo método para búsqueda filtrada
        public DataSet consultarBitacoraFiltrada(string campo, string valor)
        {
            OdbcDataAdapter dat = null;
            DataSet ds = null;
            try
            {
                ds = new DataSet();
                string query = $"SELECT PK_id_bitacora as Id, FK_id_usuario as Usuario, fecha_bitacora, hora_bitacora, host_bitacora, ip_bitacora, accion_bitacora, tabla, aplicacion FROM tbl_bitacora WHERE {campo} LIKE ?";

                using (OdbcConnection conexion = cn.conectar())
                {
                    dat = new OdbcDataAdapter(query, conexion);
                    dat.SelectCommand.Parameters.AddWithValue("?", $"%{valor}%");
                    dat.Fill(ds);
                }
            }
            catch (OdbcException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return ds;
        }

        public void insertarBitacora(string idUsuario, string accion, string tabla, string idAplicacion)
        {
            try
            {
                string ipLocal = ObtenerDireccionIPLocal();
                string nombreHost = Dns.GetHostName();

                using (OdbcConnection conexion = cn.conectar())
                {
                    string obtenerIdUsuarioQuery = "SELECT Pk_id_usuario FROM Tbl_usuarios WHERE username_usuario = ?";
                    OdbcCommand obtenerIdUsuarioCmd = new OdbcCommand(obtenerIdUsuarioQuery, conexion);
                    obtenerIdUsuarioCmd.Parameters.AddWithValue("?", idUsuario);



                    object resultado = obtenerIdUsuarioCmd.ExecuteScalar();
                    if (resultado != null)
                    {
                        string usuario = resultado.ToString();

                        string consulta = @"INSERT INTO tbl_bitacora 
                                (Fk_id_usuario, fecha_bitacora, hora_bitacora, host_bitacora, ip_bitacora, accion_bitacora, tabla, aplicacion) 
                                VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

                        using (OdbcCommand cmd = new OdbcCommand(consulta, conexion))
                        {
                            cmd.Parameters.AddWithValue("?", usuario);
                            cmd.Parameters.AddWithValue("?", DateTime.Now.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("?", DateTime.Now.ToString("HH:mm:ss"));
                            cmd.Parameters.AddWithValue("?", nombreHost);
                            cmd.Parameters.AddWithValue("?", ipLocal);
                            cmd.Parameters.AddWithValue("?", accion);
                            cmd.Parameters.AddWithValue("?", tabla);
                            cmd.Parameters.AddWithValue("?", idAplicacion);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Registrar la excepción o manejarla apropiadamente
                MessageBox.Show("Error al insertar en la bitácora: " + ex.Message);
            }
        }
        private string ObtenerDireccionIPLocal()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "No se pudo determinar la dirección IP local";
        }

        //Fin #########################3

        /************************Ismar Leonel Cortez Sanchez*********************************************************************/
        /*Funcion de ConsultarPermisos()*****************************************************************************************/
        public bool consultarPermisos(string idUsuario, string idAplicacion, int tipoPermiso)
        {
            try
            {
                switch (tipoPermiso)
                {
                    case 1:
                        OdbcCommand sql = new OdbcCommand("Select guardar_permiso from Tbl_permisos_aplicaciones_usuario WHERE Fk_id_usuario= '" + idUsuario + "' AND Fk_id_aplicacion ='" + idAplicacion + "'", cn.conectar());
                        OdbcDataReader almacena = sql.ExecuteReader();

                        if (almacena.Read() == true)
                        {
                            if (almacena.GetString(0) == "1")
                            {
                                almacena.Close();
                                sql.Connection.Close();
                                MessageBox.Show("es igual a true 1");
                                return true;

                            }
                        }

                        sql = new OdbcCommand("Select Tbl_permisos_aplicacion_perfil.guardar_permiso from Tbl_permisos_aplicacion_perfil " +
                            "INNER JOIN Tbl_asignaciones_perfils_usuario ON Tbl_permisos_aplicacion_perfil.Fk_id_perfil = Tbl_asignaciones_perfils_usuario.Fk_id_perfil" +
                            " WHERE Tbl_asignaciones_perfils_usuario.Fk_id_usuario= '" + idUsuario + "' AND Tbl_permisos_aplicacion_perfil.Fk_id_aplicacion ='" + idAplicacion + "'", cn.conectar());
                        almacena = sql.ExecuteReader();

                        if (almacena.Read() == true)
                        {
                            if (almacena.GetString(0) == "1")
                            {
                                almacena.Close();
                                sql.Connection.Close();
                                MessageBox.Show("es igual a true 1");
                                return true;
                            }
                        }

                        break;

                    case 2:
                        sql = new OdbcCommand("Select buscar_permiso from Tbl_permisos_aplicaciones_usuario WHERE Fk_id_usuario= '" + idUsuario + "' AND Fk_id_aplicacion ='" + idAplicacion + "'", cn.conectar());
                        almacena = sql.ExecuteReader();

                        if (almacena.Read() == true)
                        {
                            if (almacena.GetString(0) == "1")
                            {
                                almacena.Close();
                                sql.Connection.Close();
                                return true;
                            }
                        }

                        sql = new OdbcCommand("Select Tbl_permisos_aplicacion_perfil.buscar_permiso from Tbl_permisos_aplicacion_perfil " +
                            "INNER JOIN Tbl_asignaciones_perfils_usuario ON Tbl_permisos_aplicacion_perfil.Fk_id_perfil = Tbl_asignaciones_perfils_usuario.Fk_id_perfil" +
                            " WHERE Tbl_asignaciones_perfils_usuario.Fk_id_usuario= '" + idUsuario + "' AND Tbl_permisos_aplicacion_perfil.Fk_id_aplicacion ='" + idAplicacion + "'", cn.conectar());
                        almacena = sql.ExecuteReader();

                        if (almacena.Read() == true)
                        {
                            if (almacena.GetString(0) == "1")
                            {
                                almacena.Close();
                                sql.Connection.Close();
                                return true;
                            }
                        }

                        break;

                    case 3:
                        sql = new OdbcCommand("Select modificar_permiso from Tbl_permisos_aplicaciones_usuario WHERE Fk_id_usuario= '" + idUsuario + "' AND Fk_id_aplicacion ='" + idAplicacion + "'", cn.conectar());
                        almacena = sql.ExecuteReader();

                        if (almacena.Read() == true)
                        {
                            if (almacena.GetString(0) == "1")
                            {
                                almacena.Close();
                                sql.Connection.Close();
                                return true;
                            }
                        }

                        sql = new OdbcCommand("Select Tbl_permisos_aplicacion_perfil.modificar_permiso from Tbl_permisos_aplicacion_perfil " +
                            "INNER JOIN Tbl_asignaciones_perfils_usuario ON Tbl_permisos_aplicacion_perfil.Fk_id_perfil = Tbl_asignaciones_perfils_usuario.Fk_id_perfil" +
                            " WHERE Tbl_asignaciones_perfils_usuario.Fk_id_usuario= '" + idUsuario + "' AND Tbl_permisos_aplicacion_perfil.Fk_id_aplicacion ='" + idAplicacion + "'", cn.conectar());
                        almacena = sql.ExecuteReader();

                        if (almacena.Read() == true)
                        {
                            if (almacena.GetString(0) == "1")
                            {
                                almacena.Close();
                                sql.Connection.Close();
                                return true;
                            }
                        }

                        break;

                    case 4:
                        sql = new OdbcCommand("Select eliminar_permiso from Tbl_permisos_aplicaciones_usuario WHERE Fk_id_usuario= '" + idUsuario + "' AND Fk_id_aplicacion ='" + idAplicacion + "'", cn.conectar());
                        almacena = sql.ExecuteReader();

                        if (almacena.Read() == true)
                        {
                            if (almacena.GetString(0) == "1")
                            {
                                almacena.Close();
                                sql.Connection.Close();
                                return true;
                            }
                        }

                        sql = new OdbcCommand("Select Tbl_permisos_aplicacion_perfil.eliminar_permiso from Tbl_permisos_aplicacion_perfil " +
                            "INNER JOIN Tbl_asignaciones_perfils_usuario ON Tbl_permisos_aplicacion_perfil.Fk_id_perfil = Tbl_asignaciones_perfils_usuario.Fk_id_perfil" +
                            " WHERE Tbl_asignaciones_perfils_usuario.Fk_id_usuario= '" + idUsuario + "' AND Tbl_permisos_aplicacion_perfil.Fk_id_aplicacion ='" + idAplicacion + "'", cn.conectar());
                        almacena = sql.ExecuteReader();

                        if (almacena.Read() == true)
                        {
                            if (almacena.GetString(0) == "1")
                            {
                                almacena.Close();
                                sql.Connection.Close();
                                return true;
                            }
                        }

                        break;

                    case 5:
                        sql = new OdbcCommand("Select imprimir_permiso from Tbl_permisos_aplicaciones_usuario WHERE Fk_id_usuario= '" + idUsuario + "' AND Fk_id_aplicacion ='" + idAplicacion + "'", cn.conectar());
                        almacena = sql.ExecuteReader();

                        if (almacena.Read() == true)
                        {
                            if (almacena.GetString(0) == "1")
                            {
                                almacena.Close();
                                sql.Connection.Close();
                                return true;
                            }
                        }

                        sql = new OdbcCommand("Select Tbl_permisos_aplicacion_perfil.imprimir_permiso from Tbl_permisos_aplicacion_perfil " +
                            "INNER JOIN Tbl_asignaciones_perfils_usuario ON Tbl_permisos_aplicacion_perfil.Fk_id_perfil = Tbl_asignaciones_perfils_usuario.Fk_id_perfil" +
                            " WHERE Tbl_asignaciones_perfils_usuario.Fk_id_usuario= '" + idUsuario + "' AND Tbl_permisos_aplicacion_perfil.Fk_id_aplicacion ='" + idAplicacion + "'", cn.conectar());
                        almacena = sql.ExecuteReader();

                        if (almacena.Read() == true)
                        {
                            if (almacena.GetString(0) == "1")
                            {
                                almacena.Close();
                                sql.Connection.Close();
                                return true;
                            }
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return false;
        }

        /*Se tendrian que recorrer un par de iteraciones el consultarpermisos() en donde ahi se ver que permisos como tal tiene el usuario e ir habilitando los botones como tal
         
          
         for each(hasta 5){
        int tipoPermiso=1;
         consultarPermisos(string idUsuario, string idAplicacion, int tipoPermiso)
        tipoPermiso=+1;
        
        } 

        /*************************************************************************************************************************************************************/

        //###############INICIA CÓDIGO PARA BOTON ELIMINAR ALYSON RODRÍGUEZ 
        public OdbcDataAdapter EliminarModulo(string ID_modulo, string nombre, string descripcion, string estado)
        {
            Console.WriteLine("ESTO SE INGRESA EN LA SENTENCIA: " + ID_modulo + ", " + nombre + ", " + descripcion + ", " + estado);
            try
            {
                using (OdbcConnection connection = cn.conectar())
                {
                    string sqlBorrarModulo = "UPDATE tbl_modulos SET nombre_modulo = ?, descripcion_modulo = ?, estado_modulo = '0' WHERE PK_id_modulos = ?";

                    using (OdbcCommand command = new OdbcCommand(sqlBorrarModulo, connection))
                    {
                        // Agregar parámetros al comando
                        command.Parameters.AddWithValue("?", nombre);
                        command.Parameters.AddWithValue("?", descripcion);
                        command.Parameters.AddWithValue("?", ID_modulo);

                        // Crear un OdbcDataAdapter para ejecutar el comando de actualización
                        OdbcDataAdapter adapter = new OdbcDataAdapter();
                        adapter.UpdateCommand = command;

                        // Ejecutar el comando de actualización
                        adapter.UpdateCommand.ExecuteNonQuery();

                        // Registrar la acción en la bitácora
                        insertarBitacora(idUsuario, "Eliminó un módulo: " + ID_modulo + " - " + nombre, "tbl_modulos", "1003");

                        return adapter; // Aunque no se usa típicamente así, se retorna el adaptador
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al realizar el borrado lógico del módulo: " + ex.Message);
                return null;
            }
        }
        //########################FINALIZA CÓDIGO BOTÓN ELIMINAR ALYSON RODRIGUEZ


        /*******************Ismar Leonel Cortez Sanchez  -0901-21-560*******************************************************/
        /****************************Combo box inteligente******************************************************************/


        public string[] llenarCmb(string tabla, string campo1, string campo2)
        {
            conexion cn = new conexion();
            string[] Campos = new string[300];
            int i = 0;

            string sql = "SELECT DISTINCT " + campo1 + "," + campo2 + " FROM " + tabla;

            /* La sentencia consulta el modelo de la base de datos con cada campo */
            try
            {
                // Muestra la consulta SQL antes de ejecutarla
                Console.Write(sql);
                MessageBox.Show(sql);

                OdbcCommand command = new OdbcCommand(sql, cn.conectar());
                OdbcDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Campos[i] = reader.GetValue(0).ToString() + "-" + reader.GetValue(1).ToString();
                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nError en asignarCombo, revise los parámetros \n -" + tabla + "\n -" + campo1);
            }

            return Campos;
        }


        public DataTable obtener(string tabla, string campo1, string campo2)
        {
            conexion cn = new conexion();
            string sql = "SELECT DISTINCT " + campo1 + "," + campo2 + " FROM " + tabla;

            OdbcCommand command = new OdbcCommand(sql, cn.conectar());
            OdbcDataAdapter adaptador = new OdbcDataAdapter(command);
            DataTable dt = new DataTable();
            adaptador.Fill(dt);


            return dt;
        }
        /****************************************************************************************************************/

        /*******************Ismar Leonel Cortez Sanchez  -0901-21-560*******************************************************/
        /****************************Combo box inteligente 2******************************************************************/


        public string[] llenarCmb2(string tabla, string campo1, string campo2)
        {
            conexion cn = new conexion();
            string[] Campos = new string[300];
            int i = 0;

            string sql = "SELECT DISTINCT " + campo1 + "," + campo2 + " FROM " + tabla;

            /* La sentencia consulta el modelo de la base de datos con cada campo */
            try
            {
                // Muestra la consulta SQL antes de ejecutarla
                Console.Write(sql);
                MessageBox.Show(sql);

                OdbcCommand command = new OdbcCommand(sql, cn.conectar());
                OdbcDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Campos[i] = reader.GetValue(0).ToString() + "-" + reader.GetValue(1).ToString();
                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nError en asignarCombo, revise los parámetros \n -" + tabla + "\n -" + campo1);
            }

            return Campos;
        }


        public DataTable obtener2(string tabla, string campo1, string campo2)
        {
            conexion cn = new conexion();
            string sql = "SELECT DISTINCT " + campo1 + "," + campo2 + " FROM " + tabla;

            OdbcCommand command = new OdbcCommand(sql, cn.conectar());
            OdbcDataAdapter adaptador = new OdbcDataAdapter(command);
            DataTable dt = new DataTable();
            adaptador.Fill(dt);


            return dt;
        }
        /****************************************************************************************************************/
        //******************************************KATERYN DE LEON***************************
        //BUSCAR
        public OdbcDataAdapter consultarAsignacion_moduloAplicaciones()
        {
            cn.conectar();
            string sqlUsuarios = " SELECT  a.Fk_id_modulos AS ModuloID,   m.nombre_modulo AS NombreModulo, a.Fk_id_aplicacion AS AplicacionID,  ap.nombre_aplicacion AS NombreAplicacion  FROM Tbl_asignacion_modulo_aplicacion a  JOIN Tbl_modulos m ON a.Fk_id_modulos = m.Pk_id_modulos JOIN Tbl_aplicaciones ap ON a.Fk_id_aplicacion = ap.Pk_id_aplicacion";
            OdbcDataAdapter dataUsuarios = new OdbcDataAdapter(sqlUsuarios, cn.conectar());
            insertarBitacora(idUsuario, "Realizo una consulta  a Asignacion modulo aplicaciones", "tbl_asignacion_modulo_aplicacion", "1101");
            return dataUsuarios;

        }

        //*************************************KATERYN DE LEON*********************************************************
        // AGREGAR
        public int ObtenerIdModulo(string nombreModulo)
        {
            using (OdbcConnection connection = cn.conectar())
            {
                if (connection == null) return -1;

                try
                {
                    string query = "SELECT Pk_id_modulos FROM Tbl_modulos WHERE nombre_modulo = ?";
                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre_modulo", nombreModulo);
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            // Si no existe, inserta el módulo
                            string insertQuery = "INSERT INTO Tbl_modulos (nombre_modulo, descripcion_modulo) VALUES (?, ?)";
                            using (OdbcCommand insertCommand = new OdbcCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@nombre_modulo", nombreModulo);
                                insertCommand.Parameters.AddWithValue("@descripcion_modulo", "Descripción del módulo"); // Ajusta según sea necesario
                                insertCommand.ExecuteNonQuery();
                            }

                            // Obtener el ID del nuevo módulo
                            string idQuery = "SELECT LAST_INSERT_ID()";
                            using (OdbcCommand idCommand = new OdbcCommand(idQuery, connection))
                            {
                                return Convert.ToInt32(idCommand.ExecuteScalar());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener o insertar módulo: " + ex.Message);
                    return -1;
                }
            }
        }
        //********************************************KATERYN DE LEON*********************************************************
        // AGREGAR
        public int ObtenerIdAplicacion(string nombreAplicacion)
        {
            using (OdbcConnection connection = cn.conectar())
            {
                if (connection == null) return -1;

                try
                {
                    string query = "SELECT Pk_id_aplicacion FROM Tbl_aplicaciones WHERE nombre_aplicacion = ?";
                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre_aplicacion", nombreAplicacion);
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            // Si no existe, inserta la aplicación
                            string insertQuery = "INSERT INTO Tbl_aplicaciones (nombre_aplicacion, descripcion_aplicacion) VALUES (?, ?)";
                            using (OdbcCommand insertCommand = new OdbcCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@nombre_aplicacion", nombreAplicacion);
                                insertCommand.Parameters.AddWithValue("@descripcion_aplicacion", "Descripción de la aplicación"); // Ajusta según sea necesario
                                insertCommand.ExecuteNonQuery();
                            }

                            // Obtener el ID de la nueva aplicación
                            string idQuery = "SELECT LAST_INSERT_ID()";
                            using (OdbcCommand idCommand = new OdbcCommand(idQuery, connection))
                            {
                                return Convert.ToInt32(idCommand.ExecuteScalar());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener o insertar aplicación: " + ex.Message);
                    return -1;
                }
            }
        }
        //*************************************************KATERYN DE LEON*********************************************************
        // AGREGAR
        public bool InsertarAsignacionModuloAplicacion(int idModulo, int idAplicacion)
        {
            using (OdbcConnection connection = cn.conectar())
            {
                if (connection == null) return false;

                try
                {
                    string query = "INSERT INTO Tbl_asignacion_modulo_aplicacion (Fk_id_modulos, Fk_id_aplicacion) VALUES (?, ?)";
                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Fk_id_modulos", idModulo);
                        command.Parameters.AddWithValue("@Fk_id_aplicacion", idAplicacion);

                        int resultado = command.ExecuteNonQuery();
                        insertarBitacora(idUsuario, "Realizo un ingreso a aplicacion-modulos", "tbl_asignacion_modulo_aplicacion", "1101");
                        return resultado > 0; // Devuelve true si la inserción fue exitosa


                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al insertar asignación: " + ex.Message);
                    return false;
                }
            }
        }
        //***************************************************************************************************/

        //###################  empieza lo que hizo Karla  Sofia Gómez Tobar #######################
        // combo usuario y perfil

        public string[] llenarCmbUsuario(string tabla, string campo1, string campo2)
        {
            conexion cn = new conexion();
            string[] Campos = new string[300];
            int i = 0;

            string sql = "SELECT DISTINCT " + campo1 + "," + campo2 + " FROM " + tabla;

            /* La sentencia consulta el modelo de la base de datos con cada campo */
            try
            {
                // Muestra la consulta SQL antes de ejecutarla
                Console.Write(sql);
                MessageBox.Show(sql);

                OdbcCommand command = new OdbcCommand(sql, cn.conectar());
                OdbcDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Campos[i] = reader.GetValue(0).ToString() + "-" + reader.GetValue(1).ToString();
                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nError en asignarCombo, revise los parámetros \n -" + tabla + "\n -" + campo1);
            }

            return Campos;
        }


        public DataTable obtenerUsuario(string tabla, string campo1, string campo2)
        {
            conexion cn = new conexion();
            string sql = "SELECT DISTINCT " + campo1 + "," + campo2 + " FROM " + tabla;

            OdbcCommand command = new OdbcCommand(sql, cn.conectar());
            OdbcDataAdapter adaptador = new OdbcDataAdapter(command);
            DataTable dt = new DataTable();
            adaptador.Fill(dt);


            return dt;
        }
        /****************************************************************************************************************/

        public string[] llenarCmbPerfiles(string tabla, string campo1, string campo2)
        {
            conexion cn = new conexion();
            string[] Campos = new string[300];
            int i = 0;

            string sql = "SELECT DISTINCT " + campo1 + "," + campo2 + " FROM " + tabla;


            try
            {

                Console.Write(sql);
                MessageBox.Show(sql);

                OdbcCommand command = new OdbcCommand(sql, cn.conectar());
                OdbcDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Campos[i] = reader.GetValue(0).ToString() + "-" + reader.GetValue(1).ToString();
                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nError en asignarCombo, revise los parámetros \n -" + tabla + "\n -" + campo1);
            }

            return Campos;
        }


        public DataTable obtenerPerfiles(string tabla, string campo1, string campo2)
        {
            conexion cn = new conexion();
            string sql = "SELECT DISTINCT " + campo1 + "," + campo2 + " FROM " + tabla;

            OdbcCommand command = new OdbcCommand(sql, cn.conectar());
            OdbcDataAdapter adaptador = new OdbcDataAdapter(command);
            DataTable dt = new DataTable();
            adaptador.Fill(dt);


            return dt;
        }

        // ////////////////////////////////////////////////////////////////


        public string[] llenarCboAsigUP(string tabla, string campo1)
        {
            conexion cn = new conexion();
            string[] Campos = new string[300];
            int i = 0;

            string sql = "SELECT DISTINCT " + campo1 + " FROM " + tabla;


            try
            {

                Console.Write(sql);
                MessageBox.Show(sql);

                OdbcCommand command = new OdbcCommand(sql, cn.conectar());
                OdbcDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Campos[i] = reader.GetValue(0).ToString();
                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nError en asignarCombo, revise los parámetros \n -" + tabla + "\n -" + campo1);
            }

            return Campos;
        }


        public DataTable obtenerAsigUP(string tabla, string campo1)
        {
            conexion cn = new conexion();
            string sql = "SELECT DISTINCT " + campo1 + " FROM " + tabla;

            OdbcCommand command = new OdbcCommand(sql, cn.conectar());
            OdbcDataAdapter adaptador = new OdbcDataAdapter(command);
            DataTable dt = new DataTable();
            adaptador.Fill(dt);


            return dt;
        }
        //###################  termina lo que hizo  Karla  Sofia Gómez Tobar #######################

        //*********************************KEVIN LOPEZ*********************************************
        public OdbcDataAdapter consultarAplicacionesP(string nombreModulo)
        {
            cn.conectar();

            try
            {
                string sqlAplicaciones = @"
        SELECT a.Pk_id_aplicacion, a.nombre_aplicacion 
        FROM tbl_aplicaciones a
        JOIN tbl_asignacion_modulo_aplicacion ama ON a.pk_id_aplicacion = ama.fk_id_aplicacion
        JOIN tbl_modulos m ON m.pk_id_modulos = ama.fk_id_modulos
        WHERE m.nombre_modulo = ?";

                OdbcDataAdapter dataAplicaciones = new OdbcDataAdapter(sqlAplicaciones, cn.conectar());
                dataAplicaciones.SelectCommand.Parameters.AddWithValue("?", nombreModulo);

                // Registro de la bitacora
                insertarBitacora(idUsuario, "Realizó una consulta a aplicaciones", "tbl_aplicacion", "1002");

                return dataAplicaciones;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

//*********************************FIN KEVIN LOPEZ*********************************************


    }
}