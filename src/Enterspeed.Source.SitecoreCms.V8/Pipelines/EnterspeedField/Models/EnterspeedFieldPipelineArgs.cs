using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Enterspeed.Source.SitecoreCms.V8.Pipelines.EnterspeedField.Models
{
    public class EnterspeedFieldPipelineArgs : PipelineArgs
    {


            public string Value { get; set; }

        public Item Item { get; set; }
        public Field Field { get; set; }

    }
}