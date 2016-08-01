#r "System.dll"

open System
open System.Net

let download (url:string) =
    printfn "%s" url;
    try
      use client = new WebClient()
      let proxy = WebRequest.DefaultWebProxy
      proxy.Credentials <- CredentialCache.DefaultCredentials
      client.Proxy <- proxy
      let result = client.DownloadString url
      printf "%s" result
    with
    | ex -> printfn "Exception! %s " (ex.Message)

//download "http://www.nuget.org/api/v2/Packages"
//download "http://api.nuget.org/v3/index.json"
//download "http://api.nuget.org/v3/catalog0/index.json"
download "http://api.nuget.org/v3/Json.Net.json"