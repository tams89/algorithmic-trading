namespace Algorithm.Core.AlgorithmicTrading

module AlgoLiveSystem = 

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
 
 /// Backtest by polling the market every minute via the yahoo finance api. 
 /// Based on AlgoBackTester.
 type LiveBackTest (symbol, stockService : IStockService, startingCash, shortVwap, longVwap, coverBarrier, coverAfter, vwapPeriod)  = 
  
  member this.BackTest () = 
   
   // Database service for logging.
   let db = new Database()                                           
  
   // Instantiate system components.
   let portfolio = new Portfolio(startingCash)
   let finCalc = new Calculation()
   let trader = new Trader(symbol, portfolio, finCalc, None)
   let logRecs = new System.Collections.Generic.List<Log*DateTime*DateTime>()

   // Hardcoded list of non-trading days specific for NYSE.
   // (Day,Month)
   let nonTradingDays = 
    [| (1,1); (20,1); (17,2); (18,4); (26,5); (4,7); (1,9); (127,11); (25,12) |]
   
   // Checks to see if the current month and day are in the non-trading days list.
   let algoLives = 
    let checkDay = 
     if nonTradingDays |> Array.exists (fun x -> x = (DateTime.Today.Day, DateTime.Today.Month)) then false
     else true 
    let marketOpen = 
     if (DateTime.UtcNow.TimeOfDay.Hours > 9 && DateTime.UtcNow.TimeOfDay.Minutes > 30) && (DateTime.UtcNow.TimeOfDay.Hours < 4) 
     then true
     else false
    checkDay && marketOpen

   // Poll the market every minute within market open hours specified (US Eastern, NYSE). 
   while true = true do 

    // Get current tick
    let tick = stockService.GetRealTimePrice(symbol)
    
    // Execute trading.
    trader.IncomingTick()

    System.Threading.Thread.Sleep(1*60*1000)


  // Constructors
  new () = LiveBackTest("IBM", new YahooFinanceService.Stock.StockDataService(), 10000M, 0.995M, 1.005M, 0.998M, 1.0, 1.0)