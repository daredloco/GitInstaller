# GitInstaller
A simple installer for software released on GitHub for Windows

![Installer Preview](https://www.rowa-digital.ch/installer.png)


## Usage
The installer is very easy to setup.
All you have to do is to change the values inside the config.json file and you're ready to go.

```json
{
    "project": "GitInstaller",
    "user": "daredloco",
    "repo": "GitInstaller",
    "unzip": true,
    "preview": true,
    "uninstall": true,
    "ignored-tags": ["v1.0"],
    "ignored-files": ["somefiletoignore.zip"]
}
```

- project: The name of the project shown as Window Title
- user: Your github username or the name of your company
- repo: The name of your repository
- unzip: Will unpack all zip files to the folder selected by the user and delete the zip files afterwards
- preview: If true, preview releases will be included, if false they'll not but the user will always be able to enable them afterwards.
- uninstall: If true, uninstall informations will be saved so the user can easily uninstall the software
- ignored-tags: Releases with this tag will be ignored
- ignored-files: Files with this name will be ignored


You can also add a zipsettings.json file to handle special cases like subdirectories for certain folders/files:

```json
{
    "Subfolders": [
        { "Sub1": [ "GitInstaller.exe", "config.json" ] },
        { "Sub2": [ "FileThatDoesntExist.jpg" ] }
    ]
}
```
Create an array named "Subfolders" and create an object with the name of the subfolder as key and the files included as value (array).
Afterwards add this file to your zip archive. It won't be unpacked!

## Manual Installation
If the Installer won't find a "config.json" file in the same directory, it will show a prompt where you can enter the url of the repository.
WARNING: This is a preview function so exception handling isn't completely done!

## Licenses
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md)
- [Fody](https://github.com/Fody/Fody/blob/master/License.txt) 
