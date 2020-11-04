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
    public class EventGridClient : IEventBus
    {
        private HttpClient _client;
        private readonly Uri _eventGridUri;
        private JsonSerializerSettings _jsonSettings = new JsonSerializerSettings 
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };

        public EventGridClient(Uri eventGridUri, string sasKey, HttpMessageHandler messageHandler = null) {
            _eventGridUri = eventGridUri;
            _client = new HttpClient(messageHandler ?? new HttpClientHandler());
            _client.DefaultRequestHeaders.Add("aeg-sas-key", sasKey);
        }

        public void Publish(IntegrationEvent @event)
        {
            var jsonMessage = JsonConvert.SerializeObject(@event, _jsonSettings);

            _ = Send(new Event
            {
                Subject = "/foo",
                EventType = "super-event",
                Data = jsonMessage
            });
        }

        public void Send(IEnumerable<Event> eventItems)
        {
            var json = JsonConvert.SerializeObject(eventItems, _jsonSettings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(_eventGridUri, content).Result;
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("El mensaje: " + json.ToString() + " se envio con éxito");
                return;
            }
            var responseContent = response.Content.ReadAsStringAsync();
            throw new EventGridException((int)response.StatusCode, response.ReasonPhrase, responseContent.Result);
        }

        public async Task Send(Event eventItem) => Send(new Event[1] { eventItem });

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
