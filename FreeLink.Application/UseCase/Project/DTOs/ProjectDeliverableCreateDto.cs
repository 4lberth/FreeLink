namespace FreeLink.Application.UseCase.Project.DTOs
{
    public sealed class ProjectDeliverableCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateOnly? DueDate { get; set; }  // null permitido
    }
}