"use strict";

var initDatatable = function () {
    var datatable = $('#islem_yapilan_izinler_table').DataTable({
        "info": true,
        'order': [],
        'pageLength': 10,
        'lengthMenu': [10, 20, 50, 100],
        'buttons': [
            'copyHtml5',
            'excelHtml5',
            'csvHtml5',
            'pdfHtml5'
        ],
        language: {
            infoEmpty: 'Kayıt bulunamadı!',
            sEmptyTable: `<div class="d-flex flex-column flex-center">\n<div class="fs-1 fw-bolder text-dark">Kayıt bulunamadı!</div>\n <div class="fs-6"></div>\n </div>`,
            sZeroRecords: `<div class="d-flex flex-column flex-center">\n<div class="fs-1 fw-bolder text-dark">Kayıt bulunamadı!</div>\n <div class="fs-6"></div>\n </div>`,
            sInfo: "Toplam _TOTAL_ kayıttan _START_ - _END_ arası gösteriliyor",
            infoFiltered: "(_MAX_ kayıt arasından filtrelendi)",
            search: '<span>Dinamik Filtre:</span> _INPUT_',
            searchPlaceholder: 'Tabloda ara...',
            lengthMenu: '<span class="fw-bold fs-7">Listelenen</span> _MENU_ ',
            loadingRecords: `<div class="d-flex flex-column flex-center">\n<div class="fs-1 fw-bolder text-dark">Yükleniyor</div>\n <div class="fs-6">Kayıtlar getiriliyor..</div>\n </div>`,
            processing: `<div class="d-flex flex-column flex-center">\n<div class="fs-1 fw-bolder text-dark">Yükleniyor</div>\n <div class="fs-6">Kayıtlar getiriliyor..</div>\n </div>`,
            language: {
                emptyTable: `<div class="d-flex flex-column flex-center">\n<div class="fs-1 fw-bolder text-dark">Kayıt bulunamadı!</div>\n <div class="fs-6"></div>\n </div>`
            },
            select: {
                rows: {
                    _: "%d satır seçildi.",
                    0: "Satırlar seçilebilir.",
                    1: "1 satır seçildi."
                }
            }
        },
        ajax: {
            url: '/admin/islemler/izinler/islem-yapilanlar',
            type: 'GET',
            dataType: 'json'
        },
        'columns': [
            { data: 'id' },
            { data: 'istekTarihi' },
            { data: 'adSoyad' },
            { data: 'unvan' },
            { data: 'birim' },
            { data: 'dogrulamaYontemi' },
            { data: 'genelIzinDurumu' },
        ],
        columnDefs: [
            {
                targets: 0,
                visible: false,
                render: function (data, type, row) {
                    return data;
                }
            },
            {
                targets: 5,
                render: function (data, type, row) {
                    var content = '';
                    if (data.toLowerCase() == "e-posta") {
                        content = `<img alt="e-posta" class="mh-20px" src="/assets/media/svg/misc/mailbox.svg">`;
                    } else if (data.toLowerCase() == "sms") {
                        content = `<img alt="sms" class="mh-20px" src="/assets/media/svg/misc/sms.svg">`;
                    }

                    return `<div class="d-flex">
                                <div class="me-3">${content}</div>
                                <div>${data}</div>
                            </div>`;
                }
            },
            {
                targets: 6,
                render: function (data, type, row) {
                    return getBadge(data);
                }
            },
            {
                targets: 7,
                render: function (data, type, row) {
                    return `<button onClick="setIzinDetay(${row['id']})" type="button" class="btn btn-sm btn-light-info fs-8 mx-1" data-bs-toggle="modal" data-bs-target="#izin-detay">
                                  <i class="fa-sharp fa-solid fa-eye fs-5"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>
                                  <span>Görüntüle</span>
                               </button>`;
                }
            }
        ]
    });

    datatable.on('draw', function () {
        KTMenu.createInstances();
    });

    // Hook export buttons
    var exportButtons = new $.fn.dataTable.Buttons(datatable, {
        buttons: [
            {
                extend: 'copyHtml5',
                title: 'İşlem Yapılan İzinler'
            },
            {
                extend: 'excelHtml5',
                title: 'İşlem Yapılan İzinler'
            },
            {
                extend: 'csvHtml5',
                title: 'İşlem Yapılan İzinler'
            },
            {
                extend: 'pdfHtml5',
                title: 'İşlem Yapılan İzinler'
            }
        ]
    }).container().appendTo($('#islem_yapilan_izinler_table_buttons'));

    // Hide
    $(".dataTables_filter").addClass("d-none");
    $(".dt-buttons").addClass("d-none");

    // Hook dropdown menu click event to datatable export buttons
    const exportButtonsDropdown = document.querySelectorAll('[data-kt-export]');
    exportButtonsDropdown.forEach(exportButton => {
        exportButton.addEventListener('click', e => {
            e.preventDefault();
            const exportValue = e.target.getAttribute('data-kt-export');
            const target = document.querySelector('.dt-buttons .buttons-' + exportValue);
            target.click();
        });
    });

    // Search Datatable
    const filterSearch = document.querySelector('[data-kt-filter="search"]');
    filterSearch.addEventListener('keyup', function (e) {
        datatable.search(e.target.value).draw();
    });
};

