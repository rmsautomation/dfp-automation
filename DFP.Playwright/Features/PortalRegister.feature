@PortalRegister
Feature: PortalRegister
  As a new user
  I want to register for the DFP Portal

@272 @NOINT @login
Scenario: New User registration - a new user registers for the portal_272
  #------Create YOPMAIL user----------
  When I go to yopmail URL
  Then I store the now var
  And I store the new contact email ""
  And I create my yopmail email ""
  #--------Register New User in the PORTAL-------------
  When I open the portal URL "without int"
  Then I click on Register button
  And I should see the create your account page in the Portal
  And I enter the Full Name ""
  And I enter the email ""
  And I enter the password ""
  When I click on continue button to register the user
  Then I enter the company name "QA Team"
  And I accept the terms
  When I click on create your account button in the Portal
  Then I should see the created account page
  # ── Check email --------
  When I Check the email for "" with username ""
  Then I should receive an email with text "Thank you for signing up! We'd like to take a moment of your time to confirm your email address by clicking the button below" in the body for shipment ""
  When I confirm the email
  Then I should see confirmation successfull
  #----------Verify notification in the Hub-------------
  Given I login to Hub as user "without Int"
  Then the login dashboard should be visible
  And I verify the notification on the Dashboard page
  
