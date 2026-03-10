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


  @9340 @NOINT
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


  @9344_MoreThan5tagsSH @NOINT
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
    Then all created tags should be visible in Shipment Table view
    When I open the tagged shipment details view
    Then all created tags should be visible in Shipment details
    


 @API @9634 @9652 @NOINT
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
    Given I am on the dashboard page
    Then I store the total Shipments in the Dashboard
    Given I have a hub API token
    When I hide shipment via API
    Then the hide shipment request should succeed
   # ── Verify HIDE shipment in List View─────────
    Given I navigated to Shipments List
    When I click on Show More filters
    And I click on List View button
    And I enter the shipment name in Shipment Reference field
    And I click on Search button
    Then the shipment should not appear in the search results
   # ── Verify HIDE shipment in Table View─────────
    When I click on Table View 
    And I click on Search button
    Then the shipment should not appear in the search results
   # ── Verify HIDE shipment in Reports─────────
    Given I am on the Reports page
    When I click on "Shipments" option
    Then I should see "Generate Shipments" Report text
    When I select Predefined Range with text Last 7 days
    Then I should select Custom option
    When I select the Calendar
    And I should click on Today option
    And I click on Search button
    When I see the Save report button
    Then the shipment name should not appear in the report results
   # ── Verify HIDE shipment in Dashboard─────────
    Given I am on the dashboard page
    Then I store the total Shipments in the Dashboard after operation
    Then I see initial total shipment -1
 Given I login to Hub as user "without Int"
    Then the login dashboard should be visible
    # ── Verify HIDE shipment in the HUB─────────
    Given I navigated to shipment List in the Hub
    When I click on Customer Reference input field in the Hub
    And I enter the shipment name in Customer Reference field in the Hub
    And I click on Search button in the hub
    Then the shipment should NOT appear in the search results in the hub
    When I unhide shipment via API
    Then the unhide shipment request should succeed
  # ── Verify UNHIDE shipment in the HUB─────────
    When I click on Search button in the hub
    Then the shipment should appear in the search results in the hub
    Given I navigated to Shipments List
   # ── Verify UNHIDE shipment in List View─────────
    And I click on Show More filters
    And I click on List View button
    And I enter the shipment name in Shipment Reference field
    And I click on Search button
    Then the shipment should appear in the search results
   # ── Verify UNHIDE shipment in Table View─────────
    When I click on Table View 
    And I click on Search button
    Then the shipment should appear in the search results
    # ── Verify UNHIDE shipment in Reports─────────
    Given I am on the Reports page
    When I click on "Shipments" option
    Then I should see "Generate Shipments" Report text
    When I select Predefined Range with text Last 7 days
    Then I should select Custom option
    When I select the Calendar
    And I should click on Today option
    And I click on Search button
    When I see the Save report button
    Then I could not see the text We couldn't find any matching report, try changing your search filters.
    And I should see the shipment Name
   # ── Verify UNHIDE shipment in Dashboard─────────
    Given I am on the dashboard page
    Then I store the total Shipments in the Dashboard after operation
    Then I see initial total shipment
    
    Examples:
      | shipment_id |
      |             |
 

@API @7873 @NOINT
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
    # ── Verify PO Link in the shipment─────────
   Given I login to Portal as user "without Int"
    Given I navigated to Shipments List
    And I click on Show More filters
    And I click on List View button
    And I enter the shipment name in Shipment Reference field
    And I click on Search button
    Then the shipment should appear in the search results
    When I click on the shipment
    Then I should be on the Shipment Details page
    When I click on Booking Details Tab
    Then I should see the Purchase Order section in the Shipment Portal
    # ── Verify PO Link in the shipment redirects to the PO Details─────────
    When I click on Purchase Order link
    Then I should be on the Purchase Order Details 
    # ── Verify PO in In Progress and PI is linked to the SH─────────
    And I should see the Status of the PO In Progress
    And I should see Booked Shipments section in the Purchase order
    When I click on the Shipment Name link
    Then I should be on the Shipment Details page
    When I click on Booking Details Tab
    Then I should see the Purchase Order section in the Shipment Portal
    # ── Verify Cargo Link in the shipment─────────
    When I click on Cargo section with PO
    Then I should be on the Purchase Order Details 
    # ── Verify Oder Line has  the shipment relation────────
    And Order Line has a Shipment Name link related

