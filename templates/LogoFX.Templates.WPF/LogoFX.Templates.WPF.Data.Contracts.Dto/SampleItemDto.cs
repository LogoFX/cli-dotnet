using System;

namespace LogoFX.Templates.WPF.Data.Contracts.Dto
{    
    public sealed class SampleItemDto
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public int Value { get; set; }
    }
}