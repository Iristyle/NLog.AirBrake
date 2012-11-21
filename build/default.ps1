function Get-CurrentDirectory
{
  $thisName = $MyInvocation.MyCommand.Name
  [IO.Path]::GetDirectoryName((Get-Content function:$thisName).File)
}

# HACK: this is a monkey patch used to create MSBuild friendly
# output until Psake does something with
# https://github.com/psake/psake/pull/34
$originalResolveError = (Get-Item Function:\Resolve-Error).ScriptBlock

function Resolve-Error
{
  $msg = &$originalResolveError $args
  $msg += "Error PSAKE1: {0}: `n{1}" -f (Get-Date), (&$originalResolveError $args -Short)

  return $msg
}

Properties {
  $currentDirectory = Get-CurrentDirectory
  $BuildOutDir = Join-Path $currentDirectory 'BuildArtifacts'
}

Task default -Depends Test, Package

Task Compile -Depends Init,Clean {
  "Starting compilation process... "

  #build .NET2 and .NET4 versions separately
  exec { msbuild build.proj }
  exec { msbuild build.proj /p:TargetFrameworkVersion=v4.0 }
}

Task Package -Depends Compile, Test {
  'Publishing a public NuGet package'
  $sourceNuspec = Join-Path $currentDirectory 'NLog.AirBrake.nuspec'
  $nuspecPath = Join-Path $BuildOutDir 'NLog.AirBrake.nuspec'
  Copy-Item $sourceNuspec -Destination $nuspecPath

  $nuspec = [Xml](Get-Content $nuspecPath)
  $compiled = Join-Path $BuildOutDir 'v2.0\NLog.AirBrake.dll'
  $version = (Get-Command $compiled).FileVersionInfo.FileVersion
  $nuspec.package.metadata.version = $version
  $nuspec.Save($nuspecPath)

  if ($Env:NUGET_SOURCE)
  {
    Publish-NugetPackage -Path $BuildOutDir -Force
  }
  else
  {
    "No API key value found. Skipping Nuget publish."
  }
}

Task Clean -Depends Init {
  "Removing old zips"

  #old cruft if it's hanging around
  if (Test-Path $BuildOutDir)
  {
    Get-ChildItem $BuildOutDir |
      Remove-Item -Include '*' -Recurse -Force -ErrorAction SilentlyContinue
  }
}

Task Init {
  "init"
}

Task Test -Depends Compile {
  Invoke-Xunit -Path $BuildOutDir
}
