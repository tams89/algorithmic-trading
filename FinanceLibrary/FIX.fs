namespace FinanceLibrary

module FIX = 
 
 open System
 open System.Globalization
 open QuickFix
 open QuickFix.Transport
 open QuickFix.FIX42
 open QuickFix.Fields

 type ClientInitiator () = 
  
  interface IApplication with
  
   member this.OnCreate(sessionID : SessionID) : unit
    = printfn "OnCreate"

   member this.ToAdmin(msg : QuickFix.Message, sessionID : SessionID) : unit
    = printfn "ToAdmin"
   
   member this.FromAdmin(msg : QuickFix.Message, sessionID : SessionID) : unit 
    = printfn "FromAdmin"

   member this.ToApp(msg : QuickFix.Message, sessionID : SessionID) : unit
    = printfn "ToApp"

   member this.FromApp(msg : QuickFix.Message, sessionID : SessionID) : unit
    = printfn "FromApp -- %A" msg

   member this.OnLogout(sessionID : SessionID) : unit
    = printfn "OnLogout"

   member this.OnLogon(sessionID : SessionID) : unit
    = printfn "OnLogon"
 
 type ConsoleLog () = 

  interface System.IDisposable with
   member this.Dispose() : unit 
    = printfn "Disposing"

  interface ILog with

   member this.Clear() : unit
    = printfn "hello"
   
   member this.OnEvent(str : string) : unit
    = printfn "%s" str

   member this.OnIncoming(str : string) : unit
    = printfn "%s" str

   member this.OnOutgoing(str : string) : unit
    = printfn "%s" str

 type ConsoleLogFactory(settings : SessionSettings) =
  
  interface ILogFactory with
   
   member this.Create(sessionID : SessionID) : ILog
    = new NullLog() :> ILog
 
 type FIXEngine () = 
  
  let settings = new SessionSettings("C:\Users\Tam\Documents\Repositories\sivaguru-capital\FinanceLibrary\config.cfg")
  let application = new ClientInitiator()
  let storeFactory = FileStoreFactory(settings)
  let logFactory = new ConsoleLogFactory(settings)
  let messageFactory = new MessageFactory()
  let initiator = new SocketInitiator(application, storeFactory, settings)
  let currentID = initiator.GetSessionIDs() |> Seq.head
  
  member this.init() : unit
   = initiator.Start()

  member this.start() : unit
   = initiator.Start()

  member this.stop() : unit
   = initiator.Stop()

 let execute () = 
  let engine = new FIXEngine()
  engine.init()
  engine.start()
  ()