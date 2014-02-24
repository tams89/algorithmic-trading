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

 type dbSchema = SqlDataConnection<"Data Source=.;Initial Catalog=AlgorithmicTrading;Integrated Security=True">

 let dataToIterationTable data = 
  let log,ds,ed = data
  let table = new dbSchema.ServiceTypes.Portfolio_Iterations()
  table.IterationId <- Guid.NewGuid()
  table.Symbol <- log.Variables.Symbol
  table.StartDate <- ds
  table.EndDate <- ed
  table.StartingCash <- log.Portfolio.StartingCash 
  table.CurrentCash <- log.Portfolio.Cash 
  table.PortfolioValue <- log.Portfolio.PortfolioValue 
  table.CurrentPositions <- log.Portfolio.Positions.Count 
  table.ShortPositions <- (log.Portfolio.ShortPositions |> Seq.length) 
  table.ClosedPositions <- log.Portfolio.ClosedPositions.Count 
  table.ClosedShortPositions <- (log.Portfolio.ClosedShortPositions |> Seq.length) 
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
  table.MinLimit <- log.Variables.MinLimit 
  table.MaxLimit <- log.Variables.MaxLimit 
  table.CoverAfterDays <- log.Variables.CoverAfterDays 
  table

 type GetStockDataDB() = 
     let db = dbSchema.GetDataContext()
     do 
         // Enable SQL logging to console.
         db.DataContext.Log <- System.Console.Out
     
     let fetchData (symbol : string) (daysBack : int) = 
         query { for row in db.InterDay_HistoricalStock do
                 where (row.Symbol = symbol && row.Date >= DateTime.Today.AddDays(float(-daysBack)))
                 sortBy row.Date
                 select row }
         |> Seq.map (fun x -> 
                { Date = x.Date
                  Open = x.Open
                  High = x.High
                  Low = x.Low
                  Close = x.Close
                  Volume = (decimal x.Volume)
                  AdjClose = 0M })
         |> Seq.toArray

     interface IStockService with
         member this.GetStockPrices symbol daysBack = fetchData symbol daysBack
 
 /// Allows writing of data to iteration table.
 type WriteIterationData () = 
  let db = dbSchema.GetDataContext()
  let iterationTable = db.Portfolio_Iterations
  do 
   // Enable SQL logging to console.
   db.DataContext.Log <- System.Console.Out

  /// Writes iteration data results to database.
  member this.InsertIterationData (data: Log*DateTime*DateTime) = 
   iterationTable.InsertOnSubmit(dataToIterationTable data)

  /// Writes iteration data results to database.
  member this.InsertIterationData (data : System.Collections.Generic.IEnumerable<Log*DateTime*DateTime>) = 
   let newData = [ for i in data -> dataToIterationTable i ]
   iterationTable.InsertAllOnSubmit(newData)

  /// Commits any open transactions.
  member this.Commit () = 
   try
    db.Portfolio_Iterations.Context.SubmitChanges()
   with
    | exn -> printfn "Exception: \n%s" exn.Message