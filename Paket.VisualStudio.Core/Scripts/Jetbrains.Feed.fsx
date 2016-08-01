#r "System"
#r "System.Xml.Linq"

open System
open System.Xml.Linq

let getAllResharperPackages =
    let getPackageNames (doc : XDocument) =
        doc.Root.Elements()
        |> Seq.filter(fun e -> e.Name.LocalName ="entry")
        |> Seq.map (fun e -> e.Elements() |> Seq.find (fun e -> e.Name.LocalName = "title"))
        |> Seq.map (fun e -> e.Value)
        |> Seq.distinct

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

System.IO.File.WriteAllLines("C:\Temp\paket.exclude", getAllResharperPackages)

