using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoDSW_Cosmeticos.DAO;
using ProyectoDSW_Cosmeticos.Models;
using System.Data;
using Microsoft.AspNetCore.Session;

namespace ProyectoDSW_Cosmeticos.Controllers
{
    public class AccesoController : Controller
    {
        string cadena = @"server = DESKTOP-53VKG7K;database = Cosmetica;Trusted_Connection = True;MultipleActiveResultSets = True;TrustServerCertificate = False;Encrypt = False";

        string verifica(string lg, string clave, char rol)
        {
            //debe retornar el valor de @fullname ejecutando el procedure usp_acceso_usuario
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_acceso_usuario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@usuario", lg);
                    cmd.Parameters.AddWithValue("@clave", clave);
                    cmd.Parameters.Add("@fullname", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@sw", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@rol", rol);
                    cmd.ExecuteNonQuery(); //ejecutar


                    //guardar los resultados
                    HttpContext.Session.SetString("fullname", cmd.Parameters["@fullname"].Value.ToString());

                    HttpContext.Session.SetInt32("sw", (int)(cmd.Parameters["@sw"].Value));
                }
                catch (Exception ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }


        public IActionResult Logueo()
        {
            //enviar un nuevo Usuario

            return View(new Usuario());

        }

        /*
        [HttpPost]
        public IActionResult Logueo(Usuario reg)
        {
            //validar el ingreso de datos DataAnnotations
            if (!ModelState.IsValid)
            {
                ViewBag.mensaje = "Ingrese el usuario y la clave";
                return View(reg);
            }

            //si los datos estan ingresados envio los datos para verificar
            string mensaje = verifica(reg.login, reg.clave);
            //valuo a sw
            if (HttpContext.Session.GetInt32("sw") == 0)
            {
                //significa que no existe envio el mensaje del metodo y el mensaje del fullname
                ModelState.AddModelError("", mensaje);
                ViewBag.mensaje = HttpContext.Session.GetString("fullname");
                ViewBag.alerta = "ALERTA";
                return View(reg);
            }
            else if (HttpContext.Session.GetInt32("sw") == 1)
            {
                //redirecciono a la Vista Plataforma
                return RedirectToAction("Portal", "Ecomerce");
            }
            else 
            //if (HttpContext.Session.GetInt32("sw") == 2)
            {
                //redirecciono a la Vista Lista Productos
                return RedirectToAction("ListaProductos", "Mantenimiento");
            }
        }
        */

        [HttpPost]
        public IActionResult Logueo(Usuario reg)
        {
            //validar el ingreso de datos DataAnnotations
            if (!ModelState.IsValid)
            {
                ViewBag.mensaje = "Ingrese el usuario y la clave";
                return View(reg);
            }

            //si los datos estan ingresados envio los datos para verificar
            string mensaje = verifica(reg.login, reg.clave, reg.rol);
            //valuo a sw
            switch (HttpContext.Session.GetInt32("sw"))
            {
                case 0:
                    //significa que no existe envio el mensaje del metodo y el mensaje del fullname
                    ModelState.AddModelError("", mensaje);
                    ViewBag.mensaje = HttpContext.Session.GetString("fullname");
                    ViewBag.alerta = "ALERTA";
                    return View(reg);                    

                case 1:
                    return RedirectToAction("Portal", "Ecomerce");

                default:
                    return RedirectToAction("ListaProductos", "Mantenimiento");
            }
        }

    }

}

