$(document).ready(function () {
    //Tooltiplerin click olayından sonra gösterilmesini engellemek için
    $('[data-toggle="tooltip"]').click(function () {
        $('.tooltip').hide();
    });

    //dinamik (sonradan) oluşturulan düğmeler için tooltip gözükmesini sağlıyor
    var tooltip = $('body').tooltip({
        selector: '.genelTooltip'
    });

    //Tooltiplerin click olayından sonra gösterilmesini engellemek için
    tooltip.click(function () {
        $('.tooltip').remove();

    });;

    $.fn.modal.Constructor.prototype.enforceFocus = function () { };

    $('.tarihAlani').datepicker({
        format: "dd.mm.yyyy",
        todayBtn: true,
        language: "tr",
        autoclose: true,
        todayHighlight: true,
        toggleActive: true
    });
    $(".tarihAlani").attr("placeholder", "gg.aa.yyyy");

    AutoNumericYap();

    CalisiyorDugmeOlustur();

    $('textarea').on('input', function () {
        this.style.height = 'auto';
        if (this.scrollHeight > 0)
            this.style.height = (this.scrollHeight) + 'px';
    });

    $('textarea').change(function () {
        var id = this.id;
        if (id != '') {
            setTimeout(function () {
                var el = $('#' + id);

                var max = 0;
                if (el.css("max-height") != 'none') {
                    max = parseInt(el.css("max-height").replace("px", ""));
                }

                if (max > 0 && el.prop('scrollHeight') > max) {
                    el.css({ height: el.css("max-height") });
                    el.addClass("textarea-autoheight-no");
                }
                else {
                    el.css({ height: 'auto' });
                    if (el.prop('scrollHeight') != "0")
                        el.css({ height: el.prop('scrollHeight') + 'px' });
                }
            }, 200);
        }
    });

});

var numericControlsInfo;
function AutoNumericYap() {
    try {
        for (var i = 0; i < numericControlsInfo.length; i++) {
            numericControlsInfo[i].global.remove();
            numericControlsInfo[i].global.wipe();
        }
    } catch (e) { }

    try {
        numericControlsInfo = new AutoNumeric.multiple('.autonumeric', {
            digitGroupSeparator: '.',
            decimalCharacter: ',',
            decimalPlacesOverride: 2,
            currencySymbol: "",
            currencySymbolPlacement: "s",
            minimumValue: "0"
        });
        //$(".autonumeric").attr("placeholder", "");
    } catch (e) { }
}

function TarihDegeriDDMMYYYY(tarih) {
    if (tarih == undefined) return "";
    else if (tarih == null) return "";
    else if (tarih == "") return "";

    return moment(tarih, 'YYYY-MM-DD').format('DD.MM.YYYY');
}

function SayiDegeriniOku(kontrolAdi) {//kontrolAdini başına # koyarak Örn: #txtTutar
    var deger = AutoNumeric.getNumericString(kontrolAdi);

    deger = deger.replace(".", ',');

    return deger;
}

//Sunucudan gelen mesajı bootstrap alert tipinde sayfada gösterimi
HataYazisiGoster = (id, mesaj) => {
    $("#" + id).html("");

    $('<div class="alert alert-danger">' +
        '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
        unescape(mesaj) +
        '</div>'
    ).hide().appendTo('#' + id).fadeIn(100);
}

//Sunucudan gelen mesajı popup pencerede gösterilmesi
HataPenceresiGoster = (mesaj) => {
    Swal.fire({
        title: "<span class='text-danger'>Hata</span>",
        icon: "error",
        html: "<div class='text-left'>" + unescape(mesaj) + "</div>",
        type: "warning"
    });
}

//Sunucudan gelen mesajı popup pencerede gösterilmesi
BilgiPenceresiGoster = (mesaj) => {
    Swal.fire({
        title: "<span class='text-info'>Bilgi</span>",
        icon: "info",
        html: "<div class='text-left'>" + unescape(mesaj) + "</div>",
        type: "info"
    });
}

//işlemlerden sonra bilgi vermek için init değerler
ShowToastr = (type, css, msg) => {
    toastr[type](msg);
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": "200",
        "hideDuration": "500",
        "timeOut": "2000",
        "extendedTimeOut": "500",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
}

//işlem öncesi bekleme arka planı göster
ShowOverLoading = () => {
    $.LoadingOverlay("show", {
        background: "rgba(192, 192, 192, 0.5)"
    });
}

//işlem sonrası bekleme arka planı gizle
HideOverLoading = () => {
    $.LoadingOverlay("hide");
}

