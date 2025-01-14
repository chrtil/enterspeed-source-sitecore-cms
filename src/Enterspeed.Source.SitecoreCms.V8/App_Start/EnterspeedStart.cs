using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using RazorGenerator.Mvc;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Enterspeed.Source.SitecoreCms.V8.EnterspeedStart), "Start")]

namespace Enterspeed.Source.SitecoreCms.V8
{
    public static class EnterspeedStart
    {
        public static void Start()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }
    }
}
