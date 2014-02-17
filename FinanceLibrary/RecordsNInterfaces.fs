namespace FinanceLibrary

open System

type Tick = 
    { Date : DateTime
      Open : decimal
      High : decimal
      Low : decimal
      Close : decimal
      Volume : decimal
      AdjClose : decimal }

type OptionsData = 
    { Symbol : string
      ExpiryDate : DateTime
      DaysToExpiry : string
      StrikePrice : Decimal
      LastPrice : Decimal
      ChangeDirection : string
      Change : Decimal
      Bid : Decimal
      Ask : Decimal
      Vol : int
      OpenInt : int
      InTheMoney : bool
      AtTheMoney : bool }

type OrderType = 
    | Long
    | Short
    | Cover

type Order = 
    { Symbol : string
      Quantity : int
      OrderType : OrderType
      Value : decimal
      mutable Covered : bool }
    override this.ToString() = 
        sprintf "%A, %A, %A, %A, %A" this.Symbol this.Quantity this.OrderType 
            this.Value this.Covered

type IStockService = 
    abstract GetStockPrices : string -> int -> Tick []

type IOptionService = 
    abstract GetOptionTable : string -> OptionsData list