namespace FinanceLibrary.AlgorithmicTrading

module AlgoBackTester =

 open System
 open Microsoft.FSharp.Collections
 open FinanceLibrary
 open Records
 open Interfaces
 open YahooFinanceAPI.Stock
 open AlgorithmicTrading.AlgoPortfolio
 open AlgorithmicTrading.AlgoCalculation
 open AlgorithmicTrading.AlgoTrader
 open DatabaseLayer

 let console (item,value) = 
  printfn "Current of %A is %A" item value

 // Filter any useless or erroneous data.
 let cleanPrices (prices: Tick []) = 
  let limit = prices.GetUpperBound(0)
  let rec tickList list counter =
   match prices.[counter] with 
   | tick when counter < limit && tick.High >= 1M && tick.Low >= 1M && tick.Close >= 1M && tick.Volume >= 1M 
     -> tickList (tick::list) (counter + 1)
   | tick when counter = limit -> List.rev list
   | _ -> tickList list (counter + 1)
  tickList List.empty<Tick> 0

 /// Backtest using historical data to simulate returns.
 let backTest () = 
  let logRecs = new System.Collections.Generic.List<Log*DateTime*DateTime>()
 
  // Set data variables.
  let symbol = "MSFT"                                                             // Get historical stock prices for the symbol.
  let backTestPeriod = 3000                                                       // Previous days worth of historical data to obtain.
  let logger = new WriteIterationData()                                           // Database logging service.
  let stockService = new GetStockDataWeb() :> IStockService                       // Historical data service.
  let prices =                                                                    // Obtain historical data.
   List.toArray (cleanPrices (stockService.GetStockPrices symbol backTestPeriod))

  let executeRun iterate = 
   
   // Instantiate system components.
   let startingCash = 10000M      // Portfolio starting capital.
   let startDate = DateTime.Today // Portfolio creation date. 
   let portfolio = new Portfolio(startingCash, startDate)
   let finCalc = new Calculation()
   let trader = new Trader(symbol, portfolio, finCalc)

   // ALGORITHM VARIABLES.
   let shortVwap = 0.995M                // percentage of vwap to allow short position. (default = 0.998M / 0.1% less than vwap)
   let longVwap = 1.005M                 // percentage of vwap to allow long position. (default = 1.100M / 1% greater than vwap)
   let coverBarrier = 0.995M             // percentage of current price to begin covering at. (default 0.99M)
   let minLimit = - portfolio.Cash       // must be negative, used for short positions.
   let maxLimit = portfolio.Cash + 0.1M  // must be postive, used for long positions.
   let coverAfter = 1.0                  // Days to cover any open positions after.
   let vwapPeriod = 5.0                  // Period of days to use to calculate vwap.

   // Execute trading algorithm on the historical data.
   prices 
   |> Seq.iter (fun tick -> trader.IncomingTick(tick, shortVwap, longVwap, coverBarrier, minLimit, maxLimit, coverAfter, vwapPeriod))

   // Return the portfolio on market close / simulation over.
   let variables = 
    { Symbol = symbol
      BackTestPeriod = backTestPeriod; 
      ShortVwap = shortVwap; 
      LongVwap = longVwap; 
      VwapPeriod = vwapPeriod; 
      CoverBarrierPrice = coverBarrier; 
      MinLimit = minLimit; 
      MaxLimit = maxLimit; 
      CoverAfterDays = coverAfter  }

   let log = 
    { Portfolio = portfolio
      Variables = variables  }
   
   printfn "%A" iterate
   log
  
  // Store the constant iterated over, and portfolio results.
  let addToLog (log : Log, startDate, endDate) =
   logRecs.Add(log, startDate, endDate)

  [ 0M ]
  |> PSeq.ordered
  |> PSeq.iter (fun i -> 
     ((executeRun i), prices.[0].Date, prices.[prices.Length - 1].Date)
     |> addToLog)
     
  // Insert collection of log data to database.
  logger.InsertIterationData(logRecs)
  logger.Commit()
  
  printfn "Algorithm Ended."