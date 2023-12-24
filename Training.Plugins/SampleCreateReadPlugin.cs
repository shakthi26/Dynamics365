using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.Plugins
{
    public class SampleCreateReadPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            if(context.PrimaryEntityName == "account")
            {
                Entity accountRecord = service.Retrieve("account", context.PrimaryEntityId, new ColumnSet("name", "telephone1"));
                string accountName = accountRecord.GetAttributeValue<string>("name");
                string accountPhone = accountRecord.GetAttributeValue<string>("telephone1");
                Entity contactRecord = new Entity("contact");
                contactRecord["firstname"] = accountName;
                contactRecord["lastname"] = "Test";
                contactRecord["telephone1"] = accountPhone;
                contactRecord["mobilephone"] = accountPhone;
                contactRecord["familystatuscode"] = new OptionSetValue(1);
                contactRecord["birthdate"] = new DateTime(2000, 06, 26);
                contactRecord["creditlimit"] = new Money(1000);
                contactRecord["parentcustomerid"] = new EntityReference("account", context.PrimaryEntityId);
                Guid contactId = service.Create(contactRecord);
            }

        }

       

    }
}
