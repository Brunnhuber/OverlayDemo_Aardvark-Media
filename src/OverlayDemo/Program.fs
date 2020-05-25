open OverlayDemo

open Aardium
open Aardvark.Service
open Aardvark.UI
open Suave
open Suave.WebPart
open Aardvark.Rendering.Vulkan
open Aardvark.Base
open System


type EmbeddedResources = EmbeddedResources

[<EntryPoint>]
let main args =
    Ag.initialize()
    Aardvark.Init()
    Aardium.init()

    let app = new HeadlessVulkanApplication()

    WebPart.startServer 4321 [
        MutableApp.toWebPart' app.Runtime false (App.start App.app)
        Reflection.assemblyWebPart typeof<EmbeddedResources>.Assembly
    ] |> ignore
    
    Aardium.run {
        title "Aardvark rocks this overlay demo \\o/"
        width 1024
        height 768
        url "http://localhost:4321/"
    }

    0
