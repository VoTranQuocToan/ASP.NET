using System.Web;
using System.Web.Mvc;

namespace VoTranQuocToan_2122110425
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
