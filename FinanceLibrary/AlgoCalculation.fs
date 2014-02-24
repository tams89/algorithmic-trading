namespace FinanceLibrary.AlgorithmicTrading

module AlgoCalculation = 
    
    open FinanceLibrary.Records
    
    /// Financial Calculations
    type Calculation() = 
        class
            // Calculated using mean high low close.
            let volumeWeightedAvgPrice (prices : array<Tick>) = 
                let rec SumTradePriceVolume sum volSum counter = 
                    let limit = prices.Length
                    if counter < limit then 
                        let tick = prices.[counter]
                        let tickPrice = (tick.High + tick.Low + tick.Close) / 3.0M
                        SumTradePriceVolume (sum + tickPrice * tick.Volume) (volSum + tick.Volume) (counter + 1)
                    else sum / volSum
                SumTradePriceVolume 0M 0M 0
            /// Volume Weighted Average Price
            member this.VWAP(prices) = volumeWeightedAvgPrice prices
        end