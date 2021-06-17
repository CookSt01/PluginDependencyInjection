using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace SamplePluginLibrary
{
    public class SamplePluginModule : Autofac.Module
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Load(ContainerBuilder builder)
        {
            // If no dependencies are registered then assembly unloads fine
            // If registrations are created that reference the assembly, the assembly won't unload

            //builder.RegisterAssemblyTypes(Assembly.GetAssembly(GetType())).AsImplementedInterfaces();
            builder.RegisterType<PluginDependency>().As<IPluginDependency>();
        }
    }
}
