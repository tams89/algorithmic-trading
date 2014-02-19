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
   with get() = startingCash - (positions |> Seq.filter (fun x -> x.OrderType = Long || x.OrderType = Short || x.OrderType = Cover) 
                               |> Seq.sumBy (fun x -> x.Value))
  
  /// Total profit and loss up until the current time.
  member this.ProfitAndLoss
   with get() = (this.Cash + this.PositionsValue) - startingCash
  
  /// Dictionary of open positions (i.e. Stock GOOG 124).
  member this.Positions
   with get() = positions

  member this.ShortPositions
   with get() = positions |> Seq.filter (fun x -> x.OrderType = Short && not x.Covered) |> Seq.toList

  /// Total value of the open positions.
  member this.PositionsValue
   with get() = 
    let positionValue (order:Order) = 
     match order.OrderType with 
     | Long -> decimal order.Quantity * currentPrice // postive must be adjusted for current market value rather than value at purchase.
     | Short -> abs order.Value // Expected value by shorting.
     | Cover -> - abs order.Value // negative must be subtracted from the price of shorts and start of shorting.
    positions |> Seq.sumBy (fun x -> positionValue x)

  /// Total value of all short positions.
  member this.ShortPositionsValue
   with get() = positions |> Seq.filter (fun x -> x.OrderType = Short && not x.Covered) |> Seq.sumBy (fun x -> x.Value)

  /// Sum of positionsValue and cash.
  member this.PortfolioValue 
   with get() = this.Cash + this.PositionsValue + this.ShortPositionsValue

  /// Cumulative percentage returns for entire portfolio up until now. Pentage gain or loss of start cash.
  member this.Returns
   with get() = this.ProfitAndLoss / startingCash

  // A new position / order.
  member this.AddPosition(order) = positions.Add(order)
  
  // Profit gained will be the initial short price say $100 * Quantity minus the price they shares were bought back at say $75 * Quantity. 
  member this.CloseShortPositions(short,order) = 
   short.Covered <- true
   positions.Add(order)

  override this.ToString() = 
   sprintf "Starting Cash %A, 
            Current Cash %A,
            Total Portfolio Value %A, 
            Current Positions %A, 
            Current Short Positions %A,
            Position Value %A,
            Short Position Value %A,
            Cumulative Returns %A, 
            Cumulative PnL %A " this.StartingCash 
                                this.Cash 
                                this.PortfolioValue 
                                this.Positions.Count 
                                this.ShortPositions.Length 
                                this.PositionsValue 
                                this.ShortPositionsValue 
                                this.Returns 
                                this.ProfitAndLoss 
 end  