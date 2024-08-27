using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PublicHolidaysApp
{
    public class RemoveSchemas : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            foreach (string key in context.SchemaRepository.Schemas.Keys)
            {
                context.SchemaRepository.Schemas.Remove(key);
            }
        }
    }
}
