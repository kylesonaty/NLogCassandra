namespace NLogCassandra

open Cassandra
open NLog
open NLog.Targets
open System.Collections.Generic

type CassandraTarget(nodes:string[], keyspace:string, replication:int, columnFamily:string) = 
    inherit TargetWithLayout()
    let cluster = Cluster.Builder().AddContactPoints(nodes).Build()
    let session = cluster.Connect()
    do 
        let q = Queries.CreateKeyspace(keyspace, replication)
        session.Execute(q) |> ignore
        session.Execute(Queries.CreateTable(keyspace, columnFamily)) |> ignore
   
    let statement = session.Prepare(Queries.Insert(keyspace, columnFamily))

    override this.Write(logEvent:LogEventInfo) =
        let message = this.Layout.Render(logEvent)
        let stackTrace = match logEvent.HasStackTrace with
                            | true -> logEvent.StackTrace.ToString()
                            | _ -> ""
        let boundedStatement = 
            statement.Bind(logEvent.SequenceID, logEvent.TimeStamp, logEvent.Level.ToString(), 
                logEvent.FormattedMessage, stackTrace, logEvent.LoggerName)
        session.Execute(boundedStatement) |> ignore
       
