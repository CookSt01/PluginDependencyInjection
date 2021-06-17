using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePluginLibrary
{
    public class SamplePlugin : IMyPlugin
    {
        public SamplePlugin(IPluginDependency dependency)
        {
                
        }

        public async Task StartAsync()
        {

        }
    }
}
