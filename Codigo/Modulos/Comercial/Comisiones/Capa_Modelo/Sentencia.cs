﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using Capa_Modelo_Seguridad;
using System.Windows.Forms;

namespace Capa_Modelo
{
    public class Sentencia
    {
        private Conexion cn = new Conexion();

        public string sIdUsuario { get; set; }


    // Método para obtener el listado de vendedores
    public OdbcDataAdapter funObtenerVendedores()
        {
            try
            {
                string sQuery = "SELECT Pk_id_vendedor, CONCAT(vendedores_nombre, ' ', vendedores_apellido) AS NombreCompleto FROM Tbl_vendedores";
                OdbcDataAdapter vendedoresAdapter = new OdbcDataAdapter(sQuery, cn.conectar());

                return vendedoresAdapter;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los vendedores: " + ex.Message);
                return null;
            }
        }

        // Método para obtener ventas de un vendedor según filtros
        public OdbcDataAdapter FunObtenerVentasPorVendedor(int iIdVendedor, string sFiltro, DateTime dFechaInicio, DateTime dFechaFin, string sValorFiltro)
        {
            try
            {
                string query = $"SELECT Tbl_factura_encabezado.Pk_id_facturaEnc AS IdVenta, " +
                               $"Tbl_factura_encabezado.CotizacionEnc_fechaCrea AS FechaVenta, " +
                               $"Tbl_Productos.nombreProducto AS Producto, " +
                               $"Tbl_Marca.nombre_Marca AS Marca, " +
                               $"Tbl_Linea.nombre_linea AS Linea, " +
                               $"Tbl_pedido_detalle.PedidoDet_cantidad AS CantidadVendida, " +
                               $"Tbl_factura_encabezado.facturaEnc_total AS Total, ";

                // Agrega el campo de comisión según el filtro seleccionado
                if (sFiltro == "Inventario")
                {
                    query += $"Tbl_Productos.comisionInventario AS Comision ";
                }
                else if (sFiltro == "Marcas")
                {
                    query += $"Tbl_Marca.comision AS Comision ";
                }
                else if (sFiltro == "Lineas")
                {
                    query += $"Tbl_Linea.comision AS Comision ";
                }
                else if (sFiltro == "Costo")
                {
                    query += $"Tbl_Productos.comisionCosto AS Comision ";
                }

                query += "FROM Tbl_factura_encabezado " +
                         "JOIN Tbl_pedido_encabezado ON Tbl_factura_encabezado.Fk_id_PeidoEnc = Tbl_pedido_encabezado.Pk_id_PedidoEnc " +
                         "JOIN Tbl_factura_detalle ON Tbl_factura_encabezado.Pk_id_facturaEnc = Tbl_factura_detalle.Fk_id_facturaEnc " +
                         "JOIN Tbl_pedido_detalle ON Tbl_pedido_encabezado.Pk_id_PedidoEnc = Tbl_pedido_detalle.Fk_id_pedidoEnc " +
                         "JOIN Tbl_Productos ON Tbl_factura_detalle.Fk_id_producto = Tbl_Productos.Pk_id_Producto " +
                         "JOIN Tbl_Marca ON Tbl_Productos.Pk_id_Producto = Tbl_Marca.fk_id_Producto " +
                         "JOIN Tbl_Linea ON Tbl_Marca.Pk_id_Marca = Tbl_Linea.fk_id_marca " +
                         "WHERE Tbl_pedido_encabezado.Fk_id_vendedor = ? AND Tbl_factura_encabezado.CotizacionEnc_fechaCrea BETWEEN ? AND ?";

                // Condiciones adicionales para filtrar por marca o línea si corresponde
                if (sFiltro == "Marcas")
                {
                    query += " AND Tbl_Marca.Pk_id_Marca = ?";
                }
                else if (sFiltro == "Lineas")
                {
                    query += " AND Tbl_Linea.Pk_id_linea = ?";
                }

                OdbcCommand command = new OdbcCommand(query, cn.conectar());
                command.Parameters.AddWithValue("?", iIdVendedor);
                command.Parameters.AddWithValue("?", dFechaInicio);
                command.Parameters.AddWithValue("?", dFechaFin);

                if (sFiltro == "Marcas" || sFiltro == "Lineas")
                {
                    command.Parameters.AddWithValue("?", sValorFiltro);
                }

                OdbcDataAdapter ventasAdapter = new OdbcDataAdapter(command);
                return ventasAdapter;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener las ventas filtradas: " + ex.Message);
                return null;
            }
        }

