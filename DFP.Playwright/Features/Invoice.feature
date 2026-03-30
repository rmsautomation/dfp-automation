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
    And I enter the invoice name "" in search field
    And I click on Search button
    Then the invoice should appear in the search results in the List with text "invoice"
    And I select the invoice in the search results with text "invoice"
    And the invoice details should be displayed with the name "TC1083_1087"
    And I should verify the following label headers in invoice details:
      # | Header                  | Value                    |
      | Invoice date    | 03/30/2026       |
      | Bill to         | automation       |
      | Payment terms   | Net 30           |
      | Due date        | 04/29/2026       |
      | Approval status | None             |
      | Notes           | Mar30105742Notes |
    #----------Verify Custom Fields-----------------
    And I should verify the following custom field values in invoice details in DFP:
      #| Header              | Value         |
      | Boolean Custom Field  | No          |
      | Date Custom Field     | 01/17/2025  |
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