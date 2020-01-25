# IdentityServerPlus
A bloody massive extension on top of IdentityServer4 that makes it actualy usefull to the lazy programer.
Primary Goals:
 - Plugable
 - Easy to setup
 - Scalable and Relabile
 - Secure
 - Faster than the crap that exsists today
  

# Status: In Dev
This project is still heavly in development and will have breaking changes. Please see the project plan below to see what still needs to be implemented.

# TODO
Check out [The current project kanban for an up to date list](https://github.com/coman3/IdentityServerPlus/projects)

Just a quick general gist of what needs to happen still (not including the done stuff), with expanded info below.
Short term:
- [ ] Cleanup Plugin system for more flexability (interface everything)
- [ ] Implement other external providers and provide config options for each plugin
- [ ] Implement external provider mapping system through plugins
- [ ] Add support for any database type through plugins

Long term:
- [ ] Add support for custom controllers through plugins
- [ ] Add support for themes through plugins
- [ ] Add support for login stages through plugins

Unknown term: 
- [ ] Create a Manage Account system for the end user
- [ ] Implement linking of external accounts to an existing account
- [ ] Implement a User Management API, Event Management API, and or Metrics API

### Plugin System
Im mostly looking implementing a bunch of interfaces such as `IServicePlugin`, `IAppPlugin`, `IAuthenticationPlugin`, `IIdentityPlugin` which relate to `ConfigureServices`, `Configure (app)`, `AddAuthentication`, `AddIdentityServer` respectively.

Also need to ensure that plugins have a 'base type' which defines the major thing its gonna do, for example `MongoDBConnector` or `SQLConnector` would be a `DatabaseConnector` and `MicrosoftLoginProvider` would be an `ExternalProvider`.

### External Providers
All of these providers will have:
- [ ] Support for mapping information from their claims to the current user model.
- [ ] Support for saving the AccessTokens received (if capable) to allow the server to perform actions on their behalf.
      
      These tokens will be accessable via a resource claim when authenticating to the server to allow resource servers to use them on the users behalf as well
      This is mostly for Enterprise use cases, such as Office365 directory edits, but can also be used to keep the information of the user up to date.

Social and Enterprise:
 - [ ] Microsoft & Office365
 - [ ] Facebook
 - [ ] Twitter
 - [ ] Github
 - [ ] Paypal
 - [ ] Google & Gsuite
 Misc: 
 - [ ] Any OpenIDConnect
 - [ ] Any OAuth2.0
