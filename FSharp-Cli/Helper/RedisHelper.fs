module RedisHelper

open StackExchange.Redis
open System

let readRedisKey (connectionString: string) (key: string) =
    let connection = ConnectionMultiplexer.Connect(connectionString)
    let db = connection.GetDatabase()
    let value = db.StringGet(key)
    if value.IsNullOrEmpty then
        None
    else
        Some (value.ToString()) // 使用括号包裹