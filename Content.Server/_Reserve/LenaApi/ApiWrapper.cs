// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Content.Server._Reserve.LenaApi;

public sealed partial class ApiWrapper
{
    private HttpClient _httpClient;
    private readonly Func<string> _apiKeyProvider;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ApiWrapper(string baseUri, Func<string> apiKeyProvider)
    {
        _apiKeyProvider = apiKeyProvider;
        _httpClient = new HttpClient(new ApiKeyHandler(apiKeyProvider));
        _httpClient.BaseAddress = new Uri(baseUri);

        _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy =  JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };
        _jsonSerializerOptions.Converters.Add(new OptionalJsonConverter<string>());
        _jsonSerializerOptions.Converters.Add(new OptionalJsonConverter<int?>());
    }

    public void SetBaseUri(string baseUri)
    {
        var old = _httpClient;
        _httpClient = new HttpClient(new ApiKeyHandler(_apiKeyProvider));
        _httpClient.BaseAddress = new Uri(baseUri);
        old.Dispose();
    }

    private async Task<Result<T>> Send<T>(
        Func<Task<HttpResponseMessage>> request
    )
    {
        try
        {
            var response = await request();
            var content = await response.Content.ReadAsStringAsync();

            switch (response.StatusCode)
            {
                case HttpStatusCode.UnprocessableContent:
                    return Result<T>.Failure(new ValidationError(content));
                case HttpStatusCode.Unauthorized:
                    return Result<T>.Failure(new UnauthorizedError());
                case HttpStatusCode.NotFound:
                    return Result<T>.Failure(new NotFoundError());
                case HttpStatusCode.Forbidden:
                    return Result<T>.Failure(new ForbiddenError());
                default:
                    if (!response.IsSuccessStatusCode)
                        return Result<T>.Failure(new UnknownError((int) response.StatusCode, content));
                    break;
            }

            var deserializedResponse = JsonSerializer.Deserialize<T>(content, _jsonSerializerOptions);

            return Result<T>.Success(deserializedResponse);
        }
        catch (TaskCanceledException)
        {
            return Result<T>.Failure(new NetworkError("Timeout"));
        }
        catch (HttpRequestException e)
        {
            return Result<T>.Failure(new NetworkError(e.Message));
        }
    }

    public readonly struct Optional<T>
    {
        public bool Present { get; }
        public T? Value { get; }

        public Optional(T? value)
        {
            Present = true;
            Value = value;
        }

        public static implicit operator Optional<T>(T? value) => new(value);
    }

    private sealed class OptionalJsonConverter<T> : JsonConverter<Optional<T>>
    {
        public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return new Optional<T>(default);
            return JsonSerializer.Deserialize<Optional<T>>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }

    public sealed class ApiKeyHandler : DelegatingHandler
    {
        private readonly Func<string> _apiKeySupplier;
        public ApiKeyHandler(Func<string> keySupplier)
        {
            _apiKeySupplier = keySupplier;
            InnerHandler = new HttpClientHandler();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.Add("x-api-key", _apiKeySupplier());
            return base.SendAsync(request, cancellationToken);
        }
    }

    public record Result<T>
    {
        public T? Value { get; init; }
        public ApiError? Error { get; init; }

        public bool IsSuccess => Error == null;

        public static Result<T> Success(T? value) =>
            new Result<T> { Value = value };

        public static Result<T> Failure(ApiError error) =>
            new Result<T> { Error = error };
    }

    public abstract record ApiError;
    public record UnknownError(int StatusCode, string Body) : ApiError;
    public record ValidationError(string Body) : ApiError;
    public record NetworkError(string Message) : ApiError;
    public record UnauthorizedError : ApiError;
    public record ForbiddenError : ApiError;
    public record NotFoundError : ApiError;
}


