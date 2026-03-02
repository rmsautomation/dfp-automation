@ShipmentsHub 
Feature: ShipmentsHub
  As a hub user
  I want to search for a shipment by reference in the Hub
  So I can quickly find and verify shipment details
  
@login
  Scenario: Search Shipment by Reference in the Hub
    Given I login to Hub as user "without Int"
    Then the login dashboard should be visible
    Given I navigated to shipment List in the Hub
    When I click on Shipment Reference input field in the Hub
    And I enter the shipment name in Shipment Reference field in the Hub
    And I click on Search button in the hub
    Then the shipment should appear in the search results in the hub
