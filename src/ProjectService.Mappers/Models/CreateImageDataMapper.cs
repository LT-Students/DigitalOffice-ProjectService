using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
{
    public class CreateImageDataMapper : ICreateImageDataMapper
    {
        private IHttpContextAccessor _httpContextAccessor;

        public CreateImageDataMapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public CreateImageData Map(CreateProjectImageRequest request)
        {
            return new CreateImageData(
                request.Name,
                request.Content,
                request.Extension,
                _httpContextAccessor.HttpContext.GetUserId());
        }
    }
}
