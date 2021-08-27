using eSigningEnvelope.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSigningEnvelope.DTO
{
    public class EnvelopeLogDTO
    {
        public string ProcessedBy { get; set; }
        public Statuses CurrentStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}