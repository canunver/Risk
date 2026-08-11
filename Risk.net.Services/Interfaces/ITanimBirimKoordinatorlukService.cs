using Risk.net.Services.Objects;
using System.Threading.Tasks;

namespace Risk.net.Services.Interfaces
{
    public interface ITanimBirimKoordinatorlukService
    {
        Task<Sonuc> GetAll(string birimKod);
        Task<Sonuc> Save(string birimKod, string koordinatorlukKod);
        Task<Sonuc> Delete(string birimKod, string koordinatorlukKod);
    }
}
