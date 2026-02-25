using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using SkillShare.Application.Resources;
using SkillShare.Domain.Dto.CourseDto;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Enum;
using SkillShare.Domain.Interfaces.Repositories;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Interfaces.Validations;
using SkillShare.Domain.Result;
using SkillShare.Domain.Settings;
using SkillShare.Producer.Interfaces;

namespace SkillShare.Application.Services;

public class CourseService : ICourseService
{
    private readonly IMessageProducer _messageProducer;
    private readonly IOptions<RabbitMqSettings> _rabbitMqOptions;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<Course> _courseRepository;
    private readonly ICourseValidator _courseValidator;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public CourseService(IBaseRepository<Course> courseRepository,
        ILogger logger, IMapper mapper,
        ICourseValidator courseValidator,
        IBaseRepository<User> userRepository, IMessageProducer messageProducer, IOptions<RabbitMqSettings> rabbitMqOptions)
    {
        _courseValidator = courseValidator;
        _courseRepository = courseRepository;
        _logger = logger;
        _mapper = mapper;
        _userRepository = userRepository;
        _messageProducer = messageProducer;
        _rabbitMqOptions = rabbitMqOptions;
    }

    public async Task<DataResult<CourseDto>> CreateAsync(long userId, CreateCourseDto dto, CancellationToken ct = default)
    {
        var user = await _userRepository.GetAll().Where(x => x.Id == userId).FirstOrDefaultAsync(ct);
        var course = await _courseRepository.GetAll().Where(x => x.Title == dto.Title).FirstOrDefaultAsync(ct);
        var result = _courseValidator.ValidatorCreate(course, user);
        if (!result.IsSuccess)
        {
            return DataResult<CourseDto>.Failure((int)ErrorCodes.CourseNotFound, ErrorMessage.CourseNotFound);
        }

        course = new Course()
        {
            Title = dto.Title,
            Description = dto.Description,
            ParentId = dto.ParentId,
            Price = dto.Price,
            AuthorId = userId
        };
        await _courseRepository.CreateAsync(course);
        await _courseRepository.SaveChangesAsync();

        _messageProducer.SendMessage(course, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

        return DataResult<CourseDto>.Success(_mapper.Map<CourseDto>(course));
    }



    public async Task<DataResult<CourseDto>> DeleteAsync(long id, CancellationToken ct = default)
    {
        var course = await _courseRepository.GetAll()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);

        var result = _courseValidator.ValidateOnNull(course);
        if (!result.IsSuccess)
        {
            return DataResult<CourseDto>.Failure((int)ErrorCodes.CourseNotFound, ErrorMessage.CourseNotFound);
        }
        _courseRepository.Remove(course);
        await _courseRepository.SaveChangesAsync();

        return DataResult<CourseDto>.Success(_mapper.Map<CourseDto>(course));
    }


    public async Task<CollectionResult<CourseDto>> GetByAuthorIdAsync(long AuthorId, CancellationToken ct = default)
    {
        var course = await _courseRepository.GetAll()
                .Where(x => x.AuthorId == AuthorId)
                .ProjectToType<CourseDto>()
                .ToArrayAsync(ct);
        if (course == null)
        {
            return CollectionResult<CourseDto>.Failure((int)ErrorCodes.CourseNotFound, ErrorMessage.CourseNotFound);
        }
        return CollectionResult<CourseDto>.Success(course);
    }

    public async Task<DataResult<CourseDto>> GetByIdAsync(long courseId, CancellationToken ct = default)
    {
        var course = await _courseRepository.GetAll()
            .Where(x => x.Id == courseId)
            .ProjectToType<CourseDto>()
            .FirstOrDefaultAsync(ct);
        if (course == null)
        {
            return DataResult<CourseDto>.Failure((int)ErrorCodes.CourseNotFound, ErrorMessage.CourseNotFound);
        }
        return DataResult<CourseDto>.Success(course);
    }

    public async Task<DataResult<CourseDto>> UpdateAsync(UpdateCourseDto dto, CancellationToken ct = default)
    {
        var course = await _courseRepository.GetAll()
             .Where(x => x.Id == dto.Id)
             .FirstOrDefaultAsync(ct);
        var result = _courseValidator.ValidateOnNull(course);
        if (!result.IsSuccess)
        {
            return DataResult<CourseDto>.Failure((int)ErrorCodes.CourseNotFound, ErrorMessage.CourseNotFound);
        }

        course.Title = dto.Title;
        course.Description = dto.Description;
        course.Price = dto.Price;

        var updatedCourse = _courseRepository.Update(course);
        await _courseRepository.SaveChangesAsync();

        return DataResult<CourseDto>.Success(_mapper.Map<CourseDto>(updatedCourse));
    }
}
