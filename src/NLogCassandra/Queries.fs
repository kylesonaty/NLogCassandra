namespace NLogCassandra

open NLog
open System 

module Queries =

    let CreateTable(keyspace, columnFamily) =
        String.Format("CREATE TABLE IF NOT EXISTS \"{0}\".\"{1}\" 
            (Id timeuuid PRIMARY KEY,
            SequenceId int,
            Timestamp timestamp,
            Level text,
            Message text,
            StackTrace text,
            Logger text);", keyspace, columnFamily)

    let Insert(keyspace, columnFamily) = 
        String.Format("INSERT INTO \"{0}\".\"{1}\" (Id, SequenceId, Timestamp, Level, Message, StackTrace, Logger)
        VALUES (now(),?,?,?,?,?,?);", keyspace, columnFamily)