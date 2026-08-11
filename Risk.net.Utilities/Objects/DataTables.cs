using Microsoft.AspNetCore.Http;
using Risk.net.Utilities.Functions;
using System.Collections.Generic;
using System.Linq;

namespace Risk.net.Utilities.Objects
{
    public class DataTablesYapi
    {
        public string adi { get; set; }
        public string anahtarAlani { get; set; }
        public string anahtarAlani2 { get; set; }
        public string listeUrl { get; set; }
        public string silUrl { get; set; }
        public string duzenleUrl { get; set; }
        public string dom { get; set; } = "";
        public string dataFunc { get; set; } = "";
        public bool secim { get; set; } = false;
        public bool aramaAlaniGoster { get; set; } = true;
        public string initComplate { get; set; } = "";
        public string satirOlustur { get; set; } = "";
        public string baslik { get; set; } = "";
        public int siralamaKolonNo { get; set; } = 0;
        public string siralamaYonu { get; set; } = "asc";

        public DataTablesIslemler islemler { get; set; } = new DataTablesIslemler();
        public List<DataTablesAlanlar> alanlar { get; set; } = new List<DataTablesAlanlar>();
    }
    public class DataTablesIslemler
    {
        public bool goster { get; set; } = true;
        public bool duzenleme { get; set; } = true;
        public bool silme { get; set; } = true;
        public string duzenlemeSonucMetodu { get; set; }
        public string genislik { get; set; } = "";
        public string digerIslemler { get; set; } = "";
    }
    public class DataTablesAlanlar
    {
        public string veriAlani { get; set; }
        public string adi { get; set; }
        public string tipi { get; set; } = "string";
        public string genislik { get; set; } = "";
        public bool siralama { get; set; } = true;
        public bool otomatikGenislik { get; set; } = true;
        public string render { get; set; } = "";
        public bool tablodaGoster { get; set; } = true;
        public bool aramadaGoster { get; set; } = true;
        public string aramaAdi { get; set; }
        public string aramaTipi { get; set; } = "text";
        public string aramaListeDoldurUrl { get; set; } = "";
        public string aramaListeDoldurKosul { get; set; } = "";
        public string aramaListeDoldurMetodu { get; set; } = "";
        public string aramaAlaniSecimTetikle { get; set; } = "";
        public string aramaAlanAdi { get; set; } = "";
    }
    public class DataTablesParam
    {
        public int draw { get; set; } = 0;
        public int start { get; set; } = 0;
        public int length { get; set; } = 0;
        public string sortColumn { get; set; } = "";
        public string sortColumnDirection { get; set; } = "";
        public string searchValue { get; set; } = "";
        public string pageName { get; set; } = "";

        public int pageSize
        {
            get
            {
                int tmp = length == -1 ? 1000000 : length;

                return Arac.ConvertToInt(tmp);
            }
        }

        public int skip
        {
            get
            {
                return start != 0 ? Arac.ConvertToInt(start) : 0;
            }
        }

        public DataTablesParam(HttpRequest request)
        {
            //https://codewithmukesh.com/blog/jquery-datatable-in-aspnet-core/

            this.draw = Arac.ConvertToInt(request.Form["draw"].FirstOrDefault());
            this.start = Arac.ConvertToInt(request.Form["start"].FirstOrDefault());
            this.length = Arac.ConvertToInt(request.Form["length"].FirstOrDefault());
            this.sortColumn = request.Form["columns[" + request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault();
            this.sortColumnDirection = request.Form["order[0][dir]"].FirstOrDefault();
            this.searchValue = request.Form["search[value]"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(sortColumnDirection))
                sortColumnDirection = "asc";
        }
    }
}
