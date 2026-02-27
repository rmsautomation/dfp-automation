Feature: Dashboard
  As a user
  I want to access Transactions from the dashboard

  Scenario: Open shipments from dashboard
    Given I am on the dashboard page
    When I click on the "Shipments" option
    Then I should see the create shipments option

  @3072
  Scenario: Verify Customs fields in Warehouse Receipt
    Given the transaction "WH" "TC3072" with Custom Fields is imported

  
  Scenario: Verify Customs fields in Shipment
    Given the transaction "SH" "TC5305" with Custom Fields is imported
    