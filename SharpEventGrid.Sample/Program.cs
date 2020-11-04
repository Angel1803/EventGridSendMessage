using FluentNHibernate.Testing.Values;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpEventGrid.Sample 
{
    class Program 
    {
        public static void Main(string[] args) 
        {
            Console.WriteLine("***EventGrid Iniciado***");
            var eventGridUrl = "https://new-cars.southcentralus-1.eventgrid.azure.net/api/events";
            var eventGridKey = "YJW+hGFNyCmLHfBycJDSwGqbg7Ds9ZU1mPZKXIksaxk=";

            //Obtencion de los datos y serializo el mensaje
            Message message = GetMessageData();
            var serializeMessage = JsonConvert.SerializeObject(message);
            //Se crea la conexión
            var clientConnection = new EventGridClient(new Uri(eventGridUrl), eventGridKey);
            //Envio el mensaje a traves del cliente que establece la conexion 
            clientConnection.Publish(message);

            //var client = new EventGridClient(new Uri(url), key);
            //await client.Send(new Event {
            //    Subject = "/foo",
            //    EventType = "super-event",
            //    Data = data
            //});
        }

        private static Message GetMessageData()
        {
            //Obtencion de los datos
            //Message listMessage = new List<Message>();
            Message message = new Message();

            message.Id = 04160015;
            message.Name = "Miguel Angel Garcia Uc";
            message.Profesion = "Ingenieria en Sistemas Computacionales";
            message.Edad = 22;

            //listMessage.Add(message);

            return message;
        }
    }
}
