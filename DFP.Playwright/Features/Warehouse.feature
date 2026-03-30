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
      | StringCustomField   | STRINGCUSTOMFIELD3072 |
      | BooleanCustomField  | No                    |
      | IntegerCustomField  | 3072                  |
      | DecimalCustomField  | 30.72                 |
      | PickListCustomField | Option1               |
      | DateCustomField     | 02/24/2026            |
      | MoneyCustomField    | USD 30.72             |
      | LookupCutomsField   | automationUpdated     |
      | QATest              | QATEST3072            |
      | GUIDWH              | GUID                  |

  @923 @3373 @API @INT @login
  Scenario:923_3373MagayaToQWYK_WarehouseReceipt
    #-----------Magaya Steps--------------------------------------
    Given the transaction "WH" "TC923_3373" is imported via API
    Given I login to Portal as user "with Int"
    # ── Verify the WR is displayed in Warehouse Receipt List ─────────────
    Given I navigated to Warehouse Receipts List
    Given I set the warehouse receipt name to "TC923_3373"
    And I enter the warehouse receipt name in search field
    And I click on Search button
    Then the warehouse receipt should appear in the search results in the List with text "automation"
    And I select the warehouse receipt in the search results with text "automation"
    And the warehouse receipt details should be displayed with the name "TC923_3373"
    And I should verify label header "Number" contains "TC923_3373"
    And I should verify the following label headers in warehouse receipt details:
    # | Header                  | Value                    |
      | Number                  | TC923_3373               |
      | Carrier PRO Number      | 1485PONumber             |
      | Carrier Tracking Number | 1485TrackingNumber       |
      | Driver                  | 1485DriverName           |
      | License                 | 1485LicenseNumber        |
      | Supplier                | automation               |
      | PO Number               | 1485PONumber             |
      | PO Invoice Number       | 1485INVNumber            |
      | Billing Client          | automation               |
      | Note                    | 1485WR created in Magaya |
    #----------Verify Custom Fields-----------------
    And I should verify custom fields label header "BooleanCustomField" contains "Yes"
    And I should verify the following custom field values in warehouse receipt details in DFP:
      #| Header              | Value         |
      | BooleanCustomField  | Yes               |
      | DateCustomField     | 01/25/2025        |
      | DecimalCustomField  | 0.5               |
      | IntegerCustomField  | 10                |
      | MoneyCustomField    | USD 50            |
      | PickListCustomField | Option1           |
      | StringCustomField   | StringCustom      |
      | GUIDWH              | GUID              |
      | LookupCutomsField   | automation        |
      | QATest              | QAAutomationTests |
    #------Verify Parties in WH details page---------
    And I should verify the following parties in warehouse receipt details:
      #| Party Type | Party Name  |
      | Billing Client    | automation       |
      | Carrier           | MSC              |
      | Consignee         | automation       |
      | Destination Agent | AgentDestination |
      | Issued By         | Postgress SQL II |
      | Shipper           | automation       |
      | Supplier          | automation       |
    #------Verofy Event in WH details page---------
    When I go to tracking tab
    Then I should see the event "Arrived at warehouse"
    #------Verify Commodity in WH details page---------
    When I go to cargo tab
    Then I should see the cargo items page
    And I should see the commodity "UpdateCommodity" in cargo details warehouse
    #------Verify charge in WH details page---------
    When I go to charge and invoice tab
    And I should see the amount "$150.00" for the charge "Cartage Fee"
    #------Verify attachment in WH details page---------
    When I go to attachments tab
    And I select the pagination number "50"
    And I should see the uploaded file "test.jpg"
    And I should see the uploaded file "test2.pdf"
    #------Verify ALL the uploaded files in attachments tab---------
    And I should see the uploaded file "DOCDFP.docx"
    And I should see the uploaded file "MSGDFP.msg"
    And I should see the uploaded file "test1.png"
    And I should see the uploaded file "CSVDFP.csv"
    And I should see the uploaded file "test.jpg"
    And I should see the uploaded file "TXT_MAGAYA.txt"
    And I should see the uploaded file "XML_MAGAYA.xml"
    And I should see the uploaded file "JSON_MAGAYA.json"
    And I should see the uploaded file "XLSDFP.xlsx"
    And I should see the uploaded file "PDFDFP.pdf"

