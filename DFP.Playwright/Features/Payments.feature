@Payments
Feature: Payments
    As a user
    I want to access to Payments in the Portal and verify that the information is correct

  @1790 @INT @login
  Scenario: 1790_MagayaQWYK_CreatePayment
    Given the transaction "IN" "TC1790" is imported via API
    Given the transaction "PM" "TC1790" is imported via API
    Given I login to Portal as user "with Int"
    Then the login dashboard should be visible
    Given I am on the Payments page
    Then I should see the Payments list
    Then I enter the Payment number "TC1790" in the payments section
    Then I click on "Search" button
    Then I should see the payment with number "TC1790" in the List in the Available payments section
    Then I select the payment with number "TC1790" in the List in the Available payments section
    Then I should see the details of the payment with number "TC1790"
    Then I should verify the Payment INFO
      | Total amount         | $100.00       |
      | Reference            | TC1790        |
      | Payer                | automation    |
      | Cusotm Field Integer | 50            |
      | Custom Field Text    | QA Automation |
    Then I should verify the Invoices section in the payment details page
      | Invoice number | TC1790  |
      | Amount         | $100.00 |
    When I go to attachments tab
    Then I should see the uploaded file "test.jpg"

  @1792 @INT @login
  Scenario: 1792_MagayaQWYKUpdatePayment
    Given the transaction "IN" "TC1792" is imported via API
    Given the transaction "PM" "TC1792" is imported via API
    Given I login to Portal as user "with Int"
    Then the login dashboard should be visible
    Given I am on the Payments page
    Then I should see the Payments list
    Then I enter the Payment number "TC1792" in the payments section
    Then I click on "Search" button
    Then I should see the payment with number "TC1792" in the List in the Available payments section
    Then I select the payment with number "TC1792" in the List in the Available payments section
    Then I should see the details of the payment with number "TC1792"
    Then I should verify the Payment INFO
      | Total amount         | $100.00       |
      | Reference            | TC1792        |
      | Payer                | automation    |
      | Cusotm Field Integer | 50            |
      | Custom Field Text    | QA Automation |
    Then I should verify the Invoices section in the payment details page
      | Invoice number | TC1792  |
      | Amount         | $100.00 |
    When I go to attachments tab
    Then I should see the uploaded file "test.jpg"
    #----------MAGAYA STEPS---------------------
    #UPDATE PAYMENT TC1792
    #-----------Verify Updates in DFP----------------------
    Given I am on the Payments page
    Then I should see the Payments list
    Then I enter the Payment number "TC1792UPDATED" in the payments section
    Then I click on "Search" button
    Then I should see the payment with number "TC1792UPDATED" in the List in the Available payments section
    Then I select the payment with number "TC1792UPDATED" in the List in the Available payments section
    Then I should see the details of the payment with number "TC1792UPDATED"
    Then I should verify the Payment INFO
      | Total amount         | $200.00                           |
      | Credit amount        | $100.00                           |
      | Reference            | TC1792UPDATED                     |
      | Payer                | automation                        |
      | Notes                | Payment created in Magaya-Updated |
      | Cusotm Field Integer | 50                                |
      | Custom Field Text    | QA Automation                     |
    Then I should verify the Invoices section in the payment details page
      | Invoice number | TC1792  |
      | Amount         | $100.00 |
    #--------Verify All Type attachmments in DFP---------------
    When I go to attachments tab
    And I select the pagination number "25"
    Then I should see the uploaded file "RoundPriceUpdated.xlsx"
    And I should see the uploaded file "DOCDFP.docx"
    And I should see the uploaded file "PDFDFP.pdf"
    And I should see the uploaded file "XLSDFP.xlsx"
    And I should see the uploaded file "CSVDFP.csv"
    And I should see the uploaded file "TXT_MAGAYA.txt"
    And I should see the uploaded file "MSGDFP.msg"
    And I should see the uploaded file "XML_MAGAYA.xml"
    And I should see the uploaded file "test1.png"
    And I should see the uploaded file "JSON_MAGAYA.json"
