@QInventory
Feature: Inventory
    As a user
    I want to manage inventory in the DFP Portal

  @1307 @INT @login
  Scenario: 1307_MagayaToQWYK_InventoryItem
    Given the transaction "IV" "Mar31155414InvItem" is imported via API
    Given I login to Portal as user "with Int"
    Then the login dashboard should be visible
    Given I am on the Inventory page
    Then the inventory page should be visible
    Given I search for the inventory item "Part number" with value "Mar31155414InvItem"
    Then I click on "Search" button
    Then the inventory item should be visible in the List
    Then I select the inventory item from the list with text "Mar31155414InvItem"
    Then I should see the inventory item details page
    Then I should verify the following inventory item details:
      | Part number       | Mar31155414InvItem        |
      | Model             | Mar31155415InvModel       |
      | Description       | Mar31155415InvDescription |
      | Manufacturer      | automation                |
      | Customer          | automation                |
      | Amount per pallet | 10                        |
      | Packaging         | Pallet                    |
      | Commodity type    | Freight All Kinds         |

  @1308 @INT @login
  Scenario: 1308_MagayaToQWYK_UpdateInvItem
    # Given the transaction "IV" "TC1308" is imported via API
    Given I login to Portal as user "with Int"
    Then the login dashboard should be visible
    Given I am on the Inventory page
    Then the inventory page should be visible
    Given I search for the inventory item "Part number" with value "TC1308"
    Then I click on "Search" button
    Then the inventory item should be visible in the List
    Then I select the inventory item from the list with text "TC1308"
    Then I should see the inventory item details page
    Then I should verify the following inventory item details:
      | Part number       | TC1308                    |
      | Model             | Apr01111816InvModel       |
      | Description       | Apr01111816InvDescription |
      | Manufacturer      | automation                |
      | Customer          | automation                |
      | Amount per pallet | 10                        |
      | Packaging         | Pallet                    |
      | Commodity type    | Freight All Kinds         |
    #---------------MAGAYA Steps---------------------------
    #Update InventoryItem in Magaya and create a WH using the new   InventoryItem with 100 pieces
    #---------------DFP Steps to Verify Item Updated---------------------------
    Given I am on the Inventory page
    Then the inventory page should be visible
    Given I search for the inventory item "Part number" with value "TC1308"
    Then I click on "Search" button
    Then the inventory item should be visible in the List
    Then I select the inventory item from the list with text "Updated"
    Then I should see the inventory item details page
    Then I should verify the following inventory item details:
      | Part number       | TC1308               |
      | Model             | Updated              |
      | Description       | Updated              |
      | Notes             | InventoryItemUpdated |
      | Manufacturer      | automation           |
      | Customer          | automation           |
      | Amount per pallet | 500                  |
      | Packaging         | Package              |
      | Commodity type    | Freight All Kinds    |
    Then I should verify the total pieces in the inventory item details page is "100"
    Then I click on On Hand icon
    Given I set the warehouse receipt name to "TC1308"
    And I enter the warehouse receipt name in search field
    And I click on Search button
    Then the warehouse receipt should appear in the search results in the List with text "automation"
    And I select the warehouse receipt in the search results with text "automation"
    And the warehouse receipt details should be displayed with the name "TC1308"
    And I should verify label header "Number" contains "TC1308"
    And I should verify the following label headers in warehouse receipt details:
      # | Header                  | Value                    |
      | Number         | TC1308     |
      | Supplier       | automation |
      | Billing Client | automation |
    When I go to cargo tab
    Then I should see the cargo items page
    Then I should see the commodity "Updated" in cargo details warehouse
    Then I should see the text "TC1308" in the cargo items column
    Then I should see the text "100x Package" in the cargo items column