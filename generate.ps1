function PrepareGen ([string] $originalFolder) {
  
  $serviceName = $originalFolder.Split("\")[-2]
 
  #check swagger spec
  $configFile = [System.IO.Path]::Combine($RestSpecPath, $serviceName, "resource-manager\readme.md")
  if(-not (Test-Path $configFile))
  {
    Write-Error($configFile)
    return
  }

  #Copy template and rename
  $newFolder = $originalFolder -replace "Microsoft.Azure.Management", "Azure.Management"
  $folderName = $newFolder.Split("\")[-1]
  mkdir $newFolder
  Copy-Item .\sdk\template\Azure.Template\* $newFolder -Recurse -Force
  Get-ChildItem $newFolder -Filter "Azure.Template.*" -Recurse | ForEach-Object { Move-Item $_.FullName ($_.FullName -replace "Azure.Template", $folderName) }

  $projPath = [System.IO.Path]::Combine($newFolder, "src", $folderName + ".csproj")

  $content = Get-Content -Path $projPath -Raw
  $content = $content.Replace('<Import Project="$(MSBuildThisFileDirectory)..\..\..\core\Azure.Core\src\Azure.Core.props" />', "  <PropertyGroup> <AutorestInput>$configFile</AutorestInput> </PropertyGroup> `n <Import Project=`"`$(MSBuildThisFileDirectory)..\..\..\core\Azure.Core\src\Azure.Core.props`" />")
  Set-Content -Path $projPath $content
  #build and test
}

$dir = (Get-ChildItem .\sdk -Directory -Depth 2 -Filter "*Microsoft.Azure.Management*")[0].FullName

$RestSpecPath = "C:\AME\azure-rest-api-specs\specification"

PrepareGen($dir)
