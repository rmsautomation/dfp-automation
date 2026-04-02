# Historial de sesiones Claude

---

## 44. Sesión: 2026-03-31 — Reports TC1107: Saved reports ng-select, resultados tabla, download Excel

### Nuevos métodos en ReportsPage.cs
- `IClickOnInvoicesOption()` — click en `a[href*='/reports/invoices']`
- `SelectSavedReportAsync(name)` — abre ng-select `[name='selectedReport']` con click en `.ng-arrow-wrapper`, luego click en `.ng-option` que contiene el texto del parámetro
- `ShouldSeeInvoicesInReportResultsAsync()` — espera hasta 30s que `table tbody tr` sea visible y count > 0
- `ClickButtonAsync(string)` — click genérico sin lógica de dialog (para botones de Reports que no abren dialogs)
- `ClickDownloadToExcelAsync()` — captura el download con `Page.RunAndWaitForDownloadAsync` → retorna `IDownload`
- `VerifyExcelRowCountAsync(IDownload, int)` (static) — guarda el archivo en temp, lee el xlsx como ZIP (System.IO.Compression), parsea `xl/worksheets/sheet1.xml` con XDocument, cuenta `<row>` elements y compara con el esperado. **Sin NuGet extra** — solo System.IO.Compression + System.Xml.Linq

### Nuevos steps en ReportsStepDefinitions.cs
- `I click on "Invoices" option`
- `I select saved reports with name {string}` (Given/When/Then)
- `I should see the invoices in the reports Results` (Given/When/Then)
- `I click on {string} button` con `[Scope(Feature = "Reports")]` — evita conflicto con el global de ShipmentStepDefinitions que espera dialog; si el texto contiene "Download" → usa `ClickDownloadToExcelAsync` + guarda `_lastDownload`; si no → `ClickButtonAsync`
- `I should verify the downloaded excel contains {string} rows` (Given/When/Then)

### Patrón de descarga de archivos (reusar en futuros tests)
```csharp
// En Page: capturar descarga
public async Task<IDownload> ClickDownloadXxxAsync()
{
    var btn = Page.Locator("button").Filter(new LocatorFilterOptions { HasText = "Download..." }).First;
    await btn.WaitForAsync(...Visible, 15000);
    await WaitForEnabledAsync(btn, 15000);
    return await Page.RunAndWaitForDownloadAsync(() => btn.ClickAsync());
}

// En StepDefinitions: almacenar y verificar
private IDownload? _lastDownload;
// Step click → _lastDownload = await _page.ClickDownloadXxxAsync();
// Step verify → await Page.VerifyExcelRowCountAsync(_lastDownload!, expectedRows);

// Para xlsx sin NuGet (System.IO.Compression + System.Xml.Linq):
using var archive = ZipFile.OpenRead(filePath);
var entry = archive.GetEntry("xl/worksheets/sheet1.xml");
var doc = XDocument.Load(entry.Open());
XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
var rowCount = doc.Descendants(ns + "row").Count(); // incluye header
```

### Archivos modificados
- `DFP.Playwright/Pages/Web/ReportsPage.cs` — using System.IO.Compression/Xml.Linq; 5 métodos nuevos
- `DFP.Playwright/StepDefinitions/ReportsStepDefinitions.cs` — `IDownload? _lastDownload`; 5 nuevos steps

---

## 45. Sesión: 2026-03-31 — Inventory TC1307: página completa, steps, DI, feature con tabla

### Nuevos archivos
- `DFP.Playwright/Pages/Web/InventoryPage.cs` — página completa con TestContext para `PortalOrigin()`
- `DFP.Playwright/StepDefinitions/InventoryStepDefinitions.cs` — 7 step bindings con Given/When/Then
- `DFP.Playwright/Features/Inventory.feature` — escenario @1307 @INT con tabla de datos

### Métodos en InventoryPage.cs
- `NavigateToInventoryPageAsync()` → `/my-portal/warehouse-inventory`
- `VerifyInventoryPageVisibleAsync()` → `h3.font-weight-normal.m-0` HasText "Your warehouse inventory"
- `SearchInventoryItemAsync(fieldLabel, value)` → `input[placeholder='{fieldLabel}']`
- `ClickButtonAsync(string)` — click genérico sin lógica de dialog (scoped a Inventory)
- `VerifyInventoryItemVisibleInListAsync()` — loop de 3 min, da click en Search cada 2s mientras no hay filas en `.p-datatable-tbody tr`
- `SelectInventoryItemFromListAsync(text)` → `.p-datatable-tbody tr` Filter(HasText) + click
- `VerifyInventoryItemDetailsPageAsync()` → `h5.font-weight-normal.m-0` "Definition" visible+enabled
- `VerifyInventoryDetailLabelAsync(label, value)` → XPath `//label[normalize-space()='{label}']/following-sibling::div[1]` (mismo patrón que InvoicePage)
- `VerifyInventoryDetailsAsync(pairs)` — itera sobre tabla de datos y llama VerifyInventoryDetailLabelAsync

### Steps en InventoryStepDefinitions.cs
- `I am on the Inventory page` → NavigateToInventoryPageAsync
- `the inventory page should be visible` → VerifyInventoryPageVisibleAsync
- `I search for the inventory item {string} with value {string}` → SearchInventoryItemAsync
- `I click on {string} button` con `[Scope(Feature = "Inventory")]` — evita conflicto con el global
- `the inventory item should be visible in the List` → VerifyInventoryItemVisibleInListAsync
- `I select the inventory item from the list with text {string}` → SelectInventoryItemFromListAsync
- `I should see the inventory item details page` → VerifyInventoryItemDetailsPageAsync
- `I should verify the following inventory item details:` → tabla con pairs (label, value)

### Archivos modificados
- `DFP.Playwright/Support/DependencyInjection.cs` — InventoryPage registrado con `new InventoryPage(tc.Page!, tc)`
- `DFP.Playwright/Features/Inventory.feature` — step de verificación con tabla de 8 campos (Part number, Model, Description, Manufacturer, Customer, Amount per pallet, Packaging, Commodity type)

---

## 43. Sesión: 2026-03-31 — TC10899: step descripción attachment + fixes upload flow Invoice

### Problema 1: `When I click on 'Upload attachment' button` — Timeout por PrimeNG backdrop
El botón `<button class="btn btn-primary"> Upload attachment </button>` está dentro de un diálogo PrimeNG. La clase `p-dialog-mask p-component-overlay-enter` es permanente mientras el diálogo está abierto (no es una transición temporal), por lo que esperar a que desaparezca nunca funciona.
**Fix:** `ClickButtonByTextAsync` usa `Force = true` para bypasear el backdrop.
```csharp
await btn.ClickAsync(new LocatorClickOptions { Force = true });
```

### Problema 2: `Then I should see the screen to upload the attachment` — texto cambió
El texto anterior "Select a file from your system" ya no existe en el HTML. El nuevo texto es "Files you upload here will be visible to your customer".
**Fix:** `ShouldSeeUploadScreenAsync` intenta ambos textos con `TryFindLocatorAsync`.

### Problema 3: `Then I click on Drop your file here option` — step undefined
No existía el step binding para esperar el dropzone.
**Fix:** Nuevo step `I click on Drop your file here option` → `WaitForDropzoneAsync()` (global, sin scope).

### Nuevo step: `I enter the description {string} for the attachment`
TC10899 necesita completar el textarea de descripción antes de hacer upload.
HTML: `<textarea id="description" formcontrolname="description" rows="3" ...>`
**Implementación:** `EnterAttachmentDescriptionAsync(description)` — espera visible + enabled, limpia, y tipea carácter a carácter con `TypeAsync`.

### Archivos modificados
- `DFP.Playwright/Pages/Web/ShipmentPage.cs` — `ClickButtonByTextAsync` con `Force = true`; `ShouldSeeUploadScreenAsync` con ambos textos; `WaitForDropzoneAsync`; nuevo `EnterAttachmentDescriptionAsync`
- `DFP.Playwright/StepDefinitions/ShipmentStepDefinitions.cs` — steps `I click on {string} button`, `I click on Drop your file here option`, `I enter the description {string} for the attachment`

---

## 42. Sesión: 2026-03-31 — Múltiples fixes: ambigüedad steps, selectores, Invoice retry logic

### Problema 1: CS0104 — TestContext ambiguo en InvoicePage, WarehouseReceiptPage, PurchaseOrderPage, ReportsPage
Después de cambiar constructores a `(IPage, TestContext tc)`, el compilador no sabía si `TestContext` era `DFP.Playwright.Support.TestContext` o `Microsoft.VisualStudio.TestTools.UnitTesting.TestContext`.
**Fix:** `using TestContext = DFP.Playwright.Support.TestContext;` en los 4 archivos.

### Problema 2: Step `I should see the uploaded file` ambiguo entre Shipment y Warehouse
`ShipmentStepDefinitions` y `WarehouseReceiptStepDefinitions` tenían el mismo texto de binding.
**Fix:** `[Scope(Feature = "Shipments")]` en ShipmentStepDefinitions y `[Scope(Feature = "Warehouse Receipts")]` en WarehouseReceiptStepDefinitions.

### Problema 3: Step `I enter the created email "" in the Portal` no existía
TC1483 necesitaba introducir el email del contacto creado en Hub (guardado en `ScenarioContext["ContactEmail"]`).
**Fix:** Nuevo step `I enter the created email {string} in the Portal` en `LoginPortalHubStepDefinitions` — si vacío, usa `ContactEmail` → fallback `usernamePortal`.

### Problema 4: Reset button en Shipments no encontrado
Selectores buscaban por texto pero el botón tiene `type="reset"` con texto con espacios (` Reset `).
**Fix:** Selectores actualizados a `button[type='reset']` como primero; timeout de `WaitForEnabledAsync` a 2 min.

### Problema 5: Booked quotation selector no encontraba nada
Selectores con `qwyk-quotation-card`, `qwyk-quotation-list-item` no existían en el DOM real. El DOM usa `<li>` con `<a href="/my-portal/quotations/{uuid}">`.
**Fix:** Selectores reescritos con `li:has-text('Booked') a[href*='/my-portal/quotations/']` + UUID discriminado por guión (`-`) para excluir nav links.

### Problema 6: `IClickOnSearchButton` (ShipmentPage) muy rápido — lista sin filtro aplicado
`ClickAndWaitForNetworkAsync` dispara antes que Angular re-renderice la lista.
**Fix:** `await Page.WaitForLoadStateAsync(LoadState.NetworkIdle)` + `await Page.WaitForTimeoutAsync(1000)` después del click.

### Problema 7: Invoice retry loop completaba en 0.1s sin esperar la factura
`//div[contains(normalize-space(),'invoice')]` encontraba inmediatamente el header estático "Invoice number". Ambos métodos (`SelectInvoiceInSearchResultsWithTextAsync` y `TheInvoiceShouldAppearInSearchResultsInListAsync`) afectados.
**Fix:** Detectar `h5:has-text('No Invoices found')` como señal de "sin resultados". Mientras esté visible → click Search cada 2s, máximo 3 min. Cuando desaparece + hay `a[href*='/invoices/']` real → retornar (o hacer click).

### Problema 8: Save changes timeout 10s insuficiente en ShipmentHub
**Fix:** Timeout de `editBtn.WaitForAsync` de 10s a 60s.

### Archivos modificados
- `DFP.Playwright/Pages/Web/InvoicePage.cs` — alias `using TestContext`, reescrita lógica retry
- `DFP.Playwright/Pages/Web/WarehouseReceiptPage.cs` — alias `using TestContext`, Scope
- `DFP.Playwright/Pages/Web/PurchaseOrderPage.cs` — alias `using TestContext`
- `DFP.Playwright/Pages/Web/ReportsPage.cs` — alias `using TestContext`
- `DFP.Playwright/Pages/Web/ShipmentPage.cs` — Reset selectors, Search wait, Booked quotation selectors
- `DFP.Playwright/Pages/Web/ShipmentHubPage.cs` — timeout 60s save changes
- `DFP.Playwright/StepDefinitions/ShipmentStepDefinitions.cs` — Scope + Given/When/Then a uploaded file
- `DFP.Playwright/StepDefinitions/WarehouseReceiptStepDefinitions.cs` — Scope uploaded file
- `DFP.Playwright/StepDefinitions/LoginPortalHubStepDefinitions.cs` — nuevo step email creado

---

## 41. Sesión: 2026-03-30 — Patrón definitivo: TestContext.ActivePortalBaseUrl para todas las pages de portal

### Problema / Consulta
El patrón `_baseUrl` inyectado vía `BASE_URL` env var fallaba cuando el test usaba `@INT` vs `@NOINT` — ambas variantes leían distintas variables y la lógica condicional era propensa a errores. `ReportsPage` usaba `Page.Url` con fallback a env vars y navegaba a la URL INT en escenarios NOINT. `PurchaseOrderPage` quedó con un método `Unused_Delete` placeholder roto que impedía compilar.

### Solución / Regla establecida
**Patrón definitivo**: `TestContext.ActivePortalBaseUrl` es la única fuente de verdad para navegación portal.
- Se asigna en el paso de login (`GivenILoginToPortal`, `GivenILoginToPortalWithIntegration`) y en `PlaywrightHooks.EnsureLoggedInAsync()`.
- Todas las pages leen `_tc.ActivePortalBaseUrl` con fallback `Page.Url` origin solo si está vacío.
- No se usa `BASE_URL`, `PORTAL_BASE_URL` ni `PORTAL_INT_BASE_URL` para construir destinos de navegación en pages — solo en los pasos de login para saber a dónde navegar inicialmente.

