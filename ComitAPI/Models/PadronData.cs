using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ComitAPI.Models
{
    [Table("padron_data")]
    public class PadronData
    {
        [Key]
        public Int32 IdPadron { get; set; }

        public String Version { get; set; }

        public DateTime FechaSubida { get; set; }

    }
}