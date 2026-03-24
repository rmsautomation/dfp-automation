@HubRadar
Feature: HubRadar
  As a user
  I want to access to Radar in the DFP Hub

@281 @NOINT @login
Scenario: Hub-Radar_281
Given I login to Hub as user "aylin.rodriguez@magaya.com"
Then the login dashboard should be visible
When I navigate to radar page
Then I should see the section header "Sales" in the Hub
When I click on view sales radar
Then I should see the section header "Sales Radar" in the Hub
#------Verify Performance over Time section ----------
Then I verify Performance over Time section
#------Verify Rankings ------------------
And I verify Rankings section
#D------Verify ata Table --------------
And I verify Data Table section
#---------Hub - Filter by day/week/month---------------
When I filter radar by period "1w"
Then the radar charts are visible
#---------Hub - Filter by Transport Mode PRODUCT AIR-----------
When I open radar filters
And I filter radar by transport mode "AIR"
And I apply radar filters
When I click the radar refresh button
Then the radar charts are visible
And the data table column "Transport Mode" should have value "AIR"
When I reset radar filters
#---------Hub - Filter by Load Type LCL LTL--------
When I open radar filters
And I filter radar by load type "LCL/LTL"
And I apply radar filters
When I click the radar refresh button
Then the radar charts are visible
And the data table column "Load type" should have value "lcl"
When I reset radar filters
#--------Hub - Filter by Account Manager Aylin Rodriguez
When I open radar filters
And I filter radar by account manager "Aylin Rodriguez"
And I apply radar filters
When I click the radar refresh button
Then the radar charts are visible
And the data table column "Account manager" should have value "Aylin Rodriguez,AutomationHub,AutomationHub"
#--------Hub - Refresh graphics----------------
When I reset radar filters
When I click the radar refresh button
Then the radar charts are visible
#--------Hub - Full Screen--------------------
When I click the radar full screen button
Then the radar is displayed in full screen
And I exit full screen mode
#---------	Hub - Filter by Selected Metric by quotation value----------
When I filter radar rankings by metric "Quotation Value"
And I click the radar refresh button
Then the radar charts are visible
#-----------Hub - Export to CSV-------------------
When I export radar data to CSV
Then a radar file is downloaded
#----------Hub - Export to Excel-----------------------
When I export radar data to Excel
Then a radar file is downloaded
