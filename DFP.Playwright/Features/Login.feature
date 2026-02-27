@Login
Feature: Login

Login into DFP Web Portal and Hub


Scenario Outline: Login to Hub
    Given I login to Hub as user "<user_type>"
    Then the dashboard should be visible

    Examples:
      | user_type                    |
      | without Int                  |
      | with Int                     |


Scenario Outline: Login to Portal
    Given I login to Portal as user "<user_type>"
    Then the dashboard should be visible

    Examples:
      | user_type   |
      | without Int |
      | with Int    |



Scenario: Log out from Portal
    Given I login to Portal with integration
    When I log out
    Then I should be in login page
    

