@Reports
Feature: Reports
  As a user
  I want to see the reports page in the portal
  So that I can see the reports and download them

 @1107 @INT @login
  Scenario: 41_1107_DFPReportsDownloadExcel
    Given I login to Portal as user "with Int"
    Then the login dashboard should be visible
    Given I am on the Reports page
    When I click on "Invoices" option
    Then I should see "Generate Invoices" Report text
    When I select Predefined Range with text Last 7 days
    Then I should select Custom option
    Then I select saved reports with name "00AAUTOMATIONMAY" 
    Then I click on "Search" button
    Then I should see the invoices in the reports Results
    Then I click on "Download to Excel" button
    Then I should verify the downloaded excel contains "15" rows