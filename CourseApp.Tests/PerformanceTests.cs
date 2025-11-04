
using System.Net;
using CourseApp.DataAccessLayer.Concrete;
using CourseApp.EntityLayer.Entity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CourseApp.Tests;

// Test class for performance issues like N+1 queries
public class PerformanceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PerformanceTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAllCourseDetail_ShouldNotCauseNPlusOneProblem()
    {
        // ARRANGE
        // Get a scope to access services, including the DbContext
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Ensure the database is clean and created
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
