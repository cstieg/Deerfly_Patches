using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Deerfly_Patches.Modules
{
    /// <summary>
    /// Helper class for controller extensions
    /// </summary>
    public static class ControllerHelper
    {
        /// <summary>
        /// Extension to return success response acceptable to Javascript
        /// </summary>
        public static JsonResult JOk(this Controller controller)
        {
            return new JsonResult { Data = new { success = "True" } };
        }

        /// <summary>
        /// Gets the names of all controllers in project
        /// </summary>
        /// <returns>A list of the names of all the controllers</returns>
        public static List<string> GetControllerNames()
        {
            List<string> controllerNames = new List<string>();
            GetSubClasses<Controller>().ForEach(
                type => controllerNames.Add(type.Name));
            return controllerNames;
        }

        /// <summary>
        /// Helper method to get subclasses of class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static List<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(
                Type => Type.IsSubclassOf(typeof(T))).ToList();

        }
    }
}