using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.Plugins
{
    public class ValidationPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)
            serviceProvider.GetService(typeof(IPluginExecutionContext));
            Entity entity = (Entity)context.InputParameters["Target"];
            if (entity.Contains("ita_name"))
            {
                string name = (string)entity["ita_name"];
                if ("Test".Equals(name))
                {
                    throw new InvalidPluginExecutionException("Cannot use this name");
                }
            }
        }
    }
}

