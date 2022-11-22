using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using ProyectoDSW_Cosmeticos.DAO;
using ProyectoDSW_Cosmeticos.Models;

namespace ProyectoDSW_Cosmeticos.Controllers
{
    public class MantenimientoController : Controller
    {
        private readonly IConfiguration _configuration;

        public MantenimientoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        IEnumerable<Producto> productos()
        {
            List<Producto> lista = new List<Producto>();
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("exec usp_productos", cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Producto()
                    {
                        idproducto = dr.GetInt32(0),
                        nombreproducto = dr.GetString(1),
                        nombrecategoria = dr.GetString(2),
                        precio = dr.GetDecimal(3),
                        unidades = dr.GetInt16(4)
                    });
                }
            }
            return lista;
        }
        IEnumerable<Categoria> categorias()
        {
            List<Categoria> lista = new List<Categoria>();
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("exec usp_categorias", cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Categoria()
                    {
                        id = dr.GetInt32(0),
                        nombre = dr.GetString(1),
                    });
                }
            }
            return lista;
        }
        Producto Buscar(int id = 0)
        {
            Producto reg = productos().Where(p => p.idproducto == id).FirstOrDefault();
            if (reg == null)
                reg = new Producto();

            return reg;

        }
        public string agregarProd(Producto reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cn"]))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("exec usp_registrar_producto @Nombre,@IdCat ,@Pre,@stock", cn);
                    cmd.Parameters.AddWithValue("@Nombre", reg.nombreproducto);
                    cmd.Parameters.AddWithValue("@IdCat", reg.idcategoria);
                    cmd.Parameters.AddWithValue("@Pre", reg.precio);
                    cmd.Parameters.AddWithValue("@stock", reg.unidades);
                    cmd.ExecuteReader();
                    mensaje = $"Se ha agregado el producto {reg.nombreproducto}";
                }
                catch (Exception ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }
        public string actualizarProd(Producto reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cn"]))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("exec usp_editar_producto @id,@Nombre,@IdCat ,@Pre,@stock", cn);
                    cmd.Parameters.AddWithValue("@id", reg.idproducto);
                    cmd.Parameters.AddWithValue("@Nombre", reg.nombreproducto);
                    cmd.Parameters.AddWithValue("@IdCat", reg.idcategoria);
                    cmd.Parameters.AddWithValue("@Pre", reg.precio);
                    cmd.Parameters.AddWithValue("@stock", reg.unidades);
                    cmd.ExecuteReader();
                    mensaje = $"Se ha actualizado el producto {reg.nombreproducto}";
                }
                catch (Exception ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }
        public string eliminarProd(int id)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cn"]))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("exec usp_eliminar_producto @id", cn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteReader();
                    mensaje = $"Se ha eliminado el producto {id}";
                }
                catch (Exception ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }


        public IActionResult ListaProductos()
        {
         
            return View(productos());
        }
        public IActionResult AgregarProducto()
        {
            ViewBag.categorias= new SelectList(categorias(), "id", "nombre");

            return View(new Producto());
        }
        [HttpPost]public IActionResult AgregarProducto(Producto reg)
        {
            ViewBag.categorias = new SelectList(categorias(), "id", "nombre", reg.idcategoria);
            ViewBag.mensaje = agregarProd(reg);
            return View(reg);
        }
        public IActionResult EditarProducto(int id)
        {
            Producto reg = Buscar(id);
            if (reg == null) return RedirectToAction("ListaProductos");
            ViewBag.categorias = new SelectList(categorias(), "id", "nombre");

            return View(reg);
        }
        [HttpPost]
        public IActionResult EditarProducto(Producto reg)
        {
            ViewBag.categorias = new SelectList(categorias(), "id", "nombre", reg.idcategoria);
            ViewBag.mensaje = actualizarProd(reg);
            return View(reg);
        }
        public IActionResult EliminarProducto(int id)
        {
            Producto reg = Buscar(id);
            if (reg == null) return RedirectToAction("ListaProductos");
            ViewBag.mensaje = eliminarProd(reg.idproducto);
            return RedirectToAction("ListaProductos");
        }

    }
}
