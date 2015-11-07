using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DocumentFlow.Models
{
    public class EditModel
    {
        [DataType(DataType.Text)]
        public string Login { get; set; }

        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [DataType(DataType.Text)]
        public string Patronymic { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public int Position { get; set; }
    }
}