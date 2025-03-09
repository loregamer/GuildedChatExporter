#!/usr/bin/env pwsh

param(
    [Parameter()]
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release',

    [Parameter()]
    [ValidateSet('win-x64', 'linux-x64', 'osx-x64')]
    [string[]] $Runtime = @('win-x64', 'linux-x64', 'osx-x64'),

    [Parameter()]
    [switch] $NoRestore,

    [Parameter()]
    [switch] $NoBuild,

    [Parameter()]
    [switch] $NoPublish
)

$ErrorActionPreference = 'Stop'

# Set up directories
$rootDir = $PSScriptRoot
$artifactsDir = Join-Path $rootDir 'artifacts'

# Clean up artifacts
if (Test-Path $artifactsDir) {
    Remove-Item -Path $artifactsDir -Recurse -Force
}

New-Item -Path $artifactsDir -ItemType Directory -Force | Out-Null

foreach ($rt in $Runtime) {
    Write-Output "Processing $rt..."
    
    # Restore
    if (-not $NoRestore) {
        Write-Output "Restoring for $rt..."
        dotnet restore GuildedChatExporter.sln -r $rt
    }

    # Publish
    if (-not $NoPublish) {
        Write-Output "Publishing for $rt..."
        
        $runtimeDir = Join-Path $artifactsDir $rt
        New-Item -Path $runtimeDir -ItemType Directory -Force | Out-Null
        
        # Publish CLI
        dotnet publish GuildedChatExporter.Cli `
            --configuration $Configuration `
            --runtime $rt `
            --self-contained true `
            --output $runtimeDir
            
        # Publish GUI
        dotnet publish GuildedChatExporter.Gui `
            --configuration $Configuration `
            --runtime $rt `
            --self-contained true `
            --output $runtimeDir
            
        # Zip the artifacts
        $zipPath = Join-Path $artifactsDir "GuildedChatExporter-$rt.zip"
        Compress-Archive -Path $runtimeDir\* -DestinationPath $zipPath -Force
    }
}

Write-Output 'Done!'
