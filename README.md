# ArasDeveloperTool
Command Line Application for handling developer tasks in Aras Innovator
####Features

- [ArasDeveloperTool](#arasdevelopertool)
  - [How To Use](#how-to-use)
    - [Setup an Aras connection (Setup)](#setup-an-aras-connection-setup)
    - [Aras Info](#aras-info)
    - [Check Latest Updates (CheckLatestUpdates)](#check-latest-updates-checklatestupdates)
    - [Check if Items are in packages (PackageChecker)](#check-if-items-are-in-packages-packagechecker)
    - [Backup/Restore Database (Database)](#backuprestore-database-database)
    - [AMLRunner](#amlrunner)
    - [ApplyMethod](#applymethod)

* Test Aras connection
* Check latest changes in Aras
* Check that everything is packaged, and package if needed. 
* Run Amls from files
* Check Aras Database Info
* Backup and Restore Databases


## How To Use
 
Download ArasDevTool and extract it to a folder. 
Open Cmd/Powershell/Windows Terminal and navigate to the folder and .

``` powershell
ArasDevTool.exe
```

``` 
Availible commands for: ArasDevTool
  AmlRunner
  ArasInfo
  BackupDB
  CheckLatestUpdates
  Dummy
  PackageChecker
  RestoreDB
  Setup
  TestConnection

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
  -env dev
  Non specified is equivalent with '-env dev'

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
  -env dev
  Non specified is equivalent with '-env dev'

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

### AMLRunner
Run AMLs from files

``` powershell
ArasDevTool.exe AMLRunner -mf \AMLRunner\AmlRunnerTest.mf
``` 

Example of manifest file running aml-files in the Pack1 and Pack2 directories. Where if the any aml fails in Pack2, execution will be stopped.
``` xml
<?xml version="1.0" encoding="utf-8" ?>
<imports>
  <package  path="Pack1" />
  <package path="Pack2" stopOnError="TRUE" />
</imports>
``` 


Example Test1.xml in directory Pack1
``` xml
<AMLS>
  <AML>
    <Item action="delete" type="Form" id="F596909BBC7746D99683DB1E759E582E">
    </Item>
  </AML>
   <AML>
    <Item action="delete" type="Field" id="53ADDFF20E5C43B7B51A5E4B818465E3">
    </Item>
  </AML>
</AMLS>
``` 

Or user the -file parameters to execute a single file of AML(s) like:

``` powershell
ArasDevTool.exe AMLRunner -file \AMLRunner\Pack1\Test1.xml
``` 

### ApplyMethod
Run an ApplyMethod

``` powershell
ArasDevTool.exe ApplyMethod -name HC_Motd
```

Output
``` powershell
Executing ApplyMethod
SUCCESS:
    Message of the day
    Don't forget to unlock your items Finish your tasks Be good
```
