
using Microsoft.AspNetCore.Mvc;
using BILLETERACRIPTOMONEDASBACKEND.Models;
using BILLETERACRIPTOMONEDASBACKEND.Data;
using Microsoft.EntityFrameworkCore;

namespace BILLETERACRIPTOMONEDASBACKEND.Controller
{
    [Route("api/[controller]/")]
    [ApiController]
    public class ClienteController: ControllerBase
    {

        private readonly AppDbContext _context;
        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CrearCliente([FromBody] Cliente nuevoCliente)
        {
            try
            {
                _context.Clientes.Add(nuevoCliente);

                await _context.SaveChangesAsync();
                return Ok(nuevoCliente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet]

        public async Task<IActionResult> GetAllClientes()
        {

            var allClientes = await _context.Clientes.ToListAsync();
            return Ok(allClientes);
            
        }
        
    }

}