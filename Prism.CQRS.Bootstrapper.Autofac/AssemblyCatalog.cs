using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SimpleCQRS.Framework
{
    public class AssemblyCatalog : IEnumerable<Assembly>
    {
        private readonly List<Assembly> _assemblies;

        public AssemblyCatalog()
        {
            _assemblies = new List<Assembly>();
        }

        public AssemblyCatalog Add(Assembly assembly)
        {
            _assemblies.Add(assembly);
            return this;
        }

        public IEnumerator<Assembly> GetEnumerator()
        {
            return _assemblies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
