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

 type Portfolio =
  {  CapitalUsed:decimal
     Cash:decimal
     ProfitAndLoss:decimal
     Positions:decimal
     PositionsValue:decimal
     Returns:decimal
     StartingCash:decimal
     StartDate:DateTime
  }
 
 type Tick = 
  { Symbol:string
    Date:DateTime
    Time:TimeSpan
    Open:decimal
    High:decimal
    Low:decimal
    Close:decimal
    Volume:decimal }
 
 let handle_tick = 
  
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