//modal pencere açmak için
jModalShow = (id, baslik) => {

    //modal başlığını set et
    $('#' + id + ' .modal-title').html(baslik);

    //modal başlığı taşınabilir olsun
    $('#' + id + ' .modal-dialog').draggable({
        handle: ".modal-header"
    });

    //modal açıldıktan sonra class adı autofocus olan kontrole focus olsun
    $('#' + id).on('shown.bs.modal', function () {
        $(this).find('.autofocus').focus();
    });

    //modalı göster
    $('#' + id).modal('show');
}

//modal pencere kapatmak için
jModalHide = (id) => {
    $('#' + id).modal('hide');
}

var durumlarListe;
function DurumAdiVer(data, tur, renk) {
    var ad = "";
    if (durumlarListe != undefined && durumlarListe != null) {
        $.each(durumlarListe, function (a, b) {
            if (b.id == data)
                ad = b.text;
        });

        return ad;
    }

    if (data == undefined)
        return "Yeni Kayıt";

    if (tur == "AKTIFPASIF") {
        if (data == 1) return "Aktif";
        if (data == 99) return "Pasif";
    }
    else if (tur == "AKTIFPASIFONAYLI") {
        if (data == 1) return "Aktif";
        if (data == 10) return "Onaylı";
        if (data == 99) return "Pasif";
    }
    else if (tur == "YENIKAYITONAYLI") {
        if (data == 1) return "Yeni Kayıt";
        if (data == 10) return "Onaylı";
        if (data == 3) return "Onaya Gönderildi";
        if (data == 99) return "Pasif";
    }
    else if (tur == "BULGUYONETIMI") {
        if (data == 1) return "Açık";
        if (data == 2) return "Kısmen Kapalı";
        if (data == 11) return "Risk üstlenildi";
        if (data == 12) return "Kapalı";
        if (data == 99) return "Pasif";
    }
    else if (tur == "EVETHAYIR") {
        if (data == 1) return "Evet";
        if (data == 2) return "Hayır";
    }
    else if (tur == "DEVAMEDEN") {
        if (data == 1) return "Devam ediyor";
        if (data == 2) return "Tamamlandı";
        if (data == 99) return "Pasif";
    }

    //renk = "kırmızı";
}

function DurumDoldur(kontrolAdi, tur, parentTanila) {

    var data = [];

    if (tur == "AKTIFPASIF") {
        data = [
            { id: "1", text: "Aktif" },
            { id: "99", text: "Pasif" }
        ];
    }
    else if (tur == "AKTIFPASIFONAYLI") {
        data = [
            { id: "1", text: "Aktif" },
            { id: "10", text: "Onaylı" },
            { id: "99", text: "Pasif" }
        ];
    }
    else if (tur == "YENIKAYITONAYLI") {
        data = [
            { id: "1", text: "Yeni Kayıt" },
            { id: "10", text: "Onaylı" },
            { id: "3", text: "Onaya Gönderildi" },
            { id: "99", text: "Pasif" },
        ];
    }
    else if (tur == "DEVAMEDEN") {
        data = [
            { id: "1", text: "Devam Ediyor" },
            { id: "2", text: "Tamamlandı" },
            { id: "99", text: "Pasif" },
        ];
    }
    else if (tur == "BULGUYONETIMI") {
        data = [
            { id: "1", text: "Açık" },
            { id: "2", text: "Kısmen Kapalı" },
            { id: "11", text: "Risk üstlenildi" },
            { id: "12", text: "Kapalı" },
        ];
    }
    else if (tur == "EVETHAYIR") {
        data = [
            { id: "1", text: "Evet" },
            { id: "2", text: "Hayır" },
        ];
    }

    Select2Yap(kontrolAdi, parentTanila, data);
}

function TanimGenelComboDoldur(kontrolAdi, tur) {
    $.ajax({
        url: "/TanimGenel/SelectListesiVer",
        type: "POST",
        data: { tur: tur },
        success: function (result) {

            Select2Yap(kontrolAdi, true, result);
        }
    });
}

function BirimDoldur(kontrolAdi, parentKontrolAdi, parentTanila) {

    var parentKod = $("#" + parentKontrolAdi).val();

    Select2Yap(kontrolAdi, parentTanila, null);
    if (parentKod == undefined || parentKod == null || parentKod == "")
        return;

    $.ajax({
        url: "/TanimBirim/SelectListesiVer",
        data: { koordinatorlukKod: parentKod },
        type: "POST",
        success: function (result) {

            Select2Yap(kontrolAdi, parentTanila, result);
        }
    });
}

