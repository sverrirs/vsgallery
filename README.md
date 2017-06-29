# Visual Studio Private Gallery

Ultra simple self-hosted extension gallery for Visual Studio 2010 and newer. 

Offering a single click deployment and super easy configuration this solution provides a fully featured solution for the Extension Gallery feature available in Microsoft Visual Studio.

<p align="center">
  <img src="https://raw.githubusercontent.com/sverrirs/vsgallery/master/img/extension-manager-01.png" />
</p>

See my blog for more details [blog.sverrirs.com](https://blog.sverrirs.com/2017/05/vs-selfhosted-extension-gallery.html)

## Features

* Fully featured Extension Gallery ready to use in Microsoft Visual Studio. 
* Counts downloads of extensions
* Displays star ratings, release notes and links to project home pages
* Offers a simple to use REST API to submit ratings and upload new VSIX packages
* Atom and JSON feeds for available packages
* Free!

:heart:

## How to install into Visual Studio

In Visual Studio

```
Tools > Options > Environment > Extensions and Updates
```

Add a new entry and copy in the URL of the main Microservice Atom Feed.

> By default the URL is `http://YOUR_SERVER:5100/feeds/atom.xml`

Please consult [this MSDN document](https://msdn.microsoft.com/en-us/library/hh266746.aspx) for any further details and alternative options on how to install a Private Extension Gallery in Visual Studio.

# How it works
The microservice is configured via the `config.ini` file that sits in the same folder as the main executable.

The `.vsix` files, along with their download counts and ratings data are stored in a subfolder of the main service executable `VsixStorage/` (this subfolder is configurable). This makes taking backups and moving the service between machines super easy as the root folder contains the entire Microservice state and data.

<pre>
root-folder
  |--vsgallery.exe
  |--config.ini
  |--VsixStorage
    |--atom.xml
    |--First.vsix
    |--Second.vsix
    |--AndSoForth.vsix
</pre>

## The vsgallery API

The Microservice comes with a rich HTTP based API. You can plug the data and its functionality directly into your development portal or company intranet with minimal web programming. Even direct integration into your continuous integration platforms and communication pipelines such as #slack are possible.

> The `vsix_id` required by many of the endpoints can be obtained by reading the `id` field in the feed endpoints.

### [GET] /feeds/atom.xml
This is the main entry point for the VSIX feed and serves up the Syndicate-Feed compatible Atom file containing all available extensions on the server. 

**This is the URL endpoint that should be used in Visual Studio.**

See [How to install into Visual Studio](#how-to-install) for more information.

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

## Thanks to

This work is made possible by the fantastic job done on the following projects.

[Topshelf](https://github.com/Topshelf/Topshelf): Windows Service Hosting

[Nancy](https://github.com/NancyFx/Nancy): Embedded HTTP hosting

[VSGallery.AtomGenerator](https://github.com/garrettpauls/VSGallery.AtomGenerator): VSIX parsing logic

[Ini-Parser](https://github.com/rickyah/ini-parser): Configuration ini file parsing

[Costura](https://github.com/Fody/Costura/): Assembly merging
