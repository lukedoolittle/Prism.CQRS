using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SimpleCQRS.Framework
{
    public static class TypeExtensions
    {
        public static Type Unbind(this Type instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException();
            }

            return instance.GetTypeInfo().IsGenericType ? 
                    instance.GetGenericTypeDefinition() : 
                    instance;
        }

        public static string GetNonGenericName(this Type instance)
        {
            if (!instance.GetTypeInfo().IsGenericType)
            {
                return instance.Name;
            }

            else
            {
                var argumentCount = instance.GetTypeInfo().GenericTypeParameters.Count();
                if (argumentCount == 0)
                {
                    argumentCount = instance.GetTypeInfo().GenericTypeArguments.Count();
                }
                return instance.Name.Replace($"`{argumentCount}", "");
            }
        }
    }
}