function SurecDoldur(kontrolAdi, parentKontrolAdi, parentKontrol2Adi, parentTanila) {

    var parentKod = $("#" + parentKontrolAdi).val();
    var parentKod2 = $("#" + parentKontrol2Adi).val();

    Select2Yap(kontrolAdi, parentTanila, null);
    if (parentKod == undefined || parentKod == null || parentKod == "")
        return;

    $.ajax({
        url: "/Surec/SelectListesiVer",
        data: { koordinatorlukKod: parentKod, birimKod: parentKod2 },
        type: "POST",
        success: function (result) {

            Select2Yap(kontrolAdi, parentTanila, result);
        }
    });
}

function AmacDoldur(kontrolAdi, parentKontrolAdi, parentTanila) {

    var parentKod = $("#" + parentKontrolAdi).val();

    Select2Yap(kontrolAdi, parentTanila, null);
    if (parentKod == undefined || parentKod == null || parentKod == "")
        return;

    $.ajax({
        url: "/StratejikPlan/SelectListesiVer",
        data: { koordinatorlukKod: parentKod },
        type: "POST",
        success: function (result) {
            Select2Yap(kontrolAdi, parentTanila, result);
        }
    });
}

function HedefDoldur(kontrolAdi, parentKontrolAdi, parentTanila) {

    var parentKod = $("#" + parentKontrolAdi).val();

    Select2Yap(kontrolAdi, parentTanila, null);
    if (parentKod == undefined || parentKod == null || parentKod == "")
        return;

    $.ajax({
        url: "/StratejikPlan/SelectListesiVerHedef",
        data: { stratejikPlanKod: parentKod },
        type: "POST",
        success: function (result) {
            Select2Yap(kontrolAdi, parentTanila, result);
        }
    });
}

function ComboyaElemanEkle(kontrolAdi, data) {
    var kontrol = $("#" + kontrolAdi);

    for (var i = 0; i < data.length; i++) {
        var newOption = new Option(data[i].text, data[i].id, false, false);
        kontrol.append(newOption);
    }
}

function Select2Yap(kontrolAdi, parentTanimla, data, clear) {

    var kontrol = $("#" + kontrolAdi);

    if (clear == undefined || clear == null)
        clear = true;

    var secimYazisi = "";
    secimYazisi = kontrol.attr("data-placeHolder");

    if (!secimYazisi || secimYazisi == undefined || secimYazisi == "")
        secimYazisi = "Lütfen Seçiniz";

    var dropdownParent = "";
    if (parentTanimla)
        dropdownParent = kontrol.parent();

    kontrol.select2().empty();

    if (data == undefined || data == null) {
        data = [];
    }

    kontrol.select2({
        placeholder: secimYazisi,
        dropdownParent: dropdownParent,
        allowClear: clear,
        data: data
    });


    kontrol.val("").trigger('change');
}

function Select2YapTemplate(kontrolAdi, parentTanimla, data, clear, templateResult) {

    var kontrol = $("#" + kontrolAdi);

    if (clear == undefined || clear == null)
        clear = true;

    var secimYazisi = "";
    secimYazisi = kontrol.attr("data-placeHolder");

    if (!secimYazisi || secimYazisi == undefined || secimYazisi == "")
        secimYazisi = "Lütfen Seçiniz";

    var dropdownParent = "";
    if (parentTanimla)
        dropdownParent = kontrol.parent();

    kontrol.select2().empty();

    if (data == undefined || data == null) {
        data = [];
    }

    kontrol.select2({
        placeholder: secimYazisi,
        dropdownParent: dropdownParent,
        allowClear: clear,
        data: data,
        templateResult: templateResult,
        escapeMarkup: function (elm) {
            return elm;
        }
    });


    kontrol.val("").trigger('change');
}

//Formlarda submit işlemi yapılmadığı zaman burası validation için kullanılılır
//validation için
//1.<form class="needs-validation" novalidate>
//2.<input type="text" required />
//3.düğme işleminde burası çağrılacak
//4.<div class="invalid-feedback"> alanı hemen controlün altında konulacak aynı div içinde olacak
function ValidateForm(cssName) {
    var validation = true;

    var cssName = cssName;
    if (cssName == null || cssName == undefined || cssName == '')
        cssName = 'needs-validation';

    var forms = $('.' + cssName + ':visible');

    Array.prototype.filter.call(forms, function (form) {

        form.checkValidity();
        form.classList.add('was-validated');

        for (var i = 0; i < form.length; i++) {

            if ($(form[i]).is(":hidden")) continue;

            var elementValidation = form[i].checkValidity();

            if (elementValidation === false) {
                validation = elementValidation;
                event.preventDefault();
                event.stopPropagation();
            }
        }

    });

    return validation;
}

function ClearValidation(cssName) {

    var cssName = cssName;
    if (cssName == null || cssName == undefined || cssName == '')
        cssName = 'needs-validation';

    var forms = document.getElementsByClassName(cssName);

    Array.prototype.filter.call(forms, function (form) {

        form.classList.remove('was-validated');
    });
}

