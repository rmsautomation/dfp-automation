@Shipment
Feature: Shipment
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
    When I click on Edit button to Edit the Shipment Name
    Then I should edit the Shipment Name
    When I click on save button
    Then I should see the new Shipment Name
    When I click on Send Booking button
    Then I should click on Go To Shipment button to see the shipment
    And the shipment should display the shipment name


  Scenario: Search shipment by reference
    Given user navigated to Shipments List
    When I click on Show More filters
    And I enter the shipment name in Shipment Reference field
    And I click on Search button
    Then the shipment should appear in the search results


  Scenario: Select first Shipment in the List
    Given user navigated to Shipments List
    When user selects the first shipment from the list


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
    Given user navigated to Shipments List
    When I click on Show More filters
    And I enter the shipment name in Shipment Reference field
    And I click on Search button
    Then the shipment should appear in the search results
    Then a tag icon should be displayed below the shipment name or on the left side of existing tags
    And the tag icon tooltip should say "Add a customized tag to your shipment. You can add up to 5 tags"
    When user clicks the tag icon on the shipment
    Then a field should appear to select an existing tag or create a new tag
    When user creates and assigns a new tag to the shipment
    Then the tag should be visible on the selected shipment
    # ── Shipment 1: Verify tag in List view and Shipment Details ─────────────
    Then the tag should be visible in Shipment list view
    When user opens the tagged shipment details view
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
    Given user navigated to Shipments List
    When I click on Show More filters
    And I enter the shipment name in Shipment Reference field
    And I click on Search button
    Then the shipment should appear in the search results
    When user clicks the tag icon on the shipment
    Then a field should appear to select an existing tag or create a new tag
    When user assigns the existing tag to the shipment
    Then the tag should be visible on the selected shipment
    # ── Shipment 2: Verify tag in List view and Shipment Details ─────────────
    Then the tag should be visible in Shipment list view
    When user opens the tagged shipment details view
    Then the tag should be visible in Shipment details
    # ── Reset filters and verify both shipments have the tag ──────────────────
    Given user navigated to Shipments List
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
    Given user navigated to Shipments List
    When I click on Show More filters
    And I enter the shipment name in Shipment Reference field
    And I click on Search button
    Then the shipment should appear in the search results
    # ── Add tag 1 ─────────────────────────────────────────────────────────────
    When user clicks the tag icon on the shipment
    Then a field should appear to select an existing tag or create a new tag
    When user creates and assigns a new tag to the shipment
    Then the tag should be visible on the selected shipment
    # ── Add tag 2 ─────────────────────────────────────────────────────────────
    When user clicks the tag icon on the shipment
    Then a field should appear to select an existing tag or create a new tag
    When user creates and assigns a new tag to the shipment
    Then the tag should be visible on the selected shipment
    # ── Add tag 3 ─────────────────────────────────────────────────────────────
    When user clicks the tag icon on the shipment
    Then a field should appear to select an existing tag or create a new tag
    When user creates and assigns a new tag to the shipment
    Then the tag should be visible on the selected shipment
    # ── Add tag 4 ─────────────────────────────────────────────────────────────
    When user clicks the tag icon on the shipment
    Then a field should appear to select an existing tag or create a new tag
    When user creates and assigns a new tag to the shipment
    Then the tag should be visible on the selected shipment
    # ── Add tag 5 ─────────────────────────────────────────────────────────────
    When user clicks the tag icon on the shipment
    Then a field should appear to select an existing tag or create a new tag
    When user creates and assigns a new tag to the shipment
    Then the tag should be visible on the selected shipment
    # ── Try to add a 6th tag (should be blocked by max limit) ─────────────────
    When user clicks the tag icon on the shipment
    Then the system should show the error "A maximum of 5 Tags are allowed per shipment"
    # ── Verify all 5 tags across all views ────────────────────────────────────
    Then all created tags should be visible in Shipment list view
    When user opens the tagged shipment details view
    Then all created tags should be visible in Shipment details
    Then all created tags should be visible in Shipment Table view


@API @9634
  Scenario Outline: Hide a completed shipment via API
    Given I have a portal API token
    When I hide shipment with id "<shipment_id>" via API
    Then the hide shipment request should succeed

    Examples:
      | shipment_id                          |
      | 27d587ed-a14c-4474-964b-d6d5c7c9b348 |
      | a0f26e1f-073b-4522-8862-38a5d68a29e4 |


@API @9634
  Scenario Outline: Unhide a completed shipment via API
    Given I have a portal API token
    When I unhide shipment with id "<shipment_id>" via API
    Then the unhide shipment request should succeed

    Examples:
      | shipment_id                          |
      | 27d587ed-a14c-4474-964b-d6d5c7c9b348 |
      | a0f26e1f-073b-4522-8862-38a5d68a29e4 |

@API @7873
  Scenario Outline: Link shipment to purchase order and order line via API
    Given I have a portal API token
    When I link shipment with id "<shipment_id>" to purchase order "<purchase_order_id>" via API
    And I link cargo item "<cargo_item_id>" to order line "<order_line_id>" for shipment "<shipment_id>" via API
    Then the link requests should succeed

    Examples:
      | shipment_id     | purchase_order_id | cargo_item_id     | order_line_id     |
      | PUT_SHIPMENT_ID | PUT_PO_ID         | PUT_CARGO_ITEM_ID | PUT_ORDER_LINE_ID |



