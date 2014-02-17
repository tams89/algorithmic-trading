namespace FinanceLibrary
    module Main =
        [<EntryPoint>]
        let main args =
            printfn "ans %A" (AlgorithmicTrading.MomentumVWAP.execute)
            0