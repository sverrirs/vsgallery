# Visual Studio Private Gallery

Ultra simple self-hosted extension gallery for Visual Studio 2010 and newer. 

Offering a single click deployment and super easy configuration this solution provides a fully featured solution for the Extension Gallery feature available in Microsoft Visual Studio.

<p align="center">
  <img src="https://raw.githubusercontent.com/sverrirs/vsgallery/master/img/extension-manager-01.png" />
</p>

## Features

* Single self-contained executable. Execute it and you're up and running in seconds!
* File system based storage. No databases! All files and data are stored on the local file system.
* Self-hosted. No IIS, Apache, Ngix or other webserver configuration necessary.
* Offers all the same features as the built in official galleries. Download counting, star ratings, searching, auto-updating and so forth.
* Free!

:heart:

## How to install

__Todo__

Meanwhile please consult [this MSDN document](https://msdn.microsoft.com/en-us/library/hh266746.aspx) on the different options on installing a Private gallery in Visual Studio.

## How to configure
All configuration is stored in the `config.ini` file that must sit in the same directory as the main service executable.
__Todo__

## How it works
The service itself _just works_ and is a completely self-contained hosting solution.

The `.vsix` files, download and ratings data are stored in a subfolder of the main service executable (this subfolder is configurable). This makes taking backups and moving the service between machines as the entire folder contains the entire service current state.

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

## Retrieving data

### [GET] /feeds/atom.xml
This is the main entry point for the feed and serves up the Syndicate-Feed compatible Atom file containing all available extensions on the server. This URL should be used in Visual Studio.

See [How to install into Visual Studio](#how-to-install) for more information.

### [GET] /api/json
JSON feed for the entire package catalog. Same data that is being fed through the atom feed but just in a handier JSON format.

### [GET] /api/ratings/{vsix_id}
Retrieves the rating value and vote count for a particular VSIX package by its ID. The return type is JSON.

Example return

``` json
{
  "rating": 4.3,
  "count": 19
}
```

## Writing data
You can submit ratings for existing packages and upload new packages to the system.

### [POST/PUT] /api/ratings/{vsix_id}
Submitting rating values for a particular VSIX package by its ID. The post payload should be just raw string and contain a single floating point number on the range between [0,5].

### [POST/PUT] /api/upload
This endpoint accepts uploads of one or more .vsix files to the hosting service.


## Thanks to

This work is made possible by the fantastic job done on the following projects.

[Topshelf](https://github.com/Topshelf/Topshelf): Windows Service Hosting

[Nancy](https://github.com/NancyFx/Nancy): Embedded HTTP hosting

[VSGallery.AtomGenerator](https://github.com/garrettpauls/VSGallery.AtomGenerator): VSIX parsing logic

[Ini-Parser](https://github.com/rickyah/ini-parser): Configuration ini file parsing

[Costura](https://github.com/Fody/Costura/): Assembly merging
