using Abp.Application.Services.Dto;

namespace MyProject.Module.Products.Dtos
{

    public class InputProductDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
