using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using eSigningEnvelope;
using eSigningEnvelope.DTO;
using eSigningEnvelope.Models;

namespace eSigningEnvelope.Controllers
{
    [RoutePrefix("Envelope")]
    public class TransmissionController : ApiController
    {
        private Context db = new Context();
        private string CreatedBy, SignedBy, SentBy = string.Empty;

        public TransmissionController()
        {
            CreatedBy = ConfigurationManager.AppSettings["CreatedBy"].ToString();
            SignedBy = ConfigurationManager.AppSettings["SignedBy"].ToString();
            SentBy = ConfigurationManager.AppSettings["SentBy"].ToString();
        }

        // GET: api/Transmission
        [Route("GetEnvelopes")]
        public string GetEnvelope()
        {
            var envelopes = db.Envelope.Select(z =>
            new EnvelopeDTO
            {
                Id = z.EnvelopeId,
                CurrentStatus = z.CurrentStatus,
                NoOfItems = z.NoOfItems,
                Title = z.Title
            }).ToList();

            var envelopeLogs = db.EnvelopeLog.ToList();

            foreach (var env in envelopes)
            {
                env.Details = envelopeLogs.Where(z => z.EnvelopeId == env.Id).Select(r =>
                new EnvelopeLogDTO
                {
                    CurrentStatus = r.CurrentStatus,
                    CreatedBy = r.CreatedBy,
                    CreatedDate = r.CreatedDate,
                    ProcessedBy = r.ProcessedBy
                }).ToList();
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(envelopes);
        }

        // GET: api/Transmission/5
        [Route("{id}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetEnvelope(int id)
        {
            Envelope envelope = await db.Envelope.FindAsync(id);
            if (envelope == null)
            {
                return NotFound();
            }

            EnvelopeDTO envDTO = new EnvelopeDTO { Id = envelope.EnvelopeId, CurrentStatus = envelope.CurrentStatus, NoOfItems = envelope.NoOfItems, Title = envelope.Title };
            envDTO.Details = db.EnvelopeLog.Where(z => z.EnvelopeId == envDTO.Id).Select(r =>
            new EnvelopeLogDTO
            {
                CurrentStatus = r.CurrentStatus,
                CreatedBy = r.CreatedBy,
                CreatedDate = r.CreatedDate,
                ProcessedBy = r.ProcessedBy
            }).ToList();
            return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(envDTO));
        }

        // PUT: api/Transmission/5
        [ResponseType(typeof(void))]
        [Route("sent/{id}")]
        public async Task<IHttpActionResult> PutEnvelope(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var env = await db.Envelope.Where(z => z.EnvelopeId == id).FirstOrDefaultAsync();
            if (env == null) { return NotFound(); }
            if (env.CurrentStatus == Constant.Statuses.New)
            {

                env.CurrentStatus = Constant.Statuses.Sent;
                db.EnvelopeLog.Add(new EnvelopeLog { EnvelopeId = env.EnvelopeId, CurrentStatus = Constant.Statuses.Sent, CreatedBy = CreatedBy, CreatedDate = DateTime.Now, ProcessedBy = SentBy });
                db.Entry(env).State = EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnvelopeExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return StatusCode(HttpStatusCode.NoContent);
            }
            else { return NotFound(); }
        }

        // PUT: api/Transmission/5
        [ResponseType(typeof(void))]
        [Route("signed/{id}")]
        public async Task<IHttpActionResult> PutEnvelope2(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var env = await db.Envelope.Where(z => z.EnvelopeId == id).FirstOrDefaultAsync();
            if (env == null) { return NotFound(); }
            if (env.CurrentStatus == Constant.Statuses.Sent)
            {
                env.CurrentStatus = Constant.Statuses.Signed;
                db.EnvelopeLog.Add(new EnvelopeLog { EnvelopeId = env.EnvelopeId, CurrentStatus = Constant.Statuses.Signed, CreatedBy = CreatedBy, CreatedDate = DateTime.Now, ProcessedBy = SentBy });
                db.Entry(env).State = EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnvelopeExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: api/Transmission
        [HttpPost]
        [ResponseType(typeof(Envelope))]
        public async Task<IHttpActionResult> PostEnvelope(Envelope envelope)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            envelope.CreatedDate = DateTime.Now;
            envelope.CreatedBy = CreatedBy;
            db.Envelope.Add(envelope);
            await db.SaveChangesAsync();

            db.EnvelopeLog.Add(new EnvelopeLog { EnvelopeId = envelope.EnvelopeId, CreatedBy = CreatedBy, CreatedDate = DateTime.Now, CurrentStatus = Constant.Statuses.New, ProcessedBy = "Sys User" });
            await db.SaveChangesAsync();
            return Ok(envelope.EnvelopeId);
        }

        // DELETE: api/Transmission/5
        [ResponseType(typeof(Envelope))]
        public async Task<IHttpActionResult> DeleteEnvelope(int id)
        {
            Envelope envelope = await db.Envelope.FindAsync(id);
            if (envelope == null)
            {
                return NotFound();
            }

            db.Envelope.Remove(envelope);
            await db.SaveChangesAsync();

            return Ok(envelope);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EnvelopeExists(int id)
        {
            return db.Envelope.Count(e => e.EnvelopeId == id) > 0;
        }
    }
}