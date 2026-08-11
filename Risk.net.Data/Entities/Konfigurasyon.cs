using Risk.net.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki Konfigurasyon tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class Konfigurasyon : EntityBase, IEntity
    {
        public Konfigurasyon()
        {
        }

        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        public int OnayBekleyenRiskler { get; set; }

        public int OnayBekleyenAzaltma { get; set; }

        public int AzaltmaPlaniBitis { get; set; }

        public int AzaltmaPlaniOlusturulmadi { get; set; }

        [Column(TypeName = "varchar(5000)")]
        public string EtkiOlasilikMatrisi { get; set; }

        [Column(TypeName = "varchar(5000)")]
        public string RiskKategoriFinansal { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        [NotMapped]
        public int DigerKaydinDurumu { get; set; } = 0;


    }
}
