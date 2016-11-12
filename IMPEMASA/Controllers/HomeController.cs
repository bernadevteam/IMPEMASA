using OfficeOpenXml;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace IMPEMASA.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public FileResult DescargarReporte(char reporte, int? mes, int? anio)
        {
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var fileStream = new MemoryStream();
            ObtenerReporte(reporte, mes ?? DateTime.Now.Month, anio ?? DateTime.Now.Year).SaveAs(fileStream);
            fileStream.Position = 0;

            var fsr = new FileStreamResult(fileStream, contentType);
            string reporteNam = string.Empty;
            switch (reporte) {
                case 'D':
                    reporteNam = "Depositos";
                    break;
                case 'V':
                    reporteNam = "Ventas";
                    break;
                case 'A':
                    reporteNam = "Antiguedad";
                    break;
            }
            fsr.FileDownloadName = string.Format("{0}_{1}.xlsx", reporteNam,DateTime.Now.ToString("MM_dd_yyyy"));

            return fsr;
        }

        private ExcelPackage ObtenerReporte(char reporte, int mes, int anio)
        {
            ExcelPackage pkg = null;
            switch (reporte)
            {
                case 'V':
                    pkg = LlenarVentas(mes, anio);
                    break;
                case 'D':
                    pkg = LlenarDepositos(mes, anio);
                    break;
                case 'A':
                    pkg = LlenarAntiguedad();
                    break;
            }


            return pkg;
        }

        private ExcelPackage LlenarVentas(int mes, int anio)
        {
            var package = new ExcelPackage(new FileInfo(Server.MapPath(@"~/Archivos/VENTAS.xlsx")));
            var wkbk = package.Workbook;
            IMPEMASAEntities db = new IMPEMASAEntities();
            int initFila = 8;

            int filaNum = initFila;
            int filaTipo1 = initFila;
            int filaTipo2 = initFila;
            int filaTipo3 = initFila;
            int filaTipo4 = initFila;

            var libTotal = wkbk.Worksheets[1];
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("es-ES");
            var ventas = db.Ventas.Include(cl => cl.Clientes)
                .Include(vt => vt.VentaTipos)
                .OrderByDescending(f => f.Fecha).Where(v => v.Fecha.Month.Equals(mes) && v.Fecha.Year.Equals(anio)).ToArray();

            libTotal.Cells[1, 1].Value = string.Format("Ventas del mes de {0}", ventas.FirstOrDefault().Fecha.ToString("MMMM", ci));

            foreach (var venta in ventas)
            {

                int complentaria = 0;
                switch (venta.IdVentaTipo)
                {
                    case 1:
                        complentaria = filaTipo1;
                        filaTipo1++;
                        break;
                    case 2:
                        complentaria = filaTipo2;
                        filaTipo2++;
                        break;
                    case 3:
                        complentaria = filaTipo3;
                        filaTipo3++;
                        break;
                    case 4:
                        complentaria = filaTipo4;
                        filaTipo4++;
                        break;
                }
                AsignarVentaCelda(wkbk.Worksheets[venta.IdVentaTipo + 1], complentaria, venta, false);
                AsignarVentaCelda(libTotal, filaNum, venta);
                filaNum++;


            }

            return package;
        }

        private ExcelPackage LlenarDepositos(int mes, int anio)
        {
            var package = new ExcelPackage(new FileInfo(Server.MapPath(@"~/Archivos/DEPOSITOS.xlsx")));
            var wkbk = package.Workbook;
            IMPEMASAEntities db = new IMPEMASAEntities();
            int initFila = 8;

            int filaNum = initFila;
            int filaTipo1 = initFila;
            int filaTipo2 = initFila;
            int filaTipo3 = initFila;
            int filaTipo4 = initFila;

            var libTotal = wkbk.Worksheets[1];
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("es-ES");
            var depositos = db.Depositos
                .Include(d => d.Cuentas)
                .Include(d => d.Ventas)
                .OrderByDescending(f => f.Fecha).Where(v => v.Fecha.Month.Equals(mes) && v.Fecha.Year.Equals(anio)).ToArray();

            libTotal.Cells[1, 1].Value = string.Format("Depositos del mes de {0}", depositos.FirstOrDefault().Fecha.ToString("MMMM", ci));

            foreach (var deposito in depositos)
            {
               
                int complentaria = 0;
                int hoja = 0;
                switch (deposito.Cuentas.IdBanco)
                {
                    case 1:
                        complentaria = filaTipo1;
                        filaTipo1++;
                        hoja = 1;
                        break;
                    case 2:
                        complentaria = filaTipo2;
                        filaTipo2++;
                        hoja = 2;
                        break;
                    case 3:
                        complentaria = filaTipo3;
                        filaTipo3++;
                        hoja = 3;
                        break;
                    case 4:
                        complentaria = filaTipo2;
                        filaTipo2++;
                        hoja = 2;
                        break;
                    case 5:
                        complentaria = filaTipo3;
                        filaTipo3++;
                        hoja = 3;
                        break;
                }
                AsignarDepositoCelda(wkbk.Worksheets[hoja+ 1], complentaria, deposito);
                AsignarDepositoCelda(libTotal, filaNum, deposito);
                filaNum++;


            }

            return package;
        }

        private ExcelPackage LlenarAntiguedad()
        {
            var package = new ExcelPackage(new FileInfo(Server.MapPath(@"~/Archivos/ANTIGUEDAD.xlsx")));
            var wkbk = package.Workbook;
            IMPEMASAEntities db = new IMPEMASAEntities();
            int initFila = 8;

            var libTotal = wkbk.Worksheets[1];
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("es-ES");

            libTotal.Cells[1, 1].Value = string.Format("SALDO DE ANTIGUEDAD POR CLIENTES AL {0}", DateTime.Today.ToShortDateString());

            foreach (var antiguedad in db.ReporteBalanceAntiguedad())
            {
                AsignarSaldoPendienteCelda(libTotal, initFila, antiguedad);
                initFila++;
            }

            return package;
        }

        private void AsignarVentaCelda(ExcelWorksheet ws, int filaNum, Ventas venta, bool mostrarTipo = true)
        {

            ws.Cells[filaNum, 1].Value = venta.NoFactura;
            ws.Cells[filaNum, 2].Value = venta.Fecha.ToShortDateString();
            ws.Cells[filaNum, 3].Value = venta.Clientes.Nombre;
            ws.Cells[filaNum, 4].Value = venta.SubTotal;
            ws.Cells[filaNum, 5].Value = venta.ITBIS;
            ws.Cells[filaNum, 6].Value = venta.Total;
            ws.Cells[filaNum, 7].Value = venta.RNC;
            if (mostrarTipo)
            {
                ws.Cells[filaNum, 8].Value = venta.VentaTipos.Nombre;
            }
        }

        private void AsignarDepositoCelda(ExcelWorksheet ws, int filaNum, Depositos deposito)
        {
            Ventas venta = deposito.Ventas;
            ws.Cells[filaNum, 1].Value = deposito.Fecha.ToShortDateString();
            ws.Cells[filaNum, 2].Value = venta.Clientes.Nombre;
            ws.Cells[filaNum, 3].Value = venta.NoFactura;
            ws.Cells[filaNum, 4].Value = deposito.DepositoTipos.Nombre;
            ws.Cells[filaNum, 5].Value = deposito.Cuentas.Numero;
            ws.Cells[filaNum, 6].Value = deposito.Monto;
        }

        private void AsignarSaldoPendienteCelda(ExcelWorksheet ws, int filaNum, ReporteBalanceAntiguedad_Result antiguedad)
        {
            ws.Cells[filaNum, 1].Value = antiguedad.Cliente;
            ws.Cells[filaNum, 2].Value = antiguedad.Telefono;
            ws.Cells[filaNum, 3].Value = string.Format("RD{0:c}", antiguedad.Balance);
            ws.Cells[filaNum, 4].Value = string.Format("RD{0:c}", antiguedad.Treinta);
            ws.Cells[filaNum, 5].Value = string.Format("RD{0:c}", antiguedad.Sesenta);
            ws.Cells[filaNum, 6].Value = string.Format("RD{0:c}", antiguedad.Noventa);
            ws.Cells[filaNum, 7].Value = string.Format("RD{0:c}", antiguedad.CientoVeinte);
        }


    }
}
