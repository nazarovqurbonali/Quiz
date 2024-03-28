using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api")]
public class BaseController : ControllerBase
{
    [NonAction]
    protected List<string> ModelStateErrors() =>
        ModelState.SelectMany(e => e.Value!.Errors.Select(er => er.ErrorMessage)).ToList();
}