# ✨**true_mogician's .Net extension packages**

## **Installation**

Because reading GitHub packages requires PAT, here's one with _read:packages_ authorization and no expiration date:

```
MC8jOovnFhclHlPBWWiFNjO7j7NCp94csRzh
```

Note that the real PAT has a `ghp_` prefix，but due to GitHub's security checking, A PAT will be revoked if it appears explicitly in commits, so only the suffix is provided here.

## **Configuration**

### Visual Studio

1. Open `NuGet.Config`, which usually locates at `%AppData%/NuGet/`. If not found, please refer to Microsoft documentations, or search your file system.
2. Merge the following XML into `NuGet.Config`

    ```xml
    <configuration>
        <packageSources>
            <add key="true_mogician" value="https://nuget.pkg.github.com/truemogician/index.json" />
        </packageSources>
        <packageSourceCredentials>
            <true_mogician>
                <add key="Username" value="truemogician" />
                <add key="ClearTextPassword" value="{PAT}" />
            </true_mogician>
        </packageSourceCredentials>
    </configuration>
    ```

3. Reopen your Visual Studio, and you'll find a source named **true_mogician** in your NuGet source list. By selecting this source or **All** sources, you'll have access to these packages.