function DinamikTabloVeriYok(tabloAdi) {
    $("#div" + tabloAdi + "VeriYok").html("<div class='text-center'><img src='/img/images/no-data-found.png' alt='' /><br />Gösterilecek kayıt yok</div>");
}

function DinamikTabloSatirEkle(tabloAdi, kod, satirBilgi, duzenleFunc, silFunc, ekDugme) {

    if (duzenleFunc == undefined || duzenleFunc == null) duzenleFunc = "";
    if (silFunc == undefined || silFunc == null) silFunc = "";
    if (ekDugme == undefined || ekDugme == null) ekDugme = "";

    if (!kod) kod = "";

    var kontrolKod = kod;
    kontrolKod = kontrolKod.replaceAll("null", "");
    kontrolKod = kontrolKod.replaceAll("_", "");

    if (kontrolKod.trim() == "")
        return;

    var tabloSatirAlan = $("#" + tabloAdi + " tbody");

    var degisecekSatir;
    tabloSatirAlan.find("tr").each(function (index, element) {
        if ($(element)[0].id == "sttb_" + kod) {
            degisecekSatir = $(element);
        }
    });

    var duzenleDugme = "<button type='button' class='btn btn-success btn-sm btn-icon waves-effect waves-themed text-white genelTooltip' data-toggle='tooltip' title='Düzenle' onClick='" + duzenleFunc + "(this)'><i class='fal fa-pencil'></i></button>";
    var silDugme = "<button type='button' class='btn btn-danger btn-sm btn-icon waves-effect waves-themed text-white ml-1 genelTooltip' data-toggle='tooltip' title='Sil' onClick='" + silFunc + "(this)'><i class='fal fa-trash-alt'></i></button>";
    if (duzenleFunc == "") duzenleDugme = "";
    if (silFunc == "") silDugme = "";

    var dugmeler = duzenleDugme + silDugme + ekDugme;
    if (dugmeler != "") dugmeler = "<td nowrap>" + dugmeler + "</td>";

    satirBilgi = satirBilgi.replaceAll("undefined", "");
    var satirBilgisi = "<tr id='sttb_" + kod + "'>" + satirBilgi + dugmeler + "</tr>";

    if (degisecekSatir) {
        degisecekSatir.html(satirBilgi + dugmeler);
    }
    else
        tabloSatirAlan.append(satirBilgisi);

    $("#" + tabloAdi).show();
    $("#div" + tabloAdi + "VeriYok").html("");
}

function DataTablesTarihDonustur(data) {
    return moment(data, 'YYYY-MM-DD').format('DD.MM.YYYY');
}

function DataTablesSayiFormatliDonustur(data) {
    return moment(data, 'YYYY-MM-DD').format('DD.MM.YYYY');
}

var DataTablesBaslikAyarla = {
    init: function (dt, node, config) {
        var table = dt.table().context[0].nTable;
        if (table) config.title = $(table).data('export-title')
    },
    title: 'Risk Uygulaması'
};

var exportPDF = {
    orientation: 'landscape',
    //pageSize: 'LEGAL',
}//PDF Export sırasında sayfanın landscape olması için



try {
    if (gTabloKolonSayisi < 8)//gTabloKolonSayisi değişkeni \Views\Shared\Components\DataTable\Default.cshtml dosyasında tanımlanıyor
        exportPDF = {};//PDF Export sırasında sayfanın portrait olması için
} catch { }



