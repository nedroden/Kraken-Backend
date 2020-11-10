# Measurement queue

Measurements must be added to the `KrakenMeasurements` queue. They must have the following format:

```
{
    "timestamp": long,
    "houses": [
        {
            "houseId": string,
            "consumption": float
        }
    ],
    "pipes": [
        {
            "pipeId": string,
            "waterFlowVolume": float,
            "waterQuality": float
        }
    ],
    "sources": [
        {
            "sourceId": string,
            "production": float,
            "waterQuality": float
        }
    ]
}
```