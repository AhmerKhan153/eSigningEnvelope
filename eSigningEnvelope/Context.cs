using eSigningEnvelope.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSigningEnvelope
{
    public class Context : DbContext
    {
        public DbSet<Envelope> Envelope { get; set; }
        public DbSet<EnvelopeLog> EnvelopeLog { get; set; }

    }
}