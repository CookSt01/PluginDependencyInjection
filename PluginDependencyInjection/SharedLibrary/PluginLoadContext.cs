using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string readerLocation) : base(true)
        {
            _resolver = new AssemblyDependencyResolver(readerLocation);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            // If assembly is already loaded in the default context, return that assembly. This allows sharing of .net libraries without loading them multiple times.
            var assemblyDll = assemblyName.Name + ".dll";
            var defaultContext = AssemblyLoadContext.Default;
            var matchingAssemblies = defaultContext.Assemblies.Where(a => a.ManifestModule.Name == assemblyDll);

            if (matchingAssemblies.Any())
            {
                return matchingAssemblies.First();
            }

            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }
    }
}
