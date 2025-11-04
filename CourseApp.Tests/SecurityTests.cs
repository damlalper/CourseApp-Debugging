
using System.Net;
using System.Net.Http.Json;
using CourseApp.EntityLayer.Dto.CourseDto;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CourseApp.Tests;

// Test class for security vulnerabilities
public class SecurityTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SecurityTests(WebApplicationFactory<Program> factory)
    {
        // The WebApplicationFactory automatically starts the API in-memory
        _factory = factory;
    }

    [Fact]
    public async Task CreateCourse_WithoutAuthToken_ShouldReturnUnauthorized()
    {
        // ARRANGE
        // Create a client that does not have any authentication headers
        var client = _factory.CreateClient();
        
        var newCourse = new CreateCourseDto
        {
            CourseName = "Unauthorized Test Course",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(1),
            InstructorID = "1" // Assuming a valid instructor ID
        };

        // ACT
        // Send a POST request to the endpoint that creates a course
        var response = await client.PostAsJsonAsync("/api/courses", newCourse);

        // ASSERT
        // After the [Authorize] attribute was added, this should return 401 Unauthorized.
        // Before the fix, this would have returned 200 OK, indicating a major security flaw.
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
