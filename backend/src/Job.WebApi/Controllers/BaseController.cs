using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Job.WebApi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class BaseController : ControllerBase
{
    protected string? GetCnpj()
    {
        var cnpj = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;
        return cnpj;
    }
}