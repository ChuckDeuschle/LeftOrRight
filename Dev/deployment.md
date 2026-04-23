[← Index](INDEX.md)

# Deployment

Ship WebGL builds of LeftOrRight to [itch.io](https://itch.io) via [butler](https://itch.io/docs/butler/), Itch's official upload CLI. You build locally in Unity, then run one command to push.

## One-time setup

### 1. Install butler

1. Download butler for Windows: https://broth.itch.ovh/butler/windows-amd64/LATEST
2. Extract to a stable path (e.g. `C:\tools\butler\`) and add that folder to your user PATH.
3. Open a new terminal so the PATH change takes effect, then run:
   ```
   butler login
   ```
   This opens a browser for Itch authentication. Token is stored in `%USERPROFILE%\.config\itch\butler_creds`.

### 2. Configure the Itch.io project page

In your Itch.io dashboard for the project, set:

- **Kind of project:** HTML
- **Embed options:** Embed in page, dimensions **960×600** (matches `PlayerSettings.WebGL` screen size)
- **Fullscreen button:** enabled

The Itch `html5` channel is created on first push — no dashboard action needed.

### 3. Fill in `deploy.sh`

At the repo root, edit [deploy.sh](../deploy.sh) and set `ITCH_USER` and `ITCH_PROJECT` to the values from your Itch dashboard URL.

## Building and publishing a release

1. **Bump the version** in `ProjectSettings/ProjectSettings.asset` — find the `bundleVersion:` line and set the new semantic version (e.g. `0.1.0`).
2. **Build WebGL** — in Unity, menu `Build > WebGL (Itch)`. The build script ([Assets/Editor/BuildScript.cs](../Assets/Editor/BuildScript.cs)) reads enabled scenes from Build Settings, switches the active target to WebGL if needed, sets Gzip compression, and writes to `Build/WebGL/`.
3. **Test locally** — WebGL won't run from `file://`. Serve the folder and open it:
   ```
   python -m http.server 8080 --directory Build/WebGL
   ```
   Open http://localhost:8080, confirm the splash screen loads and gameplay works.
4. **Push** — from Git Bash (not PowerShell — see section below), pass the same version you set in ProjectSettings:
   ```
   ./deploy.sh 0.1.0
   ```
   The script validates the build exists, pushes to `<user>/<project>:html5`, and prints channel status.
5. **Verify** — open the public Itch page in an incognito window and confirm the new build loads. If the embed still shows the old game, see *Troubleshooting* below.

## Running the deploy script (Git Bash on Windows)

Windows PowerShell can't execute `.sh` files directly — `./deploy.sh` just prompts to pick a program. Run it through Git Bash instead (you already have it because Git for Windows ships with it; the repo's `.githooks/pre-commit` depends on it).

1. **Open Git Bash at the repo folder.** In File Explorer, navigate to the repo root, right-click in the empty space → **Open Git Bash here** (on Windows 11, under *Show more options*).

   *Alternative:* Launch "Git Bash" from the Start menu, then:
   ```
   cd "/c/Chuck PC/GitHub/LeftOrRight/LeftOrRight"
   ```
   Git Bash maps `C:\` to `/c/`, and paths containing spaces need quoting.

2. **Confirm you're in the right place:**
   ```
   pwd && ls deploy.sh
   ```

3. **Confirm butler is reachable from bash.** Git Bash inherits the Windows PATH, so if PowerShell finds butler, bash will too:
   ```
   butler --version
   ```

4. **Run the deploy** with the version string you want published:
   ```
   ./deploy.sh 0.2.0
   ```

## Troubleshooting

**New build pushed but the Itch embed still shows the old game.** Either the iframe is browser-cached, or the project has more than one upload marked *"This file will be played in the browser"* and Itch is still serving the old one.

1. Hard-refresh the public page (Ctrl+F5), or open it in a fresh incognito window.
2. If it's still wrong, go to the project's **Edit game** page → **Uploads** section. Only the newest `left-or-right-html5.zip` should have *"This file will be played in the browser"* checked. Uncheck it on any older upload (or delete the old upload entirely), then click **Save** at the bottom of the page. Reload the public page.

## Batch-mode / CI entry point

[`BuildScript.BuildWebGL`](../Assets/Editor/BuildScript.cs) is a public static method, so the same build is callable from the Unity CLI without opening the Editor:

```
Unity.exe -batchmode -nographics -projectPath . -executeMethod BuildScript.BuildWebGL -quit
```

In batch mode the script exits with code 0 on success and 1 on failure. This is what a future GitHub Actions workflow (e.g. [game-ci/unity-builder](https://github.com/game-ci/unity-builder)) would invoke — no script changes needed to add CI later.

## Why Gzip (not Brotli)

[BuildScript.cs](../Assets/Editor/BuildScript.cs) forces `WebGLCompressionFormat.Gzip`. Brotli is smaller but requires the web server to send a `Content-Encoding: br` header for pre-compressed files; Itch.io doesn't guarantee that header, which leaves players with a broken build. Gzip works out of the box on Itch.
