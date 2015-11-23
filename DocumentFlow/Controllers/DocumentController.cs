using DocumentFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DocumentFlow.Controllers
{
    public class DocumentController : Controller
    {

        [HttpGet]
        public ActionResult Convert(DocumentTemplate template)
        {
            template.Text = ReplaceBy(template.Text, BuildDictionary());
            return View(template);
        }

        private string ReplaceBy(string text, Dictionary<string, string> dictionary)
        {
            string pattern = @"#(\w+)";

            MatchCollection matchCollection =
                Regex.Matches(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

            if (matchCollection.Count != 0)
            {
                foreach (var match in matchCollection)
                {
                    if (dictionary.ContainsKey(match.ToString()))
                    {
                        text = text.Replace(match.ToString(), dictionary[match.ToString()]);
                    }
                }
            }

            return text;
        }

        private Dictionary<string, string> BuildDictionary()
        {
            IEnumerable<Position> positions;
            using (ApplicationContext context = new ApplicationContext())
            {
                positions = new List<Position>(context.Positions);
            }

            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            foreach (var position in positions)
            {
                var lowerCasePosition = position.Name.ToLower();
                dictionary.Add("#" + lowerCasePosition, SelectHtml(position.Id));
            }

            return dictionary;
        }

        private string SelectHtml(string id)
        {
            return "<select>" + OptionsHtml(id) + "</select>";
        }

        private string OptionsHtml(string id)
        {
            List<ApplicationUser> users;
            using (var context = new ApplicationContext())
            {
                users = new List<ApplicationUser>(context.Users.Where(x => x.PositionId == id));
            }

            StringBuilder optionsString = new StringBuilder();
            foreach (var user in users)
            {
                optionsString.Append
                    ("<option value=" + user.Id + ">" + user.FirstName + " " + user.LastName + " " + user.Patronymic + "</option>");
            }
            return optionsString.ToString();
        }
    }
}