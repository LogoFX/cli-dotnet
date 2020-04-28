using System;

namespace Samples.Specifications.Data.Contracts.Dto
{    
    public sealed class WarehouseItemDto
    {
        public Guid Id { get; set; }
        public string Kind { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}