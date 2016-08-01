#r "System.dll"
#r "../../packages/Paket.Core/lib/net45/Paket.Core.dll"
#r "../../packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"

open System
open Paket
open Paket.PackageSources

let source = PackageSource.NuGetV3Source Constants.DefaultNuGetV3Stream
let packages = NuGetV3.FindPackages(source.Auth, source.Url, "Paket.Core", 100) |> Async.RunSynchronously
