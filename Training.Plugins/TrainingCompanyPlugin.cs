using Microsoft.SqlServer.Server;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.DirectoryServices.ActiveDirectory;

namespace Training.Plugins
{
    public class TrainingCompanyPlugin : IPlugin
    {
        string fetchXML =
                        @"<fetch version = '1.0' output-format = 'xml-platform' mapping = 'logical' distinct = 'false'>
                                <entity name = 'ita_trainingcontact'>
                                <attribute name = 'ita_trainingcontactid' />
                                <attribute name = 'ita_priority' />                        
                                <order attribute = 'ita_name' descending = 'false' />
                                <filter type = 'and' >
                                <condition attribute = 'ita_company' operator= 'eq' value = '{0}' />
                                </filter>
                                </entity>
                         </fetch>";
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory =(IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);           
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                if(entity.LogicalName == "ita_trainingcompany")
                {

                    Entity trainingCompanyRecord = service.Retrieve("ita_trainingcompany", entity.Id, new ColumnSet("ita_priority"));
                    var priority = trainingCompanyRecord.GetAttributeValue<OptionSetValue>("ita_priority").Value;                          
                    EntityCollection updateTrainingContactRecords = service.RetrieveMultiple(new FetchExpression(string.Format(fetchXML, entity.Id)));
                    foreach(Entity e in updateTrainingContactRecords.Entities)
                    {

                        e.Attributes["ita_priority"] = new OptionSetValue(priority);
                        service.Update(e);
                    }
  
                }
            }
        }
    }
}

