using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SkillShare.Application.Resources;
using SkillShare.Domain.Dto.Role;
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
    private readonly IBaseRepository<Course> _courseRepository;
    private readonly IMapper _mapper;

    public UserCourseGradeService(
        IBaseRepository<UserCourseGrade> gradeRepository,
        IBaseRepository<User> userRepository,
        IBaseRepository<Course> courseRepository,
        IMapper mapper)
    {
        _gradeRepository = gradeRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<DataResult<UserCourseGradeDto>> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var grade = await _gradeRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        
        if (grade == null)
        {
            return DataResult<UserCourseGradeDto>.Failure((int)ErrorCodes.GradeNotFound, ErrorMessage.GradeNotFound);
        }

        return DataResult<UserCourseGradeDto>.Success(_mapper.Map<UserCourseGradeDto>(grade));
    }

    public async Task<CollectionResult<UserCourseGradeDto>> GetByUserIdAsync(long userId, CancellationToken ct = default)
    {
        var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == userId, ct);
        if (user == null)
        {
            return CollectionResult<UserCourseGradeDto>.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }

        var grades = await _gradeRepository.GetAll()
            .Where(x => x.UserId == userId)
            .ToListAsync(ct);

        if (grades == null || grades.Count == 0)
        {
            return CollectionResult<UserCourseGradeDto>.Failure((int)ErrorCodes.GradeNotFound, ErrorMessage.GradeNotFound);
        }

        return CollectionResult<UserCourseGradeDto>.Success(_mapper.Map<IEnumerable<UserCourseGradeDto>>(grades).ToList());
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