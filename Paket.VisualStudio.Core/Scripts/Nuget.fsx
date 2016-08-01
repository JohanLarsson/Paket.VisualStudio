#r "System.dll"
#r "../../packages/Paket.Core/lib/net45/Paket.Core.dll"
#r "../../packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"

open System
open Paket

let catalog = Paket.NuGetV3.getCatalog Paket.Constants.DefaultNuGetV3Stream None |> Async.RunSynchronously

//download "http://www.nuget.org/api/v2/Packages"
//download "http://api.nuget.org/v3/index.json"
//download "http://api.nuget.org/v3/catalog0/index.json"
//download "http://api.nuget.org/v3/Json.Net.json"