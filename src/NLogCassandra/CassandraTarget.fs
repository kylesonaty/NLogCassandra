namespace NLogCassandra

open Cassandra
open NLog
open NLog.Config
open NLog.Targets
open System
open System.Collections.Generic

[<Target("Cassandra")>]
type CassandraTarget(nodes:string[], keyspace:string, replication:int, columnFamily:string, ?ttl:int) = 
    inherit TargetWithLayout()

    let mutable _initialized = false
    let mutable _keyspace = keyspace
    let mutable _replication = replication
    let mutable _columnFamily = columnFamily
    let mutable _nodes = nodes
    let mutable _ttl = defaultArg ttl 0

    let rep = dict [ ("class", "SimpleStrategy"); ("replication_factor", _replication.ToString()) ]
    
    let cluster = new Lazy<Cluster>(fun _ -> 
        let nds = _nodes |> Array.map(fun n -> n.Trim())
        Cluster.Builder().WithDefaultKeyspace(_keyspace).AddContactPoints(nds).Build())

    let session = new Lazy<ISession>(fun _ -> cluster.Value.ConnectAndCreateDefaultKeyspaceIfNotExists(new Dictionary<string, string>(rep)))
    let statement = new Lazy<PreparedStatement>(fun _ -> session.Value.Prepare(Queries.Insert(_keyspace, _columnFamily, _ttl)))

    let init = fun _ -> 
        let createColumnFamilyStatement = (new SimpleStatement(Queries.CreateTable(_keyspace, _columnFamily))).SetConsistencyLevel(new Nullable<ConsistencyLevel>(ConsistencyLevel.All))
        let createResult = session.Value.Execute(createColumnFamilyStatement)
        cluster.Value.RefreshSchema() |> ignore
        _initialized <- true

    member this.Nodes  with get () = _nodes and set (value) = _nodes <- value

    [<RequiredParameterAttribute>]
    member this.Node 
        with get() = 
            String.Join(",", this.Nodes)
        and set(value:string) = 
            this.Nodes <- value.Split(',') |> Array.map(fun h -> h.Trim())

    [<AdvancedAttribute>]
    member this.Keyspace with get() = _keyspace and set(value:string) = _keyspace <- value.Trim()
    [<AdvancedAttribute>]
    member this.Replication with get() = _replication and set(value:int) = _replication <- value
    [<AdvancedAttribute>]
    member this.ColumnFamily with get() = _columnFamily and set(value:string) = _columnFamily <- value.Trim()
    [<AdvancedAttribute>]
    member this.Ttl with get() = _ttl and set(value:int) = _ttl <- value

    override this.Write(logEvent:LogEventInfo) =
        if (not _initialized) then
            init()

        let message = this.Layout.Render(logEvent)
        let stackTrace = match logEvent.HasStackTrace with
                            | true -> logEvent.StackTrace.ToString()
                            | _ -> ""
        let boundedStatement = statement.Value.Bind(logEvent.LoggerName, logEvent.SequenceID, logEvent.TimeStamp, logEvent.Level.ToString(), 
                                logEvent.FormattedMessage, stackTrace)
        session.Value.Execute(boundedStatement) |> ignore
       
    new() as this = new CassandraTarget(Array.empty<string>, "LogSpace", 1, "LogFamily")
