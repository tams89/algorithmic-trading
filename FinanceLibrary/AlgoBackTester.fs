namespace FinanceLibrary.AlgorithmicTrading

module AlgoBackTester =

 open System
 open Microsoft.FSharp.Collections
 open Microsoft.FSharp.Quotations
 open Microsoft.FSharp.Quotations.Patterns
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
 
 /// Returns the name of the variable.
 let fn (e) =
  match e with
    | PropertyGet (_, pi, _) -> pi.Name
    | _ -> failwith "not a let-bound value"

 // Filter any useless or erroneous data.
 let cleanPrices (prices: Tick []) = 
  let limit = prices.GetUpperBound(0)
  let rec tickList list counter =
   match prices.[counter] with 
   | tick when counter < limit && tick.High >= 1M && tick.Low >= 1M && tick.Close >= 1M && tick.Volume >= 1M 
     -> tickList (tick::list) (counter + 1)
   | tick when counter = limit -> list
   | _ -> tickList list (counter + 1)
  tickList List.empty<Tick> 0
 
 /// Backtest using historical data to simulate returns.
 let backTest () = 
  let logRecs = new System.Collections.Generic.List<Log*string*decimal*DateTime*DateTime>()
 
  // Set data variables.
  let symbol = "MSFT"                                                          // Get historical stock prices for the symbol.
  let backTestPeriod = 3000                                                    // Previous days worth of historical data to obtain.
  let logger = new WriteIterationData()                                        // Database logging service.
  let stockService = new GetStockDataWeb() :> IStockService                    // Historical data service.
  let prices = cleanPrices (stockService.GetStockPrices symbol backTestPeriod) // Obtain historical data.

  let executeRun iterate = 
   
   // Instantiate system components.
   let startingCash = 10000M      // Portfolio starting capital.
   let startDate = DateTime.Today // Portfolio creation date. 
   let portfolio = new Portfolio(startingCash, startDate)
   let trader = new Trader(symbol, portfolio)
   let finCalc = new Calculation(prices)

   // ALGORITHM VARIABLES.
   let shortVwap = iterate               // percentage of vwap to allow short position. (defauly = 0.998M / 0.2% less than vwap)
   let longVwap = 0.540M                 // percentage of vwap to allow long position. (default = 1.100M / 1% greater than vwap)
   let coverBarrier = 0.99M              // percentage of current price to begin covering at.
   let minLimit = - portfolio.Cash       // must be negative, used for short positions.
   let maxLimit = portfolio.Cash + 0.1M  // must be postive, used for long positions.
   let numOfShares = 100M                // Shares limit to buy/sell.
   let coverAfter = 1.0                  // Days to cover any open shorts after.
   let vwapPeriod = 35.0                 // Period of days to use to calculate vwap.
   let vwap = finCalc.VWAP(vwapPeriod)   // Volume Weighted Average Price calculated from cleaned prices.

   // Execute trading algorithm on the historical data.
   prices |> Seq.iter (fun tick -> 
    trader.IncomingTick(tick, shortVwap, longVwap, coverBarrier, minLimit, maxLimit, numOfShares, coverAfter, vwap))
 
   // Return the portfolio on market close / simulation over.
   let variables = 
    { Symbol = symbol
      BackTestPeriod = backTestPeriod; 
      ShortVwap = shortVwap; 
      LongVwap = longVwap; 
      VwapPeriod = vwapPeriod; 
      Vwap = vwap; 
      CoverBarrierPrice = coverBarrier; 
      MinLimit = minLimit; 
      MaxLimit = maxLimit; 
      NumShares = numOfShares; 
      CoverAfterDays = coverAfter  }

   let log = 
    { Portfolio = portfolio
      Variables = variables  }

   log
  
  // Store the constant iterated over, and portfolio results.
  let addToLog (log : Log, iterationType:string, iterationValue:decimal, startDate, endDate)  =
   logRecs.Add(log,iterationType,iterationValue, startDate, endDate)
   (iterationType,iterationValue)
  
  // Iterate variable to determine best value.
  [ 0.000M..0.005M..2.000M ]
  |> PSeq.ordered
  |> PSeq.iter (fun i -> 
      ((executeRun i), fn <@ i @>, i, prices.Head.Date, (prices |> List.rev).Head.Date)
      |> addToLog 
      |> console)
  
  // Insert collection of log data to database.
  logger.InsertIterationData(logRecs)
  logger.Commit()
  
  printfn "Algorithm Ended."