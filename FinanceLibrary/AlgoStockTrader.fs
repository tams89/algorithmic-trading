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
 open FinanceLibrary.YahooFinanceAPI.Stock.YahooStockAPI
 open FinanceLibrary.AlgorithmicTrading.AlgoPortfolio
 
 /// TRADER
 type Trader (portfolio:Portfolio, symbol:string, backTestPeriod) = class
  // Get historical stock prices for the symbol
  let prices = getStockPrices symbol backTestPeriod

  // Limit the exposure on open positions.
  let maxlimit = portfolio.StartingCash + 0.1M
  let minlimit = -portfolio.StartingCash 

  // Calculated using mean high low close.
  let volumeWeightedAvgPrice (prices:Tick[]) (period:float) = 
    let ticksInPeriod =
     prices 
     |> Array.filter (fun x -> x.Date <= DateTime.Today.AddDays(-period) || x.Date >= DateTime.Today.AddDays(+period)) // get data within period relative to now.
    (ticksInPeriod |> Array.sumBy (fun x-> ((x.High + x.Low + x.Close)/3.0M) * x.Volume)) // Sum price times volume per trade
     / (ticksInPeriod |> Array.sumBy (fun x -> x.Volume)) // Sum volume over whole period

  /// Place order / make trade
  member private x.PlaceOrder(order:Order) = portfolio.AddPosition(order)

  /// Close position
  member private x.CloseShortPosition(order) = portfolio.CloseShortPositions(order)
  
  /// Call on incoming data
  member private x.IncomingTick(tick:Tick) = 
   let currentPrice = tick.Low
   let calcVwap = volumeWeightedAvgPrice prices 3.0

   // Shares limit to buy/sell
   let numOfShares = 5M

   // if the stocks price less than the vwap by 0.5% and the limit has not been exceeded.
   if currentPrice < (calcVwap * 0.995M) && (portfolio.PositionsValue > minlimit) then
    let order = { Symbol = symbol; Quantity = - int numOfShares; OrderType = Short; Value = -numOfShares * currentPrice; Covered = false }
    x.PlaceOrder(order)

   // if the stock price has increased by 0.1% to the vwap and we havent reached exposure limit then buy.
   elif currentPrice > (calcVwap * 1.001M) && (portfolio.PositionsValue < maxlimit) then
    let order = { Symbol = symbol; Quantity = + int numOfShares; OrderType = Long; Value = +numOfShares * currentPrice; Covered = false }
    x.PlaceOrder(order)

   // Close any short positions when the market starts to rise again.
   // If there any shorts where the market value has risen close to the the initial shorting value then close the position.
   elif not portfolio.ShortPositions.IsEmpty then
    let shortsToClose = portfolio.ShortPositions |> Seq.filter (fun y -> not y.Covered && abs(y.Value / numOfShares) >= 0.8M * currentPrice) |> Seq.toList
    for short in shortsToClose do 
     let order = { Symbol = short.Symbol; Quantity = abs short.Quantity; OrderType = Cover; Value = abs (decimal short.Quantity) * currentPrice; Covered = true }
     portfolio.CloseShortPositions(short,order)

  member x.BackTest () = 
   printfn "Algorithm Started."
   
   for tick in prices do 
    portfolio.CurrentPrice <- prices.[prices.Length - 1].Close // Assign most recent price for position end value calculations.
    x.IncomingTick(tick)

   printfn "Shorted %A Shares" (portfolio.Positions |> Seq.filter (fun y -> y.OrderType = Short) |> Seq.sumBy (fun y -> y.Quantity))
   printfn "Bought (Long) %A Shares" (portfolio.Positions |> Seq.filter (fun y -> y.OrderType = Long) |> Seq.sumBy (fun y -> y.Quantity))
   printfn "Covered %A Shorts" (portfolio.Positions |> Seq.filter (fun y -> y.OrderType = Cover) |> Seq.sumBy (fun y -> y.Quantity))
   printfn "Algorithm Ended."

  /// Default constructor for Google, 4 years back, trading limits are $startingCash + 0.1 to -$startingCash.
  new (portfolio) = Trader(portfolio, "GOOG", 1000) 
 end

 /// EXECUTION
 let execute = 
  let p = new Portfolio(10000M, DateTime.Today)
  let trader = new Trader(p)
  trader.BackTest()