using JetBrains.Annotations;

namespace Hosting.Infrastructure.MediatR.Results
{
    public class CreatedAtActionResult : IRequestResult
    {
        public CreatedAtActionResult(
            [AspMvcAction] string actionName,
            [AspMvcController] string controllerName,
            object? value = null,
            object? routeValues = null)
        {
            ActionName = actionName;
            ControllerName = controllerName;
            Value = value;
            RouteValues = routeValues;
        }

        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public object? Value { get; set; }
        public object? RouteValues { get; set; }
    }
}