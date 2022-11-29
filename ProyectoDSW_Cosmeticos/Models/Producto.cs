using System.ComponentModel.DataAnnotations;
namespace ProyectoDSW_Cosmeticos.Models
{
    public class Producto
    {
        [Display (Name ="Codigo")]public int idproducto { get; set; }
        [Display(Name = "Description")] public string nombreproducto { get; set; }
        [Display(Name = "Categoria")] public string nombrecategoria { get; set; }
        [Display(Name = "IdCategoria")] public int idcategoria { get; set; }
        [Display(Name = "Precio")] public decimal precio { get; set; }
        [Display(Name = "Unidades Disponibles")] public Int16 unidades { get; set; }
        [Display(Name = "Foto")] public string ruta { get; set; }        
    }
}