//Datatables kontrolü için button değerleri
var dataTablesButtons =
    [
        {
            extend: 'collection',
            text: '<i class="fal fa-download"></i>',
            className: 'btn-outline-default btn-sm ml-5 mr-1 with-data-toggle genelTooltip',
            titleAttr: 'Listeyi Dışa Aktar',
            buttons: [

                $.extend(true, {}, DataTablesBaslikAyarla, {
                    extend: 'pdfHtml5',
                    orientation: 'landscape',
                    pageSize: 'A3',
                    ...exportPDF,
                    text: '<i class="fal fa-file-pdf mr-1"></i>PDF',
                    exportOptions: {
                        columns: ':not(.NotExport)',
                        modifier: {
                            selected: null
                        }
                    }
                }),
                $.extend(true, {}, DataTablesBaslikAyarla, {
                    extend: 'excelHtml5',
                    text: '<i class="fal fa-file-excel mr-1"></i>Excel',
                    exportOptions: {
                        columns: ':not(.NotExport)',
                        modifier: {
                            selected: null
                        }
                    }
                }),
                {
                    charset: 'UTF-8',
                    //fieldSeparator: ';',
                    bom: true,
                    extend: 'csvHtml5',
                    text: '<i class="fal fa-table mr-1"></i>CSV',
                    exportOptions: {
                        columns: ':not(.NotExport)',
                        modifier: {
                            selected: null
                        }
                    }
                },
                {
                    extend: 'copyHtml5',
                    title: '',
                    exportOptions: {
                        columns: ':not(.NotExport)',
                        modifier: {
                            selected: null
                        }
                    }
                    // text: '<i class="fal fa-copy mr-1"></i>@CustomResource["Dugme.Kopyala"]',
                },
                $.extend(true, {}, DataTablesBaslikAyarla, {
                    extend: 'print',
                    orientation: 'landscape',
                    pageSize: 'A3',
                    exportOptions: {
                        columns: ':not(.NotExport)',
                        modifier: {
                            selected: null
                        }
                    }
                    // text: '<i class="fal fa-print mr-1"></i>@CustomResource["Dugme.Yazdir"]',
                })
            ]
        },
        {
            extend: 'colvis',
            postfixButtons: ['colvisRestore'],
            text: '<i class="fal fa-columns"></i>',
            className: 'btn-outline-default btn-sm mr-1 with-data-toggle genelTooltip',
            titleAttr: 'Kolon İşlemleri'
        }
    ];

////Datatables kontrolü için dom değeri
var dataTablesDOM =
    "<'row mb-1'<'col-sm-12 col-md-6 d-flex align-items-left justify-content-start'><'col-sm-12 col-md-6 d-flex align-items-right justify-content-end'Bl>>" +
    "<'row'<'col-sm-12'tr>>" +
    "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 text-right'p>>";

var dataTablesLangTr = {
    "emptyTable": "<div class='text-center text-primary'><img src='img/images/no-data-found.png' alt='' /><br /><h6>Gösterilecek kayıt yok</h6></div>",
    "info": "_TOTAL_ kayıttan _START_ - _END_ arasındaki kayıtlar gösteriliyor",
    "infoEmpty": "Kayıt yok",
    "infoFiltered": "(_MAX_ kayıt içerisinden bulunan)",
    "infoThousands": ".",
    "lengthMenu": "_MENU_",
    "loadingRecords": "Yükleniyor...",
    "processing": '<div class="d-flex align-items-center justify-content-center fs-lg"><div class="spinner-border spinner-border-sm text-primary mr-2" role="status"><span class="sr-only"> Loading...</span></div> İşleniyor...</div>',
    "search": '<div class="input-group-text d-inline-flex width-3 align-items-center justify-content-center border-bottom-right-radius-0 border-top-right-radius-0 border-right-0"><i class="fal fa-search"></i></div>',
    "searchPlaceholder": "Ara",
    "paginate": {
        "first": "İlk",
        "last": "Son",
        "next": "Sonraki",
        "previous": "Önceki"
    },
    "aria": {
        "sortAscending": ": artan sütun sıralamasını aktifleştir",
        "sortDescending": ": azalan sütun sıralamasını aktifleştir"
    },
    "select": {
        "rows": {
            "_": "",
            "1": "1 kayıt seçildi"
        },
        "cells": {
            "_": " ",
            "1": "1 hücre seçildi"
        },
        "columns": {
            "_": " ",
            "1": "1 sütun seçildi"
        }
    },
    "thousands": ".",
    "decimal": ",",
    "buttons": {
        "colvis": "Sütun görünürlüğü",
        "colvisRestore": "İlk haline getir",
        "copySuccess": {
            "1": "1 satır panoya kopyalandı",
            "_": "%ds satır panoya kopyalandı"
        },
        "copyTitle": "Panoya kopyala",
        "csv": "CSV",
        "excel": "Excel",
        "pageLength": {
            "-1": "Hepsi",
            "_": "%d satır göster"
        },
        "pdf": "PDF",
        "print": "Yazdır",
        "copy": "Kopyala",
    },
}

//Datatables kontrolü için init değerler
try {
    $.extend($.fn.dataTable.defaults, {
        processing: true,
        stateSave: false,
        serverSide: true,
        filter: true,
        responsive: true,
        select: true,
        dom: dataTablesDOM,
        buttons: dataTablesButtons,
        //language: {
        //    url: "https://cdn.datatables.net/plug-ins/1.10.25/i18n/Turkish.json"
        //}
        language: dataTablesLangTr,
    });
} catch (e) {

}

