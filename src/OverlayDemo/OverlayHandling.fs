namespace OverlayDemo

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI

open System

open OverlayDemo.Model


module OverlayHandling = 


  let inline (==>) a b = Aardvark.UI.Attributes.attribute a b  

  let dependencies = 
    [   
        { name="notify"; url = "../../notify.js"; kind = Script }
    ]

  let createMessageOverlay (messages : IMod<List<(DateTime*string*MessageType)>>)  = 
    let attr = 
      AttributeMap.ofList[ 
        "display"               ==> "block"; 
        "width"                 ==> "0%"; 
        "height"                ==> "0%"; 
        "preserveAspectRatio"   ==> "xMidYMid meet"; 
        "viewBox"               ==> "0 0 20 20";
        "style"                 ==> "position:absolute; left: 0%; top: 0%"
      ]
  
    let messageTag msgType msg = 
       match msgType with
        | MessageType.Success -> onBoot (sprintf "successNotification('%s')" msg) (div [clazz "prettyprint runnable"] [text "notification div"])
        | MessageType.Info    -> onBoot (sprintf "infoNotification('%s')"    msg) (div [clazz "prettyprint runnable"] [text "notification div"])
        | MessageType.Warning -> onBoot (sprintf "warnNotification('%s')"    msg) (div [clazz "prettyprint runnable"] [text "notification div"])
        | MessageType.Error   -> onBoot (sprintf "errorNotification('%s')"   msg) (div [clazz "prettyprint runnable"] [text "notification div"])



    let messageOverlay = 
      alist{
        let! msgList     = messages
        
        let notifyMsgs  = 
          msgList     
            |> List.filter(fun (time,_,_) -> DateTime.Now.Subtract(time).Milliseconds < 20)
            |> List.map(fun (_,msg,msgType)  -> (msg.Replace("\n"," "),msgType))
          
        yield require dependencies (
          div [] [
            for (msg, msgType) in notifyMsgs  do 
              yield messageTag msgType msg
           ])
      }

    Incremental.Svg.svg attr messageOverlay

  let notificationM message messageType m = 
    
    let messageMap = 
      m.messages 
        |> List.filter(fun (f,_,_) -> DateTime.Now.Subtract(f).Milliseconds < 50)
    
    { m with messages = messageMap |> List.append[DateTime.Now,message,messageType]}
  
    

    //match messageType with
    //| MessageType.Message -> {m with messagestoShow = (messageMap |> List.append [DateTime.Now,message]); infostoShow = infoMap; warningstoShow = warnMap; errorstoShow = errorMap}
    //| MessageType.Info    -> {m with messagestoShow = messageMap; infostoShow = (infoMap |> List.append[DateTime.Now,message]); warningstoShow = warnMap; errorstoShow = errorMap}
    //| MessageType.Warning -> {m with messagestoShow = messageMap; infostoShow = infoMap; warningstoShow = (warnMap |> List.append [DateTime.Now,message]); errorstoShow = errorMap}
    //| MessageType.Error   -> {m with messagestoShow = messageMap; infostoShow = infoMap; warningstoShow = warnMap; errorstoShow = (errorMap |> List.append [DateTime.Now,message])}
