
namespace GW2PAO.Modules.Map.Interfaces
{
    public interface IMapViewController
    {
        void Initialize();
        void Shutdown();

        void OpenMap();
        bool CanOpenMap();
    }
}
