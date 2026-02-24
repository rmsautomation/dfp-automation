@Login
Feature: Login

Login into DFP Web Portal


Scenario: Validating Login with valid credentials
    Given the user is on the login page
    When the user logs in with valid credentials
    Then the dashboard should be visible


Scenario: Validating user can log out of the system
    Given user navigated to the dashboard
    When user logs out
    Then user should be in login page
    
