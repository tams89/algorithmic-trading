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
    open YahooFinanceAPI.Stock
    open AlgorithmicTrading.AlgoPortfolio
    open DatabaseLayer
    
    /// TRADER
    type Trader(portfolio : Portfolio, symbol : string, prices : Tick []) = 
        class
           
            /// Calculated using mean high low close.
            let volumeWeightedAvgPrice (prices : Tick []) (period : float) = 
                let ticksInPeriod = 
                    /// get data within period relative to now.
                    prices 
                    |> Array.filter 
                           (fun x -> 
                           x.Date <= DateTime.Today.AddDays(-period) || x.Date >= DateTime.Today.AddDays(+period))
                /// Sum price times volume per trade
                (ticksInPeriod |> Array.sumBy (fun x -> ((x.High + x.Low + x.Close) / 3.0M) * x.Volume)) 
                / /// Sum volume over whole period
                (ticksInPeriod |> Array.sumBy (fun x -> x.Volume))
            
            /// Place order / make trade
            member private this.PlaceOrder(order : Order) = portfolio.AddPosition(order)
            
            /// Close position
            member private this.CloseShortPosition(order) = portfolio.CloseShortPositions(order)
            
            /// The algorithm
            member private this.IncomingTick(tick : Tick) = 
                
                // Check that a valid price has come in.
                if tick.Low <> 0.0M then ()
//                printfn "Current tick price %A, Date %A" tick.Low tick.Date
                
                let currentPrice = tick.Low
                // Update portfolio with latest price information.
                portfolio.CurrentPrice <- currentPrice
                /// Limit the exposure on open positions.
                let maxlimit = portfolio.Cash + 0.1M
                let minlimit = - abs (portfolio.Cash * 0.9M)
                printfn "MinLimit %A, MaxLimit %A" minlimit maxlimit

                let calcVwap = volumeWeightedAvgPrice prices 3.0
//                printfn "Current Volume Weighted Average Price = %A" calcVwap

                /// Shares limit to buy/sell
                let numOfShares = floor (portfolio.Cash / currentPrice)
                printfn "Share sell/buy limit = %A" numOfShares

                /// if the stocks price less than the vwap by 0.5% and the limit has not been exceeded.
                if currentPrice < (calcVwap * 0.995M) && (portfolio.ShortPositionsValue > minlimit) then 
                    let order = 
                        { Symbol = symbol
                          Quantity = -int numOfShares
                          OrderType = Short
                          Value = -numOfShares * currentPrice
                          Covered = false }
                    this.PlaceOrder(order)
//                    printfn "Short position assumed. Shares = %A, Price = %A" numOfShares currentPrice

                /// if the stock price has increased by 0.1% to the vwap and we havent reached exposure limit then buy.
                elif currentPrice > (calcVwap * 1.001M) && (portfolio.PositionsValue < maxlimit) then 
                    let order = 
                        { Symbol = symbol
                          Quantity = +int numOfShares
                          OrderType = Long
                          Value = +numOfShares * currentPrice
                          Covered = false }
                    this.PlaceOrder(order)
//                    printfn "Long position assumed. Shares = %A, Price = %A" numOfShares currentPrice

                /// If there any shorts where the market value has risen close to the the initial shorting value 
                /// then close the positions.
                else 
                    if not portfolio.ShortPositions.IsEmpty then 
                        let shortsToClose = 
                            portfolio.ShortPositions
                            // current price is greater than the short price by 1%.
                            |> Seq.filter (fun x -> not x.Covered &&  abs (x.Value / decimal x.Quantity) > currentPrice * 0.99M) 
                        for short in shortsToClose do
                            let order = 
                                { Symbol = short.Symbol
                                  Quantity = abs short.Quantity
                                  OrderType = Cover
                                  Value = abs (decimal short.Quantity) * currentPrice
                                  Covered = true }
                            portfolio.CloseShortPositions(short, order)
//                    printfn "Short Covered. Shares = %A, Price = %A" numOfShares currentPrice
            
            /// Backtest uses historical data to simulate returns.
            member this.BackTest() = 

                printfn "Algorithm Started."

                for tick in prices do
                 let filterCrappyData data = 
                  match data with 
                   | x when not(String.IsNullOrEmpty(tick.Date.ToString())) -> Some x
                   | x when tick.High > 0M && tick.Low > 0M && tick.Close > 0M && tick.Volume > 0M -> Some x
                   | _ -> None
                 if (filterCrappyData tick).IsSome then 
                  this.IncomingTick(tick)
                 else printfn "Crappy data detected %A" tick

                printfn "Shorted %A Shares" (portfolio.Positions
                                             |> Seq.filter (fun x -> x.OrderType = Short)
                                             |> Seq.sumBy (fun x -> x.Quantity))

                printfn "Bought (Long) %A Shares" (portfolio.Positions
                                                   |> Seq.filter (fun x -> x.OrderType = Long)
                                                   |> Seq.sumBy (fun x -> x.Quantity))

                printfn "Covered %A Shorts" (portfolio.Positions
                                             |> Seq.filter (fun x -> x.OrderType = Cover)
                                             |> Seq.sumBy (fun x -> x.Quantity))

                printfn "%A" portfolio

                printfn "Algorithm Ended."
        end