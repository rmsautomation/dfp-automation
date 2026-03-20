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
