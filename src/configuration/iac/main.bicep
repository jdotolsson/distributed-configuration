param environmentName string
param locations array
param primaryLocation string
param skuName string

resource appcs 'Microsoft.AppConfiguration/configurationStores@2023-03-01' = [for (location, index) in locations: {
  name: 'appcs-dist-cfg-${environmentName}-${padLeft(index + 1, 2, '0')}'
  location: location
  sku: {
    name: skuName
  }
  properties: {
    softDeleteRetentionInDays: 7
  }
}]

resource kv 'Microsoft.KeyVault/vaults@2023-02-01' = {
  name: 'kv-dist-cfg-${environmentName}'
  location: primaryLocation
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
    accessPolicies: []
    enableRbacAuthorization: true
    enabledForTemplateDeployment: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    tenantId: tenant().tenantId
  }
}
