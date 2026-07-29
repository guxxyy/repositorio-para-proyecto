
using Microsoft.AspNetCore.Mvc;
using BILLETERACRIPTOMONEDASBACKEND.Models;
using BILLETERACRIPTOMONEDASBACKEND.Data;
using Microsoft.EntityFrameworkCore;

namespace BILLETERACRIPTOMONEDASBACKEND.Controller
{
    [Route("api/[controller]/")]
    [ApiController]
    public class BilleteraController: ControllerBase
    {
        private readonly AppDbContext _context;
        public BilleteraController(AppDbContext context)
        {
            _context = context; 
        }
        
        [HttpGet]
        public async Task<IActionResult> ObtenerBilleteras()
        {
            var allBilleteras = await _context.Billetera.ToListAsync();
            return Ok(allBilleteras);
        }
    }

}