@10351 @NOINT
Scenario: Shipments - Validate global search bar - Behavior when filtering by Quick search - Table/List View
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
    # ── Search the shipment Using Quick Filter ────────────────────────────────────────────────────
    Given I navigated to Shipments List
   When I enter the shipment Reference in Quick filter
   And I click on Search button
   Then the shipment should appear in the search results
   # ── Show More filters Verify Quick Filter does NOT exist in List View────────────────────────────────────────────────────
   When I click on Show More filters
   Then I should not see the quick filter field
   # ── Verify Quick Filter exists in List View────────────────────────────────────────────────────
   When I click on Show Less
   Then I should see the quick filter field
   # ── Verify Quick Filter  exists in Table View───────────────────
   When I click on Table View 
   And I click on Search button
   Then the shipment should appear in the search results
   # ── Verify Quick Filter  DOES NOT exist in Table View───────────────────
   When I click on Show More filters
   Then I should not see the quick filter field
   When I click on Show Less
   Then I should see the quick filter field

@4508 @NOINT @login
Scenario: Remove View Shipments permission in Hub and validate shipment data is hidden in Portal
    Given I login to Hub as user "without Int"
    Then the login dashboard should be visible
    When I go to Portal Users
    Then the Portal Users page should be displayed
    When I search the User by email automation_noint_permissions@yopmail.com
    And I click on search button
    Then I should see the user in the results
    When I click on the Customer Name in the User Page 
    Then I should see the User Details page 
    # ── Disable View Shipment Permissions─────────
    When I click on the Permissions dropdwon 
    And I enter the permission in the  search section
    And I unchecked the "View Shipments" permission
    And I click on Save User button
    Then the Portal Users page should be displayed
    Given I login to Portal as user "automation_noint_permissions@yopmail.com"
    Then the login dashboard should be visible
    # ── Verify in the Portal Disable View Shipment Permissions─────────
    Then the "Shipments List" option should not be displayed
    And the dashboard should not show shipment related information

@3986 @INT @login
Scenario: Magaya to DFP - Update House shipment - Exclude from Tracking = True
# ──I have a House Shipment created in Magaya with enable the Exclude from Tracking option
Given I login to Portal as user "with Int"
   # ── Verify the House Shipment is not displayed in the in List View─────────
    Given I navigated to Shipments List
    When I click on Show More filters
    And I click on List View button
    Given I set the shipment name to "HOUSE3986"
    And I enter the shipment name in Shipment Reference field
    And I click on Search button
    Then the shipment should not appear in the search results
   # ── Verify the Shipment is not displayed  in Table View─────────
    When I click on Table View 
    And I click on Search button
    Then the shipment should not appear in the search results
   # ── Verify the MASTER Shipment is  displayed─────────
   Given I set the shipment name to "TC3986"
   When I enter the shipment name in Shipment Reference field
    And I click on Search button
    Then the shipment should appear in the search results
 # ── Verify the House tab is not displayed in MASTER Shipment ────────
   When I open the tagged shipment details view
   Then I should not see House tab
   # ── Verify HOUSE shipment should NOT APPEAR in Reports─────────
    Given I set the shipment name to "HOUSE3986"
    Given I am on the Reports page
    When I click on "Shipments" option
    Then I should see "Generate Shipments" Report text
    When I select Predefined Range with text Last 7 days
    Then I should select Custom option
    When I select the Calendar
    And I should click on Today option
    And I click on Search button
    When I see the Save report button
    Then the shipment name should not appear in the report results

@10255 @NOINT
 Scenario: Milestone - Update expected_timestamp for a milestone - Check Date history in the Portal
 # ── Create Shipment in the Portal────────
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
 Given I login to Hub as user "without Int"
    Then the login dashboard should be visible
# ── Go To shipment in the HUB─────────
    Given I navigated to shipment List in the Hub
    When I click on Customer Reference input field in the Hub
    And I enter the shipment name in Customer Reference field in the Hub
    And I click on Search button in the hub
    Then the shipment should appear in the search results in the hub
    And I select the created Shipment
    # ── Update the Expected date for a milestone─────────
    When I go to Milestones tab
    Then I click on Edit button related to "Container empty to shipper"
    When I click on the calendar button
    Then I should select the "date" in the calendar
    When I click on save changes button 
    Then I should see the "date" in the Milestone tab
    # ── Adding second date to see the history─────────
    Then I click on Edit button related to "Container empty to shipper"
    When I click on the calendar button
    Then I should select the next week "date" in the calendar
    When I click on save changes button 
    # ── Verify in the PORTAL that a new label is displayed next to the milestone date.─────────
    When I open the portal URL "without int"
    Given I navigated to Shipments List
    When I click on Show More filters
    And I enter the shipment name in Shipment Reference field
    And I click on Search button
    Then the shipment should appear in the search results
    When I select the first shipment from the list
    Then I should see a new label next week to the milestone date in "Container empty to shipper"
    When I click on the new label next week to the milestone date in "Container empty to shipper"
    # ── Clicking the tab should display a new pop-up window showing the current date and the historical changes in the dates.───────
    Then I should see a popup  with the current date 
    And I should see the historical changes

