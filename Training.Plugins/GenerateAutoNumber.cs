using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// hello
namespace Training.Plugins
{
    public class GenerateAutoNumber : IPlugin
    {
        string fetchXML =
            @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                    <entity name='ita_autocountertable'>
                    <attribute name='ita_autocountertableid' />
                    <attribute name='ita_currentnumber' />
                    <attribute name='createdon' />
                    <order attribute='ita_currentnumber' descending='false' />
                    <filter type='and'>
                    <condition attribute='ita_rule' operator='eq' value='AUTONUMBER' />
                    </filter>
                    </entity>
             </fetch>";       
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                        Entity entity = (Entity)context.InputParameters["Target"];                    
                        EntityCollection ecAuto = service.RetrieveMultiple(new FetchExpression(string.Format(fetchXML)));
                        Entity entAuto = ecAuto[0];
                        var autoNumberRecordId = entAuto.Id;

                        Entity counterTable = new Entity("ita_autocountertable");
                        counterTable.Attributes["ita_note"] = "lock " + DateTime.Now;
                        counterTable.Id = autoNumberRecordId;
                        service.Update(counterTable);

                        Entity autoPost = service.Retrieve("ita_autocountertable", autoNumberRecordId, new ColumnSet("ita_currentnumber"));
                        var currentRecordCounterNumber = autoPost.GetAttributeValue<string>("ita_currentnumber");

                        var newCounterValue = Convert.ToInt32(currentRecordCounterNumber) + 1;

                        Entity trainingContactRecord = service.Retrieve("ita_trainingcontact", entity.Id, new ColumnSet("ita_contactid"));
                        Entity trainingRecordToUpdate = new Entity("ita_trainingcontact");
                        trainingRecordToUpdate.Id = trainingContactRecord.Id;
                        trainingRecordToUpdate["ita_contactid"] = newCounterValue.ToString();
                        service.Update(trainingRecordToUpdate);

                        Entity newAutoCounterTable = new Entity("ita_autocountertable");
                        newAutoCounterTable.Id = autoNumberRecordId;
                        newAutoCounterTable["ita_currentnumber"] = newCounterValue.ToString();
                        service.Update(newAutoCounterTable);
                    }                
            }catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
