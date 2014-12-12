namespace NLogCassandra

open Cassandra
open NLog
open NLog.Config
open NLog.Targets
open System
open System.Collections.Generic

[<Target("Cassandra")>]
type CassandraTarget(nodes:string[], keyspace:string, replication:int, columnFamily:string) = 
    inherit TargetWithLayout()

    let mutable _initialized = false
    let mutable _keyspace = keyspace
    let mutable _replication = replication
    let mutable _columnFamily = columnFamily
    let mutable _nodes = nodes

    let cluster = new Lazy<Cluster>(fun _ -> Cluster.Builder().AddContactPoints(_nodes).Build())
    let session = new Lazy<ISession>(fun _ -> cluster.Value.Connect())
    let statement = new Lazy<PreparedStatement>(fun _ -> session.Value.Prepare(Queries.Insert(_keyspace, _columnFamily)))

    let init = fun _ -> 
        let q = Queries.CreateKeyspace(_keyspace, _replication)
        session.Value.Execute(q) |> ignore
        session.Value.Execute(Queries.CreateTable(_keyspace, _columnFamily)) |> ignore
        _initialized <- true    

    member this.Nodes  with get () = _nodes and set (value) = _nodes <- value

    [<RequiredParameterAttribute>]
    member this.Node 
        with get() = 
            String.Join(",", this.Nodes)
        and set(value:string) = 
            this.Nodes <- value.Split(',') |> Array.map(fun h -> h.Trim())

    [<AdvancedAttribute>]
    member this.Keyspace with get() = _keyspace and set(value) = _keyspace <- value
    [<AdvancedAttribute>]
    member this.Replication with get() = _replication and set(value) = _replication <- value
    [<AdvancedAttribute>]
    member this.ColumnFamily with get() = _columnFamily and set(value) = _columnFamily <- value

    override this.Write(logEvent:LogEventInfo) =
        if (not _initialized) then
            init()

        let message = this.Layout.Render(logEvent)
        let stackTrace = match logEvent.HasStackTrace with
                            | true -> logEvent.StackTrace.ToString()
                            | _ -> ""
        let boundedStatement = statement.Value.Bind(logEvent.SequenceID, logEvent.TimeStamp, logEvent.Level.ToString(), 
                                logEvent.FormattedMessage, stackTrace, logEvent.LoggerName)
        session.Value.Execute(boundedStatement) |> ignore
       
    new() as this = new CassandraTarget(Array.empty<string>, "LogSpace", 1, "LogFamily")
