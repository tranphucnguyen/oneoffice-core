using Microsoft.AspNetCore.Http;
using OneOffice.Api.Foundation;

namespace OneOffice.Tests.Unit;

[Trait("Category", "Unit")]
public sealed class ApiProblemsTests
{
    [Theory]
    [InlineData(400, "request.invalid")]
    [InlineData(401, "auth.session_invalid")]
    [InlineData(403, "access.denied")]
    [InlineData(404, "resource.not_found")]
    [InlineData(409, "concurrency.conflict")]
    [InlineData(429, "rate_limit.exceeded")]
    [InlineData(500, "server.unexpected")]
    public void Status_maps_to_stable_contract(int status, string code)
    {
        var context = new DefaultHttpContext();
        var problem = ApiProblems.Create(context, status);
        Assert.Equal(status, problem.Status);
        Assert.Equal(code, problem.Extensions["code"]);
        Assert.Equal($"urn:oneoffice:problem:{code}", problem.Type);
        Assert.Null(problem.Detail);
    }
}
