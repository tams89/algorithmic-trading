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

    open System
    open Microsoft.FSharp.Collections
    open FinanceLibrary
    open Records
    open AlgorithmicTrading.AlgoPortfolio
    open AlgorithmicTrading.AlgoCalculation
    
    /// TRADER
    type Trader(symbol : string, portfolio : Portfolio, calculator : Calculation) = class
     
     // Field containing previous 10 ticks.
     let ticksToday = new System.Collections.Concurrent.ConcurrentQueue<Tick>()

     // Allows the addition of a tick to the queue.
     member private this.AddTickToQueue(tick) = 
       if ticksToday.Count >= 10 then ignore(ticksToday.TryDequeue(ref tick))
       ticksToday.Enqueue(tick)

     member private this.CalculateVWAP (tickDate : DateTime, period) = 
      let rangePrices = ticksToday.ToArray() |> Array.filter (fun x -> x.Date <= tickDate.AddDays(-period)) 
      if rangePrices.Length > 0 then calculator.VWAP(PSeq.toArray rangePrices)
      else 0M

     /// Makes a trade
     member private this.PlaceOrder(symbol, date, quantity, price, orderType) = 
      let orderRecord = 
       match orderType with
       | Long -> { Symbol = symbol; 
                   Date = date;
                   Quantity = quantity;
                   OrderType = Long;
                   Value = quantity * price; }

       | Short -> { Symbol = symbol; 
                    Date = date;
                    Quantity = - quantity;
                    OrderType = Short;
                    Value = - quantity * price; }

       | Cover -> { Symbol = symbol; 
                    Date = date;
                    Quantity = quantity;
                    OrderType = Cover;
                    Value = - quantity * price; }
      
      portfolio.AddPosition(orderRecord)
     
     /// Closes a position.
     member private this.ClosePosition(order) = portfolio.ClosePosition(order)

     /// THE ALGORITHM
     member this.IncomingTick(tick, shortVwap, longVwap, coverBarrier, minLimit, maxLimit, coverAfter, vwapPeriod) = 
         
         // Add tick to record queue.
         this.AddTickToQueue(tick)
         
         // Update portfolio with latest price information.
         let currentPrice = tick.Low
         portfolio.CurrentPrice <- currentPrice

         // Calculate the latest Volume-Weighted-Average-Price.
         let calcVwap = this.CalculateVWAP(tick.Date, vwapPeriod)

         if calcVwap > 1M then

          // Shares limit to buy/sell per trade.
          let numOfShares = floor (portfolio.Cash / calcVwap)

          // SHORT
          /// if the stocks price less than the vwap by 0.5% and the limit has not been exceeded.
          if currentPrice < (calcVwap * shortVwap) && (portfolio.PositionsValue > minLimit) then 
           this.PlaceOrder(symbol, tick.Date, numOfShares, currentPrice, Short)
          
          /// LONG
          /// if the stock price has increased by 0.1% to the vwap and we havent reached exposure limit then buy.
          elif currentPrice > (calcVwap * longVwap) && (portfolio.PositionsValue < maxLimit) then 
           this.PlaceOrder(symbol, tick.Date, numOfShares, currentPrice, Long)
          
          /// COVER SHORT
          /// If there any shorts where the market value has risen close to the the initial shorting value 
          /// then close the positions.
          elif not (portfolio.ShortPositions |> Seq.isEmpty) then
           portfolio.ShortPositions 
           |> Seq.filter (fun x -> x.Value < 0M && abs (x.Value / decimal x.Quantity) > currentPrice * coverBarrier)
           |> Seq.iter (fun short -> this.PlaceOrder(short.Symbol, tick.Date, (abs short.Quantity), currentPrice, Cover)
                                     this.ClosePosition(short))
          
          /// COVER SHORT after period
          elif not (portfolio.ShortPositions |> Seq.isEmpty) then
           portfolio.ShortPositions 
           |> Seq.filter (fun x -> x.Date.Date.AddDays(coverAfter) > tick.Date.Date)
           |> Seq.iter (fun short -> this.PlaceOrder(short.Symbol, tick.Date, (abs short.Quantity), currentPrice, Cover)
                                     this.ClosePosition(short))
          
          /// COVER LONG after period
          elif not (portfolio.LongPositions |> Seq.isEmpty) then
           portfolio.LongPositions 
           |> Seq.filter (fun x -> x.Date.Date.AddDays(coverAfter) > tick.Date.Date)
           |> Seq.iter (fun long -> this.ClosePosition(long))

    end