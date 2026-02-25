Feature: Dashboard
  As a user
  I want to access shipments from the dashboard
  So I can review shipment activity

  Scenario: Open shipments from dashboard
    Given I am on the dashboard page
    When I click on the "Shipments" option
    Then I should see the create shipments option

  @3072
  Scenario: Verify Customs fields in Warehouse Receipts
    Given the Warehouse Receipt "TC_3072" with Custom Fields is imported

  @7873
  Scenario: Link Shipment with PO
    Given the Shipment "TC5305" with Custom Fields is imported
