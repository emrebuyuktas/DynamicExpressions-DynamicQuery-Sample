You can find a sample dynamic expression example in this repository

example curl:

curl --location 'https://localhost:44363/api/Person/search' \
--header 'accept: */*' \
--header 'Content-Type: application/json' \
--data '{
  "size": 50,
  "index": 0,
  "filters": [
    {
      "field": "Age",
      "operator": 3,
      "value": 42
    },
    {
      "field": "Age",
      "operator": 5,
"logic": 1,
      "value": 45
    }
  ],
  "sorts": [
    {
      "field": "Age"
    }
  ]
}'
