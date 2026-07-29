using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BILLETERACRIPTOMONEDASBACKEND.Data;
using BILLETERACRIPTOMONEDASBACKEND.Models;
using BILLETERACRIPTOMONEDASBACKEND.DTOS;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace BILLETERACRIPTOMONEDASBACKEND.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransaccionesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        public TransaccionesController(
            AppDbContext context,
            HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] TransaccionDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.Action.ToLower() != "purchase" && request.Action.ToLower() != "sale")
            {
                return BadRequest(new { error = "La acción debe ser 'purchase' o 'sale'" });
            }


            if (!System.DateTime.TryParseExact(request.DateTime, "yyyy-MM-dd HH:mm",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime transactionDateTime))
            {
                return BadRequest(new { error = "El formato de fecha debe ser 'yyyy-MM-dd HH:mm'" });
            }

            try
            {
                string claveMonedaCryptoYa = "";
                string claveMoneda = request.CryptoCode;
                decimal cantidadCryptoComprada = request.CryptoAmount;
                string nombreDelCliente = request.NombreCliente;

                decimal bitcoin = 0;
                decimal ethereum = 0;
                decimal usdt = 0;


                if (claveMoneda == "bitcoin")
                {
                    claveMonedaCryptoYa = "btc";
                    bitcoin = cantidadCryptoComprada;
                }
                else if (claveMoneda == "ethereum")
                {
                    claveMonedaCryptoYa = "eth";
                    ethereum = cantidadCryptoComprada;
                }
                else
                {
                    usdt = cantidadCryptoComprada;
                    claveMonedaCryptoYa = "usdt";
                }

                string urlApi = $"https://criptoya.com/api/{claveMonedaCryptoYa}/ars";


                var response = await _httpClient.GetAsync(urlApi);


                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(400, new { error = $"No se encontró la criptomoneda: {request.CryptoCode}" });
                }

                var contenidoJson = await response.Content.ReadAsStringAsync();


                dynamic datosCripto = JsonConvert.DeserializeObject(contenidoJson);


                decimal precioCripto = datosCripto["binance"]["totalAsk"];


                decimal montoTotal = request.CryptoAmount * precioCripto;

                string action = request.Action.ToLower();
                string cryptocode = request.CryptoCode.ToLower();

                var transaccion = new Transaccion
                {
                    CryptoCode = cryptocode,
                    Action = action,
                    NombreCliente = request.NombreCliente,
                    CryptoAmount = request.CryptoAmount,
                    MoneySpent = montoTotal,
                    TransactionDateTime = transactionDateTime
                };

                _context.Transacciones.Add(transaccion);


                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.NombreCliente == nombreDelCliente);
                var idCliente = cliente.Id;


                var billeteraExistente = await _context.Billetera.FirstOrDefaultAsync(b => b.IdCliente == idCliente);


                if (action == "purchase")
                {



                    if (billeteraExistente == null)
                    {
                        var billeteraCliente = new Billetera
                        {
                            Bitcoin = bitcoin,
                            Ethereum = ethereum,
                            USDT = usdt,
                            IdCliente = idCliente

                        };

                        _context.Billetera.Add(billeteraCliente);

                    }
                    else
                    {
                        billeteraExistente.Bitcoin += bitcoin;
                        billeteraExistente.Ethereum += ethereum;
                        billeteraExistente.USDT += usdt;
                    }





                }
                else
                {

                    if (cryptocode == "bitcoin")
                    {
                        billeteraExistente.Bitcoin -= request.CryptoAmount;
                    }
                    else if (cryptocode == "ethereum")
                    {
                        billeteraExistente.Ethereum -= request.CryptoAmount;
                    }
                    else
                    {
                        billeteraExistente.USDT -= request.CryptoAmount;
                    }

                }
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    idTransaccion = transaccion.Id,
                    message = "Transacción registrada exitosamente",
                    nombreCliente = transaccion.NombreCliente,
                    cryptoCode = transaccion.CryptoCode,
                    cryptoPrice = precioCripto,
                    cryptoAmount = transaccion.CryptoAmount,
                    totalMoneySpent = transaccion.MoneySpent
                });


            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error al procesar la transacción: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTransacciones()
        {
            var transacciones = await _context.Transacciones.ToListAsync();
            return Ok(transacciones);
        }

        [HttpGet("{nombreDelCliente}")]
        public async Task<IActionResult> GetTransaccion(string nombreDelCliente)
        {
            var historialCliente = _context.Transacciones
            .Where(c => c.NombreCliente == nombreDelCliente)
            .ToList();
            return Ok(historialCliente);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaccion(int id, [FromBody] TransaccionDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.Action.ToLower() != "purchase" && request.Action.ToLower() != "sale")
            {
                return BadRequest(new { error = "La acción debe ser 'purchase' o 'sale'" });
            }

            if (!DateTime.TryParse(request.DateTime, out DateTime transactionDateTime))
            {
                return BadRequest(new { error = "El formato de fecha es inválido." });
            }

            try
            {
                string claveMonedaCryptoYa = "";
                string claveMoneda = request.CryptoCode;
                decimal cantidadCrypto = request.CryptoAmount;
                string nombreDelCliente = request.NombreCliente;




                if (claveMoneda == "bitcoin")
                {
                    claveMonedaCryptoYa = "btc";

                }
                else if (claveMoneda == "ethereum")
                {
                    claveMonedaCryptoYa = "eth";

                }
                else
                {

                    claveMonedaCryptoYa = "usdt";
                }


                string urlApi = $"https://criptoya.com/api/{claveMonedaCryptoYa}/ars";


                var response = await _httpClient.GetAsync(urlApi);


                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(400, new { error = $"No se encontró la criptomoneda: {request.CryptoCode}" });
                }

                var contenidoJson = await response.Content.ReadAsStringAsync();


                dynamic datosCripto = JsonConvert.DeserializeObject(contenidoJson);


                decimal precioCripto = datosCripto["binance"]["totalAsk"];


                decimal montoTotal = request.CryptoAmount * precioCripto;

                string action = request.Action.ToLower();
                string cryptocode = request.CryptoCode.ToLower();

                var transaccion = new Transaccion
                {
                    Id = id,
                    CryptoCode = cryptocode,
                    Action = action,
                    NombreCliente = request.NombreCliente,
                    CryptoAmount = request.CryptoAmount,
                    MoneySpent = montoTotal,
                    TransactionDateTime = transactionDateTime
                };



                
                var transaccionModificar = await _context.Transacciones.FirstOrDefaultAsync(t => t.Id == id);
                if (transaccionModificar == null) return NotFound(new { error = "La transacción no existe." });

                if (transaccionModificar.NombreCliente != request.NombreCliente)
                {
                    return BadRequest(new { error = "No está permitido cambiar el cliente de una transacción existente." });
                }

                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.NombreCliente == request.NombreCliente);
                if (cliente == null) return BadRequest(new { error = "El cliente no existe." });

                var billeteraCliente = await _context.Billetera.FirstOrDefaultAsync(b => b.IdCliente == cliente.Id);
                if (billeteraCliente == null) return BadRequest(new { error = "El cliente no tiene una billetera activa." });

                string monedaVieja = transaccionModificar.CryptoCode;
                decimal montoViejo = transaccionModificar.CryptoAmount;

                
                if (transaccionModificar.Action == "purchase")
                {
                    if (monedaVieja == "bitcoin") billeteraCliente.Bitcoin -= montoViejo;
                    else if (monedaVieja == "ethereum") billeteraCliente.Ethereum -= montoViejo;
                    else if (monedaVieja == "usdt") billeteraCliente.USDT -= montoViejo;
                }
                else 
                {
                    if (monedaVieja == "bitcoin") billeteraCliente.Bitcoin += montoViejo;
                    else if (monedaVieja == "ethereum") billeteraCliente.Ethereum += montoViejo;
                    else if (monedaVieja == "usdt") billeteraCliente.USDT += montoViejo;
                }

                
                decimal montoNuevo = transaccion.CryptoAmount;
                string monedaNueva = transaccion.CryptoCode;

                if (transaccion.Action == "purchase")
                {
                    if (monedaNueva == "bitcoin") billeteraCliente.Bitcoin += montoNuevo;
                    else if (monedaNueva == "ethereum") billeteraCliente.Ethereum += montoNuevo;
                    else if (monedaNueva == "usdt") billeteraCliente.USDT += montoNuevo;
                }
                else 
                {
                
                    if (monedaNueva == "bitcoin" && (billeteraCliente.Bitcoin - montoNuevo) < 0)
                        return BadRequest(new { error = "No tiene suficiente saldo en Bitcoin para esta modificación." });

                    if (monedaNueva == "ethereum" && (billeteraCliente.Ethereum - montoNuevo) < 0)
                        return BadRequest(new { error = "No tiene suficiente saldo en Ethereum para esta modificación." });

                    if (monedaNueva == "usdt" && (billeteraCliente.USDT - montoNuevo) < 0)
                        return BadRequest(new { error = "No tiene suficiente saldo en USDT para esta modificación." });

                    if (monedaNueva == "bitcoin") billeteraCliente.Bitcoin -= montoNuevo;
                    else if (monedaNueva == "ethereum") billeteraCliente.Ethereum -= montoNuevo;
                    else if (monedaNueva == "usdt") billeteraCliente.USDT -= montoNuevo;
                }

                
                _context.Entry(transaccionModificar).CurrentValues.SetValues(transaccion);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error al procesar la transacción: {ex.Message}" });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaccion(int id)
        {
            var transaccion = await _context.Transacciones.FindAsync(id);
            if (transaccion == null) return NotFound(new { error = "La transacción no existe." });

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.NombreCliente == transaccion.NombreCliente);
            if (cliente != null)
            {
                var billeteraCliente = await _context.Billetera.FirstOrDefaultAsync(b => b.IdCliente == cliente.Id);

                if (billeteraCliente != null)
                {
                    string accion = transaccion.Action;
                    decimal monto = transaccion.CryptoAmount;
                    string criptomoneda = transaccion.CryptoCode;

                    
                    if (accion == "purchase")
                    {
                        if (criptomoneda == "bitcoin" && billeteraCliente.Bitcoin < monto)
                            return BadRequest(new { error = "No se puede borrar. El cliente ya gastó estos Bitcoins." });
                        if (criptomoneda == "ethereum" && billeteraCliente.Ethereum < monto)
                            return BadRequest(new { error = "No se puede borrar. El cliente ya gastó estos Ethereums." });
                        if (criptomoneda == "usdt" && billeteraCliente.USDT < monto)
                            return BadRequest(new { error = "No se puede borrar. El cliente ya gastó estos USDT." });

                        if (criptomoneda == "bitcoin") billeteraCliente.Bitcoin -= monto;
                        if (criptomoneda == "ethereum") billeteraCliente.Ethereum -= monto;
                        if (criptomoneda == "usdt") billeteraCliente.USDT -= monto;
                    }
                    else 
                    {
                        if (criptomoneda == "bitcoin") billeteraCliente.Bitcoin += monto;
                        if (criptomoneda == "ethereum") billeteraCliente.Ethereum += monto;
                        if (criptomoneda == "usdt") billeteraCliente.USDT += monto;
                    }
                }
            }

            _context.Transacciones.Remove(transaccion);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Transacción eliminada con éxito" }); 
        }



    }
}