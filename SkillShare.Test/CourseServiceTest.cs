using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using SkillShare.Application.Services;
using SkillShare.Domain.Dto.CourseDto;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Interfaces.Repositories;
using SkillShare.Domain.Interfaces.Validations;
using SkillShare.Domain.Result;
using SkillShare.Domain.Settings;
using SkillShare.Producer.Interfaces;
using SkillShare.Test.Configuration;
using Xunit;

namespace SkillShare.Test;


public class CourseServiceTests
{
    private IMapper GetMapper()
    {
        var config = new TypeAdapterConfig();

        var mockServiceProvider = new Mock<IServiceProvider>();

        return new ServiceMapper(mockServiceProvider.Object, config);
    }


    [Fact]
    public async Task GetCourse_ShouldBe_NotNull()
    {
        // Arrange
        var mockRepository = MockRepositoriesGetter.GetMockCourseRepository();
        var mockDistributedCache = new Mock<IDistributedCache>();
        var mapper = GetMapper();

        var reportService = new CourseService(mockRepository.Object, null, mapper, null,
         null, null, null, mockDistributedCache.Object, null);

        // Act
        var result = await reportService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
    }

    [Fact] 
    public async Task CreateCourse_ShouldBe_Return_NewCourse()
    {
        // Arrange
        var mockCourseRepository = MockRepositoriesGetter.GetMockCourseRepository();
        var mockUserRepository = MockRepositoriesGetter.GetMockUserRepository();
        var mockDistributedCache = new Mock<IDistributedCache>();
        var mapper = GetMapper();

        var mockMessageProducer = new Mock<IMessageProducer>();
        var mockValidator = new Mock<ICourseValidator>();
        var mockOptions = new Mock<IOptions<RabbitMqSettings>>();
        mockOptions.Setup(o => o.Value).Returns(new RabbitMqSettings { RoutingKey = "test", ExchangeName = "test" });
        mockValidator.Setup(v => v.ValidatorCreate(It.IsAny<Course>(), It.IsAny<User>())).Returns(BaseResult.Success());

        var user = MockRepositoriesGetter.GetUsers().FirstOrDefault();
        var createCourseDto = new CreateCourseDto(
            Title: "Test Course",
            Description: "Test Description",
            Price: 100,
            ParentId: null
        );

        // Act 
        var courseService = new CourseService(
            mockCourseRepository.Object,
            null, 
            mapper,                     
            null,   
            mockUserRepository.Object,                    
            mockMessageProducer.Object,
            mockOptions.Object,       
            mockDistributedCache.Object,
            null
        );

        var result = await courseService.CreateAsync(user.Id, createCourseDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task DeleteCourse_ShouldBe_Return_TrueSuccess()
    {
        // Arrange
        var mockCourseRepository = MockRepositoriesGetter.GetMockCourseRepository();
        var mapper = GetMapper();
        var course = MockRepositoriesGetter.GetCourses().FirstOrDefault();

        // Act
        var courseService = new CourseService(mockCourseRepository.Object, null, mapper, null,
        null, null, null, null, null);
        var result = await courseService.DeleteAsync(course.Id);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateReport_ShouldBe_Return_NewData_For_Report()
    {
        // Arrange
        var mockCourseRepository = MockRepositoriesGetter.GetMockCourseRepository();
        var mapper = GetMapper();
        var course = MockRepositoriesGetter.GetCourses().FirstOrDefault();
        var updateCourseDto = new UpdateCourseDto(course.Id, "New name", "New description", 100);

        // Act
        var courseService = new CourseService(mockCourseRepository.Object, null, mapper, null,
        null, null, null, null, null);
        var result = await courseService.UpdateAsync(updateCourseDto);

        // Assert
        Assert.True(result.IsSuccess);
    }
}