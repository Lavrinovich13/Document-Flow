using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentFlow.Models
{
    public class DocumentTemplate
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int TypeID { get; set; }
        public string Text {get; set;}
    }
}