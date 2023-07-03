param configurationStoreNames array = [
  'appcs-dist-cfg-nonprod-01'
  'appcs-dist-cfg-nonprod-02'
]

var json = loadJsonContent('env-configuration.json')

module m_keyValues 'keyvalues.bicep' = [for configurationStoreName in configurationStoreNames: {
  name: '${configurationStoreName}-m_keyValues'
  params: {
    configs: json.Configs
    configurationStoreName: configurationStoreName
  }
}]
