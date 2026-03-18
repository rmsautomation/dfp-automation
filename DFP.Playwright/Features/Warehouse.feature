@WarehouseReceipts
Feature: Warehouse Receipts
  As a user 
  I want to verify that Warehouse Receipts with Exclude from Tracking enabled in Magaya
  are not visible in the DFP Portal


@3907 @INT @login
Scenario: Magaya to DFP - Warehouse Receipt - Exclude from Tracking = True
  # WR TC3907 was created in Magaya with Exclude from Tracking = True
  Given I login to Portal as user "with Int"
  # ── Verify the WR is not displayed in Warehouse Receipt List ─────────────
  Given I navigated to Warehouse Receipts List
  Given I set the warehouse receipt name to "TC3907"
  And I enter the warehouse receipt name in search field
  And I click on Search button
  Then the warehouse receipt should not appear in the search results
  # ── Verify the WR is not displayed in Cargo Detail Page ──────────────────
  Given I navigated to Cargo Detail Page
  Then I enter the warehouse receipt name in search field in Cargo Detail
  And I click on Search button
  Then the warehouse receipt should not be displayed in Cargo Detail
  # ── Verify the WR is not displayed in Reports > Warehouse Receipts ────────
  When I go to Reports Warehouse
  Then I should see "Generate Warehouse Receipts" Report text
  And I click on Search button
  When I see the Save report button
  Then the warehouse receipt name should not appear in the report results

@5439 @login @INT
Scenario: User edits a Table View and verifies selected columns
    Given I login to Portal as user "with Int"
    Given I navigated to Warehouse Receipts List
    When I click on Table View in WH Receipt List
    And I select a view to edit
    And I click on Configuration button
    And I click on Columns tab
    And I enter the column Name in the field
    And I select the column Name
    And I close the Customize View
    Then I should see the selected columns in the Table View  

@3072 @API @INT @login
  Scenario: Verify Customs fields in Warehouse Receipt
  Given the transaction "WH" "TC3072" is imported via API
  Given I login to Portal as user "with Int"
  # ── Verify the WR Custom Fields are displayed in Warehouse Receipt List ─────────────
  Given I navigated to Warehouse Receipts List
  Given I set the warehouse receipt name to ""
  And I enter the warehouse receipt name in search field
  When I click on Table View in WH Receipt List
  And I click on Search button
  Then the warehouse receipt should appear in the search results
  And I check the custom field "StringCustomField"
  And I check the following custom field values in the table view:
  #Example
  # | Column            | Value             |
    | StringCustomField | STRINGCUSTOMFIELD3072 |
    | BooleanCustomField | No |
    | IntegerCustomField | 3072 |
    | DecimalCustomField | 30.72 |
    | PickListCustomField | Option1 |
    | DateCustomField | 02/24/2026 |
    | MoneyCustomField | USD 30.72 |
    | LookupCutomsField | automationUpdated |
    | QATest | QATEST3072 |
    | GUIDWH | GUID |
  
