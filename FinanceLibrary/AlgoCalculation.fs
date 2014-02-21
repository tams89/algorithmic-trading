namespace FinanceLibrary.AlgorithmicTrading

module AlgoCalculation = 
    open FinanceLibrary.Records
    
    /// Financial Calculations
    type Calculation(prices) = 
        class
            
            // Calculated using mean high low close.
            let volumeWeightedAvgPrice (prices : seq<Tick>) (period : float) = 
                let pricesInRange = 
                    let prices = prices |> Seq.toArray
                    prices |> Array.filter (fun x -> x.Date >= prices.[prices.GetUpperBound(0)].Date.AddDays(-period))
                
                let rec SumTradePriceVolume sum volSum counter = 
                    let limit = pricesInRange.Length
                    if counter < limit then 
                        let tick = pricesInRange.[counter]
                        let tickPrice = (tick.High + tick.Low + tick.Close) / 3.0M
                        SumTradePriceVolume (sum + tickPrice * tick.Volume) (volSum + tick.Volume) (counter + 1)
                    else sum / volSum
                
                SumTradePriceVolume 0M 0M 0
            
            /// Volume Weighted Average Price
            member this.VWAP(period) = volumeWeightedAvgPrice prices period
        end