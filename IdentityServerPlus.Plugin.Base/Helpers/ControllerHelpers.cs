using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServerPlus.Plugin.Base.Helpers
{
    public static class ControllerHelpers
    {
        public static void CopyFrom(this ModelStateDictionary target, ModelStateDictionary source)
        {
            CopyTo(source, target);
        }
        public static void CopyTo(this ModelStateDictionary source, ModelStateDictionary target)
        {
            foreach (var error in source)
            {
                foreach (var modelError in error.Value.Errors)
                {
                    if (string.IsNullOrWhiteSpace(modelError.ErrorMessage))
                        target.AddModelError(error.Key, modelError.Exception, null);
                    else
                        target.AddModelError(error.Key, modelError.ErrorMessage);
                }
            }
        }

        /// <summary>
        /// Execute a controller method, and copy the ModelState, ViewBag and other view related information to the response from the source controller
        /// </summary>
        /// <typeparam name="TController">The controller whos action you want to call</typeparam>
        /// <typeparam name="TReturnType">The response type from the action</typeparam>
        /// <param name="target">The controller whos action you want to call</param>
        /// <param name="source">The controller who is perfoming the call</param>
        /// <param name="controllerAction">The controller action to perform on the target</param>
        /// <returns></returns>
        public static async Task<TReturnType> ExecuteWithStateAsync<TController, TReturnType>(this TController target, Controller source, Func<TController, Task<TReturnType>> controllerAction)
            where TController : Controller
        {
            source.ModelState.CopyTo(target.ModelState);

            var result = await controllerAction.Invoke(target);

            target.ModelState.CopyTo(source.ModelState);
            
            return result;
        }
    }
}
