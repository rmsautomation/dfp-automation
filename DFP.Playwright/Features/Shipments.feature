@Shipments
Feature: Shipments
  As a user
  I want to create a shipment from an existing quotation in the DFP Portal
  So I can convert created quotes into active shipments without re-entering data


  Scenario: Create a shipment from a created quotation
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
    When I click on save button
    Then I should see the new Shipment Name
    When I click on Send Booking button
    Then I should click on Go To Shipment button to see the shipment
    And the shipment should display the shipment name


  Scenario: Search shipment by reference
    Given I navigated to Shipments List
    When I click on Show More filters
    And I enter the shipment name in Shipment Reference field
    And I click on Search button
    Then the shipment should appear in the search results


  Scenario: Select first Shipment in the List
    Given I navigated to Shipments List
    When I select the first shipment from the list


  @9340
  Scenario: Add and validate tags across shipment list, table and details views
    # ── Shipment 1: Create ────────────────────────────────────────────────────
    Given I am on the Quotations List page
    When I open the first quotation in Status Booked
    Then I should be on the Quotation Details page
    When I click the "Offers" button
    Then the list of the offers should appear
    When I click on Book Now button
    Then a confirmation dialog should appear
    When I confirm the shipment creation
    Then I should be on the Shipment Details page
    When I click on Edit button to Edit the Shipment Name
    Then I should edit the Shipment Name
    When I click on save button
    Then I should see the new Shipment Name
    When I click on Send Booking button
    Then I should click on Go To Shipment button to see the shipment
    And the shipment should display the shipment name
    # ── Shipment 1: Search + verify tag icon + add NEW tag ────────────────────
    Given I navigated to Shipments List
    When I click on Show More filters
    And I enter the shipment name in Shipment Reference field
    And I click on Search button
    Then the shipment should appear in the search results
    Then a tag icon should be displayed below the shipment name or on the left side of existing tags
    And the tag icon tooltip should say "Add a customized tag to your shipment. You can add up to 5 tags"
    When I click the tag icon on the shipment
    Then a field should appear to select an existing tag or create a new tag
    When I create and assigns a new tag to the shipment
    Then the tag should be visible on the selected shipment
    # ── Shipment 1: Verify tag in List view and Shipment Details ─────────────
    Then the tag should be visible in Shipment list view
    When I open the tagged shipment details view
    Then the tag should be visible in Shipment details
    # ── Shipment 2: Create ────────────────────────────────────────────────────
    Given I am on the Quotations List page
    When I open the first quotation in Status Booked
    Then I should be on the Quotation Details page
    When I click the "Offers" button
    Then the list of the offers should appear
    When I click on Book Now button
    Then a confirmation dialog should appear
    When I confirm the shipment creation
    Then I should be on the Shipment Details page
    When I click on Edit button to Edit the Shipment Name
    Then I should edit the Shipment Name
    When I click on save button
    Then I should see the new Shipment Name
    When I click on Send Booking button
    Then I should click on Go To Shipment button to see the shipment
    And the shipment should display the shipment name
    # ── Shipment 2: Search + add the SAME (existing) tag ─────────────────────
    Given I navigated to Shipments List
    When I click on Show More filters
    And I enter the shipment name in Shipment Reference field
    And I click on Search button
    Then the shipment should appear in the search results
    When I click the tag icon on the shipment
    Then a field should appear to select an existing tag or create a new tag
    When I assign the existing tag to the shipment
    Then the tag should be visible on the selected shipment
    # ── Shipment 2: Verify tag in List view and Shipment Details ─────────────
    Then the tag should be visible in Shipment list view
    When I open the tagged shipment details view
    Then the tag should be visible in Shipment details
    # ── Reset filters and verify both shipments have the tag ──────────────────
    Given I navigated to Shipments List
    When I reset the search filters
    Then the tag should be visible on 2 shipments in Shipment list view
    And the tag should be visible on 2 shipments in Shipment Table view


  @9344_MoreThan5tagsSH
  Scenario: Validate maximum 5 tags per shipment and visibility across all views
    # ── Shipment: Create ──────────────────────────────────────────────────────
    Given I am on the Quotations List page
    When I open the first quotation in Status Booked
    Then I should be on the Quotation Details page
    When I click the "Offers" button
    Then the list of the offers should appear
    When I click on Book Now button
    Then a confirmation dialog should appear
    When I confirm the shipment creation
    Then I should be on the Shipment Details page
    When I click on Edit button to Edit the Shipment Name
    Then I should edit the Shipment Name
    When I click on save button
    Then I should see the new Shipment Name
    When I click on Send Booking button
    Then I should click on Go To Shipment button to see the shipment
    And the shipment should display the shipment name
    # ── Search the shipment ────────────────────────────────────────────────────
    Given I navigated to Shipments List
    When I click on Show More filters
    And I enter the shipment name in Shipment Reference field
    And I click on Search button
    Then the shipment should appear in the search results
    # ── Add tag 1 ─────────────────────────────────────────────────────────────
    When I click the tag icon on the shipment
    Then a field should appear to select an existing tag or create a new tag
    When I create and assigns a new tag to the shipment
    Then the tag should be visible on the selected shipment
    # ── Add tag 2 ─────────────────────────────────────────────────────────────
    When I click the tag icon on the shipment
    Then a field should appear to select an existing tag or create a new tag
    When I create and assigns a new tag to the shipment
    Then the tag should be visible on the selected shipment
    # ── Add tag 3 ─────────────────────────────────────────────────────────────
    When I click the tag icon on the shipment
    Then a field should appear to select an existing tag or create a new tag
    When I create and assigns a new tag to the shipment
    Then the tag should be visible on the selected shipment
    # ── Add tag 4 ─────────────────────────────────────────────────────────────
    When I click the tag icon on the shipment
    Then a field should appear to select an existing tag or create a new tag
    When I create and assigns a new tag to the shipment
    Then the tag should be visible on the selected shipment
    # ── Add tag 5 ─────────────────────────────────────────────────────────────
    When I click the tag icon on the shipment
    Then a field should appear to select an existing tag or create a new tag
    When I create and assigns a new tag to the shipment
    Then the tag should be visible on the selected shipment
    # ── Try to add a 6th tag (should be blocked by max limit) ─────────────────
    When I click the tag icon on the shipment
    Then the system should show the error "A maximum of 5 Tags are allowed per shipment"
    # ── Verify all 5 tags across all views ────────────────────────────────────
    Then all created tags should be visible in Shipment list view
    When I open the tagged shipment details view
    Then all created tags should be visible in Shipment details
    Then all created tags should be visible in Shipment Table view


 @API @9634 @9652
  Scenario Outline: Hide_Unhide a shipment created from a quotation
    Given I login to Portal as user "without Int"
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
    When I click on save button
    Then I should see the new Shipment Name
    When I click on Send Booking button
    Then I should click on Go To Shipment button to see the shipment
    And the shipment should display the shipment name
    When I log out
    Given I have a hub API token
    When I hide shipment via API
    Then the hide shipment request should succeed
    When I unhide shipment via API
    Then the unhide shipment request should succeed
	Given I login to Hub as user "without Int"
    When I log out
    Given I login to Portal as user "automation_noint@yopmail.com"


    Examples:
      | shipment_id |
      |             |
 

@API @7873
  Scenario: Shipment with cargo items - add lines from a PO
    Given I have a portal API token
    When I create shipment via webhook
    And I get cargo items for current shipment via API
    And I create purchase order via API
    And I create purchase order line via API
    Then a cargo item id should be available
    And a purchase order id should be available
    And an order line id should be available
    When I link shipment to purchase order via API
    And I link cargo item to order line for shipment via API
    Then the link requests should succeed





