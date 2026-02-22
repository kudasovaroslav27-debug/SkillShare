using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SkillShare.Application.Resources;
using SkillShare.Domain.Dto;
using SkillShare.Domain.Dto.Role;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Enum;
using SkillShare.Domain.Interfaces.Repositories;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;

namespace SkillShare.Application.Services;

public class RoleService : IRoleService
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<Role> _roleRepository;
    private readonly IBaseRepository<UserRole> _userRoleRepository;
    private readonly IValidator<CreateRoleDto> _createValidator;
    private readonly IValidator<UpdateRoleDto> _updateValidator;
    private readonly IMapper _mapper;

    public RoleService(IBaseRepository<User> userRepository, IBaseRepository<Role> roleRepository, IValidator<CreateRoleDto> createValidator,
        IValidator<UpdateRoleDto> updateValidator, IMapper mapper, IBaseRepository<UserRole> userRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mapper = mapper;
    }


    public async Task<DataResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto, CancellationToken ct = default)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return DataResult<RoleDto>.Failure((int)ErrorCodes.ValidError, ErrorMessage.ValidError);
        }

        var existingRole = await _roleRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Name == dto.Name, ct);

        if (existingRole != null)
        {
            return DataResult<RoleDto>.Failure((int)ErrorCodes.RoleAlreadyExists, ErrorMessage.RoleAlreadyExists);
        }

        var role = new Role
        {
            Name = dto.Name
        };

        await _roleRepository.CreateAsync(role);

        return DataResult<RoleDto>.Success(_mapper.Map<RoleDto>(role));
    }

    public async Task<DataResult<RoleDto>> UpdateRoleAsync(UpdateRoleDto dto, CancellationToken ct = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return DataResult<RoleDto>.Failure((int)ErrorCodes.ValidError, ErrorMessage.ValidError);
        }

        var role = await _roleRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Id == dto.Id, ct);
        if (role == null)
        {
            return DataResult<RoleDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
        }

        role.Name = dto.Name;

        var updatedRole = _roleRepository.Update(role);
        await _roleRepository.SaveChangesAsync();

        return DataResult<RoleDto>.Success(_mapper.Map<RoleDto>(updatedRole));
    }


    public async Task<DataResult<RoleDto>> DeleteRoleAsync(int id, CancellationToken ct = default)
    {
        var role = await _roleRepository.GetAll()
             .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (role == null)
        {
            return DataResult<RoleDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
        }

        _roleRepository.Remove(role);
        await _roleRepository.SaveChangesAsync();

        return DataResult<RoleDto>.Success(_mapper.Map<RoleDto>(role));
    }

    public async Task<DataResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto, CancellationToken ct = default)
    {
        var user = await _userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Login == x.Login, ct);
        if (user == null)
        {
            return DataResult<UserRoleDto>.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }

        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.RoleName, ct);
        if (role == null)
        {
            return DataResult<UserRoleDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
        }

        var userRole = new UserRole()
        {
            RoleId = role.Id,
            UserId = user.Id
        };

        await _userRoleRepository.CreateAsync(userRole);

        return DataResult<UserRoleDto>.Success(_mapper.Map<UserRoleDto>(userRole));

    }
}