function YardimGoster(sayfaAdi) {

    var kontrol = $("#divDinamikYardimAlani").html();
    if (kontrol == undefined)
        $("<div id='divDinamikYardimAlani'></div>").appendTo("body");
    else {
        $("#divDinamikYardimAlani").html("");
    }

    $.ajax({
        type: "GET",
        url: "/ViewComponent/YardimGoster",
        data: { sayfaAdi: sayfaAdi },
        success: function (result) {
            $("#divDinamikYardimAlani").html(result);
            //$("#pnlYardimTamEkran").trigger('click');
            jModalShow("mdlDinamikYardimModal", "Yardım");//@CustomResource["Baslik.Tarihce"]
        }
    });

}

function TarihceGoster(ilgiKod) {

    var kontrol = $("#divDinamikTarihceAlani").html();
    if (kontrol == undefined)
        $("<div id='divDinamikTarihceAlani'></div>").appendTo("body");
    else {
        $("#divDinamikTarihceAlani").html("");
    }

    $.ajax({
        type: "GET",
        url: "/ViewComponent/TarihceGoster",
        data: { ilgiKod: ilgiKod },
        success: function (result) {
            $("#divDinamikTarihceAlani").html(result);

            jModalShow("mdlDinamikTarihceModal", "Tarihçe");//@CustomResource["Baslik.Tarihce"]
        }
    });

}

function EtkiOlasilikMatrisiGoster() {

    var kontrol = $("#divDinamikTabloAlani").html();
    if (kontrol == undefined)
        $("<div id='divDinamikTabloAlani'></div>").appendTo("body");
    else {
        $("#divDinamikTabloAlani").html("");
    }

    $.ajax({
        type: "GET",
        url: "/ViewComponent/EtkiOlasilikMatrisiGoster",

        success: function (result) {
            $("#divDinamikTabloAlani").html(result);

            jModalShow("mdlDinamikEtkiOlasilikMatrisiModal", "Etki / Olasılık Matrisi");
        }
    });
}

function KontrolKriterMatrisiGoster() {

    var kontrol = $("#divDinamikTabloAlani").html();
    if (kontrol == undefined)
        $("<div id='divDinamikTabloAlani'></div>").appendTo("body");
    else {
        $("#divDinamikTabloAlani").html("");
    }

    $.ajax({
        type: "GET",
        url: "/ViewComponent/KontrolKriterMatrisiGoster",

        success: function (result) {
            $("#divDinamikTabloAlani").html(result);

            jModalShow("mdlDinamikKontrolKriterMatrisiModal", "Kontrol Kriter Matrisi");
        }
    });
}

function IslemYapiliyorBasla(kontrol) {

    kontrol = $("#" + kontrol);

    kontrol.prop("disabled", true);
    kontrol.html(kontrol.attr("data-bekle-text"));
}

function IslemYapiliyorBitir(kontrol) {

    kontrol = $("#" + kontrol);

    kontrol.prop("disabled", false);
    kontrol.html(kontrol.attr("data-orjinal-text"));
}

function CalisiyorDugmeOlustur() {

    $(".spinnerButton").each(function () {
        var bekleYazi = $(this).attr("data-bekle-text");
        if (bekleYazi == undefined) {
            bekleYazi = "İşlem Yapılıyor...";
        }

        var bekleHtml = "<span class='spinner-border spinner-border-sm mr-2'></span>" + bekleYazi;

        $(this).attr("data-bekle-text", bekleHtml);
        $(this).attr("data-orjinal-text", $(this).html());

        //Bu işlem olursa form validate işleminde düğme çalışıyor gösteriyor
        //$(this).click(function () {
        //    IslemYapiliyorBasla($(this));
        //});
    });
}

//Verilen Baglantı koduna göre dosyayı indirme
function DinamikDosyaBaglantiKoduylaIndir(baglantiKod) {

    $.ajax({
        url: "/Dosya/DosyaIndirBaglanti",
        type: "POST",
        data: { baglantiKod: baglantiKod },
        xhrFields: {
            responseType: 'blob'
        },
        success: function (blob, status, xhr) {

            DosyaIndirJS(blob, status, xhr);

        },
        error: function (jqXHR, exception) {
            IslemYapiliyorBitir($("#btnDosyaIndir" + baglantiKod))
            alert(exception);
        },
    });
}

