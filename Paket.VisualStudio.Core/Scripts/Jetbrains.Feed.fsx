#r "System"
#r "System.Xml.Linq"
#r "System.dll"
#r "../../packages/Paket.Core/lib/net45/Paket.Core.dll"
#r "../../packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"

open System
open Paket
open Paket.PackageSources
open System
open System.Xml.Linq
fsi.PrintLength <- 1000

let getAllResharperPackages =
    let getPackageNames (doc : XDocument) =
        doc.Root.Elements()
        |> Seq.filter(fun e -> e.Name.LocalName ="entry")
        |> Seq.map (fun e -> e.Elements() |> Seq.find (fun e -> e.Name.LocalName = "title"))
        |> Seq.map (fun e -> e.Value)
        |> Seq.distinct
    let isNuget package = 
        let source = PackageSource.NuGetV3Source Constants.DefaultNuGetV3Stream
        let packages = NuGetV3.FindPackages(source.Auth, source.Url, package, 1) |> Async.RunSynchronously
        packages.Length > 0

    let urls = [
                   "https://resharper-plugins.jetbrains.com/api/v2/curated-feeds/Wave_v1.0/Packages"
                   "https://resharper-plugins.jetbrains.com/api/v2/curated-feeds/Wave_v2.0/Packages"
                   "https://resharper-plugins.jetbrains.com/api/v2/curated-feeds/Wave_v3.0/Packages"
                   "https://resharper-plugins.jetbrains.com/api/v2/curated-feeds/Wave_v4.0/Packages"
                   "https://resharper-plugins.jetbrains.com/api/v2/curated-feeds/Wave_v5.0/Packages"
                   "https://resharper-plugins.jetbrains.com/api/v2/curated-feeds/Wave_v6.0/Packages"
               ]
    urls
    |> Seq.map XDocument.Load
    |> Seq.map getPackageNames
    |> Seq.collect id
    |> Seq.distinct
    |> Seq.filter(fun x -> not (isNuget x))
    |> Seq.sort

let packages = getAllResharperPackages

for package in packages do
    printfn "\"%s\"" package

System.IO.File.WriteAllLines(@"C:\Temp\paket.ignore", packages)

