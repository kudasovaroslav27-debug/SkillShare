using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Serilog;
using SkillShare.Application.Commands;
using SkillShare.Application.Queries;
using SkillShare.Application.Resources;
using SkillShare.Domain.Dto.CourseDto;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Enum;
using SkillShare.Domain.Extensions;
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
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public CourseService(IBaseRepository<Course> courseRepository,
                         ILogger logger,
                         IMapper mapper,
                         ICourseValidator courseValidator,
                         IBaseRepository<User> userRepository,
                         IMessageProducer messageProducer,
                         IOptions<RabbitMqSettings> rabbitMqOptions,
                         IDistributedCache distributedCache,
                         IMediator mediator)
    {
        _courseValidator = courseValidator;
        _courseRepository = courseRepository;
        _logger = logger;
        _mapper = mapper;
        _userRepository = userRepository;
        _messageProducer = messageProducer;
        _rabbitMqOptions = rabbitMqOptions;
        _distributedCache = distributedCache;
        _mediator = mediator;
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

        var newCourse = await _mediator.Send(new CreateCourseCommand(
            dto.Title,
            dto.Description,
            dto.Price,
            dto.ParentId,
            userId), ct);

        _messageProducer.SendMessage(newCourse, _rabbitMqOptions.Value.RoutingKey, _rabbitMqOptions.Value.ExchangeName);

        return DataResult<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse));
    }

    public async Task<DataResult<CourseDto>> DeleteAsync(int id, CancellationToken ct = default)
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
        var courses = await _mediator.Send(new GetCourseByAuthorIdQuery(AuthorId), ct);

        if (courses == null || !courses.Any())
        {
            return CollectionResult<CourseDto>.Failure((int)ErrorCodes.CourseNotFound, ErrorMessage.CourseNotFound);
        }
        return CollectionResult<CourseDto>.Success(courses.ToList());
    }



    public async Task<DataResult<CourseDto>> GetByIdAsync(int courseId, CancellationToken ct = default)
    {
        var course = await _mediator.Send(new GetCourseQuery(courseId), new CancellationToken());

        if (course == null)
        {
            return DataResult<CourseDto>.Failure((int)ErrorCodes.CourseNotFound, ErrorMessage.CourseNotFound);
        }
        _distributedCache.SetObject($"Сourse_{courseId}", course);

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

        var updatedCourseEntity = await _mediator.Send(new UpdateCourseCommand(
            dto.Id,
            dto.Title,
            dto.Description,
            dto.Price), ct);

        return DataResult<CourseDto>.Success(_mapper.Map<CourseDto>(updatedCourseEntity));
    }
}
