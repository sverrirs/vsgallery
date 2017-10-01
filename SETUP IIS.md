# Setup and Configuring on IIS 

## How it works
The microservice is configured via the `----.XML` file that sits in the same folder as the main dll.

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

## TODO