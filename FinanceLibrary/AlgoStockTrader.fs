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
    open AlgorithmicTrading.AlgoMarket
    
    /// TRADER
    type Trader(symbol : string, portfolio : Portfolio, calculator : Calculation, market : Market) = class
     
     // Field containing previous 10 ticks.
     let ticksToday = new System.Collections.Concurrent.ConcurrentQueue<Tick>()

     // Allows the addition of a tick to the queue. Only maintains the latest 10 ticks.
     member private this.AddTickToQueue(tick) = 
       if ticksToday.Count >= 10 then ignore(ticksToday.TryDequeue(ref tick))
       ticksToday.Enqueue(tick)

     member private this.CalculateVWAP (tickDate : DateTime, period) = 
      let rangePrices = ticksToday |> Seq.filter (fun x -> x.Date <= tickDate.AddDays(-period))
      calculator.VWAP(rangePrices |> Seq.toArray)

     /// Makes a trade
     member private this.PlaceOrder(tick : Tick, quantity, price, orderType) = 
      
      let orderRecord = 
       let order symbol date quantity orderType value = 
        { Symbol = symbol; Date = date; Quantity = quantity; OrderType = orderType; Value = value  } 
       match orderType with
       | Long -> order symbol tick.Date quantity Long (quantity * price)
       | Short -> order symbol tick.Date (-quantity) Short (- quantity * price)
       | Cover -> order symbol tick.Date (quantity) Cover (- quantity * price)

      match market.PricesShouldSlip with 
      | true -> let slipped = market.GenerateSlippage(orderRecord, tick) 
                slipped |> Seq.iter (fun x -> portfolio.AddPosition(x))
      | false -> portfolio.AddPosition(orderRecord)
     
     /// Closes a position.
     member private this.ClosePosition(order) = portfolio.ClosePosition(order)

     /// THE ALGORITHM
     member this.IncomingTick(tick, shortVwap, longVwap, coverBarrier, coverAfter, vwapPeriod) = 
         
         // Add tick to record queue.
         this.AddTickToQueue(tick)
         
         // Update portfolio with latest price information.
         let currentPrice = tick.Low
         portfolio.CurrentPrice <- currentPrice

         let minLimit = - portfolio.StartingCash                          // must be negative, used for short positions.
         let maxLimit= portfolio.StartingCash + 0.1M                      // must be postive, used for long positions.
         let calcVwap = this.CalculateVWAP(tick.Date, vwapPeriod)         // Calculate the latest Volume-Weighted-Average-Price.

         // Shares limit to buy/sell per trade.
         // Allows major increase in returns, wonder if real market will be liquid enough for the larger transactions.
         // Perhaps the algorithm should inspect market liquidity or have a record or the markets current liquidity.
         // For large orders the order could chunked into smaller pieces with a delay between each order. 
         let numOfShares = 
          match calcVwap with
          | x when x > 0M -> floor (List.min [ (tick.Volume * 0.1M); (portfolio.Cash / calcVwap) ])
          | _ -> 0M

         if numOfShares > 1M then
          
          // Calculate maximum order capital available.
          let cashLimit = portfolio.Cash > ceil(numOfShares * currentPrice)

          // SHORT
          /// if the stocks price less than the vwap by 0.5% and the limit has not been exceeded.
          if currentPrice < (calcVwap * shortVwap) && (portfolio.PositionsValue > minLimit) && cashLimit then 
           this.PlaceOrder(tick, numOfShares, currentPrice, Short)
          
          /// LONG
          /// if the stock price has increased by 0.1% to the vwap and we havent reached exposure limit then buy.
          elif currentPrice > (calcVwap * longVwap) && (portfolio.PositionsValue < maxLimit) && cashLimit then 
           this.PlaceOrder(tick, numOfShares, currentPrice, Long)
          
          /// COVER SHORT
          /// If there any shorts where the market value has risen close to the the initial shorting value 
          /// then close the positions.
          elif Seq.length portfolio.ShortPositions > 0 then
           portfolio.ShortPositions 
           |> Seq.filter (fun x -> abs (x.Value / decimal x.Quantity) > currentPrice * coverBarrier)
           |> Seq.iter (fun short -> this.PlaceOrder(tick, (abs short.Quantity), currentPrice, Cover)
                                     this.ClosePosition(short))
          
          /// COVER SHORT after period
          elif Seq.length portfolio.ShortPositions > 0 then
           portfolio.ShortPositions 
           |> Seq.filter (fun x -> x.Date.AddDays(coverAfter) > tick.Date)
           |> Seq.iter (fun short -> this.PlaceOrder(tick, (abs short.Quantity), currentPrice, Cover)
                                     this.ClosePosition(short))
          
          /// COVER LONG after period
          elif Seq.length portfolio.LongPositions > 0 then
           portfolio.LongPositions 
           |> Seq.filter (fun x -> x.Date.AddDays(coverAfter) > tick.Date)
           |> Seq.iter (fun long -> this.ClosePosition(long))
          
    end