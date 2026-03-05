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
