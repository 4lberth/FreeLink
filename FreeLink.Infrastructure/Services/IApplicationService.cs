using System.Collections.Generic;
using System.Threading.Tasks;
using FreeLink.Application.UseCase.Application.DTOs;

namespace FreeLink.Application.Services
{
    public interface IApplicationService
    {
        Task<ApplicationViewDto> SubmitApplicationAsync(int projectId, ApplicationCreateDto dto);
        Task<IEnumerable<ApplicationViewDto>> GetApplicationsForProjectAsync(int projectId);
        Task<IEnumerable<ApplicationViewDto>> GetApplicationsByFreelancerAsync(int freelancerId);
    }
}