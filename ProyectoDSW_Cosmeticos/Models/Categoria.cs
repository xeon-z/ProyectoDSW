using System.ComponentModel.DataAnnotations;
namespace ProyectoDSW_Cosmeticos.Models
{
    public class Categoria
    {
        [Display(Name = "ID")] public int id { get; set; }
        [Display(Name = "Nombre")] public string nombre { get; set; }
        public Categoria() {
            nombre = "";
        }
    }
}
