namespace FinanceLibrary.AlgorithmicTrading

module AlgoPortfolio =

 open System
 open FinanceLibrary.Records

 /// PORTFOLIO
 type Portfolio(startingCash:decimal, startDate:DateTime) = class
  let positions = new System.Collections.Concurrent.BlockingCollection<Order>()
  let closedPositions = new System.Collections.Concurrent.BlockingCollection<Order>()

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
   with get() = startingCash + this.ClosedPositionsValue
  
  /// Total profit and loss up until the current time.
  member this.ProfitAndLoss
   with get() = this.Cash + this.PositionsValue - startingCash
  
  /// List of all positions.
  member this.Positions
   with get() = positions

  /// List of all closed positions.
  member this.ClosedPositions
   with get() = closedPositions

  /// Long positions.
  member this.LongPositions
   with get() = this.Positions
    |> Seq.filter (fun x -> x.OrderType = Long) 
 
  /// Short positions.
  member this.ShortPositions
   with get() = this.Positions 
    |> Seq.filter (fun x -> x.OrderType = Short) 

  /// Closed Short Positions
  member this.ClosedShortPositions
   with get() = this.ClosedPositions
    |> Seq.filter (fun x -> x.OrderType = Short)

  /// Total value of all open positions at the current time.
  member this.PositionsValue
   with get() = 
    let positionValue (order:Order) = 
     match order.OrderType with 
     | Long -> order.Quantity * currentPrice
     | _ -> order.Value
    this.Positions |> Seq.sumBy (fun x -> positionValue x)

  /// Value of all short positions.
  /// http://fundmanagersoftware.com/help/short_positions.html
  member this.ShortPositionsValue
   with get() = this.ShortPositions 
    |> Seq.sumBy (fun x -> x.Value)
  
  /// Value of all closed positions.
  member this.ClosedPositionsValue
   with get() = 
    let closedPositionValue (order:Order) = 
     match order.OrderType with 
     | Cover -> order.Value // should be negative always.
     | _ -> abs(order.Value) // should always be positive
    (this.ClosedPositions |> Seq.sumBy (fun x -> closedPositionValue x))
  
  /// Value of all closed short positions.
  member this.ClosedShortPositionsValue
   with get() = this.ClosedShortPositions
    |> Seq.sumBy (fun x -> abs x.Value)

  /// Sum of positionsValue and cash.
  member this.PortfolioValue 
   with get() = this.Cash + this.PositionsValue

  /// Cumulative percentage returns for entire portfolio up until now. Percentage gain or loss of start cash.
  member this.Returns
   with get() = this.ProfitAndLoss / startingCash

  /// A new position / order.
  member this.AddPosition(order) = 
   match order.OrderType with
   | Cover -> closedPositions.Add(order)
   | _ -> positions.Add(order)

  /// Close a position. Removes it from positions and places it in the closed
  /// positions collection.
  member this.ClosePosition(order : Order) = 
   positions.TryTake(ref order, -1) |> ignore
   closedPositions.Add(order)
  
  override this.ToString() = 
   sprintf "Starting Cash %A
            Current Cash %A
            Total Portfolio Value %A
            Position Value %A
            Short Position Value %A
            Closed Positions Value %A
            Closed Short Positions Value %A
            Cumulative Returns %A
            Cumulative PnL %A" 
            this.StartingCash
            this.Cash
            this.PortfolioValue
            this.PositionsValue
            this.ShortPositionsValue 
            this.ClosedPositionsValue
            this.ClosedShortPositionsValue
            this.Returns
            this.ProfitAndLoss
 end