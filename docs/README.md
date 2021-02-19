# Contents

<!-- MDTOC maxdepth:6 firsth1:0 numbering:0 flatten:0 bullets:1 updateOnSave:1 -->

- [Description](#description)   
   - [Platforms supported](#platforms-supported)   
   - [Functionality showcased](#functionality-showcased)   
- [Overview](#overview)   
- [Place search & geocoding](#place-search-geocoding)   
- [Place suggestions](#place-suggestions)   
- [Searching from a suggestion](#searching-from-a-suggestion)   
- [Routing](#routing)   
- [Turn-by-turn directions (direction maneuvers)](#turn-by-turn-directions-direction-maneuvers)   
- [Authentication](#authentication)   
- [Switching basemaps](#switching-basemaps)   
- [Using web maps](#using-web-maps)   
- [Architecture](#architecture)   
   - [Solution overview](#solution-overview)   
   - [Model-View-ViewModel pattern](#model-view-viewmodel-pattern)   
   - [Xamarin and WPF - platform differences](#xamarin-and-wpf-platform-differences)   
- [Configuration and customization](#configuration-and-customization)   

<!-- /MDTOC -->
---

## Description

The Maps App suite showcases best practices in building cross platform applications with the ArcGIS Runtime SDK for .NET. The suite is built using Xamarin Forms and WPF. Comprised of four applications for four separate platforms, it is designed with maximized code sharing in mind.

### Platforms supported

- iOS (iPhone and iPad)
- Android (phones and tablets)
- WPF (devices running Windows 7 and above)
- UWP (devices running Windows 10)

Get your organization's authoritative map data into the hands of your workers with this suite of applications. The applications can be easily customized to include a web map from your [ArcGIS Online organization](https://doc.arcgis.com/en/arcgis-online/reference/what-is-agol.htm) or you can use the [Living Atlas](https://doc.arcgis.com/en/living-atlas/item/?itemId=26888b0c21a44eb1ba2f26d1eb7981fe) as a starting place. The Maps App also includes examples of place searching and routing capabilities using either ArcGIS Online's powerful services or your own services. It can also leverage your organization's configured basemaps to allow users to switch to the basemap that makes the most sense for them.

### Functionality showcased

- Place Search
- Geocode addresses
- Reverse Geocode
- Turn-by-turn Directions
- Dynamically switch basemaps
- Open Web Maps
- Work with ArcGIS Online or an on-premise ArcGIS Portal
- OAuth2 authentication

Configure this app for your organization or use it to learn how to integrate similar capabilities into your own app.

## Overview

*Disclaimer: For screen real estate purposes, only Android and WPF screenshots are posted. Since the app was built with Xamarin Forms, iOS and UWP user interfaces are very similar to the Android interface, with exception of the iconography and platform specific controls.*

When the app starts, you will be presented with the default empty map. In the initial run of the app, you may be prompted to accept that the app wants to use your current location.

| Android | WPF |
|:-------:|:---:|
|<img src="/docs/images/start_screen_and.png" width="450"/>|<img src="/docs/images/start_screen_wpf.png"/>|

The default map is created inside the `MapViewModel.cs` by initializing the `Map` with a new Topographic Vector basemap.

```csharp
private Map _map = new Map(Basemap.CreateTopographicVector());

/// <summary>
/// Gets or sets the map
/// </summary>
public Map Map
{
    get
    {
        return _map;
    }

    set
    {
        _map = value;
        OnPropertyChanged();
    }
}
```

Inside the view (XAML), a `MapView` control is created and its `Map` property is bound to the `Map` property on the `MapViewModel`

**Xamarin & WPF:**

```xml
<esri:MapView x:Name="MapView" Map="{Binding Map, Source={StaticResource MapViewModel}}"/>
```

## Place search & geocoding

At the top of the screen, there is a menu button and a search bar. The search bar provides the geocoding functionality. [Geocoding](https://developers.arcgis.com/net/geocode-and-search/) lets you transform an address or a place name to a specific geographic location. Reverse geocoding lets you use a geographic location to find a description of the location, like a postal address or place name.

In the solution, the logic for geocoding is contained inside the shared `GeocodeViewModel`. First, a `LocatorTask` is defined to use the [ArcGIS World Geocoding Service](https://developers.arcgis.com/features/geocoding/). Before using the `LocatorTask`, it must be loaded. The loadable pattern is described [here](https://developers.arcgis.com/net/programming-patterns/loadable/).

```csharp
/// <summary>
/// Gets the locator
/// </summary>
private LocatorTask Locator { get; set; }

// Load locator
Locator = await LocatorTask.CreateAsync(new Uri(Configuration.GeocodeUrl));
```

The search box is bound to the `SearchText` property inside the `GeocodeViewModel`. When the text inside the search box is changed the `SearchText` property is updated and begins a new location suggestion search.

**Xamarin:**

```xml
<SearchBar x:Name="AddressSearchBar" Placeholder="Address or Place" Text="{Binding SearchText, Mode=TwoWay}"
    BindingContext="{StaticResource GeocodeViewModel}" SearchCommand="{Binding SearchCommand}" />
```

**WPF:**

```xml
<TextBox x:Name="SearchTextBox" Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}" />
<Button Command="{Binding SearchCommand}" CommandParameter="{Binding SearchText}" />
```

```csharp
/// <summary>
/// Gets or sets the search text the user has entered
/// </summary>
public string SearchText
{
    get
    {
        return _searchText;
    }

    set
    {
        if (_searchText != value)
        {
            _searchText = value;
            OnPropertyChanged();

            if (!string.IsNullOrEmpty(_searchText))
            {
                // Call method to get location suggestions
                GetLocationSuggestionsAsync(_searchText).ContinueWith((t) =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                        SuggestionsList = t.Result;
                });
            }
            else
            {
                SuggestionsList = null;
            }
        }
    }
}
```

## Place suggestions

Typing the first few letters of a place into the Maps App search box (e.g. “West End Bikes”) shows a number of suggestions near the device’s location.

| Android | WPF |
|:-------:|:---:|
|<img src="/docs/images/suggestions_and.png" width="450"/>|<img src="/docs/images/suggestions_wpf.png"/>|

This is handled inside the `GeocodeViewModel`. When the search text property is updated the `GetLocationSuggestionsAsync` method is called. A check is first performed to ensure the locator supports suggestions. When [creating your own locator](https://pro.arcgis.com/en/pro-app/tool-reference/geocoding/create-address-locator.htm), you can enable suggestions and thus be able to take advantage of this functionality.

```csharp
/// <summary>
/// Gets list of suggested locations from the locator based on user input
/// </summary>
private async Task GetLocationSuggestionsAsync(string userInput)
{
    // make sure input is defined and suggestions are supported
    if (Locator?.LocatorInfo?.SupportsSuggestions ?? false && !string.IsNullOrEmpty(userInput))
    {
        // restrict the search to return no more than 10 suggestions
        // set preferred search location to location around current map center
        var suggestParams = new SuggestParameters { MaxResults = 10, PreferredSearchLocation = AreaOfInterest?.TargetGeometry as MapPoint, };

        // get suggestions for the text provided by the user
        var suggestions = await Locator.SuggestAsync(userInput, suggestParams);
        var s = new ObservableCollection<string>();
        foreach (var suggestion in suggestions)
        {
            s.Add(suggestion.Label);
        }
        SuggestionsList = s;
    }
}
```

## Searching from a suggestion

Once a suggestion in the list has been selected by the user, the suggested address is geocoded using the `LocatorTask.GeocodeAsync` function. Along with the address, specific geocoding parameters can be set to tune the results. For example, in the Maps App, we set the `PreferredSearchLocation` property to prioritize results closer to the center of the map.

```csharp
/// <summary>
/// Get location searched by user from the locator
/// </summary>
private async Task<GeocodeResult> GetSearchedLocationAsync(string geocodeAddress)
{
    // Locate the searched feature
    var geocodeParameters = new GeocodeParameters
    {
        MaxResults = 1,
        PreferredSearchLocation = AreaOfInterest?.TargetGeometry as MapPoint,
    };

    // return the first match
    var matches = await Locator.GeocodeAsync(geocodeAddress, geocodeParameters);
    return matches.FirstOrDefault();
}
```

| Android | WPF |
|:-------:|:---:|
|<img src="/docs/images/geocode_and.png" width="450"/>|<img src="/docs/images/geocode_wpf.png"/>|

## Routing

Getting routing directions in the Maps App is easy with the Runtime SDK and the [ArcGIS World Routing Service](https://developers.arcgis.com/features/directions/). You can also [customize](https://doc.arcgis.com/en/arcgis-online/administer/configure-services.htm#ESRI_SECTION1_567C344D5DEE444988CA2FE5193F3CAD) your routing service for your organization.

Navigating from point to point in the Maps App is enabled by first geocoding or reverse geocoding a location. You can then get directions to that location from the current GPS location (or if GPS is disabled, from a location of your choice). In the Maps App, routing requires you to provide credentials to your Portal or ArcGIS Online organization. See the [Authentication](#authentication) section for more details.

Routing in the app is handled inside the shared `RouteViewModel` which creates and loads a `RouteTask`.

```csharp
/// <summary>
/// Gets the router for the map
/// </summary>
internal RouteTask Router { get; set; }

private async Task CreateRouteTask()
{
    ...
    Router = await RouteTask.CreateAsync(new Uri(Configuration.RouteUrl));
    ...
}
```

The `RouteParameters` object is used to specify input parameters such as start point and end point for the route. You can instantiate a new `RouteParameters` object by using the `CreateDefaultParametersAsync` function on the `RouteTask` instance. This retrieves the appropriate default settings for the route service. Then add the stops and request route directions. After setting the parameters, call `SolveRouteAsync` to request the route from the server.

```csharp
/// <summary>
/// Generates route from the geocoded locations
/// </summary>
private async Task GetRouteAsync()
{
    // set the route parameters
    var routeParams = await Router.CreateDefaultParametersAsync();
    routeParams.ReturnDirections = true;
    routeParams.ReturnRoutes = true;

    // add route stops as parameters
        routeParams.SetStops(new List<Stop>() { new Stop(FromPlace),
                                                new Stop(ToPlace) });
        Route = await Router.SolveRouteAsync(routeParams);
    }
}
```

| Android | WPF |
|:-------:|:---:|
|<img src="/docs/images/route_and.png" width="450"/>|<img src="/docs/images/route_wpf.png"/>|

## Turn-by-turn directions (direction maneuvers)

After a `RouteResult` has been retrieved, the list of direction maneuvers is available as a list on each `Route`. In WPF, the direction maneuvers are displayed on the same page. Due to the smaller screen size in Android, iOS, and UWP the turn-by-turn directions are shown on the `TurnByTurnDirections` view when you tap the Direction Maneuvers icon. In both cases, the direction maneuvers are set in the shared `RouteViewModel`.

| Android | WPF |
|:-------:|:---:|
|<img src="/docs/images/turn_by_turn_and.png" width="450"/>|<img src="/docs/images/route_wpf.png"/>|

```csharp
/// <summary>
/// Gets or sets the turn-by-turn directions for the returned route
/// </summary>
public IReadOnlyList<DirectionManeuver> DirectionManeuvers
{
    get
    {
        return _directionManeuvers;
    }
    set
    {
        if (_directionManeuvers != value)
        {
            _directionManeuvers = value;
            OnPropertyChanged();
        }
    }
}

private async Task GetRouteAsync()
{
    ...
    // Set turn-by-turn directions
    DirectionManeuvers = Route.Routes.FirstOrDefault()?.DirectionManeuvers;
    ...
}
```

Each of the items in the `DirectionManeuvers` property inside the `RouteViewModel` is shown as an item in a `ListView`. An `ItemTemplate` is defined to show the image and text of each direction maneuver.

**Xamarin:**

```xml
 <ListView x:Name="DirectionsListView" ItemsSource="{Binding DirectionManeuvers}" CachingStrategy="RecycleElement">
    <ListView.ItemTemplate>
        <DataTemplate>
            <ImageCell ImageSource="{Binding ManeuverType, Converter={StaticResource DirectionManeuverToImagePathConverter}}" Text="{Binding DirectionText}" />
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

**WPF:**

```xml
<ListView ItemsSource="{Binding DirectionManeuvers}">
    <ListView.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal" >
                <Image Source="{Binding ManeuverType, Converter={StaticResource DirectionManeuverToImagePathConverter}}" />
                <TextBlock  Text="{Binding DirectionText}" />
            </StackPanel>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

## Authentication

The Maps App leverages the ArcGIS [identity](https://developers.arcgis.com/authentication/) model to provide access to resources via the the [named user login pattern](https://developers.arcgis.com/documentation/core-concepts/security-and-authentication/#named-user-login). During the routing workflow, or if clicking the `Sign In` button in the menu, the app prompts you for your organization’s ArcGIS Online credentials. The ArcGIS Runtime SDKs provide a simple to use API for dealing with ArcGIS logins.

The process of accessing token secured services with a challenge handler is illustrated in the following diagram.

<img src="/docs/images/identity.png" />

1. A request is made to a secured resource.
2. The portal responds with an unauthorized access error.
3. A challenge handler associated with the authentication manager is asked to provide a credential for the portal.
4. A UI displays and the user is prompted to enter a user name and password.
5. If the user is successfully authenticated, a credential (token) is included in requests to the secured service.
6. The authentication manager stores the credential for this portal and all requests for secured content include the token in the request.

For an application to use this pattern, follow these [guides](https://developers.arcgis.com/net/security-and-authentication/) to register your app. The `AuthenticationManager` provided by the ArcGIS Runtime SDK abstracts much of the authentication logic for you. In the Maps App, the `AuthenticationManager` is configured to prompt the user for credentials.

| Android | WPF |
|:-------:|:---:|
|<img src="/docs/images/signin_and.png" width="450"/>|<img src="/docs/images/signin_wpf.png"/>|

To set up the `AuthenticationManager` for the Maps App, we register the server info with the `AuthenticationManager` and set up the `ChallengeHandler`.

```csharp
/// <summary>
/// Set up singleton instance of Authentication manager
/// </summary>
private void UpdateAuthenticationManager()
{
    // Define the server information for ArcGIS Online
    var portalServerInfo = new ServerInfo
    {
        ServerUri = new Uri(Configuration.ArcGISOnlineUrl),
        TokenAuthenticationType = TokenAuthenticationType.OAuthImplicit,
        OAuthClientInfo = new OAuthClientInfo
        {
            ClientId = Configuration.AppClientID,
            RedirectUri = new Uri(Configuration.RedirectURL)
        },
    };

    // Register the ArcGIS Online server information with the AuthenticationManager
    AuthenticationManager.Current.RegisterServer(portalServerInfo);

#if WPF
    // In WPF, use the OAuthAuthorize class to create a new web view to show the sign-in UI
    AuthenticationManager.Current.OAuthAuthorizeHandler = new WPF.Views.OAuthAuthorize();
#endif

    // Create a new ChallengeHandler that uses a method in this class to challenge for credentials
    AuthenticationManager.Current.ChallengeHandler = new ChallengeHandler(CreateCredentialAsync);
}

```

When a challenge is issued, such as when the user has hit the Sign In button or is attempting a Route, `CreateCredentialsAsync` is called. The user is prompted to enter username and password, and if the authentication is successful, the credential is stored inside the `AuthenticationManager`. The `AuthenticatedUser` is then stored in a separate bindable property.

```csharp
/// <summary>
/// ChallengeHandler function that will be called whenever access to a secured resource is attempted
/// </summary>
private async Task<Credential> CreateCredentialAsync(CredentialRequestInfo info)
{
    // IOAuthAuthorizeHandler will challenge the user for OAuth credentials
    var credential = await AuthenticationManager.Current.GenerateCredentialAsync(new Uri(Configuration.ArcGISOnlineUrl));
    AuthenticationManager.Current.AddCredential(credential);

    // Create connection to Portal and provide credential
    var portal = await ArcGISPortal.CreateAsync(new Uri(Configuration.ArcGISOnlineUrl), credential);
    AuthenticatedUser = portal.User;
    return credential;
}
```

_A note of caution on Authentication. The implementation used in this and other open source apps is simplified and generalized to apply to an array of apps and scenarios. You should research and carefully consider their own security implementation._

## Switching basemaps

As an administrator for your ArcGIS Online Organization, you can [create and publish basemaps](https://pro.arcgis.com/en/pro-app/help/mapping/map-authoring/author-a-basemap.htm) that your users can switch between. We retrieve the basemaps inside the shared `BasemapsViewModel` and then display them in the view.

Due to sufficient screen real estate in WPF, the basemap and user map selector can be displayed in the same view. In the Xamarin apps, the basemap and user map selectors are on separate views.


```csharp
var portal = await ArcGISPortal.CreateAsync();
var items = await portal.GetBasemapsAsync();
Basemaps = items?.Select(b => b.Item).OfType<PortalItem>();
```

To use you own organization's default basemaps, pass the URL to your organization to `CreateAsync`.

Each of the items in the `Basemaps` property inside the `BasemapsViewModel` is shown as an item in a `ListView`. An `ItemTemplate` is defined to show the thumbnail and title of each basemap.

**Xamarin:**

```xml
<!-- List of basemaps for the user to select from -->
<ListView x:Name="BasemapListView" ItemsSource="{Binding Basemaps}" SelectedItem="{Binding SelectedBasemap}" ItemTapped="ListView_ItemTapped" >
    <ListView.ItemTemplate>
        <DataTemplate>
            <ImageCell ImageSource="{Binding ThumbnailUri}" Text="{Binding Title}" Detail="{Binding Snippet}"/>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

**WPF:**

```xml
<ListView  DataContext="{StaticResource BasemapsViewModel}"  ItemsSource="{Binding Basemaps}" SelectedItem="{Binding SelectedBasemap}">
    <ListView.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Vertical" >
                <Image Stretch="UniformToFill"  Source="{Binding ThumbnailUri}" />
                <TextBlock VerticalAlignment="Center" Text="{Binding Title}" />
            </StackPanel>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

| Android | WPF |
|:-------:|:---:|
|<img src="/docs/images/change_basemap_and.png" width="450"/>|<img src="/docs/images/change_basemap_wpf.png"/>|

## Using web maps

Loading and switching web maps is similar to switching basemaps in Maps App.

You can author your own web maps from ArcGIS Online or ArcGIS Pro and share them in your app via your ArcGIS Online organization. Building an app which uses a web map allows the cartography and map configuration to be completed in ArcGIS Online rather than in code. This then allows the map to change over time, without any code changes or app updates. Learn more about the benefits of developing with web maps [here](https://developers.arcgis.com/documentation/core-concepts/web-maps/). Also, learn about authoring web maps in [ArcGIS Online](https://doc.arcgis.com/en/arcgis-online/get-started/get-started-with-maps.htm) and [ArcGIS Pro](https://pro.arcgis.com/en/pro-app/help/mapping/map-authoring/author-a-web-map.htm).

Once authenticated the shared `UserItemsViewModel` retrieves the user's web maps from Portal and displays them together with descriptions and thumbnails. When the user chooses a web map, the `Map` property on the `MapViewModel` is replaced with a new `Map` object created from the web map's URL.

```csharp
// Set the item types you want the user to be able to select from.
// This does not have to be limited to web maps
private static readonly ICollection<PortalItemType> _validUserItemTypes = new PortalItemType[] { PortalItemType.WebMap};

/// <summary>
/// Loads user maps from Portal
/// </summary>
public async Task LoadUserItems()
{
    var portalUser = AuthViewModel.Instance.AuthenticatedUser?.Portal?.User as PortalUser;

    ...
    var userContent = await portalUser.GetContentAsync();

    UserItems = new ObservableCollection<PortalItem>();
    foreach (var item in userContent.Items)
    {
        if (_validUserItemTypes.Contains(item.Type))
            UserItems.Add(item);
    }
    ...
}
```

| Android | WPF |
|:-------:|:---:|
|<img src="/docs/images/change_user_map_and.png" width="450"/>|<img src="/docs/images/change_user_map_wpf.png"/>|

## Architecture

### Solution overview

The four applications that comprise the Maps App suite are all contained inside one solution. The diagram below represents how the solution is structured to make best use of shared logic between the apps:

<img src="/docs/images/xamarin_diagram.png" />

The `MapsApp.iOS`, `MapsApp.Android` and `MapsApp.UWP` projects belong to the Xamarin Forms part of the solution. The code contained inside the `MapsApp.Xamarin.Shared` project is common to the three applications. The three apps share UI components and some Xamarin specific logic.

The `MapsApp.WPF` project has its own UI  which is independent of the Xamarin Forms UI.

All four apps share the logic contained inside the `MapsApp.Shared` project.

### Model-View-ViewModel pattern

The app uses the MVVM (Model-View-ViewModel) pattern. It makes heavy use of data binding to separate the presentation layer (the view) from the logic of the application (the view-model and the model), thus facilitating code sharing. Generally speaking, in .NET, business logic is commonly cross-platform compatible, whereas the presentation layer is often not. Learn more about MVVM [here](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel).

The application is structured to demonstrate separation of concerns using the Model-View-ViewModel (MVVM) architecture. The views have minimal logic in the code behind file and it is all view related. According to MVVM principles, the model should not know about the view model and the view model should not know about the view. The way the view model communicates with the view is through bindable properties. A good example of this is the `Map` property on the `MapView` control.

```csharp
/// <summary>
/// Gets or sets the map
/// </summary>
public Map Map
{
    get
    {
        return _map;
    }

    set
    {
        _map = value;
        OnPropertyChanged();
    }
}
```

The map is bound to the Map property on the MapView control in XAML

**Xamarin & WPF:**

```xml
<esri:MapView x:Name="MapView" Map="{Binding Map, Source={StaticResource MapViewModel}}"/>
```

There are cases however when the properties that need to be set are not bindable. A good example in the Maps App is the `Graphic` element. It has a `Geometry` and `Symbol`, but they are not bindable because they are not dependency properties. But they are UI elements, so setting them inside the view model would not be appropriate as it would cause the view logic to "bleed" into the view model. So setting these properties is done in the code behind of the view.

```csharp
geocodeViewModel.PropertyChanged += (o, e) =>
{
    switch (e.PropertyName)
    {
        case nameof(GeocodeViewModel.Place):
            {
                var graphicsOverlay = MapView.GraphicsOverlays["PlacesOverlay"];
                graphicsOverlay?.Graphics.Clear();

                GeocodeResult place = geocodeViewModel.Place;

                var graphic = new Graphic(geocodeViewModel.Place.DisplayLocation, new PictureMarkerSymbol(new RuntimeImage(imageData)));
                graphicsOverlay?.Graphics.Add(graphic);

                break;
            }
    }
}
```

Another option when setting non-bindable properties is through the use of [attached properties](https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/attached-properties-overview). In Maps App, we have created an extension to the GeoView which contains a `HoldingLocation` attached property. The implementation code can be found below. When the `GeoViewHolding` event fires, the attached property is set and can be bound to from the view model.

```csharp
/// <summary>
/// Creates a HoldingLocation property
/// </summary>
public static readonly DependencyProperty HoldingLocationProperty = BindingFramework.DependencyProperty.Register("HoldingLocation", typeof(MapPoint), typeof(HoldingLocationController),null);

/// <summary>
/// Invoked when the GeoViewHolding event fires
/// </summary>
private void GeoView_GeoViewHolding(object sender, GeoViewInputEventArgs e)
{
    if (!_isOnHoldingLocationChangedExecuting)
    {
        _isGeoViewHoldingEventFiring = true;
        // get the Location the user is holding from the event args
        HoldingLocation = e.Location;
        _isGeoViewHoldingEventFiring = false;
    }
}
```

**Xamarin & WPF:**

```xml
<utils:GeoViewExtensions.HoldingLocationController>
    <utils:HoldingLocationController HoldingLocation="{Binding ReverseGeocodeInputLocation, Mode=TwoWay, Source={StaticResource GeocodeViewModel}}"/>
</utils:GeoViewExtensions.HoldingLocationController>
```

### Xamarin and WPF - platform differences

There are significant differences between Xamarin Forms and WPF and not just in the XAML syntax. A detailed explanation of the differences can be found [here](https://developer.xamarin.com/guides/cross-platform/windows/desktop/controls/wpf/). In the Maps App, one way we overcome such differences is by using [shims](https://en.wikipedia.org/wiki/Shim_(computing)).

For example, we register a Xamarin `BindableProperty` as a `DependencyProperty` and provide the equivalent methods and properties.

```csharp
/// <summary>
/// Provides members for registering BindableProperty instances
/// </summary>
internal class DependencyProperty
{
    /// <summary>
    /// Registers a BindableProperty for the specified type
    /// </summary>
    internal static BindableProperty Register(string propertyName, Type returnType, Type declaringType, PropertyMetadata metadata)
    {
        BindableProperty prop = null;

        if (metadata != null)
        {
            prop = BindableProperty.Create(propertyName, returnType, declaringType,
              metadata.DefaultValue, BindingMode.OneWay, null, metadata.BindablePropertyChanged);
            metadata.Property = prop;
        }
        else
        {
            prop = BindableProperty.Create(propertyName, returnType, declaringType,
              GetDefaultValue(returnType), BindingMode.OneWay, null, null);
        }

        return prop;
    }
}
```

Then in the shared code, we define the `BindableProperty` using aliases

```csharp
#if __IOS__ || __ANDROID__ || NETFX_CORE
using DependencyProperty = Xamarin.Forms.BindableProperty;
#endif
```

## Configuration and customization

The Maps App can be easily configured. The `Configuration` file contains all the information you need to modify the app to work with your organization's data.

```csharp
// URL of the server to authenticate with (ArcGIS Online)
public const string ArcGISOnlineUrl = @"https://www.arcgis.com/sharing/rest";

// Client ID for the app registered with the server
public const string AppClientID = "YourClientID";

// Redirect URL after a successful authorization (configured for the Maps App application)
public const string RedirectURL = @"https://developers.arcgis.com";

// Url used for the geocode service
public const string GeocodeUrl = @"https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer";

// Url used for the routing service
public const string RouteUrl = @"https://route.arcgis.com/arcgis/rest/services/World/Route/NAServer/Route_World";
```

If changing the above mentioned values does not provide you with all the needed functionality, you are welcome to customize the app by modifying the source code and making the app your own. And if you happen to fix a bug, or add an enhancement that you think others may want, please submit a pull request and we will happily review it and incorporate it into the main app.
