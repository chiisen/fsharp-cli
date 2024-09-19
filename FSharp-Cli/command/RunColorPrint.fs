module RunColorPrint

open System

// 定義 runColorPrint 函式
let RunColorPrint color print =
    let originalColor = Console.ForegroundColor
    Console.ForegroundColor <- color
    printfn "%s" print
    Console.ForegroundColor <- originalColor
    Ok()