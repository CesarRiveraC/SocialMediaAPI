using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SocialMedia.Infrastructure.Filters
{
    /*Esta filtro de validación es implementado en caso que no queremos dar uso a las prestaciones del [ApiController],
     o simplemente queremos personalizar el tipo de objeto con el que vamos a responder en la petición,
    en este caso específico, sustituimos modelState por un customObject*/
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
            await next();
        }
    }
}
