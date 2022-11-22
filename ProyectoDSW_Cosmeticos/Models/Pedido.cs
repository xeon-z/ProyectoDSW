namespace ProyectoDSW_Cosmeticos.Models
{
    public class Pedido
    {
        public string dni { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
        public Pedido()
        {
            dni = "";
            nombre = "";
            email = "";
        }
    }
}
