# Reports
A simple interface for displaying our data in a friendly way.

# API - V2
## report/create
Creates a new report and returns its URL.

### Endpoint
`http://reports.sobotics.org/api/v2/report/create`

### Request

Your request can contain the following data (fields in bold are required):

 - **`appName`** *string*: The name of your app.
 - `appURL` *string*: A url pointing to your app's website.
 - `expiresAt` *string*: The ISO 8601 formatted date and time of when you would like your report to be deleted. If not specified, defaults to 30 days. Maximum value: one year from the current date.
 - **`fields`** *object*: An array of an array of `field` objects.

`field` objects can contain the following data:

 - **`id`** *string*: A unique string that identifies this field from others. This field's value must not change if you want  to maintain a consistent report layout.
 - **`name`** *string*: The name of the field. This text will be displayed next to the field's value for non-special field types.
 - **`value`** *sting*: The data this field represents. This data will be displayed next to the fields's name for non-special field types.
 - `type` *string*: the name of the special type this field represents.

`type` tells the API that the `field` object contains special data that needs additional processing before being displayed. The supported values are currently: `answers`, `link`, and `date`.

 - `answers`: This type of field will change colour if the associated question has an accepted answer. The field's `value` must start with an `A` (case-insensitive) to indicate that an answer has been accepted followed by the total number of answers. Example: `A5` tells us there are 5 answers, one of which is accepted. `2` indicates that there are 2 answers in total, neither are accepted.

 - `date` This type of field will display how far away the specified date is in minutes/hours/days/weeks. If the date is in the future, or is beyond 4 weeks in the past, just the date is rendered. The field's `value` must conform to the ISO 8601 format.

 - `link` This type of field will be displayed as a link. The data must be a URL. A report item can only contain one link field.

Example JSON:

```json
{
    "appName": "My Awesome App",
    "appURL": "https://example.com",
    "expiresAt": "2019-02-02T19:34:00Z",
    "fields": [
        [{
                "id": "link",
                "name": "This is some text",
                "value": "https://example.com",
                "type": "link"
            },
            {
                "id": "answers",
                "name": "Answers",
                "value": "a7",
                "type": "answers"
            },
            {
                "id": "revDate",
                "name": "Review scheduled",
                "value": "2018-02-06T14:55:00Z",
                "type": "date"
            },
            {
                "id": "score",
                "name": "Quality score",
                "value": "2"
            }
        ],
        [{
                "id": "link",
                "name": "This is some more text",
                "value": "https://example.com",
                "type": "link"
            },
            {
                "id": "answers",
                "name": "Answers",
                "value": "a1",
                "type": "answers"
            },
            {
                "id": "revDate",
                "name": "Review scheduled",
                "value": "2018-01-02T19:34:00Z",
                "type": "date"
            },
            {
                "id": "score",
                "name": "Quality score",
                "value": "5"
            }
        ],
        [{
                "id": "link",
                "name": "Blah blah blah blah blah blah blah blah blah blah",
                "value": "https://example.com",
                "type": "link"
            },
            {
                "id": "answers",
                "name": "Answers",
                "value": "4",
                "type": "answers"
            },
            {
                "id": "revDate",
                "name": "Review scheduled",
                "value": "2018-03-04T19:34:00Z",
                "type": "date"
            },
            {
                "id": "score",
                "name": "Quality score",
                "value": "8"
            }
        ]
    ]
}
```

Which will then produce [this](https://i.imgur.com/HF5faIn.png) report.

### Response
If your request is successful, you'll receive the URL of the newly created report.

```json
{
    "reportURL": "http://reports.sobotics.org/r/22CVxw"
}
```
