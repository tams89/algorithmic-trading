namespace FinanceLibrary

module Records = 
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
       StrikePrice : decimal
       LastPrice : decimal
       ChangeDirection : string
       Change : decimal
       Bid : decimal
       Ask : decimal
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
       Date : DateTime
       Quantity : decimal
       OrderType : OrderType
       Value : decimal }
     override this.ToString() = sprintf "%A, %A, %A, %A, %A" this.Symbol this.Date this.Quantity this.OrderType this.Value

 type Variables = 
  { BackTestPeriod:int
    ShortVwap:decimal
    LongVwap:decimal
    VwapPeriod: float
    Vwap:decimal
    CoverBarrierPrice:decimal
    MinLimit:decimal
    MaxLimit:decimal
    NumShares:decimal
    CoverAfterDays:float }

module Interfaces =
 open Records

 type IStockService = 
     abstract GetStockPrices : string -> int -> Tick []
 
 type IOptionService = 
     abstract GetOptionTable : string -> OptionsData list