using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ComitAPI.Models;
using System.Web.Http.Description;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace ComitAPI.Controllers
{
    public class PadronTISHController : ApiController
    {

        [HttpGet]
        [Route("padrontish/cuit")]
        public List<PadronTISH> Get()
        {
            using (var db = new ComitAPIDBContext())
            {
                return db.Padron.ToList();
            }
        }

        [HttpGet]
        [Route("padrontish/cuit/{id}")]
        public PadronTISH Get(Int64 id)
        {
            using (var db = new ComitAPIDBContext())
            {
                return db.Padron.Find(id);

            }
        }

        [HttpGet]
        [Route("padrontish/percepcion/{id}")]
        [ResponseType(typeof(Decimal))]
        public Decimal Percepcion(Int64 id)
        {
            using (var db = new ComitAPIDBContext())
            {
                var p = db.Padron.Find(id);
                if (p != null)
                    return p.AlicuotaPercepcion;
                else
                    return 0;

            }
        }

        [HttpGet]
        [Route("padrontish/retencion/{id}")]
        [ResponseType(typeof(Decimal))]
        public Decimal Retencion(Int64 id)
        {
            using (var db = new ComitAPIDBContext())
            {
                var p = db.Padron.Find(id);
                if (p != null)
                    return p.AlicuotaRetencion;
                else
                    return 0;
            }
        }

        [HttpPost, AllowAnonymous]
        [Route("padrontish/actualizar")]
        public HttpResponseMessage ActualizarPadron()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    var filePath = String.Empty;
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);
                        postedFile.SaveAs(filePath);
                    }

                    var message1 = string.Format("Archivo subido correctamente! Que tenga un buen dia.");
                    Task.Run(() =>
                    {
                        using (var db = new ComitAPIDBContext())
                        {
                            var sr = new StreamReader(filePath);
                            var linea = sr.ReadLine();
                            do
                            {
                                var datos = linea.Split(';');
                                string nCUIT = datos[3];

                                string Percepcion = datos[4];
                                string Retencion = datos[5];
                                linea = sr.ReadLine();
                                var p = new PadronTISH()
                                {
                                    Cuit = Convert.ToInt64(nCUIT),
                                    AlicuotaPercepcion = Convert.ToDecimal(Percepcion),
                                    AlicuotaRetencion = Convert.ToDecimal(Retencion)
                                };
                                db.Padron.Add(p);
                            }
                            while (linea != null);
                            sr.Close();
                            db.SaveChanges();
                        }
                    });

                    return Request.CreateErrorResponse(HttpStatusCode.Created, message1); 
                }
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }
    }

}
