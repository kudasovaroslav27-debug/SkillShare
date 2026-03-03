using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SkillShare.Application.Resources;
using SkillShare.Domain.Dto.Role;
using SkillShare.Domain.Dto.StudentAnswer;
using SkillShare.Domain.Dto.UserCourseGrade;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Enum;
using SkillShare.Domain.Interfaces.Repositories;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;

namespace SkillShare.Application.Services;

public class UserCourseGradeService : IUserCourseGradeService
{
    private readonly IBaseRepository<UserCourseGrade> _gradeRepository;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IMapper _mapper;

    public UserCourseGradeService(
        IBaseRepository<UserCourseGrade> gradeRepository,
        IBaseRepository<User> userRepository,
        IMapper mapper)
    {
        _gradeRepository = gradeRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<DataResult<UserCourseGradeDto>> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var grade = await _gradeRepository.GetAll()
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
        var userExists = await _userRepository.ExistsAsync(x => x.Id == userId, ct);
        if (!userExists)
        {
            return CollectionResult<UserCourseGradeDto>.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }

        var grades = await _gradeRepository.GetAll()
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
        var grade = await _gradeRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (grade == null)
        {
            return DataResult<UserCourseGradeDto>.Failure((int)ErrorCodes.GradeNotFound, ErrorMessage.GradeNotFound);
        }

        _gradeRepository.Remove(grade);
        await _gradeRepository.SaveChangesAsync();

        return DataResult<UserCourseGradeDto>.Success(_mapper.Map<UserCourseGradeDto>(grade));
    }
}
