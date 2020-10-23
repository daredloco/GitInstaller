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
    "targetdir": "",
    "unzip": true,
    "preview": true,
    "ignored-tags": ["v1.0"],
    "ignored-files": ["GitInstaller.zip"]
}
```

- project: The name of the project shown as Window Title
- user: Your github username or the name of your company
- repo: The name of your repository
- targetdir: No use at the moment
- unzip: Will unpack all zip files to the folder selected by the user and delete the zip files afterwards
- preview: If true, preview releases will be included, if false they'll not but the user will always be able to enable them afterwards.
- ignored-tags: Releases with this tag will be ignored
- ignored-files: Files with this name will be ignored

Example from a github link: https://github.com/{user}/{repo}/

## Licenses
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md)
- [Fody](https://github.com/Fody/Fody/blob/master/License.txt) 
