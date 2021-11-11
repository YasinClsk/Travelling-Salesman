using GMap.NET;
using System.Drawing;

namespace KargoDagitimSistemi
{
    class Coordinates
    {
        public PointLatLng Point { get; set; }
        public Pen Color { get; set; }
        public int CargoId { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public bool DeliveryStatus { get; set; }

        Coordinates AddCoordinate(PointLatLng point,Pen pen,string customerName,bool deliveryStatus,int cargoId,string location)
        {
            Coordinates coordinates = new Coordinates()
            {
                Point = point,
                Color = pen,
                CustomerName = customerName,
                DeliveryStatus = deliveryStatus,
                CargoId = cargoId,
                Address = location
            };

            return coordinates;
        }
    }
}
