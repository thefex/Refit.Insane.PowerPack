# Refit.Insane.PowerPack
Refit Insane PowerPack is a Refit library extensions which provides attribute based cache and auto-retry features.

# Installation Guide
Install-Package Refit.Insane.PowerPack

# Changelog
v. 6.3.2.2-rc, v. 5.2.4.2-rc
- **FIXED BROKEN HttpClientDiagnosticsHandler - make sure you do not use it in production env if you are using 6.3.2.0/5.2.4.0 !!**
- Extended Refit Cache Behaviour - force cache refresh is now controlled via **[RefitCacheBehaviour]** - Default (cache not refreshed), Force Update without fallback to cache ( cache refreshed + refresh response returned), Force Update with fallback to cache ( cache refreshed + in case refresh response failed - cache returned if available, in case refresh response succeed - response returned) **Please read RefitCacheBehaviour summary (intellisense)**

v. 6.3.2.1-rc, v. 5.2.4.1-rc

Release candidate.

- Added RefitCacheLocation into [RefitCacheAttribute] - support for Akavache BlobStorage (please read akavache documentation or read attribute summary) - default IPersistedCache service.
- Added possibility to force cache refresh (even if cached version already exists ) - optional boolean on Execute method. Use-case: you have cache that never expires but you want to update it (without clearing) for ex. lists and pull to refresh. 
- Added possibility to force cache refresh based on time elapsed since last save - use new Execute method with Func<TimeSpan?, bool> parameter - nullable TimeSpan (null = cache has no entry) since last update. Use-case: you have app settings that should never be cleared (persisted) - but it should also be updated every ~24 hours.  

v. 6.3.2.0, v. 5.2.4.0

- New versioning rule = RefitVersionNumber.x - where x is PowerPack version
- Added method to retrieve cache manually via RefitCacheService.Instance
- Added comment/description into ApiAttribute ctor - regarding HttpClientHandlers
- Version bump of Akavache
- Added packages for Refit 5.2.4 & 6.3.2
- Fixed HttpClientDiagnosticsHandler - it never worked with Release version -.- -> if debugger attached it will trace request/response/performance etc..

v. 1.2.4
- Updated refit/akavache/polly to latest stable.
- Nuget Package supports NetStandard 2.0 again.
- Nuget Package supports net6.0 and fixed net5.0 support (it might be broken in 1.2.3).

v. 1.2.3
- Updated refit/akavache/polly to most recent stable.
- Changed AppRestService methods to async - PrepareResponse, CanPrepareResponse etc. (breaking change)
- Package supports net5.0 and netstandard2.1 now.

v. 1.1.1

- NetStandard 2.0 support

v. 1.0.3

- "User friendly" (with suggestion) instead of NullReferenceException is thrown when BaseUri/ApiDefinition Uri is not set #8
- Support for setting global timeout (when ApiDefinitionAttribute is not set on interface) in BaseApiConfiguration class #7
- Fixed bugs which prevented handling ApiException (custom status codes) when inheriting from RefitRestService #6


# Documentation
1. Check Sample app which is available in this repository. 
2. Read changelog above!
3. Read presentation I have created for Cracow #Xamarines - Xamarines.com
https://github.com/thefex/Refit.Insane.PowerPack/blob/master/refit_presentation.pdf

4. In order to use ApiDefinitionAttribute you need to either:
* Attach attributes to each refit API interface
* Attach attribute to only specific API interfaces but also set BaseApiConfiguration.ApiUri which will be used as base uri for all interfaces without ApiDefinition attribute

# Acknowledgment
Thanks to Artur Malendowicz (https://github.com/Immons) for implementing multiple API support (ApiDefinition)

Thanks to Jakub Kaprzyk (https://github.com/qbus00) for implementing ApiDefinition Timeout support.
