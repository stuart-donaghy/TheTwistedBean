namespace TheTwistedBean.Core.Domain;

public class DeskBooking : DeskBookingBase
{
    public int? Id { get; set; }
    public int DeskId { get; set; }
}