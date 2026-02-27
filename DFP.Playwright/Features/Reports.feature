Feature: Reports
  As a user
  I want to see the reports 

  Scenario: Generate custom Report Today for Shipment
    Given I am on the Reports page
    When I click on "Shipments" option
    Then I should see "Generate Shipments" Report text
    When I select Predefined Range with text Last 7 days
    Then I should select Custom option
    When I select the Calendar
    And I should click on Today option
    When I click on Search button
    Then I should see the Save report button

  Scenario: There are no results in the Report Page
    Given I already click on Search button in the Reports Section
    When I see the Save report button
    Then I see the text We couldn't find any matching report, try changing your search filters.

  Scenario: There are results in the Report Page
    Given I already click on Search button in the Reports Section
    When I see the Save report button
    Then I could not see the text We couldn't find any matching report, try changing your search filters.
    And I should see the shipment Name