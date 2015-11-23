using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DocumentFlow.Models
{
    public class DocumentTemplate
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public int TypeID { get; set; }

        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
    }
}