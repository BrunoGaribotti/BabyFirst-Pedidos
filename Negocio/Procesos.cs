using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BaseDatos;
using System.Data.Common;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Negocio
{
    public class Procesos
    {

        bool prueba = false;

        List<Empresa> Empresas = new List<Empresa>();

        bool pasar = false;
        bool paso = false;

        string Talonario_Pedidos = "";
        string Lista_web = "";

        public static string Carpeta = "";
        string mhost = "";
        string mfrom = "";
        string mdestino1 = "";
        string mdestino2 = "";
        string muser = "";
        string mpass = "";
        int mport = 21;
        bool mEnableSsl = false;

        string userFTP = "";
        string passFTP = "";

        string urlFtp = "";

        /// <summary>
        /// Carga la información de las empresas desde EMPRESAS.XML en la carpeta CONFIG.
        /// </summary>
        /// <returns></returns>
        public string CargarEmpresas()
        {
            string ret = "";
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(Procesos.Carpeta + "\\CONFIG\\EMPRESAS.XML");

                for(int i = 0;i < ds.Tables[0].Rows.Count;i++)
                {
                    Empresa Emp = new Empresa();
                    
                    Emp.tg = ds.Tables[0].Rows[i]["TG"].ToString();
                    Emp.web = ds.Tables[0].Rows[i]["WEB"].ToString();
                    Emp.inicial = ds.Tables[0].Rows[i]["INICIAL"].ToString();
                    Empresas.Add(Emp);
                }
            }
            catch (Exception ex)
            {
                ret = ex.Message.ToString();
            }
            return ret;
        }

        /// <summary>
        /// Carga los datos de configuración del email desde MAIL.XML
        /// </summary>
        public string CargarConfigMail()
        {
            string ret = "";
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(Procesos.Carpeta + "\\CONFIG\\MAIL.XML");

                mhost = ds.Tables[0].Rows[0]["HOST"].ToString();
                mfrom = ds.Tables[0].Rows[0]["MAIL"].ToString();
                muser = ds.Tables[0].Rows[0]["USER"].ToString();
                mpass = ds.Tables[0].Rows[0]["PASS"].ToString();
                mport = int.Parse(ds.Tables[0].Rows[0]["PORT"].ToString());
                mEnableSsl = bool.Parse(ds.Tables[0].Rows[0]["ENABLESSL"].ToString());
                mdestino1 = ds.Tables[0].Rows[0]["DESTINO"].ToString();
                mdestino2 = ds.Tables[0].Rows[0]["DESTINO2"].ToString();
            }
            catch (Exception ex)
            {
                ret = ex.Message.ToString();
            }
            return ret;
        }

        /// <summary>
        /// Carga la configuración de la conección al FTP desde FTP.XML
        /// </summary>
        /// <returns></returns>
        public string CargarConfigFTP()
        {
            string ret = "";
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(Procesos.Carpeta + "\\CONFIG\\FTP.XML");

                urlFtp = ds.Tables[0].Rows[0]["URLFTP"].ToString();
                userFTP = ds.Tables[0].Rows[0]["USERFTP"].ToString();
                passFTP = ds.Tables[0].Rows[0]["PASSFTP"].ToString();
            }
            catch (Exception ex)
            {
                ret = ex.Message.ToString();
            }
            return ret;
        }

        /// <summary>
        /// Obtiene la configurción desde el archivo CONFIG.XML
        /// </summary>
        /// <returns></returns>
        public string CargarConfig()
        {
            string ret = "";
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\CONFIG\\CONFIG.XML");

                db.cadenaConexion = ds.Tables[0].Rows[0]["SQLSTRING"].ToString();
                Procesos.Carpeta = ds.Tables[0].Rows[0]["CARPETA_PED"].ToString();
                Talonario_Pedidos = ds.Tables[0].Rows[0]["TALONARIO_PEDIDOS"].ToString();
                Lista_web = ds.Tables[0].Rows[0]["LISTA_WEB"].ToString();
                db.Configurar();
                db.Conectar();
                db.Desconectar();

            }
            catch (Exception ex)
            {
                ret = ex.Message.ToString();
            }
            return ret;
        }

        /// <summary>
        /// Comprueba si existe el pedido segúin el NRO_PEDIDO (GVA21)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool ExistePedido(string p)
        {
            bool existe = false;

            db.CrearComando("SELECT NRO_PEDIDO FROM GVA21 WHERE NRO_PEDIDO = '" + p + "'");
            DataTable dt = new DataTable();
            DbDataReader dr = db.EjecutarConsulta();
            dt.Load(dr);

            if (dt.Rows.Count == 1)
            {
                existe = true;
            }
            return existe;
        }

        /// <summary>
        /// Comprueba si existe la empresa según el IDEmpresa, desde la tabla EMPRESA.
        /// </summary>
        /// <param name="empresa"></param>
        /// <returns></returns>
        private bool ExisteEmpresa(string empresa)
        {
            bool existe = false;

            db.CrearComando(@"
                            SELECT IDEmpresa FROM EMPRESA
                            WHERE 1=1
                            --AND NombreEmpresa = '" + empresa + @"'
                            AND NombreBD = '" + empresa + @"'");

            DataTable dt = new DataTable();
            DbDataReader dr = db.EjecutarConsulta();
            dt.Load(dr);

            if (dt.Rows.Count == 1)
            {
                existe = true;
            }
            return existe;
        }

        /// <summary>
        /// Comprueba si existe el artículo según COD_ARTICU, en la tabla STA11.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="empresa"></param>
        /// <returns></returns>
        private bool ExisteArticulo(string p,string empresa)
        {
            bool existe = false;

            db.CrearComando("SELECT COD_ARTICU FROM " + empresa + @".DBO.STA11 WHERE COD_ARTICU = '" + p + "'");
            DataTable dt = new DataTable();
            DbDataReader dr = db.EjecutarConsulta();
            dt.Load(dr);

            if (dt.Rows.Count == 1)
            {
                existe = true;
            }
            return existe;
        }

        /// <summary>
        /// Comprueba si existe el artículo en la empresa G360.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="empresa"></param>
        /// <returns></returns>
        //(B)
        private bool ExisteArticuloInG(string p, string empresa)
        {
            bool existe = false;

            db.CrearComando("SELECT COD_ARTICU FROM G360.DBO.STA11 WHERE COD_ARTICU = '" + p + "'");
            DataTable dt = new DataTable();
            DbDataReader dr = db.EjecutarConsulta();
            dt.Load(dr);

            if (dt.Rows.Count == 1)
            {
                existe = true;
            }
            return existe;
        }

        public void DesonectarDb()
        {
            db.Desconectar();
        }

        public void ConectarDb()
        {
            db.Conectar();
        }

        /// <summary>
        /// Carga los pedidos en las BD.
        /// <para>Procesa los archivos de la carpeta IN y los correctamente exportados pasan a la carpeta OUT.</para>
        /// <para>Genera una copia de respaldo en las carpetas ZZ_IN_COPY y ZZ_OUT_COPY</para>
        /// </summary>
        /// <returns></returns>
        public string Procesar()
        {
            string log = "";
            DirectoryInfo di = new DirectoryInfo(Procesos.Carpeta + "\\IN\\");
            
            foreach (var fi in di.GetFiles())
            {
                bool transac = false;
                bool procesoLineas = false;

                PedidoBf oPedBf = new PedidoBf();

                string archivo = fi.Name;
                string ArchOrigen = Procesos.Carpeta + "\\IN\\" + fi.Name;
                string ArchDest = Procesos.Carpeta + "\\OUT\\" + fi.Name;
                string ArchOrigenCopy = Procesos.Carpeta + "\\ZZ_IN_COPY\\" + fi.Name;
                string ArchDestinoCopy = Procesos.Carpeta + "\\ZZ_OUT_COPY\\" + fi.Name;
                fi.CopyTo(ArchOrigenCopy, true);
                try
                {
                    using (var reader = new StreamReader(ArchOrigen))
                    {                       
                        List<string> lineas = new List<string>();

                        //string titulos = reader.ReadLine();

                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();

                            lineas.Add(line);
                        }

                        oPedBf = CargarPedidos(lineas);
                    }

                    procesoLineas = true;

                    if (oPedBf.error)
                    {
                        throw new Exception("Error!");
                    }
                    
                    //fi.Delete();
                    db.ComenzarTransaccion();
                    transac = true;
                    foreach (GVA21 ped in oPedBf.pedidos)
                    {

                        string sqlgva21 = ped.insert() + Environment.NewLine;
                        db.CrearComando(ped.insert());
                        db.EjecutarComando();

                        foreach (GVA03 item in ped.Renglones)
                        {
                            string sqlgva03 = item.insert(ped.Empresa) + Environment.NewLine;
                            db.CrearComando(item.insert(ped.Empresa));
                            db.EjecutarComando();
                        }

                        db.CrearComando(@"update " + ped.Empresa + @".DBO.GVA21 SET TOTAL_PEDI = (select 
                                            CAST(
                                            SUM(CANT_PEDID * PRECIO)- ((SUM(CANT_PEDID * PRECIO) * PORC_DESC) / 100)
                                            AS DECIMAL(17,2))  
                                            from " + ped.Empresa + @".DBO.gva21 
                                            LEFT OUTER JOIN " + ped.Empresa + @".DBO.GVA03
                                            ON GVA21.TALON_PED = GVA03.TALON_PED
                                            AND GVA21.NRO_PEDIDO = GVA03.NRO_PEDIDO
                                            where GVA21.TALON_PED = " + ped.TALON_PED + @"  
                                            AND GVA21.NRO_PEDIDO = '" + ped.NRO_PEDIDO + @"'
                                            GROUP BY PORC_DESC) 
                                            WHERE TALON_PED = " + ped.TALON_PED + @" 
                                            AND NRO_PEDIDO = '"  + ped.NRO_PEDIDO + @"'");
                        db.EjecutarComando();
                    
                    }
                    
                    GuardarArchivo(ArchDestinoCopy, oPedBf.lineas);
                    GuardarArchivo(ArchDest, oPedBf.lineas);

                    db.ConfirmarTransaccion();
                    
                    fi.Delete();
                    log += "Pedido " + oPedBf.pedidos[0].NRO_PEDIDO + " - Ok";
                }
                catch (Exception ex)
                {
                    if (transac)
                    {
                        db.CancelarTransaccion();
                    }
                    string error = " No se pudo procesar el archivo " + fi.Name + Environment.NewLine;
                    error += System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    error += ex.Message.ToString() + "<br>" + Environment.NewLine;
                    error += ex.StackTrace.ToString() + "<br>" + Environment.NewLine;
                    error += "*".PadLeft(100, '*') + "<br>" + Environment.NewLine;
                    error += "<br>" + Environment.NewLine;
                    foreach (string[] lin in oPedBf.lineas)
                    {
                        lin[14] = ex.Message.ToString().Replace("\"", "") + " - " + lin[14].ToString().Replace("\"", "");
                        string lineaOut = "@";
                        foreach(string x in lin)
                        {
                            lineaOut += "," + x;
                        }
                        error += lineaOut.Replace("@,", "") + "<br>" + Environment.NewLine;
                    }

                    grabarLog(error);
                    if(procesoLineas)
                    {
                        GuardarArchivo(ArchDest, oPedBf.lineas);
                    }
                    GuardarArchivo(ArchDestinoCopy, oPedBf.lineas);
                    fi.Delete();
                    try
                    {

                        MandarMail(error);// (B) Omitido por Google
                    }
                    catch (Exception e) { };
                    log += error.Replace("<br>", "");
                }
            }
            return log;
        }

        /// <summary>
        /// Guarda le archivo generado.
        /// </summary>
        /// <param name="ArchDest"></param>
        /// <param name="lineas"></param>
        public void GuardarArchivo(string ArchDest,List<String[]> lineas)
        {
            StreamWriter writer = new StreamWriter(ArchDest, false);

            foreach (string[] lin in lineas)
            {
                lin[14] = "\"" + lin[14].Replace("\"","") + "\"";
                string lineaOut = "@";
                foreach (string x in lin)
                {
                    lineaOut += "," + x;
                }
                writer.WriteLine(lineaOut.Replace("@,", ""));
            }
            writer.Close();
        }

        private void ValidarPedidos(PedidoBf oPedBf)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Carga toda la información de los pedidos de BabyFirst.
        /// </summary>
        /// <param name="lineas"></param>
        /// <returns>Devuelve un array de string con las lineas que van en el archivo.</returns>
        private PedidoBf CargarPedidos(List<string> lineas)
        {
            bool agregar = true;

            PedidoBf pedBf = new PedidoBf();
            //int i = 1;
            string Leyenda5 = "";

            foreach (string line in lineas)
            {
                string[] campos = (line + ",").Split(',');
                //campos[17] = ",";
                //campos[5] = campos[5].Trim('\"');
                agregar = true;


                GVA21 pedido = new GVA21();
                pedido.TALON_PED = Talonario_Pedidos;
                pedido.NRO_PEDIDO = " 00000" + campos[0].PadLeft(8,'0');
                pedido.COTIZ = "1";
                pedido.HORA_INGRESO = System.DateTime.Now.ToString("HHmmss");
                pedido.HORA = pedido.HORA_INGRESO;

                string inicial = TraerInicial(campos[11].Trim('\"') + campos[12].Trim('\"'));

                if (Leyenda5.IndexOf(inicial) < 0)
                {
                    Leyenda5 += inicial;
                }

                pedido.Empresa = TraerEmpresa(campos[11].Trim('\"') + campos[12].Trim('\"'));
                
                pedido.FECHA_PEDI = campos[1];
                pedido.COD_CLIENT = campos[2].Trim('\"');
                
                //(B)
                if(pedido.COD_CLIENT.ToString().StartsWith("G")) pedido.Empresa = "G360";

                pedido.COD_SUCURS = "01";
                //pedido.ID_DIRECCION_ENTREGA = TraerDireccionEntrega(pedido.COD_CLIENT,pedido.Empresa);

                pedido.ID_ASIENTO_MODELO_GV = TraerAsientoModelo(pedido.Empresa);

                pedido.LEYENDA_1 = TraerLeyenda(1, campos[13].Replace("\"",""));
                pedido.LEYENDA_2 = TraerLeyenda(2, campos[13].Replace("\"", ""));
                pedido.LEYENDA_3 = TraerLeyenda(3, campos[13].Replace("\"", ""));
                pedido.LEYENDA_4 = TraerLeyenda(4, campos[13].Replace("\"", ""));
                pedido.LEYENDA_5 = TraerLeyenda(5, campos[13].Replace("\"", ""));

                GVA03 item = new GVA03();
                
                item.CANT_PEDID = campos[7];                               
                item.CANT_A_DES = item.CANT_PEDID;
                item.CANT_A_FAC = item.CANT_PEDID;
                item.CANT_PEN_D = item.CANT_PEDID;
                item.CANT_PEN_F = item.CANT_PEDID;
                
                item.PRECIO = campos[10];
                item.NRO_PEDIDO = pedido.NRO_PEDIDO;
                item.TALON_PED = pedido.TALON_PED;
                
                if (!ExisteEmpresa(pedido.Empresa))
                {
                    campos[14] += " Empresa no existe ";
                    pedBf.lineas.Add(campos);
                    pedBf.error = true;
                    continue;
                }

                HClient hClient = ExisteCliente(pedido.COD_CLIENT, pedido.Empresa);

                if (hClient.cod_client != "")
                {
                    pedido.COD_TRANSP = hClient.cod_transp;
                    pedido.COD_VENDED = hClient.cod_vended;
                    pedido.COND_VTA = hClient.cond_vta;
                    pedido.N_LISTA = Lista_web;
                    pedido.PORC_DESC = hClient.porc_bonif.ToString("0.00").Replace(",", ".");
                    pedido.ID_DIRECCION_ENTREGA = hClient.id_de;
                    if (pedido.ID_DIRECCION_ENTREGA == "")
                    {
                        pedido.ID_DIRECCION_ENTREGA = "null";
                    }
                    pedido.TALONARIO = hClient.talonario;
                }
                else
                {
                    campos[14] += " Cliente no existe ";
                    pedBf.error = true;
                }

                if (ExisteArticulo(campos[5].Trim('\"'), pedido.Empresa))
                {
                    item.COD_ARTICU = campos[5].Trim('\"');
                    item.ID_MEDIDA_STOCK = TraerUnidadMedida("ID_MEDIDA_STOCK",item.COD_ARTICU, pedido.Empresa);
                    item.ID_MEDIDA_VENTAS = TraerUnidadMedida("ID_MEDIDA_VENTAS", item.COD_ARTICU, pedido.Empresa);
                    decimal precio = TraerPrecio(pedido.N_LISTA, item.COD_ARTICU,pedido.Empresa);

                    item.PRECIO = precio.ToString("0.00").Replace(",", ".");
                                                            
                    if (precio == 0 && !prueba)
                    {
                        campos[14] += " - Precio no esta en la lista de precios " + pedido.N_LISTA;
                        pedBf.error = true;
                    }

                }
                //(B)
                else if(ExisteArticuloInG(campos[5].Trim('\"'), pedido.Empresa))
                {
                    pedido.Empresa = "G360";
                    item.COD_ARTICU = campos[5].Trim('\"');
                    item.ID_MEDIDA_STOCK = TraerUnidadMedida("ID_MEDIDA_STOCK", item.COD_ARTICU, pedido.Empresa);
                    item.ID_MEDIDA_VENTAS = TraerUnidadMedida("ID_MEDIDA_VENTAS", item.COD_ARTICU, pedido.Empresa);
                    decimal precio = TraerPrecio(pedido.N_LISTA, item.COD_ARTICU, pedido.Empresa);

                    item.PRECIO = precio.ToString("0.00").Replace(",", ".");

                    if (precio == 0 && !prueba)
                    {
                        campos[14] += " - Precio no esta en la lista de precios " + pedido.N_LISTA;
                        pedBf.error = true;
                    }
                }
                else
                {
                    campos[14] += " Articulo no existe ";
                    pedBf.error = true;
                }


                foreach (GVA21 ped in pedBf.pedidos)
                {
                    if (ped.Empresa == pedido.Empresa)
                    {
                        item.N_RENGLON = (ped.Renglones.Count + 1).ToString();
                        ped.Renglones.Add(item);
                        agregar = false;
                    }
                }
                if (agregar)
                {
                    item.N_RENGLON = (pedido.Renglones.Count + 1).ToString();
                    pedido.Renglones.Add(item);
                    pedBf.pedidos.Add(pedido);
                }

                if (!pedBf.error)
                {
                    campos[14] += "Ok";
                }
                campos[14] = campos[14];
                
                //(B) los registros que sean de Grupo360 (campos[11]=="G") no serán contados a pedido de Nicolás Chame 03/06
                //if (campos[11] != "G")
                //{
                    pedBf.lineas.Add(campos);
                //}
            }

            foreach (GVA21 ped in pedBf.pedidos)
            {
                ped.LEYENDA_5 = "(" + Leyenda5 + ")";
            }
            return pedBf;
        }

        /// <summary>
        /// Trae la inicial de la empresa por cada empresa (A y B).
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string TraerInicial(string p)
        {
            string ret = "";
            foreach (Empresa emp in Empresas)
            {
                if (emp.web.Equals(p))
                {
                    ret = emp.inicial;
                }
            }
            return ret;
        }

        /// <summary>
        /// Devuelve la leyenda de la empresa.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        /// <returns></returns>
        private string TraerLeyenda(int p, string p_2)
        {
            string ret = "";
            try
            {
                if (p_2.Length > (60 * p))
                {
                    ret = p_2.Substring((p - 1) * 60, 60);
                }
                else
                {
                    ret =  p_2.Substring((p - 1) * 60, p_2.Length - ((p - 1) * 60));
                }
            }
            catch (Exception ex)
            {
                ret = "";
            }
            return ret;
        }
   
        /// <summary>
        /// Trae los datos del asiento modelo según la empresa - tabla ASIENTO_MODELO
        /// </summary>
        /// <param name="empresa"></param>
        /// <returns></returns>
        private string TraerAsientoModelo(string empresa)
        {
            string ret = "null";

            db.CrearComando(@"SELECT [ID_ASIENTO_MODELO]
                                  ,[EMPRESA]
                                  ,[ID_ASIENTO_MODELO_TG]
                              FROM [BA_INFO].[dbo].[ASIENTO_MODELO]
                            WHERE EMPRESA = '" + empresa + @"'");

            DataTable dt = new DataTable();
            DbDataReader dr = db.EjecutarConsulta();
            dt.Load(dr);

            if (dt.Rows.Count == 1)
            {
                ret = dt.Rows[0]["ID_ASIENTO_MODELO_TG"].ToString();
            }
            return ret;
        }

        /// <summary>
        /// Trae las listas de precios según la empresa, nro de lista y cod de artículo.
        /// </summary>
        /// <param name="lista"></param>
        /// <param name="articulo"></param>
        /// <param name="empresa"></param>
        /// <returns></returns>
        private decimal TraerPrecio(string lista, string articulo,string empresa)
        {
            decimal ret = 0;

            db.CrearComando(@"SELECT PRECIO FROM " + empresa + @".DBO.GVA17
                            WHERE NRO_DE_LIS = '" + lista + @"'
                            AND COD_ARTICU = '" + articulo + @"'");

            DataTable dt = new DataTable();
            DbDataReader dr = db.EjecutarConsulta();
            dt.Load(dr);

            if (dt.Rows.Count == 1)
            {
                ret = (decimal)dt.Rows[0]["PRECIO"];
            }
            return ret;
        }

        /// <summary>
        /// Trae los datos de la empresa.
        /// </summary>
        /// <param name="empresa"></param>
        /// <returns></returns>
        private string TraerEmpresa(string empresa)
        {
            string ret = "";
            foreach (Empresa emp in Empresas)
            {
                if (emp.web.Equals(empresa))
                {
                    ret = emp.tg;
                }
            }
            return ret;
        }

        /// <summary>
        /// Trae la dirección de entrega desde DIRECCION_ENTREGA
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="empresa"></param>
        /// <returns></returns>
        private string TraerDireccionEntrega(string cliente,string empresa)
        {
            string ret = "null";

            db.CrearComando(@"SELECT 
                            ID_DIRECCION_ENTREGA
                            FROM " + empresa + @".DBO.DIRECCION_ENTREGA
                            WHERE COD_CLIENTE = '" + cliente + @"'
                            AND HABILITADO = 'S' AND HABITUAL = 'S'
                            ");
            DataTable dt = new DataTable();
            DbDataReader dr = db.EjecutarConsulta();
            dt.Load(dr);

            if (dt.Rows.Count == 1)
            {
                ret = dt.Rows[0]["ID_DIRECCION_ENTREGA"].ToString();
            }
            return ret;
        }

        /// <summary>
        /// Trae la unidad de medida desde STA11.
        /// </summary>
        /// <param name="campo"></param>
        /// <param name="cod_articu"></param>
        /// <param name="empresa"></param>
        /// <returns></returns>
        private string TraerUnidadMedida(string campo,string cod_articu, string empresa)
        {
            string ret = "null";

            db.CrearComando(@"SELECT 
                            " + campo + @"
                            FROM " + empresa + @".DBO.STA11
                            WHERE COD_ARTICU = '" + cod_articu + "'");
            DataTable dt = new DataTable();
            DbDataReader dr = db.EjecutarConsulta();
            dt.Load(dr);

            if (dt.Rows.Count == 1)
            {
                ret = dt.Rows[0][campo].ToString();
            }
            return ret;
        }

        /// <summary>
        /// Chequea si existe el cliente del pedido.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="empresa"></param>
        /// <returns></returns>
        private HClient ExisteCliente(string p, string empresa)
        {
            HClient client = new HClient();

            //bool existe = false;

            string sqlstr = @"SELECT 
                            COD_CLIENT
                            ,COD_VENDED
                            ,COND_VTA
                            ,COD_TRANSP
                            ,NRO_LISTA
                            ,IVA_L
                            ,IVA_D
                            ,PORC_DESC
                            ,ID_DIRECCION_ENTREGA
                            ,IVA_L
                            ,CASE WHEN IVA_D = 'S' THEN TAL_A 
                                ELSE TAL_B  
                                END AS TALONARIO
                            FROM " + empresa + @".DBO.GVA14
                            LEFT OUTER JOIN " + empresa + @".DBO.DIRECCION_ENTREGA
                            ON DIRECCION_ENTREGA.COD_CLIENTE = GVA14.COD_CLIENT COLLATE Latin1_General_BIN
                            AND DIRECCION_ENTREGA.HABITUAL = 'S'
                            AND DIRECCION_ENTREGA.HABILITADO = 'S'
							LEFT OUTER JOIN BA_INFO.DBO.TALONARIOS
							ON  TALONARIOS.EMPRESA = '" + empresa + @"'
                            WHERE COD_CLIENT = '" + p + "'";
            db.CrearComando(sqlstr);
            DataTable dt = new DataTable();
            DbDataReader dr = db.EjecutarConsulta();
            dt.Load(dr);

            if (dt.Rows.Count == 1)
            {
                client.cod_client = dt.Rows[0]["COD_CLIENT"].ToString();
                client.cod_vended = dt.Rows[0]["COD_VENDED"].ToString();
                client.cod_transp = dt.Rows[0]["COD_TRANSP"].ToString();
                client.cond_vta = dt.Rows[0]["COND_VTA"].ToString();
                client.lista = dt.Rows[0]["NRO_LISTA"].ToString();
                client.porc_bonif = (decimal)dt.Rows[0]["PORC_DESC"];
                client.id_de = dt.Rows[0]["ID_DIRECCION_ENTREGA"].ToString();
                client.talonario = dt.Rows[0]["TALONARIO"].ToString();
            }
            return client;
        }

        /// <summary>
        /// Elimina pedidos con errores que no fueron procesados.
        /// </summary>
        public void EliminarPedidoAveriado()
        {
            Ftp ftp = new Ftp(urlFtp + "pendientes", userFTP, passFTP);
            ftp.delete("pedido-tango-20200608-122219-3189.csv");
        }

        public void DescargarArchivos()
        {
            try
            {
                Ftp ftp = new Ftp(urlFtp + "pendientes", userFTP , passFTP);

                string[] archivos = ftp.directoryListSimple("");
                int cant = 0;
                for (int i = 0; i < archivos.Length; i++)
                {
                    if (!archivos[i].Equals(""))
                    {
                        if(archivos[i].StartsWith("pendientes/")) archivos[i] = archivos[i].Substring(11,archivos[i].Length-11);
                        ftp.download(archivos[i], Procesos.Carpeta + "\\IN\\" + archivos[i]);
                        //ftp.download(archivos[i], Procesos.Carpeta + "\\IN\\TESTO\\" + archivos[i]);
                        ftp.delete(archivos[i]);

                        if (archivos[i].StartsWith("pedido"))
                        {
                            grabarLog("Archivo " + cant + ", " + archivos[i]);
                            cant++;
                        }
                    }
                }
                grabarLog("La cantidad de archivos descargados son:" + cant + "User: " + userFTP + " Pass: "+ passFTP + " URL: "+ urlFtp);

                //(B)
                 // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://explorerfan.com.ar/");
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("explorerfan_tango", "XVaQie012G");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            grabarLog("READER:" + reader.ReadToEnd() + " RESPONSE: "+ response.StatusDescription);


            reader.Close();
            response.Close();


            }
            catch (Exception ex)
            {
                string a = ex.Message.ToString();
                grabarLog(a);
            }
        }

        /// <summary>
        /// Sube los archivos procesados al FTP. En el log anota los errores.
        /// </summary>
        /// <param name="destino"></param>
        /// <param name="local"></param>
        private void SubirArchivos(string destino,string local)
        {
            try
            {
                Ftp ftp = new Ftp(urlFtp + "procesados", userFTP, passFTP);

                ftp.upload(destino, local);

            }
            catch (Exception ex)
            {
                grabarLog("Error " + local + "  - " + ex.Message.ToString());
            }
        }

        /// <summary>
        /// Sube al FTP los archivos Maestros. En el log anota los errores.
        /// </summary>
        /// <param name="destino"></param>
        /// <param name="local"></param>
        private void SubirArchivosMaestros(string destino, string local)
        {
            try
            {
                Ftp ftp = new Ftp(urlFtp + "importar", userFTP, passFTP);

                ftp.upload(destino, local);

            }
            catch (Exception ex)
            {
                grabarLog("Error subir maestros  " + local + " - " + ex.Message.ToString());
                
            }
        }

        /// <summary>
        /// Lista el directorio del FTP.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        static List<string> listarFTP(string dir, string user, string pass)
        {
            FtpWebRequest dirFtp = ((FtpWebRequest)FtpWebRequest.Create(dir));

            // Los datos del usuario (credenciales)
            NetworkCredential cr = new NetworkCredential(user, pass);
            dirFtp.Credentials = cr;

            // El comando a ejecutar
            dirFtp.Method = "LIST";

            // También usando la enumeración de WebRequestMethods.Ftp
            dirFtp.Method = WebRequestMethods.Ftp.ListDirectory;

            // Obtener el resultado del comando
            StreamReader reader =
                new StreamReader(dirFtp.GetResponse().GetResponseStream());

            // Leer el stream
            //string res = reader.ReadToEnd();


            List<string> archivos = new List<string>();

            var line = reader.ReadLine();

            while (!string.IsNullOrEmpty(line))
            {
                archivos.Add(line);
                line = reader.ReadLine();
            }

            // Mostrarlo.
            //Console.WriteLine(res);

            // Cerrar el stream abierto.
            reader.Close();
            
            return archivos;
        }

        /// <summary>
        /// Envía el email según los datos de configuración. 
        /// </summary>
        /// <param name="mensaje"></param>
        public void MandarMail(string mensaje)
        {
            try
            {

                if (mhost == "")
                {
                    return;
                }

                string asunto = "Error - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                SmtpClient smtp = new SmtpClient();
                smtp.Host = mhost;
                smtp.Port = mport;
                smtp.EnableSsl = mEnableSsl;
                smtp.Credentials = new NetworkCredential(muser, mpass);
                if (mensaje == null) mensaje += " ERROR 133";
                grabarLog(mensaje + " " + asunto + " " + smtp.Host + " " + smtp.Port);

                smtp.Timeout = 4000;//(B)20000;

                string output = null;

                MailMessage email = new MailMessage();
                if (mdestino1 == "")
                {
                    return;
                }
                email.To.Add(new MailAddress(mdestino1));

                if (mdestino2 != "")
                {
                    email.To.Add(new MailAddress(mdestino2));
                }
                //email.CC.Add(new MailAddress("internaleshop@widex.com.ar"));
                //email.CC.Add(new MailAddress("sa@widex.com.ar"));
                email.From = new MailAddress(mfrom);
                email.Subject = asunto;
                email.Body = mensaje;
                email.IsBodyHtml = true;
                email.Priority = MailPriority.Normal;

                try
                {
                    smtp.Send(email);
                    email.Dispose();
                    //output = "Corre electrónico fue enviado satisfactoriamente.";
                }
                catch (Exception ex)
                {
                    output = "Error enviando correo electrónico: " + ex.Message.ToString() + Environment.NewLine;
                    output += ex.InnerException.ToString() + Environment.NewLine;
                    grabarLog(output);
                }

            }catch (Exception ex) { }

        }

        /// <summary>
        /// Guarda datos en el archivo log.txt
        /// </summary>
        /// <param name="mensaje"></param>
        public void grabarLog(string mensaje)
        {
            string line = DateTime.Now.ToString("dd - MMM - yyy hh:mm:ss");

            line += " -   " + mensaje;

            StreamWriter sr = new StreamWriter(Procesos.Carpeta + "\\log.txt", true);

            sr.WriteLine(line);

            sr.Close();
        }

        /// <summary>
        /// Sube al FTP los archivos que quedaron pendientes.
        /// </summary>
        public void SubirPendientes()
        {
            DirectoryInfo di = new DirectoryInfo(Procesos.Carpeta + "\\OUT\\");

            foreach (var fi in di.GetFiles())
            {
                grabarLog("Subidos pendientes: Destino: " + fi.Name + " Local: " + fi.FullName);
                SubirArchivos(fi.Name, fi.FullName);
            }

            DirectoryInfo diM = new DirectoryInfo(Procesos.Carpeta + "\\MAESTROS\\");

            foreach (var fiM in diM.GetFiles())
            {
                SubirArchivosMaestros(fiM.Name, fiM.FullName);
            }
        }

        public void SalirProceso(string p)
        {
//            db.CrearComando(@"UPDATE BA_INFO.DBO.PROCESO SET 
//                             EN_PROCESO = 0, PROGRAMA = ''
//                                WHERE PROGRAMA = '" + p + @"' ");
//            db.EjecutarComando();
        }

        public bool EnProceso(string p)
        {
//            bool ret = false;

//            db.CrearComando(@"UPDATE BA_INFO.DBO.PROCESO SET 
//                             EN_PROCESO = 1, PROGRAMA = '" + p + @"'
//                                WHERE EN_PROCESO = 0 AND PROGRAMA = '' ");
//            int i = db.EjecutarComando();

//            if (i == 1)
//            {
//                ret = true;
//            }

//            return ret;
            return true;
        }

        /// <summary>
        /// Crea el archivo según el formato pactado con el cliente.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string CrearArchivo(DataTable dt)
        {
            string ret = "";
            if (dt.Rows.Count < 1)
            {
                return ret;
            }

            ret += "@";
            for(int i = 0;i < dt.Columns.Count;i++)
            {
                ret += ",\"" + dt.Columns[i].ColumnName + "\"";
            }
            ret += Environment.NewLine;

            for(int f = 0;f< dt.Rows.Count;f++)
            {
                string fila = "@";
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    if (dt.Columns[c].DataType == typeof(System.String))
                    {
                        if (dt.Rows[f][c].ToString().Equals(""))
                        {
                            fila += "," + dt.Rows[f][c].ToString() + "";
                        }
                        else
                        {
                            fila += ",\"" + dt.Rows[f][c].ToString() + "\"";
                        }
                    }
                    else
                    {
                        if (dt.Columns[c].DataType == typeof(System.Decimal))
                        {
                            fila += "," + dt.Rows[f][c].ToString().Replace(",", ".");
                        }
                        else
                        {
                            fila += "," + dt.Rows[f][c].ToString();
                        }
                    }
                }
                ret += fila.Replace("@,", "") + Environment.NewLine;
            }
            ret = ret.Replace("@,", "");

            return ret;
        }

        /// <summary>
        /// Crea los archivos Maestros.
        /// </summary>
        /// <returns></returns>
        public string CrearMaestros()
        {
            string asd = "";
            SqlExport oSql = new SqlExport();
            CrearArchivo("clientes.csv",oSql.Clientes());
            asd += "Archivo clientes creado +";
            CrearArchivo("clientes_domicilios.csv", oSql.Clientes_Domicilios());
            asd += "Archivo clientes_domicilios creado +";
            CrearArchivo("productos.csv", oSql.Productos());
            asd += "Archivo productos creado +";
            CrearArchivo("productos_precios.csv", oSql.Precios());
            asd += "Archivo productos_precios creado +";
            CrearArchivo("vendedores.csv", oSql.Vendedores());
            asd += "Archivo vendedores creado";
            return asd;
        }

        private void CrearArchivo(string xArchivo, string consulta)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlExport oSQL = new SqlExport();
                db.CrearComando(consulta);
                DbDataReader dr = db.EjecutarConsulta();
                dt.Load(dr);

                string archivo = CrearArchivo(dt);
                if (archivo == "")
                {
                    return;
                }

                //if (xArchivo == "clientes_domicilios.csv")
                //{
                //    archivo = Separar(archivo);
                //}

                string ArchDest = Procesos.Carpeta + "\\MAESTROS\\" + xArchivo;
                StreamWriter sw = new StreamWriter(ArchDest, false);
                sw.Write(archivo);
                sw.Close();
            }
            catch (Exception ex)
            {
                
            }
        }

        //private string Separar(string archivo)
        //{
        //    string nuevo = "";
        //    string[] stringSeparators = new string[] { "\r\n" };
        //    string[] Lineas = archivo.Split(stringSeparators, StringSplitOptions.None);

        //    for (int i = 0; i < Lineas.Length; i++)
        //    {
        //        string[] temp = Lineas[i].Split(',');

        //        string[] direccion = temp[1].Trim('\"').Split(' ');

        //        for (j = 0; j < direccion.Length; j++)
        //        {

        //        }

        //        nuevo += Lineas[i] + Environment.NewLine;
        //    }
        //    return nuevo;

        //}


        public string Correr(string p, bool conMaestros,bool manual)
        {
            string errorlog = "";

            bool conectado = false;


            p += System.DateTime.Now.ToString("yyyyMMddHHmmss");

            try
            {
                conectado = false;
                string ret = "";

                ret += CargarConfig();
                ret += CargarConfigMail();
                ret += CargarEmpresas();
                ret += CargarConfigFTP();

                if (ret != "")
                {
//                  errorlog += ret;
                    StreamWriter sw = new StreamWriter(Procesos.Carpeta + "\\error.log", true);
                    sw.WriteLine(ret);
                    sw.Close();
                    throw new Exception(ret);
                }

                ConectarDb();
                conectado = true;

                if (EnProceso(p))
                {
                    errorlog += "Ejecutando Proceso Manual!" + Environment.NewLine;
                    StreamWriter sw = new StreamWriter(Procesos.Carpeta + "\\error.log", true);
                    sw.Close();
                    try
                    {
                        if (!prueba)
                        {
                            //EliminarPedidoAveriado(); //(B) Para pruebas
                            DescargarArchivos();
                        }
                        sw = new StreamWriter(Procesos.Carpeta + "\\error.log", true);
                        sw.WriteLine("Descarga de archivos completada");
                        sw.Close();
                    }
                    catch (Exception x)
                    {
                        string error = "Error al Descargar Archivos desde la Web!";
                        error += System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                        error += x.Message.ToString();
                        error += x.StackTrace.ToString();
                        error += "*".PadLeft(100, '*');
                        
                        errorlog += error + Environment.NewLine;

                        sw = new StreamWriter(Procesos.Carpeta + "\\error.log", true);
                        sw.WriteLine(error);
                        sw.Close();
                        try
                        {
                            MandarMail(error); //(B) Omitido porque Google lo bloquea
                        }
                        catch(Exception e){};
                    }
                    sw = new StreamWriter(Procesos.Carpeta + "\\error.log", true);
                    sw.WriteLine("conMaestros:"+conMaestros.ToString());
                    sw.Close();
                    //conMaestros = false;//(B) conMaestros = false para que no sincronice la base G360 con BF
                    if (conMaestros)
                    {
                        if (System.DateTime.Now.Hour == 20 && paso == false)
                        {
                            pasar = true;
                        }
                        if (System.DateTime.Now.Hour == 21)
                        {
                            paso = false;
                        }
                        if (pasar == true)
                        {
                            paso = true;
                            pasar = false;
                            string aux = CrearMaestros();
                        }
                    }
                    sw = new StreamWriter(Procesos.Carpeta + "\\error.log", true);
                    sw.WriteLine("Manual:"+manual.ToString());
                    sw.Close();
                    //manual = false;//(B) manual = false para que no sincronice la base G360 con BF
                    if (manual)
                    {
                        string aux2 = CrearMaestros();
                    }
                    errorlog += Procesar() + Environment.NewLine;
                    sw = new StreamWriter(Procesos.Carpeta + "\\error.log", true);
                    sw.WriteLine("Procesados");
                    sw.Close();
                    try
                    {
                        if (!prueba)
                        {
                            errorlog += "Procesando Pendientes!" + Environment.NewLine;
                            
                            SubirPendientes();
                        }
                    }
                    catch (Exception y)
                    {
                        string error = "Error al subir Archivos a la Web! - ";
                        error += System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - ";
                        error += y.Message.ToString();
                        error += y.StackTrace.ToString();
                        error += "*".PadLeft(100, '*');

                        errorlog += error + Environment.NewLine;

                        sw = new StreamWriter(Procesos.Carpeta + "\\error.log", true);
                        sw.WriteLine(error);
                        sw.Close();
                        try
                        {
                            MandarMail(error); //(B) Omitido por Google bloqueo
                        }
                        catch (Exception e) { };
                    }
                }
                else
                {
                    //textBox1.Text += "Actualente esta corriendo el proceso Automtico vuelva a intentarlo en unos instantes!!!" + Environment.NewLine;
                    errorlog += "Ejecutando Proceso Automatico!" + Environment.NewLine;
                }

                if (conectado)
                {
                    SalirProceso(p);

                    DesonectarDb();
                    errorlog += "Desconectado!" + Environment.NewLine;
                }
                errorlog += "Fin!" + Environment.NewLine;
            }
            catch (Exception ex)
            {
                string error = "";
                error += System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                error += ex.Message.ToString();
                error += ex.StackTrace.ToString();
                error += "*".PadLeft(100, '*');
                StreamWriter sw = new StreamWriter(Procesos.Carpeta + "\\error.log", true);
                sw.WriteLine(error);
                sw.Close();
                errorlog += error + Environment.NewLine;
                if (conectado)
                {
                    try
                    {
                        MandarMail(error); //(B) Omitido porque Google bloquea el acceso al mail.
                    }catch(Exception e){};
                    SalirProceso(p);

                    DesonectarDb();

                    errorlog += "Desconectado!" + Environment.NewLine;
                }
            }
            return errorlog;
        }
    }
}
