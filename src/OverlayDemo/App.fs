namespace OverlayDemo

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI
open Aardvark.UI.Primitives
open Aardvark.Base.Rendering
open OverlayDemo.Model

type Message =
    | ToggleModel
    | WarningTest
    | ErrorTest
    | CameraMessage of FreeFlyController.Message

module App =
    
    let initial = { currentModel = Box; cameraState = FreeFlyController.initial; messages = List.Empty }

    let update (m : Model) (msg : Message) =
        match msg with
            | ToggleModel -> 
                match m.currentModel with
                  | Box -> { m with currentModel = Sphere } |> OverlayHandling.notificationM "Wow, I am a Sphere now" MessageType.Success
                  | Sphere -> { m with currentModel = Box } |> OverlayHandling.notificationM "Suddenly I am a Box" MessageType.Info
                
            | WarningTest -> 
              m |> OverlayHandling.notificationM "You should go on a break" MessageType.Warning
            | ErrorTest -> 
              m |> OverlayHandling.notificationM "You made a huge mistake" MessageType.Error
            | CameraMessage msg ->
              { m with cameraState = FreeFlyController.update m.cameraState msg }

    let view (m : MModel) =

        let frustum = 
            Frustum.perspective 60.0 0.1 100.0 1.0 
                |> Mod.constant

        let sg =
            m.currentModel |> Mod.map (fun v ->
                match v with
                    | Box -> Sg.box (Mod.constant C4b.Cyan) (Mod.constant (Box3d(-V3d.III, V3d.III)))
                    | Sphere -> Sg.sphere 5 (Mod.constant C4b.Green) (Mod.constant 1.5)
                    
            )
            |> Sg.dynamic
            |> Sg.shader {
                do! DefaultSurfaces.trafo
                do! DefaultSurfaces.simpleLighting
            }

        let att =
            [
                style "position: fixed; left: 0; top: 0; width: 100%; height: 100%"
                attribute "data-customLoaderImg" "url('https://www.pinclipart.com/picdir/middle/306-3060913_png-file-clipart.png')"
            ]

        body [] [
            FreeFlyController.controlledControl m.cameraState CameraMessage frustum (AttributeMap.ofList att) sg

            div [style "position: fixed; left: 20px; top: 20px"] [
                button [onClick (fun _ -> ToggleModel)] [text "Toggle Model"]                
            ]
            div [style "position: fixed; left: 20px; top: 50px"] [
                button [onClick (fun _ -> WarningTest)] [text "Warning"]                
            ]
            div [style "position: fixed; left: 20px; top: 80px"] [
                button [onClick (fun _ -> ErrorTest)] [text "Error"]                
            ]
            OverlayHandling.createMessageOverlay m.messages
            

        ]

    let app =
        {
            initial = initial
            update = update
            view = view
            threads = Model.Lens.cameraState.Get >> FreeFlyController.threads >> ThreadPool.map CameraMessage
            unpersist = Unpersist.instance
        }