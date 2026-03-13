Feature: Dashboard
  As a user
  I want to access Transactions from the dashboard


  @API @INT
  Scenario: Verify Customs fields in Shipment
    Given the transaction "SH" "TC5305" with Custom Fields is imported
  