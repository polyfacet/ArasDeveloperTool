# Developer Notes

How to pack and test tool installation

``` powershell
# In ArasDevTool

# Create a nupkg
dotnet pack

# Install it globally
dotnet tool install --global --add-source ./nupkg ArasDeveloperTool
# Or the pre release
dotnet tool install --global --prerelease --add-source ./nupkg ArasDeveloperTool


# Uninstall it
dotnet tool uninstall --global ArasDeveloperTool

```
