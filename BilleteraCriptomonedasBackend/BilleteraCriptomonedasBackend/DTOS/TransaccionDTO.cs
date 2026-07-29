
using System.ComponentModel.DataAnnotations;
 
namespace BILLETERACRIPTOMONEDASBACKEND.DTOS
{
    public class TransaccionDTO
    {
        [Required(ErrorMessage = "El código de criptomoneda es requerido")]
        public string CryptoCode { get; set; }
 
        [Required(ErrorMessage = "La acción es requerida")]
        public string Action { get; set; } 
 
        
        public string NombreCliente { get; set; }
 
        [Required(ErrorMessage = "La cantidad de criptomoneda es requerida")]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal CryptoAmount { get; set; }
 
        [Required(ErrorMessage = "La fecha y hora son requeridas")]
        public string DateTime { get; set; } 
    }
    
}