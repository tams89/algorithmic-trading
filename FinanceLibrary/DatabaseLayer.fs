namespace FinanceLibrary

open System
open System.Data.Linq
open Microsoft.FSharp.Data.TypeProviders
open FinanceLibrary.Interfaces
open FinanceLibrary.Records

module DatabaseLayer = 
 
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
  member this.InsertIterationData 
   (data: decimal*decimal*decimal*int*int*decimal*decimal*decimal*decimal*decimal*string) = 
   
   // Make individual data accessible from tuple.
   let sc,cc,pv,cp,sp,psv,spv,r,pnl,cv,ct = data
   
   // data formatted to SQL table.
   let newData = 
    new dbSchema.ServiceTypes.Portfolio_Iterations(
     IterationId = Guid.NewGuid(),
     StartingCash = sc,
     CurrentCash = cc,
     PortfolioValue = pv,
     CurrentPositions = cp,
     ShortPositions = sp,
     PositionValue = psv,
     ShortPositionValue = spv,
     Returns = r,
     ProfitAndLoss = pnl,
     ConstantValue = cv,
     ConstantType = ct)

   iterationTable.InsertOnSubmit(newData)


  /// Writes iteration data results to database.
  member this.InsertIterationData 
   (data : System.Collections.Generic.IEnumerable<decimal*decimal*decimal*int*int*decimal*decimal*decimal*decimal*decimal*string>) = 
   
   let newData = 
    [ for i in data ->
       let sc,cc,pv,cp,sp,psv,spv,r,pnl,cv,ct = i
       new dbSchema.ServiceTypes.Portfolio_Iterations(
        IterationId = Guid.NewGuid(),
        StartingCash = sc,
        CurrentCash = cc,
        PortfolioValue = pv,
        CurrentPositions = cp,
        ShortPositions = sp,
        PositionValue = psv,
        ShortPositionValue = spv,
        Returns = r,
        ProfitAndLoss = pnl,
        ConstantValue = cv,
        ConstantType = ct) ]

   iterationTable.InsertAllOnSubmit(newData)

  member this.Commit () = 
   try
    db.Portfolio_Iterations.Context.SubmitChanges()
   with
    | exn -> printfn "Exception: \n%s" exn.Message