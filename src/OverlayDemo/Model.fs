namespace OverlayDemo.Model

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI.Primitives

type Primitive =
  | Box
  | Sphere

type MessageType =
  | Info
  | Success
  | Warning
  | Error


[<DomainType>]
type Model =
  {
      currentModel    : Primitive
      cameraState     : CameraControllerState

      messages        : List<(DateTime*string*MessageType)>
  }