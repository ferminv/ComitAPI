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
        [Route("padrontish/cuit/{cuit}")]
        public PadronTISH Get(Int64 cuit)
        {
            using (var db = new ComitAPIDBContext())
            {
                return db.Padron.Find(cuit);

            }
        }

        [HttpGet]
        [Route("padrontish/percepcion/{cuit}")]
        [ResponseType(typeof(Decimal))]
        public Decimal Percepcion(Int64 cuit)
        {
            using (var db = new ComitAPIDBContext())
            {
                var p = db.Padron.Find(cuit);
                if (p != null)
                    return p.AlicuotaPercepcion;
                else
                    return 0;

            }
        }

        [HttpGet]
        [Route("padrontish/retencion/{cuit}")]
        [ResponseType(typeof(Decimal))]
        public Decimal Retencion(Int64 cuit)
        {
            using (var db = new ComitAPIDBContext())
            {
                var p = db.Padron.Find(cuit);
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
                    var postedFile = httpRequest.Files[file];
                    if (nuevaVersion(postedFile.FileName)) 
                    {
                        var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);
                        postedFile.SaveAs(filePath);

                        Task.Run(() =>
                        {
                            using (var db = new ComitAPIDBContext())
                            {
                                //guardamos la data de la nueva version del padron
                                var padronData = db.PadronData.Find(1);
                                padronData.Version = postedFile.FileName.Split('_')[1];
                                padronData.FechaSubida = DateTime.Today.Date;
                                //limpiamos el padron para cargar los nuevos registros
                                db.Database.ExecuteSqlCommand("TRUNCATE TABLE padron_tish;");
                                db.SaveChanges();

                                //leemos archivo y guardamos registro a registro en la bd
                                var sr = new StreamReader(filePath);
                                var linea = sr.ReadLine();
                                do
                                {
                                    var datos = linea.Split(';');

                                    var nCUIT = datos[3];
                                    var percepcion = datos[4];
                                    var retencion = datos[5];
                                    var p = new PadronTISH
                                    {
                                        Cuit = Convert.ToInt64(nCUIT),
                                        AlicuotaPercepcion = Convert.ToDecimal(percepcion),
                                        AlicuotaRetencion = Convert.ToDecimal(retencion)
                                    };
                                    db.Padron.Add(p);

                                    linea = sr.ReadLine();
                                }
                                while (linea != null);
                                sr.Close();
                                db.SaveChanges();
                            }
                        });
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.Created, "El Archivo ya esta en su ultima version.");
                    }
                }
                return Request.CreateErrorResponse(HttpStatusCode.Created, "Archivo(s) subido(s) correctamente! Que tenga un buen dia.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

        private static Boolean nuevaVersion(string nombreArchivo)
        {
            using (var db = new ComitAPIDBContext())
            {
                var padronDataVersion = db.PadronData.Find(1)?.Version;

                var versionArchivoNuevo = nombreArchivo.Split('_')[1];

                if (padronDataVersion != null)
                {
                    return (padronDataVersion != versionArchivoNuevo);
                }
                else //si no hay nada cargado
                {
                    var pd = new PadronData
                    {
                        IdPadron = 1,
                        Version = versionArchivoNuevo,
                        FechaSubida = DateTime.Today.Date

                    };
                    db.PadronData.Add(pd);
                    db.SaveChanges();
                    return true;
                }
            }
        }

    }

}
