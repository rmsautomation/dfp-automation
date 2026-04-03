@CargoReleases
Feature: CargoReleases
    As a user
    I want to access to Cargo Releases in the Portal and verify that the information is correct

  @1593 @1614 @INT @login
  Scenario:1593_1614_CRWithWRAutomaticEvent
    #-------Create WH in Magaya-----------
    Given the transaction "WH" "TC1593_1614" is imported via API
    Given the transaction "CR" "TC1593_1614" is imported via API
    Given I login to Portal as user "with Int"
    Then the login dashboard should be visible
    Given I am on the Cargo Releases page
    Then the Cargo Releases page should be visible
    Given I search for the Cargo Release with value "TC1593_1614"
    Then I click on "Search" button
    Then the Cargo Release should be visible in the List with text "TC1593_1614"
    Then I select the Cargo Release from the list with text "TC1593_1614"
    Then I should see the Cargo Release details page
    Then I should verify the status in "Loaded"
    Then I should verify the CR INFO
      | Number                  | TC1593_1614          |
      | Released to             | automation           |
      | Carrier PRO Number      | PRONumber            |
      | Carrier Tracking Number | TrackingNumber       |
      | Driver                  | DriversName          |
      | License                 | DriversLicenseNumber |
      | Carrier Name            | MSC                  |
      | PO Number               | PONumber             |
    #------Verify Parties in CR details page---------
    And I should verify the following parties in warehouse receipt details:
      #| Party Type | Party Name  |
      | Billing Client | automation       |
      | Carrier        | MSC              |
      | Issued By      | Postgress SQL II |
      | Released to    | automation       |

    #------Verify Event in CR details page---------
    When I go to tracking tab
    Then I should see the event "Cargo has been created automatic Event Magaya"
    #------Verify attachment in CR details page---------
    When I go to attachments tab
    And I select the pagination number "50"
    #------Verify ALL the uploaded files in attachments tab---------
    And I should see the uploaded file "DOCDFP.docx"
    And I should see the uploaded file "MSGDFP.msg"
    And I should see the uploaded file "test1.png"
    And I should see the uploaded file "CSVDFP.csv"
    And I should see the uploaded file "test.jpg"
    And I should see the uploaded file "test2.pdf"
    And I should see the uploaded file "TXT_MAGAYA.txt"
    And I should see the uploaded file "XML_MAGAYA.xml"
    And I should see the uploaded file "JSON_MAGAYA.json"
    And I should see the uploaded file "XLSDFP.xlsx"
    And I should see the uploaded file "PDFDFP.pdf"
    #------Verify Commodity in CR details page---------
    When I go to cargo tab
    Then I should see the cargo items page
    Then I should see the commodity "UpdateCommodity" in cargo details warehouse
    Then I should verify the WH "TC1593_1614" is linked to CR
    When I click on the WH Link "TC1593_1614" in the cargo details page
    Then the warehouse receipt details should be displayed with the name "TC1593_1614"

  @1594 @1599 @INT @login
  Scenario:1594_1599MagayaToQWYK_UpdateCR
    Given the transaction "WH" "TC1594_1599" is imported via API
    Given the transaction "CR" "TC1594_1599" is imported via API
    Given I login to Portal as user "with Int"
    Then the login dashboard should be visible
    Given I am on the Cargo Releases page
    Then the Cargo Releases page should be visible
    Given I search for the Cargo Release with value "TC1594_1599"
    Then I click on "Search" button
    Then the Cargo Release should be visible in the List with text "TC1594_1599"
    Then I select the Cargo Release from the list with text "TC1594_1599"
    Then I should see the Cargo Release details page
    Then I should verify the status in "Loaded"
    Then I should verify the CR INFO
      | Number                  | TC1594_1599          |
      | Released to             | automation           |
      | Carrier PRO Number      | PRONumber            |
      | Carrier Tracking Number | TrackingNumber       |
      | Driver                  | DriversName          |
      | License                 | DriversLicenseNumber |
      | Carrier Name            | MSC                  |
      | PO Number               | PONumber             |
    #------Verify Parties in CR details page---------
    And I should verify the following parties in warehouse receipt details:
      #| Party Type | Party Name  |
      | Billing Client | automation       |
      | Carrier        | MSC              |
      | Issued By      | Postgress SQL II |
      | Released to    | automation       |
    #------Verify Event in CR details page---------
    When I go to tracking tab
    Then I should see the event "Cargo has been created automatic Event Magaya"
    #------MAGAYA STEPS---------
    #Update the CR in Magaya with new information ----------------
    #-------------Verify UPDATED IN DFP----------------
    Given I am on the Cargo Releases page
    Then the Cargo Releases page should be visible
    Given I search for the Cargo Release with value "TC_1594_1599UPDATED"
    Then I click on "Search" button
    Then the Cargo Release should be visible in the List with text "TC_1594_1599UPDATED"
    Then I select the Cargo Release from the list with text "TC_1594_1599UPDATED"
    Then I should see the Cargo Release details page
    Then I should verify the status in "Loaded"
    Then I should verify the CR INFO
      | Number                  | TC_1594_1599UPDATED         |
      | Released to             | automation                  |
      | Carrier PRO Number      | PRONumberUpdated            |
      | Carrier Tracking Number | TrackingNumberUpdated       |
      | Driver                  | DriversNameUpdated          |
      | License                 | DriversLicenseNumberUpdated |
      | PO Number               | PONumberUpdated             |
      | Notes                   | NOTE UPDATED                |
    #------Verify Parties in CR details page---------
    And I should verify the following parties in warehouse receipt details:
      #| Party Type | Party Name  |
      | Billing Client | automation       |
      | Carrier        | CMA              |
      | Issued By      | Postgress SQL II |
      | Released to    | automation       |
    #------Verify Event in CR details page---------
    When I go to tracking tab
    Then I should see the event "Arrived at destination"
    #------Verify charge in CR details page---------
    When I go to charge and invoice tab
    Then I should see the amount "$200.00" for the charge "Storage Fee"
    #------Verify attachment in CR details page---------
    When I go to attachments tab
    And I select the pagination number "50"
    #------Verify ALL the uploaded files in attachments tab---------
    And I should see the uploaded file "RoundPriceUpdated.xlsx"

  @1612 @INT @login
  Scenario:1612_QWYKToMagayaCR_Attachments
    Given the transaction "WH" "TC1612" is imported via API
    Given the transaction "CR" "TC1612" is imported via API
    Given I login to Portal as user "with Int"
    Then the login dashboard should be visible
    Given I am on the Cargo Releases page
    Then the Cargo Releases page should be visible
    Given I search for the Cargo Release with value "TC1612"
    Then I click on "Search" button
    Then the Cargo Release should be visible in the List with text "TC1612"
    Then I select the Cargo Release from the list with text "TC1612"
    Then I should see the Cargo Release details page
    Then I should verify the status in "Loaded"
    #-------Add attachments to CR in DFP---------
    When I go to attachments tab
    And I click on Attahments tab
    Then I should see the uploaded file "attachCommodity.pdf"
    #------Adding attachments in DFP for CR TC1612 and verifying the attachments are displayed in DFP and Magaya---------
    #----Upload attachDFP--------------------------
    When I click on "Upload attachment" button
    Then I should see the screen to upload the attachment
    Then I click on Drop your file here option
    When I select the file to upload "attachDFP.pdf"
    Then I enter the description "DFPAttach" for the attachment
    Then I click on Upload button
    And I should see the uploaded file "attachDFP.pdf"
    #----Upload test.jpg--------------------------
    When I click on "Upload attachment" button
    Then I should see the screen to upload the attachment
    Then I click on Drop your file here option
    When I select the file to upload "test.jpg"
    Then I enter the description "DFPAttach" for the attachment
    Then I click on Upload button
    And I should see the uploaded file "test.jpg"
    #----Upload CSVDFP--------------------------
    When I click on "Upload attachment" button
    Then I should see the screen to upload the attachment
    Then I click on Drop your file here option
    When I select the file to upload "CSVDFP.csv"
    Then I enter the description "DFPAttach" for the attachment
    Then I click on Upload button
    And I should see the uploaded file "CSVDFP.csv"
    #----Upload test1.png--------------------------
    When I click on "Upload attachment" button
    Then I should see the screen to upload the attachment
    Then I click on Drop your file here option
    When I select the file to upload "test1.png"
    Then I enter the description "DFPAttach" for the attachment
    Then I click on Upload button
    And I should see the uploaded file "test1.png"
    #----Upload DFPAttachPDF.pdf--------------------------
    When I click on "Upload attachment" button
    Then I should see the screen to upload the attachment
    Then I click on Drop your file here option
    When I select the file to upload "DFPAttachPDF.pdf"
    Then I enter the description "DFPAttach" for the attachment
    Then I click on Upload button
    And I should see the uploaded file "DFPAttachPDF.pdf"
    #----Upload MSGDFP--------------------------
    When I click on "Upload attachment" button
    Then I should see the screen to upload the attachment
    Then I click on Drop your file here option
    When I select the file to upload "MSGDFP.msg"
    Then I enter the description "DFPAttach" for the attachment
    Then I click on Upload button
    And I should see the uploaded file "MSGDFP.msg"
    #------MAGAYA STEPS to verify the attachments added in DFP are displayed in Magaya---------
    # Verify in Magaya that the attachment added in DFP is displayed in Magaya for CR TC1612

    @2210 @INT @login
  Scenario:2210_QWYKMagayaCRCargoLinked
    Given the transaction "WH" "TC2210" is imported via API
    Given the transaction "PK" "TC2210" is imported via API
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
    Then I enter the Warehouse receipt with number "TC2210" in the Available cargo section
    Then I click on "Search" button
    Then I select the item with number "TC2210" in the List in the Available cargo section
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
      | Number                  | {cr_id}              |
      | Released to             | automation           |
    #------Verify Parties in CR details page---------
    And I should verify the following parties in warehouse receipt details:
      #| Party Type | Party Name  |
      | Billing Client | automation       |
      | Released to    | automation       |
    #------Verify Event in CR details page---------
    When I go to tracking tab
    Then I should see the event "Cargo has been created automatic Event Magaya"
    #------Verify Commodity in CR details page---------
    When I go to cargo tab
    Then I should see the cargo items page
    Then I should see the commodity "UpdateCommodity" in cargo details warehouse
    #-----Verify WH and PK are linked to CR in cargo details page---------
    Then I should verify the WH "TC2210" is linked to CR
    Then I should verify the PK "TC2210" is linked to CR
    When I click on the WH Link "TC2210" in the cargo details page
    Then the warehouse receipt details should be displayed with the name "TC2210"
    When I go to cargo tab
    Then I should see the cargo items page
    Then I should verify the CR "{cr_id}" is linked to WH
    Then I should verify the PK "TC2210" is linked to CR
    When I click on the PK Link "TC2210" in the cargo details page
    Then I should see the pickup order details page with the name "TC2210"
    #------MAGAYA STEPS to verify the CR created in Portal is displayed in Magaya---------
    #Verify CR is created in Magaya with the correct information 
    #Delete CR IMPORTANT------- WH and PK created in Magaya after verification