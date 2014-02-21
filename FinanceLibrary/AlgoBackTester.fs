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
   | tick when counter = limit -> list
   | _ -> tickList list (counter + 1)
  tickList List.empty<Tick> 0
 
 /// Backtest using historical data to simulate returns.
 let backTest () = 
  let logRecs = 
   new System.Collections.Generic.List<decimal*decimal*decimal*int*int*int*int*decimal*decimal*decimal*decimal*decimal*decimal*decimal*string>()
 
  // Set data variables.
  let symbol = "MSFT"                                                          // Get historical stock prices for the symbol.
  let backTestPeriod = 3000                                                    // Previous days worth of historical data to obtain.
  let logger = new WriteIterationData()                                        // Database logging service.
  let stockService = new GetStockDataWeb() :> IStockService                    // Historical data service.
  let prices = cleanPrices (stockService.GetStockPrices symbol backTestPeriod) // Obtain historical data.

  // Instantiate system components.
  let startingCash = 10000M     
  let startDate = DateTime.Today
  let portfolio = new Portfolio(startingCash, startDate)
  let trader = new Trader(symbol, portfolio)
  let finCalc = new Calculation(prices)

  // ALGORITHM VARIABLES.
  let shortVwap = i                     // percentage of vwap to allow short position. ITERATING
  let longVwap = 1.001M                 // percentage of vwap to allow long position.
  let coverBarrier = 0.99M              // percentage of current price to begin covering at.
  let minLimit = - (abs portfolio.Cash) // must be negative, used for short positions.
  let maxLimit = portfolio.Cash + 0.1M  // must be postive, used for long positions.
  let numOfShares = 100M                // Shares limit to buy/sell.
  let coverAfter = 1.0                  // Days to cover any open shorts after.
  let vwapPeriod =  5.0                 // Period of days to use to calculate vwap.
  let vwap = finCalc.VWAP(vwapPeriod)   // Volume Weighted Average Price calculated from cleaned prices.
  
  let executeRun i = 
   prices |> Seq.iter (fun tick -> 
    trader.IncomingTick(tick, shortVwap, longVwap, coverBarrier, minLimit, maxLimit, numOfShares, coverAfter, vwap))
   portfolio // Return the portfolio on market close / simulation over.
  
  // Store the constant iterated over, and portfolio results.
  let addToLog (portfolio:Portfolio, iterateVal:decimal, whatIterated:string)  =
   logRecs.Add(
    (portfolio.StartingCash,
     portfolio.Cash,
     portfolio.PortfolioValue, 
     portfolio.Positions  |> Seq.length, 
     portfolio.ShortPositions  |> Seq.length, 
     portfolio.ClosedPositions  |> Seq.length,
     portfolio.ClosedShortPositions |> Seq.length,
     portfolio.PositionsValue, 
     portfolio.ShortPositionsValue, 
     portfolio.ClosedPositionsValue, 
     portfolio.ClosedShortPositionsValue, 
     portfolio.Returns, 
     portfolio.ProfitAndLoss, 
     iterateVal, 
     whatIterated))
   (whatIterated,iterateVal)
  
  // Iterate variable to determine best value.
  [ 0.000M..0.005M..2.000M ]
  |> PSeq.ordered
  |> PSeq.iter (fun i -> 
   ((executeRun i), i, backTestPeriod.ToString() + " DaysBack " + " ShortVWAP " + " Cover after " + coverAfter.ToString()) 
   |> addToLog 
   |> console)
  
  // Insert collection of log data to database.
  logger.InsertIterationData(logRecs)
  logger.Commit()
  
  printfn "Algorithm Ended."