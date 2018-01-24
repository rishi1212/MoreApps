using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace MoreApps
{
    public static class HelperClass
    {
        /// <summary>
        /// get user time zone
        /// </summary>
        /// <param name="service">IOrganizationService object</param>
        /// <param name="userId">initiating user id from context of plugin</param>
        /// <param name="utcnow">universal time zone format date time</param>
        /// <returns>return date time converted in initiating user time zone</returns>
        public static DateTime UserTimeZone(IOrganizationService service, Guid userId, DateTime utcnow)
        {
            ////replace userid with id of user
            Entity userSettings = service.Retrieve("usersettings", userId, new ColumnSet("timezonecode"));
            int timeZoneCode = 85;
            ////retrieving timezonecode from usersetting
            if ((userSettings != null) && (userSettings["timezonecode"] != null))
            {
                timeZoneCode = (int)userSettings["timezonecode"];
            }
            ////retrieving standard name
            var qe = new QueryExpression("timezonedefinition");
            qe.ColumnSet = new ColumnSet("standardname");
            qe.Criteria.AddCondition("timezonecode", ConditionOperator.Equal, timeZoneCode);
            EntityCollection timeZoneDef = service.RetrieveMultiple(qe);
            TimeZoneInfo userTimeZone = null;
            if (timeZoneDef.Entities.Count == 1)
            {
                string timezonename = timeZoneDef.Entities[0]["standardname"].ToString();
                userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezonename);
            }
            ////converting date from UTC to user time zone
            DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(utcnow, userTimeZone);
            return cstTime;
        }
    }
}
