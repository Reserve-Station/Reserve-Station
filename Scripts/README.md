# Dev Scripts

After making changes to the code, you can use these scripts to build and run the server and client. You can also run tests to make sure your changes didn't break anything.

They come in two flavors: [`sh`](sh/) for Linux and Mac, and [`bat`](bat/) for Windows. The commands are the same, but the syntax is slightly different.

## Building

Run one of these after each change to any `.cs` files.

- `buildAllDebug` - Builds all projects with debug configuration
- `buildAllRelease` - Builds all projects with release configuration
- `buildAllTools` - Builds all projects with tools configuration

> The debug vs release build is simply what people develop in vs the actual server.
>
> The release build contains various optimizations, while the debug build contains debugging tools.
>
> If you're **mapping**, use the **release** or **tools** build as it will **run smoother with less crashes**.

## Running

- `runQuickAll` - Runs the client and server without building
- `runQuickClient` - Runs the client without building
- `runQuickServer` - Runs the server without building

## Testing

- `runTests` - Runs the unit tests, makes sure various C# systems work as intended
- `runTestsIntegration` - Runs the integration tests, makes sure various C# systems work as intended
- `runTestsYAML` - Runs the YAML linter and finds issues with the YAML files that you probably wouldn't otherwise
