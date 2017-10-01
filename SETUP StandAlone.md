# Setup and Configuring as a Standalone Service

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

## Downloading

## Installing

### Install as a windows service
See more details in the [Topshelf](https://github.com/Topshelf/Topshelf) documentation.

## Configuration

## Common issues
