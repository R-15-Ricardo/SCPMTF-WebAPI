using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace scpmtf_webapi.Models
{
    public class ScpGPT3Summary
    {
        public int Id { get; set; }
        [Required]
        public int ItemNo { get; set; }
        [Required]
        public string CombatSummary { get; set; }
    }
}
