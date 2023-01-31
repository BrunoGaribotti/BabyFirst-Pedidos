using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Negocio
{
    /// <summary>
    /// Objeto de Dirección de entrega - tabla DIRECCION_ENTREGA
    /// </summary>
    public class DIRECCION_ENTREGA
    {
        public string ID_DIRECCION_ENTREGA = "";
        public string COD_DIRECCION_ENTREGA = "";
        public string COD_CLIENTE = "";
        public string DIRECCION = "";
        public string COD_PROVINCIA = "";
        public string LOCALIDAD = "";
        public string HABITUAL = "";
        public string CODIGO_POSTAL = "";
        public string TELEFONO1 = "";
        public string TELEFONO2 = "";
        public string TOMA_IMPUESTO_HABITUAL = "";
        public string FILLER = "";
        public string OBSERVACIONES = "";
        public string AL_FIJ_IB3 = "";
        public string ALI_ADI_IB = "";
        public string ALI_FIJ_IB = "";
        public string IB_L = "";
        public string IB_L3 = "";
        public string II_IB3 = "";
        public string LIB = "";
        public string PORC_L = "";
        public string HABILITADO = "";
        public string HORARIO_ENTREGA = "";
        public string ENTREGA_LUNES = "";
        public string ENTREGA_MARTES = "";
        public string ENTREGA_MIERCOLES = "";
        public string ENTREGA_JUEVES = "";
        public string ENTREGA_VIERNES = "";
        public string ENTREGA_SABADO = "";
        public string ENTREGA_DOMINGO = "";
        public string CONSIDERA_IVA_BASE_CALCULO_IIBB = "";
        public string CONSIDERA_IVA_BASE_CALCULO_IIBB_ADIC = "";
        public string WEB_ADDRESS_ID = "";
        public string insert()
        {
            string sql = "";
            sql = @"
            INSERT INTO DIRECCION_ENTREGA 
            (
            ID_DIRECCION_ENTREGA
            ,COD_DIRECCION_ENTREGA
            ,COD_CLIENTE
            ,DIRECCION
            ,COD_PROVINCIA
            ,LOCALIDAD
            ,HABITUAL
            ,CODIGO_POSTAL
            ,TELEFONO1
            ,TELEFONO2
            ,TOMA_IMPUESTO_HABITUAL
            ,FILLER
            ,OBSERVACIONES
            ,AL_FIJ_IB3
            ,ALI_ADI_IB
            ,ALI_FIJ_IB
            ,IB_L
            ,IB_L3
            ,II_IB3
            ,LIB
            ,PORC_L
            ,HABILITADO
            ,HORARIO_ENTREGA
            ,ENTREGA_LUNES
            ,ENTREGA_MARTES
            ,ENTREGA_MIERCOLES
            ,ENTREGA_JUEVES
            ,ENTREGA_VIERNES
            ,ENTREGA_SABADO
            ,ENTREGA_DOMINGO
            ,CONSIDERA_IVA_BASE_CALCULO_IIBB
            ,CONSIDERA_IVA_BASE_CALCULO_IIBB_ADIC
            ,WEB_ADDRESS_ID
            ) VALUES ( 
            (SELECT MAX(ID_DIRECCION_ENTREGA) +1  FROM DIRECCION_ENTREGA)
           ,'" + COD_DIRECCION_ENTREGA + @"'
           ,'" + COD_CLIENTE + @"'
           ,'" + DIRECCION + @"'
           ,'" + COD_PROVINCIA + @"'
           ,'" + LOCALIDAD + @"'
           ,'" + HABITUAL + @"'
           ,'" + CODIGO_POSTAL + @"'
           ,'" + TELEFONO1 + @"'
           ,'" + TELEFONO2 + @"'
           ,'" + TOMA_IMPUESTO_HABITUAL + @"'
           ,'" + FILLER + @"'
           ,'" + OBSERVACIONES + @"'
            ," + AL_FIJ_IB3 + @" 
           ,'" + ALI_ADI_IB + @"'
           ,'" + ALI_FIJ_IB + @"'
            ," + IB_L + @" 
            ," + IB_L3 + @" 
            ," + II_IB3 + @" 
           ,'" + LIB + @"'
            ," + PORC_L + @" 
           ,'" + HABILITADO + @"'
           ,'" + HORARIO_ENTREGA + @"'
           ,'" + ENTREGA_LUNES + @"'
           ,'" + ENTREGA_MARTES + @"'
           ,'" + ENTREGA_MIERCOLES + @"'
           ,'" + ENTREGA_JUEVES + @"'
           ,'" + ENTREGA_VIERNES + @"'
           ,'" + ENTREGA_SABADO + @"'
           ,'" + ENTREGA_DOMINGO + @"'
           ,'" + CONSIDERA_IVA_BASE_CALCULO_IIBB + @"'
           ,'" + CONSIDERA_IVA_BASE_CALCULO_IIBB_ADIC + @"'
            ," + WEB_ADDRESS_ID + @"
            )
            ";
            return sql;
        }
        public DIRECCION_ENTREGA()
        {
            DataSet ds = new DataSet();
            ds.ReadXml(Procesos.Carpeta + "\\CONFIG\\DIRECCION_ENTREGA.xml");
            ID_DIRECCION_ENTREGA = ds.Tables[0].Rows[0]["ID_DIRECCION_ENTREGA"].ToString();
            COD_DIRECCION_ENTREGA = ds.Tables[0].Rows[0]["COD_DIRECCION_ENTREGA"].ToString();
            COD_CLIENTE = ds.Tables[0].Rows[0]["COD_CLIENTE"].ToString();
            DIRECCION = ds.Tables[0].Rows[0]["DIRECCION"].ToString();
            COD_PROVINCIA = ds.Tables[0].Rows[0]["COD_PROVINCIA"].ToString();
            LOCALIDAD = ds.Tables[0].Rows[0]["LOCALIDAD"].ToString();
            HABITUAL = ds.Tables[0].Rows[0]["HABITUAL"].ToString();
            CODIGO_POSTAL = ds.Tables[0].Rows[0]["CODIGO_POSTAL"].ToString();
            TELEFONO1 = ds.Tables[0].Rows[0]["TELEFONO1"].ToString();
            TELEFONO2 = ds.Tables[0].Rows[0]["TELEFONO2"].ToString();
            TOMA_IMPUESTO_HABITUAL = ds.Tables[0].Rows[0]["TOMA_IMPUESTO_HABITUAL"].ToString();
            FILLER = ds.Tables[0].Rows[0]["FILLER"].ToString();
            OBSERVACIONES = ds.Tables[0].Rows[0]["OBSERVACIONES"].ToString();
            AL_FIJ_IB3 = ds.Tables[0].Rows[0]["AL_FIJ_IB3"].ToString();
            ALI_ADI_IB = ds.Tables[0].Rows[0]["ALI_ADI_IB"].ToString();
            ALI_FIJ_IB = ds.Tables[0].Rows[0]["ALI_FIJ_IB"].ToString();
            IB_L = ds.Tables[0].Rows[0]["IB_L"].ToString();
            IB_L3 = ds.Tables[0].Rows[0]["IB_L3"].ToString();
            II_IB3 = ds.Tables[0].Rows[0]["II_IB3"].ToString();
            LIB = ds.Tables[0].Rows[0]["LIB"].ToString();
            PORC_L = ds.Tables[0].Rows[0]["PORC_L"].ToString();
            HABILITADO = ds.Tables[0].Rows[0]["HABILITADO"].ToString();
            HORARIO_ENTREGA = ds.Tables[0].Rows[0]["HORARIO_ENTREGA"].ToString();
            ENTREGA_LUNES = ds.Tables[0].Rows[0]["ENTREGA_LUNES"].ToString();
            ENTREGA_MARTES = ds.Tables[0].Rows[0]["ENTREGA_MARTES"].ToString();
            ENTREGA_MIERCOLES = ds.Tables[0].Rows[0]["ENTREGA_MIERCOLES"].ToString();
            ENTREGA_JUEVES = ds.Tables[0].Rows[0]["ENTREGA_JUEVES"].ToString();
            ENTREGA_VIERNES = ds.Tables[0].Rows[0]["ENTREGA_VIERNES"].ToString();
            ENTREGA_SABADO = ds.Tables[0].Rows[0]["ENTREGA_SABADO"].ToString();
            ENTREGA_DOMINGO = ds.Tables[0].Rows[0]["ENTREGA_DOMINGO"].ToString();
            CONSIDERA_IVA_BASE_CALCULO_IIBB = ds.Tables[0].Rows[0]["CONSIDERA_IVA_BASE_CALCULO_IIBB"].ToString();
            CONSIDERA_IVA_BASE_CALCULO_IIBB_ADIC = ds.Tables[0].Rows[0]["CONSIDERA_IVA_BASE_CALCULO_IIBB_ADIC"].ToString();
            WEB_ADDRESS_ID = ds.Tables[0].Rows[0]["WEB_ADDRESS_ID"].ToString();
            ds.Dispose();
        }
    }

}
