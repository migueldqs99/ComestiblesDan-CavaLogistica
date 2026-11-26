using System;
using System.Collections.Generic;
using System.IO;

namespace ComestiblesDanLogistica
{
    public class GestorCavaDan
    {
        // Matriz física de la Cava de Alistamiento: 4 estibas x 5 bahías de carga
        private LoteInventario[,] matrizCava = new LoteInventario[4, 5];
        private List<PedidoDistribucion> historialPedidos = new List<PedidoDistribucion>();
        private readonly string rutaArchivo = "datos_cava_comestibles_dan.txt";
        public const double CapacidadCamionKg = 3500.0; // Capacidad máxima camión refrigerado

        public LoteInventario[,] ObtenerMatrizCava() => matrizCava;
        public List<PedidoDistribucion> ObtenerPedidos() => historialPedidos;

        // Funcionalidad 1: Ingresar productos al inventario en matriz 4x5
        public bool RegistrarLote(LoteInventario nuevoLote, out string mensaje)
        {
            if (nuevoLote.Estiba < 0 || nuevoLote.Estiba >= 4 || nuevoLote.Bahia < 0 || nuevoLote.Bahia >= 5)
            {
                mensaje = "Coordenada de cava inválida. Debe estar entre Estiba 1-4 y Bahía 1-5.";
                return false;
            }
            if (matrizCava[nuevoLote.Estiba, nuevoLote.Bahia] != null && matrizCava[nuevoLote.Estiba, nuevoLote.Bahia].Unidades > 0)
            {
                mensaje = $"La Estiba {nuevoLote.Estiba + 1}, Bahía {nuevoLote.Bahia + 1} ya está ocupada por el lote {matrizCava[nuevoLote.Estiba, nuevoLote.Bahia].CodigoLote}.";
                return false;
            }

            matrizCava[nuevoLote.Estiba, nuevoLote.Bahia] = nuevoLote;
            GuardarDatosTxt();
            mensaje = $"Lote {nuevoLote.CodigoLote} ({nuevoLote.Producto}) almacenado en Estiba {nuevoLote.Estiba + 1}, Bahía {nuevoLote.Bahia + 1}.";
            return true;
        }

        // Funcionalidades 2 y 3: Consultar inventario activo y vida útil
        public List<LoteInventario> ObtenerLotesActivos()
        {
            List<LoteInventario> lista = new List<LoteInventario>();
            for (int f = 0; f < 4; f++)
            {
                for (int c = 0; c < 5; c++)
                {
                    if (matrizCava[f, c] != null && matrizCava[f, c].Unidades > 0)
                    {
                        lista.Add(matrizCava[f, c]);
                    }
                }
            }
            return lista;
        }

        public int ConsultarUnidadesPorProducto(string producto)
        {
            int total = 0;
            for (int f = 0; f < 4; f++)
            {
                for (int c = 0; c < 5; c++)
                {
                    if (matrizCava[f, c] != null && matrizCava[f, c].Producto.Equals(producto, StringComparison.OrdinalIgnoreCase))
                    {
                        total += matrizCava[f, c].Unidades;
                    }
                }
            }
            return total;
        }

        // Funcionalidades 4 y 5: Procesar pedidos con polimorfismo y descuento en cava
        public bool ProcesarPedido(PedidoDistribucion pedido, int estiba, int bahia, out string mensaje)
        {
            LoteInventario lote = matrizCava[estiba, bahia];
            if (lote == null || lote.Unidades <= 0)
            {
                mensaje = "La celda seleccionada en la cava está vacía.";
                return false;
            }
            if (!lote.Producto.Equals(pedido.Producto, StringComparison.OrdinalIgnoreCase))
            {
                mensaje = $"El lote seleccionado contiene {lote.Producto}, no {pedido.Producto}.";
                return false;
            }
            if (lote.Unidades < pedido.CantidadUnidades)
            {
                mensaje = $"Stock insuficiente en esa ubicación. Disponibles: {lote.Unidades} unidades.";
                return false;
            }

            pedido.DiasVidaUtilLote = lote.DiasVidaUtil();
            pedido.PesoTotalKg = pedido.CantidadUnidades * lote.PesoUnitarioKg;
            pedido.UbicacionCava = $"Estiba {estiba + 1} - Bahía {bahia + 1} ({lote.CodigoLote})";

            // Validación polimórfica según canal (12 días Urbano / 28 días Población)
            bool esValido = pedido.ValidarVidaUtil(out mensaje);
            historialPedidos.Add(pedido);

            if (esValido)
            {
                lote.Unidades -= pedido.CantidadUnidades;
                if (lote.Unidades == 0)
                {
                    matrizCava[estiba, bahia] = null; // Libera espacio físico en cava
                }
            }

            GuardarDatosTxt();
            return esValido;
        }

