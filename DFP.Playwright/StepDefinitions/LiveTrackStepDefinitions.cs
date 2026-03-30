using Reqnroll;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class LiveTrackStepDefinitions
    {
        private readonly LiveTrackPage _liveTrackPage;

        public LiveTrackStepDefinitions(LiveTrackPage liveTrackPage)
        {
            _liveTrackPage = liveTrackPage;
        }

        [Given("I login to LiveTrack as user {string} networkID {string} and password {string}")]
        [When("I login to LiveTrack as user {string} networkID {string} and password {string}")]
        [Then("I login to LiveTrack as user {string} networkID {string} and password {string}")]
        public async Task ILoginToLiveTrackAsUser(string username, string networkId, string password)
            => await _liveTrackPage.LoginAsync(username, networkId, password);

        [Given("I go to Invoices in LiveTrack")]
        [When("I go to Invoices in LiveTrack")]
        [Then("I go to Invoices in LiveTrack")]
        public async Task IGoToInvoicesInLiveTrack()
            => await _liveTrackPage.GoToInvoicesAsync();

        [Given("I filter by number {string} in Livetrack")]
        [When("I filter by number {string} in Livetrack")]
        [Then("I filter by number {string} in Livetrack")]
        public async Task IFilterByNumberInLivetrack(string number)
            => await _liveTrackPage.FilterByNumberAsync(number);

        [Given("I click on OK button in Livetrack")]
        [When("I click on OK button in Livetrack")]
        [Then("I click on OK button in Livetrack")]
        public async Task IClickOnOkButtonInLivetrack()
            => await _liveTrackPage.ClickOkButtonAsync();

        [Given("I refresh the view in the LiveTrack")]
        [When("I refresh the view in the LiveTrack")]
        [Then("I refresh the view in the LiveTrack")]
        public async Task IRefreshTheViewInTheLiveTrack()
            => await _liveTrackPage.RefreshViewAsync();

        [Given("I approve the {string} invoice in LiveTrack with comment {string}")]
        [When("I approve the {string} invoice in LiveTrack with comment {string}")]
        [Then("I approve the {string} invoice in LiveTrack with comment {string}")]
        public async Task IApproveTheInvoiceInLiveTrackWithComment(string invoiceName, string comment)
            => await _liveTrackPage.ApproveInvoiceAsync(invoiceName, comment);
    }
}
