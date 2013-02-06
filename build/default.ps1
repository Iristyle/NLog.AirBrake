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
  $compiledV2 = Join-Path $BuildOutDir 'v2.0\NLog.AirBrake.dll'
  $compiledV4 = Join-Path $BuildOutDir 'v4.0\NLog.AirBrake.dll'
  $versionV2 = (Get-Command $compiledV2).FileVersionInfo.FileVersion
  $versionV4 = (Get-Command $compiledV4).FileVersionInfo.FileVersion

  if ($versionV2 -ne $versionV4)
  {
    throw ".NET 2 version [$versionV2] does not match .NET 4 version [$versionV4]"
  }

  if ((Get-Item $compiledV2).Length -eq (Get-Item $compiledV4).Length)
  {
    throw ".NET 2 and .NET 4 binaries cannot be equal size"
  }

  $nuspec.package.metadata.version = $versionV2
  $nuspec.Save($nuspecPath)

  Publish-NugetPackage -Path $BuildOutDir -Force
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
