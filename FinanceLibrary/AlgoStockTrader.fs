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
 open System.Net

 let url = "http://ichart.finance.yahoo.com/table.csv?s="

 type Tick = 
  { Date:DateTime
    Open:decimal
    High:decimal
    Low:decimal
    Close:decimal
    Volume:decimal
    AdjClose:decimal }
 
 /// Returns prices (as tuple) of a given stock for a 
 /// specified number of days (starting from the most recent)
 let getStockPrices stock count =
  // Download the data and split it into lines
  let wc = new WebClient()
  let data = wc.DownloadString(url + stock)
  let dataLines = 
      data.Split([| '\n' |], StringSplitOptions.RemoveEmptyEntries) 
 
  // Parse lines of the CSV file and take specified
  // number of days using in the oldest to newest order
  seq { for line in dataLines |> Seq.skip 1 do
            let infos = line.Split(',')
            yield { Date = DateTime.Parse infos.[0]
                    Open = decimal infos.[1]
                    High = decimal infos.[2]
                    Low = decimal infos.[3]
                    Close = decimal infos.[4]
                    Volume = decimal infos.[5]
                    AdjClose = decimal infos.[6] } }
   |> Seq.take count |> Array.ofSeq |> Array.rev

 type OrderType = Long | Short

 type Order = 
  { Symbol:string
    Quantity:int
    OrderType:OrderType
    Value:decimal }

 type Portfolio(startingCash:decimal, startDate:DateTime) = class
  
  let mutable positions = new System.Collections.Generic.List<Order>()
  let mutable positionsValue = 0.0M
  let mutable currentPrice = 0M

  member x.StartDate = startDate /// The starting date of the portfolio.
  member x.StartingCash = startingCash /// Starting capital.

  member x.CurrentPrice
   with get() = currentPrice
   and set(value) = currentPrice <- value

  /// Current amount of available cash.
  member x.Cash
   with get() = startingCash - x.PositionsValue
  
  /// Total profit and loss up until the current time.
  member x.ProfitAndLoss
   with get() = x.Cash + x.PositionsValue - startingCash
  
  /// Dictionary of open positions (i.e. Stock GOOG 124).
  member x.Positions
   with get() = positions

  /// Total value of the open positions.
  /// TODO: This changes with every tick and so must be recalculated using most recent tick price.
  member x.PositionsValue
   with get() = x.Positions |> Seq.filter (fun y -> y.Value = 0M) |> Seq.sumBy (fun y -> y.Value)

  /// Sum of positionsValue and cash.
  member x.PortfolioValue 
   with get() = x.Cash + x.PositionsValue

  /// Cumulative percentage returns for entire portfolio up until now. Pentage gain or loss of start cash.
  member x.Returns
   with get() = x.ProfitAndLoss / startingCash

  member x.AddPosition(order) = positions.Add(order)
  member x.ClosePosition(value) = positions.Remove(value)

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
  member private x.PlaceOrder(order:Order) =
    let placed = portfolio.AddPosition(order)
    printfn "Order placed: %A, successful: %A" order placed
  
  /// Call on incoming data
  member private x.IncomingTick(tick:Tick) = 
   // TODO Trading logic meat in here
   let currentPrice = tick.Close
   let calcVwap = volumeWeightedAvgPrice prices 5.0
   // if the stocks price less than the vwap by 0.5% and the limit has not been exceeded.
   if tick.Close < (calcVwap * 0.995M) && (portfolio.PositionsValue > -limit) then
    let order : Order = { Symbol = symbol; Quantity = -100; OrderType = Short; Value =  -100M * tick.Close }
    x.PlaceOrder(order)
   // if the stock price has increased by 0.1% to the vwap and we havent reached exposure limit then buy.
   elif tick.Close > calcVwap * 1.001M && (portfolio.PositionsValue < +limit) then
    let order : Order = { Symbol = symbol; Quantity = +100; OrderType = Long; Value =  100M * tick.Close }
    x.PlaceOrder(order)

  member x.BackTest () = 
   printfn "Algorithm Started."
   for tick in prices do 
    x.IncomingTick(tick)
   printfn "Total Long: %A" (portfolio.Positions |> Seq.filter (fun y -> y.OrderType = Long) |> Seq.sumBy (fun u -> u.Value))
   printfn "Total Short: %A" (portfolio.Positions |> Seq.filter (fun y -> y.OrderType = Short) |> Seq.sumBy (fun u -> u.Value))
   printfn "Algorithm Ended."

 end

 let system = 
  let p = new Portfolio(1000000M, DateTime.Today)
  let trader = new Trader(p, "GOOG")
  trader.BackTest()