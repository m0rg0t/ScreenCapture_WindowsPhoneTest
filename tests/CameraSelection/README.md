# Camera selection regression tests

This dependency-free .NET 10 console harness links the **entire actual**
`VideoWatermark1/MainPage.xaml.cs`. It does not copy the selection logic.
Both the production page and test code compile with `LangVersion=5`.

Run from the repository root with a .NET 10 SDK installed:

```sh
tests/CameraSelection/run-tests.sh
# Or supply an installed SDK executable:
DOTNET=/path/to/dotnet tests/CameraSelection/run-tests.sh
```

The local NuGet configuration clears all package sources. No packages, Windows
Phone SDK, cameras, media libraries, screen capture, network, user files, or
authentication credentials are used by the tests. `PlatformStubs.cs` is explicitly
test-only: camera capabilities and instances are in memory; unrelated platform
operations throw if called. The real TotpHelper is neither linked nor executed.

The four scenarios assert:
- Primary-only devices select the primary camera
- Front-only devices select the front-facing camera
- Devices with both cameras prefer front-facing, matching the existing branch
  and comment
- Devices with neither camera allocate nothing and leave the viewfinder unset

For supported cameras, tests also check viewfinder binding, all four event
subscriptions, and disposal/unsubscription on departure. These checks exercise
existing behavior; the production change only corrects the front-facing
constructor argument and removes its obsolete commented-out duplicate.

## Baseline regression check

Export `VideoWatermark1/MainPage.xaml.cs` from original commit
`68049fa7eab723706f4e36a498184bbff16e6497` to a temporary file, then run:

```sh
tests/CameraSelection/run-tests.sh /absolute/path/to/MainPage.original.cs
tests/CameraSelection/run-tests.sh
```

Verified with .NET SDK 10.0.401:
- Original source: 2 pass, 2 fail; exit code 1. Front-only throws
  "Unsupported camera: Primary"; both-camera scenario selects the wrong type
- Fixed source: 4 pass, 0 fail; exit code 0

The compiler reports pre-existing unused-variable/unawaited-call warnings from
the production page and unused-event warnings from the test stubs. These are not
changed or hidden by this narrow fix.

This harness verifies managed control flow against a deliberately limited model.
It is **not** a Windows Phone 8.1 build, device/emulator test, recording test, or
validation of actual platform APIs. The production project remains Windows Phone
8.1 and requires its original legacy toolchain for a real app build.
