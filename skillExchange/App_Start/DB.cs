using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace skillExchange.App_Start
{
    public class DB
    {
        public static string connectionString =
            ConfigurationManager.ConnectionStrings["SkillExchangeDB"].ConnectionString;
    }
}