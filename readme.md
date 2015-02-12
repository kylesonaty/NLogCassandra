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
            var targetWithTtl = new CassandraTarget(new [] {"server1", "server2"}, "yourkeyspace", 2, "yourcolumnfamily", 600);
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