### Archivos modificados
- `DFP.Playwright/Pages/Web/PurchaseOrderPage.cs` — eliminado método `Unused_Delete` roto; constructor usa `TestContext tc`
- `DFP.Playwright/Pages/Web/ReportsPage.cs` — constructor cambiado a `(IPage page, TestContext tc)`; `PortalOrigin()` usa `_tc.ActivePortalBaseUrl`
- `DFP.Playwright/Support/DependencyInjection.cs` — `WarehouseReceiptPage`, `PurchaseOrderPage`, `InvoicePage`, `ReportsPage` registradas pasando `tc` en lugar de `baseUrl`

---

## 40. Sesión: 2026-03-30 — Regla: Pages DFP deben usar BASE_URL, nunca Page.Url

### Problema / Consulta
`WarehouseReceiptPage` e `InvoicePage` derivaban la URL base de `new Uri(Page.Url).GetLeftPart(UriPartial.Authority)`. Si el test navega primero a un portal externo (ej. LiveTrack en `tracking.magaya.com`), la navegación siguiente iría al sitio equivocado.

### Solución / Regla establecida
Toda `XxxPage` que navegue al portal DFP debe:
1. Recibir `string baseUrl` en el constructor y guardar como `_baseUrl = baseUrl.TrimEnd('/')`
2. Usar `_baseUrl` en todos los `Page.GotoAsync(...)`
3. Registrarse en `DependencyInjection.cs` con `Environment.GetEnvironmentVariable("BASE_URL") ?? ""`

`Page.Url` solo se usa para leer la URL actual (assertions), nunca para construir destinos de navegación.

Excepción: pages que navegan a URLs externas fijas (ej. `LiveTrackPage`) hardcodean su URL como `const` — no necesitan `baseUrl`.

### Archivos modificados
- `DFP.Playwright/Pages/Web/WarehouseReceiptPage.cs` — constructor + `_baseUrl` reemplaza `Page.Url`
- `DFP.Playwright/Pages/Web/InvoicePage.cs` — constructor + `_baseUrl` reemplaza `Page.Url`
- `DFP.Playwright/Support/DependencyInjection.cs` — ambas pages registradas con `BASE_URL`
- Memoria guardada en `feedback_base_url.md`

---

## 39. Sesión: 2026-03-30 — LiveTrack: 5 nuevos métodos de interacción con portal externo

### Problema / Consulta
Se necesitaban 5 nuevos métodos en `LiveTrackPage.cs` para interactuar con el portal externo ExtJS LiveTrack (tracking.magaya.com) después del login, junto con sus step definitions.

### Solución

#### LiveTrackPage.cs — Métodos agregados
- `GoToInvoicesAsync()` — click en `(//strong[text()='Invoices'])[2]` con wait de 30s (la página puede demorar en cargar)
- `FilterByNumberAsync(number)` — espera a que aparezca `div#combo-1079-trigger-picker` (grilla cargada), hace click en el trigger, click en `a#splitbutton-1176` (botón Filter), escribe el número en `input#textfield-1234-inputEl`
- `ClickOkButtonAsync()` — click en `//span[contains(@class,'x-btn-inner') and normalize-space()='OK']`
- `RefreshViewAsync()` — click en `//button[.//*[contains(@class,'fa-refresh')]]`
- `ApproveInvoiceAsync(name, comment)` — right-click en `//div[.='{name}']`, loop 1-15 buscando `//span[contains(normalize-space(),'Approve Invoice')]` visible, click, llena textarea Comments (label-for pattern ExtJS), click Submit, espera 2s

#### LiveTrackStepDefinitions.cs — Steps agregados (todos con Given/When/Then)
- `I go to Invoices in LiveTrack`
- `I filter by number {string} in Livetrack`
- `I click on OK button in Livetrack`
- `I refresh the view in the LiveTrack`
- `I approve the {string} invoice in LiveTrack with comment {string}`

### Archivos modificados
- `DFP.Playwright/Pages/Web/LiveTrackPage.cs` — 5 métodos nuevos + 9 nuevas constantes de selectores
- `DFP.Playwright/StepDefinitions/LiveTrackStepDefinitions.cs` — 5 nuevos step bindings

---

## 38. Sesión: 2026-03-27 — TC2244: Implementación de steps Master/House SH, WH Cargo Tab y polling de actualización

### Problema / Consulta
El escenario `2244_AirShipment_UpdateMasterHouse` requería múltiples steps nuevos para verificar la relación Master SH → House SH → Warehouse Receipt en el portal, incluyendo navegación, almacenamiento de variables, verificaciones de campos del booking y un polling de actualización desde Magaya.

### Solución

#### Steps TC2244 — Master / House SH (ShipmentPage + ShipmentStepDefinitions)
- `I should see the Master SH icon in the search results` — verifica badge `span.badge` con `svg[data-icon='folder-tree']` y texto "Master"
- `I go to House tab` — click en nav link `//a[contains(@href,'?view=house-bsl')]`
- `I should see the House SH linked to the Master SH contains {string}` — verifica `li.list-group-item` con HAWB
- `I store the master total pieces` / `I store house total pieces` — extrae int de `//div[contains(normalize-space(text()), 'piece(s)')]`, guarda en `_tc.Data["masterPieces"]` / `["housePieces"]`
- `I store the houseId` — lee `//span[contains(text(), 'HAWB')]`, guarda en `_tc.Data["houseId"]`
- `I click on the houseId in the shipment details page` — click en span con el valor guardado
- `I should see the shipment details page` — verifica Summary tab nav (rss icon) enabled
- `I should see the house total pieces` — verifica visibilidad del panel pieces
- `I should see the house was created in DFP` — log "House was created in DFP correctly!"
- `I verify the house total pieces is Expected HousePIeces+1= Master Shipment total pieces` — assert `masterPieces == housePieces + 1`
- `I click on {string} link in the House SH details page` — click link `//a[contains(@href,'/{linkType}/')]`, extrae GUID y guarda en `_tc.Data["WH"]`
- `I should see the {string} details page` — verifica `h3` con título
- `I should see the correct {string} GUID in the URL` — verifica URL contiene `_tc.Data[guidKey]`

#### Steps WH Cargo Tab (WarehouseReceiptPage + WarehouseReceiptStepDefinitions)
- `I go to cargo tab` — click en `//a[contains(@href,'?view=cargo-items')]` con `svg[data-icon='list']`
- `I should see the cargo items page` — verifica `h5.font-weight-normal "Cargo Items"` enabled
- `I click on the {string} link in the cargo item details` — click `//a[contains(@href,'/{linkType}/')]`

#### Steps de verificación en shipment después de navegar desde WH (ShipmentPage + ShipmentStepDefinitions)
- `I store the shipmentId` — extrae primer `span.text-muted` de `h3`, guarda en `_tc.Data["shipmentNumber"]`
- `I should see shipmentId` — verifica el valor guardado en el h3
- `I should see the oringin {string}` — reutiliza `VerifyOriginAsync` (con typo tal como está en feature)
- `I should see the shipper {string}` — reutiliza `VerifyShipmentDetailFieldAsync("shipper", ...)`
- `I should see the destination {string}` — usa último `h6.font-weight-normal.text-primary.text-truncate`
- `I should see the description contains {string}` — `//div[contains(normalize-space(text()), ...)]`
- `I should see panel {string} in the booking details` — ídem
- `I should see the GUID {string} in the booking details` — ídem
- `I should see the shipmentRef contains {string} in the booking details` — ídem
- `I should see the link entities contains {string} in the booking details` — ídem

#### Polling de actualización desde Magaya
- `the shipment should appear in the search results with text {string}` — poll cada 3 segundos hasta 5 minutos, haciendo click en Reload (`arrows-rotate`), hasta que un card/row contenga el texto del parámetro (e.g. "UPDATED"). Variante del step existente que usa 2 s / 3 min.

### Archivos modificados
- `Pages/Web/ShipmentPage.cs` — métodos para Master/House SH, heading, destination, VerifyDivContainsTextAsync, polling con texto
- `StepDefinitions/ShipmentStepDefinitions.cs` — todos los steps anteriores + `the shipment should appear in the search results with text {string}`
- `Pages/Web/WarehouseReceiptPage.cs` — ClickCargoTabAsync, VerifyCargoItemsHeadingAsync, ClickLinkInCargoDetailsAsync
- `StepDefinitions/WarehouseReceiptStepDefinitions.cs` — steps de cargo tab

---

## 32. Sesión: 2026-03-25 — Fix TC5305: UserNavigatedToShipmentsList navega a URL incorrecta en contexto @INT

### Problema / Consulta
TC5305 (`@5305 @API @INT @login`) hace login con `"with Int"` (usa `PORTAL_INT_BASE_URL`) pero el step `Given I navigated to Shipments List` siempre navegaba a `PORTAL_BASE_URL` (portal no-INT), causando un fallo de navegación al portal equivocado.

### Causa raíz
`UserNavigatedToShipmentsList()` en `ShipmentPage.cs` llamaba hardcodeado a `GetPortalBaseUrl()` que solo lee `PORTAL_BASE_URL`, ignorando si el test estaba en contexto INT.

### Solución
Se implementó la **Opción A**: leer la URL actual del browser al momento de ejecutar el step y compararla con `PORTAL_INT_BASE_URL`. Si coincide (contexto INT activo), navega al portal INT; de lo contrario, usa `PORTAL_BASE_URL`.

```csharp
var portalBaseUrl = GetPortalBaseUrl();
var intBaseUrl = Environment.GetEnvironmentVariable(Constants.PORTAL_INT_BASE_URL) ?? "";

string baseUrl;
if (!string.IsNullOrWhiteSpace(intBaseUrl)
    && Page.Url.StartsWith(intBaseUrl.TrimEnd('/'), StringComparison.OrdinalIgnoreCase))
    baseUrl = intBaseUrl;
else
    baseUrl = portalBaseUrl;
```

### Análisis de impacto (sin regresiones)
Se revisaron los 15 usos del step en `Shipments.feature`. Solo los TCs con tag `@INT` (@3986 y @5305) se benefician del fix. Los 13 restantes (`@NOINT`) no se ven afectados porque su URL actual nunca coincide con `PORTAL_INT_BASE_URL`.

| Caso especial revisado | Resultado |
|---|---|
| @9634 line 208: login previo fue al Hub | Hub URL ≠ INT portal URL → cae a `portalBaseUrl` ✅ |
| @3986: login con "with Int" | URL actual = INT portal → usa `PORTAL_INT_BASE_URL` ✅ |
| @5305: login con "with Int" | URL actual = INT portal → usa `PORTAL_INT_BASE_URL` ✅ |

### Archivos modificados
- `Pages/Web/ShipmentPage.cs` — método `UserNavigatedToShipmentsList()` (línea ~699)

---

## 4. Sesión: 2026-03-25 — Referencia: Operaciones SOAP API disponibles en el proyecto

### Problema / Consulta
Entender cómo usar el proyecto SOAP API para realizar operaciones de setup de datos via API en tests con tag `@API`, específicamente:
- Crear un Contact (sin email) o Customer y configurar LiveTrack Access
- Convertir un Booking en Shipment via API
- Crear un Shipment via API

### Solución — Mapa de operaciones SOAP API

**Patrón base siempre igual:**
```csharp
var session = new ApiSession(username, password);
await session.StartSessionAsync();
SoapClientConfigurator.Configure(session.CSSoap);
// ... operaciones ...
await session.EndSessionAsync();
```

#### 1. Crear Shipment via API (método ya funcional)
Usar el step existente en `DashboardStepDefinitions.cs`:
```gherkin
Given the transaction "SH" "TC_NUMERO" is imported via API
```
Requiere XML en `Resources/TransactionsXml/Shipments/SH_TC_NUMERO.xml`. Estructura de referencia: `SH_TC5305.xml`.

#### 2. Convertir Booking → Shipment (`SubmitShipmentAsync`)
```csharp
var res = await session.CSSoap.SubmitShipmentAsync(new SubmitShipmentRequest
{
    access_key = session.Key,
    shipment_xml = bookingXml,  // XML del booking
    flags = 0
});
// res.shipment_number → número del shipment generado
```

#### 3. Crear Contact/Customer (`SetEntityAsync`)
```csharp
// Contact sin email: Type="CO", elemento <Contact>, sin nodo <Email>
// Customer: Type="CL", elemento <Client>
var req = new SetEntityRequest
{
    access_key = session.Key,
    flags = 0,
    entity_xml = contactXml  // XML de la entidad
};
await session.CSSoap.SetEntityAsync(req);
```

#### 4. Configurar LiveTrack Access (`SetTrackingUserAsync`)
```csharp
await session.CSSoap.SetTrackingUserAsync(
    access_key: session.Key,
    contact_uuid: "GUID-DEL-CONTACT-O-CLIENT",
    login: "username_livetrack",
    password: "password_livetrack",
    user_config_xml: "",   // vacío para config por defecto
    xml_flags: 0
);
```

#### 5. Buscar GUID de entidad existente (`GetEntitiesOfTypeAsync`)
```csharp
var res = await session.CSSoap.GetEntitiesOfTypeAsync(new GetEntitiesOfTypeRequest
{
    access_key = session.Key,
    flags = 0,
    start_with = "nombre",
    entity_type = 1   // 1=Client/Customer, 2=Contact (verificar)
});
// res.entity_list_xml → XML con GUIDs
```

### Tabla resumen de métodos SOAP

| Operación | Método SOAP |
|---|---|
| Crear Shipment | `SetTransactionAsync` (type="SH") |
| Crear WH Receipt | `SetTransactionAsync` (type="WH") |
| Crear Contact/Customer | `SetEntityAsync` |
| Configurar LiveTrack | `SetTrackingUserAsync` |
| Convertir Booking → Shipment | `SubmitShipmentAsync` |
| Buscar entidades por tipo | `GetEntitiesOfTypeAsync` |
| Eliminar transacción | `DeleteTransactionAsync` |

