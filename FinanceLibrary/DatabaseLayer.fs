namespace FinanceLibrary

module DatabaseLayer = 

 open System
 open System.Data.Linq
 open Microsoft.FSharp.Data.TypeProviders
 open FinanceLibrary.Interfaces
 open FinanceLibrary.Records
 open FinanceLibrary.AlgorithmicTrading.AlgoPortfolio
 
 type Log =
   { Portfolio:Portfolio
     Variables:Variables }

 type dbSchema = SqlDataConnection<"Data Source=.;Initial Catalog=SivaguruCapital;Integrated Security=True">

 let iterationToIterationTable data = 
  let log,ds,ed = data
  let table = new dbSchema.ServiceTypes.Portfolio_Iteration()
  table.IterationId <- Guid.NewGuid()
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
  table.Returns <- log.Portfolio.Returns 
  table.ProfitAndLoss <- log.Portfolio.ProfitAndLoss 
  table.ShortVwap <- log.Variables.ShortVwap 
  table.LongVwap <- log.Variables.LongVwap 
  table.VwapPeriod <- log.Variables.VwapPeriod 
  table.CoverBarrierPrice <- log.Variables.CoverBarrierPrice 
  table.CoverAfterDays <- log.Variables.CoverAfterDays 
  table

 let orderToOrderTable (data : Order, isClosed : bool, iterationId : Guid option) = 
  let table = new dbSchema.ServiceTypes.Portfolio_Order()
  table.OrderId <- Guid.NewGuid()
  match iterationId.IsSome with 
  | true -> table.IterationId <- new Nullable<Guid>(iterationId.Value)
  | false -> table.IterationId <- new Nullable<Guid>()
  table.Symbol <- data.Symbol
  table.Date <- data.Date
  table.Quantity <- data.Quantity
  match data.OrderType with
  | Long -> table.OrderType <- "Long"
  | Short -> table.OrderType <- "Short"
  | Cover -> table.OrderType <- "Cover"
  table.Value <- data.Value
  match isClosed with
  | true -> table.IsClosed <- true
  | false -> table.IsClosed <- false
  table

 type Database () = 

  let db = dbSchema.GetDataContext()
  let iterationTable = db.Portfolio_Iteration
  let orderTable = db.Portfolio_Order

  do
   db.DataContext.Log <- Console.Out

  /// Writes iteration data results to database.
  member this.InsertIterationData (data: Log*DateTime*DateTime) = 
   let newData = iterationToIterationTable data
   iterationTable.InsertOnSubmit(newData)
   Some newData.IterationId

  /// Writes iteration data results to database.
  member this.InsertIterationData (data : System.Collections.Generic.IEnumerable<Log*DateTime*DateTime>) = 
   let newData = [ for i in data -> iterationToIterationTable i ]
   iterationTable.InsertAllOnSubmit(newData)
  
  /// Writes order to database.
  member this.InsertOrder (data : Order, isClosed : bool, iterationId : Guid option) =
   orderTable.InsertOnSubmit(orderToOrderTable(data,isClosed,iterationId))

  /// Writes orders to database.
  member this.InsertOrders (data : System.Collections.Generic.IEnumerable<Order>, isClosed : bool, iterationId : Guid option) =
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
      query { for row in db.HFT_Tick do
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
            AdjClose = 0M }) |> Seq.toArray

       /// Stock prices for number of days before today.
   member this.GetStockPrices symbol numRecs = 
      query { for row in db.HFT_Tick do 
              where (row.Symbol = symbol && row.Date >= query { for row in db.HFT_Tick do maxBy (row.Date.AddDays(float(-numRecs))) } )
              sortBy row.Date 
              select row }
      |> Seq.map (fun x -> 
          { Date = x.Date
            Open = decimal x.Open
            High = decimal x.High
            Low = decimal x.Low
            Close = decimal x.Close
            Volume = (decimal x.Volume)
            AdjClose = 0M }) |> Seq.toArray