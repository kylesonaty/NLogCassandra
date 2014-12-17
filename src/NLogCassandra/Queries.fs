namespace NLogCassandra

open NLog
open System 

module Queries =
    let CreateTable(keyspace, columnFamily) =
        String.Format("CREATE TABLE IF NOT EXISTS \"{0}\".\"{1}\" 
            (Logger text,
            Id timeuuid,
            SequenceId int,
            Timestamp timestamp,
            Level text,
            Message text,
            StackTrace text,
            PRIMARY KEY (Logger, Id))
            WITH CLUSTERING ORDER BY (Id ASC);", keyspace, columnFamily)

    let Insert(keyspace, columnFamily) = 
        String.Format("INSERT INTO \"{0}\".\"{1}\" (Logger, Id, SequenceId, Timestamp, Level, Message, StackTrace)
        VALUES (?,now(),?,?,?,?,?);", keyspace, columnFamily)