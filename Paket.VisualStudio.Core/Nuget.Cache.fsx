#r "System.dll"
#r "./packages/paket"
open System
open System.IO

let cacheDirectory =
    let dir = System.Environment.GetFolderPath System.Environment.SpecialFolder.LocalApplicationData
    System.IO.Path.Combine(dir, "NuGet", "Cache")

let files = Directory.EnumerateFiles(cacheDirectory, "*.nupkg") |> Array.ofSeq

Paket
