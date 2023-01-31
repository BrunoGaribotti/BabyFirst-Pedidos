using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Negocio
{
    /// <summary>
    /// Objeto que contiene los datos del pedido de BabyFirst
    /// </summary>
    public class PedidoBf
    {
        public bool error = false;

        public List<string[]> lineas = new List<string[]>();

        public List<GVA21> pedidos = new List<GVA21>();    
    }
}
