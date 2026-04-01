@CargoReleases
Feature: CargoReleases
    As a user
    I want to access to Cargo Releases in the Portal and verify that the information is correct

  @1593 @1614 @INT @login
  Scenario: 1593_1614_CRWithWRAutomaticEvent
    #-------Create WH in Magaya-----------
    Given the transaction "WH" "TC1593_1614" is imported via API
    Given the transaction "CR" "TC1593_46_1614" is imported via API
    Given I login to Portal as user "with Int"
    Then the login dashboard should be visible
    Given I am on the Cargo Releases page
    Then the Cargo Releases page should be visible
    Given I search for the Cargo Release with value "TC1593_1614"
    Then I click on "Search" button
    Then the Cargo Release should be visible in the List with text "TC1593_1614"
    Then I select the Cargo Release from the list with text "TC1593_1614"
    Then I should see the Cargo Release details page
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
    #------Verify charge in CR details page---------
    When I go to charge and invoice tab
    And I should see the amount "$150.00" for the charge "Cartage Fee"
#------Verify attachment in WH details page---------
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
