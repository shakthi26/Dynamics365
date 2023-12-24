using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.Plugins
{
    public class SampleUpdatePlugin : IPlugin
    {
        string getContactRecords =
            @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                <entity name='contact'>
                <attribute name='fullname' />
                <attribute name='telephone1' /> 
                <attribute name='parentcustomerid' /> 
                <order attribute='fullname' descending='false' />
                <filter type='and'>
                <condition attribute='parentcustomerid' operator='eq' value='{0}' />
                </filter>
                 </entity>
            </fetch>";
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));  
            if(context.PrimaryEntityName == "account")
            {
                Entity accountRecord = service.Retrieve("account", context.PrimaryEntityId, new ColumnSet("creditlimit"));
                int creditLimit = Convert.ToInt32(accountRecord.GetAttributeValue<Money>("creditlimit").Value);
                Entity accountToUpdate = new Entity("account");
                accountToUpdate.Id = context.PrimaryEntityId;
                if (creditLimit <= 10000)
                {
                    accountToUpdate["paymenttermscode"] = new OptionSetValue(1);

                }
                else if (creditLimit > 10000 && creditLimit <= 20000)
                {
                    accountToUpdate["paymenttermscode"] = new OptionSetValue(2);
                }
                else if (creditLimit > 20000 && creditLimit <= 30000)
                {
                    accountToUpdate["paymenttermscode"] = new OptionSetValue(3);
                }
                else
                {
                    accountToUpdate["paymenttermscode"] = new OptionSetValue(4);
                }

                service.Update(accountToUpdate);

                //QueryExpression qe = new QueryExpression("contact");
                //qe.ColumnSet = new ColumnSet("fullname", "parentcustomerid");
                //qe.Criteria.AddCondition(new ConditionExpression("parentcustomerid", ConditionOperator.Equal, context.PrimaryEntityId));
                //EntityCollection contactRecords = service.RetrieveMultiple(qe);
                string str = "";
                //foreach (Entity e in contactRecords.Entities)
                //{
                //    str = str + e.Attributes["fullname"].ToString() + "," + " ";
                //}

                //throw new InvalidPluginExecutionException(str);

                EntityCollection contactRecordsFetchXML = service.RetrieveMultiple(new FetchExpression(string.Format(getContactRecords, context.PrimaryEntityId)));
                foreach (Entity e in contactRecordsFetchXML.Entities)
                {
                    str = str + e.Attributes["fullname"].ToString() + "," + " ";
                }

                tracingService.Trace(str);
            }
        }
    }
}
