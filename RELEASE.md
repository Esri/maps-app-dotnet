# Release notes

## 1.0.3

- Updated to ArcGIS Runtime 100.8.
- Updated Xamarin.Forms dependency to 4.5 and UWP to 6.2.10.
- Updated UWP minimum platform version from 16299 to 17134.
- Bug fixes and quality improvements:
    - Adds Y offset to pin symbols, so that they are anchored at the pin point, rather than the symbol's center. This fixes the appearance of pins 'floating' as you zoom in and out.
    - Improves the layout of the basemap and map list pages.
    - Adds margins around the search bar in the UWP version of the app, to match Android and iOS.
    - Fixes issue where the map would stop rendering after returning from the basemap picker and map picker pages on UWP.
    - Fixes issue where the menu fails to disappear when a search is started.
    - Fixes issue where routing would sometimes fail to work on iOS, Android. Note that with the workaround, the user may occasionally need to authenticate twice.

## 1.0.2

- Updated to ArcGIS Runtime 100.7.

## 1.0.1

- Comprehensive [app documentation](/documentation/index.md) from the ArcGIS for Developers site.
