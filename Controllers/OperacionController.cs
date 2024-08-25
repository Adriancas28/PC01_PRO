using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PC01PRO.Controllers
{
    [Route("[controller]")]
    public class OperacionController : Controller
    {
       private readonly ILogger<OperacionController> _logger;

        public OperacionController(ILogger<OperacionController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Index(Operacion operacion)
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registrar(Operacion operacion)
        {
            if (operacion.Instrumentos == null || !operacion.Instrumentos.Any())
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un instrumento");
                return View("Index", operacion);
            }

            // Definir precios de los instrumentos
            var precios = new Dictionary<string, double>
            {
                { "S&P 500", 500 },
                { "Dow Jones", 300 },
                { "Bonos US", 120 }
            };

            // Calcular el monto total de los instrumentos seleccionados
            double montoTotIns = operacion.Instrumentos.Sum(instrumento => precios.ContainsKey(instrumento) ? precios[instrumento] : 0);

          

            // Calcular Comisión basada en el monto ingresado
            double Abono = operacion.Monto.HasValue ? operacion.Monto.Value : 0;
            double comision = Abono <= 300 ? 3 : 1;
            double igv = Abono * 0.18;
            // Calcular Total a Pagar
            double total = montoTotIns + (Abono + igv + comision)*operacion.Instrumentos.Count ;

            // Pasar los valores calculados a la vista
            ViewData["Instrumentos"] = string.Join(", ", operacion.Instrumentos);
            ViewData["FecOpe"] = operacion.Fecha.ToString("dd/MM/yyyy");
            ViewData["IGV"] = igv.ToString("F2");
            ViewData["Comision"] = comision.ToString("F2");
            ViewData["Total"] = total.ToString("F2");
            ViewData["Monto"] = Abono.ToString("F2");

            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }

    public class Operacion{
        public string? Nombre { get; set;}
        public string? Apellido { get;set;}
        public string? Correo { get; set;}
        public DateOnly Fecha{ get; set;}
        public List<string>? Instrumentos { get; set;}
        public double? Monto { get; set;}
    }
    
}