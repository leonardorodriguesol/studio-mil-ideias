using StudioMilIdeias.Domain.Entities;
using StudioMilIdeias.Domain.Enums;

namespace StudioMilIdeias.Domain.Tests;

public sealed class EntityDefaultsTests
{
    [Fact]
    public void NewProduct_IsActive_ShouldDefaultToTrue()
    {
        var product = new Product();

        Assert.True(product.IsActive);
    }

    [Fact]
    public void NewUser_Role_ShouldDefaultToCustomer()
    {
        var user = new User();

        Assert.Equal(UserRole.Customer, user.Role);
    }
}
