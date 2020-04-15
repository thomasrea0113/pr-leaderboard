using System;
using Microsoft.AspNetCore.Mvc;

namespace Leaderboard.Extensions
{
    public static class ControllerExtensions
    {
        public static BadRequestObjectResult ModelValidationError(this ControllerBase controller) =>
            controller.BadRequest(
                controller.ProblemDetailsFactory.CreateValidationProblemDetails(
                    controller.HttpContext, controller.ModelState));
    }
}
