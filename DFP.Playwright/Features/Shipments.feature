@Shipments
Feature: Shipments
    As a user
    I want to create a shipment from an existing quotation in the DFP Portal
    So I can convert created quotes into active shipments without re-entering data

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
    And I store shipment Name
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
    And I store shipment Name
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
    And I store shipment Name
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
      |             |


  @API @7873 @NOINT
  Scenario: Shipment with cargo items - add lines from a PO_7873
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
    And I store shipment Name
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
    And I store shipment Name
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

  @8086 @NOINT @login
  Scenario: Status update - List View - Subscribe to notifications
    # ── Create shipment with Owner-------------------------------
    Given I login to Portal as user "automationdfpowner@gmail.com"
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
    When I log out
    Then I should be in login page
    # ── Suscribe shipment with suscriptordfpautomation@yopmail.com-------------------------------
    Given I login to Portal as user "suscriptordfpautomation@yopmail.com"
    Given I navigated to Shipments List
    When I click on Show More filters
    And I enter the shipment name in Shipment Reference field
    And I click on Search button
    Then the shipment should appear in the search results
    When I click on the Subscribe button
    Then I should see a new panel to select the Notification
    And I enable the option "Receive Shipment Status Notification"
    When I click on save button
    Then I should see the subscribe text changed to Unsubscribe in the List
    #--------When I open the shipment from search results
    When I open the tagged shipment details view
    Then I should see the subscribe text changed to Unsubscribe in the Details View
    # ── Go TO Hub and Change Shipment Milestone to Confirmed-------------------------------
    Given I login to Hub as user "aylin.rodriguez@magaya.com"
    Then the login dashboard should be visible
    Given I navigated to shipment List in the Hub
    When I click on Customer Reference input field in the Hub
    And I enter the shipment name in Customer Reference field in the Hub
    And I click on Search button in the hub
    Then the shipment should appear in the search results in the hub
    And I select the created Shipment
    # ── Change Shipment Milestone to Confirmed─────────
    When I go to Milestones tab
    Then I click on Confirm button from "Confirmation" section
    And I should see the Confirmation Page
    And I select the Expected date "" in the calendar
    And I select the Actual date "" in the calendar
    When I click on save changes button
    Then I should see the green icon
    # ── Check how the subscripted user suscriptordfpautomation@yopmail.com receives a notification (Email, Inbox and Popup).
    When I open the portal URL "without int"
    Given I am on the dashboard page
    Then I click on notifications button
    And I should see the updated status "Confirmed" in the notifications
    And I should see the shipment Name in the notifications
    And I log out from Portal
    # ── Check how the owner user automationdfpowner@gmail.com receives a notification (Email, Inbox and Popup).
    Given I login to Portal as user "automationdfpowner@gmail.com"
    Given I am on the dashboard page
    Then I click on notifications button
    And I should see the updated status "Confirmed" in the notifications
    And I should see the shipment Name in the notifications
    # ── Check email suscriptordfpautomation@yopmail.com
    When I Check the email for "suscriptordfpautomation@yopmail.com" with username ""
    Then I should receive an email with text "Your shipment's status was updated to Confirmed" in the body for shipment ""
    # ── Check email automationdfpowner@gmail.com
    When I Check the email for "automationdfpowner@gmail.com" with username ""
    Then I should receive an email with text "Your shipment's status was updated to Confirmed" in the body for shipment ""



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
    And I store shipment Name
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
    And I store shipment Name
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

  @5305 @API @INT @login
  Scenario: Shipment - Create a shipment - Verify Custom fields
    Given the transaction "SH" "TC5305" is imported via API
    Given I login to Portal as user "with Int"
    # ── Verify the SH Custom Fields are displayed in Shipment List ─────────────
    Given I navigated to Shipments List
    When I enter "TC5305" in Quick filter
    And I click on Search button
    Then the shipment should appear in the search results
    And I click on Table View
    And I select the "DefaultWithcustom" column view
    And I check the following custom field values in the table view for shipment
      #Example
      # | Column                  | Value                   |
      | INCO Terms              | Exworks          |
      | Boolean                 | Yes              |
      | DFP Shipper Reference   | SHIPPER5305      |
      | DFP Consignee Reference | CONSIGNEEREF5305 |
      #| DFP Pricing             | PRICING5305             |
      #| DFP Forwarder           | FORWARDER5305           |
      | DFP Payment terms       | COLLECT          |
      #| DFP INCO Terms          | DAT                     |
      #| DFP Cargo ready         | CARGO5305               |
      | Shipper Reference       | SHIPPER5305      |
      | Shipment Guid           | GUID5305         |
    #Commented customs fields are related to this bug in DFP https://gocatapult.atlassian.net/browse/QWYK-9584

  @553 @login @Int
  Scenario: Integration - QWYK to Magaya - Shipment- Attach a jpg file_553
    #-------Create Shipment in Portal With attachments-----------------------
    Given I login to Portal as user "with Int"
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
    And I store shipment Name
    #----------Adding attachment JPG in DFP Shipment ------------
    And I click on Attahments tab
    When I click on Attach document button
    Then I should see the screen to upload the attachment
    When I select the file to upload "test.jpg"
    Then I click on Upload button
    And I should see the uploaded file "test.jpg"
    When I click on Send Booking button
    Then I should click on Go To Shipment button to see the shipment
    And the shipment should display the shipment name
    #---------MAGAYA STEPS------------------------
    #Go To Magaya and verify the Booking is created
    #Verify the attachment exists in the Booking
    #Create Shipment from Bookign in Magaya
    #Verify attachment in the shipment in Magaya


  @840 @858 @login @Int
  Scenario: 840_858_SHDFPToMagaya_UpdateBookingMagaya
    #-------Create Shipment in Portal With ALL INFO-----------------------
    Given I login to Portal as user "with Int"
    Given I am on the Quotations List page
    When I open the first quotation in Status Booked
    Then I should be on the Quotation Details page
    When I click the "Offers" button
    Then the list of the offers should appear
    #------Get the Quotation ID----------------------------------------------
    And I store the quote ID
    When I click on Book Now button
    Then a confirmation dialog should appear
    When I confirm the shipment creation
    Then I should be on the Shipment Details page
    Then I store the shipment id from the URL
    And I store shipment Name
    And I store the now var
    And I enter the vessel "AutomationVessel" in the Portal
    When I go to "References" Tab in the Shipment Portal
    Then I enter the Shipper "ShipperReference" in the Shipment Portal
    And I enter the Consignee "ConsigneeReference" in the Shipment Portal
    And I enter the Notify "NotifyReference" in the Shipment Portal
    And I enter the Forwarder "ForwarderReference" in the Shipment Portal
    When I go to "Shipment Parties" Tab in the Shipment Portal
    Then I enter the name "AutomationName" in the Shipment Portal
    And I enter the address "AutomationAddress" in the Shipment Portal
    When I go to "Terms & Requirements" Tab in the Shipment Portal
    #If the parameter is Empty this value should be the NOW var
    Then I enter the  Instructions remarks  "" in the Shipment Portal
    And I click on Attahments tab
    When I click on Attach document button
    Then I should see the screen to upload the attachment
    When I select the file to upload "test.jpg"
    Then I click on Upload button
    And I should see the uploaded file "test.jpg"
    When I click on Send Booking button
    Then I should click on Go To Shipment button to see the shipment
    And the shipment should display the shipment name
    #-------In MAGAYA, verify that the Booking was received with ALL INFO------------
    #-------Update the booking in MAGAYA.--------------------------------
    #------Verify the UPDATES in DFP.-------------------------
    Given I navigated to Shipments List
    When I enter the shipment Reference in Quick filter
    And I click on Search button
    #If the shipment is updated in Magaya, the shipment name should be updated with "UPDATED" text in DFP
    #Click the reload button every 3 seconds for 5 minutes, until the shipment name is updated in DFP with the text "UPDATED" following the update in Magaya.
    And the shipment should appear in the search results with text "UPDATED"
    When I open the tagged shipment details view
    Then I the shipment name should contains "UPDATED"
    And I verify the origin "MIAMI"
    And I verify the status "Arrived" in the shipment detail page
    And I verify the shipper contains "Updated" in the shipment detail page
    And I verify the consignee contains "Updated" in the shipment detail page
    And I verify the Forwarder contains "Updated" in the shipment detail page
    And I verify the quote id in the shipment detail page
    When I go to Tracking tab
    Then I should see the event "Arrived at destination"
    And I click on Attahments tab
    And I should see the uploaded file "RoundPriceUpdated.xlsx"
    And I should see the uploaded file "DOCDFP.docx"
    And I should see the uploaded file "PDFDFP.pdf"
    And I should see the uploaded file "XLSDFP.xlsx"
    And I should see the uploaded file "CSVDFP.csv"
    And I should see the uploaded file "TXT_MAGAYA.txt"
    And I should see the uploaded file "MSGDFP.msg"
    And I should see the uploaded file "XML_MAGAYA.xml"
    And I should see the uploaded file "test1.png"
    And I should see the uploaded file "test.jpg"
    And I should see the uploaded file "JSON_MAGAYA.json"
    When I go to Booking Details tab
    Then I should see the commodity "UpdateCommodity"
    And I should see the Remarks Instructions contains "UPDATED"
    And I should see the shipper contains "Updated"
    And I should see billing client contains "automation"
    And I should see link entities shipper contains "updated"
    When I go to Charge and Invoices tab
    Then I should see the charge "Storage Fee"

  @841 @login @Int
  Scenario: 841_SHQWYKToMagayaShipmentSendAttcahment
    #-------Create Shipment in Portal With attachments-----------------------
    Given I login to Portal as user "with Int"
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
    And I store shipment Name
    When I click on Send Booking button
    Then I should click on Go To Shipment button to see the shipment
    And the shipment should display the shipment name
    #---------MAGAYA STEPS------------------------
    #Go To Magaya and verify the Booking is created
    #In Magaya, convert the booking into a shipment
    #Update the name in Magaya with UPDATED Add All Type of Attachments in the shipment in Magaya (PDF, Excel, Word, CSV, TXT, MSG, XML, JSON, PNG, JPG) AT COMMODITY LEVEL
    #-------------------Verify the attachments are displayed in the shipment in DFP VerifyDFPAllAttachmentsToShInMagayaCommodity----------------
    Given I navigated to Shipments List
    When I enter the shipment Reference in Quick filter
    And I click on Search button
    #If the shipment is updated in Magaya, the shipment name should be updated with "UPDATED" text in DFP
    #Click the reload button every 3 seconds for 5 minutes, until the shipment name is updated in DFP with the text "UPDATED" following the update in Magaya.
    And the shipment should appear in the search results with text "UPDATED"
    When I open the tagged shipment details view
    When I click on Attahments tab
    And I should see the uploaded file "DOCDFP.docx"
    And I should see the uploaded file "PDFDFP.pdf"
    And I should see the uploaded file "XLSDFP.xlsx"
    And I should see the uploaded file "CSVDFP.csv"
    And I should see the uploaded file "TXT_MAGAYA.txt"
    And I should see the uploaded file "MSGDFP.msg"
    And I should see the uploaded file "XML_MAGAYA.xml"
    And I should see the uploaded file "test1.png"
    And I should see the uploaded file "test.jpg"
    And I should see the uploaded file "JSON_MAGAYA.json"
    And I should see the uploaded file "attachCommodity.pdf"
    #-------------------Upload Attachments in DFP DFPAttachment.jpg and DFPAttachPDF.pdf----------------
    When I click on Attach document button
    Then I should see the screen to upload the attachment
    When I select the file to upload "DFPAttachment.jpg"
    Then I click on Upload button
    And I should see the uploaded file "DFPAttachment.jpg"
    When I click on Attach document button
    Then I should see the screen to upload the attachment
    When I select the file to upload "DFPAttachPDF.pdf"
    Then I click on Upload button
    And I should see the uploaded file "DFPAttachPDF.pdf"
    #---------MAGAYA STEPS------------------------
    #IN MAGAYA Verify the attachment is displayed DFPAttachment.jpg and DFPAttachPDF.pdf----------------

  @2244 @login @Int
  Scenario: 2244_AirShipment_UpdateMasterHouse
    #---------MAGAYA STEPS------------------------
    #Go To Magaya and create a WH
    #I store the "WH" GUID
    #Go To Magaya and create the Master AIR Shipment with attachments  test.jpg and test2.pdf, AIRCarrier, Forwarde AgentDestination, Los Angeles to Shanghai, etc
    #I store shipment reference and shipment name
    #-------Verify Shipment in Portal With attachments-----------------------
    Given I login to Portal as user "with Int"
    Given I navigated to Shipments List
    When I enter the shipment Reference in Quick filter
    And I click on Search button
    #_shipmentName wihtout parameter should be the same as the stored shipment name
    #If the shipment is updated in Magaya, the shipment name should be updated with "UPDATED" text in DFP
    #Click the reload button every 2 seconds for 3 minutes, until the shipment name is updated in DFP with the text "UPDATED" following the update in Magaya.
    Then the shipment should appear in the search results
    When I open the tagged shipment details view
    Then I should see the oringin "Los Angeles"
    And I should see the destination "Shanghai"
    And I should see the shipper "SHIPPER"
    And I should see the consignee "CONSIGNEE"
    When I go to attachments tab
    Then I should see the uploaded file "test.jpg"
    And I should see the uploaded file "test2.pdf"
    When I go to charge and invoice tab
    Then I should see the charge "Crating Fee"
    When I go to Booking Details tab
    Then I should see the commodity "Automation Commodity"
    #---------MAGAYA STEPS-----------------------
    #In Magaya, create a House SH linked to the Master SH
    #Update Master shipment name to have text "WithHouse" in Magaya
    #---------Portal STEPS Verify House create linked to the Master SH-----------------------
    Given I navigated to Shipments List
    When I enter the shipment Reference in Quick filter
    And I click on Search button
    #_shipmentName wihtout parameter should be the same as the stored shipment name
    #If the shipment is updated in Magaya, the shipment name should be updated with "UPDATED" text in DFP
    #Click the reload button every 2 seconds for 3 minutes, until the shipment name is updated in DFP with the text "UPDATED" following the update in Magaya.
    Then the shipment should appear in the search results with text "WithHouse"
    And  I should see the Master SH icon in the search results
    When I open the tagged shipment details view
    Then I should see the shipment details page
    And I store the shipmentId
    And I go to booking details tab
    Then I store the master total pieces
    And I go to House tab
    Then I should see the House SH linked to the Master SH contains "HAWB"
    And I store the houseId
    #---------Portal STEPS Verify House SH Details-----------------------
    When I click on the houseId in the shipment details page
    Then I should see the shipment details page
    And I should see the origin "Los Angeles"
    And I should see the destination "Shanghai"
    When I go to attachments tab
    And I should see the uploaded file "test2.pdf"
    And I should see the uploaded file "Arrival Notice - Air - Unrated.pdf"
    When I go to charge and invoice tab
    Then I should see the charge "Crating Fee"
    When I go to booking details tab
    Then I should see the house total pieces
    And I store house total pieces
    And I verify the house total pieces is  Expected HousePIeces+1= Master Shipment total pieces
    And I should see the house was created in DFP
    #---------Portal STEPS Verify RelationShipMasterHouseWH-------------------------
    When I click on "warehouse-receipts" link in the House SH details page
    Then I should see the "Warehouse receipt" details page
    And I should see the correct "WH" GUID in the URL
    When I go to cargo tab
    Then I should see the cargo items page
    And I click on the "shipments" link in the cargo item details
    And I should see the shipment details page
    And I should see shipmentId
    And I go to House tab
    Then I should see the House SH linked to the Master SH contains "HAWB"
    When I click on the houseId in the shipment details page
    Then I should see the shipment details page
    #---------MAGAYA STEPS-----------------------
    #Update MASTER SHIPMENT in Magaya
    #Update Shipment Name with UPDATED
    #---------Portal STEPS Verify Updated Master SH-----------------------
    Given I navigated to Shipments List
    When I enter the shipment Reference in Quick filter
    And I click on Search button
    Then the shipment should appear in the search results with text "UPDATED"
    And  I should see the Master SH icon in the search results
    When I open the tagged shipment details view
    Then I should see the shipment details page
    And I should see the oringin "New York"
    And I should see the destination "Rotterdam"
    And I should see the shipper "UPDATEDSHIPPER"
    And I should see the consignee "UPDATEDCONSIGNEE"
    When I go to tracking tab
    Then I should see the event "Arrived at destination"
    When I go to attachments tab
    And I should see the uploaded file "RoundPriceUpdated.xlsx"
    When I go to booking details tab
    Then I should see the commodity "UpdateCommodity"
    And I should see the description contains "Description of Goods UPDATED"
    And I should see panel "COLLECT" in the booking details
    And I should see the GUID "ShipmentGuidUPDATED" in the booking details
    And I should see the shipmentRef contains "UPDATED" in the booking details
    And I should see the link entities contains "Updated" in the booking details
    #-----  ---Portal STEPS Verify Updated House SH Details-----------------------
    When I go to House tab
    Then I should see the House SH linked to the Master SH contains "HAWB"
    When I click on the houseId in the shipment details page
    Then I should see the shipment details page
    And I should see the oringin "New York"
    And I should see the destination "Rotterdam"
    When I go to booking details tab
    Then I should see the link entities contains "Updated" in the booking details
    #---------MAGAYA STEPS-----------------------
    #Update HOUSE SHIPMENT in Magaya, add attachCommodity.pdf at commodity level
    #Update Master Shipment  Name with UPDATEDHOUSE
    #--------Verify the UPDATES in DFP for Master Shipment includes houseCommodity -------------------------
    Given I navigated to Shipments List
    When I enter the shipment Reference in Quick filter
    And I click on Search button
    Then the shipment should appear in the search results with text "UPDATEDHOUSE"
    And  I should see the Master SH icon in the search results
    When I open the tagged shipment details view
    Then I should see the shipment details page
    When I go to booking details tab
    Then I should see the commodity "houseUpdatedDescription"
    #---------Portal STEPS Verify Updated House SH-----------------------
    When I go to House tab
    Then I should see the House SH linked to the Master SH contains "HAWB"
    When I click on the houseId in the shipment details page
    Then I should see the shipment details page
    And I should see the oringin "New York"
    And I should see the destination "Rotterdam"
    And I should see the shipper "UPDATEDSHIPPER"
    And I should see the consignee "UPDATEDCONSIGNEE"
    When I go to tracking tab
    Then I should see the event "Arrived at destination"
    When I go to attachments tab
    And I should see the uploaded file "RoundPriceUpdated.xlsx"
    And I should see the uploaded file "attachCommodity.pdf"
    When I go to booking details tab
    Then I should see the commodity "houseUpdatedDescription"
    And I store the house total pieces after updating the house
    And And I should see the description contains "General Goods UPDATEDSHIPMENT"
    And I should see panel "COLLECT" in the booking details
    And I should see panel "Test" in the booking details
    And I should see the GUID "ShipmentGuidUPDATED" in the booking details
    And I should see the shipmentRef contains "UPDATED" in the booking details
    And I should see the link entities contains "Updated" in the booking details
    When I go to charge and invoice tab
    Then I should see the charge "Storage Fee"

    










   






