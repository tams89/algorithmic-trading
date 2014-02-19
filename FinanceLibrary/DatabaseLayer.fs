﻿namespace FinanceLibrary

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
  /// Writes iteration data results to database.
  member this.InsertIterationData (data : decimal*decimal*decimal*decimal*decimal*decimal*decimal*decimal*decimal*decimal*string) = 
   let sc,cc,pv,cp,sp,psv,spv,r,pnl,cv,ct = data // data from tuple.
   
   // data formatted to SQL table.
   let newData = dbSchema.ServiceTypes.Portfolio_Iterations(StartingCash = sc,
                                                            CurrentCash = cc,
                                                            PortfolioValue = pv,
                                                            CurrentPositions = int cp,
                                                            ShortPositions = int sp,
                                                            PositionValue = psv,
                                                            ShortPositionValue = spv,
                                                            Returns = r,
                                                            ProfitAndLoss = pnl,
                                                            ConstantValue = cv,
                                                            ConstantType = ct)
   
   try
    dbSchema.GetDataContext().Portfolio_Iterations.InsertOnSubmit(newData)
    dbSchema.GetDataContext().Portfolio_Iterations.Context.SubmitChanges()
    printfn "Inserted new rows"
   with
    | exn -> printfn "Exception: \n%s" exn.Message