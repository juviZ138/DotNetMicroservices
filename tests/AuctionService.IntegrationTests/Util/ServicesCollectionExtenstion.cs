using System;
using AuctionService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests.Util;

public static class ServicesCollectionExtenstion
{
    public static void RemoveDbContext<T>(this IServiceCollection services)
    {
        //set up postgre
        var descriptor = services.SingleOrDefault(d =>
            d.ServiceType == typeof(DbContextOptions<AuctionDbContext>));

        if (descriptor != null) services.Remove(descriptor);
    }

    public static void EnsureCreated<T>(this IServiceCollection services)
    {
        //set up database data
        var sp = services.BuildServiceProvider();

        using var scope = sp.CreateScope();

        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<AuctionDbContext>();

        db.Database.Migrate();

        DbHelpers.InitDbForTest(db);
    }
}
