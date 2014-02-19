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
      Date: DateTime
      Quantity : decimal
      OrderType : OrderType
      mutable Value : decimal }

    override this.ToString() = 
        sprintf "%A, %A, %A, %A" this.Symbol this.Quantity this.OrderType this.Value

type IStockService = 
    abstract GetStockPrices : string -> int -> Tick []

type IOptionService = 
    abstract GetOptionTable : string -> OptionsData list