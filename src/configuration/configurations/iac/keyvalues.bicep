param configurationStoreName string
param configs array

resource appcs 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: configurationStoreName
}

resource keyvalues 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-03-01' = [for (item, index) in configs: {
  name: contains(item, 'Label') ? '${item.Name}$${item.Label}' : item.Name
  parent: appcs
  properties: {
    value: '${item.Value}'
    contentType: item.ContentType
    tags: item.Tags
  }
}]
