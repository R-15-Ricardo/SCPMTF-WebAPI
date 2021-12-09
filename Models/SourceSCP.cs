using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace scpmtf_webapi.Models
{
    public class SourceSCP
    {
        public string ObjectClass { get; set; }
        public string Description { get; set; }
        public string ObjectImage { get; set; }

        public void FieldsNormalize ()
        {
            var objClassSplit = this.ObjectClass.Split(' ');
            this.ObjectClass = Regex.Replace((objClassSplit.Length > 4) ? objClassSplit.Skip(4).FirstOrDefault() : objClassSplit[3],
                                            "[^a-zA-Z]+", "",
                                            RegexOptions.Compiled).Trim();

            this.Description = Regex.Replace(this.Description, "[^a-zA-Z\u00C0-\u017F0-9_. ,-]+", "", RegexOptions.Compiled).Trim();
        }
    }
}
