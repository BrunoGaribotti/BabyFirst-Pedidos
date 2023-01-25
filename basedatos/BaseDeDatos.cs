using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Configuration;


namespace BaseDatos
{
    public class db
    {
        //public static string BA_EMPRESA = "BA_BOMBEROS.dbo.";
        private static DbConnection conexion = null;
        public static DbCommand comando = null;
        private static DbTransaction transaccion = null;
        public static string cadenaConexion;

        private static DbProviderFactory factory = null;

        /// <summary>
        /// Crea una instancia del acceso a la base de datos.
        /// </summary>
        public db()
        {
            Configurar();
        }

        /// <summary>
        /// Configura el acceso a la base de datos para su utilización.
        /// </summary>
        /// <exception cref="BaseDatosException">Si existe un error al cargar la configuración.</exception>
        public  static void Configurar()
        {
            try
            {
                string proveedor = "System.Data.SqlClient";
                
                //cadenaConexion = "data source = " + ConfigurationManager.AppSettings.Get("sqlserver").ToString() + "; initial catalog = " + ConfigurationManager.AppSettings.Get("Empresa").ToString()
                //    + "; user id = " + ConfigurationManager.AppSettings.Get("user").ToString() + "; password = " + ConfigurationManager.AppSettings.Get("pass").ToString()+";";
                //cadenaConexion = "";                
                //cadenaConexion = "data source = cfvb.no-ip.org,1433; initial catalog =CONSEJO_EJERCICIO_2008 ; user id =Axoft; password = Axoft;";
                
                db.factory = DbProviderFactories.GetFactory(proveedor);
                //Conectar();
            }
            catch (ConfigurationException ex)
            {
                throw new BaseDatosException("Error al cargar la configuración del acceso a datos.", ex);
            }
        }

        /// <summary>
        /// Permite desconectarse de la base de datos.
        /// </summary>
        public static void Desconectar()
        {
            if (conexion.State.Equals(ConnectionState.Open))
            {
                conexion.Close();
            }
        }

        /// <summary>
        /// Se concecta con la base de datos.
        /// </summary>
        /// <exception cref="BaseDatosException">Si existe un error al conectarse.</exception>
        public static void Conectar()
        {
            if (conexion != null && !conexion.State.Equals(ConnectionState.Closed))
            {
                throw new BaseDatosException("La conexión ya se encuentra abierta.");
            }
            
            try
            {
                if (conexion == null)
                {
                    conexion = factory.CreateConnection();
                    conexion.ConnectionString = cadenaConexion;
                }
                conexion.Open();
            }
            catch (DataException ex)
            {
                throw new BaseDatosException("Error al conectarse a la base de datos.", ex);
            }
        }

        /// <summary>
        /// Crea un comando en base a una sentencia SQL.
        /// Ejemplo:
        /// <code>SELECT * FROM Tabla WHERE campo1=@campo1, campo2=@campo2</code>
        /// Guarda el comando para el seteo de parámetros y la posterior ejecución.
        /// </summary>
        /// <param name="sentenciaSQL">La sentencia SQL con el formato: SENTENCIA [param = @param,]</param>
        public static void CrearComando(string sentenciaSQL)
        {
           comando = factory.CreateCommand();
           comando.Connection =conexion;
           comando.CommandType = CommandType.Text;
           comando.CommandText = sentenciaSQL;
            if (transaccion != null)
            {
               comando.Transaction =transaccion;
            }
        }

        /// <summary>
        /// Setea un parámetro como nulo del comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro cuyo valor será nulo.</param>
        public void AsignarParametroNulo(string nombre)
        {
            AsignarParametro(nombre, "", "NULL");
        }

        /// <summary>
        /// Asigna un parámetro de tipo cadena al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void AsignarParametroCadena(string nombre, string valor)
        {
            AsignarParametro(nombre, "'", valor);
        }

        /// <summary>
        /// Asigna un parámetro de tipo entero al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void AsignarParametroEntero(string nombre, int valor)
        {
            AsignarParametro(nombre, "", valor.ToString());
        }

        /// <summary>
        /// Asigna un parámetro al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="separador">El separador que será agregado al valor del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        private void AsignarParametro(string nombre, string separador, string valor)
        {
            int indice =comando.CommandText.IndexOf(nombre);
            string prefijo =comando.CommandText.Substring(0, indice);
            string sufijo =comando.CommandText.Substring(indice + nombre.Length);
           comando.CommandText = prefijo + separador + valor + separador + sufijo;
        }

        /// <summary>
        /// Asigna un parámetro de tipo fecha al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void AsignarParametroFecha(string nombre, DateTime valor)
        {
            AsignarParametro(nombre, "'", valor.ToString());
        }

        /// <summary>
        /// Ejecuta el comando creado y retorna el resultado de la consulta.
        /// </summary>
        /// <returns>El resultado de la consulta.</returns>
        /// <exception cref="BaseDatosException">Si ocurre un error al ejecutar el comando.</exception>
        public static DbDataReader EjecutarConsulta()
        {
            return comando.ExecuteReader();
        }

        /// <summary>
        /// Ejecuta el comando creado y retorna un escalar.
        /// </summary>
        /// <returns>El escalar que es el resultado del comando.</returns>
        /// <exception cref="BaseDatosException">Si ocurre un error al ejecutar el comando.</exception>
        public int EjecutarEscalar()
        {
            int escalar = 0;
            try
            {
                escalar = int.Parse(comando.ExecuteScalar().ToString());
            }
            catch (InvalidCastException ex)
            {
                throw new BaseDatosException("Error al ejecutar un escalar.", ex);
            }
            return escalar;
        }

        /// <summary>
        /// Ejecuta el comando creado.
        /// </summary>
        public static int EjecutarComando()
        {
           return comando.ExecuteNonQuery();
        }

        /// <summary>
        /// Comienza una transacción en base a la conexion abierta.
        /// Todo lo que se ejecute luego de esta ionvocación estará 
        /// dentro de una tranasacción.
        /// </summary>
        public static void ComenzarTransaccion()
        {
            if (transaccion == null)
            {
                transaccion = conexion.BeginTransaction();
            }
            else
            {
                transaccion = null;
                transaccion = conexion.BeginTransaction();
            }
        }

        /// <summary>
        /// Cancela la ejecución de una transacción.
        /// Todo lo ejecutado entre ésta invocación y su 
        /// correspondiente <c>ComenzarTransaccion</c> será perdido.
        /// </summary>
        public static void CancelarTransaccion()
        {
            if (transaccion != null)
            {
               transaccion.Rollback();
            }
        }

        /// <summary>
        /// Confirma todo los comandos ejecutados entre el <c>ComanzarTransaccion</c>
        /// y ésta invocación.
        /// </summary>
        public static void ConfirmarTransaccion()
        {
            if (transaccion != null)
            {
               transaccion.Commit();
            }
        }

    }
}
