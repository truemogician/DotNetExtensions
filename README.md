# :sparkles:**true_mogician's .Net extension packages**

## **How to install** :question:
Because reading GitHub packages requires PAT, here's one with *read:packages* authorization and no expiration date: 
```
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
```

## **How to configurate Visual Studio's package manager to work with the PAT** :question:
1. ### Go to the location of your ***NuGet.Config***, which usually locates at ***%appdata%/NuGet/NuGet.Config***. If not found, please refer to Microsoft Docs, or search your file system.
2. ### Merge the following XML into ***NuGet.Config***
    ```xml
    <configuration>
        <packageSources>
            <add key="true_mogician" value="https://nuget.pkg.github.com/truemogician/index.json" />
        </packageSources>
        <packageSourceCredentials>
            <true_mogician>
                <add key="Username" value="truemogician" />
                <add key="ClearTextPassword" value="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" />
            </true_mogician>
        </packageSourceCredentials>
    </configuration>
    ```
3. ### Reopen your Visual Studio, and you'll find a source named **true_mogician** in your NuGet source list. By selecting this source or **All** sources, you'll get access to these packages.