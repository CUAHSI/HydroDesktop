# Log4net

[Log4net](http://logging.apache.org/log4net/index.html)sends messages (to multiple destinations) and can be configured without having to restart the web service or application.
If you use log4net, you can turn logging on and off without having to recompile the code. You can also configure your plugin to use a remote logger, so that usage, logs, and possibly, errors  could be submitted automatically.
 
# Programming code sample 
See [http://logging.apache.org/log4net/release/manual/introduction.html](http://logging.apache.org/log4net/release/manual/introduction.html) for more details
## Initial Code 
In one class add the attribute 
{{
Using ... statements

// Load the configuration from the 'wateroneflow.logging.log4net' file
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "wateroneflow.logging.log4net", Watch = true)](assembly_-log4net.Config.XmlConfigurator(ConfigFile-=-_wateroneflow.logging.log4net_,-Watch-=-true))
}}

After the class definition:
{{
  public class Service_1_0 : WebService, IService_1_0
    {
      private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
     // if you want to log queries to a separate "file" or logging stream
      private static readonly ILog queryLog = LogManager.GetLogger("QueryLog");
}
}}

Generally log an error message:
{{  log.Error("Cannot retrieve information from connection " + e.Message + spatialTableAdapter.Connection.DataSource);
}}
or
 {{ log.Fatal("Cannot retrieve information from connection " + e.Message + spatialTableAdapter.Connection.DataSource);
}}

or 
 {{ log.Debug("Location Parameter:" + location.ToString());
}}

or log a query (beware of nulls, so use  C# Conditional Operator ( ?: ) and Null-Coalescing Operator ( ?? ) 
 {{ queryLog.InfoFormat("Location: {0}  variable:{1} begin:{2} end{3}",  location ?? String.Empty, variable?? String.Empty,
beginDate?? string.Empty, endDate?? string.Empty) 
}}
