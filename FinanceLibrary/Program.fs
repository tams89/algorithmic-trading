namespace FinanceLibrary
    module Main =
        [<EntryPoint>]
        let main args =
            printfn "ans %A" (FinanceLibrary.AlgorithmicTrading.MomentumVWAP.execute)
            0