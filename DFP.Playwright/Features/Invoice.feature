@Invoices
Feature: Invoices
    As a user
    I want to access to Invoices in the Portal and verify that the information is correct

  @1083 @1087 @INT @login
  Scenario: 36_1083_39_1087MagayaToDFP_InvoiceMagayaAttchment
    #----------MAGAYA STEPS------------------------------------------------
    Given the transaction "IN" "TC1083_1087" is imported via API
    # Invoice TC1083_1087 was created in Magaya with an attachment
    #----------PORTAL STEPS------------------------------------------------
    Given I login to Portal as user "with Int"
    Then the login dashboard should be visible
    # ── Verify the Invoice is displayed in Invoice List with correct information ─────────────
    # ── Verify the Invoice attachment is displayed in Invoice Details and the content is correct
    Given I navigated to Invoices List
    Given I set the invoice name to "TC1083_1087"
    And I enter the invoice name "TC1083_1087" in search field
    And I click on Search button
    Then the invoice should appear in the search results in the List with text "invoice"
    And I select the invoice in the search results with text "invoice"
    And the invoice details should be displayed with the name "TC1083_1087"
    And I should verify the following label headers in invoice details:
      # | Header                  | Value                    |
      | Bill to         | automation       |
      | Payment terms   | Net 30           |
      | Approval status | None             |
      | Notes           | Mar30105742Notes |
    #----------Verify Custom Fields-----------------
    And I should verify the following custom field values in invoice details in DFP:
      #| Header              | Value         |
      | Boolean Custom Field  | No          |
      | Decimal Custom Field  | 50.5        |
      | Integer Custom Field  | 50          |
      | Money Custom Field    | USD 100     |
      | PickList Custom Field | Option1     |
      | String Custom Field   | String Test |
    #------Verify Event in Invoice details page---------
    Then I should see the event "In Transit"
    #------Verify charge in Invoice details page---------
    When I go to charges invoice tab in the invoice details page
    And I should see the amount "$100.00" for the charge "Documentation"
    #------Verify attachment in Invoice details page---------
    When I go to attachments tab
    And I select the pagination number "25"
    And I should see the uploaded file "Testing Attachments.docx"
    And I should see the uploaded file "test.jpg"
    And I should see the uploaded file "test1.png"
    And I should see the uploaded file "test2.pdf"

  @1084 @1086 @INT @login @LiveTrack
  Scenario: 37_1084_38_1086MagayaToDFP_UpdateInvoiceEventAut
    #----------MAGAYA STEPS------------------------------------------------
    Given the transaction "IN" "TC1084_1086" is imported via API
    # Invoice TC1084_1086 was created in Magaya without events and then the event "In Transit" was added later in Magaya
    #----------PORTAL STEPS------------------------------------------------
    Given I login to Portal as user "with Int"
    Then the login dashboard should be visible
    # ── Verify the Invoice is displayed in Invoice List with correct information ─────────────
    # ── Verify the Invoice event is updated in Invoice Details after being added in Magaya ─────────────
    Given I navigated to Invoices List
    Given I set the invoice name to "TC1084_1086"
    And I enter the invoice name "" in search field
    And I click on Search button
    Then the invoice should appear in the search results in the List with text "invoice"
    And I select the invoice in the search results with text "invoice"
    And the invoice details should be displayed with the name "TC1084_1086"
    And I should verify the following label headers in invoice details:
      # | Header                  | Value                    |
      | Bill to         | automation       |
      | Payment terms   | Net 30           |
      | Approval status | None             |
      | Notes           | Mar30144433Notes |
    #----------Verify Custom Fields-----------------
    And I should verify the following custom field values in invoice details in DFP:
      #| Header              | Value         |
      | Boolean Custom Field  | No          |
      | Decimal Custom Field  | 50.5        |
      | Integer Custom Field  | 50          |
      | Money Custom Field    | USD 100     |
      | PickList Custom Field | Option1     |
      | String Custom Field   | String Test |
    #------Verify Event in Invoice details page---------
    Then I should see the event "In Transit"
    #------Verify charge in Invoice details page---------
    When I go to charges invoice tab in the invoice details page
    And I should see the amount "$100.00" for the charge "Documentation"
    #------Verify attachment in Invoice details page---------
    When I go to attachments tab
    And I select the pagination number "25"
    And I should see the uploaded file "Testing Attachments.docx"
    And I should see the uploaded file "test.jpg"
    And I should see the uploaded file "test1.png"
    And I should see the uploaded file "test2.pdf"
    #------LiveTrack Steps to APPROVE INVOICE---------
    Given I login to LiveTrack as user "automation" networkID "38442" and password ""
    When I go to Invoices in LiveTrack
    #-------Steps in LiveTrack to approve the invoice---------
    Then I filter by number "TC1084_1086" in Livetrack
    Then I click on OK button in Livetrack
    Then I approve the "automation" invoice in LiveTrack with comment "Approved in LiveTrack"
    #-------Magaya Steps UPDATE INVOICE--------------
    # Update Invoice TC1084_1086 in Magaya
    #---------PORTAL STEPS Verify UPDATED AND APPROVED INVOICE----------------
    #Given I login to Portal as user "with Int"
    #Then the login dashboard should be visible
    Given I navigated to Invoices List
    Given I set the invoice name to "TC1084_1086UPDATED"
    And I enter the invoice name "" in search field
    And I click on Search button
    Then the invoice should appear in the search results in the List with text "invoice"
    And I select the invoice in the search results with text "invoice"
    And the invoice details should be displayed with the name "TC1084_1086UPDATED"
    And I should verify the following label headers in invoice details:
      # | Header                  | Value                    |
      | Approval status | Approved                     |
      | Notes           | Mar30144433NotesNOTE UPDATED |
    #----------Verify UpdatedCustom Fields-----------------
    And I should verify the following custom field values in invoice details in DFP:
      #| Header              | Value         |
      | Boolean Custom Field  | Yes           |
      | Decimal Custom Field  | 150.5         |
      | Integer Custom Field  | 150           |
      | Money Custom Field    | USD 500       |
      | PickList Custom Field | Option2       |
      | String Custom Field   | StringUpdated |
    #------Verify Updated Event in Invoice details page---------
    Then I should see the event "Transaction Approved by Customer"
    Then I should see the event "Arrived at destination"
    #------Verify Updated charge in Invoice details page---------
    When I go to charges invoice tab in the invoice details page
    And I should see the amount "$10.00" for the charge "Security Surcharge"
    #------Verify Updated attachment in Invoice details page---------
    When I go to attachments tab
    And I select the pagination number "25"
    And I should see the uploaded file "RoundPriceUpdated.xlsx"

  @1089 @INT @login
  Scenario:40_1089_DFP_AddAttachments_VerifyMagaya
    #----------MAGAYA STEPS------------------------------------------------
    #Given the transaction "IN" "TC1089" is imported via API
    #----------PORTAL STEPS------------------------------------------------
    Given I login to Portal as user "with Int"
    Then the login dashboard should be visible
    # ── Verify the Invoice is displayed in Invoice List with correct information ─────────────
    Given I navigated to Invoices List
    Given I set the invoice name to "TC1089"
    And I enter the invoice name "TC1089" in search field
    And I click on Search button
    Then the invoice should appear in the search results in the List with text "invoice"
    And I select the invoice in the search results with text "invoice"
    And the invoice details should be displayed with the name "TC1089"
    #------Adding attachments in DFP for Invoice TC1089 and verifying the attachments are displayed in DFP and Magaya---------
    When I go to attachments tab
    And I click on Attahments tab
    #----Upload test.jpg--------------------------
    When I click on "Upload attachment" button
    Then I should see the screen to upload the attachment
    Then I click on Drop your file here option
    When I select the file to upload "test.jpg"
    Then I enter the description "DFPAttach" for the attachment
    Then I click on Upload button
    And I should see the uploaded file "test.jpg"
    #----Upload RoundPriceUpdated--------------------------
    When I click on "Upload attachment" button
    Then I should see the screen to upload the attachment
    Then I click on Drop your file here option
    When I select the file to upload "RoundPriceUpdated.xlsx"
    Then I enter the description "DFPAttach" for the attachment
    Then I click on Upload button
    And I should see the uploaded file "RoundPriceUpdated.xlsx"
    #----Upload test1.png--------------------------
    When I click on "Upload attachment" button
    Then I should see the screen to upload the attachment
    Then I click on Drop your file here option
    When I select the file to upload "test1.png"
    Then I enter the description "DFPAttach" for the attachment
    Then I click on Upload button
    And I should see the uploaded file "test1.png"
    #----Upload test2.pdf--------------------------
    When I click on "Upload attachment" button
    Then I should see the screen to upload the attachment
    Then I click on Drop your file here option
    When I select the file to upload "test2.pdf"
    Then I enter the description "DFPAttach" for the attachment
    Then I click on Upload button
    And I should see the uploaded file "test2.pdf"
    #----Upload Testing Attachments.docx--------------------------
    When I click on "Upload attachment" button
    Then I should see the screen to upload the attachment
    Then I click on Drop your file here option
    When I select the file to upload "Testing Attachments.docx"
    Then I enter the description "DFPAttach" for the attachment
    Then I click on Upload button 
    And I should see the uploaded file "Testing Attachments.docx"
    #------MAGAYA STEPS to verify the attachments added in DFP are displayed in Magaya---------
    # Verify in Magaya that the attachment added in DFP is displayed in Magaya for Invoice TC1089
