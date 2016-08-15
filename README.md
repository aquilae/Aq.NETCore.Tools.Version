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
`dotnet version increment` - will increment PATCH number  
`dotnet version increment major` - will increment MAJOR number  
`dotnet version increment minor` - will increment MINOR number  
`dotnet version increment patch` - will increment PATCH number
