open System
open Argu
open RunColorPrint // 引用 RunColorPrint 模組
open RunPrint // 引用 RunPrint 模組
open RedisHelper // 引用 RedisHelper 模組

// 定义命令行错误类型
type CliError =
    | SomeError of string // 定义为带有额外信息的错误
    | ArgumentsNotSpecified // 定义为不带额外信息的错误
    | OtherError

// 定义命令行参数类型
type CmdArgs =
    | [<AltCommandLine("-p")>] Print of message: string
    | [<AltCommandLine("-a")>] Add of message: string
    | [<AltCommandLine("-r")>] ReadRedis of key: string

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Print _ -> "Print a message"
            | Add _ -> "新增一個命令。"
            | ReadRedis _ -> "讀取 Redis 中指定的 key。"

// 获取退出代码，根据结果返回相应的整数值
let getExitCode result =
    match result with
    | Ok() -> 0
    | Error err ->
        match err with
        | ArgumentsNotSpecified -> 1
        | _ -> 2 // 对于其他未指定的错误，返回一个默认的退出代码，例如2

[<EntryPoint>]
// 程序的入口点，处理命令行参数并执行相应的功能
let main argv =
    let errorHandler =
        ProcessExiter(
            colorizer =
                function
                | ErrorCode.HelpText -> None
                | _ -> Some ConsoleColor.Red
        )

    let parser =
        ArgumentParser.Create<CmdArgs>(programName = "fsharp-cli", errorHandler = errorHandler)

    let exitCode =
        match parser.ParseCommandLine argv with
        | p when p.Contains(Print) ->
            (RunPrint (p.GetResult(Print)) |> ignore)
            0 // 返回合适的退出代码（表示成功）
        | p when p.Contains(Add) ->
            (RunColorPrint ConsoleColor.Red (p.GetResult(Add)) |> ignore)
            0 // 返回合适的退出代码（表示成功）
        | p when p.Contains(ReadRedis) ->
            let key = p.GetResult(ReadRedis)
            let connectionString = "127.0.0.1:6379" // 替換為你的 Redis 連接字串
            match readRedisKey connectionString key with
            | Some value -> printfn "Redis key [ %s ] 的值是: %s" key value
            | None -> printfn "Redis key [ %s ] 不存在或沒有值" key
            0 // 返回合适的退出代码（表示成功）
        | p ->
            // 处理其他未被捕获的情况
            printfn "未识别的参数: %A" p
            1 // 返回错误的退出代码

    let getExitCode code =
        if code = 0 then
            Ok() // 或者其他适当的处理
        else
            Error(CliError.SomeError("出错信息")) // 提供具体的错误信息

    printfn "按任何鍵繼續..."
    Console.ReadKey() |> ignore // 等待用户按下任何键
    exitCode // 返回整型值以结束程序
