#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export DOTNET_GENERATE_ASPNET_CERTIFICATE=false
export DOTNET_CLI_HOME="${DOTNET_CLI_HOME:-${TMPDIR:-/tmp}/screen-camera-dotnet}"
dotnet="${DOTNET:-dotnet}"
args=()
if [[ $# -gt 0 ]]; then
  source_file="$(realpath "$1")"
  args+=("-p:MainPageSource=$source_file")
fi
"$dotnet" run --project CameraSelection.csproj --configuration Release "${args[@]}"
