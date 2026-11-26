using System;

namespace ComestiblesDanLogistica
{
    public abstract class PedidoDistribucion
    {
        public string IdPedido { get; set; }
        public string Cliente { get; set; }
        public string Producto { get; set; }
        public int CantidadUnidades { get; set; }
        public double PesoTotalKg { get; set; }
        public string UbicacionCava { get; set; }
        public int DiasVidaUtilLote { get; set; }
        public bool Aprobado { get; set; }
        public string MotivoRechazo { get; set; }

        protected PedidoDistribucion(string idPedido, string cliente, string producto, int cantidadUnidades, double pesoTotalKg, string ubicacionCava, int diasVidaUtilLote)
        {
            IdPedido = idPedido;
            Cliente = cliente;
            Producto = producto;
            CantidadUnidades = cantidadUnidades;
            PesoTotalKg = pesoTotalKg;
            UbicacionCava = ubicacionCava;
            DiasVidaUtilLote = diasVidaUtilLote;
            Aprobado = false;
            MotivoRechazo = "";
        }

        public abstract int DiasMinimosVidaUtil();
        public abstract string TipoCanal();
        public abstract decimal CalcularCostoLogistico();

        public virtual bool ValidarVidaUtil(out string mensaje)
        {
            int minimo = DiasMinimosVidaUtil();
            if (DiasVidaUtilLote < minimo)
            {
                Aprobado = false;
                MotivoRechazo = $"Vida útil insuficiente ({DiasVidaUtilLote} días). Canal {TipoCanal()} exige mínimo {minimo} días.";
                mensaje = MotivoRechazo;
                return false;
            }
            Aprobado = true;
            MotivoRechazo = "Aprobado";
            mensaje = $"Pedido aprobado para {TipoCanal()} ({DiasVidaUtilLote} días de vida útil).";
            return true;
        }

        public virtual string ToLineaArchivo()
        {
            return $"{TipoCanal()}|{IdPedido}|{Cliente}|{Producto}|{CantidadUnidades}|{PesoTotalKg}|{UbicacionCava}|{DiasVidaUtilLote}|{Aprobado}|{MotivoRechazo}";
        }
    }
}