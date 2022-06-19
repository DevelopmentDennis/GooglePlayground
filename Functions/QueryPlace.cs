using System;
using GooglePlaces.Models.Options;
using GooglePlaces.Models.Responses;
using GooglePlaces.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GooglePlaces.Functions
{
    public class QueryPlace
    {
        private readonly ILogger<QueryPlace> _logger;
        private readonly GoogleMapsPlacesApiOptions _options;
        private readonly IEmailService _emailService;
        private readonly IGoogleMapsService _googleMapsService;

        // Constants
        private const int NUMBER_OF_DAYS_IN_WEEK = 7;
        private const string OPENING_TIME = "0900";
        private const string CLOSING_TIME = "2000";

        public QueryPlace(IOptions<GoogleMapsPlacesApiOptions> options, ILogger<QueryPlace> logger, IEmailService emailService, IGoogleMapsService googleMapsService)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _googleMapsService = googleMapsService;
        }

        // Run this function every 15 minutes between 5am and 11 pm
        [Function(nameof(QueryPlace))]
        public async Task Run([TimerTrigger("0 */15 5-23 * * *")] TimerInfo timer, ILogger log)
        {
            if (string.IsNullOrEmpty(_options.ApiKey))
            {
                throw new ArgumentNullException($"Options not set: {nameof(_options.ApiKey)}");
            }
            if (string.IsNullOrEmpty(_options.PlaceId))
            {
                throw new ArgumentNullException($"Options not set: {nameof(_options.PlaceId)}");
            }

            var details = await _googleMapsService.GetPlaceDetailsAsync();

            if (details == null)
            {
                throw new ArgumentException("place details empty");
            }

            var status = _googleMapsService.GetPlaceStatus(details);

            if (status == PlacesDetailsStatus.NOT_FOUND || status == PlacesDetailsStatus.ZERO_RESULTS || status == PlacesDetailsStatus.UNKNOWN_ERROR)
            {
                _logger.LogError($"Place deleted with status {status}");
                return;
            }

            if (IsPlaceOperational(details))
            {
                await _emailService.SendEmail();
                return;
            }

            if (IsPlaceOpen(details))
            {
                await _emailService.SendEmail("Die Öffnungszeiten des Spielplatzes wurden angepasst");
                return;
            }


            _logger.LogInformation($"Function exectuted at {DateTime.Now}");

        }

        private bool IsPlaceOperational(PlaceDetailsResponse placeDetails)
        {
            var businessStatus = _googleMapsService.GetPlaceBusinessStatus(placeDetails.Result);
            if (businessStatus != PlaceBusinessStatus.OPERATIONAL)
            {
                return true;
            }

            return false;
        }

        private bool IsPlaceOpen(PlaceDetailsResponse placeDetails)
        {
            if (placeDetails.Result.OpeningHours.periods.Count() != NUMBER_OF_DAYS_IN_WEEK || !placeDetails.Result.OpeningHours.periods.All(el => el.Open.Time == OPENING_TIME && el.Close.Time == CLOSING_TIME))
            {
                return true;
            }

            return false;
        }

    }
}
