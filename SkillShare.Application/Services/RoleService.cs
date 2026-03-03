using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SkillShare.Application.Resources;
using SkillShare.Domain.Dto.Role;
using SkillShare.Domain.Dto.UserRole;
using SkillShare.Domain.Entities;
using SkillShare.Domain.Enum;
using SkillShare.Domain.Interfaces.Databases;
using SkillShare.Domain.Interfaces.Repositories;
using SkillShare.Domain.Interfaces.Services;
using SkillShare.Domain.Result;

namespace SkillShare.Application.Services;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateRoleDto> _createValidator;
    private readonly IValidator<UpdateRoleDto> _updateValidator;
    private readonly IMapper _mapper;

    public RoleService(IValidator<CreateRoleDto> createValidator,
        IValidator<UpdateRoleDto> updateValidator, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }


    public async Task<DataResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto, CancellationToken ct = default)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return DataResult<RoleDto>.Failure((int)ErrorCodes.ValidError, ErrorMessage.ValidError);
        }

        var existingRole = await _unitOfWork.Roles.GetAll()
            .FirstOrDefaultAsync(x => x.Name == dto.Name, ct);

        if (existingRole != null)
        {
            return DataResult<RoleDto>.Failure((int)ErrorCodes.RoleAlreadyExists, ErrorMessage.RoleAlreadyExists);
        }

        var role = new Role
        {
            Name = dto.Name
        };

        await _unitOfWork.Roles.CreateAsync(role);

        return DataResult<RoleDto>.Success(_mapper.Map<RoleDto>(role));
    }

    public async Task<DataResult<RoleDto>> UpdateRoleAsync(UpdateRoleDto dto, CancellationToken ct = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return DataResult<RoleDto>.Failure((int)ErrorCodes.ValidError, ErrorMessage.ValidError);
        }

        var role = await _unitOfWork.Roles.GetAll()
            .FirstOrDefaultAsync(x => x.Id == dto.Id, ct);
        if (role == null)
        {
            return DataResult<RoleDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
        }

        role.Name = dto.Name;

        var updatedRole = _unitOfWork.Roles.Update(role);
        await _unitOfWork.Roles.SaveChangesAsync();

        return DataResult<RoleDto>.Success(_mapper.Map<RoleDto>(updatedRole));
    }


    public async Task<DataResult<RoleDto>> DeleteRoleAsync(int id, CancellationToken ct = default)
    {
        var role = await _unitOfWork.Roles.GetAll()
             .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (role == null)
        {
            return DataResult<RoleDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
        }

        _unitOfWork.Roles.Remove(role);
        await _unitOfWork.Roles.SaveChangesAsync();

        return DataResult<RoleDto>.Success(_mapper.Map<RoleDto>(role));
    }

    public async Task<DataResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto, CancellationToken ct = default)
    {
        var user = await _unitOfWork.Users.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Login == dto.Login, ct);
        if (user == null)
        {
            return DataResult<UserRoleDto>.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }

        var role = await _unitOfWork.Roles.GetAll().FirstOrDefaultAsync(x => x.Name == dto.RoleName, ct);
        if (role == null)
        {
            return DataResult<UserRoleDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
        }

        var userRole = new UserRole()
        {
            RoleId = role.Id,
            UserId = user.Id
        };

        await _unitOfWork.UserRoles.CreateAsync(userRole);

        await _unitOfWork.UserRoles.SaveChangesAsync();

        return DataResult<UserRoleDto>.Success(_mapper.Map<UserRoleDto>(userRole));

    }

    public async Task<DataResult<UserRoleDto>> DeleteRoleForUserAsync(RemoveUserRoleDto dto, CancellationToken ct = default)
    {
        var user = await _unitOfWork.Users.GetAll()
           .Include(x => x.Roles)
           .FirstOrDefaultAsync(x => x.Login == dto.Login, ct);
        if (user == null)
        {
            return DataResult<UserRoleDto>.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }

        var role = user.Roles.FirstOrDefault(x => x.Id == dto.RoleId);
        if (role == null)
        {
            return DataResult<UserRoleDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
        }

        var userRole = await _unitOfWork.UserRoles.GetAll()
            .Where(x => x.RoleId == role.Id)
            .FirstOrDefaultAsync(x => x.UserId == user.Id, ct);
        _unitOfWork.UserRoles.Remove(userRole);
        await _unitOfWork.UserRoles.SaveChangesAsync();

        return DataResult<UserRoleDto>.Success(_mapper.Map<UserRoleDto>(userRole));
    }

    public async Task<DataResult<UserRoleDto>> UpdateRoleForUserAsync(UpdateUserRoleDto dto, CancellationToken ct = default)
    {
        var user = await _unitOfWork.Users.GetAll()
           .Include(x => x.Roles)
           .FirstOrDefaultAsync(x => x.Login == dto.Login, ct);
        if (user == null)
        {
            return DataResult<UserRoleDto>.Failure((int)ErrorCodes.UserNotFound, ErrorMessage.UserNotFound);
        }

        var role = user.Roles.FirstOrDefault(x => x.Id == dto.FromRoleId);
        if (role == null)
        {
            return DataResult<UserRoleDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
        }

        var newRoleForUser = await _unitOfWork.Roles.GetAll().FirstOrDefaultAsync(x => x.Id == dto.ToRoleId);
        if (newRoleForUser == null)
        {
            return DataResult<UserRoleDto>.Failure((int)ErrorCodes.RoleNotFound, ErrorMessage.RoleNotFound);
        }

        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                var userRole = await _unitOfWork.UserRoles.GetAll()
                .Where(x => x.RoleId == role.Id)
                .FirstAsync(x => x.UserId == user.Id, ct);

                _unitOfWork.UserRoles.Remove(userRole);
                await _unitOfWork.SaveChangesAsync();

                var newUserRole = new UserRole()
                {
                    UserId = user.Id,
                    RoleId = newRoleForUser.Id
                };

                await _unitOfWork.UserRoles.CreateAsync(newUserRole);

                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
            }
        }
        return DataResult<UserRoleDto>.Success(_mapper.Map<UserRoleDto>(role));
    }
}

