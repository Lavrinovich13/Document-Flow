using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DocumentFlow.Models
{
    public class DocumentModel  
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string TemplateId { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string Text { get; set; }
        public string Pass { get; set; }
        public string CurrentUserId { get; set; }
    }
}