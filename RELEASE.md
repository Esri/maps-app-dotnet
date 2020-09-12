# Release notes

## 1.0.6

- Added support for dark mode for Xamarin.Forms.
- Updated to latest Xamarin.Forms release.
- Migrated the Android project to use AndroidX and build using the latest Android SDK.
- Added robust support for devices with notches on Android and iOS.
- Migrated from PNG icons to font-based icons for most graphics on UWP, Android, iOS.

## 1.0.5

- Updated to ArcGIS Runtime 100.9.
- Updated to Xamarin.Forms 4.8.0.
- Moved the WPF (.NET Core) project to a separate folder to prevent possible build issues.

## 1.0.4

- Adds doc table of contents to root README.md and docs/index.md.
- Renames docs/index.md to [docs/README.md](/docs/README.md).

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
    - Fixes issue where routing would sometimes fail to work on iOS, Android.
    - Fixes an issue where the route start and end location text wasn't visible without user interaction on UWP.
    - Fixes issue where basemaps occasionally fail to load on iOS.

## 1.0.2

- Updated to ArcGIS Runtime 100.7.

## 1.0.1

- Comprehensive [app documentation](/docs/README.md) from the ArcGIS for Developers site.
