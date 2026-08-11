using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki MailTarihce tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
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