function setIzinHareketleri(izinId) {
    $.ajax({
        url: `/izinhareketleri/izin/${izinId}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            $('#izin-hareketleri-not-found').removeClass('d-none');
            $('#izin-hareketleri-table-wrapper').removeClass('d-none');

            if (data && data.length > 0) {
                $('#izin-hareketleri-not-found').addClass('d-none');

                var element = document.getElementById('izin-hareketleri-table-wrapper');

                var tbody = "";

                data.forEach(function (item, index) {
                    var islemYapanCalisanAdSoyad = item.islemYapanCalisanAdSoyad;
                    var islemYapanCalisanUnvan = item.islemYapanCalisanUnvan;
                    var islemTarihi = item.islemTarihi;
                    var izinDurum = item.izinDurum;
                    var isleminIzinOnayTanimi = item.isleminIzinOnayTanimi == "" ? "-" : item.isleminIzinOnayTanimi;

                    tbody += `<tr>
                                    <th>${index + 1}</th>
                                    <td>${islemTarihi}</th >
                                    <td>${isleminIzinOnayTanimi}</td>
                                    <td class="text-center">${islemYapanCalisanAdSoyad}<br />${islemYapanCalisanUnvan}</th >
                                    <td>${getBadge(izinDurum)}</td>
                              </tr>`
                });

                element.innerHTML = `<table class="table table-bordered">
                                      <thead>
                                        <tr>
                                          <th>Sıralama</th>
                                          <th>İşlem Tarihi</th>
                                          <th>İşlem Yapanın Onay Tanım Grubu</th>
                                          <th class="text-center">İşlem Yapan Kişi</th>
                                          <th>Yapılan İşlem</th>
                                        </tr>
                                      </thead>
                                      <tbody>
                                            ${tbody}
                                      </tbody>
                                 </table>`;

            } else {
                $('#izin-hareketleri-table-wrapper').addClass('d-none');
            }
        },
        error: function (xhr, status, error) {
            toastr.error("Beklenmeyen bir hata oluştu!");
        }
    });
}

function setIzinFormVerileri(izinId) {
    $.ajax({
        url: `/izin-form-tab-verileri/izin/${izinId}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {

            var telefon = data.telefon;
            var adres = data.adres;
            var aciklama = data.aciklama;
            var yerineBakacakKisi = data.yerineBakacakKisi;
            var izinTur = data.izinTur;
            var mahsubenBaslangicTarihi = data.mahsubenBaslangicTarihi;
            var mahsubenGunSayisi = data.mahsubenGunSayisi;

            $("#telefonVal").parent().removeClass("d-none");
            $("#adresVal").parent().removeClass("d-none");
            $("#aciklamaVal").parent().removeClass("d-none");
            $("#goreviniYapacakKisiVal").parent().removeClass("d-none");
            $("#yillikIzinUcretiIstegiVal").parent().removeClass("d-none");
            $("#mahsubenGunSayisiVal").parent().removeClass("d-none");
            $("#mahsubenBaslangicTarihiVal").closest('tr').removeClass("d-none");


            if (izinTur != "Senelik")
                $("#yillikIzinUcretiIstegiVal").parent().addClass("d-none");

            if (telefon == null)
                $("#telefonVal").parent().addClass("d-none");

            if (adres == null)
                $("#adresVal").parent().addClass("d-none");

            if (aciklama == null)
                $("#aciklamaVal").parent().addClass("d-none");

            if (yerineBakacakKisi == null)
                $("#goreviniYapacakKisiVal").parent().addClass("d-none");

            if (mahsubenBaslangicTarihi == null) {
                $("#mahsubenBaslangicTarihiVal").closest('tr').addClass("d-none");
                $("#mahsubenGunSayisiVal").parent().addClass("d-none");
            }

            $("#telefonVal").html(telefon);
            $("#adresVal").html(adres);
            $("#aciklamaVal").html(aciklama);
            $("#goreviniYapacakKisiVal").html(yerineBakacakKisi);
            $("#izinTurVal").html(izinTur);

            $('#gunSayisiVal').html(data.toplamGunSayisi);
            $("#izinDurumuVal").html(getBadge(data.izinDurumu));
            $("#istekTarihiVal").html(data.istekTarihi);
            $("#baslangicTarihiVal").html(data.baslangicTarihi);
            $("#bitisTarihiVal").html(data.bitisTarihi);
            $("#iseBaslamaTarihiVal").html(data.iseBaslamaTarihi);
            $("#yillikIzinUcretiIstegiVal").html(data.yillikIzinUcretiIstegi);
            $("#mahsubenBaslangicTarihiVal").html(data.mahsubenBaslangicTarihi);
            $("#mahsubenGunSayisiVal").html(data.mahsubenGunSayisi);
            $("#mahsubenBitisTarihiVal").html(data.bitisTarihi);
        },
        error: function (xhr, status, error) {
            toastr.error("Beklenmeyen bir hata oluştu!");
        }
    });
}

function setIzinDetay(izinId) {
    setIzinHareketleri(izinId);
    setIzinFormVerileri(izinId);
}

// On document ready
$(function () {
    initDatatable();
});