# NLogCassandra #

NLogCassandra is a NLog target that allows you log your messages to Cassandra using DataStax C# driver.


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
           	LogManager.Configuration = config;
            var logger = LogManager.GetCurrentClassLogger();
            logger.Debug("This is a test of the emergency broadcast system");
        }
    }
}
```

## Nuget ##

This is a work in process. For now clone the repository and build and add a reference.  
