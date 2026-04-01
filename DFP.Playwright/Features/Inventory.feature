@QInventory
Feature: Inventory
  As a user
  I want to manage inventory in the DFP Portal

  @1307 @INT @login
  Scenario: 42_1307_MagayaToQWYK_InventoryItem
  #Given the transaction "IV" "Mar31155414InvItem" is imported via API
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