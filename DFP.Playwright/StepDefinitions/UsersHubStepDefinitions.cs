using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class UsersHubStepDefinitions(UsersHubPage usersHubPage)
    {
        private readonly UsersHubPage _usersHubPage = usersHubPage;

        ///////////////User Permissions HUB ///////////////////////////////////////////

        [When("I go to Portal Users")]
        public async Task IGoToPortalUsers()
        {
            await _usersHubPage.IGoToPortalUsers();
        }

        [Then("the Portal Users page should be displayed")]
        public async Task ThePortalUsersPageShouldBeDisplayed()
        {
            await _usersHubPage.ThePortalUsersPageShouldBeDisplayed();
        }

        [When(@"I search the User by email (.+)")]
        public async Task ISearchTheUserByEmail(string email)
        {
            await _usersHubPage.ISearchTheUserByEmail(email);
        }

        [When("I click on search button")]
        public async Task IClickOnSearchButton()
        {
            await _usersHubPage.IClickOnSearchButton();
        }

        [Then("I should see the user in the results")]
        public async Task IShouldSeeTheUserInTheResults()
        {
            await _usersHubPage.IShouldSeeTheUserInTheResults();
        }

        [When("I click on the Customer Name in the User Page")]
        public async Task IClickOnTheCustomerNameInTheUserPage()
        {
            await _usersHubPage.IClickOnTheCustomerNameInTheUserPage();
        }

        [Then("I should see the User Details page")]
        public async Task IShouldSeeTheUserDetailsPage()
        {
            await _usersHubPage.IShouldSeeTheUserDetailsPage();
        }

        [When("I click on the Permissions dropdwon")]
        public async Task IClickOnThePermissionsDropdown()
        {
            await _usersHubPage.IClickOnThePermissionsDropdown();
        }

        [When("I enter the permission in the  search section")]
        public async Task IEnterThePermissionInTheSearchSection()
        {
            await _usersHubPage.IEnterThePermissionInTheSearchSection("View Shipments");
        }

        [When("I unchecked the {string} permission")]
        public async Task IUncheckedThePermission(string _)
        {
            await _usersHubPage.IUncheckedThePermission();
        }

        [When("I click on Save User button")]
        public async Task IClickOnSaveUserButton()
        {
            await _usersHubPage.IClickOnSaveUserButton();
        }
    }
}
