Param([string]$repo)

# **** Cloning BHoM Test_Toolkit ****
$repo = "Test_Toolkit"

# **** Defining Paths ****
$slnPath = "$ENV:BUILD_SOURCESDIRECTORY\$repo\$repo.sln"

# **** Cloning Repo ****
Write-Output ("Cloning " + $repo + " to " + $ENV:BUILD_SOURCESDIRECTORY + "\" + $repo)
git clone -q --branch=master https://github.com/BHoM/$repo.git  $ENV:BUILD_SOURCESDIRECTORY\$repo

# **** Restore NuGet ****
Write-Output ("Restoring NuGet packages for " + $repo)
& NuGet.exe restore $slnPath

# **** Cloning BuroHappold_Test_Toolkit ****
$repo = "BuroHappold_Test_Toolkit"

# **** Defining Paths ****
$slnPath = "$ENV:BUILD_SOURCESDIRECTORY\$repo\$repo.sln"

# **** Cloning Repo ****
Write-Output ("Cloning " + $repo + " to " + $ENV:BUILD_SOURCESDIRECTORY + "\" + $repo)
git clone -q --branch=master https://pat:$env:BHOM_ACCESSTOKEN@github.com/BuroHappoldEngineering/$repo.git  $ENV:BUILD_SOURCESDIRECTORY\$repo

# **** Restore NuGet ****
Write-Output ("Restoring NuGet packages for " + $repo)
& NuGet.exe restore $slnPath