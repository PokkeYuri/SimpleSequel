using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSequel
{
    internal static class OrmDataHelper
    {
        public static T? GetClass<T>(this string statement)
        {
            bool isConnectionOnInputOpen = SimpleSequelManager.Instance.Connection.State == ConnectionState.Open;
            try
            {
                int propertieCounter = 0;
                T result = Activator.CreateInstance<T>();
                using (var reader = statement.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        PropertyInfo[] properties = typeof(T).GetProperties();
                        foreach (PropertyInfo propertyInfo in properties)
                        {
                            try
                            {
                                var value = reader.GetValue(propertyInfo.Name.ToLower());
                                var type = propertyInfo.PropertyType switch
                                {
                                    _ when propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) => propertyInfo.PropertyType.GenericTypeArguments[0],
                                    _ => propertyInfo.PropertyType
                                };
                                var convertedValue = Convert.ChangeType(value, type);
                                if (convertedValue != null)
                                {
                                    propertyInfo.SetValue(result, convertedValue);
                                    propertieCounter++;
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }

                return propertieCounter > 0 ? result : default;
            }
            catch (Exception ex)
            {
                throw new SimpleSequelException(ex.Message, statement, ex);
            }
            finally
            {
                if (!isConnectionOnInputOpen)
                    SimpleSequelManager.Instance.Connection.Close();
            }
        }
    }
}