function DosyaIndirJS(blob, status, xhr) {
    // gelen dosya adını al
    var filename = "";
    var disposition = xhr.getResponseHeader('Content-Disposition');
    if (disposition && disposition.indexOf('attachment') !== -1) {
        var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
        var matches = filenameRegex.exec(disposition);
        if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
    }

    if (typeof window.navigator.msSaveBlob !== 'undefined') {
        // IE workaround for "HTML7007: One or more blob URLs were revoked by closing the blob for which they were created. These URLs will no longer resolve as the data backing the URL has been freed."
        window.navigator.msSaveBlob(blob, filename);
    } else {
        var URL = window.URL || window.webkitURL;
        var downloadUrl = "";

        try {
            downloadUrl = URL.createObjectURL(blob);

            if (filename) {
                // use HTML5 a[download] attribute to specify filename
                var a = document.createElement("a");
                // safari doesn't support this yet
                if (typeof a.download === 'undefined') {
                    window.location.href = downloadUrl;
                } else {
                    a.href = downloadUrl;
                    a.download = filename;
                    document.body.appendChild(a);
                    a.click();
                }
            } else {
                window.location.href = downloadUrl;
            }

            setTimeout(function () { URL.revokeObjectURL(downloadUrl); }, 100); // cleanup

        } catch (e) {
            alert(e);

        }
    }
}

function AktifRolSecGoster() {

    var kontrol = $("#divDinamikTabloAlani").html();
    if (kontrol == undefined)
        $("<div id='divDinamikTabloAlani'></div>").appendTo("body");
    else {
        $("#divDinamikTabloAlani").html("");
    }

    $.ajax({
        type: "GET",
        url: "/ViewComponent/AktifRolSecGoster",

        success: function (result) {
            $("#divDinamikTabloAlani").html(result);

            jModalShow("mdlAktifRolSecModal", "Aktif Rol Seçimi");
        }
    });
}

function DosyaKontrolGoster(dosyaKontrol, baglantiKod, zorunlu, maxDosyaSayisi, silmeYetkisi, sadeceOkuma) {

    var modalGoster = false;
    if (dosyaKontrol == undefined) dosyaKontrol = "";

    if (dosyaKontrol == "") {
        //Popup yapıda gösterilecek
        var dinamikKontrol = $("#divDinamikDosyaKontrolAlani");
        //Daha önce yaratılmış ise onu kullan
        if (dinamikKontrol.html() == undefined)
            $("<div id='divDinamikDosyaKontrolAlani'></div>").appendTo("body");

        dosyaKontrol = $("#divDinamikDosyaKontrolAlani")
        modalGoster = true;
    }

    $(dosyaKontrol).html("");

    if (zorunlu == undefined) zorunlu = false;
    if (maxDosyaSayisi == undefined) maxDosyaSayisi = 999;
    if (silmeYetkisi == undefined) silmeYetkisi = true;
    if (sadeceOkuma == undefined) sadeceOkuma = false;

    var form = {
        BaglantiKod: baglantiKod,
        ModalGoster: modalGoster,
        Zorunlu: zorunlu,
        MaxDosyaSayisi: maxDosyaSayisi,
        SilmeYetkisi: silmeYetkisi,
        SadeceOkuma: sadeceOkuma
    };

    $.ajax({
        type: "GET",
        url: "/ViewComponent/DosyaKontrolGoster",
        data: { form: JSON.stringify(form) },
        success: function (result) {
            $(dosyaKontrol).html(result);

            if (modalGoster)
                jModalShow("mdlDosyaKontrolModal", "Dosya İşlemleri");
        }
    });
}

function GuidUret() {
    return 'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx'.replace(/[xy]/g,
        function (c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
}

var pencereVerisiDegisti = false;//Eğer Bulgu kaydında değişiklik olduysa Denetim listesi yenilensin
function PenceredeGoster(kontrol, src) {
    $(kontrol).lightGallery({
        counter: false,
        showThumbByDefault: false,
        thumbnail: false,
        closable: false,
        loop: false,
        dynamic: true,
        html: true,
        mobileSrc: true,
        zoom: false,
        download: false,
        fullScreen: false,
        iframeMaxHeight: "85%",
        iframeMaxWidth: "85%",
        escKey: false,
        dynamicEl: [
            { "iframe": "true", "src": src },
        ]
    });

}

function toBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve({ adi: file["name"], icerik: reader.result });
        reader.onerror = error => reject(error);
    });
};

async function tobase64Handler(files) {
    const filePathsPromises = [];

    $.each(files, function (idx, file) {
        filePathsPromises.push(toBase64(file));
    });

    const filePaths = await Promise.all(filePathsPromises);
    const mappedFiles = filePaths.map(base64File => base64File);
    return mappedFiles;
}

