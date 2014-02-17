namespace FinanceLibrary.AlgorithmicTrading

module AlgoPortfolio =

 open System
 open FinanceLibrary

 /// PORTFOLIO
 type Portfolio(startingCash:decimal, startDate:DateTime) = class
  let positions = new System.Collections.Generic.List<Order>()
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
   with get() = startingCash - (positions |> Seq.filter (fun x -> x.OrderType = Long || x.OrderType = Short) |> Seq.sumBy (fun x -> x.Value))
  
  /// Total profit and loss up until the current time.
  member this.ProfitAndLoss
   with get() = (this.Cash + this.PositionsValue) - startingCash
  
  /// Dictionary of open positions (i.e. Stock GOOG 124).
  member this.Positions
   with get() = positions

  member this.ShortPositions
   with get() = positions |> Seq.filter (fun x -> x.OrderType = Short) |> Seq.toList

  /// Total value of the open positions.
  member this.PositionsValue
   with get() = 
    decimal(positions |> Seq.filter (fun x -> x.OrderType = Long) |> Seq.sumBy (fun x -> x.Quantity)) * currentPrice + // long positions value.
     (positions |> Seq.filter (fun x -> x.OrderType = Short) |> Seq.sumBy (fun x -> abs x.Value)) - // short positions value.
      decimal(positions |> Seq.filter (fun x -> x.OrderType = Cover) |> Seq.sumBy (fun x -> x.Quantity)) * currentPrice // short covering cost.

  /// Total value of all short positions.
  member this.ShortPositionsValue
   with get() = positions |> Seq.filter (fun x -> not x.Covered &&  x.OrderType = Short) |> Seq.sumBy (fun x -> x.Value)

  /// Sum of positionsValue and cash.
  member this.PortfolioValue 
   with get() = this.Cash + this.PositionsValue

  /// Cumulative percentage returns for entire portfolio up until now. Pentage gain or loss of start cash.
  member this.Returns
   with get() = this.ProfitAndLoss / startingCash

  // A new position / order.
  member this.AddPosition(order) = positions.Add(order)
  
  // Profit gained will be the initial short price say $100 * Quantity minus the price they shares were bought back at say $75 * Quantity. 
  member this.CloseShortPositions(short,order) = 
   short.Covered <- true
   positions.Add(order)
 end