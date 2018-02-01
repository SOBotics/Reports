# Reports

Sample request to the create report function (more docs on the way):
```json
{
    "appName": "Queen",
    "appURL": "http://google.com",
    "expiresAt": "2019-02-26T21:25:50.5126763Z",
    "records": [
        {
            "fields": [
                {
                    "id": "link",
                    "name": "This is a title, blah blah blah",
                    "value": "http://google.com",
                    "type": 0
                },
                {
                    "id": "score",
                    "name": null,
                    "value": "-5",
                    "type": 0
                }
            ]
        },
        {
            "fields": [
                {
                    "id": "link",
                    "name": "This is a title, blah blah blah",
                    "value": "http://google.com",
                    "type": "Link"
                },
                {
                    "id": "score",
                    "name": null,
                    "value": "-5",
                    "type": 0
                }
            ]
        }
    ]
}
```
