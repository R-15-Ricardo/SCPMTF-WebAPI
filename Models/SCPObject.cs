using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace scpmtf_webapi.Models
{
    public class SCPObject
    {
        public int Id { get; set; }
        [Required]
        public string ItemNo { get; set; }
        [Required]
        public string ObjectClass { get; set; }
        [Required]
        public string Description { get; set; }
        public string ImageAttachment { get; set; }
    }
}
