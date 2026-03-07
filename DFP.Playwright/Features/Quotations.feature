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