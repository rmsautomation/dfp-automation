Feature: Mailing Rules Hub

  @7831 @NOINT @login
  Scenario: Mailing Rules - New Shipment/All options - An email is sent to HUB Users, Mailing List users and Customer Related users
    Given I login to Hub as user "aylin.rodriguez@magaya.com"
    Then the login dashboard should be visible
    When I go to Admin Portal Notifications
    Then I should see the Create Mailing List button
    When I search the mailing list by Name "AutomationMailingList"
    Then I click on search button
    And I check if the list exists to delete it
    When I click on Create Mailing List button
    Then I enter the name of the mailing "AutomationMailingList"
    And I click on Create mailing List
    And I should see the Create Mailing List button
    When I select the created mailing list "AutomationMailingList"
    Then I should see the available members list
    #--------Adding Mail "automationhub@yopmail.com" to Mailing List-------------------
    And I enter the email "automationhub@yopmail.com" to add the member
    And I add the member
    When I save the list
    Then I should see the Create Mailing List button
    When I go to Mailing Rules
    Then I should see the Mailing Rules
    When I search the mailing rule by Name "AutomationMailingRule"
    Then I click on search button
    And I check if the Rule exists to delete it
    When I click on Create Rule button
    Then I should see the view to create the Rule
    And I enter the Mailing Rule Name "AutomationMailingRule"
    #--------Adding Rule "New Booking Created"------------------
    And I select the Notification Type "New Booking Created"
    When I click on Create Mailing Rule
    #--------Adding Mailing List to the Rule------------
    Then I should select the Mailing List "AutomationMailingList"
    And I should see the Mailing List "AutomationMailingList" in the Recipients tab
    #--------Adding HubUser to the Rule Aylin QuotationOP email aylinquotationop@yopmail.com------------
    When I select the Hub User "Aylin QuotationOP"
    Then I should see the Name "Aylin QuotationOP" in the Hub Users Recipients
    When I go to Customers tab
    #--------Adding AutomationOwner to the Rule email child_noint@yopmail.com------------
    Then I select the Customer "AutomationOwner"
    When I click on save Mailing Rule button
    Then I should see the Mailing Rules
    Given I log out from Hub
    #--------Create Shipment using ANOTHER USER AND CUSTOMER Customer1013----------
    Given I login to Portal as user "aylinportalinfra@yopmail.com"
    Then the login dashboard should be visible
    Given I am on the Quotations List page
    When I open the first quotation in Status Booked
    Then I should be on the Quotation Details page
    When I click the "Offers" button
    Then the list of the offers should appear
    When I click on Book Now button
    Then a confirmation dialog should appear
    When I confirm the shipment creation
    Then I should be on the Shipment Details page
    Then I store the shipment id from the URL
    When I click on Edit button to Edit the Shipment Name
    Then I should edit the Shipment Name
    And I store shipment Name
    When I click on save button
    Then I should see the new Shipment Name
    When I click on Send Booking button
    Then I should click on Go To Shipment button to see the shipment
    And the shipment should display the shipment name
    #--------EMAIL VERIFICATIONS Verify Email in the Mailing List automationhub@yopmail.com------------------------
    When I Check the email for "automationhub@yopmail.com" with username ""
    Then I should receive an email with text "A new shipment was booked.|Shipment:" in the body for shipment ""
    #--------EMAIL VERIFICATIONS Verify Email for Rule Hub User aylinquotationop@yopmail.com ------
    When I Check the email for "aylinquotationop@yopmail.com" with username ""
    Then I should receive an email with text "A new shipment was booked.|Shipment:" in the body for shipment ""
    #--------EMAIL VERIFICATIONS Verify Email for AutomationOwner  automationdfpowner@gmail.com ------
    #When I Check the email for "automationdfpowner@gmail.com" with username ""
      #Then I should receive an email with text "A new shipment was booked.|Shipment:" in the body for shipment ""
    #-------- Verify Hub Notifications for Rule Hub User aylinquotationop@yopmail.com ---
    Given I login to Hub as user "aylinquotationop@yopmail.com"
    Then the login dashboard should be visible
    And I should see the last notifications
    And I should see the notification related to the created Shipment "booked a new shipment"
    When I click on View shipment button
    Then I should see the shipment details