namespace FinanceLibrary.AlgorithmicTrading

module AlgoPortfolio =

 open System
 open FinanceLibrary.Records
 open Microsoft.FSharp.Collections

 /// PORTFOLIO
 type Portfolio(startingCash:decimal, startDate:DateTime) = class
  let positions = System.Collections.Generic.List<Order>()
  let closedPositions = System.Collections.Generic.List<Order>()
  let mutable currentPrice = 0M

  /// The starting date of the portfolio.
  member this.StartDate = startDate

  /// Starting capital.
  member this.StartingCash = startingCash 

  member this.CurrentPrice
   with get() = currentPrice
   and set(value) = currentPrice <- value

  /// Current amount of available cash.
  /// Sum of starting capital minus the positions as they were ordered/covered (not the current value of the positions).
  member this.Cash
   with get() = startingCash + (closedPositions |> Seq.sumBy (fun x -> abs x.Value))
  
  /// Total profit and loss up until the current time.
  member this.ProfitAndLoss
   with get() = this.Cash + this.PositionsValue - startingCash
  
  /// List of all positions.
  member this.Positions
   with get() = positions

  /// List of all closed positions.
  member this.ClosedPositions
   with get() = closedPositions
 
  /// Short positions.
  member this.ShortPositions
   with get() = positions 
    |> Seq.filter (fun x -> x.OrderType = Short) 
    |> Seq.toList

  /// Long positions.
  member this.LongPositions
   with get() = positions 
    |> Seq.filter (fun x -> x.OrderType = Long) 

  /// Total value of all open positions.
  member this.PositionsValue
   with get() = 
    let positionValue (order:Order) = 
     match order.OrderType with 
     | Long -> decimal (order.Quantity * currentPrice)
     | _ -> order.Value
    positions |> Seq.sumBy (fun x -> positionValue x)

  /// Value of all short positions.
  /// http://fundmanagersoftware.com/help/short_positions.html
  member this.ShortPositionsValue
   with get() = this.ShortPositions 
    |> Seq.sumBy (fun x -> x.Value)

  member this.ClosedPositionsValue
   with get() = closedPositions
    |> Seq.sumBy (fun x -> x.Value)

  /// Sum of positionsValue and cash.
  member this.PortfolioValue 
   with get() = this.Cash + this.PositionsValue

  /// Cumulative percentage returns for entire portfolio up until now. Pentage gain or loss of start cash.
  member this.Returns
   with get() = this.ProfitAndLoss / startingCash

  /// A new position / order.
  member this.AddPosition(order) = positions.Add(order)

  member this.ClosePosition(order) = 
   positions.Remove(order) |> ignore
   closedPositions.Add(order)
  
  override this.ToString() = 
   sprintf "Starting Cash %A, 
            Current Cash %A,
            Total Portfolio Value %A, 
            Position Value %A,
            Short Position Value %A,
            Closed Positions Value %A,
            Cumulative Returns %A, 
            Cumulative PnL %A" 
            this.StartingCash 
            this.Cash 
            this.PortfolioValue 
            this.PositionsValue 
            this.ShortPositionsValue 
            this.ClosedPositionsValue
            this.Returns 
            this.ProfitAndLoss 
 end  