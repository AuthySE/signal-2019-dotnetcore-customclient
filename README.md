# Node.js - Zero to Hero:  Adding Account Security to your Stack

Simple .NET Core app adding Lookup and Verify to your stack to help identify your users and prevent fraudsters from abusing your stacks.

#### Setup and Run Instructions
- Git clone
- Open /signal-2019-dotnetcore/appsettings.json
- Open /signal-2019-dotnetcore/appsettings.Development.json
- Login to Twilio Console
- Add Twilio Account SID and Auth Token to both appsettings files
- Browse to https://www.twilio.com/console/verify/services
- Create a Verify Service.  Name it appropriately
- Add new Verify Service SID to both appsettings files
- Browse to https://www.twilio.com/console/authy/applications
- Create an Authy application
- Copy Authy API Key to both appsettings files
- ```dotnet build```
- ```dotnet run```
- Browse to https://localhost:5001


#### Verification Flow
- Prompt user for country code and phone number.
- Click to Lookup Phone Number
- Execute a Lookup.  This detects the line-type (voip,mobile,landline)
- Request Verification Code.
    - If mobile, send OTP via SMS.
    - If landline, send OTP via voice call.
    - If VOIP, don't allow registration.
- If appropriate line-type, send them a Verification code.
- Prompt user for OTP.
- Verify OTP.
- Success!

#### Account Creation Flow
- Add Username, Email address, Country Code and Phone number
- Add password.  
    - Note, we override the password on the backend for demo purposes
- Click Lookup
    - Note, only mobile numbers are allowed to register.  No VOIP, No landlines.
- Click Register
    - Note, you can not re-use the same username.
    - Check db.json if you're having trouble.
- The backend will register the user with Authy and prompt you 
- If you have Authy, the Signal 2019 Authy app will be added to your Authy application.
    - Use TOTP or Push for 2FA verification with Authy
    - Use SMS or Voice if you do not have the Authy app
- Once you verify, you've now added 2FA to your user account.

#### Login with 2FA
- Using the username and password created in the Account Creation Flow, login.
- Select your 2FA mechanism.
- Provide 2FA.

#### Database
- We've an in-memory database for this use case
- Database entries are only added when creating an account.
- Authy ID registration is for the stretch goal of adding Authy

Special thanks to [Alex Vance](www.linkedin.com/in/vancealex) @ DOTSLN for .NET Core expertise!
