using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using ProyectoDSW_Cosmeticos.DAO;
using ProyectoDSW_Cosmeticos.Models;
using System.Data;

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

        IEnumerable<Pedido> pedidos()
        {
            List<Pedido> lista = new List<Pedido>();
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("exec usp_pedidos", cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Pedido()
                    {
                        idpedido = dr.GetInt32(0),
                        fpedido = dr.GetDateTime(1),
                        dni = dr.GetString(2),
                        nombre= dr.GetString(3),
                        email = dr.GetString(4)                        
                    });
                }
            }
            return lista;
        }

        IEnumerable<PedidoDetalle> pedidosDetalle(int id)
        {
            List<PedidoDetalle> lista = new List<PedidoDetalle>();
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("exec usp_pedidos_deta @idpedido", cn);
                cn.Open();
                cmd.Parameters.AddWithValue("@idpedido", id);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new PedidoDetalle()
                    {
                        idpedido = dr.GetInt32(0),
                        nomproducto = dr.GetString(1),
                        cantidad = dr.GetInt32(2),
                        precio = dr.GetDecimal(3)
                    });
                }
            }
            return lista;
        }

        public IActionResult ListaProductos()
        {         
            return View(productos());
        }

        public IActionResult ListaPedidos()
        {
            return View(pedidos());
        }

        public IActionResult ListaPedidosDetalle(int id)
        {
            return View(pedidosDetalle(id));
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
        //usuarios
        Usuario Buscar(String dni = "")
        {
            Usuario reg = usuarios().Where(p => p.Dni == dni).FirstOrDefault();
            if (reg == null)
                reg = new Usuario();

            return reg;

        }
        IEnumerable<Usuario> usuarios()
        {
            List<Usuario> lista = new List<Usuario>();
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("exec usp_usuarios", cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Usuario()
                    {
                        Dni = dr.GetString(0),
                        login = dr.GetString(1),
                        clave = dr.GetString(2),
                        NombreCompleto = dr.GetString(3),
                        intentos=dr.GetInt32(5),
                    });
                }
            }
            return lista;
        }
        public IActionResult listaUsuarios()
        {

            return View(usuarios());
        }

        public string agregarUsu(Usuario reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cn"]))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("exec usp_registrar_usuario @dni,@usuario,@clave,@nomcom,@rol", cn);
                    cmd.Parameters.AddWithValue("@dni", reg.Dni);
                    cmd.Parameters.AddWithValue("@usuario", reg.login);
                    cmd.Parameters.AddWithValue("@clave", reg.clave);
                    cmd.Parameters.AddWithValue("@nomcom", reg.NombreCompleto);
                    cmd.Parameters.AddWithValue("@rol", reg.rol);
                    cmd.ExecuteReader();
                    mensaje = $"Se ha agregado el usuario con {reg.Dni}";
                }
                catch (Exception ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }


        public string actualizarUsu(Usuario reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cn"]))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("exec usp_editar_usuario @dni,@usuario,@clave,@nomcom,@rol,@intentos,@fecBloque", cn);
                    cmd.Parameters.AddWithValue("@dni", reg.Dni);
                    cmd.Parameters.AddWithValue("@usuario", reg.login);
                    cmd.Parameters.AddWithValue("@clave", reg.clave);
                    cmd.Parameters.AddWithValue("@nomcom", reg.NombreCompleto);
                    cmd.Parameters.AddWithValue("@rol", reg.rol);
                    cmd.Parameters.AddWithValue("@intentos", reg.intentos);
                    cmd.Parameters.AddWithValue("@fecBloque", reg.fecBloqueo);

                    cmd.ExecuteReader();
                    mensaje = $"Se ha actualizado el usuario con {reg.Dni}";
                }
                catch (Exception ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }

        public IActionResult EditarUsuario(String id)
        {
            Usuario reg = Buscar(id);
            if (reg == null) return RedirectToAction("listaUsuarios");

            return View(reg);
        }
        [HttpPost]
        public IActionResult EditarUsuario(Usuario reg)
        {
            ViewBag.mensaje = actualizarUsu(reg);
            return View(reg);
        }

        public IActionResult AgregarUsuario()
        {
            return View(new Usuario());
        }

        [HttpPost]
        public IActionResult AgregarUsuario(Usuario reg)
        {
            ViewBag.mensaje = agregarUsu(reg);
            return View(reg);
        }

        public string eliminarUsu(String dni)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cn"]))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("exec usp_eliminar_usuario @dni", cn);
                    cmd.Parameters.AddWithValue("@dni", dni);
                    cmd.ExecuteReader();
                    mensaje = $"Se ha eliminado el usuario con {dni}";
                }
                catch (Exception ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }

        public IActionResult EliminarUsuario(String id)
        {
            Usuario reg = Buscar(id);
            if (reg == null) return RedirectToAction("listaUsuarios");
            ViewBag.mensaje = eliminarUsu(reg.Dni);
            return RedirectToAction("listaUsuarios");
        }

        /*
         * Mantenimiento Categorias
         */

        public IActionResult ListaCategorias()
        {
            return View(categorias());
        }

        public string agregarCat(Categoria reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cn"]))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("exec usp_registrar_categoria @nom", cn);
                    cmd.Parameters.AddWithValue("@nom", reg.nombre);                    
                    cmd.ExecuteReader();
                    mensaje = $"Se ha agregado la categoria {reg.nombre}";
                }
                catch (Exception ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }

        public IActionResult AgregarCategoria()
        {            
            return View(new Categoria());
        }
        [HttpPost] public IActionResult AgregarCategoria(Categoria reg)
        {            
            ViewBag.mensaje = agregarCat(reg);
            return View(reg);
        }
        public string actualizarCat(Categoria reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cn"]))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("exec usp_editar_categoria @id,@nom", cn);
                    cmd.Parameters.AddWithValue("@id", reg.id);
                    cmd.Parameters.AddWithValue("@nom", reg.nombre);                    
                    cmd.ExecuteReader();
                    mensaje = $"Se ha actualizado la categoria {reg.nombre}";
                }
                catch (Exception ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }
        Categoria BuscarCat(int id = 0)
        {
            Categoria reg = categorias().Where(p => p.id == id).FirstOrDefault();
            if (reg == null)
                reg = new Categoria();

            return reg;

        }
        public IActionResult EditarCategoria(int id)
        {
            Categoria reg = BuscarCat(id);
            if (reg == null) return RedirectToAction("ListaCategorias");
            return View(reg);
        }
        [HttpPost] public IActionResult EditarCategoria(Categoria reg)
        {            
            ViewBag.mensaje = actualizarCat(reg);
            return View(reg);
        }

        IEnumerable<Producto> productosCat(int id)
        {
            List<Producto> lista = new List<Producto>();
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("exec usp_catProductos @idCat", cn);
                cn.Open();
                cmd.Parameters.AddWithValue("@idCat", id);                
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Producto()
                    {
                        idproducto = dr.GetInt32(0),
                        nombreproducto = dr.GetString(1),
                        precio = dr.GetDecimal(2),
                        unidades = dr.GetInt16(3)
                    });
                }
            }
            return lista;
        }
        public IActionResult ListaProductosCategoria(int id)
        {
            return View(productosCat(id));
        }

    }
}
