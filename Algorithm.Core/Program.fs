namespace Algorithm.Core
    module Main =
        [<EntryPoint>]
        let main args =
//            printfn "ans %A" (Algorithm.Core.AlgorithmicTrading.AlgoBackTester.BackTester().BackTest())
            printfn "ans %A" (Algorithm.Core.FIX.execute())
            0