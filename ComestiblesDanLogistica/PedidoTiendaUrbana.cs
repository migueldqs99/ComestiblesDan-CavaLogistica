namespace ComestiblesDanLogistica
{
    public class PedidoTiendaUrbana : PedidoDistribucion
    {
        public string ZonaMetropolitana { get; set; }

        public PedidoTiendaUrbana(string idPedido, string cliente, string producto, int cantidadUnidades, double pesoTotalKg, string ubicacionCava, int diasVidaUtilLote, string zonaMetropolitana)
            : base(idPedido, cliente, producto, cantidadUnidades, pesoTotalKg, ubicacionCava, diasVidaUtilLote)
        {
            ZonaMetropolitana = zonaMetropolitana;
        }

        public override int DiasMinimosVidaUtil() => 12; // Regla operativa Tiendas Urbanas (12 días)
        public override string TipoCanal() => "Tienda Urbana";

        public override decimal CalcularCostoLogistico()
        {
            // Tarifa base urbana $18,000 COP + $250 COP por kg refrigerado
            return 18000m + ((decimal)PesoTotalKg * 250m);
        }

        public override string ToLineaArchivo()
        {
            return base.ToLineaArchivo() + $"|{ZonaMetropolitana}";
        }
    }

    public class PedidoPoblacion : PedidoDistribucion
    {
        public string Municipio { get; set; }
        public double HorasViaje { get; set; }

        public PedidoPoblacion(string idPedido, string cliente, string producto, int cantidadUnidades, double pesoTotalKg, string ubicacionCava, int diasVidaUtilLote, string municipio, double horasViaje)
            : base(idPedido, cliente, producto, cantidadUnidades, pesoTotalKg, ubicacionCava, diasVidaUtilLote)
        {
            Municipio = municipio;
            HorasViaje = horasViaje;
        }

        public override int DiasMinimosVidaUtil() => 28; // Regla operativa Poblaciones (28 días)
        public override string TipoCanal() => "Población";

        public override decimal CalcularCostoLogistico()
        {
            // Tarifa base intermunicipal $45,000 COP + $420/kg + $15,000 por hora de refrigeración en ruta
            return 45000m + ((decimal)PesoTotalKg * 420m) + ((decimal)HorasViaje * 15000m);
        }

        public override string ToLineaArchivo()
        {
            return base.ToLineaArchivo() + $"|{Municipio}|{HorasViaje}";
        }
    }
}