@HomeHub
Feature: HomeHub
  As a user
  I want to access to Home in the DFP Hub

@280 @login @NOINT
Scenario: Hub-Home_280
Given I login to Hub as user "aylin.rodriguez@magaya.com"
Then the login dashboard should be visible
And I should see the section header "User Approvals" in the Hub
And I should see the section header "Quotation Requests" in the Hub
And the "Quotation Requests" list should not be empty in the Hub
And I should see the section header "Recent Notifications" in the Hub
And the "Recent Notifications" list should not be empty in the Hub
And I should see the section header "Your Sites" in the Hub
And the "Your Sites" list should not be empty in the Hub
#-----Verify Link "View Quotations"-------
When I click on "View Quotations" button in the Hub
Then I should see the "quotations" page in the Hub
#-----Verify Link "View Shipments"-------
When I navigate to home in the Hub
And I click on "View Shipments" button in the Hub
Then I should see the "shipments" page in the Hub
#-----Verify Link "Manage Your Portal"-------
When I navigate to home in the Hub
And I click on "portal users" button in the Hub
Then I should see the "Portal Users" page in the Hub
#-----Verify Link "View Your Profile"-------
When I navigate to home in the Hub
And I click on "profile" button in the Hub
Then I should see the "About" page in the Hub
#-----Verify Link "View ..." in notifications section-------
When I navigate to home in the Hub
And I click on the first View button in Recent Notifications in the Hub
Then I should be redirected to a page with the Qwyk breadcrumb in the Hub
