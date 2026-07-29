using System.ComponentModel.DataAnnotations;
 
namespace BILLETERACRIPTOMONEDASBACKEND.Models
{
    
    public class Billetera
    {
        [Key]
        public int Id{get;set;}
        [Required]
        public decimal Bitcoin{get;set;}
        [Required]
        public decimal Ethereum{get;set;}
        [Required]
        public decimal USDT{get;set;}

        // relaciones
        public int IdCliente{get;set;} 
        public Cliente Cliente{get;set;} 
    }
}