#I "bin/Debug"

#r "FinanceLibrary"
#r "System.Data.Linq"
#r "System.Xml.Linq"
#r "FSharp.Data"
#r "FSharp.Data.TypeProviders"
#r "FSharp.PowerPack"

#load "RecordsNInterfaces.fs"
#load "DatabaseLayer.fs"
#load "YahooStockAPI.fs"
#load "AlgoPortfolio.fs"
#load "AlgoStockTrader.fs"

open System
open FinanceLibrary
open YahooFinanceAPI.Stock
open AlgorithmicTrading.AlgoPortfolio
open DatabaseLayer
open AlgorithmicTrading.MomentumVWAP

 /// EXECUTION
 let execute = 
  let stockService = new GetStockDataWeb() :> IStockService
  // Get historical stock prices for the symbol
  let symbol = "MSFT"
  let backTestPeriod = 2000

  let prices = stockService.GetStockPrices symbol backTestPeriod
  let p = new Portfolio(10000M, DateTime.Today)
  let trader = new Trader(p, symbol, prices)
  trader.BackTest()