@924 @API @INT @login
  Scenario:924_MagayaToQWYK_UpdateWR
  #-----------Magaya Steps--------------------------------------
    Given the transaction "WH" "TC924" is imported via API
    Given I login to Portal as user "with Int"
    # ── Verify the WR is displayed in Warehouse Receipt List ─────────────
    Given I navigated to Warehouse Receipts List
    Given I set the warehouse receipt name to "TC924"
    And I enter the warehouse receipt name in search field
    And I click on Search button
    Then the warehouse receipt should appear in the search results in the List with text "automation"
    And I select the warehouse receipt in the search results with text "automation"
    And the warehouse receipt details should be displayed with the name "TC924"
    And I should verify label header "Number" contains "TC924"
    And I should verify the following label headers in warehouse receipt details:
    # | Header                  | Value                    |
      | Number                  | TC924                    |
      | Carrier PRO Number      | 1487PONumber             |
      | Carrier Tracking Number | 1487TrackingNumber       |
      | Driver                  | 1487DriverName           |
      | License                 | 1487LicenseNumber        |
      | Supplier                | automation               |
      | PO Number               | 1487PONumber             |
      | PO Invoice Number       | 1487INVNumber            |
      | Billing Client          | automation               |
      | Note                    | 1487WR created in Magaya |
    #----------Verify Custom Fields-----------------
    And I should verify custom fields label header "BooleanCustomField" contains "Yes"
    And I should verify the following custom field values in warehouse receipt details in DFP:
      #| Header              | Value         |
      | BooleanCustomField  | Yes               |
      | DateCustomField     | 01/25/2025        |
      | DecimalCustomField  | 0.5               |
      | IntegerCustomField  | 10                |
      | MoneyCustomField    | USD 50            |
      | PickListCustomField | Option1           |
      | StringCustomField   | StringCustom      |
      | GUIDWH              | GUID              |
      | LookupCutomsField   | automation        |
      | QATest              | QAAutomationTests |
    #------Verify Parties in WH details page---------
    And I should verify the following parties in warehouse receipt details:
      #| Party Type | Party Name  |
      | Billing Client    | automation       |
      | Carrier           | MSC              |
      | Consignee         | automation       |
      | Destination Agent | AgentDestination |
      | Issued By         | Postgress SQL II |
      | Shipper           | automation       |
      | Supplier          | automation       |
    #------Verify Event in WH details page---------
    When I go to tracking tab
    Then I should see the event "Arrived at warehouse"
    #------Verify Commodity in WH details page---------
    When I go to cargo tab
    Then I should see the cargo items page
    And I should see the commodity "UpdateCommodity" in cargo details warehouse
    #------Verify charge in WH details page---------
    When I go to charge and invoice tab
    And I should see the amount "$150.00" for the charge "Cartage Fee"
    #------Verify attachment in WH details page---------
    When I go to attachments tab
    And I select the pagination number "50"
    And I should see the uploaded file "test.jpg"
    #------Verify attachment AT COMMODITY LEVEL in WH details page---------
    And I should see the uploaded file "test2.pdf"
#----------MAGAYA STEPS------------------
# ──-------- Update the WR in Magaya ─────────────
#----------XML for updating the WR in Magaya----------------
#CHECK WITH ANNIA SAME GUID ????
  #Given the XML file "TC924updated.xml" is imported via API
#----------Verigfy. update in DFP after updating the WR in Magaya----------------
    Given I navigated to Warehouse Receipts List
    Given I set the warehouse receipt name to "TC924UPDATED"
    And I enter the warehouse receipt name in search field
    And I click on Search button
    Then the warehouse receipt should appear in the search results in the List with text "automation"
    And I select the warehouse receipt in the search results with text "automation"
    And the warehouse receipt details should be displayed with the name "TC924UPDATED"
    And I should verify label header "Number" contains "TC924UPDATED"
    And I should verify the following label headers in warehouse receipt details:
    # | Header                  | Value                    |
      | Number                  | TC924UPDATED             |
      | Carrier PRO Number      | 1487PONumberUpdated             |
      | Carrier Tracking Number | 1487TrackingNumberUpdated       |
      | Driver                  | 1487DriverNameUpdated          |
      | License                 | 1487LicenseNumberUpdated        |
      | Supplier                | automation               |
      | PO Number               | 1487PONumberUpdated            |
      | PO Invoice Number       | 1487INVNumber            |
      | Billing Client          | automation               |
      | Note                    | NOTE UPDATED |
    #----------Verify Custom Fields-----------------
    And I should verify custom fields label header "BooleanCustomField" contains "Yes"
    And I should verify the following custom field values in warehouse receipt details in DFP:
      #| Header              | Value         |
      | BooleanCustomField  | Yes               |
      | DateCustomField     | 01/25/2025        |
      | DecimalCustomField  | 600              |
      | IntegerCustomField  | 400                |
      | MoneyCustomField    | USD 700            |
      | PickListCustomField | Option2           |
      | StringCustomField   | ShipperRefUpdated      |
      | GUIDWH              | GUIDUpdated              |
      | LookupCutomsField   | automation        |
      | QATest              | QAAutomationTestsUpdated |
    #------Verify Parties in WH details page---------
    And I should verify the following parties in warehouse receipt details:
      #| Party Type | Party Name  |
      | Billing Client    | automation       |
      | Carrier           | CMA              |
      | Consignee         | AgentDestination       |
      | Destination Agent | AgentDestination |
      | Issued By         | Postgress SQL II |
      | Shipper           | automation       |
      | Supplier          | automation       |
    #------Verify Event in WH details page---------
    When I go to tracking tab
    Then I should see the event "Arrived at destination"
    #------Verify Commodity in WH details page---------
    When I go to cargo tab
    Then I should see the cargo items page
    And I should see the commodity "UpdateCommodity" in cargo details warehouse
    And I should see the total pieces "503 pieces" in cargo details warehouse
    #------Verify charge in WH details page---------
    When I go to charge and invoice tab
    And I should see the amount "$200.00" for the charge "Storage Fee"
    #------Verify attachment in WH details page---------
    When I go to attachments tab
    And I select the pagination number "50"
    And I should see the uploaded file "RoundPriceUpdated.xlsx"
