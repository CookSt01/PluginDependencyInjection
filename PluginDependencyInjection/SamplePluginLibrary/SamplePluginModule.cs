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
            //builder.RegisterAssemblyTypes(Assembly.GetAssembly(GetType()));
            builder.RegisterType<PluginDependency>().As<IPluginDependency>();
        }
    }
}
