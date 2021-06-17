using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class PluginLoader
    {
        public static PluginLoadContext CreatePluginContext(string PluginLocation)
        {
            var loadContext = new PluginLoadContext(PluginLocation);
            return loadContext;
        }

        public static string GetPluginLocation(string AssemblyDirectory, string AssemblyFileName)
        {
            string pluginLocation = AppDomain.CurrentDomain.BaseDirectory
                                    + AssemblyDirectory
                                    + Path.DirectorySeparatorChar
                                    + AssemblyFileName;
            return pluginLocation;
        }

        public static Assembly LoadAssemblyByName(PluginLoadContext PluginContext, string Name)
        {
            var assemblyName = new AssemblyName(Name);
            Assembly pluginAssembly = PluginContext.LoadFromAssemblyName(assemblyName);
            return pluginAssembly;
        }
    }
}
