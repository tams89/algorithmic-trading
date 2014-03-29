namespace Algorithm.Core
    module Main =
        [<EntryPoint>]
        let main args =
//            printfn "ans %A" (Algorithm.Core.AlgorithmicTrading.AlgoBackTester.backTest())
            printfn "ans %A" (Algorithm.Core.FIX.execute())
            0