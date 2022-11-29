using System.ComponentModel.DataAnnotations;
namespace ProyectoDSW_Cosmeticos.Models
{
    public class PedidoDetalle
    {
        [Display(Name = "Id Pedido")] public int idpedido { get; set; }
        [Display(Name = "Nombre del Producto")] public string nomproducto { get; set; }
        [Display(Name = "Cantidad")] public int cantidad { get; set; }
        [Display(Name = "Precio")] public decimal precio { get; set; }

    }
}
