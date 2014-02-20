namespace FinanceLibrary.AlgorithmicTrading

module AlgoPortfolio =

 open System
 open FinanceLibrary.Records
 open Microsoft.FSharp.Collections

 /// PORTFOLIO
 type Portfolio(startingCash:decimal, startDate:DateTime) = class
  let mutable positions : Order list = []
  let mutable currentPrice = 0M

  /// The starting date of the portfolio.
  member this.StartDate = startDate

  /// Starting capital.
  member this.StartingCash = startingCash 

  member this.CurrentPrice
   with get() = currentPrice
   and set(value) = currentPrice <- value

  /// Current amount of available cash.
  /// Sum of starting capital minus the positions as they were ordered (not the current value of the positions).
  member this.Cash
   with get() = startingCash - (positions |> PSeq.sumBy (fun x -> x.Value))
  
  /// Total profit and loss up until the current time.
  member this.ProfitAndLoss
   with get() = (this.Cash + this.PositionsValue) - startingCash
  
  /// List of all positions.
  member this.Positions
   with get() = positions
 
  // Un-Covered Short positions.
  member this.ShortPositions
   with get() = positions 
    |> PSeq.filter (fun x -> x.OrderType = Short && x.Value < 0M) 
    |> PSeq.toList

  /// Total value of all open positions.
  member this.PositionsValue
   with get() = 
    let positionValue (order:Order) = 
     match order.OrderType with 
     | Long -> decimal (order.Quantity * currentPrice)
     | Short -> order.Value
     | Cover -> order.Value
    positions |> PSeq.sumBy (fun x -> positionValue x)

  /// Value of all short positions.
  member this.ShortPositionsValue
   with get() = positions 
    |> PSeq.filter (fun x -> x.OrderType = Short && x.Value < 0M) 
    |> PSeq.sumBy (fun x -> x.Value)

  /// Sum of positionsValue and cash.
  member this.PortfolioValue 
   with get() = this.Cash + this.PositionsValue

  /// Cumulative percentage returns for entire portfolio up until now. Pentage gain or loss of start cash.
  member this.Returns
   with get() = this.ProfitAndLoss / startingCash

  // A new position / order.
  member this.AddPosition(order) = ignore (order::positions)
  
  // Profit gained will be the initial short price say $100 * Quantity 
  // minus the price they shares were bought back at say $75 * Quantity. 
  // The short order has its value updated to be postive as that is the initial profit and 
  // the covering orders value will be negative.
  member this.CloseShortPositions(short:Order) = 
   short.Value <- abs(short.Value)

  override this.ToString() = 
   sprintf "Starting Cash %A, 
            Current Cash %A,
            Total Portfolio Value %A, 
            Current Positions %A, 
            Current Short Positions %A,
            Position Value %A,
            Short Position Value %A,
            Cumulative Returns %A, 
            Cumulative PnL %A" 
            this.StartingCash 
            this.Cash 
            this.PortfolioValue 
            this.Positions.Length 
            this.ShortPositions.Length 
            this.PositionsValue 
            this.ShortPositionsValue 
            this.Returns 
            this.ProfitAndLoss 
 end  