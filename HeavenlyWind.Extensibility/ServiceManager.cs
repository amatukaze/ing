using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Extensibility
{
    public static class ServiceManager
    {
        static Dictionary<string, object> r_Services = new Dictionary<string, object>();

        public static void Register<T>(object service) where T : class
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            r_Services.Add(typeof(T).FullName, service);
        }
        public static void Register(string name, object service)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (name == string.Empty)
                throw new ArgumentException(nameof(name));

            r_Services.Add(name, service);
        }

        public static dynamic GetService(string name)
        {
            object rResult;
            if (!r_Services.TryGetValue(name, out rResult))
                return null;

            return rResult;
        }
        public static T GetService<T>() where T : class => GetServiceCore<T>(typeof(T).FullName);
        public static T GetService<T>(string name) where T : class => GetServiceCore<T>(name);
        static T GetServiceCore<T>(string rpName) where T : class
        {
            object rResult;
            if (!r_Services.TryGetValue(rpName, out rResult))
                return null;

            return (T)rResult;
        }
    }
}
