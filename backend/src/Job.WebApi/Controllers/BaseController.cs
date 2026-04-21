using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Job.WebApi.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    protected string? GetCnpj()
    {
        var cnpj = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;
        return cnpj;
    }
}