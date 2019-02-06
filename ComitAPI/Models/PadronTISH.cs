using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ComitAPI.Models
{
    [Table("padron_tish")]
    public class PadronTISH
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 Cuit { get; set; }

        public Decimal AlicuotaPercepcion { get; set; }

        public Decimal AlicuotaRetencion { get; set; }

    }
}