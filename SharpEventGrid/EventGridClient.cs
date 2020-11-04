using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SharpEventGrid {
    public class EventGridClient : IEventBus// : IEventGridClient 
    {
        private HttpClient _client;
        private JsonSerializerSettings _jsonSettings = new JsonSerializerSettings 
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };
        private readonly Uri _eventGridUri;
        private const string INTEGRATION_EVENT_SUFFIX = "IntegrationEvent";

        public EventGridClient(Uri eventGridUri, string sasKey, HttpMessageHandler messageHandler = null) {
            _eventGridUri = eventGridUri;
            _client = new HttpClient(messageHandler ?? new HttpClientHandler());
            _client.DefaultRequestHeaders.Add("aeg-sas-key", sasKey);
        }

        public void Publish(IntegrationEvent @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event, _jsonSettings);

            Send(new Event
            {
                Subject = "/foo",
                EventType = "super-event",
                Data = jsonMessage
            });
        }

        public void Send(Event eventItem) => Send(new Event[1] { eventItem });

        public async Task Send(IEnumerable<Event> eventItems)
        {
            var jsonMessage = JsonConvert.SerializeObject(eventItems, _jsonSettings);
            var content = new StringContent(jsonMessage, Encoding.UTF8, "application/jsonMessage");
            var response = await _client.PostAsync(_eventGridUri, content);
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new EventGridException((int)response.StatusCode, response.ReasonPhrase, responseContent);
        }

        //public async Task Send(Event eventItem) => await Send(new Event[1] { eventItem });

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }

        public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }
    }
}
