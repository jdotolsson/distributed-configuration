param (
    [Parameter(Mandatory = $true)]
    [string]$sourceEnvironmentPath
)

function Get-PropertyValueOrDefault {
    param (
        [PSCustomObject]$Object,
        [string]$PropertyName,
        $DefaultValue
    )

    $hasProperty = [bool]($Object.PSObject.Properties.Name -match $PropertyName)
    if ($hasProperty) {
        return $Object.$PropertyName
    }
    else {
        return $DefaultValue
    }
}

function Get-VaultValues {
    param (
        [PSCustomObject]$Object,
        [string]$Environment,
        [array]$Scopes,
        [string]$Path = '',
        [object]$Tags
    )

    $vaultValues = @()
    $value = $Object.Value
    $localScopes = ($Scopes + (Get-PropertyValueOrDefault -Object $Object -PropertyName "AdditionalScopes" -DefaultValue @())) | Get-Unique

    foreach ($property in $value.PSObject.Properties) {
        if ($property.Value -is [String] -and $property.Value -match '\.vault\.azure\.net') {
            foreach ($propertyScope in $localScopes) {
                $config = [PSCustomObject]@{
                    Name        = if ($Path) { "$($Path):$($property.Name)" } else { $property.Name }
                    Value       = $property.Value
                    ContentType = "application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8"
                    Label       = "$Environment-$propertyScope"
                    Tags        = $Tags
                }
                $vaultValues += $config
            }
            # Remove the property from the source object
            $value.PSObject.Properties.Remove($property.Name)
        }
        elseif ($property.Value -is [PSCustomObject]) {
            $vaultValues += Get-VaultValues -Object $property -Environment $Environment -Scopes $Scopes -Path "$($Path):$($property.Name)" -Tags $Tags
        }
    }

    return $vaultValues
}

function Get-ConfigurationNodes {
    param (
        [string]$PropertyName,
        [object]$PropertyValue,
        [string]$Environment,
        [array]$Scopes,
        [object]$Tags
    )

    $configurationValues = @()

    $localScopes = $Scopes + (Get-PropertyValueOrDefault -Object $PropertyValue -PropertyName "AdditionalScopes" -DefaultValue @())
    $contentType = Get-PropertyValueOrDefault -Object $PropertyValue -PropertyName "ContentType" -DefaultValue ""
    $value = Get-PropertyValueOrDefault -Object $PropertyValue -PropertyName "Value" -DefaultValue $PropertyValue

    if ($PropertyValue.Value -is [PSCustomObject]) {
        $configurationValues += Get-VaultValues -Object $PropertyValue -Environment $Environment -Scopes $localScopes -Path $PropertyName -Tags $Tags
    }
    if ($value -is [PSCustomObject]) {
        $value = "$($value | ConvertTo-Json -Compress -Depth 100)"
    }

    foreach ($propertyScope in $localScopes) {
        $config = [PSCustomObject]@{
            Name        = $PropertyName
            Value       = $value
            ContentType = if ($value -match ".vault.azure.net") { "application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8" } else { $contentType }
            Label       = "$Environment-$propertyScope"
            Tags        = $Tags
        }
        $configurationValues += $config
    }

    return $configurationValues
}

function Get-EnvironmentConfiguration {
    param (
        [object]$environment
    )

    $transformedConfigs = @()
    Get-ChildItem $environment.FullName -Recurse -Filter "*.json" `
    | Select-Object -ExpandProperty FullName
    | ForEach-Object {
        $sourceJsonPath = $_

        $sourceJson = Get-Content -Path $sourceJsonPath -Raw
        if ($null -eq $sourceJson) {
            Write-Host "Could not parse contents of file: '$sourceJsonPath'"
        }
        else {
            $sourceObject = ConvertFrom-Json $sourceJson
            $scopes = $sourceObject.Scopes

            foreach ($property in $sourceObject.PSObject.Properties) {
                $propertyName = $property.Name
                $propertyValue = $property.Value
                if ($propertyName -eq "Scopes") {
                    continue
                }
                $transformedConfigs += Get-ConfigurationNodes -PropertyName $propertyName -PropertyValue $propertyValue -Environment $environment.Name -Scopes $scopes -Tags $propertyValue.Tags
            }
        }        
    }    

    $transformedObject = [PSCustomObject]@{
        Configs = $transformedConfigs
    }
    return $transformedObject
}

$environments = Get-ChildItem $sourceEnvironmentPath | Select-Object -Property Name, FullName
$environments | ForEach-Object {
    $environmentConfigurations = Get-EnvironmentConfiguration $_

    $transformedJson = $environmentConfigurations | ConvertTo-Json -Depth 10
    $transformedJson | Out-File "./$($_.Name)-result.json"
}
