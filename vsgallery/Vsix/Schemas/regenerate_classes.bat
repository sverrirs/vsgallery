set xsd="C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.2 Tools\x64\xsd.exe"

%xsd% "%~dp0\PackageManifest\PackageManifestSchema.xsd" /classes /language:CS /namespace:vsgallery.Vsix.Schemas /out:%~dp0

%xsd% "%~dp0\VsixManifest\VsixManifestSchema.xsd" /classes /language:CS /namespace:vsgallery.Vsix.Schemas /out:%~dp0

