using System;

namespace FreeLink.Application.UseCase.Project.DTOs
{
    public class ProjectCreateDto
    {
        public int ClientId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public DateOnly DeadlineDate { get; set; }
        public string? RequiredSkills { get; set; }
    }
}