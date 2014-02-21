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

 type WriteIterationData () = 
  let db = dbSchema.GetDataContext()
  let iterationTable = db.Portfolio_Iterations
  do 
   // Enable SQL logging to console.
   db.DataContext.Log <- System.Console.Out

  /// Writes iteration data results to database.
  member this.InsertIterationData (data: Log*string*decimal) = 
   
   // data formatted to SQL table.
   let log,ct,cv = data
   let newData = 
    new dbSchema.ServiceTypes.Portfolio_Iterations(
     IterationId = Guid.NewGuid(),
     StartingCash = log.Portfolio.StartingCash,
     CurrentCash = log.Portfolio.Cash,
     PortfolioValue = log.Portfolio.PortfolioValue,
     CurrentPositions = log.Portfolio.Positions.Count,
     ShortPositions = (log.Portfolio.ShortPositions |> Seq.length),
     ClosedPositions = log.Portfolio.ClosedPositions.Count,
     ClosedShortPositions = (log.Portfolio.ClosedShortPositions |> Seq.length),
     PositionValue = log.Portfolio.PortfolioValue,
     ShortPositionValue = log.Portfolio.ShortPositionsValue,
     ClosedPositionValue = log.Portfolio.ClosedPositionsValue,
     ClosedShortPositionValue = log.Portfolio.ClosedShortPositionsValue,
     Returns = log.Portfolio.Returns,
     ProfitAndLoss = log.Portfolio.ProfitAndLoss,
     ShortVwap = log.Variables.ShortVwap,
     LongVwap = log.Variables.LongVwap,
     VwapPeriod = log.Variables.VwapPeriod,
     Vwap = log.Variables.Vwap,
     CoverBarrierPrice = log.Variables.CoverBarrierPrice,
     MinLimit = log.Variables.MinLimit,
     MaxLimit = log.Variables.MaxLimit,
     NumShares = log.Variables.NumShares,
     CoverAfterDays = log.Variables.CoverAfterDays,
     ConstantValue = cv,
     ConstantType = ct)

   iterationTable.InsertOnSubmit(newData)

  /// Writes iteration data results to database.
  member this.InsertIterationData (data : System.Collections.Generic.IEnumerable<Log*string*decimal>) = 

   let newData = 
    [ for i in data ->
       let log,ct,cv = i
       new dbSchema.ServiceTypes.Portfolio_Iterations(
        IterationId = Guid.NewGuid(),
        StartingCash = log.Portfolio.StartingCash,
        CurrentCash = log.Portfolio.Cash,
        PortfolioValue = log.Portfolio.PortfolioValue,
        CurrentPositions = log.Portfolio.Positions.Count,
        ShortPositions = (log.Portfolio.ShortPositions |> Seq.length),
        ClosedPositions = log.Portfolio.ClosedPositions.Count,
        ClosedShortPositions = (log.Portfolio.ClosedShortPositions |> Seq.length),
        PositionValue = log.Portfolio.PortfolioValue,
        ShortPositionValue = log.Portfolio.ShortPositionsValue,
        ClosedPositionValue = log.Portfolio.ClosedPositionsValue,
        ClosedShortPositionValue = log.Portfolio.ClosedShortPositionsValue,
        Returns = log.Portfolio.Returns,
        ProfitAndLoss = log.Portfolio.ProfitAndLoss,
        ShortVwap = log.Variables.ShortVwap,
        LongVwap = log.Variables.LongVwap,
        VwapPeriod = log.Variables.VwapPeriod,
        Vwap = log.Variables.Vwap,
        CoverBarrierPrice = log.Variables.CoverBarrierPrice,
        MinLimit = log.Variables.MinLimit,
        MaxLimit = log.Variables.MaxLimit,
        NumShares = log.Variables.NumShares,
        CoverAfterDays = log.Variables.CoverAfterDays,
        ConstantValue = cv,
        ConstantType = ct) ]

   iterationTable.InsertAllOnSubmit(newData)

  member this.Commit () = 
   try
    db.Portfolio_Iterations.Context.SubmitChanges()
   with
    | exn -> printfn "Exception: \n%s" exn.Message