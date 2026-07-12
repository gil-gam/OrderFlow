using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace OrderFlow.UnitTests.Application.Helpers;

public static class TestDbContextFactory
{
    public static Infrastructure.Data.OrderFlowDbContext Create()
    {
        var options = new DbContextOptionsBuilder<Infrastructure.Data.OrderFlowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var publisher = new Mock<IPublisher>().Object;
        var context = new Infrastructure.Data.OrderFlowDbContext(options, publisher);
        context.Database.EnsureCreated();
        return context;
    }

    public static Infrastructure.Data.OrderFlowDbContext CreateWithSeed(
        Action<Infrastructure.Data.OrderFlowDbContext> seed)
    {
        var context = Create();
        seed(context);
        context.SaveChanges();
        return context;
    }
}