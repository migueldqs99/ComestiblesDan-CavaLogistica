# Sistema de Control de Cava y Distribución Refrigerada — Comestibles Dan S.A.

Aplicación de escritorio desarrollada en **C# con Windows Forms (construida 100% por código)** bajo el paradigma de Programación Orientada a Objetos (POO), diseñada para optimizar la recepción, almacenamiento y despacho refrigerado de carnes frías en **Comestibles Dan**.

---

## 1. Problema Real que Resuelve

En la operación logística de productos cárnicos (salchichas, mortadelas, jamones, chorizos, salchichones y tocinetas), los lotes terminados ingresan desde planta a la **Cava de Alistamiento** antes de ser despachados hacia dos canales comerciales con tiempos de tránsito y rotación muy distintos:
* **Tiendas Urbanas (Área Metropolitana):** Puntos de venta con alta rotación y entregas de corta distancia.
* **Poblaciones (Rutas Intermunicipales):** Municipios fuera del área metropolitana con trayectos largos y menor velocidad de rotación.

### Fallas operativas solucionadas:
1. **Violación de rotación y vencimientos:** Despachar lotes con pocos días de vida útil hacia municipios lejanos generaba devoluciones y pérdidas. El sistema aplica **validaciones polimórficas** exigiendo mínimo **12 días** de vida útil para tiendas urbanas y **28 días** para poblaciones.
2. **Descontrol de ubicación física en cava:** Digitaliza la cava mediante una **matriz física de 4×5** (4 estibas × 5 bahías de carga), identificando en tiempo real cada posición ocupada y su estado de frescura por colores.
3. **Sobrecupo vehicular y costos ocultos:** Calcula el flete refrigerado según el canal y consolida los kilogramos despachados frente a la capacidad máxima de un camión refrigerado (**3.500 kg**), emitiendo alertas tempranas en el cierre de turno.

---

## 2. Diagrama UML de Clases

```mermaid
classDiagram
    class LoteInventario {
        +string CodigoLote
        +string Producto
        +int Unidades
        +double PesoUnitarioKg
        +DateTime FechaVencimiento
        +int Estiba
        +int Bahia
        +LoteInventario(...)
        +DiasVidaUtil() int
        +PesoTotalKg() double
        +EstadoFrescura() string
        +ToLineaArchivo() string
        +DesdeLineaArchivo(string) LoteInventario
    }

    class PedidoDistribucion {
        <<abstract>>
        +string IdPedido
        +string Cliente
        +string Producto
        +int CantidadUnidades
        +double PesoTotalKg
        +string UbicacionCava
        +int DiasVidaUtilLote
        +bool Aprobado
        +string MotivoRechazo
        +DiasMinimosVidaUtil()* int
        +TipoCanal()* string
        +CalcularCostoLogistico()* decimal
        +ValidarVidaUtil(out string) bool
        +ToLineaArchivo() string
    }

    class PedidoTiendaUrbana {
        +string ZonaMetropolitana
        +PedidoTiendaUrbana(...)
        +DiasMinimosVidaUtil() int
        +TipoCanal() string
        +CalcularCostoLogistico() decimal
    }

    class PedidoPoblacion {
        +string Municipio
        +double HorasViaje
        +PedidoPoblacion(...)
        +DiasMinimosVidaUtil() int
        +TipoCanal() string
        +CalcularCostoLogistico() decimal
    }

    class GestorCavaDan {
        -LoteInventario[,] matrizCava
        -List~PedidoDistribucion~ historialPedidos
        -string rutaArchivo
        +double CapacidadCamionKg
        +RegistrarLote(LoteInventario, out string) bool
        +ObtenerLotesActivos() List~LoteInventario~
        +ConsultarUnidadesPorProducto(string) int
        +ProcesarPedido(PedidoDistribucion, int, int, out string) bool
        +GenerarInformeCierreTurno() string
        +GuardarDatosTxt() void
        +CargarDatosTxt() void
    }

    PedidoDistribucion <|-- PedidoTiendaUrbana : Hereda
    PedidoDistribucion <|-- PedidoPoblacion : Hereda
    GestorCavaDan o-- LoteInventario : Gestiona matriz 4x5
    GestorCavaDan o-- PedidoDistribucion : Procesa historial
    ```

---

## 3. Requisitos Técnicos Implementados

* **Interfaz 100% por código (`Form1.cs`):** Construida sin diseñador gráfico (`TabControl` con 4 pestañas, `TableLayoutPanel` dinámico 4×5, `DataGridView`, `ErrorProvider` y `MessageBox`).
* **Separación de capas:** Ninguna clase del dominio (`LoteInventario`, `PedidoDistribucion`, `PedidoTiendaUrbana`, `PedidoPoblacion`, `GestorCavaDan`) referencia `System.Windows.Forms`.
* **Estructuras de datos:** Matriz bidimensional `LoteInventario[4, 5]` para el mapa físico de la cava y `List<PedidoDistribucion>` para el historial de órdenes.
* **Informe con contadores, acumuladores y banderas:** Cierre de turno automatizado que calcula inventario total, peso consolidado, fletes acumulados y activa banderas de alerta temprana por vencimiento (`<12 días`) o sobrecupo vehicular (`>3.500 kg`).
* **Persistencia en texto plano:** Lectura y escritura automática en `datos_cava_comestibles_dan.txt` tras cada operación.

---

## 4. Cómo Ejecutar el Proyecto

### Requisitos previos
* [.NET SDK 8.0 o superior](https://dotnet.microsoft.com/download) (Windows).
* Visual Studio 2022 o Visual Studio Code.

### Ejecución desde terminal
1. Clonar el repositorio:
   ```bash
   git clone https://github.com/migueldqs99/ComestiblesDan-CavaLogistica.git
   ```
2. Entrar a la carpeta del proyecto:
   ```bash
   cd ComestiblesDan-CavaLogistica/ComestiblesDanLogistica
   ```
3. Ejecutar la aplicación:
   ```bash
   dotnet run
   ```

---

## 5. Autor
* **Desarrollador:** Miguel Dario Quiroz