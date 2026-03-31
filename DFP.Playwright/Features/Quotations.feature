@Quotations
Feature: Quotations
  As a user
  I want to create a quotation in the DFP Portal

  @6526 @NOINT
  Scenario: Spot Rates - Create Quotation - MAERSK SPOT
  Given I am on the Quotations List page
  When I click on Create Quotation button
  Then I should see the create Quotation Page
  Then I click on "Ocean" transport mode
  And I click on "Full (FCL)" load type
  And I enter "Shanghai" as the Origin Port
  And I enter "Rotterdam" as the Destination Port
  When I click on Continue your quote
  Then I should see the Origin and Destination ports
  When I click on the calendar
  Then I select the date
  When I click on currency
  Then I select "USD" as the currency
  And I select the container size "40' Container"
  And I select the container type "All Types"
  When I click on Commodity dropdown
  Then I select the Commodity "Freight All Kinds (FAK)"
  When I click on Create quotation from details
  Then I should see the offers
  When I filter By "Maersk Line"
  When I select "Maersk Line" schedules
  Then I should see the schedules
  And I store the first Vessel
  And I select first the schedule
  Then I confirm the operation
  Then I should see the quotation details to send the booking
  And I store the Vessel for the booking
  Then I compare the Vessel with the Vessel schedule
  When I click on Send Booking button
  Then I should click on Go To Shipment button to see the shipment
  And the shipment should display the shipment name

   @145 @NOINT @login
  Scenario: Portal Quotation - OCEAN Full FCL Buyer (Booked Status)
  Given I login to Portal as user "automationdfpowner@gmail.com"
  Then the login dashboard should be visible
  Given I am on the Quotations List page
  When I store the initial total Notifications
  Then I click on Create Quotation button
  And I should see the create Quotation Page
  Then I click on "Ocean" transport mode
  And I click on "Full (FCL)" load type
  And I enter "Los Angeles" as the Origin Port
  And I enter "Shanghai" as the Destination Port
  When I click on Continue your quote
  Then I should see the Origin and Destination ports
  And I click on "Buyer"
  When I click on the calendar
  Then I select the date
  When I click on currency
  Then I select "USD" as the currency
  And I select the container size "40' Container"
  And I select the container type "All Types"
  When I click on Commodity dropdown
  Then I select the Commodity "Freight All Kinds (FAK)"
  When I click on Create quotation from details
  Then the list of the offers should appear
  #------Get the Quotation ID----------------------------------------------
  And I store the quote ID
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
  #------Verify Total notifications is initial+1 -----------
  # And the final notifications should be initial notifications +1 ----------Related Bug QWYK-8130
  Then I click on notifications button
  #-----Verify Status Booked in the Notifications-----------
  And I should see the updated status "Booked" in the notifications
    And I should see the shipment Name in the notifications
  #----------Search the quotation to Verify status is Booked---
    Given I am on the Quotations List page
    When I enter the quote ID in the search
    And I click on Search button
    Then I should see the quote ID in the results
    And I should see the quote  status is "Booked" 
   # ── Check email automationdfpowner@gmail.com--------
  When I Check the email for "automationdfpowner@gmail.com" with username ""
  Then I should receive an email with text "Your shipment's status was updated to Booked" in the body for shipment ""
#------Verify HUB with BOOKED STATUS----------------
Given I login to Hub as user "aylin.rodriguez@magaya.com"
    Then the login dashboard should be visible
    Given I navigated to quotation List in the Hub
    When I click on system id input field in the Hub
    And I enter the quote id in field in the Hub
    And I click on Search button in the hub
    Then the quote should appear in the search results in the hub
    And the status should be "Booked"

