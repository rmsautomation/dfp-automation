@QuotationsHub
Feature: QuotationsHub
  As a user
  I want to create a quotation in the DFP Hub

@129 @NOINT @login
Scenario: HUB Quotation- Create Open Quotation (Full Load-Ocean)
Given I login to Hub as user "aylin.rodriguez@magaya.com"
Then the login dashboard should be visible
Given I navigated to quotation List in the Hub
When I click on Create Quotation button in the Hub
Then I should see the create quotation page in the Hub
And I select the Customer "AutomationOwner" in the Hub
#And I select the Customer "AutomationDFPRMS" in the Hub---Another Customer
And I select the Load Type "Full Load" in the Hub
And I select the modality "Ocean" in the Hub
And I enter the Origin "Los Angeles" in the Hub
And I enter the Destination "Shanghai" in the Hub
When I click on continue button in the Hub
Then I should see the quotation page to enter the details in the Hub
And I select the commodity "Agriculture" in the Hub
And I select the currency "USD" in the Hub
And I select the container Size "40' Container" in the Hub
And I select additionals "Require origin charges" in the Hub
And I select additionals "Require destination charges" in the Hub
When I click on Create Quotation in the Hub
Then I should see the quotation in "Draft" status in the Hub
And I store the quote id in the Hub
And I should see offers in the Hub
And I click on Publish Quotation in the hub
When I click on Yes button in the hub
#-------The Quote Status is Open--------
Then I should see the quotation in "Open" status in the Hub
#------The user receives a notification in the PORTAL automationdfpowner@gmail.com another gmail“rmsautomation24@gmail.com“-------
Given I login to Portal as user "automationdfpowner@gmail.com"
Then I click on notifications button
And I should see the updated status "You received a new quotation" in the notifications
And I should see the quote id in the notifications
#------The user receives an Email with the notification “You received a new quotation“------
When I Check the email for "automationdfpowner@gmail.com" with username ""
Then I should receive a quotation email with the stored quote id and text "You received a new quotation"

@131 @NOINT @login
Scenario: HUB Quotation- Create Close Quotation (Full Load-Truck)
Given I login to Hub as user "aylin.rodriguez@magaya.com"
Then the login dashboard should be visible
Given I navigated to quotation List in the Hub
When I click on Create Quotation button in the Hub
Then I should see the create quotation page in the Hub
And I select the Customer "AutomationOwner" in the Hub
#And I select the Customer "AutomationDFPRMS" in the Hub---Another Customer
And I select the Load Type "Full Load" in the Hub
And I select the modality "Truck" in the Hub
And I enter the Origin "Hefei" in the Hub
And I enter the Destination "Shanghai" in the Hub
When I click on continue button in the Hub
Then I should see the quotation page to enter the details in the Hub
And I select the commodity "Agriculture" in the Hub
And I select the currency "USD" in the Hub
And I select the container Size "40' Container" in the Hub
And I select additionals "Require origin charges" in the Hub
And I select additionals "Require destination charges" in the Hub
When I click on Create Quotation in the Hub
Then I should see the quotation in "Draft" status in the Hub
And I store the quote id in the Hub
And I should see offers in the Hub
And I click on Publish Quotation in the hub
When I click on Yes button in the hub
#-------The Quote Status is Open--------
Then I should see the quotation in "Open" status in the Hub
#------The user receives a notification in the PORTAL automationdfpowner@gmail.com another gmail“rmsautomation24@gmail.com“-------
Given I login to Portal as user "automationdfpowner@gmail.com"
Then I click on notifications button
And I should see the updated status "You received a new quotation" in the notifications
And I should see the quote id in the notifications
#------The user receives an Email with the notification “You received a new quotation“------
When I Check the email for "automationdfpowner@gmail.com" with username ""
Then I should receive a quotation email with the stored quote id and text "You received a new quotation"

@132 @NOINT @login
Scenario: HUB Quotation- Create Draft Quotation (Partial Load-Air)
Given I login to Hub as user "aylin.rodriguez@magaya.com"
Then the login dashboard should be visible
Given I navigated to quotation List in the Hub
When I click on Create Quotation button in the Hub
Then I should see the create quotation page in the Hub
And I select the Customer "AutomationOwner" in the Hub
#And I select the Customer "AutomationDFPRMS" in the Hub---Another Customer
And I select the Load Type "Partial Load" in the Hub
And I select the modality "Air" in the Hub
And I enter the Origin "Shanghai" in the Hub
And I enter the Destination "Los Angeles" in the Hub
When I click on continue button in the Hub
Then I should see the quotation page to enter the details in the Hub
And I select the commodity "Agriculture" in the Hub
And I select the currency "USD" in the Hub
And I select the Package "Carton" in the Hub
  And I enter the following cargo details in the Hub:
  | Weight | Length | Width | Height |
  | 10     | 10     | 10    | 10     |
And I select additionals "Refrigerated" in the Hub
And I select additionals "Overweight" in the Hub
When I click on Create Quotation in the Hub
#-------The Quote Status is Draft--------
Then I should see the quotation in "Draft" status in the Hub
And I store the quote id in the Hub
#------The user DOES NOT receive a notification in the PORTAL automationdfpowner@gmail.com another gmail“rmsautomation24@gmail.com“-------
Given I login to Portal as user "automationdfpowner@gmail.com"
Then I click on notifications button
And I should not see the quote id in the notifications
#------The user DOES NOT receive an Email with the notification “You received a new quotation“------
When I Check the email for "automationdfpowner@gmail.com" with username ""
Then I should not receive a quotation email with the stored quote id and text "You received a new quotation"

@133 @NOINT @login
Scenario: HUB Quotation- Create Open Quotation (Partial Load-Ocean)
Given I login to Hub as user "aylin.rodriguez@magaya.com"
Then the login dashboard should be visible
Given I navigated to quotation List in the Hub
When I click on Create Quotation button in the Hub
Then I should see the create quotation page in the Hub
And I select the Customer "AutomationOwner" in the Hub
#And I select the Customer "AutomationDFPRMS" in the Hub---Another Customer with email gmail“rmsautomation24@gmail.com“
And I select the Load Type "Partial Load" in the Hub
And I select the modality "Ocean" in the Hub
And I enter the Origin "Los Angeles" in the Hub
And I enter the Destination "Shanghai" in the Hub
When I click on continue button in the Hub
Then I should see the quotation page to enter the details in the Hub
And I select the commodity "Agriculture" in the Hub
And I select the currency "USD" in the Hub
And I select the Package "Pallet" in the Hub
  And I enter the following cargo details in the Hub:
  | Weight | Length | Width | Height |
  | 10     | 10     | 10    | 10     |
And I select additionals "Require origin charges" in the Hub
And I select additionals "Require destination charges" in the Hub
When I click on Create Quotation in the Hub
#-------The Quote Status is Draft--------
Then I should see the quotation in "Draft" status in the Hub
And I store the quote id in the Hub
And I should see offers in the Hub
And I click on Publish Quotation in the hub
When I click on Yes button in the hub
#-------The Quote Status is Open--------
Then I should see the quotation in "Open" status in the Hub
#------The user receives a notification in the PORTAL automationdfpowner@gmail.com another gmail“rmsautomation24@gmail.com“-------
Given I login to Portal as user "automationdfpowner@gmail.com"
Then I click on notifications button
And I should see the updated status "You received a new quotation" in the notifications
And I should see the quote id in the notifications
#------The user receives an Email with the notification “You received a new quotation“------
When I Check the email for "automationdfpowner@gmail.com" with username ""
Then I should receive a quotation email with the stored quote id and text "You received a new quotation"

