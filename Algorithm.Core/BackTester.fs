namespace Algorithm.Core.AlgorithmicTrading

module AlgoBackTester =

 open System
 open Microsoft.FSharp.Collections
 open Algorithm.Core
 open Records
 open Interfaces
 open YahooFinanceService.Stock
 open AlgorithmicTrading.AlgoPortfolio
 open AlgorithmicTrading.AlgoCalculation
 open AlgorithmicTrading.AlgoTrader
 open AlgorithmicTrading.AlgoMarket
 open DatabaseLayer

 /// Backtest using historical data to simulate potential returns.
 /// e.g. 
 /// let symbol = "IBM Minute" - Get historical stock prices for the symbol.
 /// let backTestPeriod = 20 - Previous days worth of historical data to obtain. (252 trading days per year) 
 /// let stockService = new Database() :> IStockService - Historical data service (local DB).
 /// let stockService = new StockDataService() :> IStockService - Historical data service (web).
 /// let startingCash = 10000M - Portfolio starting capital.
 /// let simulateSlippage = false - Should price slippage be simulated?
 /// ALGORITHM VARIABLES.
 /// let shortVwap = 0.995M    - percentage of vwap to allow short position. (default = 0.995M)
 /// let longVwap = 1.005M     - percentage of vwap to allow long position. (default = 1.005M)
 /// let coverBarrier = 0.998M - percentage of current price to begin covering at. (default 0.99M)
 /// let coverAfter = 1.0      - Days to cover any open positions after.
 /// let vwapPeriod = 1.0      - Period of days to use to calculate vwap. (5.0 ideal for interday, 1.0 for minute tick data)
 type BackTester (symbol, backTestPeriod, stockService : IStockService, startingCash, simulateSlippage, shortVwap, longVwap, coverBarrier, coverAfter, vwapPeriod) = 

  member this.BackTest () = 

   // Database service for logging.
   let db = new Database()                                           
  
   // Instantiate system components.
   let portfolio = new Portfolio(startingCash)
   let finCalc = new Calculation()
   let market = new Market(float(backTestPeriod), simulateSlippage)
   let trader = new Trader(symbol, portfolio, finCalc, Option.Some market)
   let logRecs = new System.Collections.Generic.List<Log*DateTime*DateTime>()
  
   // Filter any useless or erroneous data.
   let cleanPrices(prices: Tick []) = 
    let limit = prices.GetUpperBound(0)
    let rec tickList list counter =
     match prices.[counter] with 
     | tick when counter < limit && tick.High >= 1M && tick.Low >= 1M && tick.Close >= 1M && tick.Volume >= 1M 
       -> tickList (tick::list) (counter + 1)
     | tick when counter = limit -> List.rev list
     | _ -> tickList list (counter + 1)
    tickList List.empty<Tick> 0
  
   // Obtain historical data and clean.
   let prices =                                                     
    cleanPrices (stockService.GetStockPrices symbol backTestPeriod)
    |> List.toArray
  
   printfn "Algorithm Started..."
   
   // Execute trading algorithm on the historical data.
   let mutable x = 0
   for tick in prices do 
    trader.IncomingTick(tick, shortVwap, longVwap, coverBarrier, coverAfter, vwapPeriod)
    printfn "Tick %A of %A" x (prices.Length)
    x <- x + 1
  
   // Return the portfolio on market close / simulation over.
   let variables = 
    { Symbol = symbol
      BackTestPeriod = backTestPeriod; 
      ShortVwap = shortVwap; 
      LongVwap = longVwap; 
      VwapPeriod = vwapPeriod; 
      CoverBarrierPrice = coverBarrier; 
      CoverAfterDays = coverAfter  }
  
   let log = 
    { Portfolio = portfolio
      Variables = variables  }
   
   let startDate = prices.[0].Date
   let endDate = prices.[prices.Length - 1].Date
   logRecs.Add(log, startDate, endDate)
      
   // Insert collected data to database.
   printfn "Inserting Data..."
   for data,sd,ed in logRecs do
    let tData = data,sd,ed
    let id = db.InsertIterationData(tData)
    db.InsertOrders(data.Portfolio.Positions, false, id)
    db.InsertOrders(data.Portfolio.ClosedPositions, true, id)
   db.Commit()
   printfn "Data inserted..."
  
   printfn "Algorithm Ended..."
  
  // Constructors
  new () = BackTester("IBM Minute", 10000, new DatabaseLayer.Database(), 10000M, false, 0.995M, 1.005M, 0.998M, 1.0, 1.0)
  new (backtestPeriod) = BackTester("IBM Minute", backtestPeriod, new DatabaseLayer.Database(), 10000M, false, 0.995M, 1.005M, 0.998M, 1.0, 1.0)
  new (symbol, backtestPeriod) = BackTester(symbol, backtestPeriod, new StockDataService(), 10000M, false, 0.995M, 1.005M, 0.998M, 1.0, 1.0)