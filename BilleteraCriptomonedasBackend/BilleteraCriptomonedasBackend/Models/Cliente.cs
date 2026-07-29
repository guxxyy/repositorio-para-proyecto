using System.ComponentModel.DataAnnotations;
 
namespace BILLETERACRIPTOMONEDASBACKEND.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NombreCliente{get;set;}
 
        [Required]
        public string Nombre { get; set; }
 
        [Required]
        public string Email { get; set; }

        // relacion 
        public Billetera? Billetera{get;set;}
    }
}
 


