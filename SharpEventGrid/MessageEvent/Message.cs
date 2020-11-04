using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpEventGrid
{
    public class Message : IntegrationEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Edad { get; set; }
        public string Profesion { get; set; }
    }
}
