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
