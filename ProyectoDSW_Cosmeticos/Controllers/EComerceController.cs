using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Session;
using Newtonsoft.Json;
using ProyectoDSW_Cosmeticos.Models;
using ProyectoDSW_Cosmeticos.DAO;
using Microsoft.AspNetCore.Http;

namespace ProyectoDSW_Cosmeticos.Controllers
{
    public class EComerceController : Controller
    {
        private readonly IConfiguration _configuration;

        public EComerceController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        IEnumerable<Producto> productos()
        {
            List<Producto> lista = new List<Producto>();
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cn"])) {
                SqlCommand cmd = new SqlCommand("exec usp_productos", cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Producto() {
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

        Producto Buscar(int id = 0)
        {
            Producto reg = productos().Where(p => p.idproducto == id).FirstOrDefault();
            if (reg == null)
                reg = new Producto();

                return reg;
            
        }
        public IActionResult AgregarACarrito(int id = 0)
        {
            Producto reg = productos().FirstOrDefault(p => p.idproducto == id);
            if (reg == null) return RedirectToAction("Portal");

            return View(reg);
        }
        [HttpPost]public IActionResult AgregarACarrito(int idproducto, int cantidad)
        {
            Producto reg = productos().FirstOrDefault(p => p.idproducto == idproducto);
            ItemProducto bean = new ItemProducto()
            {
                idproducto = reg.idproducto,
                nombreproducto = reg.nombreproducto,
                nombrecategoria = reg.nombrecategoria,
                precio = reg.precio,
                cantidad = cantidad
            };
            List<ItemProducto> temporal = JsonConvert.DeserializeObject<List<ItemProducto>>(HttpContext.Session.GetString("Canasta"));
            temporal.Add(bean);
            HttpContext.Session.SetString("Canasta", JsonConvert.SerializeObject(temporal));
            ViewBag.mensaje = $"El producto {reg.nombreproducto} se ha agregado al carrito";
            return View(reg);
        }
        public IActionResult Resumen()
        {
            List<ItemProducto> temporal = JsonConvert.DeserializeObject<List<ItemProducto>>(HttpContext.Session.GetString("Canasta"));
            return View(temporal);
        }
        public IActionResult Delete(int id, int q)
        {
            List<ItemProducto> temporal = JsonConvert.DeserializeObject<List<ItemProducto>>(HttpContext.Session.GetString("Canasta"));
            temporal.Remove(temporal.FirstOrDefault(p => p.idproducto == id && p.cantidad == q));
            HttpContext.Session.SetString("Canasta", JsonConvert.SerializeObject(temporal));
            return RedirectToAction("Resumen");
        }
        public IActionResult Comprar()
        {
            ViewBag.canasta = JsonConvert.DeserializeObject<List<ItemProducto>>(HttpContext.Session.GetString("Canasta"));
            return View(new Pedido());
        }
        [HttpPost]public IActionResult Comprar(Pedido reg)
        {
            string mensaje = "";
            using(SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cn"])){
                cn.Open();
                SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_agrega_pedido", cn, tr);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@idpedido", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@dni",reg.dni);
                    cmd.Parameters.AddWithValue("@nombre", reg.nombre);
                    cmd.Parameters.AddWithValue("@email", reg.email);
                    cmd.ExecuteNonQuery();
                    int idpedido = (int)cmd.Parameters["@idpedido"].Value;
                    List<ItemProducto> temporal = JsonConvert.DeserializeObject<List<ItemProducto>>(HttpContext.Session.GetString("Canasta"));
                    foreach(var it in temporal)
                    {
                        cmd = new SqlCommand("usp_agrega_detalle", cn, tr);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@idpedido", idpedido);
                        cmd.Parameters.AddWithValue("@idproducto", it.idproducto);
                        cmd.Parameters.AddWithValue("@cantidad", it.cantidad);
                        cmd.Parameters.AddWithValue("@precio", it.precio);
                        cmd.ExecuteNonQuery();
                    }
                    tr.Commit();
                    mensaje = $"Se ha generado el pedido {idpedido}";
                }
                catch (SqlException ex)
                {
                    mensaje = ex.Message; tr.Rollback();                 
                }
                finally { cn.Close(); }
            }
            ViewBag.mensaje = mensaje;
            ViewBag.canasta = JsonConvert.DeserializeObject<List<ItemProducto>>(HttpContext.Session.GetString("Canasta"));
            return View(reg);
        }

            public async Task<IActionResult> Portal()
            {
                if(HttpContext.Session.GetString("Canasta")==null)
                    HttpContext.Session.SetString("Canasta", JsonConvert.SerializeObject(new List<ItemProducto>()));
                return View(await Task.Run(() => productos()));

            }
        }
        
    }
