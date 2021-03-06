﻿namespace Algorithm.Core

module DatabaseLayer = 

 open System
 open System.Data.Linq
 open Microsoft.FSharp.Data.TypeProviders
 open Algorithm.Core.Interfaces
 open Algorithm.Core.Records
 open Algorithm.Core.AlgorithmicTrading.AlgoPortfolio
 
 type Log =
   { Portfolio:Portfolio
     Variables:Variables }

 type dbSchema = SqlDataConnection<"Data Source=.;Initial Catalog=SivaguruCapital;Integrated Security=True">

 let iterationToIterationTable data = 
  let log,ds,ed = data
  let table = new dbSchema.ServiceTypes.Iteration()
  table.Symbol <- log.Variables.Symbol
  table.StartDate <- ds
  table.EndDate <- ed
  table.StartingCash <- log.Portfolio.StartingCash 
  table.CurrentCash <- log.Portfolio.Cash 
  table.PortfolioValue <- log.Portfolio.PortfolioValue 
  table.CurrentPositions <- log.Portfolio.Positions |> Seq.length
  table.ShortPositions <- log.Portfolio.ShortPositions |> Seq.length
  table.ClosedPositions <- log.Portfolio.ClosedPositions |> Seq.length
  table.ClosedShortPositions <- log.Portfolio.ClosedShortPositions |> Seq.length
  table.PositionValue <- log.Portfolio.PositionsValue 
  table.ShortPositionValue <- log.Portfolio.ShortPositionsValue 
  table.ClosedPositionValue <- log.Portfolio.ClosedPositionsValue 
  table.ClosedShortPositionValue <- log.Portfolio.ClosedShortPositionsValue 
  table.Return <- log.Portfolio.Returns 
  table.ProfitAndLoss <- log.Portfolio.ProfitAndLoss 
  table.ShortVwap <- log.Variables.ShortVwap 
  table.LongVwap <- log.Variables.LongVwap 
  table.VwapPeriod <- decimal log.Variables.VwapPeriod 
  table.CoverBarrierPrice <- log.Variables.CoverBarrierPrice 
  table.CoverAfterDays <- decimal log.Variables.CoverAfterDays 
  table

 let orderToOrderTable (data : Order, isClosed : bool, iterationId : int option) = 
  let table = new dbSchema.ServiceTypes.Order()
  table.IterationId <- iterationId.Value
  table.Symbol <- data.Symbol
  table.Date <- data.Date
  table.Quantity <- int data.Quantity
  match data.OrderType with
  | Long -> table.OrderType <- "Long"
  | Short -> table.OrderType <- "Short"
  | Cover -> table.OrderType <- "Cover"
  table.Value <- data.Value
  table.IsClosed <- isClosed
  table

 type Database () = 

  let db = dbSchema.GetDataContext()
  let iterationTable = db.Iteration
  let orderTable = db.Order

  do
   db.DataContext.Log <- Console.Out

  /// Writes iteration data results to database.
  member this.InsertIterationData (data: Log*DateTime*DateTime) = 
   let newData = iterationToIterationTable data
   iterationTable.InsertOnSubmit(newData)
   this.Commit()
   Some newData.IterationId

  /// Writes iteration data results to database.
  member this.InsertIterationData (data : System.Collections.Generic.IEnumerable<Log*DateTime*DateTime>) = 
   let newData = [ for i in data -> iterationToIterationTable i ]
   iterationTable.InsertAllOnSubmit(newData)
  
  /// Writes order to database.
  member this.InsertOrder (data : Order, isClosed : bool, iterationId : int option) =
   orderTable.InsertOnSubmit(orderToOrderTable(data,isClosed,iterationId))

  /// Writes orders to database.
  member this.InsertOrders (data : System.Collections.Generic.IEnumerable<Order>, isClosed : bool, iterationId : int option) =
   let newData = [ for i in data -> orderToOrderTable(i,isClosed,iterationId) ]
   orderTable.InsertAllOnSubmit(newData)

  /// Commits any open transactions.
  member this.Commit () = 
   try
    db.DataContext.SubmitChanges()
   with
    | exn -> printfn "Exception: \n%s" exn.Message

   
  interface IStockService with

   // Query to fetch all HFT.Tick datwhere within date range and has matching symbol.
   /// Stock prices for number of days before today.
   member this.GetPreviousStockPrices symbol daysBack = 
      query { for row in db.Tick do
              where (row.Symbol = symbol && row.Date >= DateTime.Today.AddDays(float(-daysBack)))
              sortBy row.Date 
              select row }
      |> Seq.map (fun x -> 
          { Date = x.Date
            Open = decimal x.Open
            High = decimal x.High
            Low = decimal x.Low
            Close = decimal x.Close
            Volume = (decimal x.Volume)
            AdjClose = 0M
            Ask = None
            Bid = None }) |> Seq.toArray

   /// Stock prices for number of days before today.
   member this.GetStockPrices symbol numRecs = 
      query { for row in db.Tick do 
              where (row.Symbol = symbol && row.Date >= query { for row in db.Tick do maxBy (row.Date.AddDays(float(-numRecs))) } )
              sortBy row.Date 
              select row }
      |> Seq.map (fun x -> 
          { Date = x.Date
            Open = decimal x.Open
            High = decimal x.High
            Low = decimal x.Low
            Close = decimal x.Close
            Volume = (decimal x.Volume)
            AdjClose = 0M
            Ask = None
            Bid = None }) |> Seq.toArray

    member this.GetRealTimePrice symbol = 
                  { Date = DateTime.Now
                    Open = 0M
                    High = 0M
                    Low = 0M
                    Close = 0M
                    Volume = 0M
                    AdjClose = 0M
                    Ask = None
                    Bid = None }