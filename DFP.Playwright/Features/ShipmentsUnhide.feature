Feature: ShipmentsUnhide

  @9634 @Login
  Scenario Outline: Unhide a completed shipment via API
    Given I have a portal API token
    When I unhide shipment with id "<shipment_id>" via API
    Then the unhide shipment request should succeed

    Examples:
      | shipment_id                          |
      | 27d587ed-a14c-4474-964b-d6d5c7c9b348 |
      | a0f26e1f-073b-4522-8862-38a5d68a29e4 |
