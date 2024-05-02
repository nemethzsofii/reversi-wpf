using System.Threading.Tasks;

namespace WpfApp1.Data
{
    public interface IReversiDataAccess
    {
        Task<WpfData> LoadAsync(string path);
        Task SaveAsync(WpfData data, string path);
    }
}