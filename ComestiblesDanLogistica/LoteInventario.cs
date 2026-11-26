using System;

namespace ComestiblesDanLogistica
{
    public class LoteInventario
    {
        public string CodigoLote { get; set; }
        public string Producto { get; set; } // Salchichas, Mortadelas, Jamones, Chorizos, Salchichones, Tocinetas
        public int Unidades { get; set; }
        public double PesoUnitarioKg { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int Estiba { get; set; } // Fila (0 a 3)
        public int Bahia { get; set; }  // Columna (0 a 4)

        public LoteInventario(string codigoLote, string producto, int unidades, double pesoUnitarioKg, DateTime fechaVencimiento, int estiba, int bahia)
        {
            CodigoLote = codigoLote;
            Producto = producto;
            Unidades = unidades;
            PesoUnitarioKg = pesoUnitarioKg;
            FechaVencimiento = fechaVencimiento.Date;
            Estiba = estiba;
            Bahia = bahia;
        }

        public int DiasVidaUtil()
        {
            return (FechaVencimiento - DateTime.Today).Days;
        }

        public double PesoTotalKg()
        {
            return Unidades * PesoUnitarioKg;
        }

        public string EstadoFrescura()
        {
            int dias = DiasVidaUtil();
            if (dias < 0) return "VENCIDO";
            if (dias < 12) return "RIESGO ALTO (<12d)";
            if (dias < 28) return "SOLO URBANO (12-27d)";
            return "ÓPTIMO TODO CANAL (>=28d)";
        }

        public string ToLineaArchivo()
        {
            return $"{CodigoLote}|{Producto}|{Unidades}|{PesoUnitarioKg}|{FechaVencimiento:yyyy-MM-dd}|{Estiba}|{Bahia}";
        }

        public static LoteInventario DesdeLineaArchivo(string linea)
        {
            string[] p = linea.Split('|');
            return new LoteInventario(
                p[0],
                p[1],
                int.Parse(p[2]),
                double.Parse(p[3]),
                DateTime.Parse(p[4]),
                int.Parse(p[5]),
                int.Parse(p[6])
            );
        }
    }
}