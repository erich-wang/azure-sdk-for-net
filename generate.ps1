
$nameMap = @{
  "kusto" = "azure-kusto";
  "recoveryservices-backup" = "recoveryservicesbackup";
  "recoveryservices-siterecovery" = "recoveryservicessiterecovery";
  "securitycenter" = "security";
  "sqlmanagement" = "sql";
  "storsimple" = "storSimple1200Series";
  "websites" = "web";
}

function Prepare-Gen ([string] $originalFolder) {
  
  $serviceName = $originalFolder.Split("\")[-2]
 
  #check swagger spec
  if($nameMap.Contains($serviceName))
  {
    $serviceName = $nameMap[$serviceName]
  }
  $configFile = [System.IO.Path]::Combine($RestSpecPath, $serviceName, "resource-manager\readme.md")
  if(-not (Test-Path $configFile))
  {
    $MissSpecList.Add($serviceName)
    # Write-Error($configFile)
    return
  }else {
    $SpecList.Add($serviceName)
  }

  #Copy template and rename
  $newFolder = $originalFolder -replace "Microsoft.Azure.Management", "Azure.Management"
  $folderName = $newFolder.Split("\")[-1]
  rmdir $newFolder -Force -Recurse
  mkdir $newFolder
  Copy-Item .\sdk\template\Azure.Template\* $newFolder -Recurse -Force
  Get-ChildItem $newFolder -Filter "Azure.Template.*" -Recurse | ForEach-Object { Move-Item $_.FullName ($_.FullName -replace "Azure.Template", $folderName) }

  $projPath = [System.IO.Path]::Combine($newFolder, "src", $folderName + ".csproj")

  $content = Get-Content -Path $projPath -Raw
  $content = $content.Replace('<Import Project="$(MSBuildThisFileDirectory)..\..\..\core\Azure.Core\src\Azure.Core.props" />', "  <PropertyGroup> <AutorestInput>$configFile</AutorestInput> </PropertyGroup> `n <Import Project=`"`$(MSBuildThisFileDirectory)..\..\..\core\Azure.Core\src\Azure.Core.props`" />")
  Set-Content -Path $projPath $content
  #build and test
}

$RestSpecPath = "C:\AME\azure-rest-api-specs\specification"
$SpecList = [System.Collections.Generic.List[string]]@()
$MissSpecList = [System.Collections.Generic.List[string]]@()

$manageFolders = (Get-ChildItem .\sdk -Directory -Depth 2 -Filter "*Microsoft.Azure.Management*").FullName
[System.Collections.ArrayList]$folderNames = $manageFolders
$folderNames.Add("$PSScriptRoot\sdk\appconfiguration\Microsoft.Azure.Management.AppConfiguration")

$folderNames | ForEach-Object -Process { Prepare-Gen($_) }

# PrepareGen($dir)
