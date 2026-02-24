Feature: Dashboard
  As a user
  I want to access shipments from the dashboard
  So I can review shipment activity

  Background:
    Given a Warehouse Receipt with Custom Fields exists

  Scenario: Open shipments from dashboard
    Given I am on the dashboard page
    When I click on the "Shipments" option
    Then I should see the create shipments option

  Scenario: Verify Customs fields in Warehouse Receipts
    Given Login into the Portal
    When Go to Warehouse / Warehouse Receipts
    And Select Table View
    Then Verify the Custom fields