        // Método para insertar un encabezado de comisión
        public void funInsertarComisionEncabezado(int iIdVendedor, decimal deTotalVenta, decimal deTotalComision)
        {
            try
            {
                string query = "INSERT INTO Tbl_comisiones_encabezado (Fk_id_vendedor, Comisiones_fecha_, Comisiones_total_venta, Comisiones_total_comision) VALUES (?, ?, ?, ?)";
                using (OdbcCommand command = new OdbcCommand(query, cn.conectar()))
                {
                    command.Parameters.AddWithValue("@Fk_id_vendedor", iIdVendedor);
                    command.Parameters.AddWithValue("@Comisiones_fecha_", DateTime.Now);
                    command.Parameters.AddWithValue("@Comisiones_total_venta", deTotalVenta);
                    command.Parameters.AddWithValue("@Comisiones_total_comision", deTotalComision);
                    command.ExecuteNonQuery();
                }
                // Crear instancia de la clase Sentencia en Capa_modelo_seguridad
                var bitacora = new Capa_Modelo_Seguridad.sentencia();

                // Llama a la función de bitácora
                bitacora.funInsertarBitacora(sIdUsuario, "Realizó una insercion a la tabla de Comisiones encabezado", "Tbl_comisiones_encabezado", "3000");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al insertar en la tabla Comisiones encabezado: " + ex.Message);
            }
        }

        // Método para insertar un detalle de comisión
        public void funInsertarComisionDetalle(int iIdComisionEnc, string sIdFactura, decimal dePorcentaje, decimal deMontoVenta, decimal deMontoComision)
        {
            try
            {
                string query = "INSERT INTO Tbl_detalle_comisiones (Fk_id_comisionEnc, Fk_id_facturaEnc, Comisiones_porcentaje, Comisiones_monto_venta, Comisiones_monto_comision) VALUES (?, ?, ?, ?, ?)";
                using (OdbcCommand command = new OdbcCommand(query, cn.conectar()))
                {
                    command.Parameters.AddWithValue("@Fk_id_comisionEnc", iIdComisionEnc);
                    command.Parameters.AddWithValue("@Fk_id_facturaEnc", sIdFactura);
                    command.Parameters.AddWithValue("@Comisiones_porcentaje", dePorcentaje);
                    command.Parameters.AddWithValue("@Comisiones_monto_venta", deMontoVenta);
                    command.Parameters.AddWithValue("@Comisiones_monto_comision", deMontoComision);
                    command.ExecuteNonQuery();
                }

                // Crear instancia de la clase Sentencia en Capa_modelo_seguridad
                var bitacora = new Capa_Modelo_Seguridad.sentencia();

                // Llama a la función de bitácora
                bitacora.funInsertarBitacora(sIdUsuario, "Realizó una insercion a la tabla de Detalle Comisiones", "Tbl_comisiones_encabezado", "3000");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al insertar en la tabla detalle comisiones: " + ex.Message);
            }
        }

        // Método para insertar en Tbl_comisiones_encabezado
        public int funInsertarComisionEncabezado(int iIdVendedor, DateTime dFecha, decimal deTotalVenta, decimal deTotalComision)
        {
            try
            {
                int iNuevoId = funObtenerSiguienteIdComisionEncabezado();

                string queryInsert = "INSERT INTO Tbl_comisiones_encabezado (Pk_id_comisionEnc, Fk_id_vendedor, Comisiones_fecha_, Comisiones_total_venta, Comisiones_total_comision) " +
                                     "VALUES (?, ?, ?, ?, ?);";
                using (OdbcCommand command = new OdbcCommand(queryInsert, cn.conectar()))
                {
                    command.Parameters.AddWithValue("@Pk_id_comisionEnc", iNuevoId);
                    command.Parameters.AddWithValue("@Fk_id_vendedor", iIdVendedor);
                    command.Parameters.AddWithValue("@Comisiones_fecha_", dFecha);
                    command.Parameters.AddWithValue("@Comisiones_total_venta", deTotalVenta);
                    command.Parameters.AddWithValue("@Comisiones_total_comision", deTotalComision);
                    command.ExecuteNonQuery();
                }

                // Crear instancia de la clase Sentencia en Capa_modelo_seguridad
                var bitacora = new Capa_Modelo_Seguridad.sentencia();

                // Llama a la función de bitácora
                bitacora.funInsertarBitacora(sIdUsuario, "Realizó una insercion a la tabla de Comisiones encabezado", "Tbl_comisiones_encabezado", "3000");

                return iNuevoId;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al insertar en la tabla Comisiones encabezado: " + ex.Message);
                return 0;
            }
            
        }

