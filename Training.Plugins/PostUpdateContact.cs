using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginsForD365
{
    public class PostUpdateContact : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
                           // Obtain the tracing service
                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                // Obtain the execution context from the service provider.
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the IOrganizationService instance which you will need for
            // web service calls.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            // The InputParameters collection contains all the data passed in the message request.
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                try
                {    // Obtain the target entity from the input parameters.
                    Entity entity = (Entity)context.InputParameters["Target"]; // updated All Values
                    Entity preImage = (Entity)context.PreEntityImages["PreImage"]; // Previous Values
                    Entity postImage = (Entity)context.PostEntityImages["PostImage"]; // Post Values

                    Entity updateContact = new Entity("contact");
                    updateContact.Id = entity.Id;
                    updateContact["cra27_previousjobtitle"] = preImage.Attributes["jobtitle"];
                    updateContact["cra27_newjobtitle"] = postImage.Attributes["jobtitle"];
                    service.Update(updateContact);
                   
                }
                catch(InvalidPluginExecutionException ex) 
                {
                    throw new InvalidPluginExecutionException(ex.Message.ToString());
                }
              
            }
        }
    }
}
