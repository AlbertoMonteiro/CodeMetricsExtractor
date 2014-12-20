using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MetricsExtractor
{
    public static class DashedParameterSerializer
    {
        public static T Deserialize<T>(IEnumerable<string> parameters)
        {
            var instance = Activator.CreateInstance<T>();

            var propertyInfos = typeof(T).GetProperties().ToList();

            PropertyInfo currentPropertyInfo = null;
            foreach (var parameter in parameters)
            {
                if (parameter.StartsWith("-"))
                    currentPropertyInfo = propertyInfos.SingleOrDefault(p => p.Name.Equals(parameter.Substring(1), StringComparison.OrdinalIgnoreCase));
                else if (currentPropertyInfo != null)
                {
                    if (currentPropertyInfo.PropertyType.IsArray)
                        currentPropertyInfo.SetValue(instance, parameter.Split(';'));
                    else
                        currentPropertyInfo.SetValue(instance, parameter);
                }
            }

            return instance;
        }
    }
}