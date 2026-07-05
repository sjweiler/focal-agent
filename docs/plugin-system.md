# FocalAgent Plugin System

FocalAgent plugins are external processes described by a local manifest. The host starts a plugin, sends one JSON request through standard input, reads one JSON response from standard output, then exits the process.

## Layout

Install plugins under the app's `plugins` directory:

```text
plugins/
  inventory/
    plugin.json
    focal-agent-inventory-plugin.exe
```

During development, files under `FocalAgent/PluginPackages` are copied to the build output as `plugins`.

## Manifest

Each plugin folder must contain `plugin.json`:

```json
{
  "id": "focal-agent.inventory",
  "name": "Inventory",
  "version": "0.1.0",
  "description": "Read-only Windows system inventory.",
  "executable": "focal-agent-inventory-plugin.exe",
  "commands": [
    {
      "method": "inventory.collect",
      "description": "Collect host, OS, CPU, and memory inventory.",
      "requiresPermission": true
    }
  ]
}
```

`executable` can be absolute or relative to the manifest directory.

## Protocol

Request:

```json
{
  "method": "inventory.collect",
  "args": {}
}
```

Success response:

```json
{
  "success": true,
  "data": {
    "hostname": "DESKTOP-12345"
  }
}
```

Error response:

```json
{
  "success": false,
  "error": "unknown method: inventory.missing"
}
```

## Current Inventory Commands

- `inventory.collect`
- `disks.list`
- `network.list`
- `processes.list`
- `services.list`

## Safety Model

Version 1 is permission-first. Commands declare `requiresPermission`; FocalAgent asks the user before execution. Plugins should be read-only unless a future manifest capability explicitly declares write access.
