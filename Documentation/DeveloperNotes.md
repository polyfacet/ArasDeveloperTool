# Developer Notes

How to pack and test tool installation

``` powershell
# In ArasDevTool

# Create a nupkg
dotnet pack

# Install it globally
dotnet tool install --global --add-source ./nupkg ArasDevTool

# Uninstall it
dotnet tool uninstall --global ArasDevTool

```
