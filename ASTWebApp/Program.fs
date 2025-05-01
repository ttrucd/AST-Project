open Parse
open ParseAndRun 

let e1 = fromString "5+7"

printfn "%A" (run (fromString "5+7"))