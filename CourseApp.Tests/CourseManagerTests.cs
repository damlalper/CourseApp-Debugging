using AutoMapper;
using CourseApp.DataAccessLayer.Concrete;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.CourseDto;
using CourseApp.ServiceLayer.Concrete;
using CourseApp.ServiceLayer.Mapping;
using Microsoft.EntityFrameworkCore;

namespace CourseApp.Tests;

public class CourseManagerTests : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;
    private readonly AppDbContext _context;
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly CourseManager _courseManager;

    public CourseManagerTests()
    {
        // Set up InMemory database
        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(_dbContextOptions);
        _unitOfWork = new UnitOfWork(_context);

        // Set up AutoMapper
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new CourseMapping());
            mc.AddProfile(new StudentMapping());
            // Add other profiles if needed by the manager
        });
        _mapper = mappingConfig.CreateMapper();

        _courseManager = new CourseManager(_unitOfWork, _mapper);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnError_WhenCourseNameIsTooShort()
    {
        // Arrange
        var courseDto = new CreateCourseDto { CourseName = "A", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) };

        // Act
        var result = await _courseManager.CreateAsync(courseDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Course Name must be between 2 and 50 characters.", result.Message);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnError_WhenEndDateIsBeforeStartDate()
    {
        // Arrange
        var courseDto = new CreateCourseDto { CourseName = "Valid Course Name", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-1) };

        // Act
        var result = await _courseManager.CreateAsync(courseDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("End date must be after the start date.", result.Message);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnError_WhenCourseNameIsNotUnique()
    {
        // Arrange: Seed the database with a course that has the same name
        await _context.Courses.AddAsync(new EntityLayer.Entity.Course { CourseName = "Existing Course" });
        await _context.SaveChangesAsync();

        var courseDto = new CreateCourseDto { CourseName = "Existing Course", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) };

        // Act
        var result = await _courseManager.CreateAsync(courseDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("A course with this name already exists.", result.Message);
    }

    [Fact]
    public async Task CreateAsync_Should_Succeed_WhenDataIsValid()
    {
        // Arrange
        var courseDto = new CreateCourseDto { CourseName = "Valid New Course", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) };

        // Act
        var result = await _courseManager.CreateAsync(courseDto);
        var count = await _context.Courses.CountAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnError_WhenInstructorIdDoesNotExist()
    {
        // Arrange
        var nonExistentInstructorId = Guid.NewGuid().ToString();
        var courseDto = new CreateCourseDto 
        {
            CourseName = "Course with Bad Instructor", 
            StartDate = DateTime.Now, 
            EndDate = DateTime.Now.AddDays(1),
            InstructorID = nonExistentInstructorId
        };

        // Act
        var result = await _courseManager.CreateAsync(courseDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Instructor not found.", result.Message);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
