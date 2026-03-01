using StudioMilIdeias.Domain.Entities;

namespace StudioMilIdeias.Application.Abstractions;

public interface ITokenProvider
{
    string Generate(User user);
}
