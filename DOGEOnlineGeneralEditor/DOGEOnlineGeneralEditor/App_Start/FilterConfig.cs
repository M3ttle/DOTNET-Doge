using DOGEOnlineGeneralEditor.Utilities;
using System.Web;
using System.Web.Mvc;

namespace DOGEOnlineGeneralEditor
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomHandleErrorAttribute());
        }
    }
}
