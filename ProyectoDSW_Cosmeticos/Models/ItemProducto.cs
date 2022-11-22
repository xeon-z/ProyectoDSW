using System.ComponentModel.DataAnnotations;

namespace ProyectoDSW_Cosmeticos.Models
{
    public class ItemProducto
    {
        [Display(Name = "Codigo")] public int idproducto { get; set; }
        [Display(Name = "Descripcion")] public string nombreproducto { get; set; }

        [Display(Name = "Categoria")] public string nombrecategoria { get; set; }
        [Display(Name = "Precio")] public decimal precio { get; set; }
        [Display(Name = "Cantidad")]public int cantidad { get; set; }
        [Display(Name = "Monto")] public decimal monto { get { return precio * cantidad; } }
        public ItemProducto()
        {
            nombrecategoria = "";
            nombreproducto = "";

        }
    }
}
