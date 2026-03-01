using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SkillShare.Domain.Entities;

namespace SkillShare.Application.Commands;

public record UpdateCourseCommand(int Id, string Title, string Description, decimal Price) : IRequest<Course>;
