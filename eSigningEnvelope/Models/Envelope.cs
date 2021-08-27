using eSigningEnvelope.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSigningEnvelope.Models
{
    public class Envelope
    {
        public int EnvelopeId { get; set; }
        public string Title { get; set; }
        public int NoOfItems { get; set; }
        public Statuses CurrentStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}