@146 @NOINT @login
  Scenario: Portal Quotation - OCEAN Partial LCL Buyer (Requested status)
  Given I login to Portal as user "automationdfpowner@gmail.com"
  Then the login dashboard should be visible
  Given I am on the Quotations List page
  When I store the initial total Notifications
  Then I click on Create Quotation button
  And I should see the create Quotation Page
  Then I click on "Ocean" transport mode
  And I click on "Partial (LCL)" load type
  And I enter "Los Angeles" as the Origin Port
  And I enter "Shanghai" as the Destination Port
  When I click on Continue your quote
  Then I should see the Origin and Destination ports
  And I click on "Buyer"
  When I click on the calendar
  Then I select the date
  When I click on currency
  Then I select "USD" as the currency
  When I click on Commodity dropdown
  Then I select the Commodity "Freight All Kinds (FAK)"
  And I select the Package "Pallet"
  And I enter the following cargo details:
  | Weight | Length | Width | Height |
  | 10     | 10     | 10    | 10     |
  And I select the accesorials "Refrigerated"
  And I select the accesorials "Overweight"
  And I select the accesorials "Stackable"
  When I click on Create quotation from details
  Then the list of the offers should appear
  #------Get the Quotation ID----------------------------------------------
  And I store the quote ID
  #---------Request a Rate-----------
  When I click on Request  a differente rate button 
  Then I should see the modal to enter the request
  When I click on select a request dropdown
  Then I select the option "I need a better rate"
  And I enter the remarks "AutomationRequest"
  When I send the request
  Then the list of the offers should appear
  #----------Search the quotation to Verify status is Requested---
    Given I am on the Quotations List page
    When I enter the quote ID in the search
    And I click on Search button
    Then I should see the quote ID in the results
    And I should see the quote  status is "Request" 
   # ── Check email automationdfpowner@gmail.com--------
  When I Check the email for "automationdfpowner@gmail.com" with username ""
  Then I should receive an email with text "A rate request was sent for a quotation." in the body for shipment ""
#------Verify HUB with Request STATUS----------------
Given I login to Hub as user "aylin.rodriguez@magaya.com"
    Then the login dashboard should be visible
    Given I navigated to quotation List in the Hub
    When I click on system id input field in the Hub
    And I enter the quote id in field in the Hub
    And I click on Search button in the hub
    Then the quote should appear in the search results in the hub
    And the status should be "Request"

  @149 @NOINT @login
  Scenario: Portal Quotation - Truck Full FTL Buyer (Requested Status)
  Given I login to Portal as user "automationdfpowner@gmail.com"
  Then the login dashboard should be visible
  Given I am on the Quotations List page
  When I store the initial total Notifications
  Then I click on Create Quotation button
  And I should see the create Quotation Page
  Then I click on "Truck" transport mode
  And I click on "Full (FTL)" load type
  And I enter "Hefei" as the Origin Port
  And I enter "Shanghai" as the Destination Port
  When I click on Continue your quote
  Then I should see the Origin and Destination ports
  And I click on "Buyer"
  When I click on the calendar
  Then I select the date
  When I click on currency
  Then I select "USD" as the currency
  And I select the container size "40' Container"
  When I click on Commodity dropdown
  Then I select the Commodity "Freight All Kinds (FAK)"
  When I click on Create quotation from details
  #------Get the Quotation ID----------------------------------------------
  And I store the quote ID
  #---------Request a Rate-----------
  When I click on Request  a differente rate button 
  Then I should see the modal to enter the request
  When I click on select a request dropdown
  Then I select the option "I need a better rate"
  And I enter the remarks "AutomationRequest"
  When I send the request
  #----------Search the quotation to Verify status is Requested---
    Given I am on the Quotations List page
    When I enter the quote ID in the search
    And I click on Search button
    Then I should see the quote ID in the results
    And I should see the quote  status is "Request" 
   # ── Check email automationdfpowner@gmail.com--------
  When I Check the email for "automationdfpowner@gmail.com" with username ""
  Then I should receive an email with text "A rate request was sent for a quotation." in the body for shipment ""
  #------Verify HUB with Request STATUS----------------
  Given I login to Hub as user "aylin.rodriguez@magaya.com"
    Then the login dashboard should be visible
    Given I navigated to quotation List in the Hub
    When I click on system id input field in the Hub
    And I enter the quote id in field in the Hub
    And I click on Search button in the hub
    Then the quote should appear in the search results in the hub
    And the status should be "Request"


