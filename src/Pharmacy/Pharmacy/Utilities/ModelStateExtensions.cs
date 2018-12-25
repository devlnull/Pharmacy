using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Utilities
{
    public static class ModelStateExtensions
    {
        public static string ErrorGathering(this ModelStateDictionary ModelState)
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (var item in ModelState.Values)
            {
                foreach (var error in item.Errors)
                    strBuilder.AppendLine(error.ErrorMessage);
            }
            return strBuilder.ToString();
        }

        public static string ErrorGathering(this IdentityResult result)
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (var item in result.Errors)
            {
                strBuilder.AppendLine(item.Description);
            }
            return strBuilder.ToString();
        }
    }
}
