Param([parameter(Mandatory=$true)] [string] $version)

$files_to_update = @(
  "SonarLink.API\Properties\AssemblyInfo.cs",
  "SonarLink.TE\Properties\AssemblyInfo.cs",
  "SonarLink.TE\source.extension.vsixmanifest"
)

$files_to_update | ForEach-Object {
  (Get-Content $_) | ForEach-Object { $_.replace("0.0.0.0", $version) } | Set-Content $_
}