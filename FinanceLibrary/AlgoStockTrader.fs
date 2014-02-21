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
module AlgoTrader = 
    
    open FinanceLibrary
    open Records
    open AlgorithmicTrading.AlgoPortfolio
    open AlgorithmicTrading.AlgoCalculation

    let console (item,value) = 
     printfn "Current of %A is %A" item value
    
    /// TRADER
    type Trader(symbol : string, portfolio : Portfolio) = class

     /// Makes a trade
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

     member private this.PlaceCoverOrder(order) = portfolio.ClosePosition(order)

     /// THE ALGORITHM
     member this.IncomingTick(tick, shortVwap, longVwap, coverBarrier, minLimit, maxLimit, numShares, coverAfter, calcVwap) = 
         
         // Update with latest price information.
         let currentPrice = tick.Low
         portfolio.CurrentPrice <- currentPrice
         
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
         elif not (portfolio.ShortPositions |> Seq.isEmpty) then
          portfolio.ShortPositions 
          |> Seq.filter (fun x -> x.Value < 0M && abs (x.Value / decimal x.Quantity) > currentPrice * coverBarrier)
          |> Seq.iter (fun (short) -> this.PlaceOrder(short.Symbol, tick.Date, (abs short.Quantity), currentPrice, Cover)
                                      this.PlaceCoverOrder(short))

         /// COVER after period
         /// If there any shorts where the market value has risen close to the the initial shorting value 
         /// then close the positions.
         elif not (portfolio.ShortPositions |> Seq.isEmpty) then
          portfolio.ShortPositions 
          |> Seq.filter (fun x -> tick.Date > x.Date.AddDays(coverAfter))
          |> Seq.iter (fun (short) -> this.PlaceOrder(short.Symbol, tick.Date, (abs short.Quantity), currentPrice, Cover)
                                      this.PlaceCoverOrder(short))
    end