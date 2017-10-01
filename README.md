# Visual Studio Private Gallery

Ultra simple Visual Studio extension Gallery implementation for Visual Studio 2010 and newer. 
This solution provides a fully featured service for the Extension Gallery feature available in Microsoft Visual Studio.

1. Self-contained and Self-hosted 
Offering a single Executable for an easy one click deployment and super easy configuration 

2. IIS Hosted Asp WebApi Service
Same Implementation as above only as an Asp.Net Web API implementation which can be hosted in an Microsoft IIS 

<p align="center">
  <img src="https://raw.githubusercontent.com/sverrirs/vsgallery/master/img/extension-manager-01.png" />
</p>

See my blog for more details [blog.sverrirs.com](https://blog.sverrirs.com/2017/05/vs-selfhosted-extension-gallery.html)

Interested in the API? [View the API documentation](API.md).

## Features

* Fully featured Extension Gallery ready to use in Microsoft Visual Studio. 
* Counts downloads of extensions
* Displays star ratings, release notes and links to project home pages
* Offers a simple to use REST API to submit ratings and upload new VSIX packages
* Atom and JSON feeds for available packages
* Free!

:heart:

## Downloading and installing

From the release page choose the latest version and download the appropriate release you are interested in.

1. standalone.zip: For running vsgallery as a stand alone Windows/Linux service or console application. [View setup guide for stand-alone](SETUP%20StandAlone.md).

2. IIS.zip: For a Asp.NET Weg API implementation that can be hosted in a Microsoft IIS environment. [View setup guide for IIS](SETUP%20IIS.md).

> Note: Currently this is under construction, separate release packages are being implemented.

## How to install into Visual Studio

In Visual Studio

```
Tools > Options > Environment > Extensions and Updates
```

Add a new entry and copy in the URL of the main Microservice Atom Feed.

> By default the URL for the self-hosted executable is `http://YOUR_SERVER:5100/feeds/atom.xml`

Please consult [this MSDN document](https://msdn.microsoft.com/en-us/library/hh266746.aspx) for any further details and alternative options on how to install a Private Extension Gallery in Visual Studio.

## Thanks to

This work is made possible by the fantastic job done on the following projects.

[Topshelf](https://github.com/Topshelf/Topshelf): Windows Service Hosting

[Nancy](https://github.com/NancyFx/Nancy): Embedded HTTP hosting

[VSGallery.AtomGenerator](https://github.com/garrettpauls/VSGallery.AtomGenerator): VSIX parsing logic

[Ini-Parser](https://github.com/rickyah/ini-parser): Configuration ini file parsing

[Costura](https://github.com/Fody/Costura/): Assembly merging


## Contributing

> Although this project is small it has a [code of conduct](CODE_OF_CONDUCT.md) that I hope everyone will do their best to follow when contributing to any aspects of this project. Be it discussions, issue reporting, documentation or programming. 

If you don't want to open issues here on Github, send me your feedback by email at [vsgallery@sverrirs.com](mailto:vsgallery@sverrirs.com).

1. Fork it ( https://github.com/sverrirs/vsgallery/fork )
2. Create your feature branch (`git checkout -b my-new-feature`)
3. Test your changes locally by adding new and running the existing unit-tests
4. Commit your changes (`git commit -am 'Add some feature'`)
5. Push to the branch (`git push origin my-new-feature`)
6. Create new Pull Request

Note: This project uses [semantic versioning](http://semver.org/).