Feature: ShipmentsHide

  @9634 @Login
  Scenario Outline: Hide a completed shipment via API
    Given I have a portal API token
    When I hide shipment with id "<shipment_id>" via API
    Then the hide shipment request should succeed

    Examples:
      | shipment_id                          |
      | 27d587ed-a14c-4474-964b-d6d5c7c9b348 |
      | a0f26e1f-073b-4522-8862-38a5d68a29e4 |

  Scenario Outline: Link shipment to purchase order and order line via API
    Given I have a portal API token
    When I link shipment with id "<shipment_id>" to purchase order "<purchase_order_id>" via API
    And I link cargo item "<cargo_item_id>" to order line "<order_line_id>" for shipment "<shipment_id>" via API
    Then the link requests should succeed

    Examples:
      | shipment_id | purchase_order_id | cargo_item_id | order_line_id |
      | PUT_SHIPMENT_ID | PUT_PO_ID | PUT_CARGO_ITEM_ID | PUT_ORDER_LINE_ID |
