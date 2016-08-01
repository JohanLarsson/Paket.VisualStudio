﻿#r "System"
#r "../../packages/Paket.Core/lib/net45/Paket.Core.dll"
open System
open System.IO
open System.Text.RegularExpressions
open Paket

let ignore = Set [|
                "ActiveMesa.R2P.R10"
                "ActiveMesa.R2P.R9"
                "AgentSmith"
                "AgentZorge.R10_0"
                "AgentZorge.R2016_1"
                "AgentZorge.R9_2"
                "Basrach.YouCantSpell"
                "Binbin.Exceptional.2016.2"
                "Bootstrap3.LiveTemplates"
                "Caliburn.Light.Annotations"
                "Catel.ReSharper.100"
                "Catel.ReSharper.90"
                "Catel.ReSharper.91"
                "Catel.ReSharper.92"
                "CitizenMatt.Clippy"
                "CitizenMatt.PreviewTab"
                "CitizenMatt.TemplateDescriptions"
                "CitizenMatt.Xunit"
                "Coconut.ReSharper"
                "CodeAnnotation.Pack"
                "Community.External.Annotations.R90"
                "ConfigureAwaitChecker.v9"
                "Drakmyth.ReSharper.Macros"
                "EggBlox.ReSharper.Plugins"
                "EngineeringTeam.Settings"
                "EtherealCode.Emmet.ReSharper"
                "EtherealCode.ReSpeller"
                "EtherealCode.ReSpellerPro"
                "Exceptional.2016.1"
                "Find.By"
                "JetBrains.XunitTemplates"
                "ReSharper.ImplicitNullability"
                "ReSharper.SerializationInspections"
                "ReSharper.XmlDocInspections"
                "ReshSettings.Discover"
                "campaignmonitor.microservice.templates"
            |]

let splitNameAndVersion (fileName : string) = 
    let m = Regex.Match(fileName, @"(?<name>.+)\.(?<version>\d+(\.\d+){0,3})", RegexOptions.RightToLeft ||| RegexOptions.ExplicitCapture)
    if m.Success then
        let name = Domain.PackageName m.Groups.["name"].Value
        let version = SemVer.Parse m.Groups.["version"].Value
        Some(name, version)
    else None

let files = Directory.EnumerateFiles(Paket.Constants.NuGetCacheFolder, "*.nupkg")
            |> Seq.map Path.GetFileNameWithoutExtension
            |> Seq.choose splitNameAndVersion
            |> Seq.filter (fun (Domain.PackageName(name, _), _) -> not (ignore.Contains(name))) 
            |> Seq.groupBy fst
            |> Seq.map (fun (_, values) -> values |> Seq.maxBy snd )
            |> List.ofSeq