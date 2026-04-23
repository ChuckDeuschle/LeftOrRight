#!/usr/bin/env bash
set -euo pipefail

# Deploy the WebGL build in Build/WebGL to Itch.io via butler.
# Build it first with the Unity menu: Build > WebGL (Itch).

VERSION="${1:?usage: ./deploy.sh <version>   e.g. ./deploy.sh 0.1.0}"

# Replace with your Itch project slug (find it in your Itch dashboard URL).
ITCH_USER="cldeuschle"
ITCH_PROJECT="left-or-right"
ITCH_CHANNEL="html5"

BUILD_DIR="Build/WebGL"

if [ "$ITCH_USER" = "<itch-username>" ] || [ "$ITCH_PROJECT" = "<project-slug>" ]; then
  echo "Edit deploy.sh and set ITCH_USER and ITCH_PROJECT before running." >&2
  exit 1
fi

if [ ! -f "$BUILD_DIR/index.html" ]; then
  echo "No build found at $BUILD_DIR/index.html." >&2
  echo "In Unity, run the menu: Build > WebGL (Itch)" >&2
  exit 1
fi

if ! command -v butler >/dev/null 2>&1; then
  echo "butler not on PATH. Install from https://itch.io/docs/butler/installing.html" >&2
  exit 1
fi

TARGET="${ITCH_USER}/${ITCH_PROJECT}:${ITCH_CHANNEL}"

echo "Pushing $BUILD_DIR -> $TARGET (version $VERSION)"
butler push "$BUILD_DIR" "$TARGET" --userversion "$VERSION"

echo
echo "Status:"
butler status "${ITCH_USER}/${ITCH_PROJECT}"
