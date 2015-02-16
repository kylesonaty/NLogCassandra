# NLogCassandra #

NLogCassandra is a NLog target that allows you log your messages to Cassandra using DataStax C# driver. You can get the [nuget package here](https://www.nuget.org/packages/NLogCassandra/).


## How to use ##

Here is an example of how to use in a console application.

```csharp
using System;
using NLog;
using NLog.Config;
using NLogCassandra;

namespace Testing
{
	class Program
    {
    	static void Main(string[] args)
        {
	        var config = new LoggingConfiguration();
            var target = new CassandraTarget(new [] {"server1", "server2"}, "yourkeyspace", 2, "yourcolumnfamily");
            config.AddTarget("cassandra",target);
            var rule = new LoggingRule("*", LogLevel.Debug, target);
			config.LoggingRules.Add(rule);
            LogManager.Configuration = config;
            var logger = LogManager.GetCurrentClassLogger();
            logger.Debug("This is a test of the emergency broadcast system");
        }
    }
}
```

If you are interested in your logs cleaning up after themselves you can use Cassandra's data expiration feature so that the log expires after a given time by specifying a TTL.

```csharp 
var targetWithTtl = new CassandraTarget(new [] {"server1", "server2"}, "yourkeyspace", 2, "yourcolumnfamily", 600);
```

### Configuration File Setup ###

Here is an example of using the configuration file to setup the Cassandra Target.

```xml
<?xml version="1.0" encoding="utf-8" ?>  
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"  
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <extensions>
    <add assembly="NLogCassandra"/>
  </extensions>
  <targets>
    <target name="cassandraTarget" type="Cassandra" node="server1,server2" keyspace="yourkeyspace" columnfamily="yourcolumnfamily" replication="2" ttl="600"/>
  </targets>
  <rules>
    <logger name="*" minlevel="Trace" writeTo="cassandraTarget" />
  </rules>
</nlog>  
```
