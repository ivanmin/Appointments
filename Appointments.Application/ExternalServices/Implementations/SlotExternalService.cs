﻿using Appointments.Application.Configurations;
using Appointments.Application.Exceptions;
using Appointments.Application.ExternalServices.Interfaces;
using Appointments.Domain.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Appointments.Application.ExternalServices.Implementations
{
    public class SlotExternalService : ISlotExternalService
    {
        private readonly ILogger<ISlotExternalService> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly SlotServiceSettings _slotServiceSettings;

        public SlotExternalService(ILogger<ISlotExternalService> logger, IHttpClientFactory clientFactory, IOptions<SlotServiceSettings> slotServiceSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _slotServiceSettings = slotServiceSettings.Value ?? throw new ArgumentNullException(nameof(slotServiceSettings));
        }

        private HttpClient CreateHttpClient()
        {
            var client = _clientFactory.CreateClient();
            string authInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_slotServiceSettings.Username}:{_slotServiceSettings.Password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
            return client;
        }

        public async Task<Schedule?> GetWeeklyAvailability(string date)
        {
            try
            {
                using HttpClient client = CreateHttpClient();
                var response = await client.GetAsync($"{_slotServiceSettings.BaseUrl}/GetWeeklyAvailability/{date}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    if (!JObject.Parse(json).HasValues)
                    {
                        _logger.LogWarning("Received empty JSON response from GetWeeklyAvailability");
                        return null;
                    }

                    return JsonConvert.DeserializeObject<Schedule>(json);
                }
                else
                {
                    _logger.LogWarning("Error while processing request from GetWeeklyAvailability with StatusCode {StatusCode}.", response.StatusCode);
                    throw new ExternalServiceException("Error while processing request in external service. Contact with Backend team");
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error while processing request from GetWeeklyAvailability");
                throw new ExternalServiceException("Error while processing request in external service. Contact with Backend team");
            }
        }

        public async Task<bool> TakeSlot(Appointment appointment)
        {
            try
            {
                using HttpClient client = CreateHttpClient();

                var json = JsonConvert.SerializeObject(appointment);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_slotServiceSettings.BaseUrl}/TakeSlot", data);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error while processing request from TakeSloot");
                throw new ExternalServiceException("Error while processing request in external service. Contact with Backend team");
            }
        }
    }
}

