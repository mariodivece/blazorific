# Blazorific
An experimental Blazor Control Library
You need to check out the sample application in this repo as documentation is still a WIP.

## Adding This Package Source
 - Simply create a `nuget.config` file in the same directory as your solution file
 - The token used could be your own PAT or the generic one I have provided here.
 - The file has to look something like this:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
 <packageSources>
    <add key="mariodivece" value="https://nuget.pkg.github.com/mariodivece/index.json" />
 </packageSources>
 <packageSourceCredentials>
  <mariodivece>
   <add key="Username" value="token" />
   <add key="ClearTextPassword" value="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" />
  </mariodivece>
 </packageSourceCredentials>
</configuration>
```
