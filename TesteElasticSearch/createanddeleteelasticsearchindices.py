
# coding: utf-8

from elasticsearch import Elasticsearch
import elasticsearch.helpers
import json

def createMapping():
    settings = {
        "settings": {
                "index.mapping.total_fields.limit": 2000,
                "index.mapping.nested_fields.limit": 100,
                "number_of_shards": 1,
                "number_of_replicas": 0
        },
        "split": {
           "field": "clientId",
            "separator": "|"
        },
        "split": {
            "field": "groupId",
            "separator": "|"
        },
        "split": {
            "field": "actionErrors",
            "separator": "|"
        },
        "mappings": {
            "diagnostic-summary": {
                "properties": {
                    "id":{"type": "string"},
                    "deviceSerialNumber":{"type": "string"},
                    "deviceId":{"type": "string"},
                    "deviceManufacturer":{"type": "string"},
                    "deviceModelId":{"type": "string"},
                    "deviceModelName":{"type": "string"},
                    "deviceVersionFirmware":{"type": "string"},
                    "deviceVersionHardware":{"type": "string"},
                    "trackableObjectId":{"type": "string"},
                    "vehiclePlate":{"type": "string"},
                    "vehicleModel":{"type": "string"},
                    "vehicleBrand":{"type": "string"},
                    "date":{"type": "date"},
                    "amountEvent":{"type": "integer"},
                    "amountEventFix":{"type": "integer"},
                    "amountEventNoFix":{"type": "integer"},
                    "amountPosition":{"type": "integer"},
                    "amountPositionSleep":{"type": "integer"},
                    "amountSleepControl":{"type": "integer"},
                    "amountIgnition":{"type": "integer"},
                    "amountBattery":{"type": "integer"},
                    "amountFake":{"type": "integer"},
                    "amountGps":{"type": "integer"},
                    "amountSuddenEvent":{"type": "integer"},
                    "amountOthers":{"type": "integer"},
                    "amountMoving":{"type": "integer"},
                    "amountStopped":{"type": "integer"},
                    "distance":{"type": "integer"},
                    "amountTimeMoving":{"type": "string"},
                    "amountTimeStopped":{"type": "string"},
                    "avgTimePosition":{"type": "string"},
                    "avgTimePositionSleep":{"type": "string"},
                    "avgTimeSleepControl":{"type": "string"},
                    "avgTimeIgnition":{"type": "string"},
                    "avgTimeBattery":{"type": "string"},
                    "avgTimeFake":{"type": "string"},
                    "avgTimeGps":{"type": "string"},
                    "avgTimeSuddenEvent":{"type": "string"},
                    "avgTimeOthers":{"type": "string"},
                    "avgTimeMoving":{"type": "string"},
                    "avgTimeStopped":{"type": "string"},
                    "clientId":{"type": "string"},
                    "groupId":{"type": "string"},
                    "telecomProvider":{"type": "string"},
                    "telecomAreaCode":{"type": "string"},
                    "telecomPhoneNumber":{"type": "string"},
                    "telecomIccid":{"type": "string"},
                    "amountIgnitionOn":{"type": "integer"},
                    "amountIgnitionOff":{"type": "integer"},
                    "amountBatteryExternal":{"type": "integer"},
                    "amountBatteryInternal":{"type": "integer"},
                    "amountBatteryBackup":{"type": "integer"},
                    "amountJamming":{"type": "integer"},
                    "avgTimeIgnitionOn":{"type": "string"},
                    "avgTimeIgnitionOff":{"type": "string"},
                    "avgTimeBatteryExternal":{"type": "string"},
                    "avgTimeBatteryInternal":{"type": "string"},
                    "avgTimeBatteryBackup":{"type": "string"},
                    "avgTimeJamming":{"type": "string"},
                    "isOdometerWrong":{"type": "boolean"},
                    "avgOdometerWrongByDistance":{"type": "float"},
                    "avgOdometerWrongByFix":{"type": "float"},
                    "avgOdometerWrongBySpeed":{"type": "float"},
                    "perOdometerWrongByDistance":{"type": "float"},
                    "perOdometerWrongByFix":{"type": "float"},
                    "perOdometerWrongBySpeed":{"type": "float"},
                    "distanceGPS":{"type": "float"},
                    "odometerMeanVariation":{"type": "float"},
                    "standardDeviationOdometerVariation":{"type": "float"},
                    "meanVariationVelocityAvg":{"type": "float"},
                    "actionMessage":{"type": "string"},
                    "actionCode":{"type": "integer"},
                    "actionErrors":{"type": "string"},
                    "multiplyingFactor":{"type": "float"},
                    "observation":{"type": "string"},
                    "amountBatteryExternalLost":{"type": "integer"},
                    "amountBatteryExternalRecovered":{"type": "integer"},
                    "avgTimeBatteryExternalLost":{"type": "string"},
                    "avgTimeBatteryExternalRecovered":{"type": "string"}
                }
            }
        }
    }
    return settings


elasticsearch_url = "https://search-ceabs-es-dev-zky7zrh7go4h4rdi652u5visjq.us-east-1.es.amazonaws.com"

es_conn = Elasticsearch(hosts=elasticsearch_url, timeout=60)

#indice = 'teste_elastic_search'
indice = 'diagnostic'
doc_type = 'diagnostic-summary'
#indice = 'diagnostic2'

#delete index
es_conn.indices.delete(index=indice, ignore=[400,404])

#create index
es_conn.indices.create(index=indice, ignore=400, body=createMapping())

res = es_conn.search(index=indice, doc_type=doc_type, body={"query":{"match":{"device_id":"206375"}}}, ignore=[400,404])
print json.dumps(res, indent=4, sort_keys=True)

res = es_conn.search(index=indice, doc_type=doc_type, ignore=[400,404], body={"query": {"match_all": {}}})
print json.dumps(res, indent=4, sort_keys=True)


#Para verificar como fazer a autenticação no elasticsearch via python, consultar a lambda LoadDiagnosticSummaryToElasticsearch no aws.