### Nota
`SetEntityAsync` + `SetTrackingUserAsync` **no tienen step Gherkin reutilizable aún** — habría que crear un nuevo step en StepDefinitions o un helper similar a `TransactionImportHelper` si se necesita para un TC.

### Archivos clave del proyecto SOAP
- `SoapApi/ServiceReference/CssSoapClient.cs` — cliente generado con todos los métodos
- `SoapApi/Models/ApiSession.cs` — gestión de sesión (StartSession/EndSession/Key)
- `SoapApi/Common/SoapClientConfigurator.cs` — configuración de timeouts y tamaños
- `DFP.Playwright/Helpers/TransactionImportHelper.cs` — helper para importar transacciones desde XML
- `DFP.Playwright/StepDefinitions/DashboardStepDefinitions.cs` — step `the transaction "X" "Y" is imported via API`

---

## 3. Sesión: 2026-03-24 (continuación)

### Problema
El step `And the data table column "Transport Mode" should have value "AIR"` en `HubRadar.feature` no tenía binding en `HubRadarStepDefinitions.cs` tras hacer el método genérico `VerifyDataTableColumnAsync(string columnName, string expectedValue)` en la sesión anterior.

### Solución
Se agregó el binding faltante en `StepDefinitions/HubRadarStepDefinitions.cs`:
```csharp
[Then("the data table column {string} should have value {string}")]
public async Task ThenTheDataTableColumnShouldHaveValue(string columnName, string expectedValue)
    => await _radarPage.VerifyDataTableColumnAsync(columnName, expectedValue);
```

### Archivos modificados
- `StepDefinitions/HubRadarStepDefinitions.cs` — binding agregado para el step genérico de verificación de columna en tabla.

---

## 2. Sesión: 2026-03-24

### Problema
Implementar el TC281 `@HubRadar` completo. El feature file existía pero tenía todos los sub-tests comentados y los archivos de Page y StepDefinitions estaban vacíos. Era necesario:
- Navegar al portal con MCP Playwright para descubrir selectores reales.
- Implementar todos los sub-tests: View Sales Radar, secciones (Performance over Time, Rankings, Data Table), filtros por período, Transport Mode (AIR), Load Type (LCL), Account Manager (Aylin Rodriguez), Refresh, Full Screen, Selected Metric, Export CSV y Export Excel.

### Solución
Se usó MCP Playwright para navegar a `/radar` y `/radar/sales` y capturar la estructura real de la página. Flujo de cada filtro del panel Filters: abrir Filters → seleccionar opción → Apply → verificar → reset (open Filters → Clear → Apply).

### Archivos modificados/creados
- `Features/HubRadar.feature` — Escenario con todos los steps para los 10 sub-tests.
- `Pages/Web/HubRadarPage.cs` — Implementación completa: navegación, secciones, filtros de período, panel Filters, Refresh, Full Screen, Selected Metric, Export CSV/Excel.
- `StepDefinitions/HubRadarStepDefinitions.cs` — Bindings para todos los steps.
- `Support/DependencyInjection.cs` — Registro de `HubRadarPage` como scoped.

### Selectores clave descubiertos via MCP
- Radar nav: `a[href='/radar']` | View Sales Radar: `a.btn[href='/radar/sales']`
- Secciones: `//a[contains(normalize-space(.), 'Performance over Time|Rankings|Data Table')]`
- Charts: `qwyk-total-quotations-graph-widget`, `qwyk-quotations-grouped-by-top-graph-widget`
- Filters panel: dialog con `ng-select` para Product / Load type / Account Manager + Apply / Clear
- Full screen: `//div[@role='group']/following-sibling::button[1]`

---

## 1. Sesión: 2026-03-05

### Problema
Crear un nuevo test case TC3907 para validar que un Warehouse Receipt con "Exclude from Tracking = True" en Magaya no aparece en el Portal DFP, siguiendo el patrón de TC3986 (@INT @login).

### Solución
Se crearon los siguientes archivos siguiendo todas las convenciones del proyecto (BasePage, múltiples selectores de fallback, DI, Reqnroll):

1. **Feature**: `Features/Warehouse.feature` — Escenario `@3907 @INT @login` con 3 secciones de verificación (WR List, Cargo Detail, Reports).
2. **Page**: `Pages/Web/WarehouseReceiptPage.cs` — Navegación a `/my-portal/warehouse-receipts` y `/my-portal/cargo`, búsqueda por nombre de WR, aserciones negativas.
3. **Steps**: `StepDefinitions/WarehouseReceiptStepDefinitions.cs` — Steps para navegación, set name, enter search, y aserciones "not appear".
4. **DI**: `Support/DependencyInjection.cs` — Registro de `WarehouseReceiptPage` como scoped.
5. **ReportsPage.cs** — Nuevos selectores y método `IClickOnWarehouseReceiptsOption()` + `TheNameShouldNotAppearInResults()`.
6. **ReportsStepDefinitions.cs** — Nuevos steps: `I click on "Warehouse Receipts" option` y `the warehouse receipt name should not appear in the report results`.

### Archivos modificados/creados
- `Features/Warehouse.feature` (nuevo)
- `Pages/Web/WarehouseReceiptPage.cs` (nuevo)
- `StepDefinitions/WarehouseReceiptStepDefinitions.cs` (nuevo)
- `Support/DependencyInjection.cs` (modificado)
- `Pages/Web/ReportsPage.cs` (modificado)
- `StepDefinitions/ReportsStepDefinitions.cs` (modificado)

---

## 2. Sesión: 2026-03-05 (continuación)

### Problema
Las dos aserciones de TC3907 pasaban incorrectamente aunque el WR TC3907 SÍ aparecía en el portal. Root cause:

1. **WR List** (`TheWarehouseReceiptShouldNotAppearInResultsAsync`): El nombre del WR se trunca visualmente a "TC39…" en la UI, por lo que los selectores `//a[normalize-space()='TC3907']` nunca encontraban el elemento → la aserción `IsNull` siempre pasaba aunque el WR estuviera visible.
2. **Cargo Detail** (`TheWarehouseReceiptShouldNotBeDisplayedInCargoDetailAsync`): La tabla usa lazy loading — la columna "Warehouse Receipt" muestra un botón "Load" en lugar del número del WR. Los selectores `//a[...]` y `//*[contains(...,'TC3907')]` nunca encontraban el texto → la aserción `IsNull` siempre pasaba aunque el WR estuviera en los resultados.

### Solución
Se navegó con MCP Playwright al portal STG y se verificó el estado "sin resultados" en ambas páginas:

**WR List (empty state):**
```
listitem > heading "No warehouse receipts found" [h5]
listitem > paragraph "We couldn't find any matching warehouse receipt..."
count: "0 warehouse receipts found"
```

**Cargo Detail (empty state):**
```
rowgroup > row "Nothing found Clear filters"
  cell > generic "Nothing found" + button "Clear filters"
```

### Cambios aplicados
`Pages/Web/WarehouseReceiptPage.cs`:
- `TheWarehouseReceiptShouldNotAppearInResultsAsync`: ahora busca `heading "No warehouse receipts found"` (aserción `IsNotNull`).
- `TheWarehouseReceiptShouldNotBeDisplayedInCargoDetailAsync`: ahora busca `"Nothing found"` en la tabla (aserción `IsNotNull`).

Ambas aserciones ahora **fallan correctamente** si el WR aparece en los resultados.

### Archivos modificados
- `Pages/Web/WarehouseReceiptPage.cs` (modificado)

---

## 1. Sesión: 2026-03-05

### Refinamiento (misma sesión) — Selectores verificados con MCP Playwright en vivo

Se navegó al portal STG (`https://38442-dfpstag-magayaprod-auto.next.qwykportals.com/`) con credenciales INT y se verificaron todos los selectores y URLs reales:

| Elemento | Valor verificado |
|---|---|
| WR List URL | `/my-portal/warehouse-receipts` → redirige a `/list` |
| Campo búsqueda WR | `textbox "Warehouse Receipt #"` |
| Cargo Detail URL | `/my-portal/cargo-detail` (submenú de Warehouse) |
| Reports WR URL | `/my-portal/reports/warehouse-receipts` |
| Heading Report WR | `"Generate Warehouse Receipts Report"` |
| Report WR date selector | `combobox` (NO usa calendar — default "Last 7 days") |
| WR list items | Render como `list > listitem > link` (NO tabla) |

Cambios aplicados post-verificación:
- `WarehouseReceiptPage.cs`: campo de búsqueda corregido, URL `/my-portal/cargo-detail` confirmada, aserciones usan XPath de links.
- `Warehouse.feature`: eliminados pasos de calendario innecesarios en Reports section.
- `ReportsPage.cs`: eliminado TODO, selector confirmado.

---

## 3. Sesión: 2026-03-05 — TC5439 Table View

### Problema
Implementar el escenario TC5439 "User edits a Table View and verifies selected columns" ya esbozado en `Warehouse.feature`. Faltaba implementar todos los steps: hacer clic en Table View, crear una vista custom, abrir el panel Customize (gear), cambiar el nombre, agregar columna "Shipper" en la pestaña Columns, guardar y verificar que la columna aparece en la tabla.

### Root cause (TC3907 assertions)
Las aserciones de TC3907 pasaban incorrectamente:
- **WR List**: el nombre "TC3907" se trunca a "TC39…" en la UI → los selectores `//a[normalize-space()='TC3907']` nunca matcheaban → `IsNull` siempre pasaba.
- **Cargo Detail**: la columna Warehouse Receipt usa lazy load (botón "Load") → el texto del WR nunca aparece → `IsNull` siempre pasaba.

Fix: ambas aserciones ahora buscan el empty state del componente (`"No warehouse receipts found"` y `"Nothing found"`) con `IsNotNull`.

### Solución TC5439
Verificado en vivo via MCP Playwright en el portal STG:
- **Table View button**: `svg[data-icon='table']` dentro de `button.btn-primary`
- **Create button**: `button.btn-primary:has-text('Create')` (distinto de "Create Report" que es `btn-outline-primary`)
- **Dialog**: textbox `"Custom View Name"`, botón `"Save"`
- **Gear/Customize**: `button.btn-primary:has(svg[data-icon='gear'])` — solo habilitado con vista custom seleccionada
- **Columns tab**: `role=tab name="Columns"`, columnas como `//tr[.//td[normalize-space()='Shipper']]` con `role=checkbox`
- **Panel close**: `//aside[@role='complementary']//button[.//img][1]` — edits se auto-aplican

### Archivos modificados
- `Pages/Web/WarehouseReceiptPage.cs` — 5 nuevos grupos de selectores, 6 nuevos métodos (`SelectViewToEditAsync`, `OpenViewConfigurationAsync`, `ChangeViewNameAsync`, `AddColumnInColumnsTabAsync`, `SaveViewConfigurationAsync`, `TableViewShouldDisplayColumnAsync`)
- `StepDefinitions/WarehouseReceiptStepDefinitions.cs` — 6 nuevos bindings para TC5439
- `Features/Warehouse.feature` — ya tenía el escenario TC5439 esbozado por el usuario; no se modificó

---

## 3. Sesión: 2026-03-05 (continuación)

### Problema
Implementar los steps del escenario `@5439` con los selectores reales del portal y limpiar código muerto.

### Cambios de implementación
Steps re-escritos con selectores verificados desde el HTML real del portal:
- `I select a view to edit` — click en `span[role='combobox'][aria-label='Default View']`, luego `span[text()='AutomationCustomize']`
- `I click on Configuration button` — click en `button.btn-primary:has(svg[data-icon='gear'])`
- `I click on Columns tab` — click en tab `role=tab name="Columns"`
- `I enter the column Name in the field` — tipea "commodities" en `input[placeholder='Search']`, assert que aparece `td` con "Commodities Description"
- `I select the column Name` — verifica si `div.p-checkbox-box` tiene clase `p-highlight`; si no, hace click
- `I close the Customize View` — click en `button:has(svg.p-sidebar-close-icon)`
- `I should see the selected columns in the Table View` — assert que `th[role='columnheader']` conteniendo "Commodities Description" es visible

### Limpieza de código muerto
- **`WarehouseReceiptPage.cs`**: Eliminados `SaveViewDialogButtonSelectors` (nunca usado), y 5 métodos huérfanos: `OpenViewConfigurationAsync`, `ChangeViewNameAsync`, `AddColumnInColumnsTabAsync`, `SaveViewConfigurationAsync`, `TableViewShouldDisplayColumnAsync`. Corregidos comentarios erróneos.
- **`WarehouseReceiptStepDefinitions.cs`**: Eliminado binding `[Then("the Table View should display the selected columns")]` que ya no tiene feature asociado.
- **`Warehouse.feature.cs`**: Actualizado el runner auto-generado con los nuevos step texts del escenario `@5439`.

### Archivos modificados
- `Pages/Web/WarehouseReceiptPage.cs`
- `StepDefinitions/WarehouseReceiptStepDefinitions.cs`
- `Features/Warehouse.feature.cs`

---

## 4. Sesión: 2026-03-05 (continuación)

### Problema
Implementar steps del escenario TC6526 `@NOINT` de Quotations con selectors reales y steps parametrizables.

### Cambios
- `I click on Ocean Button` → `I click on {string} transport mode` (Air | Ocean | Truck). XPath: `//label[.//input[@name='transport_mode'] and .//span[normalize-space()='{0}']]`
- `I click on Full FCL button` → `I click on {string} load type` (Full (FCL) | Partial (LCL)). XPath: `//label[.//input[@name='load_type'] and contains(...)]`
- `I enter the Origin/Destination Port` → `I enter {string} as the Origin/Destination Port`. Flow: click `input[placeholder='Click to search...']`, type in `input[placeholder='Type to search...']`, click first `li.p-autocomplete-item`, click `button[type='submit'].btn-primary`
- `I select the currency` → `I select {string} as the currency`. Currency: `ng-select[formcontrolname='currency']` + `div.ng-option` by text
- Date: `span.p-datepicker-current-day` (today always pre-highlighted)
- `And I select the container` = placeholder pendiente

