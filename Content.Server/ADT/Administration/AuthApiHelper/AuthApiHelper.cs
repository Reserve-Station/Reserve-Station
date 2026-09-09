// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Schrödinger <132720404+Schrodinger71@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace Content.Server.ADT.Administration;

public sealed partial class AuthApiHelper
{
    private static HttpClient _httpClient = new HttpClient();
    private static ISawmill _sawmill = Logger.GetSawmill("AuthAPIHelper");

    public static async Task<string> GetCreationDate(string uuid)
    {
        string url = $"https://auth.spacestation14.com/api/query/userid?userid={uuid}";

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _sawmill.Warning($"API request failed for UUID {uuid}: {response.StatusCode}");
                return "Дата не найдена";
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            using JsonDocument jsonDoc = JsonDocument.Parse(jsonResponse);

            if (jsonDoc.RootElement.TryGetProperty("createdTime", out JsonElement createdTimeElement) &&
                createdTimeElement.ValueKind != JsonValueKind.Null &&
                createdTimeElement.ValueKind != JsonValueKind.Undefined)
            {
                string? createdTimeStr = createdTimeElement.GetString();
                if (!string.IsNullOrEmpty(createdTimeStr))
                {
                    DateTimeOffset dateObj = DateTimeOffset.Parse(createdTimeStr);
                    return dateObj.ToString("dd.MM.yyyy");
                }
            }

            _sawmill.Warning($"CreatedTime property missing or invalid for UUID: {uuid}");
            return "Дата не найдена";
        }
        catch (HttpRequestException httpEx)
        {
            _sawmill.Warning($"HTTP error for UUID {uuid}: {httpEx.Message}");
            return "Ошибка соединения";
        }
        catch (JsonException jsonEx)
        {
            _sawmill.Warning($"JSON parsing error for UUID {uuid}: {jsonEx.Message}");
            return "Ошибка данных";
        }
        catch (FormatException)
        {
            _sawmill.Warning($"Invalid date format for UUID: {uuid}");
            return "Неверный формат даты";
        }
        catch (Exception ex)
        {
            _sawmill.Warning($"Unexpected error for UUID {uuid}: {ex.Message}");
            return "Ошибка системы";
        }
    }
}
