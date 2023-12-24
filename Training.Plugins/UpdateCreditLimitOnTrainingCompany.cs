using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Training.Plugins
{
    public class UpdateCreditLimitOnTrainingCompany : IPlugin
    {
        //string fetchXML =
        //    @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
        //            <entity name = 'ita_creditlimit'>
        //            <attribute name='ita_creditlimitid' />
        //            <attribute name = 'ita_creditrating' />                    
        //            <order attribute = 'ita_name' descending='false' />
        //            </entity>
        //      </fetch>";
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                if (entity.LogicalName == "ita_trainingcompany")
                {
                    Entity trainingCompanyRecord = service.Retrieve("ita_trainingcompany", entity.Id, new ColumnSet("ita_creditrating", "ita_creditlimit"));
                    var creditRating = trainingCompanyRecord.GetAttributeValue<OptionSetValue>("ita_creditrating").Value;
                    Entity recordToUpdate = new Entity("ita_trainingcompany");
                    recordToUpdate.Id = trainingCompanyRecord.Id;
                    //EntityCollection creditLimitFectchXML = service.RetrieveMultiple(new FetchExpression(string.Format(fetchXML)));
                    //foreach(Entity e in creditLimitFectchXML.Entities)
                    //{
                    //    var cr = e.GetAttributeValue<OptionSetValue>("ita_creditrating").Value;
                    //    if(creditRating == cr)
                    //    {
                    //        recordToUpdate["ita_creditlimt"] = e.Attributes["ita_creditlimit"];
                    //    }
                    //}
                    if(creditRating == 455000000)
                        recordToUpdate["ita_creditlimit"] = new Money(50000);
                    else if(creditRating == 455000001)
                        recordToUpdate["ita_creditlimit"] = new Money(75000);
                    else if(creditRating == 455000002)
                        recordToUpdate["ita_creditlimit"] = new Money(100000);
                    
                    service.Update(recordToUpdate);
                }
            }
        }
    }
}
