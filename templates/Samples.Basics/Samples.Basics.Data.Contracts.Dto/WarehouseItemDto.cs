using System;

namespace Samples.Basics.Data.Contracts.Dto
{    
    public sealed class WarehouseItemDto
    {
        public Guid Id { get; set; }
        public string Kind { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}