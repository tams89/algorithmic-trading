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

    /// CONSTANTS TO BE ITERATED FOR OPTIMAL VALUE IN CAPS
    let mutable shortVwap = 0M
    
    /// TRADER
    type Trader (portfolio : Portfolio, logger : WriteIterationData, symbol : string, prices : Tick []) = 
        class

            /// Calculated using mean high low close.
            let volumeWeightedAvgPrice (prices : Tick []) (period : float) = 
                let ticksInPeriod = 
                    prices /// Get data within period relative to now.
                    |> PSeq.filter (fun x -> x.Date >= prices.[prices.GetUpperBound(0)].Date.AddDays(-period))
                    |> PSeq.toArray
                (ticksInPeriod /// Sum price times volume per trade
                 |> Array.sumBy (fun x -> ((x.High + x.Low + x.Close) / 3.0M) * x.Volume))
                 / 
                (ticksInPeriod /// Sum volume over whole period
                 |> Array.sumBy (fun x -> x.Volume))
            
            /// Place order / make trade
            member private this.PlaceOrder (symbol, date, quantity, price, orderType) = 
             let orderRecord = 
              match orderType with
              | Long -> { Symbol = symbol; 
                          Date = date;
                          Quantity = quantity;
                          OrderType = Long;
                          Value = (decimal quantity) * price; }
              | Short -> { Symbol = symbol; 
                           Date = date;
                           Quantity = - quantity;
                           OrderType = Short;
                           Value = - (decimal quantity) * price; }
              | Cover -> { Symbol = symbol; 
                           Date = date;
                           Quantity = quantity;
                           OrderType = Cover;
                           Value = - (decimal quantity) * price; }
             
             portfolio.AddPosition orderRecord

            // Sets the value of short to positive as  to represent the gained profit.
            member private this.ClosedShortOrder shortOrder = portfolio.CloseShortPositions shortOrder
            
            /// The algorithm
            member private this.IncomingTick (tick : Tick) = 

                // Update with latest price information.
                let currentPrice = tick.Low
                portfolio.CurrentPrice <- currentPrice
                
                /// Limit the exposure on open positions.
                let maxlimit = portfolio.Cash + 0.1M
                let minlimit = - portfolio.Cash
                let calcVwap = volumeWeightedAvgPrice prices 3.0
                
                /// Shares limit to buy/sell
                let numOfShares = 100M

                // SHORT
                /// if the stocks price less than the vwap by 0.5% and the limit has not been exceeded.
                if currentPrice < (calcVwap * shortVwap) && (portfolio.PositionsValue > minlimit) then 
                 this.PlaceOrder(symbol, tick.Date, numOfShares, currentPrice, Short)
                
                /// LONG
                /// if the stock price has increased by 0.1% to the vwap and we havent reached exposure limit then buy.
                elif currentPrice > (calcVwap * 1.001M) && (portfolio.PositionsValue < maxlimit) then 
                 this.PlaceOrder(symbol, tick.Date, numOfShares, currentPrice, Long)

                /// COVER
                /// If there any shorts where the market value has risen close to the the initial shorting value 
                /// then close the positions.
                elif not portfolio.ShortPositions.IsEmpty then
                 portfolio.ShortPositions 
                 |> Seq.filter (fun x -> x.Value < 0M && abs (x.Value / decimal x.Quantity) > currentPrice * 0.99M)
                 |> Seq.iter (fun (short) -> this.PlaceOrder(short.Symbol, tick.Date, (abs short.Quantity), currentPrice, Cover)
                                             this.ClosedShortOrder(short))

                /// COVER Daily
                /// If there any shorts where the market value has risen close to the the initial shorting value 
                /// then close the positions.
                elif not portfolio.ShortPositions.IsEmpty then
                 portfolio.ShortPositions 
                 |> Seq.filter (fun x -> tick.Date > x.Date.AddDays(5.0))
                 |> Seq.iter (fun (short) -> this.PlaceOrder(short.Symbol, tick.Date, (abs short.Quantity), currentPrice, Cover)
                                             this.ClosedShortOrder(short))

            /// Backtest uses historical data to simulate returns.
            member this.BackTest () = 

                // Filter any useless or erroneous data.
                let filterCrappyData (tick:Tick) = 
                      match tick with
                      | x when not (String.IsNullOrEmpty(tick.Date.ToString())) && 
                               tick.High >= 1M && 
                               tick.Low >= 1M && 
                               tick.Close >= 1M && 
                               tick.Volume >= 1M
                                -> true
                      | _ -> false

                // Cleaned list of tick data.
                let prices = prices
                             |> Seq.filter (fun tick -> filterCrappyData tick) 
                             |> Seq.toArray
                
                // Backtest execute block
                let executeRun x = 
                 
                 // Set the value to the constant being iterated for optimsation.
                 shortVwap <- x

                 printfn "Algorithm Started."
                 
                 // Execute trading algorithm on the historical data.
                 prices |> Seq.iter (fun tick -> this.IncomingTick(tick))

                 // Store the constant iterated over, and portfolio results.
                 logger.InsertIterationData(portfolio.StartingCash,
                                            portfolio.Cash,
                                            portfolio.PortfolioValue, 
                                            portfolio.Positions.Length, 
                                            portfolio.ShortPositions.Length, 
                                            portfolio.PositionsValue, 
                                            portfolio.ShortPositionsValue, 
                                            portfolio.Returns, 
                                            portfolio.ProfitAndLoss, 
                                            x, 
                                            "ShortVWAPPercentage")

                 let positionQuantity orderType = 
                  portfolio.Positions 
                  |> Seq.filter (fun x -> x.OrderType = orderType) 
                  |> Seq.sumBy (fun x -> x.Quantity)
                 printfn "Shorted %A Shares" (positionQuantity Short)
                 printfn "Covered %A Shorts" (positionQuantity Cover)
                 printfn "Bought (Long) %A Shares" (positionQuantity Long)
                 printfn "%A" portfolio
                 printfn "Date Range %A to %A" prices.[0].Date prices.[prices.GetUpperBound(0)].Date

                // Iterate constants
                [ 0.6000M..0.0020M..1.000M ] 
                |> PSeq.ordered
                |> PSeq.iter executeRun 
                
                printfn "Algorithm Ended."

        end

    /// EXECUTION
    let execute = 
     let stockService = new GetStockDataDB() :> IStockService
     // Get historical stock prices for the symbol
     let symbol = "MSFT"
     let backTestPeriod = 10000
     let prices = stockService.GetStockPrices symbol backTestPeriod
     let l = new WriteIterationData()
     let p = new Portfolio(10000M, DateTime.Today)
     let trader = new Trader(p, l, symbol, prices)
     trader.BackTest()