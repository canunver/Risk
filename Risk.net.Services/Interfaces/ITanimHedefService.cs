using Risk.net.Data.Entities;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.Services.Interfaces
{
    public interface ITanimHedefService
    {
        Task<Sonuc> KayitGetirAsync(string kod);
        Task<Sonuc> ListeleAsync();
        Task<object> TabloDoldurAsync(DataTablesParam dataTablesParam);
        Task<Sonuc> KaydetAsync(TanimHedef gelen);
        Task<Sonuc> SilAsync(string kod);
    }
}
