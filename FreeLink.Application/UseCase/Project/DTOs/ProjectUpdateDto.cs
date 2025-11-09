using System;

namespace FreeLink.Application.UseCase.Project.DTOs
{
    public class ProjectUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Budget { get; set; }
        public DateOnly? DeadlineDate { get; set; }
        public string? RequiredSkills { get; set; }
    }
}