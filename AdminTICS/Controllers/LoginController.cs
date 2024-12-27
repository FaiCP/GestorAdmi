using Comun.ViewModels;
using Logica.BLL;
using Modelo.Modelo;
using Modelo.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AdminTICS.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : ApiController
    {
        private readonly DbConexion _context; // Usa tu contexto de base de datos

        public LoginController()
        {
            _context = new DbConexion(); // Reemplaza con tu contexto
        }

        [HttpPost]
        [Route("api/login")]
        public IHttpActionResult Login(LoginVMR login)
        {
            var respuesta = new RespuestasVMR<ListadoPaginadoVMR<PersonalVMR>>();
            if (login == null || string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Email y Password son requeridos.");
            }

            // Buscar el usuario en la base de datos
            var usuario = _context.Usuarios.FirstOrDefault(u => u.email == login.Email);
            try
            {
                if (usuario == null || usuario.Pass != login.Password) // Comparar contraseñas
                {
                    return Unauthorized(); // Respuesta 401
                }
            }
            catch(Exception ex) 
            {
                respuesta.codigo = HttpStatusCode.InternalServerError;
                respuesta.datos = null;
                respuesta.mensajesErrors.Add(ex.Message);
            }
            // Generar un token (en este caso un JWT o un token simple de ejemplo)
            var token = Guid.NewGuid(); // Esto es solo un ejemplo. Usa JWT en producción.

            return Ok(new
            {
                Token = token,
                User = new
                {
                    usuario.id,
                    usuario.nombre,
                    usuario.email,
                    usuario.cargo
                }
            });
        }

    }
}