        // Requisito 6: Informe de cierre de turno con ciclos, contadores, acumuladores y banderas
        public string GenerarInformeCierreTurno()
        {
            int totalLotesActivos = 0;
            int lotesEnRiesgo = 0;
            int unidadesTotalesCava = 0;
            double pesoTotalCavaKg = 0;
            bool alertaVencimientoCritico = false; // Bandera 1

            for (int f = 0; f < 4; f++)
            {
                for (int c = 0; c < 5; c++)
                {
                    LoteInventario lote = matrizCava[f, c];
                    if (lote != null && lote.Unidades > 0)
                    {
                        totalLotesActivos++;
                        unidadesTotalesCava += lote.Unidades;
                        pesoTotalCavaKg += lote.PesoTotalKg();

                        if (lote.DiasVidaUtil() < 12)
                        {
                            lotesEnRiesgo++;
                            alertaVencimientoCritico = true;
                        }
                    }
                }
            }

            int pedidosAprobados = 0;
            int pedidosRechazados = 0;
            double pesoDespachadoKg = 0;
            decimal fleteTotal = 0;
            bool alertaSobrecupoCamion = false; // Bandera 2

            foreach (PedidoDistribucion ped in historialPedidos)
            {
                if (ped.Aprobado)
                {
                    pedidosAprobados++;
                    pesoDespachadoKg += ped.PesoTotalKg;
                    fleteTotal += ped.CalcularCostoLogistico();
                }
                else
                {
                    pedidosRechazados++;
                }
            }

            if (pesoDespachadoKg > CapacidadCamionKg)
            {
                alertaSobrecupoCamion = true;
            }

            return "=========================================================\r\n" +
                   "   INFORME DE CIERRE DE TURNO - COMESTIBLES DAN S.A.\r\n" +
                   "=========================================================\r\n\r\n" +
                   "1. ESTADO FÍSICO DE CAVA DE ALISTAMIENTO (MATRIZ 4x5):\r\n" +
                   $"   - Posiciones ocupadas: {totalLotesActivos} de 20 bahías\r\n" +
                   $"   - Unidades disponibles en cava: {unidadesTotalesCava:N0} unds\r\n" +
                   $"   - Peso inventario almacenado: {pesoTotalCavaKg:N2} kg\r\n" +
                   $"   - Lotes con alerta temprana (<12 días): {lotesEnRiesgo}\r\n\r\n" +
                   "2. CONSOLIDACIÓN DE DISTRIBUCIÓN Y RUTAS:\r\n" +
                   $"   - Pedidos APROBADOS para despacho: {pedidosAprobados}\r\n" +
                   $"   - Pedidos RECHAZADOS por rotación: {pedidosRechazados}\r\n" +
                   $"   - Carga total consolidada: {pesoDespachadoKg:N2} kg / {CapacidadCamionKg:N2} kg\r\n" +
                   $"   - Costo logístico refrigerado total: ${fleteTotal:N0} COP\r\n\r\n" +
                   "3. ALERTAS OPERATIVAS DEL SISTEMA (BANDERAS):\r\n" +
                   $"   - [ALERTA ROTACIÓN]: {(alertaVencimientoCritico ? "SÍ - Existen lotes con menos de 12 días de vida útil. Priorizar evacuación." : "NO - Todos los lotes tienen vida útil saludable.")}\r\n" +
                   $"   - [ALERTA SOBRECUPO]: {(alertaSobrecupoCamion ? "SÍ - La carga excede la capacidad de 1 camión refrigerado (3.500 kg). Asignar segundo vehículo." : "NO - Carga dentro de la capacidad vehicular.")}\r\n";
        }

        // Requisito 7: Persistencia en archivo de texto plano
        public void GuardarDatosTxt()
        {
            using (StreamWriter sw = new StreamWriter(rutaArchivo, false))
            {
                for (int f = 0; f < 4; f++)
                {
                    for (int c = 0; c < 5; c++)
                    {
                        if (matrizCava[f, c] != null && matrizCava[f, c].Unidades > 0)
                        {
                            sw.WriteLine("LOTE|" + matrizCava[f, c].ToLineaArchivo());
                        }
                    }
                }

                foreach (PedidoDistribucion p in historialPedidos)
                {
                    sw.WriteLine("PEDIDO|" + p.ToLineaArchivo());
                }
            }
        }

        public void CargarDatosTxt()
        {
            if (!File.Exists(rutaArchivo)) return;

            matrizCava = new LoteInventario[4, 5];
            historialPedidos.Clear();

            string[] lineas = File.ReadAllLines(rutaArchivo);
            foreach (string linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;

                if (linea.StartsWith("LOTE|"))
                {
                    LoteInventario lote = LoteInventario.DesdeLineaArchivo(linea.Substring(5));
                    matrizCava[lote.Estiba, lote.Bahia] = lote;
                }
                else if (linea.StartsWith("PEDIDO|"))
                {
                    string[] p = linea.Substring(7).Split('|');
                    string canal = p[0];
                    PedidoDistribucion ped = null;

                    if (canal == "Tienda Urbana")
                    {
                        ped = new PedidoTiendaUrbana(p[1], p[2], p[3], int.Parse(p[4]), double.Parse(p[5]), p[6], int.Parse(p[7]), p[10]);
                    }
                    else if (canal == "Población")
                    {
                        ped = new PedidoPoblacion(p[1], p[2], p[3], int.Parse(p[4]), double.Parse(p[5]), p[6], int.Parse(p[7]), p[10], double.Parse(p[11]));
                    }

                    if (ped != null)
                    {
                        ped.Aprobado = bool.Parse(p[8]);
                        ped.MotivoRechazo = p[9];
                        historialPedidos.Add(ped);
                    }
                }
            }
        }
    }
}