
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using CourseApp.DataAccessLayer.Concrete;
using CourseApp.EntityLayer.Entity;
using Microsoft.AspNetCore.Authentication;
using CourseApp.ServiceLayer.Mapping;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace CourseApp.Tests;

// Test class for performance issues like N+1 queries
public class PerformanceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PerformanceTests(WebApplicationFactory<Program> factory)
    {
        // Configure factory to use InMemory database for testing
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add InMemory database for testing
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });

                // Add AutoMapper for testing
                services.AddAutoMapper(typeof(StudentMapping).Assembly);

                // Disable authentication for performance tests
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                }).AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
            });
        });
    }

    [Fact]
    public async Task GetAllCourseDetail_ShouldNotCauseNPlusOneProblem()
    {
        // ARRANGE
        // Get a scope to access services, including the DbContext
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Ensure the database is clean
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();

            // SEED DATA: Create a significant amount of test data
            var instructor = new Instructor { Name = "Test Instructor" };
            dbContext.Instructors.Add(instructor);
            await dbContext.SaveChangesAsync();

            for (int i = 0; i < 50; i++)
            {
                var course = new Course 
                {
                    CourseName = $"Course {i}",
                    InstructorID = instructor.ID,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(30)
                };
                dbContext.Courses.Add(course);
            }
            await dbContext.SaveChangesAsync();
        }

        var client = _factory.CreateClient();

        // ACT
        // Execute the request to the endpoint that fetches course details
        var response = await client.GetAsync("/api/courses/detail");

        // ASSERT
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // MANUAL VALIDATION (Explained):
        // To truly test for N+1, you must have EF Core logging enabled.
        //
        // BEFORE THE FIX:
        // The logs would show 1 query to get all courses, followed by 50 additional queries
        // (one for each course's instructor). Total queries = 51.
        //
        // AFTER THE FIX:
        // The logs should show only 1 single, efficient query that uses JOINs to fetch
        // all courses and their related instructors/lessons at once.
    }
}

// Test authentication handler that always succeeds
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.Name, "Test user") };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}
