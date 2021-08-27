using eSigningEnvelope.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSigningEnvelope.DTO
{
    public class EnvelopeDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int NoOfItems { get; set; }
        public Statuses CurrentStatus { get; set; }
        public IEnumerable<EnvelopeLogDTO> Details { get; set; }
    }
}