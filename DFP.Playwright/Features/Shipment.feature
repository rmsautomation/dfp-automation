@Shipment
Feature: Shipment
  As a user
  I want to create a shipment from an existing quotation in the DFP Portal
  So I can convert created quotes into active shipments without re-entering data

  Background:
    Given I am on the Quotations List page


  Scenario: Create a shipment from a created quotation
    When I open the first  quotation in Status Booked
    Then I should be on the Quotation Details page
    When I click the "Offers" button
    Then the list of the offers should appear
    When clicks on Book Now button
    Then a confirmation dialog should appear
    When I confirm the shipment creation
    Then I should be on the Shipment Details page
    When I click on Edit button to Edit the Shipment Name
    Then I should edit the Shipment Name
    When I click on save button
    Then I should see the new Shipment Name
    When I click on Send Booking button
    Then I should click on Go To Shipment button to see the shipemnt
    And the shipment should display the shipment name


  Scenario: Add and validate tags across shipment list, table and details views
    Given user navigated to Shipments List
    Then a tag icon should be displayed below the shipment name or on the left side of existing tags
    And the tag icon tooltip should say "Add a customized tag to your shipment. You can add up to 5 tags"
    When user clicks the tag icon on a shipment
    Then a field should appear to select an existing tag or create a new tag
    When user creates and assigns a new tag to the selected shipment
    Then the tag should be visible on the selected shipment
    When user assigns the same tag to other shipments from the Shipments List
    Then the same tag should be visible on all selected shipments
    When user opens the tagged shipments details view
    Then the tag should be visible in Shipment details
    And the tag should be visible in the Tags column in Shipment Table view
    And the tag should be visible in Shipment list view


  
