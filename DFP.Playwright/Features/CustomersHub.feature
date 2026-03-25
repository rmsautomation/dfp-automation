@CustomersHub
Feature: CustomersHub
  As a user
  I want to access to Customers in the DFP Hub

@1482 @login @NOINT
Scenario: Hub-Create Customer_1482
Given I login to Hub as user "aylin.rodriguez@magaya.com"
Then the login dashboard should be visible
When I go to Portal Customers in the Hub
Then I should see the section header "Portal Customers" in the Hub
When I click on Create customer button
Then I should see the section header "Portal Customer" in the Hub
And I enter the customer name "" in the Hub
And I select the type "Company" in the Hub
And I select the segment "Customer" in the Hub
When I click on Create customer button
Then I should see the section header "Portal Customers" in the Hub
When I enter the customer name "" in the search section in the Hub
And I click on Search button in Customers Hub
Then I should see the customer name "" in the results

@1483 @login @NOINT
Scenario: HUB - Customer - Create user_1483
#------Create YOPMAIL user----------
When I go to yopmail URL
Then I store the now var
And I store the new contact email ""
And I create my yopmail email ""
Given I login to Hub as user "aylin.rodriguez@magaya.com"
Then the login dashboard should be visible
When I go to Portal Customers in the Hub
Then I should see the section header "Portal Customers" in the Hub
When I click on Create customer button
Then I should see the section header "Portal Customer" in the Hub
And I enter the customer name "" in the Hub
And I select the type "Company" in the Hub
And I select the segment "Customer" in the Hub
When I click on Create customer button
Then I should see the section header "Portal Customers" in the Hub
When I enter the customer name "" in the search section in the Hub
And I click on Search button in Customers Hub
Then I should see the customer name "" in the results
And I select the customer "" in the results in the Hub
And I should see the section header "Portal Customer" in the Hub
And I click on users tab in the Hub
When I click on create user button
Then I should see the section header "Portal User" in the Hub
#------Create User in the Hub--------
Then I enter the email for the portal user in the Hub
And I store the password for the portal user created in the Hub
And I select the site "Magaya QA"
And I enter the User Name ""
And I enter the company name "" in the Hub
And I confirm the privacy
And I click on create user button in the Hub
And I should see the section header "Portal User" in the Hub
# ── Check email --------
  When I Check the email for "" with username ""
Then I should receive an email with text "Your account has been created" in the body for shipment ""
And I store the portal user password from the email
And I click on Login to Magaya in the email
And I should see the login page
When I enter the created username "" in the Portal
And I enter the password "" in the Portal
And click on Sign in button
Then the login dashboard should be visible
