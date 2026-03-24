using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class HubRadarStepDefinitions
    {
        private readonly HubRadarPage _radarPage;
        private IDownload? _lastDownload;

        public HubRadarStepDefinitions(HubRadarPage radarPage)
        {
            _radarPage = radarPage;
        }

        // ── Navigation ────────────────────────────────────────────────────────────

        [When("I navigate to radar page")]
        public async Task WhenINavigateToRadarPage()
            => await _radarPage.NavigateToRadarPageAsync();

        [When("I click on view sales radar")]
        public async Task WhenIClickOnViewSalesRadar()
            => await _radarPage.ClickViewSalesRadarAsync();

        // ── Section verifications ─────────────────────────────────────────────────

        [Then("I verify Performance over Time section")]
        public async Task ThenIVerifyPerformanceOverTimeSection()
            => await _radarPage.VerifyPerformanceOverTimeSectionAsync();

        [Then("I verify Rankings section")]
        public async Task ThenIVerifyRankingsSection()
            => await _radarPage.VerifyRankingsSectionAsync();

        [Then("I verify Data Table section")]
        public async Task ThenIVerifyDataTableSection()
            => await _radarPage.VerifyDataTableSectionAsync();

        [Then("the radar charts are visible")]
        public async Task ThenTheRadarChartsAreVisible()
            => await _radarPage.VerifyRadarChartsVisibleAsync();

        // ── Period filter ─────────────────────────────────────────────────────────

        [When("I filter radar by period {string}")]
        public async Task WhenIFilterRadarByPeriod(string period)
            => await _radarPage.FilterByPeriodAsync(period);

        // ── Filters panel ─────────────────────────────────────────────────────────

        [When("I open radar filters")]
        public async Task WhenIOpenRadarFilters()
            => await _radarPage.OpenRadarFiltersAsync();

        [When("I filter radar by transport mode {string}")]
        public async Task WhenIFilterRadarByTransportMode(string mode)
            => await _radarPage.FilterByTransportModeAsync(mode);

        [When("I filter radar by load type {string}")]
        public async Task WhenIFilterRadarByLoadType(string loadType)
            => await _radarPage.FilterByLoadTypeAsync(loadType);

        [When("I filter radar by account manager {string}")]
        public async Task WhenIFilterRadarByAccountManager(string manager)
            => await _radarPage.FilterByAccountManagerAsync(manager);

        [When("I apply radar filters")]
        public async Task WhenIApplyRadarFilters()
            => await _radarPage.ApplyRadarFiltersAsync();

        [When("I reset radar filters")]
        public async Task WhenIResetRadarFilters()
            => await _radarPage.ResetRadarFiltersAsync();

        // ── Toolbar ───────────────────────────────────────────────────────────────

        [When("I click the radar refresh button")]
        public async Task WhenIClickTheRadarRefreshButton()
            => await _radarPage.ClickRefreshButtonAsync();

        [When("I click the radar full screen button")]
        public async Task WhenIClickTheRadarFullScreenButton()
            => await _radarPage.ClickFullScreenButtonAsync();

        [Then("the radar is displayed in full screen")]
        public async Task ThenTheRadarIsDisplayedInFullScreen()
            => await _radarPage.VerifyFullScreenModeAsync();

        [Then("I exit full screen mode")]
        public async Task ThenIExitFullScreenMode()
            => await _radarPage.ExitFullScreenModeAsync();

        // ── Data table column verification ───────────────────────────────────────

        [Then("the data table column {string} should have value {string}")]
        public async Task ThenTheDataTableColumnShouldHaveValue(string columnName, string expectedValue)
            => await _radarPage.VerifyDataTableColumnAsync(columnName, expectedValue);

        // ── Rankings metric ───────────────────────────────────────────────────────

        [When("I filter radar rankings by metric {string}")]
        public async Task WhenIFilterRadarRankingsByMetric(string metric)
            => await _radarPage.FilterRankingsByMetricAsync(metric);

        // ── Export ────────────────────────────────────────────────────────────────

        [When("I export radar data to CSV")]
        public async Task WhenIExportRadarDataToCsv()
            => _lastDownload = await _radarPage.ExportToCsvAsync();

        [When("I export radar data to Excel")]
        public async Task WhenIExportRadarDataToExcel()
            => _lastDownload = await _radarPage.ExportToExcelAsync();

        [Then("a radar file is downloaded")]
        public async Task ThenARadarFileIsDownloaded()
        {
            Assert.IsNotNull(_lastDownload, "Expected a file download to have started.");
            var filename = _lastDownload.SuggestedFilename;
            Assert.IsFalse(string.IsNullOrWhiteSpace(filename),
                "Expected the downloaded file to have a suggested filename.");

            // For CSV: verify the file contains all Data Table headers
            if (filename.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                var tempPath = Path.Combine(Path.GetTempPath(), filename);
                await _lastDownload.SaveAsAsync(tempPath);

                try
                {
                    var lines = await File.ReadAllLinesAsync(tempPath);
                    Assert.IsGreaterThan(0, lines.Length, "Downloaded CSV file is empty.");

                    var csvHeaders = lines[0].Split(',')
                        .Select(h => h.Trim().Trim('"'))
                        .ToList();

                    var pageHeaders = await _radarPage.GetDataTableHeadersAsync();
                    foreach (var header in pageHeaders)
                    {
                        Assert.IsTrue(
                            csvHeaders.Any(ch => ch.Equals(header, StringComparison.OrdinalIgnoreCase)),
                            $"Expected CSV to contain header '{header}'. Found: {string.Join(", ", csvHeaders)}");
                    }
                }
                finally
                {
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);
                }
            }
        }
    }
}
