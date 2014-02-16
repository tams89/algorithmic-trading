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

module AlgoStockTrader = 

 open System
 open FinanceLibrary.YahooAPIStockChart

 type OrderType = Long | Short

 type Order = 
  { Symbol:string
    Quantity:int
    OrderType:OrderType
    Value:decimal }

 type Portfolio(startingCash, startDate) = class
  let mutable cash = 0M 
  let mutable profitAndLoss = 0M 
  let mutable positions = new System.Collections.Generic.HashSet<Order>() 
  let mutable positionsValue = new System.Collections.Generic.Dictionary<string,decimal>() 
  let mutable portfolioValue = 0M 
  let mutable returns = 0M 

  member x.StartDate = startDate /// The starting date of the portfolio.
  member x.StartingCash = startingCash /// The date this portfolio was created.

  /// Current amount of available cash.
  member x.Cash
   with get() = cash
   and set(value) = cash <- value
  
  /// Total profit and loss up until the current time.
  member x.ProfitAndLoss
   with get() = profitAndLoss <- (cash + (positionsValue.Values |> Seq.sum)) - startingCash
  
  /// Dictionary of open positions (i.e. Stock GOOG 124).
  member x.Positions
   with get() = positions

  /// Total value of the open positions.
  member x.PositionsValue
   with get() = positionsValue.Values |> Seq.sum

  /// Sum of positionsValue and cash.
  member x.PortfolioValue 
   with get() = portfolioValue <- cash + (positionsValue.Values |> Seq.sum)

  /// Cumulative percentage returns for entire portfolio up until now. Pentage gain or loss of start cash.
  member x.Returns
   with get() = returns <- profitAndLoss / startingCash

  member x.AddPosition(value) = positions.Add(value)
  member x.RemovePosition(value) = positions.Remove(value)

 end

 type Trader (portfolio:Portfolio, symbol:string) = class

  // Get historical stock prices for the symbol
  let prices = getStockPrices symbol 60

  // Limit the exposure on open positions.
  let limit = portfolio.Cash * 0.8M

  // Calculated using mean high low close.
  let volumeWeightedAvgPrice (prices:Tick[]) (period:float) = 
    let ticksInPeriod =
     prices 
     |> Array.filter (fun x -> x.Date <= DateTime.Today.AddDays(-period) || x.Date >= DateTime.Today.AddDays(+period)) // get data within period relative to now.
    (ticksInPeriod |> Array.sumBy (fun x-> ((x.High + x.Low + x.Close)/3.0M) * x.Volume)) // Sum price times volume per trade
     / (ticksInPeriod |> Array.sumBy (fun x -> x.Volume)) // Sum volume over whole period

  /// Place order / make trade
  member x.PlaceOrder(order:Order) =
    let placed = portfolio.AddPosition(order)
    printfn "Order placed: %A, successful: %A" order placed
  
  /// Call on incoming data
  member x.IncomingTick(tick:Tick) = 
   // TODO Trading logic meat in here
   let currentPrice = tick.Close
   
   let calcVwap = volumeWeightedAvgPrice prices 3.0

   // if the stocks price less than the vwap by 0.5% and the limit has not been exceeded.
   if tick.Close < (calcVwap * 0.995M) && (portfolio.PositionsValue - limit > 0.0M) then
    let order : Order = { Symbol = symbol; Quantity = -100; OrderType = Short; Value =  100M * tick.Close }
    x.PlaceOrder(order)
   // if the stock price has increased by 0.1% to the vwap and we havent reached exposure limit then buy.
   elif tick.Close > calcVwap * 1.001M && (portfolio.PositionsValue - limit > 0.0M) then
    let order : Order = { Symbol = symbol; Quantity = +100; OrderType = Long; Value =  100M * tick.Close }
    x.PlaceOrder(order)

 end