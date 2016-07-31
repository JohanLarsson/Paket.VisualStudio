#r "System"
#r "../packages/Paket.Core/lib/net45/Paket.Core.dll"
open System
open System.IO
open System.Text.RegularExpressions

let splitNameAndVersion (fileName : string) = 
    let m = Regex.Match(fileName, @"(?<name>.+)\.(?<version>\d+(\.\d+){0,3})", RegexOptions.RightToLeft ||| RegexOptions.ExplicitCapture)
    if m.Success then
        let name = Paket.Domain.PackageName m.Groups.["name"].Value
        let version = Paket.SemVer.Parse m.Groups.["version"].Value
        Some(name, version)
    else None

let files = Directory.EnumerateFiles(Paket.Constants.NuGetCacheFolder, "*.nupkg")
            |> Seq.map Path.GetFileNameWithoutExtension
            |> Seq.choose splitNameAndVersion
            |> Seq.groupBy fst
            |> Seq.map (fun (_, values) -> values |> Seq.maxBy snd )
            |> List.ofSeq
