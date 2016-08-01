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

//fsi.PrintLength <- 1000
let getAllResharperPackages () = 
    let getPackageNames (doc : XDocument) = 
        doc.Root.Elements()
        |> Seq.filter (fun e -> e.Name.LocalName = "entry")
        |> Seq.map (fun e -> e.Elements() |> Seq.find (fun e -> e.Name.LocalName = "title"))
        |> Seq.map (fun e -> e.Value)
        |> Seq.distinct
        |> List.ofSeq
    
    let tryGetAttribute (e : XElement) name = 
        if e.HasAttributes then e.Attributes() |> Seq.tryFind (fun a -> a.Name.LocalName = name)
        else None
    
    let tryGetNext (e : XElement) = 
        if e.HasAttributes then 
            let link = e.Elements() |> Seq.tryFindBack (fun e -> e.Name.LocalName = "link")
            match link with
            | Some e -> 
                let next = (tryGetAttribute e "next", tryGetAttribute e "href")
                match next with
                | (Some _, Some a) -> Some a.Value
                | _, _ -> None
            | None -> None
        else None
    
    let rec getAllPackages (url : string) packages = 
        let doc = XDocument.Load url
        let packages = packages |> List.append (getPackageNames doc)
        let next = tryGetNext doc.Root
        match next with
        | Some url -> List.append packages (getAllPackages url packages)
        | None -> packages
    
    let isNuget package = 
        let source = PackageSource.NuGetV3Source Constants.DefaultNuGetV3Stream
        let packages = NuGetV3.FindPackages(source.Auth, source.Url, package, 1) |> Async.RunSynchronously
        packages.Length > 0
    
    let urls = 
        [ "https://resharper-plugins.jetbrains.com/api/v2/curated-feeds/Wave_v1.0/Packages"; 
          "https://resharper-plugins.jetbrains.com/api/v2/curated-feeds/Wave_v2.0/Packages"; 
          "https://resharper-plugins.jetbrains.com/api/v2/curated-feeds/Wave_v3.0/Packages"; 
          "https://resharper-plugins.jetbrains.com/api/v2/curated-feeds/Wave_v4.0/Packages"; 
          "https://resharper-plugins.jetbrains.com/api/v2/curated-feeds/Wave_v5.0/Packages"; 
          "https://resharper-plugins.jetbrains.com/api/v2/curated-feeds/Wave_v6.0/Packages" ]
    urls
    |> Seq.map (fun url -> getAllPackages url [])
    |> Seq.collect id
    |> Seq.distinct
    |> Seq.filter (fun x -> not (isNuget x))
    |> Seq.sort

let packages = getAllResharperPackages()

for package in packages do
    printfn "\"%s\"" package

System.IO.File.WriteAllLines(@"C:\Temp\paket.ignore", packages)
