using System;
using JetBrains.Annotations;
using Samples.Basics.Model.Contracts;

namespace Samples.Basics.Model
{    
    [UsedImplicitly]
    internal sealed class WarehouseItem : AppModel, IWarehouseItem
    {
        public WarehouseItem(
            string kind, 
            double price, 
            int quantity)
        {
            Id = Guid.NewGuid();
            _kind = kind;
            _price = price;
            _quantity = quantity;            
        }

        private string _kind;
        public string Kind
        {
            get => _kind;
            set => SetProperty(ref _kind, value);
        }

        private double _price;
        public double Price
        {
            get => _price;
            set
            {
                SetProperty(ref _price, value);
                NotifyOfPropertyChange(() => TotalCost);
            }
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {                
                SetProperty(ref _quantity, value);
                NotifyOfPropertyChange(() => TotalCost);
            }
        }        

        public double TotalCost => _quantity*_price;
    }
}
