namespace FinanceLibrary.AlgorithmicTrading
(*
    This module generates ticks using random number generation.
*)
module AlgoMarket = 

 open FinanceLibrary.Records
 
 type Market (duration, slippage : bool) = class

  let startDate = System.DateTime.Today.AddDays(-duration)
  let endDate = System.DateTime.Today
  let random = new MathNet.Numerics.Random.MersenneTwister(true)

  let nRandom (low,high) = 
   random.Next(low, high)

  member this.PricesShouldSlip
   with get() = slippage

  member this.Tick minPrice maxPrice maxVol minVol = 
   let o = decimal (nRandom(maxPrice,minPrice))
   let h = decimal (nRandom(maxPrice,minPrice))
   let l = decimal (nRandom(int(floor(h)),minPrice))
   let c = decimal (nRandom(maxPrice,minPrice))
   let v = decimal (nRandom(minVol maxVol))
   let tick = { Date = System.DateTime.Now; Open = o; High = h; Low = l; Close = c; Volume = v; AdjClose = 0M }
   tick

//  member this.DownTick = 
//  member this.UpTick =
  
  // Simulate market slippage.
  member this.GenerateSlippage(order : Order, tick : Tick) = 
   let quantity = abs order.Quantity
   let sign = 
    match order.OrderType with
    | Short -> -1M
    | Long -> 1M
    | Cover -> -1M

   if quantity > 500M then
    let quantities = 
     let rec x list sum = 
      if sum < quantity then
       let newQ = decimal(nRandom(10,int(quantity/5M))) // TODO normal dist.
       x (newQ::list) (sum + newQ)
      else 
       list
     x [] 0M
      
    let prices = 
     let rec x list counter = 
      let limit = quantities.Length
      if counter < limit then
       let newQ = tick.Low  * (1M + decimal(nRandom(5,20)) / 1000M)  // TODO normal dist.
       x (newQ::list) (counter + 1)
      else 
       list
     x [] 0

    [ for i = 0 to prices.Length - 1 do 
      yield { 
      Symbol = order.Symbol 
      Date = order.Date 
      Quantity = quantities.[i] * sign
      OrderType = order.OrderType
      Value = quantities.[i] * prices.[i] * sign } ]

   else 
    [ order ]

 end