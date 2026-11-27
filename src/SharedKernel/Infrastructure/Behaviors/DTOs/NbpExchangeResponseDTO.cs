using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Payments.Infrastructure.Nbp;
public record NbpExchangeResponseDTO
(
    [property: JsonPropertyName("table")]
    string table,
    [property: JsonPropertyName("currency")]
    string currency,
    [property: JsonPropertyName("code")]
    string code,
    [property: JsonPropertyName("rates")]
    List<NbpExchangeResponseRates> rates
);

public record NbpExchangeResponseRates
(
    [property: JsonPropertyName("no")]
    string no,
    [property: JsonPropertyName("effectiveDate")]
    string effectiveDate,
    [property: JsonPropertyName("mid")]
    decimal mid
);