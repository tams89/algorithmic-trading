namespace FinanceLibrary.AlgorithmicTrading

module AlgoPortfolio =

 open System

 type OrderType = Long | Short | Cover

 type Order = { 
  Symbol:string
  Quantity:int
  OrderType:OrderType
  Value:decimal
  mutable Covered:bool }
 with 
  override this.ToString() = sprintf "%A, %A, %A, %A, %A" this.Symbol this.Quantity this.OrderType this.Value this.Covered

 /// PORTFOLIO
 type Portfolio(startingCash:decimal, startDate:DateTime) = class
  let positions = new System.Collections.Generic.List<Order>()
  let mutable currentPrice = 0M

  /// The starting date of the portfolio.
  member x.StartDate = startDate

  /// Starting capital.
  member x.StartingCash = startingCash 

  member x.CurrentPrice
   with get() = currentPrice
   and set(value) = currentPrice <- value

  /// Current amount of available cash.
  /// Sum of starting capital minus the positions as they were ordered (not the current value of the positions).
  member x.Cash
   with get() = startingCash - (positions |> Seq.filter (fun y -> y.OrderType = Long || y.OrderType = Short) |> Seq.sumBy (fun y -> y.Value))
  
  /// Total profit and loss up until the current time.
  member x.ProfitAndLoss
   with get() = (x.Cash + x.PositionsValue) - startingCash
  
  /// Dictionary of open positions (i.e. Stock GOOG 124).
  member x.Positions
   with get() = positions

  member x.ShortPositions
   with get() = positions |> Seq.filter (fun y -> y.OrderType = Short) |> Seq.toList

  /// Total value of the open positions.
  member x.PositionsValue
   with get() = 
    decimal(positions |> Seq.filter (fun y -> y.OrderType = Long) |> Seq.sumBy (fun y -> y.Quantity)) * currentPrice + // long positions value.
     (positions |> Seq.filter (fun y -> y.OrderType = Short) |> Seq.sumBy (fun y -> abs y.Value)) - // short positions value.
      decimal(positions |> Seq.filter (fun y -> y.OrderType = Cover) |> Seq.sumBy (fun y -> y.Quantity)) * currentPrice // short covering cost.

  /// Total value of all short positions.
  member x.ShortPositionsValue
   with get() = positions |> Seq.filter (fun y -> not y.Covered &&  y.OrderType = Short) |> Seq.sumBy (fun y -> y.Value)

  /// Sum of positionsValue and cash.
  member x.PortfolioValue 
   with get() = x.Cash + x.PositionsValue

  /// Cumulative percentage returns for entire portfolio up until now. Pentage gain or loss of start cash.
  member x.Returns
   with get() = x.ProfitAndLoss / startingCash

  // A new position / order.
  member x.AddPosition(order) = positions.Add(order)
  
  // Profit gained will be the initial short price say $100 * Quantity minus the price they shares were bought back at say $75 * Quantity. 
  member x.CloseShortPositions(short,order) = 
   short.Covered <- true
   positions.Add(order)
 end
