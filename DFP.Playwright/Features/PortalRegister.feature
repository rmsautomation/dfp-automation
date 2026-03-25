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
  
@1275 @INT @login
Scenario:Portal - New User registration_1275
#------Create YOPMAIL user----------
  When I go to yopmail URL
  Then I store the now var
  And I store the new contact email ""
  And I create my yopmail email ""
  #--------Register New User in the PORTAL-------------
  When I open the portal URL "with int"
  Then I click on Register button
  And I should see the create your account page in the Portal
  And I enter the username ""
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
  Given I login to Hub as user "aylindfpstagmagauto@yopmail.com" 
  Then the login dashboard should be visible
  And I verify the notification on the Dashboard page
  #----------Approve the user in the Hub-----------------
  And I select the created user "Register" in the Hub to approve the access
  #And I should see the section header "Portal User" in the Hub
  And I click on search icon to search the Entity
  When I enter the Entity "Automation"
  Then I click on search button in the Entity page
  And I select the entity "automation"
  And I click on Continue button in the entity Page
  And I click on approve access button
  And I should see the section header "Portal Users" in the Hub
#----------Verify Login in the PORTAL-----------------
  When I open the portal URL "with int"
  And I should see the login page
  When I enter the created username "" in the Portal
  And I enter the password "" in the Portal
  And click on Sign in button
  Then the login dashboard should be visible
  #Delete Contact Magaya

@580 @INT @login
Scenario:Integration - Authentication_580
#------Create YOPMAIL user----------
  When I go to yopmail URL
  Then I store the now var
  And I store the new contact email ""
  And I create my yopmail email ""
#Create Contact (without email) Parent Automation in Magaya guardar username en VAR para usar despues
#A ese contact dar permisos de Livetrack con USERNAME:NOW VAR Y PASS:testingprod123.
When I open the portal URL "with int"
And I should see the login page
When I enter the created username "" in the Portal
And I enter the password "" in the Portal
And click on Sign in button
#-----Adding Email Via Portal-----------------
And I provide an email address "" first time
And I confirm the provide  email address "" second time
Then I should see the created account page
# ── Check email --------
  When I Check the email for "" with username ""
  Then I should receive an email with text "Thank you for signing up! We'd like to take a moment of your time to confirm your email address by clicking the button below" in the body for shipment ""
  When I confirm the email
  Then I should see confirmation successfull
  #----------Verify Login in the PORTAL-----------------
  When I open the portal URL "with int"
  And I should see the login page
  When I enter the created username "" in the Portal
  And I enter the password "" in the Portal
  And click on Sign in button
  And I should see Welcome text "Welcome to our new Digital Freight Portal"
  #Delete Contact Magaya

@329 @INT @login
Scenario:New Magaya Customer User-Login in QWYK Portal (using a non-existing email in the personal information)_329
#In Magaya, create a Customer VAR NOW y PASS del ENV
#------Create YOPMAIL user----------
  When I go to yopmail URL
  Then I store the now var
  And I store the new contact email ""
  And I create my yopmail email ""
  #-----Login in the Portal------------
  When I open the portal URL "with int"
  And I should see the login page
  When I enter the created username "" in the Portal
  And I enter the password "" in the Portal
  And click on Sign in button
  #----Complete Account Details-----------
  And I should see complete your account page 
  And I enter the username "" to complete my account
  And I enter the password "" to complete my account
  And I confirm the password "" to complete my account
  And I enter the first name "" to complete my account
  And I enter the last name "Last" to complete my account
  And I enter the email "" to complete my account
  When I click on continue button to register the user
  Then I should see the created account page
  # ── Check email --------
  When I Check the email for "" with username ""
  Then I should receive an email with text "Thank you for signing up! We'd like to take a moment of your time to confirm your email address by clicking the button below" in the body for shipment ""
  When I confirm the email
  Then I should see confirmation successfull
  #----------Verify Login in the PORTAL-----------------
  When I open the portal URL "with int"
  And I should see the login page
  When I enter the created username "" in the Portal
  And I enter the password "" in the Portal
  And click on Sign in button
  And I should see Welcome text "Welcome to our new Digital Freight Portal"
#Delete Customer in Magaya delete livetrack Acces first
