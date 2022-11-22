namespace ProyectoDSW_Cosmeticos.Models;
using System.ComponentModel.DataAnnotations;

public class Usuario
{

    public String Dni { get; set; }
    [Display(Name = "Login del usuario"), Required] public string login { get; set; }
    [Display(Name = "Clave del usuario"), Required] public string clave { get; set; }
    public int intentos { get; set; }
    public DateTime fecBloqueo { get; set; }
    public char rol { get; set; }

    public Usuario()
    {
        Dni = "";
        fecBloqueo = DateTime.Now;
        intentos = 0;
        clave = "";
        login = "";
    }

}


