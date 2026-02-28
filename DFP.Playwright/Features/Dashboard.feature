Feature: Dashboard
  As a user
  I want to access Transactions from the dashboard

  Scenario: Open shipments from dashboard
    Given I am on the dashboard page
    When I click on the "Shipments" option
    Then I should see the create shipments option

  @3072 @API
  Scenario: Verify Customs fields in Warehouse Receipt
    Given the transaction "WH" "TC3072" with Custom Fields is imported

  @API
  Scenario: Verify Customs fields in Shipment
    Given the transaction "SH" "TC5305" with Custom Fields is imported
    

  Scenario: Open Reports from dashboard
    Given I am on the dashboard page
    When I click on the "Reports" option
    Then I should see "Your reports" text

  Scenario: Open Dashboard 
    When I click on Dashboard icon
    Then the dashboard should be visible

  Scenario: Store the Total Shipments in the Dashboard
    Given I am on the dashboard page
    Then I store the total Shipments in the Dashboard