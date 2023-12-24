using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.Plugins
{
    public class Validation1 : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)
            serviceProvider.GetService(typeof(IPluginExecutionContext));
            Entity entity = (Entity)context.InputParameters["Target"];
            if (!entity.Contains("ita_notestallowedmessage"))
            {
       
               throw new InvalidPluginExecutionException("\"No Test Allowed Message\" field should contain data");
                
            }
        }
    }
}
