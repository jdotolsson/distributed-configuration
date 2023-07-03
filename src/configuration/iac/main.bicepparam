using './main.bicep'

param environmentName = 'nonprod'
param locations = [
  'swedencentral'
  'westeurope'
]
param primaryLocation = 'swedencentral'
param skuName = 'standard'
