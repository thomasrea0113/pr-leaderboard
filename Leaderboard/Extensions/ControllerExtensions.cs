using System;
using Microsoft.AspNetCore.Mvc;

namespace Leaderboard.Extensions
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// An error result for when additional model errors are added in the controller method.
        /// The ApiControllerAttribute adds a filter to automatically handle validaiton errors
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static BadRequestObjectResult ValidationError(this ControllerBase controller) =>
            controller.BadRequest(
                controller.ProblemDetailsFactory.CreateValidationProblemDetails(
                    controller.HttpContext, controller.ModelState));
    }
}
