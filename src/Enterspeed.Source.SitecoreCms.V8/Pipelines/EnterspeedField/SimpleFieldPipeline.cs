using Enterspeed.Source.SitecoreCms.V8.Pipelines.EnterspeedField.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Enterspeed.Source.SitecoreCms.V8.Pipelines.EnterspeedField
{
    public class SomeProcessor
    {
        public void Process(EnterspeedFieldPipelineArgs args)
        {
            args.Value = GetResult(args.Value);
        }
        private string GetResult(string value)
        {
            return value + " " +"Hello world";
        }
    }
}