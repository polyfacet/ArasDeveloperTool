# ArasDeveloperTool
Command Line Application for handling developer tasks in Aras Innovator
####Features
* Test Aras connection
* Check latest changes in Aras
* Check that everything is packaged, and package if needed. 

## How To Use
 
Download ArasDevTool and extract it to a folder. 
Open Cmd/Powershell/Windows Terminal and navigate to the folder and .

``` powershell
ArasDevTool.exe
```

``` 
Availible commands for: ArasDevTool
  dummy
  setup
  arasinfo
  checklatestupdates
  packagechecker
  testconnection

Options:
  --help    Displays help for a command

```

### Setup an Aras connection (Setup)

``` powershell
ArasDevTool.exe Setup
``` 
![Setup example](Documentation/img/SetupConnection.png)

``` powershell
ArasDevTool.exe Setup -ext
#Extended setup: Configure Database for Backup/Restore
``` 

It is an option to explicity use a "Aras ConnectionString" in Aras Commands:
E.g. 

``` powershell
ArasDevTool.exe TestConnection --cs="http://localhost/Innovator;InnovatorSolutions;admin;innovator"
#Or
ArasDevTool.exe TestConnection --cs="http://localhost/Innovator;InnovatorSolutions;admin"
# To be promted for password
``` 




### Aras Info

``` powershell
ArasDevTool.exe ArasInfo
``` 
![Setup example](Documentation/img/ArasInfo.png)


### Check Latest Updates (CheckLatestUpdates)
``` powershell
ArasDevTool.exe CheckLatestUpdates 
``` 
![CheckLatestUpdates example](Documentation/img/CheckLatestUpdates.png)


``` powershell
ArasDevTool.exe CheckLatestUpdates --help
``` 
``` 
Help for: CheckLatestUpdates
  Specify connection string: E.g.
   -cs="http://localhost/innovator;InnovatorSoluions;admin;innovator"
   -cs="http://localhost/innovator;InnovatorSoluions;admin"
  Or environment: E.g.
  -env=dev
  Non specified is equivalent with '-env=dev'

Options: -c
  (Number of items to show: "-c 20"

```


### Check if Items are in packages (PackageChecker)
``` powershell
ArasDevTool.exe PackageChecker -prefix HC_
``` 
![Package Checker example](Documentation/img/PackageChecker.png)


``` powershell
ArasDevTool.exe CheckLatestUpdates --help
``` 
``` 
Help for: PackageChecker
  Specify connection string: E.g.
   -cs="http://localhost/innovator;InnovatorSoluions;admin;innovator"
   -cs="http://localhost/innovator;InnovatorSoluions;admin"
  Or environment: E.g.
  -env=dev
  Non specified is equivalent with '-env=dev'

  Parameters:
    -prefix        Check Items with specific prefix. Example: "-prefix HC_"
  Options:
  --DryRun       Dont commit any changes.
  --Auto         Let it automatically select package for you.
```


### Backup/Restore Database (Database)
If you have setup the extended setup database backup and restore is possible via

``` powershell
ArasDevTool.exe BackupDB 
``` 

``` powershell
ArasDevTool.exe RestoreDB
``` 