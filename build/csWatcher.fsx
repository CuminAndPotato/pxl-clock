open System
open System.IO
open System.Diagnostics
open System.Threading

let watchPath = Path.Combine(__SOURCE_DIRECTORY__, "..", "apps_csharp")

let mutable currentProcess: Process option = None
let processLock = obj()

let startDotnetRunForFile (filePath: string) =
    lock processLock (fun () ->
        // Kill existing process if running
        match currentProcess with
        | Some proc when not proc.HasExited ->
            printfn "Killing existing process..."
            try
                proc.Kill(entireProcessTree = true)
                proc.WaitForExit(2000) |> ignore
            with ex ->
                printfn $"Warning: Error killing process: {ex.Message}"
        | _ -> ()

        // Start new process with specific file
        printfn $"Starting dotnet run for file: {filePath}"
        let startInfo = ProcessStartInfo()
        startInfo.FileName <- "dotnet"
        startInfo.Arguments <- $"run \"{filePath}\""
        startInfo.WorkingDirectory <- watchPath
        startInfo.UseShellExecute <- false
        startInfo.RedirectStandardOutput <- true
        startInfo.RedirectStandardError <- true

        let proc = new Process()
        proc.StartInfo <- startInfo
        
        proc.OutputDataReceived.Add(fun args ->
            if not (isNull args.Data) then
                printfn $"[dotnet] {args.Data}")
        
        proc.ErrorDataReceived.Add(fun args ->
            if not (isNull args.Data) then
                eprintfn $"[dotnet ERROR] {args.Data}")

        proc.Start() |> ignore
        proc.BeginOutputReadLine()
        proc.BeginErrorReadLine()
        
        currentProcess <- Some proc
        printfn "Process started."
    )

let mutable lastChangeTime = DateTime.MinValue
let mutable lastChangedFile = ""
let debounceMs = 500.0

let onChanged (e: FileSystemEventArgs) =
    // Debounce rapid file changes
    let now = DateTime.Now
    if (now - lastChangeTime).TotalMilliseconds < debounceMs && lastChangedFile = e.FullPath then
        ()
    else
        lastChangeTime <- now
        lastChangedFile <- e.FullPath
        
        let green = "\u001b[32m"
        let reset = "\u001b[0m"
        
        printfn $"{green}File changed: {e.FullPath}{reset}"
        printfn $"{green}Restarting...{reset}"
        
        startDotnetRunForFile(e.FullPath)

do
    let w = new FileSystemWatcher(watchPath, "*.cs")
    w.IncludeSubdirectories <- true
    w.NotifyFilter <- NotifyFilters.FileName ||| NotifyFilters.LastWrite
    w.Changed.Add(onChanged)
    w.Created.Add(onChanged)
    w.Renamed.Add(onChanged)
    w.EnableRaisingEvents <- true

printfn $"Watching {watchPath} for C# file changes (Ctrl+C to exit)..."
printfn "Waiting for file changes... (No initial run - modify a .cs file to start)"

// Wait for Ctrl+C
let exitEvent = new ManualResetEvent(false)
Console.CancelKeyPress.Add(fun args ->
    printfn "Ctrl+C pressed. Shutting down..."
    args.Cancel <- true
    
    // Kill the dotnet process
    lock processLock (fun () ->
        match currentProcess with
        | Some proc when not proc.HasExited ->
            try
                proc.Kill(entireProcessTree = true)
                proc.WaitForExit(2000) |> ignore
            with _ -> ()
        | _ -> ()
    )
    
    exitEvent.Set() |> ignore)

exitEvent.WaitOne() |> ignore

printfn "C# watcher closed."
