﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using IMPEMASA;

namespace IMPEMASA.Controllers
{
    [Authorize]
    public class VentasController : ApiController
    {
        private IMPEMASAEntities db = new IMPEMASAEntities();

        // GET: api/Ventas
        [HttpGet]
        public IEnumerable<object> Buscar()
        {
            return db.Ventas.ToList().Select(v => ConvertirVenta(v));
        }

        [HttpGet]
        public IEnumerable<object> VentasPendientes()
        {
            return db.View_VentasPendientes.OrderByDescending(v => v.NoFactura).ToList();
        }
        [HttpGet]
        public IEnumerable<object> BuscarPorCliente(int idCliente)
        {
            return db.View_VentasPendientes.Where(vp => vp.IdCliente.Equals(idCliente)).OrderByDescending(v => v.NoFactura).ToList();
        }
        [HttpGet]
        public bool ExisteFactura(int factura, int tipoVenta)
        {
            return db.Ventas.Where(v => v.NoFactura.Equals(factura) && v.IdVentaTipo.Equals(tipoVenta)).ToArray().Length > 0;
        }
        [HttpGet]
        public IEnumerable<object> VentaDepositos(int idVenta)
        {
            return db.Depositos.Where(d => d.IdVenta.Equals(idVenta)).OrderByDescending(v => v.Fecha).ToList().Select(d => DepositosController.Convertir(d));
        }
        // PUT: api/Ventas/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutVentas(Ventas ventas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(ventas).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VentasExists(ventas.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(db.View_VentasPendientes.First(v => v.Id.Equals(ventas.Id)));
        }

        // POST: api/Ventas
        [ResponseType(typeof(Ventas))]
        public IHttpActionResult PostVentas(Ventas ventas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Ventas.Add(ventas);
            db.SaveChanges();

            ventas.Clientes = db.Clientes.Find(ventas.IdCliente);
            ventas.VentaTipos = db.VentaTipos.Find(ventas.IdVentaTipo);

            return CreatedAtRoute("DefaultApi", new { id = ventas.Id }, db.View_VentasPendientes.First(v => v.Id.Equals(ventas.Id)));
        }

        // DELETE: api/Ventas/5
        [ResponseType(typeof(Ventas))]
        public IHttpActionResult DeleteVentas(int id)
        {
            Ventas ventas = db.Ventas.Find(id);
            if (ventas == null)
            {
                return NotFound();
            }
            IEnumerable<Depositos> depositos = db.Depositos.Where(dp => dp.IdVenta.Equals(id));
            db.Depositos.RemoveRange(depositos);
            db.Ventas.Remove(ventas);
            db.SaveChanges();

            return Ok(ventas);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VentasExists(int id)
        {
            return db.Ventas.Count(e => e.Id == id) > 0;
        }

        private object ConvertirVenta(Ventas v)
        {
            var venta = new Models.VentaModel()
            {
                Id = v.Id,
                IdVentaTipo = v.IdVentaTipo,
                IdCliente = v.IdCliente,
                Fecha = v.Fecha.ToString("MM/dd/yyyy"),
                FechaVencimiento = v.Fecha.AddMonths(1).AddDays(1).ToString("MM/dd/yyyy"),
                ITBIS = v.ITBIS,
                NoFactura = v.NoFactura,
                //PagoPendiente = v.PagoPendiente ?? true,
                RNC = v.RNC,
                SubTotal = v.SubTotal,
                Total = v.Total,
                Cliente = v.Clientes.Nombre,
                Tipo = v.VentaTipos.Nombre,
                Depositos = v.Depositos.Select(d => new Models.DepositoModel()
                {
                    Id = d.Id,
                    IdCuenta = d.IdCuenta,
                    IdDepositoTipo = d.IdDepositoTipo,
                    IdVenta = d.IdVenta,
                    Cuenta = d.Cuentas.Numero,
                    Banco = d.Cuentas.Bancos.Abreviatura,
                    Tipo = d.DepositoTipos.Nombre,
                    Monto = d.Monto,
                    Fecha = d.Fecha.ToString("MM/dd/yyyy")
                }).ToArray()
            };

            if (venta.PagoPendiente)
            {
                venta.DiasPendientes = (DateTime.Now - v.Fecha).Days;
            }

            return venta;
        }

        private object ConvertirVentaPut(Ventas v)
        {
            var venta = new Models.VentaModel()
            {
                Id = v.Id,
                IdVentaTipo = v.IdVentaTipo,
                IdCliente = v.IdCliente,
                Fecha = v.Fecha.ToString("MM/dd/yyyy"),
                FechaVencimiento = v.Fecha.AddMonths(1).AddDays(1).ToString("MM/dd/yyyy"),
                ITBIS = v.ITBIS,
                NoFactura = v.NoFactura,
                //PagoPendiente = v.PagoPendiente ?? true,
                RNC = v.RNC,
                SubTotal = v.SubTotal,
                Total = v.Total,
                Cliente = v.Clientes.Nombre,
                Tipo = v.VentaTipos.Nombre
            };

            if (venta.PagoPendiente)
            {
                venta.DiasPendientes = (DateTime.Now - v.Fecha).Days;
            }

            return venta;
        }

    }
}