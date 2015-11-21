using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentFlow.Models
{
    public class EditUserModel
    {

        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Patronymic { get; set; }

        public int Position { get; set; }

    }
}