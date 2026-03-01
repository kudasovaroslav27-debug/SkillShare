using MediatR;
using SkillShare.Domain.Entities;

namespace SkillShare.Application.Commands;
public record CreateCourseCommand(string Title, string Description, decimal Price, int? ParentId, long UserId) : IRequest<Course>;

