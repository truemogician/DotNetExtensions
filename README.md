# :sparkles:**true_mogician's .Net extension packages**

## **How to install** :question:
Because reading GitHub packages requires PAT, here's one with *read:packages* authorization and no expiration date: 
```
Fcq40ox6sYggIqQ0ATvHmocmWhCxvL4evQtT
```
Note that the real PAT has a `ghp_` prefix，but due to GitHub's security checking, A PAT will be revoked if it appears explicitly in commits, so only the suffix is provided here.

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
                <add key="ClearTextPassword" value="ghp_{PAT provided above}" />
            </true_mogician>
        </packageSourceCredentials>
    </configuration>
    ```
3. ### Reopen your Visual Studio, and you'll find a source named **true_mogician** in your NuGet source list. By selecting this source or **All** sources, you'll get access to these packages.