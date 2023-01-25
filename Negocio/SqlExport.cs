using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Negocio
{
    public class SqlExport
    {
        //public string empresa1 = "BF";
        public string empresa1 = "BF";
        public string empresa2 = "BF";

        public string Clientes()
        {
            string sql = @"
                            select 
                            cast(CASE WHEN FECHA_INHA > CONVERT(DATETIME,'01/01/1900',103) THEN 'N' ELSE 'S' END as varchar) as activo
                            ,'' AS condicion_iva_id
                            ,replace(cuit,'-','') as cuit
                            ,cast(PORC_DESC as decimal(17,2)) as descuento
                            ,E_MAIL as email
                            ,'' AS grupo_id
                            ,RAZON_SOCI AS nombre
                            ,'' AS orden
                            ,'' AS pagina_web
                            ,NRO_LISTA AS precio_lista_id
                            ,RAZON_SOCI AS razon_social
                            ,cast(COD_CLIENT as varchar) AS sku_id
                            ,TELEFONO_1 AS telefono
                            ,'' AS tipo_id
                            ,COD_ZONA AS vendedor_id
                            ,COD_VENDED AS zona_id
                            FROM " + empresa1 + @".DBO.GVA14
                          ";
            return sql;
        }

        internal string Clientes_Domicilios()
        {
            string sql = @"SELECT 
                            '' AS altura	
                            ,DOMICILIO AS calle	
                            ,'' AS cliente_id	
                            ,C_POSTAL AS codigo_postal	
                            ,LOCALIDAD AS localidad	
                            ,'' AS pais	
                            ,'' AS piso_depto
                            ,'' AS principal	
                            ,NOMBRE_PRO AS provincia	
                            ,COD_CLIENT AS sku_id
                            FROM " + empresa1 + @".DBO.GVA14
                            INNER JOIN " + empresa1 + @".DBO.GVA18 ON GVA18.COD_PROVIN = GVA14.COD_PROVIN 
                            ";

            return sql;
        }
        
        internal string Precios()
        {
            string sql = @"SELECT 
                        cast(PRECIO as decimal(17,2))  AS precio
                        ,'1' AS precio_lista_nro
                        ,GVA17.COD_ARTICU AS producto_id_erp
                        FROM " + empresa1 + @".DBO.GVA17
                        INNER JOIN " + empresa1 + @".DBO.STA11
                        ON STA11.COD_ARTICU = GVA17.COD_ARTICU
                        WHERE NRO_DE_LIS = 82
                        UNION 
                        SELECT 
                        cast(PRECIO as decimal(17,2))  AS precio
                        ,'1' AS precio_lista_nro
                        ,GVA17.COD_ARTICU AS producto_id_erp
                        FROM " + empresa2 + @".DBO.GVA17
                        INNER JOIN " + empresa2 + @".DBO.STA11
                        ON STA11.COD_ARTICU = GVA17.COD_ARTICU
                        WHERE NRO_DE_LIS = 82
                        ";//(B) comentarios sql
            return sql;
        }

        internal string Productos()
        {
        string sql = @"select 
                    'S' as activo,
                    --'' as alto,
                    --''  as ancho,
                    --'' as controla_stock,
                    --descripcio as descripcion,
                    --'' as destacado,
                    --cod_barra as ean13,
                    --'' AS familia_id_erp,
                    --'' AS imp_interno,
                    --'' AS iva,
                    'B' AS linea_id_erp,
                    --'' AS marca_id_erp,
                    --'' AS mercadolibre_activo,
                    --'' AS mercadolibre_id,
                    --COD_ARTICU AS nombre,
                    --'' AS orden,
                    --'' AS padre_id_erp,
                    --'' AS peso,
                    --'' AS profundidad,
                    --'' AS seo_descripcion,
                    --'' AS seo_keywords,
                    --'' AS seo_slug,
                    --'' AS seo_titulo,
                    COD_ARTICU AS id_erp
                    --'' AS stock,
                    --'' AS stock_media,
                    --'' AS stock_porcentaje,
                    --'' AS tipo,
                    --'' AS unidad_venta,
                    --'' AS unidades_vendidas,
                    --'' AS volumen
                    FROM " + empresa1 + @".DBO.STA11 WHERE PERFIL <> 'N'
                    --A PARTIR DE ACÁ DESCOMENTÉ TODO (B)
                    UNION
                    select 
                    'S' as activo,
                    '' as alto,
                    ''  as ancho,
                    '' as controla_stock,
                    descripcio as descripcion,
                    '' as destacado,
                    cod_barra as ean13,
                    '' AS familia_id_erp,
                    '' AS imp_interno,
                    '' AS iva,
                    'G' AS linea_id_erp,
                    '' AS marca_id_erp,
                    '' AS mercadolibre_activo,
                    '' AS mercadolibre_id,
                    COD_ARTICU AS nombre,
                    '' AS orden,
                    '' AS padre_id_erp,
                    '' AS peso,
                    '' AS profundidad,
                    '' AS seo_descripcion,
                    '' AS seo_keywords,
                    '' AS seo_slug,
                    '' AS seo_titulo,
                    COD_ARTICU AS id_erp
                    '' AS stock,
                    '' AS stock_media,
                    '' AS stock_porcentaje,
                    '' AS tipo,
                    '' AS unidad_venta,
                    '' AS unidades_vendidas,
                    '' AS volumen
                    FROM " + empresa2 + @".DBO.STA11 WHERE PERFIL <> 'N'
                    ";//(B) comentarios sql CUIDADO NO TODOS VER EN EL SELECT DE ARRIBA

            return sql;
        }
        
        internal string Vendedores()
        {
            string sql = @"SELECT 
                           NOMBRE_ZON AS nombre
                           ,COD_ZONA AS id_erp
                           FROM " + empresa1 + @".DBO.GVA05
                            ";

            return sql;
        }

        internal string Zonas()
        {
            string sql = @"SELECT 
                            NOMBRE_VEN AS nombre
                            ,''  AS orden
                            ,COD_CLIENT AS sku_id
                            FROM " + empresa1 + @".DBO.GVA14 
                            LEFT OUTER JOIN " + empresa1 + @".DBO.GVA23
                            ON GVA23.COD_VENDED = GVA14.COD_VENDED
                            UNION
                            SELECT 
                            NOMBRE_VEN AS nombre
                            ,''  AS orden
                            ,COD_CLIENT AS sku_id
                            FROM " + empresa2 + @".DBO.GVA14 
                            LEFT OUTER JOIN " + empresa2 + @".DBO.GVA23
                            ON GVA23.COD_VENDED = GVA14.COD_VENDED";
            return sql;//(B) comentarios sql
        }

        internal string Lineas()
        {
            string sql = @"
            SELECT 'Grupo' as nombre,'" + empresa1 + @"' as id_erp 
            union 
            SELECT 'Grupo','" + empresa2 + @"' 
            ";//(B) comentarios sql
            return sql;//(B) Baby as Grupo
        }
    }
}
