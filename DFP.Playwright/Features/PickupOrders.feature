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

  @1873 @INT @login
  Scenario:1873_MagayaToQWYK_UpdatePO
    Given the transaction "PK" "TC1873" is imported via API
    #------Portal Steps  to Verify PK is created correctly in DFP----------------
    Given I login to Portal as user "with Int"
    Then the login dashboard should be visible
    #-----Search OK in the PORTAL-----------
    Given I am on the Pickup Orders page
    Then I should see the Pickup Orders list
    Then I enter the Pickup Order number "TC1873" in the Pickup Orders section
    Then I click on "Search" button
    Then I should see the Pickup Order with number "TC1873" in the List in the Available Pickup Orders section
    #------Verify details of the PK in DFP----------------------
    Then I click on the Pickup Order with number "TC1873" in the Available Pickup Orders section
    Then I should see the Pickup Order details page with number "TC1873"
    Then I should verify the pickup order details
      | Number    | TC1873     |
      | Shipper   | automation |
      | Consignee | automation |
    #------Verify Parties in PK details page---------
    And I should verify the following parties in warehouse receipt details:
      #| Party Type | Party Name  |
      | Billing Client | automation |
      | Consignee      | automation |
      | Shipper        | automation |
      | Supplier       | automation |
      | Carrier        | MSC        |
    #------MAGAYA STEPS------------------
    #UPDATE PK in Magaya
    #-----------PORTAL STEPS to Verify the updated details of PK in DFP----------------------
    Given I am on the Pickup Orders page
    Then I should see the Pickup Orders list
    Then I enter the Pickup Order number "TC1873Updated" in the Pickup Orders section
    Then I click on "Search" button
    Then I should see the Pickup Order with number "TC1873Updated" in the List in the Available Pickup Orders section
    Then I click on the Pickup Order with number "TC1873Updated" in the Available Pickup Orders section
    Then I should see the Pickup Order details page with number "TC1873Updated"
    Then I should verify the pickup order details
      | Number                  | TC1873Updated                  |
      | Shipper                 | automation                     |
      | Consignee               | automation                     |
      | Carrier Name            | CMA                            |
      | Carrier PRO Number      | PRONumber800Updated            |
      | Carrier Booking Number  | BookingNumerUPDATED            |
      | Carrier Tracking Number | TrackingNumber800Updated       |
      | Supplier Name           | automation                     |
      | Supplier Invoice Number | INVNumberUpdated               |
      | Supplier PO Number      | PONumberUpdated                |
      | Driver                  | DriversName800Updated          |
      | License                 | UpdatedDriversLicenseNumber800 |
      | Notes                   | PO created in Magaya-Updated   |
    #------Verify Parties in PK details page---------
    And I should verify the following parties in warehouse receipt details:
      #| Party Type | Party Name  |
      | Billing Client | automation |
      | Consignee      | automation |
      | Shipper        | automation |
      | Supplier       | automation |
      | Carrier        | CMA        |
    #------Verify Event in PK details page---------
    When I go to tracking tab
    Then I should see the event "In Transit"
    Then I should see the event "Picked up"
    Then I should see the event "Delivered to consignee"
    #------Verify Commodity in PK details page---------
    When I go to cargo tab
    Then I should see the cargo items page
    Then I select the pagination number "50"
    Then I should go to the last page of the cargo items
    Then I should see the commodity "POUpdatedCommodity" in cargo details warehouse
    Then I should see the total pieces "252 pieces" in cargo details warehouse
    #------Verify attachment in PK details page---------
    When I go to attachments tab
    Then I select the pagination number "50"
    Then I should see the uploaded file "RoundPriceUpdated.xlsx"
    #------Verify ALL the uploaded files in attachments tab---------
    Then I should see the uploaded file "DOCDFP.docx"
    And I should see the uploaded file "MSGDFP.msg"
    And I should see the uploaded file "test1.png"
    And I should see the uploaded file "CSVDFP.csv"
    And I should see the uploaded file "test.jpg"
    And I should see the uploaded file "TXT_MAGAYA.txt"
    And I should see the uploaded file "XML_MAGAYA.xml"
    And I should see the uploaded file "JSON_MAGAYA.json"
    And I should see the uploaded file "XLSDFP.xlsx"
    And I should see the uploaded file "PDFDFP.pdf"
    #------Verify charge in PK details page---------
    When I go to charge and invoice tab
    And I should see the amount "$5.00" for the charge "Inland Freight"

