using Risk.net.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki ViewBulguYonetimiCevapDurum view ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class ViewBulguYonetimiCevapDurum : IEntity
    {
        [ForeignKey("BulguYonetimiKod")]
        [Column(TypeName = "varchar(40)")]
        public string BulguYonetimiKod { get; set; }
        public int Tur { get; set; }
        public int Cevaplanan { get; set; }
        public int Cevaplanmayan { get; set; }

    }

}
