using Microsoft.AspNetCore.Mvc;
using StudioMilIdeias.Api.Controllers;

namespace StudioMilIdeias.Api.Tests;

public sealed class HealthControllerTests
{
    [Fact]
    public void Get_ReturnsOkWithStatusOk()
    {
        var controller = new HealthController();

        var result = controller.Get();
        var okResult = Assert.IsType<OkObjectResult>(result);

        var statusProperty = okResult.Value?.GetType().GetProperty("status");
        var status = statusProperty?.GetValue(okResult.Value)?.ToString();

        Assert.Equal("ok", status);
    }
}