function SayfaDatasi() {

    return [
        { id: "Surec", text: "Süreç/Alt Süreç Tanımları" },
        { id: "RiskEvreni", text: "Risk Evreni" },
        { id: "RiskEvreniOnay", text: "Risk Evreni Onay İşlemleri" },
        { id: "AnahtarRiskGostergesi", text: "Anahtar Risk Göstergesi" },
        { id: "RisklerinYonetilmesi", text: "Risklerin Yönetilmesi" },
        { id: "RisklerinYonetilmesiOnay", text: "Risklerin Yönetilmesi Onay İşlemleri" },
        { id: "RiskAzaltmaPlani", text: "Risk Azaltma Planı" },
        { id: "RiskAzaltmaPlaniOnay", text: "Risk Azaltma Planı Onay İşlemleri" },
        { id: "OlayRaporlamasi", text: "Olay Raporlaması" },
        { id: "OlayRaporlamasiOnay", text: "Olay Raporlaması Onay İşlemleri" },
        { id: "IcDenetim", text: "İç Denetim" },
        { id: "DisDenetim", text: "Dış Denetim" },
        { id: "EylemPlani", text: "Eylem Planı" },
        { id: "IzlemeTakip", text: "İzleme Takip Denetimi" },
        { id: "BulguyaCevap", text: "Bulguya Cevap" },
        { id: "StratejikPlanlama", text: "Stratejik Planlama" },
        { id: "StratejikPlanlamaIzleme", text: "Stratejik Planlama İzleme" },
        { id: "BilgiMerkezi", text: "Bilgi Merkezi" },
        { id: "BildirimSistemi", text: "Bildirim Sistemi" },
        { id: "Raporlar", text: "Raporlar" },
        { id: "RiskKategorisiTanimlama", text: "Risk Kategorisi Tanımları" },
        { id: "OlayKategorisiTanimlama", text: "Olay Kategorisi Tanımları" },
        { id: "GenelTanimlama", text: "Genel Tanımlar" },
        { id: "Konfigurasyon", text: "Konfigürasyon" },
        { id: "Yardim", text: "Yardım Tanımlama" },
    ];
}

function SayfaAdiVer(gelen) {

    for (var i = 0; i < sayfaData.length; i++) {
        if (sayfaData[i].id == gelen)
            return sayfaData[i].text;
    }
    return "";
}

//Dosya yükleme Bağlı kaydı Yeni kayıt ise, bağlı kayıt kod numarası aldıktan sonra yüklenen dosyaların bağlı kodlarını değiştirmek için
function DinamikDosyaBaglantiKodGuncelle(eskiKod, yeniKod) {

    if (eskiKod == undefined || eskiKod == "") return;
    if (yeniKod == undefined || yeniKod == "") return;

    //var eskiKod = $("#hdnTmpBagliKod").val();

    $.ajax({
        url: "/Dosya/BaglantiKodGuncelle",
        data: { eskiKod: eskiKod, yeniKod: yeniKod },
        type: "POST",
        success: function (result) {

            if (result.IslemSonuc) {
            }
            else {
            }
        }
    });
}


//DOsya ekleme düğmesi üstünde o kayda ait kaç tane dosya var göstermek için
var dosyaEkleDugmesiAdi = "";
function DosyaDugmesineSayiOkuYaz(baglantiKod, dugmeKontrol) {

    dosyaEkleDugmesiAdi = dugmeKontrol;
    if (dugmeKontrol == "") return;

    $.ajax({
        url: "/Dosya/DosyaSayiVer",
        type: "POST",
        data: { baglantiKod: baglantiKod },
        success: function (result) {
            var adet = result.Nesne;

            DosyaDugmesineSayiYaz(adet, dugmeKontrol);
        }
    });
}
function DosyaDugmesineSayiYaz(adet, dugmeKontrol) {
    var baslik = 'Ekli Dosyalar';

    $(dosyaEkleDugmesiAdi).html(baslik + "<sup><span class='badge bg-danger-500 fs-nano ml-2'>" + adet + "</span></sup>");
}

//Bir sayfayı modal pencerede gösterirken menu vb. alanları gizlemek için
function SadeceIcerikGoster() {
    $(".page-header").hide();
    $(".page-sidebar").hide();

    $("body").removeClass("nav-function-fixed");
    $(".breadcrumb").hide();
    $(document).ready(function () {
        $(".page-footer").hide();
        $("#navShortcutMenu").hide();
    });
}

function KoordinatorlukDoldur(kontrol) {
    $.ajax({
        url: "/TanimKoordinatorluk/SelectListesiVer",
        type: "POST",
        success: function (result) {
            Select2Yap(kontrol, true, result);

            result.push({ "id": "9999", "text": "İl Koordinatörlüğü" });
            Select2Yap(kontrol, true, result);
        }
    });
}

function RiskNoRender(jenerikRisk, kontrolEdildi, deger) {
    if (jenerikRisk == 1)
        return "<b style='color: #e51a1a;font-size: 12px;'>" + deger + "*</b>";
    else if (kontrolEdildi == 1)
        return "<b style='font-size: 12px;'>" + deger + "&nbsp;&#x2713;</b>";
    return deger;
}

