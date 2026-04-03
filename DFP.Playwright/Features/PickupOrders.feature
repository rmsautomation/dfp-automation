@Pickup
Feature: Pickup Orders
    As a user
    I want to view and manage pickup orders

  @1875 @1880 @INT @login
  Scenario: 1875_1880_MagayDFP_POWithAttchAutEv
    Given the transaction "PK" "TC1875_1880" is imported via API
    #------Portal Steps  to Verify PK is created correctly in DFP----------------
    Given I login to Portal as user "with Int"
    Then the login dashboard should be visible
    #-----Search OK in the PORTAL-----------
    Given I am on the Pickup Orders page
    Then I should see the Pickup Orders list
    Then I enter the Pickup Order number "TC1875_1880" in the Pickup Orders section
    Then I click on "Search" button
    Then I should see the Pickup Order with number "TC1875_1880" in the List in the Available Pickup Orders section
    #------Verify details of the PK in DFP----------------------
    Then I click on the Pickup Order with number "TC1875_1880" in the Available Pickup Orders section
    Then I should see the Pickup Order details page with number "TC1875_1880"
    Then I should verify the pickup order details
      | Number                  | TC1875_1880             |
      | Shipper                 | automation              |
      | Consignee               | automation              |
      | Carrier Name            | MSC                     |
      | Carrier PRO Number      | PRONumber798            |
      | Carrier Booking Number  | BookingNumer798         |
      | Carrier Tracking Number | TrackingNumber798       |
      | Supplier Name           | automation              |
      | Driver                  | DriversName798          |
      | License                 | DriversLicenseNumber798 |
      | Notes                   | PO created in Magaya    |
    #------Verify Parties in PK details page---------
    And I should verify the following parties in warehouse receipt details:
      #| Party Type | Party Name  |
      | Billing Client | automation |
      | Consignee      | automation |
      | Shipper        | automation |
      | Supplier       | automation |
      | Carrier        | MSC        |
    #------Verify Event in PK details page---------
    When I go to tracking tab
    Then I should see the event "In Transit"
    Then I should see the event "Picked up"
    #------Verify Commodity in PK details page---------
    When I go to cargo tab
    Then I should see the cargo items page
    Then I should see the commodity "UpdateCommodity" in cargo details warehouse
    Then I should see the total pieces "251 pieces" in cargo details warehouse
    #------Verify attachment in PK details page---------
    When I go to attachments tab
    And I should see the uploaded file "test.jpg"
    #------Verify charge in PK details page---------
    When I go to charge and invoice tab
    And I should see the amount "$230.00" for the charge "Crating Fee"

  @3353 @INT @login
  Scenario: 3353_DFP_POVerifyPDF
    Given the transaction "PK" "TC3353" is imported via API
    #------Portal Steps  to Verify PK is created correctly in DFP----------------
    Given I login to Portal as user "with Int"
    Then the login dashboard should be visible
    #-----Search OK in the PORTAL-----------
    Given I am on the Pickup Orders page
    Then I should see the Pickup Orders list
    Then I enter the Pickup Order number "TC3353" in the Pickup Orders section
    Then I click on "Search" button
    Then I should see the Pickup Order with number "TC3353" in the List in the Available Pickup Orders section
    #------Verify details of the PK in DFP----------------------
    Then I click on the Pickup Order with number "TC3353" in the Available Pickup Orders section
    Then I should see the Pickup Order details page with number "TC3353"
    #------Download to PDF the PK---------
    When I go to cargo tab
    Then I should see the cargo items page
    Then I should see the commodity "UpdateCommodity" in cargo details warehouse
    Then I store the total pieces in cargo details for the pickup order
    Then I click on PDF button in the pickup order details
    Then I select "Download PDF" option
    Then I should verify the total pieces in the PDF match with the total pieces for the pickup order