### Archivos creados/modificados
- `Pages/Web/QuotationPage.cs` — nuevo
- `StepDefinitions/QuotationStepDefinitions.cs` — nuevo
- `Support/DependencyInjection.cs` — registro de QuotationPage
- `Features/Quotations.feature` — steps actualizados con parámetros

---

## 5. Sesión: 2026-03-06 — TC6526 vessel steps, TC10255 Hub milestones + Portal history

### Problemas resueltos

1. **TC6526 — Container size strict mode violation**: `//span[normalize-space()='Container Size']` matcheaba 2 elementos. Fix: `span[role='combobox'][aria-label='Container Size']`.
2. **TC6526 — Commodity step split**: Se separó el click en dropdown y la selección en dos steps (`When I click on Commodity dropdown` / `Then I select the Commodity "..."`).
3. **TC6526 — ShouldSeeOffersAsync timeout**: Reemplazado spinner-wait con `WaitForFunctionAsync` polling cada 2s hasta que aparece tab `span` con texto "Schedules" (timeout 5 min).
4. **TC6526 — Filter "Clear all" strict mode**: `a:has-text('Clear all')` matcheaba 4 elementos. Fix: `.Nth(2)` (tercero = sección carrier).
5. **TC6526 — CompareVessel dirección incorrecta**: Fixed de `_bookingVessel.Contains(_firstVessel)` a `_firstVessel.Contains(_bookingVessel)`.
6. **TC6526 — Vessel steps implementados**: `StoreFirstVesselAsync`, `SelectFirstScheduleAsync`, `StoreVesselForBookingAsync`, `CompareVesselWithScheduleAsync`.

### TC10255 — Milestone date history (Hub + Portal)

Nuevo scenario en `Features/Shipments.feature`:
- En el **Hub**: login → search shipment → select → Milestones tab → edit "Container empty to shipper" milestone × 2 (calendar, save) → verify date.
- En el **Portal**: navigate to shipments list → search → select first shipment → verify history badge → click badge → verify popup → verify historical changes.

#### Hub steps implementados (`ShipmentHubPage.cs`):
- `SelectCreatedShipmentAsync`: `td div.d-flex a[href*='/shipments/']` (scoped a td para evitar nav links)
- `GoToMilestonesTabAsync`: `a.nav-link[href*='?view=milestones']` (evita match con "Shipment Milestones" del admin nav)
- `ClickEditButtonForMilestoneAsync`: XPath scoped a `li` con nombre del milestone → button sin `text-success`
- `ClickCalendarButtonInMilestoneAsync`: `form button[type='button'].btn-outline-primary`
- `SelectDateInCalendarAsync`: Hub usa PrimeNG antiguo → `div.p-datepicker` (no `p-datepicker` tag)
- `ClickSaveChangesButtonAsync`, `ShouldSeeDateInMilestoneTabAsync`, `GetSelectedDate`

#### Portal steps implementados (`ShipmentPage.cs`):
- `ShouldSeeHistoryBadgeNextToMilestoneAsync`, `ClickHistoryBadgeForMilestoneAsync`
- `ShouldSeePopupWithCurrentDateAsync`, `ShouldSeeHistoricalChangesAsync`

#### Fix final — `UserSelectsFirstShipmentFromList` no navegaba:
- **Problema**: El método solo leía el nombre pero no hacía click. Los selectores usaban `//a` pero la card no tiene `<a>` — es `div.card[tabindex='0']`.
- **Fix**: Nuevos selectores `FirstShipmentCardSelectors` (target `div.card` en `qwyk-shipments-list-item`) y `FirstShipmentNameSelectors` (target `div.h4 > span`). El método ahora lee el nombre y luego hace click con `ClickAndWaitForNetworkAsync`.

### Archivos modificados
- `Pages/Web/QuotationPage.cs`
- `StepDefinitions/QuotationStepDefinitions.cs`
- `Features/Quotations.feature`
- `Pages/Web/ShipmentHubPage.cs`
- `StepDefinitions/ShipmentHubStepDefinitions.cs`
- `Pages/Web/ShipmentPage.cs`
- `StepDefinitions/ShipmentStepDefinitions.cs`
- `Features/Shipments.feature`

---

## 6. Sesión: 2026-03-06 — Next date calendar, hub URL navigation, historical changes

### Problemas resueltos

1. **`When I select the first shipment from the list` no navegaba**: El método solo guardaba el nombre pero no hacía click. Fix: nuevos selectores `FirstShipmentCardSelectors` (`div.card` en `qwyk-shipments-list-item`) y `FirstShipmentNameSelectors` (`div.h4 > span`). Ahora hace click con `ClickAndWaitForNetworkAsync`.

2. **Steps de navegación sin login**:
   - `When I open the portal URL "without int"` → navega a `PORTAL_BASE_URL` (fallback: magaya-qa URL)
   - `When I open the portal URL "with int"` → navega a `PORTAL_INT_BASE_URL` (fallback: 38442 URL)
   - `When I open the hub URL` → navega a `HUB_BASE_URL` (fallback: hub.next.qwykportals.com)
   - Implementados en `LoginPortalHubStepDefinitions.cs` con regex `@"I open the portal URL ""?([^""]+)""?"` y `IsIntegration()` helper.

3. **`Then I should select the next {string} in the calendar`**: Selecciona `dateParam + 1` día en el calendario PrimeNG del Hub. Si el día cruza mes, hace click en `p-datepicker-next` antes de seleccionar el día.

4. **`Then I should see the next {string} in the Milestone tab`**: Verifica que el día siguiente a `dateParam` (o `_selectedDate + 1` si param es "date") sea visible en el tab Milestones.

5. **`And I should see the historical changes`** modificado: Ahora verifica TANTO la fecha seleccionada (`date`) COMO la siguiente (`next date`) en los entries `span.history-date` del popup de historial.

### Archivos modificados
- `Pages/Web/ShipmentPage.cs` — `ShouldSeeHistoricalChangesAsync` verifica ambas fechas
- `Pages/Web/ShipmentHubPage.cs` — `SelectNextDateInCalendarAsync`, `ShouldSeeNextDateInMilestoneTabAsync`, `ClickCalendarDayAsync` (helper extraído)
- `StepDefinitions/ShipmentHubStepDefinitions.cs` — 2 nuevos bindings: `IShouldSelectTheNextDateInTheCalendar`, `IShouldSeeTheNextDateInTheMilestoneTab`
- `StepDefinitions/LoginPortalHubStepDefinitions.cs` — 2 nuevos steps: `WhenIOpenThePortalURL`, `WhenIOpenTheHubURL`

---

## 7. Sesión: 2026-03-09 — History popup fix, next week logic, attachments, quote ID, PO page, build fixes

### Problemas resueltos

1. **`ShouldSeeHistoricalChangesAsync` no encontraba `span.history-date`**: Angular renderiza nodos `<!---->` que rompen XPath `normalize-space()`. Fix: reemplazado XPath con `Filter(HasText)`. También eliminado `WaitForLoadStateAsync(NetworkIdle)` que cerraba el popup.

2. **Popup "Dates Update History" desaparecía tras click en badge**: `ScrollIntoViewIfNeededAsync` y `WaitForEnabledAsync` disparaban el handler `click-outside` de Angular. Fix: eliminados ambos calls; ahora espera el heading del popup tras hacer click.

3. **Fecha incorrecta (+2 en vez de +1)**: `ClickCalendarDayAsync` sobreescribía `_selectedDate`, causando que la comparación usara base+2. Fix: `_selectedDate` solo se asigna en `SelectDateInCalendarAsync`; `ClickCalendarDayAsync` ya no lo modifica.

4. **"next date" cambiado a "next week" (+7 días)** en todos los lugares: `SelectNextDateInCalendarAsync`, `ShouldSeeNextDateInMilestoneTabAsync`, `ShouldSeeHistoricalChangesAsync`. Bindings actualizados en `ShipmentHubStepDefinitions.cs` y `ShipmentStepDefinitions.cs`.

5. **Attachments steps implementados** (`ShipmentPage.cs`): `ClickAttachmentsTabAsync`, `ClickAttachDocumentButtonAsync`, `ShouldSeeUploadScreenAsync`, `SelectFileToUploadAsync(fileName)` (resuelve path desde raíz del proyecto via `Assembly.Location`), `ClickUploadButtonAsync`, `ShouldSeeUploadedFileAsync(fileName)`.

6. **Quote ID steps** (`QuotationPage.cs`): `StoreQuoteIdAsync` extrae `QUO-\d+` de `span.font-weight-bold` via regex. `EnterQuotationIdInSearchAsync` llena `input[formcontrolname='friendly_id']`. `ShouldSeeQuoteIdInResultsAsync` verifica `li[qwyk-quotations-list-item]` con badge "Created".

7. **`PurchaseOrderPage.cs` creado** con métodos completos: navegación a `/my-portal/orders/list`, crear PO, número auto-generado (`POAuto{ddHHmmss}`), buyer, currency (ng-select), supplier, transport mode, cargo origin/destination (p-autocomplete), save, details, search y results.

8. **`PurchaseOrderStepDefinitions.cs` reescrito** de Gherkin a C# con todos los bindings correspondientes.

9. **3 errores de build en `Shipments.feature`** corregidos: `Log out from Portal` → `And I log out from Portal`; `""Create New Purchase Order""` → `"Create New Purchase Order"`; `I click on search button` → `I click on PO search button`.

10. **Logout avatar button no encontrado**: Añadidos selectores CSS fallback en `ProfileButtonSelectors`, espera NetworkIdle y scroll to top en `LogoutAsync`. Añadidos bindings `[When/Then("I log out from Portal")]` en `LoginPortalHubStepDefinitions.cs`.

11. **`SelectCurrencyAsync` PO clickeaba combobox incorrecto**: Fix: scope con `//div[normalize-space()='Currency']` como ancla → `currencySection.Locator("input[role='combobox']")`.

