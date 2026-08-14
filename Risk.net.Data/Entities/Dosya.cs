using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki Dosya tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class Dosya : EntityBase, IEntity
    {
        public Dosya()
        {
            KayitTarihi = (DateTime?)null;
        }

        public Dosya Clone()
        {
            return (Dosya)this.MemberwiseClone();
        }

        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [Column(TypeName = "varchar(40)")]
        public string BaglantiKod { get; set; } = "";

        [Column(TypeName = "varchar(150)")]
        public string Adi { get; set; }

        [Column(TypeName = "int")]
        public int Boyut { get; set; }

        [Column(TypeName = "date")]
        public DateTime? KayitTarihi { get; set; }


        [ForeignKey("ViewPersonel")]
        public string KayitEdenKod { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        public ViewPersonel KayitEden { get; set; }

        [Column(TypeName = "varchar(250)")]
        public string Aciklama { get; set; }


        [NotMapped]
        public string IcerikBase64 { get; set; }
        [NotMapped]
        public byte[] Icerik { get; set; }
        [NotMapped]
        public string MimeType { get; set; }

        public virtual string BoyutFormatli
        {
            get
            {
                string[] tanimlar = { "B", "KB", "MB", "GB", "TB" };
                int i = Arac.ConvertToInt(Math.Floor(Math.Log(Boyut) / Math.Log(1024)));
                double db = Math.Floor(Boyut / Math.Pow(1024, i));

                return db.ToString("#,###") + " " + tanimlar[i];
            }
        }

        public virtual string Tipi
        {
            get
            {
                string ext = System.IO.Path.GetExtension(Adi);
                ext = ext.ToLower();

                if (ext == ".xls" || ext == ".xlsx") ext = "<i class='fal fa-file-excel text-success'></i>";
                else if (ext == ".doc" || ext == ".docx") ext = "<i class='fal fa-file-word text-primary'></i>";
                else if (ext == ".ppt" || ext == ".pptx") ext = "<i class='fal fa-file-powerpoint text-danger'></i>";
                else if (ext == ".pdf") ext = "<i class='fal fa-file-pdf text-danger'></i>";
                else if (ext == ".zip") ext = "<i class='fal fa-file-archive text-muted'></i>";
                else if (ext == ".txt") ext = "<i class='fal fa-file-alt text-info'></i>";
                else if (ext == ".mov" || ext == ".avi" || ext == ".mp4") ext = "<i class='fal fa-file-video text-warning'></i>";
                else if (ext == ".mp3") ext = "<i class='fal fa-file-audio text-warning'></i>";
                else if (ext == ".jpg" || ext == ".gif" || ext == ".png" || ext == ".bmp") ext = "<i class='fal fa-file-image text-danger'></i>";
                else ext = "<i class='fal fa-file text-primary'></i>";

                return ext;
            }
        }
    }
}
