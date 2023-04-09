using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Transactions;

namespace WebApi
{
    public class SwaggerHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            if (context.MethodInfo.GetCustomAttribute(typeof(JMAuth)) is JMAuth attribute)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "jm-token",
                    In = ParameterLocation.Header,
                    Description = "JWT",
                    Required = true,
                    AllowEmptyValue = true
                });
            }
        }
    }
}
