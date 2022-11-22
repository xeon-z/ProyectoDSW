namespace ProyectoDSW_Cosmeticos.Models
{
    public class Categoria
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public Categoria() {
            nombre = "";
        }
    }
}
