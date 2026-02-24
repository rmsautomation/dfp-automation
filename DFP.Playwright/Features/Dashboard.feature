Feature: Dashboard
  As a user
  I want to access shipments from the dashboard
  So I can review shipment activity

  Scenario: Open shipments from dashboard
    Given I am on the dashboard page
    When I click on the "Shipments" option
    Then I should see the create shipments option
