Feature: Dashboard
  As a user
  I want to access Transactions from the dashboard

  @3072 @API
  Scenario: Verify Customs fields in Warehouse Receipt
    Given the transaction "WH" "TC3072" with Custom Fields is imported

  @API
  Scenario: Verify Customs fields in Shipment
    Given the transaction "SH" "TC5305" with Custom Fields is imported
  