        // Método para insertar en Tbl_detalle_comisiones
        public void funInsertarDetalleComision(int iIdComisionEnc, string sIdFactura, decimal dePorcentaje, decimal deMontoVenta, decimal deMontoComision)
        {
            try
            {

                // Mostrar los valores que se van a insertar
                Console.WriteLine("Insertando en Tbl_detalle_comisiones con los siguientes valores:");
                Console.WriteLine($"ID ComisionEnc: {iIdComisionEnc}");
                Console.WriteLine($"ID FacturaEnc: {sIdFactura}");
                Console.WriteLine($"Porcentaje: {dePorcentaje}");
                Console.WriteLine($"Monto Venta: {deMontoVenta}");
                Console.WriteLine($"Monto Comision: {deMontoComision}");


                int nuevoId = funObtenerProximoIdDetalleComision(); // Obtener el nuevo ID para Pk_id_detalle_comision
                Console.WriteLine($"Nuevo ID obtenido para Detalle Comisiones: {nuevoId}");

                string query = "INSERT INTO Tbl_detalle_comisiones (Pk_id_detalle_comision, Fk_id_comisionEnc, Fk_id_facturaEnc, Comisiones_porcentaje, Comisiones_monto_venta, Comisiones_monto_comision) " +
                               "VALUES (?, ?, ?, ?, ?, ?)";

                Console.WriteLine("Consulta SQL: " + query);
                using (OdbcCommand command = new OdbcCommand(query, cn.conectar()))
                {
                    command.Parameters.AddWithValue("@Pk_id_detalle_comision", nuevoId);
                    command.Parameters.AddWithValue("@Fk_id_comisionEnc", iIdComisionEnc);
                    command.Parameters.AddWithValue("@Fk_id_facturaEnc", sIdFactura);
                    command.Parameters.AddWithValue("@Comisiones_porcentaje", dePorcentaje);
                    command.Parameters.AddWithValue("@Comisiones_monto_venta", deMontoVenta);
                    command.Parameters.AddWithValue("@Comisiones_monto_comision", deMontoComision);
                    command.ExecuteNonQuery();
                }

                // Crear instancia de la clase Sentencia en Capa_modelo_seguridad
                var bitacora = new Capa_Modelo_Seguridad.sentencia();

                // Llama a la función de bitácora
                bitacora.funInsertarBitacora(sIdUsuario, "Realizó una insercion a la tabla de detalle comisiones", "Tbl_detalle_comisiones", "3000");

            }
            catch (OdbcException ex)
            {
                // Captura la excepción OdbcException y muestra el mensaje de error
                Console.WriteLine("Excepción producida: " + ex.Message);
                Console.WriteLine("Detalles del error: " + ex.StackTrace);
                Console.WriteLine("Código del error ODBC: " + ex.ErrorCode);
            }
        }

        // Método para obtener el siguiente Pk_id_comisionEnc
        public int funObtenerSiguienteIdComisionEncabezado()
        {
            try
            {
                int iSiguienteId = 1;
                string sQuery = "SELECT MAX(Pk_id_comisionEnc) FROM Tbl_comisiones_encabezado";

                using (OdbcCommand command = new OdbcCommand(sQuery, cn.conectar()))
                {
                    var resultado = command.ExecuteScalar();
                    if (resultado != DBNull.Value)
                    {
                        iSiguienteId = Convert.ToInt32(resultado) + 1;
                    }
                }
                return iSiguienteId;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener el siguiente ID: " + ex.Message);
                return 0;
            }    
        }

        public int funObtenerProximoIdDetalleComision()
        {
            try
            {

                int iNuevoId = 1; // Comenzar con 1 como valor inicial
                string query = "SELECT MAX(Pk_id_detalle_comision) FROM Tbl_detalle_comisiones";

                using (OdbcConnection connection = cn.conectar())
                {
                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        var resultado = command.ExecuteScalar();
                        if (resultado != DBNull.Value)
                        {
                            iNuevoId = Convert.ToInt32(resultado) + 1; // Incrementar en 1 el valor máximo actual
                        }
                    }
                }
                return iNuevoId;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los proximo Id: " + ex.Message);
                return 0;
            }  
        }

        public DataTable funObtenerMarcas()
        {
            DataTable dtMarcas = new DataTable();
            string query = "SELECT Pk_id_Marca, nombre_Marca FROM Tbl_Marca WHERE estado = 1";

            try
            {
                using (OdbcConnection connection = cn.conectar())
                {
                    using (OdbcCommand cmd = new OdbcCommand(query, connection))
                    {
                        using (OdbcDataAdapter adapter = new OdbcDataAdapter(cmd))
                        {
                            adapter.Fill(dtMarcas);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener marcas: " + ex.Message);
            }

            return dtMarcas;
        }

        public DataTable funObtenerLineas()
        {
            DataTable dtLineas = new DataTable();
            string query = "SELECT Pk_id_linea, nombre_linea FROM Tbl_Linea WHERE estado = 1";

            try
            {
                using (OdbcConnection connection = cn.conectar())
                {
                    using (OdbcCommand cmd = new OdbcCommand(query, connection))
                    {
                        using (OdbcDataAdapter adapter = new OdbcDataAdapter(cmd))
                        {
                            adapter.Fill(dtLineas);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener líneas: " + ex.Message);
            }

            return dtLineas;
        }

    }
}