### Archivos modificados
- `Pages/Web/ShipmentPage.cs` — history popup fix, attachment methods
- `Pages/Web/ShipmentHubPage.cs` — next week (+7), `_selectedDate` solo en `SelectDateInCalendarAsync`
- `Pages/Web/QuotationPage.cs` — quote ID methods
- `Pages/Web/LoginPage.cs` — fallback selectors para avatar logout
- `Pages/Web/PurchaseOrderPage.cs` — nuevo (creado), fix `SelectCurrencyAsync`
- `StepDefinitions/PurchaseOrderStepDefinitions.cs` — reescrito (Gherkin → C#)
- `StepDefinitions/ShipmentStepDefinitions.cs` — attachment bindings, "next week" rename
- `StepDefinitions/ShipmentHubStepDefinitions.cs` — "next week" rename
- `StepDefinitions/QuotationStepDefinitions.cs` — quote ID bindings
- `StepDefinitions/LoginPortalHubStepDefinitions.cs` — logout bindings
- `Support/DependencyInjection.cs` — registro de PurchaseOrderPage
- `Features/Shipments.feature` — 3 correcciones de sintaxis Gherkin

---

## Session 8 — 2026-03-10

### Changes Made

**Problem 1:** Portal login (`Given I login to Portal as user "aylinportalinfra@yopmail.com"`) was too fast — the SPA hadn't fully loaded by the time `EnsureLoginFormAsync` searched for the Sign-in button, causing intermittent failures clicking it.

**Solution 1:** Added `await Page.WaitForLoadStateAsync(LoadState.NetworkIdle)` after the existing `WaitForLoadStateAsync(DOMContentLoaded)` in `NavigateAsync()` in `LoginPage.cs`. This ensures the SPA is fully rendered before any login form interaction.

**Files changed:**
- `Pages/Web/LoginPage.cs` — `NavigateAsync()`: added `NetworkIdle` wait after `DOMContentLoaded`

---

**Problem 2:** `ClickCreateRuleButtonAsync` in `MailingRulesHubPage.cs` used `WaitForEnabledAsync(btn, 8000)` to wait for the "Create rule" button, but after a rule deletion + NetworkIdle, the button was still briefly disabled/not-ready, causing intermittent clicks on a non-responsive button.

**Solution 2:** Replaced `WaitForEnabledAsync(btn, 8000)` with a two-step wait: first wait for `input[formcontrolname='name'][placeholder='Search by Name']` to be visible and enabled (used as a page-ready indicator that confirms the list has fully reloaded), then wait for the "Create rule" button itself.

**Files changed:**
- `Pages/Web/MailingRulesHubPage.cs` — `ClickCreateRuleButtonAsync()`: added search input as page-ready indicator before clicking the button

---

## Session 9 — 2026-03-11

### Changes Made

**Problem 1:** `ClickCreateMailingRuleButtonAsync` was timing out (15000ms) trying to find `button.btn-primary` with "Create mailing rule" text. The class selector was too restrictive.

**Solution 1:** Replaced class-based selector with `Page.GetByRole(AriaRole.Button, Name="Create mailing rule")` plus an XPath `translate()` fallback for case-insensitive matching.

---

**Problem 2:** `CheckIfRuleExistsToDeleteItAsync` was too fast after clicking "Yes" on the delete dialog — automation continued while the page still showed the rule detail view.

**Solution 2:** Added 10s hard wait + `NetworkIdle` + search input visibility/enabled check as page-ready indicator after the "Yes" click.

---

**Problem 3:** `And I enable the option Receive Shipment Status Notification` step was undefined because `{string}` only matches quoted parameters.

**Solution 3:** Changed binding to regex `[When(@"I enable the option ""?(.+?)""?$")]` to capture both quoted and unquoted parameters.

---

**Problem 4:** `Frame was detached` error after logout — logout click triggers navigation and the next step accesses a detached frame.

**Solution 4:** Added `DOMContentLoaded` + `NetworkIdle` waits with try-catch in `LogoutAsync()` after the logout button click.

---

**Problem 5:** `Then I should be in login page` failed — after logout the portal returns to its homepage with a nav "Sign in" link, not the login form heading.

**Solution 5:** Wrapped `WaitForLoginAsync` in try-catch (5s), added fallback check for `a:has-text('Sign in')` nav link in `LoginPortalHubStepDefinitions.cs`.

---

**Problem 6:** Portal login stayed too long on homepage — `WaitForLoadStateAsync(NetworkIdle)` added in session 8 blocked on background SPA requests that never settled.

**Solution 6:** Changed `NavigateAsync()` to wrap `NetworkIdle` in a try-catch with 5000ms timeout so it continues when DOM is ready even if network is still active.

---

**New Steps Added:** Subscribe/Unsubscribe, notification panel, milestone confirmation, calendar date selection, portal notifications — full implementation in `ShipmentPage.cs` and `ShipmentStepDefinitions.cs`.

### Files Modified
- `Pages/Web/LoginPage.cs` — `NavigateAsync()`: NetworkIdle with 5s timeout + try-catch; `LogoutAsync()`: DOMContentLoaded + NetworkIdle waits with try-catch
- `Pages/Web/MailingRulesHubPage.cs` — `ClickCreateMailingRuleButtonAsync()`: GetByRole + XPath fallback; `ClickCreateRuleButtonAsync()`: search input as page-ready indicator; `CheckIfRuleExistsToDeleteItAsync()`: 10s wait + NetworkIdle + search input enabled
- `Pages/Web/ShipmentPage.cs` — new methods: Subscribe, Unsubscribe, notification panel, milestone confirmation, calendar date selection, green icon, notifications panel
- `StepDefinitions/ShipmentStepDefinitions.cs` — new step bindings for all new ShipmentPage methods
- `StepDefinitions/LoginPortalHubStepDefinitions.cs` — `ThenIShouldBeInLoginPage()`: fallback to nav Sign-in link

---

## 12. Sesión: 2026-03-11

### Problema
`CheckCustomFieldValueAsync` encontraba la columna "StringCustomField" en el índice 47 pero `td:nth-child(47)` en `tbody tr:first-child` devolvía texto vacío. El problema radicaba en que `th` y `td` no son 100% equivalentes en PrimeNG datatables (columnas fijas, columnas virtuales, etc.) — el índice CSS nth-child no coincide con el índice del header.

### Solución
Se reemplazó la lógica de CSS nth-child por una evaluación JavaScript que opera sobre el mismo DOM `table`, garantizando alineación entre `th` y `td`:
- Itera todos los elementos `table` en la página
- Encuentra el índice de columna con `headers.findIndex(th => th.innerText.includes(columnName))`
- Lee `cells[colIndex]` (mismo índice, misma tabla) del primer `tbody tr`
- Si la columna no se encuentra en ninguna tabla, la aserción falla con mensaje claro

### Archivos modificados
- `Pages/Web/WarehouseReceiptPage.cs` — `CheckCustomFieldValueAsync`: reemplazado por `Page.EvaluateAsync<string>` con JS que itera tablas y alinea headers/cells por índice de array DOM.

---

## 13. Sesión: 2026-03-11 (continuación)

### Problema
`CheckCustomFieldValueAsync` fallaba repetidamente:
1. `innerText` vacío en columna 46 → PrimeNG list table renderiza custom field cells como `*ngIf` placeholders vacíos
2. `TextContentAsync` también vacío → los valores se cargan asincrónicamente después del render inicial
3. Navegación a WR detail page → `WaitForSelectorState.Visible` timeout 15s porque el label estaba justo en el borde del viewport
4. `Assert.IsGreaterThanOrEqualTo(colIndex, 0)` fallaba porque MSTest trata el primer param como "expected" y el segundo como "actual", verificando `0 >= colIndex` en lugar de `colIndex >= 0`

### Solución
Se cambió la estrategia completa a un enfoque con DataTable (Gherkin table):
- Nueva firma: `CheckCustomFieldValuesInTableViewAsync(IEnumerable<(string columnName, string expectedValue)>)`
- Busca el índice de columna usando `th.cellIndex` (propiedad DOM nativa, siempre alineada con `row.cells[N]`)
- **Poll con retries cada 500ms hasta 12s**: los custom field cells inician como `<!---->` (Angular `*ngIf=false`) y re-renderizan asincrónicamente cuando llega la API
- Usa `Assert.IsTrue(colIndex >= 0, ...)` en lugar de `IsGreaterThanOrEqualTo` para evitar confusión de orden de parámetros

### Archivos modificados
- `Pages/Web/WarehouseReceiptPage.cs` — `CheckCustomFieldValuesInTableViewAsync` con poll/retry y `th.cellIndex`
- `StepDefinitions/WarehouseReceiptStepDefinitions.cs` — nuevo step con DataTable, conversión a tuples via LINQ
- `Features/Warehouse.feature` — step actualizado a formato DataTable:
  ```gherkin
  And I check the following custom field values in the table view:
    | StringCustomField | ShipperRefUpdated |
  ```

---

## 14. Sesión: 2026-03-17 — Reload button antes de Search

### Problema
El step `And I click on Search button` en Shipments.feature fallaba intermitentemente porque a veces aparece un botón **Reload** (`btn-outline-danger`, icono `fa-arrows-rotate`) que indica datos obsoletos. Si no se hace clic en él primero, la búsqueda puede no funcionar correctamente.

### Solución
Se modificó `IClickOnSearchButton()` en `ShipmentPage.cs` para verificar si el botón Reload está visible antes de hacer clic en Search. Si está visible, lo clickea y espera `NetworkIdle` antes de continuar.

### Archivos modificados
- `Pages/Web/ShipmentPage.cs` — `IClickOnSearchButton`: agrega chequeo condicional del botón `button.btn-outline-danger` con texto "Reload" al inicio del método.

---

## 15. Sesión: 2026-03-18 — Fix definitivo del login portal

### Problema
El login del portal se quedaba en la landing page por mucho tiempo sin hacer clic en "Sign in". Todos los intentos anteriores de localizar y clickear el link "Sign in" del navbar fallaban intermitentemente (timing, cookie consent dialogs que interceptaban eventos, demora del SPA en renderizar).

### Investigación MCP
Se confirmó via browser MCP que navegar a `/my-portal` mientras no hay sesión activa hace lo siguiente automáticamente:
1. Redirige a `/?next=%2Fmy-portal%2Fdashboard`
2. Abre el modal de login automáticamente (sin necesidad de clickear "Sign in")

### Solución
Se reemplazó `EnsureLoginFormAsync` para que navegue directamente a `_baseUrl.TrimEnd('/') + "/my-portal"`. Esto dispara la redirección automática y abre el modal de login. Ya no hace falta esperar ni clickear el link "Sign in" del navbar.

### Archivos modificados
- `Pages/Web/LoginPage.cs` — `EnsureLoginFormAsync`: eliminado el `WaitForTimeoutAsync(2000)` + `GetByRole(Link, "Sign in")` + click. Ahora navega directamente a `/my-portal` y llama a `WaitForLoginFormAsync`.

---

## 16. Sesión: 2026-03-18 — Eliminación de esperas innecesarias en NavigateAsync

### Problema
Después de aplicar el fix de la sesión 15 (navegar a `/my-portal`), el login seguía tardando demasiado. Se analizó el flujo paso a paso y se identificó que `NavigateAsync()` tenía tres esperas acumuladas que sumaban hasta **~10 segundos extra**:
1. `WaitUntil = WaitUntilState.DOMContentLoaded` en las opciones de `GotoAsync`
2. `WaitForLoadStateAsync(LoadState.DOMContentLoaded)` redundante (ya lo hace `GotoAsync` internamente)
3. `WaitForLoadStateAsync(LoadState.NetworkIdle, Timeout=5000)` — el SPA del portal mantiene conexiones en background que nunca se "settle", por lo que siempre esperaba los 5 segundos completos antes de continuar

### Solución
Se comentaron las tres esperas en `NavigateAsync()`:
- `WaitUntil` eliminado de las opciones de `GotoAsync` (usa el default de Playwright)
- `WaitForLoadStateAsync(DOMContentLoaded)` comentado (redundante)
- Todo el bloque `try/catch` de `NetworkIdle` comentado

Esto confirmó ser la causa real de la gran demora. El login ahora es fluido.

### Archivos modificados
- `Pages/Web/LoginPage.cs` — `NavigateAsync()`: comentadas las tres esperas innecesarias

---

## 17. Sesión: 2026-03-18 — Nuevos steps LCL Cargo + Spot Rate Request

### Cambios

**Nuevos steps implementados en `QuotationPage.cs` + `QuotationStepDefinitions.cs`:**

| Step | Selector usado |
|---|---|
| `I select the Package "Carton"` | `//label[contains(@class,'btn') and .//input[@formcontrolname='packaging'] and contains(normalize-space(),'{0}')]` |
| `I enter the following cargo details:` (DataTable) | `input[formcontrolname='unit_weight/length/width/height']` |
| `I click on Request a different rate button` | `button.rounded-pill:has-text('Request a different rate')` |
| `I should see the modal to enter the request` | `//h5[contains(@class,'modal-title') and contains(normalize-space(),'Send a spot rate request')]` |
| `I click on select a request dropdown` | `#reason input[role='combobox'][aria-autocomplete='list']` |
| `I select the option "I need a better rate"` | `//div[contains(@class,'ng-option') and contains(normalize-space(),'{0}')]` |
| `I enter the remarks "AutomationRequest"` | `textarea[formcontrolname='remarks']` |
| `I send the request` | `//button[.//span[normalize-space()='Send']]` |
| `I select the accesorials "Refrigerated"` | `label[for='refrigerated']` (lowercased automático) |

**Patrón profesional para campos múltiples:** en vez de 4 steps separados (Weight, Length, Width, Height), se usa un único step con DataTable:
```gherkin
And I enter the following cargo details:
  | Weight | Length | Width | Height |
  | 10     | 10     | 10    | 10     |
```

### Strict mode violations — reglas para evitarlos

**Problema frecuente:** un selector genérico como `input[role='combobox'][aria-autocomplete='list']` matchea múltiples elementos en la página (en este caso 3 comboboxes).

**Reglas para prevenirlo:**

1. **Acotar siempre por ancestro único**: usar el `id` o clase del contenedor padre inmediato.
   - ❌ `input[role='combobox'][aria-autocomplete='list']` → matchea 3
   - ✅ `#reason input[role='combobox'][aria-autocomplete='list']` → matchea 1

2. **Distinguir readonly vs editable**: si solo uno es editable, agregar `:not([readonly])`.

3. **Usar `.First` cuando hay múltiples y solo importa el primero** (e.g. primer resultado de lista).

4. **Verificar antes de escribir**: ante cualquier selector genérico (`input`, `button`, `span`, `div.ng-option`), preguntarse ¿cuántos de estos puede haber en la página completa en este momento?

5. **Casos ya corregidos en este proyecto:**
   - `span:has-text('gathering...')` → `.First`
   - `a:has-text('Clear all')` → `.Nth(2)`
   - `input[role='combobox']` en modal → `#reason input[role='combobox']`
   - `th[role='columnheader']` → `.First`
   - Textbox "Shipment Reference" → `input#full` + `.First`

### Archivos modificados
- `Pages/Web/QuotationPage.cs` — selectores y métodos nuevos
- `StepDefinitions/QuotationStepDefinitions.cs` — bindings nuevos

---

## 18. Sesión: 2026-03-18 — Estado estable del login (referencia definitiva)

### Contexto
Después de múltiples intentos de optimización del login (sesiones 15-17), se realizaron cambios que rompieron el flujo. Se revirtió todo a la configuración que **confirmó funcionar**.

### Estado que funciona — `LoginPage.cs`

**`NavigateAsync()`** — sin ninguna espera adicional después de `GotoAsync`:
```csharp
await Page.GotoAsync(_baseUrl, new PageGotoOptions
{
    // WaitUntil = WaitUntilState.DOMContentLoaded,  // comentado
    Timeout = 60000
});
// WaitForLoadStateAsync(DOMContentLoaded) — comentado (redundante)
// WaitForLoadStateAsync(NetworkIdle, 5000) — comentado (causa 5s de espera siempre)
```
La clave: el bloque `NetworkIdle` era el culpable de los 5+ segundos extra. Sin él, el login es fluido.

**`EnsureLoginFormAsync()`** — navega a `/my-portal` con `DOMContentLoaded`:
```csharp
var myPortalUrl = _baseUrl.TrimEnd('/') + "/my-portal";
await Page.GotoAsync(myPortalUrl, new PageGotoOptions
{
    WaitUntil = WaitUntilState.DOMContentLoaded,
    Timeout = 30000
});
await WaitForLoginFormAsync(timeoutMs);
```
Navegar a `/my-portal` sin sesión activa redirige automáticamente a `/?next=...` y abre el modal de login. No hace falta clickear "Sign in" del navbar.

**`SignInButtonSelectors`** — sin selectores de links del navbar:
```csharp
"dialog button:has-text('Sign in')",
"internal:role=button[name=\"Sign in\"i]",
"internal:role=button[name=\"Log in\"i]",
"internal:role=button[name=\"Login\"i]",
"internal:role=button[name=\"Continue\"i]",
```

### Qué NO hacer
- No agregar `WaitUntil = WaitUntilState.Commit` en `NavigateAsync` ni `EnsureLoginFormAsync` — causa race conditions
- No agregar selectores de links (`"a:has-text('Sign in')"`, `"text=Sign in"`) en `SignInButtonSelectors` — pueden clickear el navbar en vez del modal
- No restaurar el bloque `NetworkIdle` en `NavigateAsync`

### Archivos con estado estable
- `Pages/Web/LoginPage.cs`

---

## 19. Sesión: 2026-03-18 — Regla: siempre usar GetActivePortalBaseUrl() en vez de GetPortalBaseUrl()

### Problema
`UserNavigatedToShipmentsList` usaba `GetPortalBaseUrl()` que solo lee `PORTAL_BASE_URL`. En tests con portal INT (`PORTAL_INT_BASE_URL`), navegaba al portal equivocado.

### Solución
Se agregó `GetActivePortalBaseUrl()` en `ShipmentPage.cs` que deriva la base URL del browser activo (`Page.Url`). Funciona para cualquier entorno sin hardcodear ni revisar env vars específicos.

```csharp
private string GetActivePortalBaseUrl()
{
    var current = Page.Url;
    if (!string.IsNullOrWhiteSpace(current)
        && !current.StartsWith("about:", StringComparison.OrdinalIgnoreCase)
        && Uri.TryCreate(current, UriKind.Absolute, out var uri))
    {
        return $"{uri.Scheme}://{uri.Host}";
    }
    // Fallback: prefer INT if set, otherwise non-INT
    return Environment.GetEnvironmentVariable(Constants.PORTAL_INT_BASE_URL)
           ?? Environment.GetEnvironmentVariable(Constants.PORTAL_BASE_URL)
           ?? Environment.GetEnvironmentVariable("BASE_URL")
           ?? throw new InvalidOperationException("PORTAL_BASE_URL (or BASE_URL) is required.");
}
```

### Regla general — aplicar siempre
> **Nunca usar `GetPortalBaseUrl()` en métodos de navegación dentro de un test en curso.**
> Usar `GetActivePortalBaseUrl()` para que el step funcione en cualquier entorno (INT, no-INT, staging, etc.) sin modificar el código.

---

## 20. Sesión: 2026-03-18 — Quick filter parametrizado + Hub logout steps

### Problema
1. `When I enter the shipment Reference in Quick filter` solo tenía binding `[When]`, sin `[Given]`/`[Then]`.
2. No existía un step para pasar texto directamente al Quick filter (ej. `When I enter "TC5305" in Quick filter`).
3. Faltaban steps de Hub logout: `Given/When/Then I log out from Hub`, `I click on the profile button in the hub`, `I click on Log out option in the hub`, `I should be in login page in the hub`.

### Solución
- **`ShipmentPage.cs`**: Se agregó `EnterTextInQuickFilterAsync(string text)` — método genérico que escribe cualquier texto en el Quick filter y valida que quedó escrito correctamente. `IEnterShipmentReferenceInQuickFilter()` refactorizado para llamar a este método con `_shipmentName`.
- **`ShipmentStepDefinitions.cs`**: Se agregaron aliases `[Given]`/`[Then]` al step existente y se agregó el nuevo step parametrizado:
  ```gherkin
  When I enter the shipment Reference in Quick filter   # usa el shipmentName almacenado
  When I enter "TC5305" in Quick filter                 # pasa texto directo por parámetro
  ```
- **`LoginPortalHubStepDefinitions.cs`**: Se agregaron steps de Hub logout usando selectores verificados con MCP:
  - `button#hubNavbarProfileItemButton` — botón de perfil
  - `span:has-text('Log out').First` — opción logout del menú
  - `input#username` — campo Auth0 para verificar login page

### Archivos modificados
- `StepDefinitions/ShipmentStepDefinitions.cs`
- `Pages/Web/ShipmentPage.cs`
- `StepDefinitions/LoginPortalHubStepDefinitions.cs`

---

## 21. Sesión: 2026-03-18 — Fix CheckCustomFieldValuesInTableViewAsync para Shipments

### Problema
`And I check the following custom field values in the table view:` fallaba con Timeout 15000ms en el escenario TC5305 (Shipments). El método en `WarehouseReceiptPage.CheckCustomFieldValuesInTableViewAsync` usaba `tbody tr.warehouse-receipt-row` como selector de fila, clase que no existe en la tabla de Shipments.

### Causa raíz
El step binding está en `WarehouseReceiptStepDefinitions.cs` y llama a `_warehouseReceiptPage.CheckCustomFieldValuesInTableViewAsync`. Reqnroll usa ese mismo binding para ambas features (WH y Shipments) porque comparten el mismo texto de step.

### Solución
Se modificó `CheckCustomFieldValuesInTableViewAsync` en `WarehouseReceiptPage.cs` para:
1. Intentar `tr.warehouse-receipt-row` con timeout de 3s (contexto WH)
2. Si no existe, usar `tbody tr` genérico (contexto Shipments)

Esto hace el método compatible con ambas páginas sin tocar el feature file ni el step definition.

### Archivos modificados
- `Pages/Web/WarehouseReceiptPage.cs`

`GetPortalBaseUrl()` solo sirve al inicio del test cuando aún no hay URL de página cargada.

### Archivos modificados
- `Pages/Web/ShipmentPage.cs` — nuevo método `GetActivePortalBaseUrl()`, usado en `UserNavigatedToShipmentsList`

---

## 22. Sesión: 2026-03-18 — Fix CheckCustomFieldValuesInShipmentTableViewAsync (Boolean not found)

### Problema
`And I check the following custom field values in the table view for shipment` fallaba con:
1. Primera versión: `Column 'Boolean': expected 'Yes' but found ''` (textContent vacío)
2. Segunda versión (con sentinels): `Column 'Boolean': expected 'Yes' but found '[__COL_NOT_FOUND__]'` (27s timeout)

El header `<th>` de "Boolean" no estaba en el DOM cuando el JS lo buscaba, aunque "INCO Terms" (columna anterior, índice 59) sí pasaba. Investigación vía MCP browser confirmó que el JS es correcto cuando el portal está completamente cargado, pero las columnas de custom fields cargan de forma asíncrona en el test.

### Causa raíz
Las columnas custom (indices 59+) se cargan asincrónicamente desde la API después del network idle inicial. En el test, `headers[59]` (INCO Terms) ya estaba presente pero `headers[60]` (Boolean) aún no. El polling JS no era suficiente — Playwright Locator con `WaitForAsync` es necesario para esperar correctamente.

### Solución
Se reescribió `CheckCustomFieldValuesInShipmentTableViewAsync` para:
1. Usar `Page.Locator("th").Filter(HasText=columnName).WaitForAsync(20000ms)` — Playwright espera el header de cada columna antes de leer el valor
2. Luego JS polling (12s) para leer el valor de la celda cuando Angular lo populea
3. `h.textContent` directo en el JS (más simple y confiable que `childNodes` iteration)

### Archivos modificados
- `Pages/Web/ShipmentPage.cs`

---

## 9. Sesión: 2026-03-18

### Problema
Implementar el escenario TC129 "HUB Quotation - Create Open Quotation (Full Load-Ocean)" en `QuotationsHub.feature` con todos los pasos del flujo de creación de cotización en el Hub.

### Solución
Se implementaron 13 nuevos pasos del flujo Hub Create Quotation:
1. Click en "Create quotation" button → `button.btn-outline-primary`
2. Verificar página de creación → `h3` heading "Create quotation"
3. Seleccionar Customer → primer `ng-select [role='combobox']`, tipo y selección
4. Seleccionar Load Type → segundo `ng-select`
5. Seleccionar Modality → tercer `ng-select`
6. Ingresar Origin → `[title='Origin']` + `.lta-suggestion-item`
7. Ingresar Destination → `[title='Destination']` + `.lta-suggestion-item`
8. Click Continue → `button.btn-primary` con texto "Continue"
9. Verificar sección commodity → `[placeholder='Select a commodity from the list']`
10. Seleccionar Commodity → ng-select de commodities
11. Seleccionar Currency → `ng-select[formcontrolname='currency']`
12. Seleccionar Container Size → `ng-select[formcontrolname='container_size']`
13. Click Create Quotation final → `button.btn-primary.btn-lg`

**Nota sobre conflictos de nombres**: Para evitar ambigüedad con steps existentes del Portal (Reqnroll es case-insensitive), se usó el sufijo "in the Hub" en 4 steps:
- `I should see the create quotation page in the Hub`
- `I select the commodity {string} in the Hub`
- `I select the currency {string} in the Hub`
- `I select the container Size {string} in the Hub`

### Archivos modificados
- `Features/QuotationsHub.feature` — Escenario TC129 completo con 13 steps
- `Pages/Web/QuotationPage.cs` — 13 nuevos métodos Hub: `ClickCreateQuotationButtonInHubAsync`, `ShouldSeeCreateQuotationPageInHubAsync`, `SelectCustomerInHubAsync`, `SelectLoadTypeInHubAsync`, `SelectModalityInHubAsync`, `EnterOriginInHubAsync`, `EnterDestinationInHubAsync`, `ClickContinueButtonInHubAsync`, `ShouldSeeCommoditySectionInHubAsync`, `SelectCommodityInHubAsync`, `SelectCurrencyInHubAsync`, `SelectContainerSizeInHubAsync`, `ClickCreateQuotationFinalInHubAsync`
- `StepDefinitions/QuotationStepDefinitions.cs` — 13 nuevos step bindings TC129

---

## 23. Sesión: 2026-03-20 — TC129 QuotationsHub completo + TC131/132/133/134

### Problema
Completar el escenario TC129 con todos los pasos restantes (additionals, Draft status, Publish, Offers, Open status, notificaciones, email). Luego migrar toda la lógica Hub a archivos separados (`QuotationsHubPage.cs` / `QuotationsHubStepDefinitions.cs`). Finalmente agregar los escenarios TC131, TC132, TC133 y TC134.

### Solución

**Arquitectura nueva:**
- `Pages/Web/QuotationsHubPage.cs` — nueva clase con todos los métodos Hub de creación de cotización (separado de `QuotationPage.cs`)
- `StepDefinitions/QuotationsHubStepDefinitions.cs` — nuevo archivo con todos los bindings Hub (separado de `QuotationStepDefinitions.cs`), inyecta `ShipmentTrackingStepDefinitions` para reutilizar verificación de email
- Todos los steps usan sufijo "in the Hub" para evitar ambigüedad con steps del Portal (Reqnroll es case-insensitive)

**Selectores verificados y problemas resueltos:**
- **ng-select dropdowns**: placeholder es `div.ng-placeholder` (no `input[placeholder]`); scope con `Has = Page.Locator(".ng-placeholder", new() { HasText = "..." })`
- **lta-button (Origin/Destination)**: es `div[title='Origin']` (no input). Click en el div → esperar `input[placeholder='Origin']` → fill → click `.lta-suggestion-item`
- **Customer ng-select**: esperar `.ng-placeholder` visible → click combobox → esperar 2s → fill → esperar 2s → Enter
- **Botón disabled Angular**: `Assertions.Expect(btn).ToBeEnabledAsync(timeout: 230s)` antes de click
- **Polling offers (hasta 5 min)**: loop chequeando badge count > 0 cada 5s
- **Quote ID**: extraer de `h3 span.text-muted` HasText "QUO-", almacenar en `_quoteId`, exponer con `GetQuoteId()`
- **`_quoteId` scope**: almacenado en `QuotationsHubPage`, NO en `QuotationPage` → siempre usar `_quotationsHubPage.GetQuoteId()` en los Hub steps
- **Email step reutilizado**: `ShipmentTrackingStepDefinitions.ThenIShouldReceiveAnEmailWithTextInTheBodyForShipment` ahora acepta `shipmentNameArg` vacío (usa `TryGetValue`, skip filter si empty)
- **`ShouldNotReceiveEmailWithTextForShipmentAsync`**: nuevo método público en `ShipmentTrackingStepDefinitions` para verificación negativa
- **Steps "in the hub" (lowercase)**: variantes de Publish y Yes button con atributos duplicados para ambas capitalizaciones

**Escenarios en `QuotationsHub.feature`:**
- TC129: Full Load - Ocean → Open (con offers, publish, notificación + email)
- TC131: Full Load - Truck → Open
- TC132: Partial Load - Air → Draft (sin notificación ni email)
- TC133: Partial Load - Ocean → Open
- TC134: Partial Load - Truck → Open (sin `I should see offers in the Hub` antes de publish)

### Archivos creados/modificados
- `Pages/Web/QuotationsHubPage.cs` — nuevo (todos los métodos Hub)
- `StepDefinitions/QuotationsHubStepDefinitions.cs` — nuevo (todos los bindings Hub)
- `Pages/Web/QuotationPage.cs` — `ShouldSeeQuoteIdInNotificationsAsync(quoteId)`, `ShouldNotSeeQuoteIdInNotificationsAsync(quoteId)`
- `StepDefinitions/ShipmentTrackingStepDefinitions.cs` — `shipmentNameArg` opcional + nuevo `ShouldNotReceiveEmailWithTextForShipmentAsync`
- `Support/DependencyInjection.cs` — registro de `QuotationsHubPage`
- `Features/QuotationsHub.feature` — TC129, TC131, TC132, TC133, TC134

### Guía MCP: verificar selectores antes de codificar
1. `browser_navigate` → URL objetivo
2. `browser_snapshot` → árbol accesibilidad + HTML visible
3. `browser_evaluate` → `document.querySelectorAll("selector").length` para contar elementos
4. `browser_click` → interactuar → `browser_snapshot` → ver qué cambió (ej: lta-button revela input tras click)
5. Regla: nunca asumir selector sin verificar — siempre confirmar con MCP antes de escribir C#

---

## 24. Sesión: 2026-03-20 — TC162 HUB Quotation Requests > Change Status to Closed

### Problema
Implementar TC162: desde la lista de Quotations del Hub, filtrar por status "Request", seleccionar la primera cotización, navegar al tab Requests, cerrar el request ingresando un reason, y verificar que el status queda "Closed".

### Selectores verificados con MCP
- **Status filter**: `p-dropdown[formcontrolname='status']` → click → `li.p-dropdown-item` HasText
- **Search button**: `button` HasText "Search"
- **First quotation**: `table tbody tr` First → `td a` First
- **Requests tab**: `a[href*='view=requests']`
- **Close button en row**: `button.btn-outline-danger` HasText "Close"
- **Dialog**: PrimeNG `p-dynamic-dialog` → textarea `placeholder='Close reason...'` + `button.confirm.btn-primary` HasText "Continue"
- **Status badge en row**: `.status-badge` en `table` con header "Status" → primer `tbody tr`
- **Continue del dialog**: reutiliza step `I click on continue button in the Hub` porque `button.btn-primary` matchea `button.confirm.btn-primary`

### Archivos modificados
- `Pages/Web/QuotationsHubPage.cs` — 7 nuevos métodos TC162
- `StepDefinitions/QuotationsHubStepDefinitions.cs` — 7 nuevos bindings TC162
- `Features/QuotationsHub.feature` — Escenario TC162 (entre TC133 y TC134)
- `Features/QuotationsHub.feature.cs` — Método TC162 + pickleIndex TC134 (4→5) + #line TC134 (+14)

### Corrección post-sesión: selector status badge TC162
El selector `.status-badge` en `ShouldSeeFirstRequestInStatusInHubAsync` fue corregido a `span.badge` porque el HTML real del elemento es `<span class="badge badge-success ng-star-inserted">Closed</span>`. El método final usa:
```csharp
requestsTable.Locator("tbody tr").First.Locator("span.badge").Filter(new LocatorFilterOptions { HasText = status })
```
con timeout 15s y `WaitForAsync` + `Assert.IsTrue(IsVisibleAsync)`.



---

## 25. Sesión: 2026-03-20 — TC280 Hub-Home_280

### Problema
Implementar el escenario TC280 en `HomeHub.feature`: verificar que la página Home del Hub muestra las secciones principales (User Approvals, Quotation Requests, Recent Notifications, Your Sites) con listas no vacías, que el botón "View Quotations" navega a la página correcta, y que el botón "View" en Recent Notifications redirige a una página con el breadcrumb Qwyk.

### Selectores verificados con MCP (live snapshot)
- **View Quotations link**: `a[title="Quotations"]` → href="/quotations" → redirige a `/quotations/list`
- **Quotations page h3**: `h3` HasText "Quotations" en URL que contiene "quotations/list"
- **Section headings**:
  - User Approvals → `h5` level=5
  - Your Sites → `h5` level=5
  - Quotation Requests → `a[href='/quotations']` link header
  - Recent Notifications → `a[href='/notifications']` link header
- **Section lists not empty**: JavaScript evaluate `el.parentElement.querySelectorAll('ul > li').length > 0`
- **View button en notifications**: `button` HasText "View" dentro del contenedor `div` que contiene `a` HasText "Recent Notifications"
- **Qwyk breadcrumb**: `a[href='/']` HasText "Qwyk" → confirmado en `/shipments/<id>` y `/quotations/<id>`

### Arquitectura nueva
- `Pages/Web/HomeHubPage.cs` — nueva clase con 6 métodos: `NavigateToHomeInHubAsync`, `ClickViewQuotationsButtonInHubAsync`, `ShouldSeeQuotationsPageInHubAsync`, `ShouldSeeSectionHeaderInHubAsync(string)`, `SectionListShouldNotBeEmptyInHubAsync(string)`, `ClickFirstViewButtonInNotificationsInHubAsync`, `ShouldSeeQwykBreadcrumbInHubAsync`
- `StepDefinitions/HomeHubStepDefinitions.cs` — nuevo archivo con 7 bindings (los de sección son parametrizables con `{string}`)
- Steps parametrizables: `I should see the section header {string} in the Hub` y `the {string} list should not be empty in the Hub`

### Archivos creados/modificados
- `Pages/Web/HomeHubPage.cs` — nuevo
- `StepDefinitions/HomeHubStepDefinitions.cs` — nuevo
- `Features/HomeHub.feature` — TC280 completo (15 steps)
- `Features/HomeHub.feature.cs` — generado con pickleIndex="0", #line 7-22
- `Support/DependencyInjection.cs` — registro de `HomeHubPage`

### Build: 0 errores, 5 advertencias (pre-existentes CS8981)

---

## 29. Sesión: 2026-03-23 — Fix TC1483 multi-problema + Fix portal login dual-form (TC145/TC146)

### Problemas resueltos

**1. `When I enter the created username "" in the Portal` — Timeout 15000ms en `input#email[name='email']`**
- **Causa raíz**: Al hacer clic en "Login to Magaya" en el email de yopmail, el enlace abría una nueva pestaña. La referencia `IPage` del test permanecía apuntando a la pestaña de yopmail, por lo que el intento de localizar `input#email` en esa pestaña fallaba (no existe en yopmail).
- **Fix**: `YopmailPage.ClickLinkInEmailAsync` usa `Page.Context.WaitForPageAsync()` antes del click para capturar la nueva pestaña. Obtiene su URL, la cierra, y navega el `IPage` actual a esa URL. Todos los page objects siguen sincronizados.

**2. `And I enter the password "" in the Portal` — KeyNotFoundException 'PortalPassword'**
- **Causa raíz**: `_scenarioContext["PortalPassword"]` lanza excepción si la clave no existe.
- **Fix**: Cambiado a `TryGetValue` con fallback a string vacío en `HubAdminUsersPortalStepsDefinitions.IEnterThePasswordInThePortal`.

**3. `And I store the password for the portal user created in the Hub` — Timeout XPath `(//strong[contains(@style, 'position: relative')])[3]`**
- **Causa raíz**: El step se usaba en dos contextos distintos — en la pantalla del Hub (línea 51) y en el email de yopmail (línea 61). El XPath del frame `#ifmail` no existe en la pantalla del Hub.
- **Fix**: Separación en dos steps:
  - Línea 51: `And I store the password for the portal user created in the Hub` → lee `input[formcontrolname='password']` + `InputValueAsync()` (Hub form disabled input)
  - Línea 61: `And I store the portal user password from the email` → nuevo step en `YopmailStepsDefinitions` que llama a `YopmailPage.ReadPasswordFromEmailAsync()` (XPath en frame `#ifmail`)

**4. `And I set the portal password "" imprime el password`**
- **Fix**: Si el parámetro está vacío → solo imprime el valor actual de `ScenarioContext["PortalPassword"]`. Si tiene valor → lo sobreescribe.

**5. `And I should see the login page` — step faltante**
- **Fix**: Nuevo método `HubAdminUsersPortalPage.ShouldSeeLoginPageAsync()` que verifica `img[alt='Logo'][src*='site_contrast_logo']`. Nuevo binding en `HubAdminUsersPortalStepsDefinitions`.

**6. `Given I login to Portal as user "automationdfpowner@gmail.com"` — "Email address is required" (TC145/TC146)**
- **Causa raíz**: El portal cambió su UI — ahora tiene DOS formularios de login en la misma página (un formulario inline en el fondo + un modal dialog). `FindPasswordInputAsync` capturaba el password del formulario inline (primero en el DOM), mientras el email se llenaba en el modal → al hacer Sign in en el formulario incorrecto, validación fallaba.
- **Fix 1**: Cambiar `Page.Locator("dialog").First` a `Page.Locator("dialog:not([aria-label='cookieconsent'])").First` para excluir los dialogs de cookie consent y apuntar solo al dialog de login.
- **Fix 2**: Usar `WaitForAsync(State=Visible, Timeout=8000)` con try/catch para detectar el dialog (en vez de `IsVisibleAsync` que es snapshot instantáneo).
- **Fix 3**: `EnsureLoginFormAsync` ahora hace click en el link "Sign in" del navbar (`nav a[href='/my-portal']:has-text('Sign in')`) con timeout 5s, con fallback a navegación directa a `/my-portal`. El usuario solicitó explícitamente click en vez de navegación.

### Archivos modificados
- `Pages/Web/YopmailPage.cs` — `ClickLinkInEmailAsync` (new tab handling), `ReadPasswordFromEmailAsync` (nuevo método)
- `StepDefinitions/YopmailStepsDefinitions.cs` — nuevo step `I store the portal user password from the email`
- `Pages/Web/HubAdminUsersPortalPage.cs` — `ReadPortalUserPasswordAsync` (Hub form), `ShouldSeeLoginPageAsync` (nuevo)
- `StepDefinitions/HubAdminUsersPortalStepsDefinitions.cs` — `ISetThePortalPassword` (print-only cuando vacío), `IEnterThePasswordInThePortal` (TryGetValue), `IShouldSeeTheLoginPage` (nuevo)
- `Pages/Web/LoginPage.cs` — `EnsureLoginFormAsync` (click Sign in navbar + fallback), `LoginToDFPAsync` (dialog:not cookieconsent + WaitForAsync)
- `Features/CustomersHub.feature` — TC1483 línea 61 → `And I store the portal user password from the email`

---

## 13. Sesión: 2026-03-20

### Problema
Implementar TC1482 Hub-Create Customer en CustomersHub.feature: navegar a Portal Customers, crear un customer (con nombre auto-generado si el parámetro está vacío), seleccionar tipo y segmento, guardar, buscar por nombre y verificar en resultados.

### Solución
1. Se creó `CustomersHubPage.cs` con 7 métodos usando MCP para verificar selectores reales:
   - `NavigateToPortalCustomersInHubAsync` — GotoAsync a `/administration/portal-teams`
   - `ClickCreateCustomerButtonInHubAsync` — botón "Create customer" (primer visible)
   - `EnterCustomerNameInHubAsync` — input `placeholder='Team name'`
   - `SelectTypeInHubAsync` — radio via `label.custom-control-label` HasText
   - `SelectSegmentInHubAsync` — ng-select con `.ng-placeholder "Segment"` y `.ng-option-label`
   - `ClickSearchButtonAsync` — espera con `WaitForFunctionAsync` hasta que `!btn.disabled`
   - `ShouldSeeCustomerNameInResultsAsync` — `td` GetByText exact
2. Se creó `CustomersHubStepDefinitions.cs` con `ScenarioContext` inyectado:
   - Si parámetro nombre vacío → genera `AutoTest{yyyyMMddHHmmss}` y guarda en `_scenarioContext["CustomerName"]`
   - Pasos de búsqueda y verificación leen `_scenarioContext["CustomerName"]` cuando param está vacío
3. Se registró `CustomersHubPage` en `DependencyInjection.cs`
4. `CustomersHub.feature.cs` ya existía correctamente generado

### Archivos creados/modificados
- `Pages/Web/CustomersHubPage.cs` — nuevo
- `StepDefinitions/CustomersHubStepDefinitions.cs` — nuevo
- `Support/DependencyInjection.cs` — registro de `CustomersHubPage`
- `Features/CustomersHub.feature.cs` — ya existía con estructura correcta

### Build: 0 errores, 5 advertencias (pre-existentes CS8981)


---

## 28. Sesión: 2026-03-23 — TC1483 HUB Customer Create Portal User (continuación)

### Problema
Completar TC1483 con los pasos de creación de usuario portal desde el Hub: leer contraseña autogenerada, seleccionar site, crear usuario, verificar email en yopmail, y hacer login en el portal con el usuario recién creado.

### Solución

**Nuevos archivos creados:**
- `Pages/Web/YopmailPage.cs` — Navegación/inbox yopmail, polling 5 min (60 × 5s) en `WaitForEmailAsync`, `ClickLinkInEmailAsync` con scroll + XPath `contains(., 'Login to Magaya')`, frame `#ifmail`, `VerifyEmailBodyContainsAsync`
- `StepDefinitions/YopmailStepsDefinitions.cs` — Steps: `I go to yopmail URL`, `I create my yopmail email {string}` (con domain-check para skip si no es yopmail), `I should receive an email with text {string} in the body`, `I click on Login to Magaya in the email`
- `StepDefinitions/HubAdminUsersPortalStepsDefinitions.cs` — Steps TC1483: `I store the now var`, `I store the new contact email {string}` (también guarda como `usernamePortal`), `I set the portal password {string}`, `I enter the email for the portal user in the Hub`, `I store the password for the portal user created in the Hub`, `I select the site {string}`, `I enter the User Name {string}`, `I enter the company name {string} in the Hub`, `I confirm the privacy`, `I click on create user button in the Hub`, 3 pasos de login al portal: `I enter the created username {string} in the Portal`, `I enter the password {string} in the Portal`, `click on Sign in button`
- `Pages/Web/HubAdminUsersPortalPage.cs` — Métodos página: `EnterEmailForPortalUserAsync`, `ReadPortalUserPasswordAsync` (XPath `(//strong[contains(@style, 'position: relative')])[3]`), `SelectSiteAsync`, `EnterUserNameAsync`, `EnterCompanyNameAsync`, `ConfirmPrivacyAsync`, `ClickCreateUserSubmitButtonAsync`, `FillPortalUsernameAsync` (`input#email[name='email']` + `ToBeEnabledAsync`), `FillPortalPasswordAsync` (`input#password[name='password']`), `ClickPortalSignInAsync` (`button[type='submit'].btn-primary` "Sign in" + `WaitForDashboardAsync`)

**Archivos modificados:**
- `Features/CustomersHub.feature` — TC1483 con todos los pasos finales + escenario debug `@1483debug` con email fijo `20260323113948contact@yopmail.com`
- `StepDefinitions/CustomersHubStepDefinitions.cs` — Pasos previos TC1483: `I click on users tab in the Hub` (href `?view=users`), `I click on create user button` (outline button), `I select the customer {string} in the results in the Hub`
- `Pages/Web/CustomersHubPage.cs` — `ClickUsersTabInHubAsync` con selector `a.nav-link[href*='?view=users']`, `SelectCustomerInResultsAsync`, `ClickCreateUserOutlineButtonAsync`
- `StepDefinitions/ShipmentTrackingStepDefinitions.cs` — `WhenICheckTheEmailForWithUsername` delega a `YopmailPage.WaitForEmailAsync`, soporte email vacío desde `ScenarioContext["ContactEmail"]`
- `Support/DependencyInjection.cs` — Registro de `YopmailPage` y `HubAdminUsersPortalPage`

### Problemas encontrados y fixes
- **Locator ambiguo** `I click on Search button`: conflicto con ShipmentStepDefinitions → renombrado a `I click on Search button in Customers Hub`
- **`ClickUsersTabInHubAsync` Timeout**: selector `a.nav-link` con texto "Users" encontraba el link de sidebar oculto → fix: `a.nav-link[href*='?view=users']`
- **`WaitForEmailAsync` TimeoutException en inbox vacío**: `GetLatestYopmailEmailBodiesFromTodayAsync` lanza cuando `div.m` no visible → fix: try/catch trata excepción como "inbox vacío, seguir polling"
- **`I click on Login to Magaya` Timeout**: XPath `//a[.='Login to Magaya']` no matcheaba porque texto real es "Login to MagayaQA" + elemento fuera de pantalla → fix: `contains(., 'Login to Magaya')` + `ScrollIntoViewIfNeededAsync` con `WaitForSelectorState.Attached`
- **`I enter the created username` Timeout**: Reemplazó `LoginPage.FillUsernameAsync` (que navegaba a `/my-portal`) por selectors directos: `input#email[name='email']` + `ToBeEnabledAsync`

### Build: 0 errores, 5 advertencias (pre-existentes CS8981)

---

## 30. Sesión: 2026-03-23 — TC3072 Custom Fields WR + Fix ShipmentPage Reload

### Problemas

1. **`CheckCustomFieldValueAsync` siempre retorna vacío**: Las celdas de custom fields en la tabla PrimeNG (`p-datatable-table`) renderizan como nodos Comment de Angular `*ngIf` y `textContent` siempre devuelve `""` hasta que carga el API async.
2. **`Assert.IsGreaterThanOrEqualTo(colIndex, 0)` invertido**: MSTest: primer param = lower bound, segundo = valor actual. Con `colIndex=47` como primer arg y `0` como segundo, chequea `0 >= 47` → FALSO aunque la columna sí se encontró.
3. **`Given I set the warehouse receipt name to ""` sin fallback**: Cuando se pasa string vacío, debía leer el WR del contexto compartido.
4. **`TheShipmentShouldAppearInSearchResults` selector incorrecto**: Botón Reload tiene clase `btn-outline-secondary` (no `btn-outline-danger`), y el intervalo/tiempo máximo eran 5s/3min en vez de 2s/1min.

### Solución

1. **`CheckCustomFieldValuesInTableViewAsync`** (`Pages/Web/WarehouseReceiptPage.cs`): Nuevo método con DataTable `(columnName, expectedValue)`. Usa JS `th.cellIndex` para obtener el índice real de columna, luego polling `row.cells[N].textContent` cada 500ms hasta 12s por columna.
2. **Assert fix**: Reemplazado `Assert.IsGreaterThanOrEqualTo(colIndex, 0)` por `Assert.IsTrue(colIndex >= 0, ...)`.
3. **Step fallback** (`StepDefinitions/WarehouseReceiptStepDefinitions.cs`): `ISetTheWarehouseReceiptNameTo` cuando `name` es vacío lee `_tc.Data["warehouseReceiptName"]`.
4. **DataTable step** (`StepDefinitions/WarehouseReceiptStepDefinitions.cs`): Nuevo step `[Then("I check the following custom field values in the table view:")]` con `Table dataTable` de Reqnroll.
5. **Warehouse.feature TC3072**: Step reemplazado por DataTable con 10 columnas custom fields.
6. **`TheShipmentShouldAppearInSearchResults`** (`Pages/Web/ShipmentPage.cs`): Selector corregido a `button.btn-outline-secondary:has(svg[data-icon='arrows-rotate'])`, intervalo 2s, máximo 1 minuto.

### Archivos modificados
- `Pages/Web/WarehouseReceiptPage.cs`
- `Pages/Web/ShipmentPage.cs`
- `StepDefinitions/WarehouseReceiptStepDefinitions.cs`
- `Features/Warehouse.feature`

---

## 31. Sesión: 2026-03-24 — TC158 HUB Quotation - Download All Quotations (PENDIENTE)

### Objetivo
Implementar TC158: desde la lista de Quotations del Hub, descargar el PDF "All Quotations" y verificar que toda la información de la cotización está correctamente reflejada en el PDF descargado.

### Estrategia de implementación
Guardar la mayor cantidad de datos de la cotización en variables (ScenarioContext o campos de la page class) **antes** de descargar el PDF, y luego verificar que esos mismos datos aparecen en el PDF.

### Datos a capturar desde la UI (HTML conocido)
Del panel de detalles de la cotización (selector base: `ul.list-group.list-group-flush`):

| Campo | Selector | Valor ejemplo |
|---|---|---|
| Modality | `li:has(.col-4:text('Modality')) .col-8` | `Ocean FCL` |
| Origin | `li:has(.col-4:text('Origin')) .col-8 a.p-element` | `Los Angeles, CA` |
| Destination | `li:has(.col-4:text('Destination')) .col-8 a.p-element` | `Shanghai Pt, 31` |
| Currency | `li:has(.col-4:text('Currency')) .col-8` | `USD` |
| Cargo ready | `li:has(.col-4:text('Cargo ready')) .col-8` | `03/23/2026` |
| Commodity | `li:has(.col-4:text('Commodity')) .col-8` | `Agriculture (Fruits and Vegetables)` |

También capturar (si visibles en la cotización):
- Quote ID → de `h3 span.text-muted` HasText "QUO-" (ya implementado: `_quotationsHubPage.GetQuoteId()`)
- Customer name
- Carrier / Vessel info (si aplica)
- Total price / charges

### Flujo del test TC158
1. Login al Hub
2. Navegar a Quotations list
3. Abrir una cotización específica (o la primera con status "Open")
4. **Capturar todos los datos en variables** (ScenarioContext)
5. Hacer clic en el botón Download / "All Quotations" (verificar selector real con MCP)
6. Esperar que se descargue el PDF
7. Leer el contenido del PDF (con `PdfSharp` o `iText7` o similar)
8. Verificar que cada dato capturado aparece en el texto del PDF

### TODO antes de implementar
- [ ] Navegar con MCP al Hub → Quotations → abrir una cotización → snapshot para ver el botón de descarga exacto
- [ ] Verificar si el botón descarga directamente o muestra un dropdown con opciones
- [ ] Confirmar selector del botón: posiblemente `button:has(svg[data-icon='download'])` o similar
- [ ] Decidir librería para leer PDF en C# (ya usada en el proyecto o nueva)
- [ ] Ver si el PDF se descarga como archivo o se abre en nueva pestaña/tab

### HTML del panel Details (capturado)
```html
<ul class="list-group list-group-flush">
  <!-- Modality: "Ocean  FCL" -->
  <li class="list-group-item border-0">
    <div class="row">
      <div class="col-4">Modality</div>
      <div class="col-8"> Ocean  FCL </div>
    </div>
  </li>
  <!-- Origin: "Los Angeles, CA" (anchor con href maps) -->
  <li class="list-group-item border-0 bg-light">
    <div class="col-4">Origin</div>
    <div class="col-8">
      <a class="p-element text-wrap text-dark flex-grow-1"
         href="https://www.google.com/maps/search/?q=Los%2BAngeles%2C%20CA+US&sll=34.05,-118.24">
        Los Angeles, CA
      </a>
    </div>
  </li>
  <!-- Destination: "Shanghai Pt, 31" -->
  <!-- Currency: "USD" -->
  <!-- Cargo ready: "03/23/2026" -->
  <!-- Commodity: "Agriculture (Fruits and Vegetables)" -->
</ul>
```

### Archivos a crear/modificar (pendiente)
- `Features/QuotationsHub.feature` — Escenario TC158
- `Features/QuotationsHub.feature.cs` — runner generado
- `Pages/Web/QuotationsHubPage.cs` — métodos de captura de datos + descarga + verificación PDF
- `StepDefinitions/QuotationsHubStepDefinitions.cs` — bindings TC158

### IMPLEMENTADO — Sesión 2026-03-24

#### Hallazgos MCP (verificados en vivo)
- **Download en la lista**: `tfoot button:has-text('Download')` → descarga `download.csv` (solo la página actual, 25 filas = 1 header + 25 data rows)
- **Download en el detalle**: `button:has-text('Download').First` → descarga `quotation-{uuid}.pdf`
- **Pagination count**: texto "1 - 25 of N" en `generic` al lado del combobox "Rows per page"
- **Panel Details**: `ul.list-group.list-group-flush > li.list-group-item` con `.col-4` (label) y `.col-8` (valor); Origin/Destination usan `a.p-element` dentro del `.col-8`

#### Lógica del test
1. **CSV**: `_totalQuotationsCount = await Page.Locator("table tbody tr").CountAsync()` → download CSV → `lines.Length - 1 == _totalQuotationsCount`
2. **PDF**: `StoreAllInformationInQuoteAsync()` guarda Modality/Origin/Destination/Currency/CargoReady/Commodity → download → verificar que el archivo .pdf existe

#### Archivos modificados
- `Features/QuotationsHub.feature` — TC158 completo (2 partes: CSV + PDF)
- `Features/QuotationsHub.feature.cs` — TestMethod actualizado con todos los steps
- `Pages/Web/QuotationsHubPage.cs` — 9 nuevos elementos: 5 campos privados TC158 + 4 métodos + `ReadDetailFieldAsync` helper + getters
- `StepDefinitions/QuotationsHubStepDefinitions.cs` — 6 nuevos bindings TC158

#### Steps PDF (adición posterior misma sesión)
- `Then the downloaded PDF file should exist` → reemplazado por dos steps:
  - `When I open the downloaded PDF in the Hub` → abre el PDF con **PdfPig** (NuGet `UglyToad.PdfPig 1.7.0-custom-5`), extrae todo el texto en `_pdfText`
  - `Then I verify all information in the PDF in the Hub` → verifica que Quote ID + Modality + Origin + Destination + Currency + Cargo ready + Commodity aparecen en `_pdfText` (OrdinalIgnoreCase); lista todos los campos faltantes en el Assert message

#### Build: 0 errores, 5 advertencias pre-existentes CS8981

---

## 5. Sesión: 2026-03-25 — Error al levantar el proyecto en otra laptop (UglyToad.PdfPig prerelease)

### Problema / Consulta
Al intentar compilar el proyecto en una laptop diferente, se producían dos errores:
1. **CS0103:** `The name 'PdfDocument' does not exist in the current context` en `QuotationsHubPage.cs:730`
2. **NU1103:** `Unable to find a stable package UglyToad.PdfPig with version (>= 0.1.9)` al intentar cambiar la versión

### Causa raíz
El paquete `UglyToad.PdfPig` versión `1.7.0-custom-5` es una versión **prerelease** (sufijo `-custom-5`). NuGet por defecto **no restaura paquetes prerelease**, por lo que el restore falla silenciosamente y `PdfDocument` no queda disponible en compilación.

### Solución
La versión en el `.csproj` debe mantenerse en `1.7.0-custom-5` (es la única disponible en nuget.org para este paquete). Para restaurarla correctamente hay que pasar el flag `--include-prerelease`:

```bash
dotnet nuget locals all --clear
dotnet restore --include-prerelease
dotnet build
```

### Archivos modificados
- `DFP.Playwright.csproj` — se revirtió a `UglyToad.PdfPig 1.7.0-custom-5` (no cambiar la versión)

---

## 33. Sesión: 2026-03-26 — Nuevos steps de formulario de Shipment en el Portal

### Problema / Consulta
Se necesitaban step definitions y métodos de página para rellenar el formulario de edición/booking de un Shipment en el Portal, incluyendo campos de Vessel, tabs de navegación, referencias (Shipper/Consignee/Notify/Forwarder), Name, Address e Instructions.

### Solución
Se añadieron selectores y métodos en `ShipmentPage.cs` y los step bindings correspondientes en `ShipmentStepDefinitions.cs`:

- `I enter the vessel {string} in the Portal` → `input[formcontrolname='vessel']`
- `I go to {string} Tab in the Shipment Portal` → XPath nav tab por texto
- `I enter the Shipper {string} in the Shipment Portal` → `input[formcontrolname='shipper_reference']`
- `I enter the Consignee {string} in the Shipment Portal` → `input[formcontrolname='consignee_reference']`
- `I enter the Notify {string} in the Shipment Portal` → `input[formcontrolname='notify_reference']`
- `I enter the Forwarder {string} in the Shipment Portal` → `input[formcontrolname='forwarder_reference']`
- `I enter the name {string} in the Shipment Portal` → `input[formcontrolname='name'][placeholder='Name']`
- `I enter the address {string} in the Shipment Portal` → `input[formcontrolname='address_1']`
- `I enter the Instructions remarks {string} in the Shipment Portal` → textarea de instrucciones; si el parámetro está vacío, usa `DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")` (NOW).

### Archivos modificados
- `DFP.Playwright/Pages/Web/ShipmentPage.cs` — selectores + 9 métodos nuevos en sección "Portal Shipment Form"
- `DFP.Playwright/StepDefinitions/ShipmentStepDefinitions.cs` — 9 step bindings nuevos en sección "Portal Shipment Form steps"

