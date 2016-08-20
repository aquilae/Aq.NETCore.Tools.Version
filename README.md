# `Aq.NETCore.Tools.Version`

## project.json version manipulation CLI

### Usage

- Reference in project.json `tools` section:  
```json
{
	"tools": {
		"Aq.NETCore.Tools.Version": "0.0.1"
	}
}
```

- In project directory:  
```bash
dotnet version --help # print usage info
dotnet version path/to/project.json --increment=patch # increment PATCH number of specified project.json by 1
dotnet version -i minor -v 2 -d json # increment MINOR number of ./project.json by 2 and print out new version in JSON format
```
