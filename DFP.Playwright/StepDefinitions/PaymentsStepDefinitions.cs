using Reqnroll;
using System.Linq;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class PaymentsStepDefinitions
    {
        private readonly PaymentsPage _paymentsPage;

        public PaymentsStepDefinitions(PaymentsPage paymentsPage)
        {
            _paymentsPage = paymentsPage;
        }

        [Given("I am on the Payments page")]
        [When("I am on the Payments page")]
        [Then("I am on the Payments page")]
        public async Task IAmOnThePaymentsPage()
            => await _paymentsPage.NavigateToPaymentsPageAsync();

        [Given("I should see the Payments list")]
        [When("I should see the Payments list")]
        [Then("I should see the Payments list")]
        public async Task IShouldSeeThePaymentsList()
            => await _paymentsPage.VerifyPaymentsListVisibleAsync();

        [Given("I enter the Payment number {string} in the payments section")]
        [When("I enter the Payment number {string} in the payments section")]
        [Then("I enter the Payment number {string} in the payments section")]
        public async Task IEnterThePaymentNumber(string number)
            => await _paymentsPage.EnterPaymentNumberAsync(number);

        // Scoped to Payments — generic click without dialog-open wait
        [Given("I click on {string} button"), Scope(Feature = "Payments")]
        [When("I click on {string} button"), Scope(Feature = "Payments")]
        [Then("I click on {string} button"), Scope(Feature = "Payments")]
        public async Task IClickOnButtonPayments(string buttonText)
            => await _paymentsPage.ClickButtonAsync(buttonText);

        [Given("I should see the payment with number {string} in the List in the Available payments section")]
        [When("I should see the payment with number {string} in the List in the Available payments section")]
        [Then("I should see the payment with number {string} in the List in the Available payments section")]
        public async Task IShouldSeeThePaymentInList(string number)
            => await _paymentsPage.VerifyPaymentVisibleInListAsync(number);

        [Given("I select the payment with number {string} in the List in the Available payments section")]
        [When("I select the payment with number {string} in the List in the Available payments section")]
        [Then("I select the payment with number {string} in the List in the Available payments section")]
        public async Task ISelectThePaymentFromList(string number)
            => await _paymentsPage.SelectPaymentFromListAsync(number);

        [Given("I should see the details of the payment with number {string}")]
        [When("I should see the details of the payment with number {string}")]
        [Then("I should see the details of the payment with number {string}")]
        public async Task IShouldSeeTheDetailsOfThePayment(string number)
            => await _paymentsPage.VerifyPaymentDetailsPageAsync(number);

        [Given("I should verify the Payment INFO")]
        [When("I should verify the Payment INFO")]
        [Then("I should verify the Payment INFO")]
        public async Task IShouldVerifyThePaymentInfo(Table dataTable)
        {
            var pairs = dataTable.Rows.Select(r => (label: r[0], value: r[1]));
            await _paymentsPage.VerifyPaymentDetailsAsync(pairs);
        }

        [Given("I should verify the Invoices section in the payment details page")]
        [When("I should verify the Invoices section in the payment details page")]
        [Then("I should verify the Invoices section in the payment details page")]
        public async Task IShouldVerifyTheInvoicesSection(Table dataTable)
        {
            var pairs = dataTable.Rows.Select(r => (label: r[0], value: r[1]));
            await _paymentsPage.VerifyInvoicesSectionAsync(pairs);
        }

        [Given("I should see the uploaded file {string}"), Scope(Feature = "Payments")]
        [When("I should see the uploaded file {string}"), Scope(Feature = "Payments")]
        [Then("I should see the uploaded file {string}"), Scope(Feature = "Payments")]
        public async Task IShouldSeeTheUploadedFile(string fileName)
            => await _paymentsPage.VerifyUploadedFileAsync(fileName);
    }
}
