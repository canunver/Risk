namespace Risk.net.Utilities.Objects
{
    public enum ENUMIslemDurum
    {
        Basarili = 0,
        Hata = 1,
        Uyari = 2,
        Bilgi = 3
    }

    public enum ENUMKullaniciRol
    {
        TANIMSIZ = 0,
        RISKSEKRETARYASI = 11,
        PLANLAMAUNITESI = 12,
        YETKILIRISKGOREVLISI = 13,
        BIRIMAMIRIDESTEKHIZMETLERI = 15,
        UZMAN = 14,
        BIRIMAMIRI = 15,
        ILKOORDINATOR = 16,
        MERKEZKOORDINATOR = 17,
        GENELKOORDINATOR = 18,
        BASKAN = 19,
        ICDENETIMUZMANI = 20,
        ICDENETIMKOORDINATOR = 21,
        SISTEMYONETICISI = 10,
        //RISKSAHIBI = 40

    }

    public enum ENUMDurum
    {
        Tanimsiz = 0,
        Aktif = 1,
        GeriGonderildi = 2,
        OnayaGonderdi = 3,
        OnayaGonderildiKoordinator = 4,
        Onayli = 10,
        Kapali = 12,//BULGUYONETIMI
        BilgilendirmeMail = 15,
        RiskSahibiDegisti = 16,
        Kopyalandı = 17,
        AzaltmaPlaniSorumlusuDegisti = 18,
        HatirlatmaMail = 19,
        Reddedildi = 98,
        Pasif = 99,
        Sil = 999,
    }


    public enum EnumTarihceIslemTur
    {
        Tanimsiz = 0,
        RiskEvreni = 1,
        OlayRaporlama = 2,
        BulguYonetimi = 3,
        StratejikPlan = 4,
        AnahtarRiskGostergesi = 5,
        RiskYonetimi = 6,
        RiskAzaltmaPlani = 7,
        StratejikPlanIzleme = 8,
        BildirimSistemi = 9,
        Denetim = 10,
        Konfigurasyon = 11,
        BulguYonetimiCevap = 12,
        RiskYonetimiBeyannamesi = 13,
        RiskYonetimiBeyannamesiMailGonder = 14,


        IcKontrolZayifliklariRaporunuHatirlat = 50,
        RiskVeAzaltmaPlaniListesiniHatirlat = 51,
        YuksekVeOrtaRiskleriHatirlat = 52,
        RiskYonetimiProsedurunuHatirlat = 53,
        RiskDegerlendirmeyiHatirlat = 54,
        RiskYonetimiBeyannameImza = 55,
        RiskYonetimiBeyannameImzaHatirlat = 56,


        RiskEvreniDurumPasif = 101,
        RiskEvreniRiskSahibiDegisti = 102,
        RiskEvreniOnaydaDegisiklik = 103,
        RisklerinDegerlendirmesiOnaydaDegisiklik = 104,
        RisklerinYonetilmesiOnaydaDegisiklik = 105,
        RiskEvreniDurumDegisti = 106,
        RisklerinDegerlendirmesiDurumDegisti = 107,
        RisklerinYonetilmesiDurumDegisti = 108,
        RiskAzaltmaPlaniSorumlusuDegisti = 109,
        RiskAzaltmaPlaniOnaylandi = 110,

    }

    public enum EnumRiskEvreniRiskTuru
    {
        Tanimsiz = 0,
        Tehdit = 1,
        Firsat = 2,
    }

    public enum EnumRiskYonetimiRiskeVerilecekCevap
    {
        Tanimsiz = 0,
        TransferEt = 1,
        KabulEt = 2,
        Reddet = 3,
        Azalt = 4,
    }

    public enum EnumRiskAzaltmaPlaniMevcutDurum
    {
        Tanimsiz = 0,
        DevamEdiyor = 1,
        Ertelendi = 2,
        Durduruldu = 3,
        Tamamlandi = 10,
        IptalEdildi = 99,
    }

    public enum EnumDenetimKaynak
    {
        Tanimsiz = 0,
        IcDenetim = 1,
        DisDenetim = 2,
    }

    //public enum ENUMDurum
    //{
    //    Tanimsiz = 0,
    //    YeniKayit = 1,
    //    GeriGonderildi = 2,
    //    OnayaGonderdi = 3,
    //    Onayli = 10,
    //    Reddedildi = 98,
    //    Iptal = 99,
    //}

    public enum EnumKoordinatorlukTur
    {
        Tanimsiz = 0,
        Baskan = 10,
        Genel = 20,
        IcDenetim = 21,
        Hukuk = 22,
        Merkez = 30,
        Il = 40,
    }

    public enum EnumBildirimSistemiIslem
    {
        Tanimsiz = 0,
        Yeni = 1,
        OnayBekliyor = 2,
        Yaklasiyor = 3,
        Hatirlatma = 4,
        SureGecti = 5,
        AzaltmaPlaniOlustur = 6,
        Uyari = 7,
        Bilgilendirme = 8,
    }

    public enum EnumStratejikPlanEylemPlaniTamamlanmaDurumu
    {
        Tanimsiz = 0,
        DevamEdiyor = 1,
        Ertelendi = 2,
        IptalEdildi = 99,
        Tamamlandi = 10,
    }

    public enum EnumAnahtarRiskGostergesiDonemPeriyot
    {
        Tanimsiz = 0,
        Aylik = 1,
        Aylik3 = 2,
        Aylik6 = 3,
        Yillik = 4,
    }

    public enum ENUMBildirimSistemiDurum
    {
        Tanimsiz = 0,
        Aktif = 1,
        GeriGonderildi = 2,
        OnayaGonderdi = 3,
        OnayaGonderildiKoordinator = 4,
        Onayli = 10,
        Reddedildi = 98,
        Pasif = 99,

        OnayDisiBildirimler = 100,

        YapisalRiskSeviyesiDustu = 101,
        YapisalRiskSeviyesiYukseldi = 102,
        ArtikRiskSeviyesiDustu = 103,
        ArtikRiskSeviyesiYukseldi = 104,
        ARGDegeriKirmiziDegerinUzerineCikti = 105,
        YapisalRiskPuaniDegisiti = 106,
        ArtikRiskPuaniDegisiti = 107,
    }
}
