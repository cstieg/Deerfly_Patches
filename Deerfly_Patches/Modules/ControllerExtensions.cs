using System.Web.Mvc;

namespace Deerfly_Patches.Modules
{
    public static class ControllerExtensions
    {
        public static JsonResult JOk(this Controller controller)
        {
            return new JsonResult { Data = new { success = "True" } };
        }
    }
}