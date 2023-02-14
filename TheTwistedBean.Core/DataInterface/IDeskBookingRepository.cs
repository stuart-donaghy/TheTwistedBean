using TheTwistedBean.Core.Domain;

namespace TheTwistedBean.Core.DataInterface;

public interface IDeskBookingRepository
{
    void Save(DeskBooking booking);
}