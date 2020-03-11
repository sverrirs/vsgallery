# The vsgallery API

The system comes with a rich HTTP based API. You can plug the data and its functionality directly into your development portal or company intranet with minimal web programming. Even direct integration into your continuous integration platforms and communication pipelines such as #slack are possible.

> The `vsix_id` required by many of the endpoints can be obtained by reading the `id` field in the feed endpoints.

### [GET] /feeds/atom.xml
This is the main entry point for the VSIX feed and serves up the Syndicate-Feed compatible Atom file containing all available extensions on the server. 

**This is the URL endpoint that should be used in Visual Studio.**

See [How to install into Visual Studio](#how-to-install-into-visual-studio) for more information.

### [GET] /api/ratings/{vsix_id}
Retrieves the rating value and vote count for a particular VSIX package by its ID. 

```
curl -X GET 
     http://VSGALLERY_SERVER:5100/api/ratings/VSIX_ID
```

The return type is the following JSON

```
{
  "rating": 4.3,
  "count": 19
}
```

### [POST/PUT] /api/ratings/{vsix_id}
Submitting rating values for a particular VSIX package by its ID. The post payload should be just raw string and contain a single floating point value in the range between [0.0, 5.0].

The example below will post a rating of `3.5` stars to VSIX package with the id `VSIX_ID`

```
curl -X POST 
     -H "Content-Type: text/plain" 
     --data "3.5" 
     http://VSGALLERY_SERVER:5100/api/ratings/VSIX_ID
```

### [GET] /api/json
JSON feed for the entire package catalog. Same data that is being fed through the atom feed but just in a handier JSON format.

### [GET] /api/download/{vsix_id}/{vsix_Name}
Retrieves a particular VSIX package by its VSIX ID and the VSIX Name.

### [POST/PUT] /api/upload
This endpoint accepts form-data uploads of one or more .vsix files to the hosting service. 

The example below will upload the file `my.vsix` to the gallery server and propose a new name for it `renamed.vsix` (you can omit the filename param to use the original name)

```
curl -X POST 
     --form "file=@my.vsix;filename=renamed.vsix" 
     http://VSGALLERY_SERVER:5100/api/upload
```

To upload multiple files simply add more form elements. The example below uploads two VSIX files at the same time.

```
curl -X POST 
     --form "file1=@my.vsix" 
     --form "file1=@your.vsix" 
     http://VSGALLERY_SERVER:5100/api/upload
```

To upload a file to the gallery server that requires an upload key

```
curl -X POST 
     --form "file=@my.vsix;filename=renamed.vsix" 
     http://VSGALLERY_SERVER:5100/api/upload/API_KEY
```