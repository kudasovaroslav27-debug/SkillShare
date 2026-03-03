using MediatR;
using SkillShare.Domain.Entities;

namespace SkillShare.Application.Commands;

public record UpdateCourseCommand(int Id, string Title, string Description, decimal Price) : IRequest<Course>;
