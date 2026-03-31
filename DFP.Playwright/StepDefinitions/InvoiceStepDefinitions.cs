using Reqnroll;
using System.Linq;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class InvoiceStepDefinitions
    {
        private readonly InvoicePage _invoicePage;
        private readonly ShipmentPage _shipmentPage;
        private readonly DFP.Playwright.Support.TestContext _tc;

        public InvoiceStepDefinitions(InvoicePage invoicePage, ShipmentPage shipmentPage, DFP.Playwright.Support.TestContext tc)
        {
            _invoicePage = invoicePage;
            _shipmentPage = shipmentPage;
            _tc = tc;
        }

        // ── Navigation steps ──────────────────────────────────────────────────────

        [Given("I go to charges invoice tab in the invoice details page")]
        [When("I go to charges invoice tab in the invoice details page")]
        [Then("I go to charges invoice tab in the invoice details page")]
        public async Task IGoToChargesInvoiceTab()
            => await _invoicePage.ClickChargesTabAsync();

        [Given("I navigated to Invoices List")]
        [When("I navigated to Invoices List")]
        [Then("I navigated to Invoices List")]
        public async Task INavigatedToInvoicesList()
            => await _invoicePage.NavigateToInvoicesListAsync();

        // ── Search steps ──────────────────────────────────────────────────────────

        [Given("I set the invoice name to {string}")]
        [When("I set the invoice name to {string}")]
        [Then("I set the invoice name to {string}")]
        public void ISetTheInvoiceNameTo(string name)
        {
            if (string.IsNullOrEmpty(name) &&
                _tc.Data.TryGetValue("invoiceName", out var ctxName) &&
                ctxName is string inv &&
                !string.IsNullOrEmpty(inv))
            {
                name = inv;
            }
            _invoicePage.SetInvoiceName(name);
        }

        [Given("I enter the invoice name {string} in search field")]
        [When("I enter the invoice name {string} in search field")]
        [Then("I enter the invoice name {string} in search field")]
        public async Task IEnterTheInvoiceNameInSearchField(string name)
            => await _invoicePage.EnterInvoiceNameInSearchFieldAsync(name);

        // ── Assertion steps ───────────────────────────────────────────────────────

        [Given("I select the invoice in the search results with text {string}")]
        [When("I select the invoice in the search results with text {string}")]
        [Then("I select the invoice in the search results with text {string}")]
        public async Task ISelectTheInvoiceInTheSearchResultsWithText(string text)
            => await _invoicePage.SelectInvoiceInSearchResultsWithTextAsync(text);

        [Given("the invoice should appear in the search results in the List with text {string}")]
        [When("the invoice should appear in the search results in the List with text {string}")]
        [Then("the invoice should appear in the search results in the List with text {string}")]
        public async Task TheInvoiceShouldAppearInTheSearchResultsInTheList(string text)
            => await _invoicePage.TheInvoiceShouldAppearInSearchResultsInListAsync(text);

        [Given("I should see the uploaded file {string}"), Scope(Feature = "Invoices")]
        [When("I should see the uploaded file {string}"), Scope(Feature = "Invoices")]
        [Then("I should see the uploaded file {string}"), Scope(Feature = "Invoices")]
        public async Task IShouldSeeTheUploadedFile(string fileName)
            => await _invoicePage.VerifyUploadedFileAsync(fileName);

        [Given("the invoice details should be displayed with the name {string}")]
        [When("the invoice details should be displayed with the name {string}")]
        [Then("the invoice details should be displayed with the name {string}")]
        public async Task TheInvoiceDetailsShouldBeDisplayedWithTheName(string name)
            => await _invoicePage.VerifyInvoiceDetailsHeadingAsync(name);

        [Given("I should verify the following label headers in invoice details:")]
        [When("I should verify the following label headers in invoice details:")]
        [Then("I should verify the following label headers in invoice details:")]
        public async Task IShouldVerifyTheFollowingLabelHeadersInInvoiceDetails(Table dataTable)
        {
            var pairs = dataTable.Rows.Select(r => (labelText: r[0], expectedValue: r[1]));
            await _invoicePage.VerifyInvoiceLabelHeadersAsync(pairs);
        }

        [Given("I should verify the following custom field values in invoice details in DFP:")]
        [When("I should verify the following custom field values in invoice details in DFP:")]
        [Then("I should verify the following custom field values in invoice details in DFP:")]
        public async Task IShouldVerifyTheFollowingCustomFieldValuesInInvoiceDetails(Table dataTable)
        {
            var pairs = dataTable.Rows.Select(r => (labelText: r[0], expectedValue: r[1]));
            await _invoicePage.VerifyInvoiceCustomFieldsAsync(pairs);
        }
    }
}
