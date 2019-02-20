using System;
using Microsoft.Xrm.Sdk;

namespace AvanadeSD.Plugin
{
    public class Account : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            #region stuff
            // Logic goes here.
            // Obtain the execution context service from the service provider.
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the tracing service from the service provider.
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the Organization Service factory service from the service provider
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Use the factory to generate the Organization Service.
            var crmService = factory.CreateOrganizationService(context.UserId);
            #endregion

          
            // Ensure there's a Target and that it's an Entity
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                try
                {
                    var entity = context.InputParameters["Target"] as Entity;

                    var followup = new Entity("task");
                    followup["subject"] = "Send e-mail to the new customer.";
                    followup["description"] = "Follow up with the customer.";
                    followup["scheduledstart"] = DateTime.Now.AddDays(1);
                    followup["scheduledend"] = DateTime.Now.AddDays(7);
                    followup["category"] = entity.LogicalName;

                    // Attach the Task to the Account
                    followup["regardingobjectid"] = entity.ToEntityReference();

                    // Create the Task
                    crmService.Create(followup);

                }
                catch (Exception ex)
                {
                    tracingService.Trace(ex.Message);
                }
            }
        }
    }
}