@150 @NOINT @login
  Scenario: Portal Quotation - Truck Partial LTL Seller (Requested Status)
  Given I login to Portal as user "automationdfpowner@gmail.com"
  Then the login dashboard should be visible
  Given I am on the Quotations List page
  When I store the initial total Notifications
  Then I click on Create Quotation button
  And I should see the create Quotation Page
  Then I click on "Truck" transport mode
  And I click on "Partial (LTL)" load type
  And I enter "Los Angeles" as the Origin Port
  And I enter "Shanghai" as the Destination Port
  When I click on Continue your quote
  Then I should see the Origin and Destination ports
  And I click on "Seller"
  When I click on the calendar
  Then I select the date
  When I click on currency
  Then I select "USD" as the currency
  When I click on Commodity dropdown
  Then I select the Commodity "Freight All Kinds (FAK)"
  And I select the Package "Bag" for LTL
  And I enter the following cargo details:
  | Weight | Length | Width | Height |
  | 10     | 10     | 10    | 10     |
  When I click on Create quotation from details
  #------Get the Quotation ID----------------------------------------------
  And I store the quote ID
  #---------Request a Rate-----------
  When I click on Request  a differente rate button 
  Then I should see the modal to enter the request
  When I click on select a request dropdown
  Then I select the option "I need a better rate"
  And I enter the remarks "AutomationRequest"
  When I send the request
  #----------Search the quotation to Verify status is Requested---
    Given I am on the Quotations List page
    When I enter the quote ID in the search
    And I click on Search button
    Then I should see the quote ID in the results
    And I should see the quote  status is "Request" 
   # ── Check email automationdfpowner@gmail.com--------
  When I Check the email for "automationdfpowner@gmail.com" with username ""
  Then I should receive an email with text "A rate request was sent for a quotation." in the body for shipment ""
#------Verify HUB with Request STATUS----------------
Given I login to Hub as user "aylin.rodriguez@magaya.com"
    Then the login dashboard should be visible
    Given I navigated to quotation List in the Hub
    When I click on system id input field in the Hub
    And I enter the quote id in field in the Hub
    And I click on Search button in the hub
    Then the quote should appear in the search results in the hub
    And the status should be "Request"

@152 @NOINT @login
  Scenario:   152_Portal Quotation - AIR Buyer (Requested Status)
  Given I login to Portal as user "automationdfpowner@gmail.com"
  Then the login dashboard should be visible
  Given I am on the Quotations List page
  When I store the initial total Notifications
  Then I click on Create Quotation button
  And I should see the create Quotation Page
  Then I click on "Air" transport mode
  And I enter "Atlanta" as the Origin Port
  And I enter "Los Angeles" as the Destination Port
  When I click on Continue your quote
  Then I should see the Origin and Destination ports
  And I click on "Buyer"
  When I click on the calendar
  Then I select the date
  When I click on currency
  Then I select "USD" as the currency
  When I click on Commodity dropdown
  Then I select the Commodity "Freight All Kinds (FAK)"
  And I select the Package "Crate"
  And I enter the following cargo details:
  | Weight | Length | Width | Height |
  | 10     | 10     | 10    | 10     |
  When I click on Create quotation from details
  #------Get the Quotation ID----------------------------------------------
  And I store the quote ID
  #---------Request a Rate-----------
  When I click on Request  a differente rate button 
  Then I should see the modal to enter the request
  When I click on select a request dropdown
  Then I select the option "I need a better rate"
  And I enter the remarks "AutomationRequest"
  When I send the request
  #----------Search the quotation to Verify status is Requested---
    Given I am on the Quotations List page
    When I enter the quote ID in the search
    And I click on Search button
    Then I should see the quote ID in the results
    And I should see the quote  status is "Request" 
   # ── Check email automationdfpowner@gmail.com--------
  When I Check the email for "automationdfpowner@gmail.com" with username ""
  Then I should receive an email with text "A rate request was sent for a quotation." in the body for shipment ""
#------Verify HUB with Request STATUS----------------
Given I login to Hub as user "aylin.rodriguez@magaya.com"
    Then the login dashboard should be visible
    Given I navigated to quotation List in the Hub
    When I click on system id input field in the Hub
    And I enter the quote id in field in the Hub
    And I click on Search button in the hub
    Then the quote should appear in the search results in the hub
    And the status should be "Request"
    