using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sol_Demos.Extensions.Services;
using Sol_Demos.Services;

namespace Sol_Demos.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class HmacDemoController : ControllerBase
{
    private readonly DataResponseFactory _dataResponseFactory;

    public HmacDemoController(DataResponseFactory dataResponseFactory)
    {
        _dataResponseFactory = dataResponseFactory;
    }

    [HttpPost("create")]
    [HmacSignatureValidationService]
    public IActionResult Create([FromBody] UserDto user)
    {
        var response = _dataResponseFactory.SetResponse<UserDto>(true, "Success", 200, user);

        return base.Ok(response);
    }
}

public class UserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}