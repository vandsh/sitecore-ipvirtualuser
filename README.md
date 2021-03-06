### What is it? ###
Passive authentication into Sitecore, driven by IPaddress.  Definitions of exact IP Addresses or Address Ranges to allow for a virtual authentication into Sitecore.

### What does it do? ###
- A Pipeline `InternalSecurityCheck` checks the current requesting IP address, if it matches a definition, it adds the Internal Access role to the current user.
- A `z.InternalSecurityCheck.config` config file to allow you to point to the exact item and database you want to use for the check.
- A `/sitecore/templates/IPVirtualUser/Internal Access Setting Item` item to define your desired settings.

### What do I need for it? ###

- Sitecore and the package at a minimum.  Has been tested on 8.1, 8.2, and 9

### What do I need if for? ###

- Use cases I have seen a few times is point to point VPNs where users are given a very limited set of permissions within Sitecore without a proper IdM in place.

### How do I use it ###
Should be as easy as:

1. Install the package in `Sitecore Packages`  _OR_

1. Clone, download, whatever, just get these files to your computer

1. Bring the source code into your project

1. Install the package to _at least_ get the Items.

1. Start defining your Internal Access ranges

Screenshot: 
![alt text](https://github.com/vandsh/sitecore-ipvirtualuser/raw/master/ipvirtualuserScreenshot.png "Internal Access Item Screenshot")