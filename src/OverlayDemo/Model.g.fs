namespace OverlayDemo.Model

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open OverlayDemo.Model

[<AutoOpen>]
module Mutable =

    
    
    type MModel(__initial : OverlayDemo.Model.Model) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<OverlayDemo.Model.Model> = Aardvark.Base.Incremental.EqModRef<OverlayDemo.Model.Model>(__initial) :> Aardvark.Base.Incremental.IModRef<OverlayDemo.Model.Model>
        let _currentModel = ResetMod.Create(__initial.currentModel)
        let _cameraState = Aardvark.UI.Primitives.Mutable.MCameraControllerState.Create(__initial.cameraState)
        let _messages = ResetMod.Create(__initial.messages)
        
        member x.currentModel = _currentModel :> IMod<_>
        member x.cameraState = _cameraState
        member x.messages = _messages :> IMod<_>
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : OverlayDemo.Model.Model) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_currentModel,v.currentModel)
                Aardvark.UI.Primitives.Mutable.MCameraControllerState.Update(_cameraState, v.cameraState)
                ResetMod.Update(_messages,v.messages)
                
        
        static member Create(__initial : OverlayDemo.Model.Model) : MModel = MModel(__initial)
        static member Update(m : MModel, v : OverlayDemo.Model.Model) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<OverlayDemo.Model.Model> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Model =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let currentModel =
                { new Lens<OverlayDemo.Model.Model, OverlayDemo.Model.Primitive>() with
                    override x.Get(r) = r.currentModel
                    override x.Set(r,v) = { r with currentModel = v }
                    override x.Update(r,f) = { r with currentModel = f r.currentModel }
                }
            let cameraState =
                { new Lens<OverlayDemo.Model.Model, Aardvark.UI.Primitives.CameraControllerState>() with
                    override x.Get(r) = r.cameraState
                    override x.Set(r,v) = { r with cameraState = v }
                    override x.Update(r,f) = { r with cameraState = f r.cameraState }
                }
            let messages =
                { new Lens<OverlayDemo.Model.Model, Microsoft.FSharp.Collections.List<(System.DateTime * System.String * OverlayDemo.Model.MessageType)>>() with
                    override x.Get(r) = r.messages
                    override x.Set(r,v) = { r with messages = v }
                    override x.Update(r,f) = { r with messages = f r.messages }
                }
