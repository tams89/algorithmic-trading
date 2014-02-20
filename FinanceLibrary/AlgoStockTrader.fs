namespace FinanceLibrary.AlgorithmicTrading
(*
    Simple Momentum Algorithm
    1. Calulate the volume weighted average i.e. 3 days/5 days.
    2. Keep the current price of the security.
    3. Portfolio object that calculates how long or short you are. (positions * price)
    4. Conditions where if the value of the security is .5% less than vwap and max short has not been reached
       then call order to sell 100 shares short.
    5. Conversely if the stock is .1% higher and we havent reached max long then call order and buy 100 shares.
*)
module MomentumVWAP = 
    open System
    open Microsoft.FSharp.Collections
    open FinanceLibrary
    open FinanceLibrary.Records
    open FinanceLibrary.Interfaces
    open YahooFinanceAPI.Stock
    open AlgorithmicTrading.AlgoPortfolio
    open DatabaseLayer

    let console (item,value) = 
     printfn "Current of %A is %A" item value

    /// TRADER
    type Trader (portfolio : Portfolio, logger : WriteIterationData, symbol : string, prices : Tick []) = class

            /// Calculated using mean high low close.
            let volumeWeightedAvgPrice (prices : Tick []) (period : float) = 

             let pricesInRange = 
              prices 
              |> Seq.toArray
              |> Array.filter (fun x -> x.Date >= prices.[prices.GetUpperBound(0)].Date.AddDays(-period))

             let rec SumTradePriceVolume sum volSum counter = 
               let limit = pricesInRange.Length
               if counter < limit then 
                let tick = pricesInRange.[counter]
                let tickPrice = (tick.High + tick.Low + tick.Close ) / 3.0M
                SumTradePriceVolume (sum + tickPrice * tick.Volume) (volSum + tick.Volume)  (counter + 1)
               else sum / volSum

             let vwap = SumTradePriceVolume 0M 0M 0
             vwap
            
            /// Place order / make trade
            member private this.PlaceOrder(symbol, date, quantity, price, orderType) = 
             let orderRecord = 
              match orderType with
              | Long -> { Symbol = symbol; 
                          Date = date;
                          Quantity = quantity;
                          OrderType = Long;
                          Value = decimal(quantity) * price; }

              | Short -> { Symbol = symbol; 
                           Date = date;
                           Quantity = - quantity;
                           OrderType = Short;
                           Value = - decimal(quantity) * price; }

              | Cover -> { Symbol = symbol; 
                           Date = date;
                           Quantity = quantity;
                           OrderType = Cover;
                           Value = - decimal(quantity) * price; }
             
             portfolio.AddPosition(orderRecord)

            member private this.ClosePosition(order) = portfolio.ClosePosition(order)

            /// THE ALGORITHM
            /// Cover barrier = 0.99M
            member private this.IncomingTick(tick, shortVwap, longVwap, coverBarrier, minLimit, maxLimit, numShares, vwapPeriod) = 

                // Update with latest price information.
                let currentPrice = tick.Low
                portfolio.CurrentPrice <- currentPrice
                let calcVwap = volumeWeightedAvgPrice prices vwapPeriod
                
                // SHORT
                /// if the stocks price less than the vwap by 0.5% and the limit has not been exceeded.
                if currentPrice < (calcVwap * shortVwap) && (portfolio.PositionsValue > minLimit) then 
                 this.PlaceOrder(symbol, tick.Date, numShares, currentPrice, Short)
                
                /// LONG
                /// if the stock price has increased by 0.1% to the vwap and we havent reached exposure limit then buy.
                elif currentPrice > (calcVwap * longVwap) && (portfolio.PositionsValue < maxLimit) then 
                 this.PlaceOrder(symbol, tick.Date, numShares, currentPrice, Long)

                /// COVER
                /// If there any shorts where the market value has risen close to the the initial shorting value 
                /// then close the positions.
                elif not portfolio.ShortPositions.IsEmpty then
                 portfolio.ShortPositions 
                 |> Seq.filter (fun x -> x.Value < 0M && abs (x.Value / decimal x.Quantity) > currentPrice * coverBarrier)
                 |> Seq.iter (fun (short) -> this.PlaceOrder(short.Symbol, tick.Date, (abs short.Quantity), currentPrice, Cover)
                                             this.ClosePosition(short))

                /// COVER Daily
                /// If there any shorts where the market value has risen close to the the initial shorting value 
                /// then close the positions.
                elif not portfolio.ShortPositions.IsEmpty then
                 portfolio.ShortPositions 
                 |> Seq.filter (fun x -> tick.Date > x.Date.AddDays(5.0))
                 |> Seq.iter (fun (short) -> this.PlaceOrder(short.Symbol, tick.Date, (abs short.Quantity), currentPrice, Cover)
                                             this.ClosePosition(short))

            /// Backtest using historical data to simulate returns.
            member this.BackTest () = 
                printfn "Algorithm Started."

                let logRecs = 
                 new System.Collections.Generic.List<decimal*decimal*decimal*int*int*decimal*decimal*decimal*decimal*decimal*string>()

                // Filter any useless or erroneous data.
                let cleanPrices = 
                 let limit = prices.GetUpperBound(0)
                 let rec tickList list counter =
                  match prices.[counter] with 
                  | tick when counter < limit && tick.High >= 1M && tick.Low >= 1M && tick.Close >= 1M && tick.Volume >= 1M 
                    -> tickList (tick::list) (counter + 1)
                  | tick when counter = limit -> list
                  | _ -> tickList list (counter + 1)
                 tickList List.empty<Tick> 0

                let executeRun shortVwap longVwap coverBarrier minLimit maxLimit numShares vwapPeriod = 
                 // Execute trading algorithm on the historical data.
                 cleanPrices |> Seq.iter (fun tick -> this.IncomingTick(tick, shortVwap, longVwap, coverBarrier, minLimit, maxLimit, numShares, vwapPeriod))
                 portfolio 

                // Store the constant iterated over, and portfolio results.
                let addToLog (portfolio:Portfolio, iterateVal:decimal, whatIterated:string)  =
                 logRecs.Add(
                  (portfolio.StartingCash,
                   portfolio.Cash,
                   portfolio.PortfolioValue, 
                   portfolio.Positions.Count, 
                   portfolio.ShortPositions.Length, 
                   portfolio.PositionsValue, 
                   portfolio.ShortPositionsValue, 
                   portfolio.Returns, 
                   portfolio.ProfitAndLoss, 
                   iterateVal, 
                   whatIterated))
                 (whatIterated,iterateVal)
                
                let shortVwap = 0.998M                // percentage of vwap to allow short position.
                let longVwap = 1.001M                 // percentage of vwap to allow long position.
                let coverBarrier = 0.99M              // percentage of current price to begin covering at.
                let minlimit = - (abs portfolio.Cash) // must be negative, used for short positions.
                let maxlimit = portfolio.Cash + 0.1M  // must be postive, used for long positions.
                let numOfShares = 100M                // Shares limit to buy/sell
                let vwapPeriod = 5.0                  // Period of days to use to calculate vwap.

                // Iterate constants
                [ 0.000M..0.005M..2.000M ] // shortVwap list to iterate
                |> Seq.iter (fun i -> 
                    ((executeRun i longVwap coverBarrier minlimit maxlimit numOfShares vwapPeriod), i, "ShortVwap") 
                    |> addToLog 
                    |> console)

                // Insert collection of log data to database.
                logger.InsertIterationData(logRecs)
                logger.Commit()

                printfn "Algorithm Ended."
        end

    /// EXECUTION
    let execute = 
     let stockService = new GetStockDataWeb() :> IStockService
     // Get historical stock prices for the symbol
     let symbol = "MSFT"
     let backTestPeriod = 600
     let prices = stockService.GetStockPrices symbol backTestPeriod
     let l = new WriteIterationData()
     let p = new Portfolio(10000M, DateTime.Today)
     let trader = new Trader(p, l, symbol, prices)
     trader.BackTest()