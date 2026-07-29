
using System;
using System.ComponentModel.DataAnnotations;
 
namespace BILLETERACRIPTOMONEDASBACKEND.Models
{
    public class Transaccion
    {
        [Key]
        public int Id { get; set; }
 
        [Required]
        public string CryptoCode { get; set; }
 
        [Required]
        public string Action { get; set; }
 
        [Required]
        public string NombreCliente{ get; set; } 
 
        [Required]
        public decimal CryptoAmount { get; set; } 
 
        [Required]
        public decimal MoneySpent { get; set; } 
 
        [Required]
        public DateTime TransactionDateTime { get; set; }
 
        
        
    }
}