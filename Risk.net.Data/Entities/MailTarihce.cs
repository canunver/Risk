using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki MailTarihce tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class MailTarihce : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string IlgiKod { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string IlgiTur { get; set; }

        [Column(TypeName = "dateTime")]
        public DateTime? IslemTarihi { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string MailAdres { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        public string MailIcerik { get; set; }

    }
}
