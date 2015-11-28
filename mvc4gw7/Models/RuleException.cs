using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;

namespace mvc4gw7.Models
{
    public class RuleException : Exception
    {
        public NameValueCollection Errors { get; private set; }
        
        public RuleException(string key, string value)
        {
            Errors = new NameValueCollection { { key, value } };
        }
        
        public RuleException(NameValueCollection errors)
        {
            Errors = errors;
        }

        public void CopyToModelState(ModelStateDictionary modelState)
        {
            foreach (string key in Errors)
                foreach (string value in Errors.GetValues(key))
                    modelState.AddModelError(key, value);
        }


    }
}