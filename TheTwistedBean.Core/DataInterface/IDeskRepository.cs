using TheTwistedBean.Core.Domain;

namespace TheTwistedBean.Core.DataInterface;

public interface IDeskRepository
{
    IEnumerable<Desk> GetAvailableDesks(DateTime date);
}