@API @9893 @9894 @NOINT
Scenario: Enable tracking for a shipment subscribe containers and send coordinates and Unsubscribe
    Given I have a portal API token
    And I have a hub API token
    When I create shipment via webhook
    Then a shipment id should be available for tracking
    When I subscribe current shipment to live tracking via API
    Then the shipment subscribe request should succeed
    Given I login to Hub as user "without Int"
    Then the login dashboard should be visible
    And I Check the tracking is enabled for the shipment in the hub
    When I subscribe first container of current shipment to live tracking via API
    Then the container subscribe request should succeed
    Given I login to Portal as user "without Int"
    Given I navigated to Shipments List
    When I click on Show More filters
    And I enter the shipment name in Shipment Reference field
    And I click on Search button
    Then the shipment should appear in the search results
    When I open the shipment from search results
    Then subscribed container should be available in Shipment Summary dropdown
    When I send tracking coordinates for the subscribed container via API
    Then the tracking event request should succeed
    And I refresh the page
    When I select the subscribed container in Shipment Summary
    Then I Check that Container LiveTrack and map coordinates are displayed
    When I click on Shipment Tracking tab
    Then the Tracking Events section should display the latest container event
    When I Unsubscribe the container "" with tracking already added ""
    Then the container unsubscribe request should succeed
    And I refresh the page
    Then unsubscribed container should not be available in Shipment Summary dropdown
    When I unsubscribe current shipment from live tracking via API
    Then the shipment unsubscribe request should succeed
    Given I login to Hub as user "without Int"
    Then the login dashboard should be visible
    And I Check the tracking is disabled for the shipment in the hub

@8086 @NOINT
Scenario: Status update - List View - Subscribe to notifications
When I Check the email for " aylinquotationop@yopmail.com" with username ""
Then I should receive an email with text "A new shipment was booked.|Shipment:" in the body for shipment "Ocean FCL Los Angeles to Shanghai Pt"


@4520 @NOINT @login
Scenario:Parent-child tree structure - All permissions
# ── Create quotation FCL in the Portal WITHOUT INT with a user whose Customer is defined as a Child ───────
Given I login to Portal as user "child_noint@yopmail.com"
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
#------Get the Quotation ID----------------------------------------------
  And I store the quote ID
#------Create Booking and SH WITHOUT ATTACHMENTS--------------------------
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
#----------Verify Quote and Booking using PARENT Login-----------------
    And I log out from Portal
    Given I login to Portal as user "aylin.rodriguez@magaya.com"
    Given I navigated to Shipments List
    When I enter the shipment Reference in Quick filter
    And I click on Search button
#------SHIPMENT APPEARS IN THE PORTAL USING PARENT--------------
    Then the shipment should appear in the search results
    Given I am on the Quotations List page
    When I enter the quotation ID in the search section
    And I click on Search button
#------QUOTE ID APPEARS IN THE PORTAL USING PARENT--------------
    Then I should see the quote ID in the results
#------Create Shipment WITH ATTACHMENTS USING CHILD--------------------------
    And I log out from Portal
    Given I login to Portal as user "child_noint@yopmail.com"
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
    And I click on Attahments tab
    When I click on Attach document button
    Then I should see the screen to upload the attachment
    When I select the file to upload "2026-03-09_13-33-33.png"
    Then I click on Upload button
    And I should see the uploaded file "2026-03-09_13-33-33.png"
    When I click on Send Booking button
    Then I should click on Go To Shipment button to see the shipment
    And the shipment should display the shipment name
#------Create Purchase Order USING CHILD------------------------
    Given I am on the purchase order list page
    When I click on the "Create New Purchase Order" button
    Then I should be on the purchase order creation page
    And I enter the Purchase Order number
    And I enter the buyer details
    And I select the currency
    And I enter the supplier details
    And I select the Transport Mode "Ocean"
    And I enter the Cargo Origin "Los Angeles"
    And I enter the Cargo destination "Shanghai"
    When I click on Save button in the Purchase Order
    Then I should see the purchase order details
#----------Verify SH ATTACHMENTS AND PO USING PARENT-----------------
    And I log out from Portal
    Given I login to Portal as user "aylin.rodriguez@magaya.com"
    Given I navigated to Shipments List
    When I enter the shipment Reference in Quick filter
    And I click on Search button
#------SHIPMENT APPEARS IN THE PORTAL USING PARENT------------------
    Then the shipment should appear in the search results
#----------Verify Search PO USING PARENT----------------------------
    Given I am on the purchase order list page
    When I enter the purchase order number in the search
    And I click on PO search button
    Then I should see the purchase order number in the list
