using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SkillShare.Application.Resources;
using SkillShare.Domain.Dto.UserCourseGrade;
using SkillShare.Domain.Enum;
using SkillShare.Domain.Interfaces.Databases;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;

namespace SkillShare.Application.Services;

public class UserCourseGradeService : IUserCourseGradeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserCourseGradeService(
        IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<DataResult<UserCourseGradeDto>> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var grade = await _unitOfWork.UserCourseGrades.GetAll()
            .AsNoTracking()
            .Where(x => x.Id == id)
            .ProjectToType<UserCourseGradeDto>()
            .FirstOrDefaultAsync(ct);

        if (grade == null)
        {
            return DataResult<UserCourseGradeDto>.Failure((int)ErrorCodes.GradeNotFound, ErrorMessage.GradeNotFound);
        }

        return DataResult<UserCourseGradeDto>.Success(grade);
    }

    public async Task<CollectionResult<UserCourseGradeDto>> GetByUserIdAsync(long userId, CancellationToken ct = default)
    {
        var userExists = await _unitOfWork.Users.ExistsAsync(x => x.Id == userId, ct);
        if (!userExists)
        {
            return CollectionResult<UserCourseGradeDto>.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }

        var grades = await _unitOfWork.UserCourseGrades.GetAll()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ProjectToType<UserCourseGradeDto>()
            .ToListAsync(ct);

        if (!grades.Any())
        {
            return CollectionResult<UserCourseGradeDto>.Failure((int)ErrorCodes.GradeNotFound, ErrorMessage.GradeNotFound);
        }

        return CollectionResult<UserCourseGradeDto>.Success(grades);
    }

    public async Task<DataResult<UserCourseGradeDto>> DeleteAsync(long id, CancellationToken ct = default)
    {
        var grade = await _unitOfWork.UserCourseGrades.GetAll()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (grade == null)
        {
            return DataResult<UserCourseGradeDto>.Failure((int)ErrorCodes.GradeNotFound, ErrorMessage.GradeNotFound);
        }

        _unitOfWork.UserCourseGrades.Remove(grade);
        await _unitOfWork.UserCourseGrades.SaveChangesAsync();

        return DataResult<UserCourseGradeDto>.Success(_mapper.Map<UserCourseGradeDto>(grade));
    }
}
