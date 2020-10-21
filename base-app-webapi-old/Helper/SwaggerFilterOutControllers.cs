using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Globalization;
using System.Linq;

namespace base_app_webapi.Helper
{
    internal class SwaggerFilterOutControllers : IDocumentFilter
    {
        CultureInfo enCulture;
        SwaggerRestrictions swaggerRestrictions;
        public SwaggerFilterOutControllers(SwaggerRestrictions swaggerRestrictions)
        {
            enCulture = new CultureInfo("en-US");
            this.swaggerRestrictions = swaggerRestrictions;
        }
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            string[] allowedEndpoints = new string[0];
            if (!string.IsNullOrEmpty(swaggerRestrictions.AllowedEndpoints))
                allowedEndpoints = swaggerRestrictions.AllowedEndpoints.Split(',', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < allowedEndpoints.Length; i++)
                allowedEndpoints[i] = allowedEndpoints[i].ToLower(enCulture);

            string[] excludedEndpoints = new string[0];
            if (!string.IsNullOrEmpty(swaggerRestrictions.ExcludedEndpoints))
                excludedEndpoints = swaggerRestrictions.ExcludedEndpoints.Split(',', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < excludedEndpoints.Length; i++)
                excludedEndpoints[i] = excludedEndpoints[i].ToLower(enCulture);

            bool status;
            string relatedPath = "", schemaName="";

            foreach (var item in swaggerDoc.Paths.ToList())
            {
                relatedPath = item.Key.ToLower(); //.Substring(item.Key.ToLower().IndexOf("/api/") + 4);

                status = CheckEndpoint(allowedEndpoints, excludedEndpoints, relatedPath);
                if (status)
                    continue;
                else
                    swaggerDoc.Paths.Remove(item.Key);
            }

            string[] allowedSchemas = new string[0];
            if (!string.IsNullOrEmpty(swaggerRestrictions.AllowedSchemas))
                allowedSchemas = swaggerRestrictions.AllowedSchemas.Split(',', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < allowedSchemas.Length; i++)
                allowedSchemas[i] = allowedSchemas[i].ToLower(enCulture);

            string[] excludedSchemas = new string[0];
            if (!string.IsNullOrEmpty(swaggerRestrictions.ExcludedSchemas))
                excludedSchemas = swaggerRestrictions.ExcludedSchemas.Split(',', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < excludedSchemas.Length; i++)
                excludedSchemas[i] = excludedSchemas[i].ToLower(enCulture);

            foreach (var schema in swaggerDoc.Components.Schemas.ToList())
            {
                schemaName = schema.Key.ToLower(enCulture);

                status = CheckSchema(allowedSchemas, excludedSchemas, schemaName);

                if (status)
                    continue;
                else
                    swaggerDoc.Components.Schemas.Remove(schema);
            }

            //swaggerDoc.definitions.Remove("Model1");
            //swaggerDoc.definitions.Remove("Model2");
        }

        private bool CheckEndpoint(string[] allowedEndpoints, string[] excludedEndpoints, string relatedPath)
        {
            bool result=false;
            if (allowedEndpoints.Length == 0 && excludedEndpoints.Length == 0)
            {
                result = true;
            }
            else if (allowedEndpoints.Length > 0 && excludedEndpoints.Length == 0)
            {
                if (allowedEndpoints[0] == "*")
                    result = true;
                else
                {
                    bool includeStatus = false;
                    for (int i = 0; i < allowedEndpoints.Length; i++)
                    {
                        if (relatedPath.Contains(allowedEndpoints[i]))
                        {
                            includeStatus = true;
                            break;
                        }
                    }
                    if (includeStatus)
                        result = true; 
                    else
                        result = false;
                }
            }
            else if (allowedEndpoints.Length == 0 && excludedEndpoints.Length > 0)
            {
                if (excludedEndpoints[0] == "*")
                    result = false;
                else
                {
                    bool excludeStatus = false;
                    for (int i = 0; i < excludedEndpoints.Length; i++)
                    {
                        if (relatedPath.Contains(excludedEndpoints[i]))
                        {
                            excludeStatus = true;
                            break;
                        }
                    }
                    if (excludeStatus)
                        result = false;
                    else
                        result = true;
                }
            }
            else if (allowedEndpoints.Length > 0 && excludedEndpoints.Length > 0)
            {
                if (allowedEndpoints[0] == "*" && excludedEndpoints[0] == "*")
                {
                    result = true;
                }
                else if (allowedEndpoints[0] == "*" && excludedEndpoints[0] != "*")
                {
                    bool excludeStatus = false;
                    for (int i = 0; i < excludedEndpoints.Length; i++)
                    {
                        if (relatedPath.Contains(excludedEndpoints[i]))
                        {
                            excludeStatus = true;
                            break;
                        }
                    }
                    if (excludeStatus)
                        result = false;
                    else
                        result = true;
                }
                else if (allowedEndpoints[0] != "*" && excludedEndpoints[0] == "*")
                {
                    bool includeStatus = false;
                    for (int i = 0; i < allowedEndpoints.Length; i++)
                    {
                        if (relatedPath.Contains(allowedEndpoints[i]))
                        {
                            includeStatus = true;
                            break;
                        }
                    }
                    if (includeStatus)
                        result = true;
                    else
                        result = false;
                }
                else if (allowedEndpoints[0] != "*" && excludedEndpoints[0] != "*")
                {
                    bool includeStatus = false;
                    for (int i = 0; i < allowedEndpoints.Length; i++)
                    {
                        if (relatedPath.Contains(allowedEndpoints[i]))
                        {
                            includeStatus = true;
                            break;
                        }
                    }
                    if (includeStatus)
                        result = true; 

                    bool excludeStatus = false;
                    for (int i = 0; i < excludedEndpoints.Length; i++)
                    {
                        if (relatedPath.Contains(excludedEndpoints[i]))
                        {
                            excludeStatus = true;
                            break;
                        }
                    }
                    if (excludeStatus)
                        result = false;

                    result = true;
                }
            }

            return result;
        }

        private bool CheckSchema(string[] allowedSchemas, string[] excludedSchemas, string schemaName)
        {
            bool result = false;
            if (allowedSchemas.Length == 0 && excludedSchemas.Length == 0)
            {
                result = true;
            }
            else if (allowedSchemas.Length > 0 && excludedSchemas.Length == 0)
            {
                if (allowedSchemas[0] == "*")
                    result = true;
                else
                {
                    result = allowedSchemas.Contains(schemaName);
                }
            }
            else if (allowedSchemas.Length == 0 && excludedSchemas.Length > 0)
            {
                if (excludedSchemas[0] == "*")
                    result = false;
                else
                {
                    result = !excludedSchemas.Contains(schemaName);
                }
            }
            else if (allowedSchemas.Length > 0 && excludedSchemas.Length > 0)
            {
                if (allowedSchemas[0] == "*" && excludedSchemas[0] == "*")
                {
                    result = true;
                }
                else if (allowedSchemas[0] == "*" && excludedSchemas[0] != "*")
                {
                    result = !excludedSchemas.Contains(schemaName);
                }
                else if (allowedSchemas[0] != "*" && excludedSchemas[0] == "*")
                {
                    result = allowedSchemas.Contains(schemaName);
                }
                else if (allowedSchemas[0] != "*" && excludedSchemas[0] != "*")
                {
                    result = allowedSchemas.Contains(schemaName);
                    if(!result)
                    {
                        result = !excludedSchemas.Contains(schemaName);
                    }
                }
            }

            return result;
        }
    }
}
