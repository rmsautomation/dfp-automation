# DFP Playwright Agent (Generator)

This generator reads `.feature` files and creates:
- Page Objects under `Pages/Generated/`
- Step Definitions under `StepDefinitions/Generated/`

It also loads selectors from:
`Artifacts/Selectors/selectors.<page>.json`

## Run (generate only)
Windows:
```powershell
powershell -ExecutionPolicy Bypass -File ".\DFP.Playwright\Artifacts\agent-generate.ps1"
```
macOS/Linux (PowerShell):
```bash
pwsh ./DFP.Playwright/Artifacts/agent-generate.ps1
```

## Run (end-to-end: watch + codegen + generate)
Windows:
```powershell
powershell -ExecutionPolicy Bypass -File ".\DFP.Playwright\Artifacts\agent-run.ps1" -PageKey login -BaseUrl "https://..."
```
macOS/Linux (PowerShell):
```bash
pwsh ./DFP.Playwright/Artifacts/agent-run.ps1 -PageKey login -BaseUrl "https://..."
```

## Notes
- The generator uses the Feature filename as the page key.
  Example: `Login.feature` -> page key `login` -> `selectors.login.json`
- Methods are generated with TODO_UI comments where selectors need manual mapping.
- If selectors exist and match step text, the agent will auto-generate click/wait actions.
- Steps call Page Objects only; no browser logic is added.

## Record selectors (manual)
Windows:
```powershell
powershell -ExecutionPolicy Bypass -File ".\DFP.Playwright\Artifacts\codegen-watch.ps1"
powershell -ExecutionPolicy Bypass -File ".\DFP.Playwright\Artifacts\codegen-start.ps1" -PageKey login -BaseUrl "https://..."
```
macOS/Linux (PowerShell):
```bash
pwsh ./DFP.Playwright/Artifacts/codegen-watch.ps1
pwsh ./DFP.Playwright/Artifacts/codegen-start.ps1 -PageKey login -BaseUrl "https://..."
```

## First-Time Setup (macOS/Linux)
You need PowerShell and the .NET SDK. On a fresh machine, run:
```bash
dotnet build ./DFP.Playwright/DFP.Playwright.csproj -c Debug
pwsh ./DFP.Playwright/bin/Debug/net9.0/playwright.sh install
```

Notes:
- `codegen-start.ps1` auto-builds if the Playwright script isn't found.
- The `install` step is needed once per machine to download browsers.
