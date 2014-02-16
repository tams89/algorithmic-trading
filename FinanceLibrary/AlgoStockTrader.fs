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

 type Portfolio(startingCash, startDate) = class
  let mutable cash = 0M 
  let mutable profitAndLoss = 0M 
  let mutable positions = new System.Collections.Generic.Dictionary<string, decimal>() 
  let mutable positionsValue = new System.Collections.Generic.Dictionary<string, decimal>() 
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
   with get() = positionsValue

  /// Sum of positionsValue and cash.
  member x.PortfolioValue 
   with get() = portfolioValue <- cash + (positionsValue.Values |> Seq.sum)

  /// Cumulative percentage returns for entire portfolio up until now. Pentage gain or loss of start cash.
  member x.Returns
   with get() = returns <- profitAndLoss / startingCash

  member x.AddPosition(value) = positions.Add(value)
  member x.RemovePosition(value) = positions.Remove(value)

 end

 type Tick = 
  { Symbol:string
    Date:DateTime
    Time:TimeSpan
    Open:decimal
    High:decimal
    Low:decimal
    Close:decimal
    Volume:decimal }
 
 /// Executed when a new price comes in.
 let incomingTick tick portfolio = 
  
  let period = 5.0 // vwap calculation number of days for period

  /// Volume Weighted Average Price
  /// Calculated using mean high low close.
  let vwap (prices: Tick []) period = 
    let ticksInPeriod =
     prices 
     |> Array.filter (fun x -> x.Date <= DateTime.Today.AddDays(-period) || x.Date >= DateTime.Today.AddDays(+period)) // get data within period relative to now.
    (ticksInPeriod |> Array.sumBy (fun x-> ((x.High + x.Low + x.Close)/3.0M) * x.Volume)) // Sum price times volume per trade
     / (ticksInPeriod |> Array.sumBy (fun x -> x.Volume)) // Sum volume over whole period

  printfn "Frame intercepted and processed"
