// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake

// Directories
let buildDir  = "./build/"
let deployDir = "./deploy/"


// Filesets
let appReferences  =
    !! "/**/*.csproj"
    ++ "/**/*.fsproj"

let testReference = !! "/build/*.Tests.exe"

// version info
let version = "0.1"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir]
)

Target "Build" (fun _ ->
    // compile all projects below src/app/
    MSBuildDebug buildDir "Build" appReferences
    |> Log "AppBuild-Output: "
)

Target "Test" (fun _ -> 
    Log "Found Tests: " testReference
    testReference
    |> Seq.map (fun f -> 
                    f,ExecProcess (fun info ->
                        info.FileName <- f
                        info.WorkingDirectory <- buildDir) 
                        (System.TimeSpan.FromMinutes 5.0))
    |> Seq.iter (fun (f,retCode) -> if retCode <> 0 then failwithf "%s returned ExitCode %d" f retCode)                
)

Target "Deploy" (fun _ ->
    !! (buildDir + "/**/*.*")
    -- "*.zip"
    |> Zip buildDir (deployDir + "ApplicationName." + version + ".zip")
)

// Build order
"Clean"
  ==> "Build"
  ==> "Test"
  ==> "Deploy"

// start build
RunTargetOrDefault "Test"
