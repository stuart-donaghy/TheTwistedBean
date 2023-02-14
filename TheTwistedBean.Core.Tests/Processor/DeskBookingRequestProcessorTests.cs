using Moq;
using TheTwistedBean.Core.DataInterface;
using TheTwistedBean.Core.Domain;
using TheTwistedBean.Core.Processor;

namespace TheTwistedBean.Core.Tests.Processor;

public class DeskBookingRequestProcessorTests
{
    private readonly List<Desk> _availableDesks;
    private readonly Mock<IDeskBookingRepository> _deskBookingRepositoryMock;
    private readonly Mock<IDeskRepository> _deskRepositoryMock;
    private readonly DeskBookingRequestProcessor _processor;
    private readonly DeskBookingRequest _request;

    public DeskBookingRequestProcessorTests()
    {
        _request = new DeskBookingRequest
        {
            FirstName = "Thomas",
            LastName = "Huber",
            Email = "thomas@thomasclaudiushubber.com",
            Date = new DateTime(2020, 1, 28)
        };

        _availableDesks = new List<Desk> { new() { Id = 7 } };
        _deskBookingRepositoryMock = new Mock<IDeskBookingRepository>();
        _deskRepositoryMock = new Mock<IDeskRepository>();
        _deskRepositoryMock.Setup(x => x.GetAvailableDesks(_request.Date)).Returns(_availableDesks);
        _processor = new DeskBookingRequestProcessor(_deskBookingRepositoryMock.Object, _deskRepositoryMock.Object);
    }

    [Fact]
    public void ShouldReturnDeskBookingResultWithRequestValues()
    {
        var result = _processor.BookDesk(_request);

        Assert.NotNull(result);
        Assert.Equal(_request.FirstName, result.FirstName);
        Assert.Equal(_request.LastName, result.LastName);
        Assert.Equal(_request.Email, result.Email);
        Assert.Equal(_request.Date, result.Date);
    }

    [Fact]
    public void ShouldThrowExceptionIfRequestIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => _processor.BookDesk(null));

        Assert.Equal("request", exception.ParamName);
    }

    [Fact]
    public void ShouldSaveDeskBooking()
    {
        DeskBooking savedDeskBooking = null;

        _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
            .Callback<DeskBooking>(deskBooking => { savedDeskBooking = deskBooking; });

        _processor.BookDesk(_request);

        _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);

        Assert.NotNull(savedDeskBooking);
        Assert.Equal(_request.FirstName, savedDeskBooking.FirstName);
        Assert.Equal(_request.LastName, savedDeskBooking.LastName);
        Assert.Equal(_request.Email, savedDeskBooking.Email);
        Assert.Equal(_request.Date, savedDeskBooking.Date);
        Assert.Equal(_availableDesks.First().Id, savedDeskBooking.DeskId);
    }

    [Fact]
    public void ShouldNotSaveDeskBookingIfNoDeskIsAvailable()
    {
        _availableDesks.Clear();

        _processor.BookDesk(_request);

        _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
    }

    [Theory]
    [InlineData(DeskBookingResultCode.Success, true)]
    [InlineData(DeskBookingResultCode.NoDeskAvailable, false)]
    public void ShouldReturnExpectedResultCode(DeskBookingResultCode expectedResultCode, bool isDeskAvailable)
    {
        if(!isDeskAvailable)
            _availableDesks.Clear();

        var result = _processor.BookDesk(_request);

        Assert.Equal(expectedResultCode,result.Code);
    }

    [Theory]
    [InlineData(7, true)]
    [InlineData(null, false)]
    public void ShouldReturnExpectedDeskBookingId(int? expectedDeskBookingId, bool isDeskAvailable)
    {
        if (!isDeskAvailable)
            _availableDesks.Clear();
        else
            _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
                .Callback<DeskBooking>(deskBooking =>
                {
                    deskBooking.Id = expectedDeskBookingId;
                });

        var result = _processor.BookDesk(_request);

        Assert.Equal(expectedDeskBookingId, result.DeskBookingId);
    }
}