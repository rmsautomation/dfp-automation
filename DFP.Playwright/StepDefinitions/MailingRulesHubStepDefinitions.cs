using Reqnroll;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;
namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class MailingRulesHubStepDefinitions
    {
        private readonly MailingRulesHubPage _mailingRulesHubPage;
        private readonly DFP.Playwright.Support.TestContext _tc;

        public MailingRulesHubStepDefinitions(MailingRulesHubPage mailingRulesHubPage, DFP.Playwright.Support.TestContext tc)
        {
            _mailingRulesHubPage = mailingRulesHubPage;
            _tc = tc;
        }

        [When("I go to Admin Portal Notifications")]
        public async Task WhenIGoToAdminPortalNotifications()
            => await _mailingRulesHubPage.NavigateToNotificationsAsync();

        [Then("I should see the Create Mailing List button")]
        public async Task ThenIShouldSeeTheCreateMailingListButton()
            => await _mailingRulesHubPage.ShouldSeeCreateMailingListButtonAsync();

        [When("I search the mailing list by Name {string}")]
        public async Task WhenISearchTheMailingListByName(string name)
            => await _mailingRulesHubPage.SearchMailingListByNameAsync(name);

        [Then("I click on search button")]
        public async Task ThenIClickOnSearchButton()
            => await _mailingRulesHubPage.ClickSearchButtonAsync();

        [Then("I check if the list exists to delete it")]
        [When("I check if the list exists to delete it")]
        public async Task ThenICheckIfTheListExistsToDeleteIt()
            => await _mailingRulesHubPage.CheckIfListExistsToDeleteItAsync();

        [When("I select the created mailing list {string}")]
        public async Task WhenISelectTheCreatedMailingList(string name)
            => await _mailingRulesHubPage.SelectCreatedMailingListAsync(name);

        [Then("I should see the available members list")]
        public async Task ThenIShouldSeeTheAvailableMembersList()
            => await _mailingRulesHubPage.ShouldSeeAvailableMembersListAsync();

        [When("I enter the email {string} to add the member")]
        [Then("I enter the email {string} to add the member")]
        public async Task WhenIEnterTheEmailToAddTheMember(string email)
            => await _mailingRulesHubPage.EnterEmailToAddMemberAsync(email);

        [When("I add the member")]
        [Then("I add the member")]
        public async Task WhenIAddTheMember()
            => await _mailingRulesHubPage.AddMemberAsync();

        [When("I save the list")]
        [Then("I save the list")]
        public async Task WhenISaveTheList()
            => await _mailingRulesHubPage.SaveListAsync();

        [When("I click on Create Mailing List button")]
        public async Task WhenIClickOnCreateMailingListButton()
            => await _mailingRulesHubPage.ClickCreateMailingListButtonAsync();

        [Then("I enter the name of the mailing {string}")]
        public async Task ThenIEnterTheNameOfTheMailing(string name)
            => await _mailingRulesHubPage.EnterMailingNameAsync(name);

        [Then("I click on Create mailing List")]
        [When("I click on Create mailing List")]
        public async Task ThenIClickOnCreateMailingList()
            => await _mailingRulesHubPage.ClickCreateMailingListSaveButtonAsync();

        [When("I go to Mailing Rules")]
        public async Task WhenIGoToMailingRules()
            => await _mailingRulesHubPage.GoToMailingRulesAsync();

        [Then("I should see the Mailing Rules")]
        public async Task ThenIShouldSeeTheMailingRules()
            => await _mailingRulesHubPage.ShouldSeeMailingRulesAsync();

        [When("I search the mailing rule by Name {string}")]
        public async Task WhenISearchTheMailingRuleByName(string name)
            => await _mailingRulesHubPage.SearchMailingRuleByNameAsync(name);

        [Then("I check if the Rule exists to delete it")]
        [When("I check if the Rule exists to delete it")]
        public async Task ThenICheckIfTheRuleExistsToDeleteIt()
            => await _mailingRulesHubPage.CheckIfRuleExistsToDeleteItAsync();

        [When("I click on Create Rule button")]
        public async Task WhenIClickOnCreateRuleButton()
            => await _mailingRulesHubPage.ClickCreateRuleButtonAsync();

        [Then("I should see the view to create the Rule")]
        public async Task ThenIShouldSeeTheViewToCreateTheRule()
            => await _mailingRulesHubPage.ShouldSeeCreateRuleViewAsync();

        [Then("I enter the Mailing Rule Name {string}")]
        [When("I enter the Mailing Rule Name {string}")]
        public async Task ThenIEnterTheMailingRuleName(string name)
            => await _mailingRulesHubPage.EnterMailingRuleNameAsync(name);

        [Then("I select the Notification Type {string}")]
        [When("I select the Notification Type {string}")]
        public async Task ThenISelectTheNotificationType(string type)
            => await _mailingRulesHubPage.SelectNotificationTypeAsync(type);

        [When("I click on Create Mailing Rule")]
        public async Task WhenIClickOnCreateMailingRule()
            => await _mailingRulesHubPage.ClickCreateMailingRuleButtonAsync();

        [Then("I should select the Mailing List {string}")]
        public async Task ThenIShouldSelectTheMailingList(string mailingListName)
            => await _mailingRulesHubPage.SelectMailingListAsync(mailingListName);

        [Then("I should see the Mailing List {string} in the Recipients tab")]
        public async Task ThenIShouldSeeTheMailingListInTheRecipientsTab(string mailingListName)
            => await _mailingRulesHubPage.ShouldSeeMailingListInRecipientsAsync(mailingListName);

        [When("I select the Hub User {string}")]
        public async Task WhenISelectTheHubUser(string userName)
            => await _mailingRulesHubPage.SelectHubUserAsync(userName);

        [Then("I should see the Name {string} in the Hub Users Recipients")]
        public async Task ThenIShouldSeeTheNameInTheHubUsersRecipients(string userName)
            => await _mailingRulesHubPage.ShouldSeeUserInHubUsersRecipientsAsync(userName);

        [When("I go to Customers tab")]
        public async Task WhenIGoToCustomersTab()
            => await _mailingRulesHubPage.GoToCustomersTabAsync();

        [Then("I select the Customer {string}")]
        public async Task ThenISelectTheCustomer(string customerName)
            => await _mailingRulesHubPage.SelectCustomerAsync(customerName);

        [When("I click on save Mailing Rule button")]
        [Then("I click on save Mailing Rule button")]
        public async Task WhenIClickOnSaveMailingRuleButton()
            => await _mailingRulesHubPage.ClickSaveButtonAsync();

        [Then("I should see the last notifications")]
        public async Task ThenIShouldSeeTheLastNotifications()
            => await _mailingRulesHubPage.ShouldSeeLastNotificationsAsync();

        [Then("I should see the notification related to the created Shipment {string}")]
        public async Task ThenIShouldSeeTheNotificationRelatedToTheCreatedShipment(string notificationText)
            => await _mailingRulesHubPage.ShouldSeeNotificationForShipmentAsync(notificationText);

        [When("I click on View shipment button")]
        [Then("I click on View shipment button")]
        public async Task WhenIClickOnViewShipmentButton()
            => await _mailingRulesHubPage.ClickViewShipmentButtonAsync();

        [Then("I should see the shipment details")]
        public async Task ThenIShouldSeeTheShipmentDetails()
        {
            var shipmentId = _tc.Data.TryGetValue("shipmentId", out var id) && id is string sid ? sid : "";
            var shipmentName = _tc.Data.TryGetValue("shipmentName", out var nm) && nm is string sn ? sn : "";
            await _mailingRulesHubPage.ShouldSeeShipmentDetailsAsync(shipmentId, shipmentName);
        }
    }
}
