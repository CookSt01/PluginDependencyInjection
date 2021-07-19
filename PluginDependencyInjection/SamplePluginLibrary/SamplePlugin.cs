using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace SamplePluginLibrary
{
    public class SamplePlugin : IMyPlugin
    {
        public SamplePlugin()
        {
        }

        //public SamplePlugin(IPluginDependency dependency)
        //{
        //}

        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task StartAsync()
        {
            var builder = new Autofac.ContainerBuilder();
            //builder.RegisterModule<SamplePluginModule>();
            builder.RegisterType<SampleWorker>().AsSelf();
            builder.RegisterType<PluginLoader>().AsSelf();

            using var scope = builder.Build();
            var worker = scope.Resolve<SampleWorker>();
            //var worker = new SampleWorker();

            worker.DoWork();

            scope.Dispose();
        }
    }
}
