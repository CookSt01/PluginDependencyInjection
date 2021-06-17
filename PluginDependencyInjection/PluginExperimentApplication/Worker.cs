using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary;

namespace PluginExperimentApplication
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ILifetimeScope _scope;

        public Worker(ILogger<Worker> logger, ILifetimeScope scope)
        {
            _logger = logger;
            _scope = scope;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var task = LoadAndRunWorkerAsync();
            await task;

            WeakReference alcWeakRef = task.Result;
            await WaitForGarbageCollectionAsync(alcWeakRef);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task<WeakReference> LoadAndRunWorkerAsync()
        {
            var assemblyDirectory = "..\\..\\..\\..\\SamplePluginLibrary\\bin\\Debug\\net5.0";
            var assemblyFileName = "SamplePluginLibrary.dll";
            var pluginTypeName = "SamplePluginLibrary.SamplePlugin";
            var pluginModuleTypeName = "SamplePluginLibrary.SamplePluginModule";
            //var cancellationSource = new CancellationTokenSource();

            var pluginLoader = new PluginLoader();
            var pluginLocation = PluginLoader.GetPluginLocation(assemblyDirectory, assemblyFileName);

            // Create ALC
            var pluginContext = PluginLoader.CreatePluginContext(pluginLocation);
            var alcWeakRef = new WeakReference(pluginContext, trackResurrection: true);

            // Load assemblies
            var assembly = PluginLoader.LoadAssemblyByName(pluginContext, Path.GetFileNameWithoutExtension(pluginLocation));

            Type pluginType = assembly.ExportedTypes.FirstOrDefault(t => t.FullName == pluginTypeName);
            Type moduleType = assembly.ExportedTypes.FirstOrDefault(t => t.FullName == pluginModuleTypeName);
            var moduleInstance = ActivatorUtilities.CreateInstance((IServiceProvider)_scope, moduleType) as IModule;

            try
            {
                using (var childScope = _scope.BeginLifetimeScope(builder =>
                {
                    builder.RegisterModule(moduleInstance);
                }))
                {
                    var instance = ActivatorUtilities.CreateInstance((IServiceProvider)childScope, pluginType) as IMyPlugin;

                    _logger.LogInformation($"LOAD: {pluginTypeName}.");

                    var pluginTask = instance.StartAsync();
                    await pluginTask;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: {exception} ", ex);
                throw;
            }
            finally
            {
                // Unload ALC
                pluginContext.Unload();
                await Task.Yield();
            }

            return alcWeakRef;
        }

        public async Task WaitForGarbageCollectionAsync(WeakReference assemblyWeakReference)
        {
            // Currently for debugging assistance in confirming assembly unload. Not for release.
            for (int i = 0; assemblyWeakReference.IsAlive && (i < 20); i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                await Task.Delay(TimeSpan.FromMilliseconds(10));
            }

            if (assemblyWeakReference.IsAlive)
            {
                _logger.LogError($"UNLOAD: failed.");
            }
            else
            {
                _logger.LogInformation($"UNLOADED successfully.");
            }
        }
    }
}
