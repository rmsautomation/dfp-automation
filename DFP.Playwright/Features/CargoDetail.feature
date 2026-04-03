@CargoDetail
Feature: Cargo Details
    As a user
    I want to view and manage cargo details

  @2058 @INT @login
  Scenario: 2058_CargoDetailRelatedToPO_WR_and_CR
    Given the transaction "WH" "TC2058" is imported via API
    Given the transaction "PK" "TC2058" is imported via API
    Given I login to Portal as user "with Int"
    Then the login dashboard should be visible
    Given I am on the Cargo Releases page
    Then the Cargo Releases page should be visible
    #---------CREATE CR IN PORTAL---------
    When I click on "Create" button
    Then I should select "New Cargo Release" option
    Then I should see the Create Cargo Release page with text "New Cargo Release"
    Then I should select Release at "Now" option
    Then I should enter the name "automation"
    Then I should select the country "Algeria" for the CR
    When I click on "Next" button in the Create Cargo Release page
    Then I enter the Warehouse receipt with number "TC2058" in the Available cargo section
    Then I click on "Search" button
    Then I select the item with number "TC2058" in the List in the Available cargo section
    When I click on "Load selected items" button in the Create Cargo Release page step2
    Then the item is loaded with text "UpdateCommodity" in the Create Cargo Release page step2
    When I click on Next button in the Create Cargo Release page step2
    When I click on "Send Cargo Release" button in the Create Cargo Release page step3
    Then I should see the text "Your cargo release was received!" in the confirmation message
    Then I store the Cargo Release number
    When I click on "Continue to cargo release" button in the confirmation message
    #-----------Verify CR details page---------
    Then I should see the Cargo Release details page
    Then I should verify the status in "Loaded"
    Then I should verify the CR INFO
      | Number      | {cr_id}    |
      | Released to | automation |
    #------Verify Parties in CR details page---------
    And I should verify the following parties in warehouse receipt details:
      #| Party Type | Party Name  |
      | Billing Client | automation |
      | Released to    | automation |
    #------Verify Event in CR details page---------
    When I go to tracking tab
    Then I should see the event "Cargo has been created automatic Event Magaya"
    #------Verify Commodity in CR details page---------
    When I go to cargo tab
    Then I should see the cargo items page
    Then I should see the commodity "UpdateCommodity" in cargo details warehouse
    #-----Verify WH and PK are linked to CR in cargo details page---------
    Then I should verify the WH "TC2058" is linked to CR
    Then I should verify the PK "TC2058" is linked to CR
    When I click on the WH Link "TC2058" in the cargo details page
    Then the warehouse receipt details should be displayed with the name "TC2058"
    When I go to cargo tab
    Then I should see the cargo items page
    Then I should verify the CR "{cr_id}" is linked to WH
    Then I should verify the PK "TC2058" is linked to CR
    When I click on the PK Link "TC2058" in the cargo details page
    Then I should see the pickup order details page with the name "TC2058"
    #------MAGAYA STEPS to verify the CR created in Portal is displayed in Magaya---------
    #Verify CR is created in Magaya with the correct information
    #Verify Commodity exist with PK Number TC2058 and the history is related to CR, WH and PK TC2058
    #---------------DFP STEPS to verify Cargo Detail Linked to WH PK AND CR----------------------
    Given I am on the Cargo Detail page
    Then I should see the Cargo Detail page
    #Verify PK in the portal cargo detail
    Then I search for Parent "Pickup Order" with number "TC2058" in the Cargo Detail page
    Then I click on "Search" button
    Then I should see the search result with the description "UpdateCommodity" and status "Loaded" in the Cargo Detail page
    Then I click on "Reset" button in the Cargo Detail page
    #Verify WH in the portal cargo detail
    Then I search for Parent "Warehouse Receipt" with number "TC2058" in the Cargo Detail page
    Then I click on "Search" button
    Then I should see the search result with the description "UpdateCommodity" and status "Loaded" in the Cargo Detail page
    Then I click on "Reset" button in the Cargo Detail page
    #Verify  CR in the portal cargo detail
    Then I search for Parent "Cargo Release" with number "{cr_id}" in the Cargo Detail page
    Then I click on "Search" button
    Then I should see the search result with the description "UpdateCommodity" and status "Loaded" in the Cargo Detail page
    #Search CR in the POrtal and Verify
    Given I am on the Cargo Releases page
    Then the Cargo Releases page should be visible
    Then I enter the Cargo Release number "{cr_id}" in the Cargo Releases search section
    Then I click on "Search" button
    Then I should see the Cargo Release with number "{cr_id}" in the List
    #Search WH in the POrtal and Verify
    Given I navigated to Warehouse Receipts List
    Given I set the warehouse receipt name to "TC2058"
    And I enter the warehouse receipt name in search field
    And I click on Search button
    Then the warehouse receipt should appear in the search results in the List with text "automation"
    #Search PK in the POrtal and Verify
    Given I am on the Pickup Orders page
    Then I should see the Pickup Orders list
    Then I enter the Pickup Order number "TC2058" in the Pickup Orders section
    Then I click on "Search" button
    Then I should see the Pickup Order with number "TC2058" in the List in the Available Pickup Orders section

