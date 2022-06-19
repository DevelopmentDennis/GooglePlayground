using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GooglePlaces.Models.Responses
{
    public class PlaceDetailsResponse
    {
        [JsonPropertyName("html_attributions")]
        public IEnumerable<object> HtmlAttributions { get; set; }

        [JsonPropertyName("result")]
        public PlaceDetailsResponseResult Result { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } // check status transformation to enum
    }

    public class PlaceDetailsResponseResult
    {
        [JsonPropertyName("business_status")]
        public string BusinessStatus { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("opening_hours")]
        public OpenHours OpeningHours { get; set; }

    }

    public class OpenHours
    {
        [JsonPropertyName("opening_hours")]
        public bool OpenNow { get; set; }

        [JsonPropertyName("periods")]
        public IEnumerable<OpenPeriods> periods { get; set; }
    }

    public class OpenPeriods
    {
        [JsonPropertyName("close")]
        public OpenPeriodDetails Close { get; set; }

        [JsonPropertyName("open")]
        public OpenPeriodDetails Open { get; set; }
    }

    public class OpenPeriodDetails
    {
        [JsonPropertyName("day")]
        public int Day { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; }
    }

    public enum PlacesDetailsStatus
    {
        OK,
        ZERO_RESULTS,
        NOT_FOUND,
        INVALID_REQUEST,
        OVER_QUERY_LIMIT,
        REQUEST_DENIED,
        UNKNOWN_ERROR
    }

    public enum PlaceBusinessStatus
    {
        OPERATIONAL,
        CLOSED_TEMPORARILY,
        CLOSED_PERMANENTLY,
        UNKNOWN_ERROR
    }
}