using GooglePlaces.Models.Options;
using GooglePlaces.Models.Responses;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace GooglePlaces.Services
{
    public interface IGoogleMapsService
    {
        public Task<PlaceDetailsResponse?> GetPlaceDetailsAsync();

        public PlacesDetailsStatus GetPlaceStatus(PlaceDetailsResponse placesDetails);

        public PlaceBusinessStatus GetPlaceBusinessStatus(PlaceDetailsResponseResult placesDetails);

    }

    public class GoogleMapsService : IGoogleMapsService
    {
        private readonly HttpClient _httpClient;
        private readonly GoogleMapsPlacesApiOptions _options;
        private readonly ILogger<GoogleMapsService> _logger;
        private const string GoogleMapsBaseUri = "https://maps.googleapis.com/maps/api/place/details/json";


        public GoogleMapsService(HttpClient httpClient, IOptions<GoogleMapsPlacesApiOptions> options, ILogger<GoogleMapsService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        public async Task<PlaceDetailsResponse?> GetPlaceDetailsAsync()
        {
            const string fieldsToRequest = "name,business_status,opening_hours,formatted_address,geometry,url";

            var query = new Dictionary<string, string>()
            {
                ["place_id"] = _options.PlaceId,
                ["fields"] = fieldsToRequest,
                ["key"] = _options.ApiKey
            };

            var uri = QueryHelpers.AddQueryString(GoogleMapsBaseUri, query);

            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            try
            {
                var data = JsonSerializer.Deserialize<PlaceDetailsResponse>(response.Content.ReadAsStream());
                return data;
            }
            catch (ArgumentException)
            {
                _logger.LogError("Response was empty");
            }
            catch (JsonException e)
            {
                _logger.LogError("Json error ", e.Message);
            }
            return null;
        }

        public PlacesDetailsStatus GetPlaceStatus(PlaceDetailsResponse placeDetails)
        {
            var result = Enum.TryParse(placeDetails.Status, out PlacesDetailsStatus status);
            if (result == true)
            {
                return status;
            }
            _logger.LogError($"Could not parse enum {nameof(PlacesDetailsStatus)}");
            return PlacesDetailsStatus.UNKNOWN_ERROR;

        }

        public PlaceBusinessStatus GetPlaceBusinessStatus(PlaceDetailsResponseResult placesDetails)
        {
            var result = Enum.TryParse(placesDetails.BusinessStatus, out PlaceBusinessStatus status);
            if (result == true)
            {
                return status;
            }
            _logger.LogError($"Could not parse enum {nameof(PlaceBusinessStatus)}");
            return PlaceBusinessStatus.UNKNOWN_ERROR;
        }
    }
}