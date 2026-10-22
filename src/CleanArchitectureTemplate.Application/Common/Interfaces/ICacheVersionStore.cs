namespace CleanArchitectureTemplate.Application.Common.Interfaces;

public interface ICacheVersionStore
{
    long GetVersion(string tag);
    void Invalidate(string tag);
}
