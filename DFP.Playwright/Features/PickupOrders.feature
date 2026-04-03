@Pickup
Feature: Pickup Orders
    As a user
    I want to view and manage pickup orders

 @searchPickUp
 Scenario: Search Pickup Order
    Given I am on the Pickup Orders page
    Then I should see the Pickup Orders list
    Then I enter the Pickup Order number "TC2058" in the Pickup Orders section
    Then I click on "Search" button
    Then I should see the Pickup Order with number "TC2058" in the List in the Available Pickup Orders section