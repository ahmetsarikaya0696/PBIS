"use strict";
const eventHandlers = {};

const maskOptions = {
    "mask": "(599) 999 99 99",
};


const textTipIzinForm = document.getElementById('text-tip-izin-form');
const formTipIzinForm = document.getElementById('form-tip-izin-form');

const formTipIzinUpdateForm = document.getElementById('form-tip-izin-update-form');
const textTipIzinUpdateForm = document.getElementById('text-tip-izin-update-form');

// Initialize Steppers
var formTipElement = document.querySelector("#kt_stepper_example_vertical");
var formTipIzinStepper = new KTStepper(formTipElement);

var textTipIzinElement = document.querySelector("#kt_stepper_example_vertical-2");
var textTipIzinStepper = new KTStepper(textTipIzinElement);

var formTipIzinUpdateElement = document.querySelector("#kt_stepper_example_vertical-update");
var formTipIzinUpdateStepper = new KTStepper(formTipIzinUpdateElement);

var textTipIzinUpdateElement = document.querySelector("#kt_stepper_example_vertical-2-update");
var textTipIzinUpdateStepper = new KTStepper(textTipIzinUpdateElement);

var modalBtn = document.getElementById('modal-button');
var calculateDatesForFormTipBtn = document.getElementById('calculateDatesBtn');

var initDatatable = function () {
    var datatable = $('#izinlerim_table').DataTable({
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
            url: '/izinlerim',
            type: 'GET',
            dataType: 'json',
        },
        'columns': [
            { data: 'id' },
            { data: 'istekTarihi' },
            { data: 'baslangicTarihi' },
            { data: 'bitisTarihi' },
            { data: 'iseBaslamaTarihi' },
            { data: 'adim' },
            { data: 'izinDurumu' },
            { data: 'izinTur' },
            { data: 'isForm' },
            { data: 'sabitGunSayisi' },
            { data: 'dogrulamaYontemi' },
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
                targets: 1,
                class: 'text-center',
                render: function (data, type, row) {
                    return `<span>${data}</span>`;
                }
            },
            {
                targets: 2,
                render: function (data, type, row) {
                    return `<span>${data}</span>`;
                }
            },
            {
                targets: 3,
                render: function (data, type, row) {
                    return `<span>${data}</span>`;
                }
            },
            {
                targets: 4,
                render: function (data, type, row) {
                    return `<span>${data}</span>`;
                }
            },
            {
                targets: 5,
                render: function (data, type, row) {
                    return `<span class="badge badge-light-info text-center">${data}</span>`;
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
                    return `<span>${data}</span>`;
                }
            },
            {
                targets: 8,
                render: function (data, type, row) {
                    var content = '';
                    if (row['dogrulamaYontemi'].toLowerCase() == "e-posta") {
                        content = `<img alt="e-posta" class="mh-20px" src="/assets/media/svg/misc/mailbox.svg">`;
                    } else if (row['dogrulamaYontemi'].toLowerCase() == "sms") {
                        content = `<img alt="sms" class="mh-20px" src="/assets/media/svg/misc/sms.svg">`;
                    }

                    return `<div class="d-flex">
                                <div class="me-3">${content}</div>
                                <div>${row['dogrulamaYontemi']}</div>
                            </div>`;
                }
            },
            {
                targets: 9,
                orderable: false,
                render: function (data, type, row) {
                    var dogrulamaBekleniyor = row['izinDurumu'] == "Doğrulama Bekleniyor";

                    var result = "";

                    var duzenleBtn = `<div class="menu-item px-3">
                                            <a href="#" class="menu-link px-3" onClick="showDuzenleForm(${row.isForm}, ${row.id}, ${row.sabitGunSayisi})">
                                                <i class="ki-duotone ki-notepad-edit fs-5 me-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>
                                                ${dogrulamaBekleniyor ? '<span class="pulse-ring"></span>' : ''}
                                                Düzenle
                                            </a>
                                        </div>`;

                    var iptalBtn = `<div class="menu-item px-3">
                                         <a href="#" onClick="izinIsteginiIptalEt(${row.id})" class="menu-link px-3""><i class="ki-duotone ki-cross-circle fs-5 me-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>İptal Et</a>
                                   </div> `;


                    var goruntuleBtn = `<div class="menu-item px-3">
                                         <a href="#" class="menu-link px-3" data-id=${row.id} onClick="setIzinDetay(${row.id})" data-bs-toggle="modal" data-bs-target="#izin-detay"><i class="fa-sharp fa-solid fa-eye fs-5 me-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>
                                            Görüntüle</a>
                                      </div>`;

                    result += `${goruntuleBtn}`;

                    if (row['adim'].startsWith("0") || row['izinDurumu'] === "Düzeltilmek Üzere Geri Gönderildi") {
                        result += `${duzenleBtn}${iptalBtn}`;
                    }

                    return `<div class="d-flex justify-content-center ${dogrulamaBekleniyor && 'pulse pulse-danger border-5'}">
                                <div class="ms-2">
                                    <button type="button" class="btn btn-sm btn-icon btn-light btn-active-light-primary me-2" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
                                        <span class="svg-icon svg-icon-5 m-0">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
                                                <rect x="10" y="10" width="4" height="4" rx="2" fill="black" />
                                                <rect x="17" y="10" width="4" height="4" rx="2" fill="black" />
                                                <rect x="3" y="10" width="4" height="4" rx="2" fill="black" />
                                            </svg>
                                        </span>
                                        ${dogrulamaBekleniyor ? '<span class="pulse-ring"></span>' : ''}
                                    </button>
                                    <div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-bold fs-7 w-175px py-4" data-kt-menu="true">
                                         ${result == "" ? "-" : result}
                                    </div>
                                </div>
                            </div>`;
                }
            },
            {
                targets: 10,
                visible: false,
                render: function (data, type, row) {
                    return row['isForm'];
                }
            },
        ],
    });

    datatable.on('draw', function () {
        KTMenu.createInstances();
    });

    // ... Diğer işlemler
    var exportButtons = new $.fn.dataTable.Buttons(datatable, {
        buttons: [
            {
                extend: 'copyHtml5',
                title: 'İzinlerim'
            },
            {
                extend: 'excelHtml5',
                title: 'İzinlerim'
            },
            {
                extend: 'csvHtml5',
                title: 'İzinlerim'
            },
            {
                extend: 'pdfHtml5',
                title: 'İzinlerim'
            }
        ]
    }).container().appendTo($('#izinlerim_table_buttons'));

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

function showDuzenleForm(isForm, izinId, sabitGunSayisi) {
    var target = "";

    if (isForm) {
        formTipIzinUpdateStepper.goFirst();
        target = "#izin-update-modal-1";
    } else {
        textTipIzinUpdateStepper.goFirst();
        target = "#izin-update-modal-2";
    }

    // Datayı al
    $.ajax({
        url: `/izin-form-verileri/izin/${izinId}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            console.log(data);

            if (isForm) {
                flatpickr("#FormTipIzinUpdateVM_BaslangicTarihi", flatpickrConfig).setDate(data.baslangicTarihi);

                flatpickr("#FormTipIzinUpdateVM_BitisTarihi", flatpickrConfig).setDate(data.bitisTarihi);

                flatpickr("#FormTipIzinUpdateVM_IseBaslamaTarihi", flatpickrConfig).setDate(data.iseBaslamaTarihi);

                $("#FormTipIzinUpdateVM_IzinTurId").val(data.izinTurId);
                $("#FormTipIzinUpdateVM_Id").val(data.id);
                $("#FormTipIzinUpdateVM_Adres").html(data.adres);

                $("#FormTipIzinUpdateVM_Telefon").val(data.telefon.substring(1));

                $("#FormTipIzinUpdateVM_YerineBakacakKisi").val(data.yerineBakacakKisi);

                $("#FormTipIzinUpdateVM_YillikIzinUcretiIstegi").val(data.yillikIzinUcretiIstegi);
                $("#FormTipIzinUpdateVM_YillikIzinUcretiIstegi").prop('checked', data.yillikIzinUcretiIstegi);

                var mahsubenIsChecked = data.mahsubenBaslangicTarihi != null;
                $("#mahsubenUpdate").val(mahsubenIsChecked);
                $("#mahsubenUpdate").prop('checked', mahsubenIsChecked);
                $("#FormTipIzinUpdateVM_MahsubenBaslangicTarihi").prop('disabled', !mahsubenIsChecked);

                flatpickr("#FormTipIzinUpdateVM_MahsubenBaslangicTarihi", flatpickrConfig).setDate(data.mahsubenBaslangicTarihi);
                flatpickr("#mahsubenBitisUpdate", flatpickrConfig).setDate(data.bitisTarihi);



                setInputDisabledAttr(sabitGunSayisi, 'FormTipIzinUpdateVM_BaslangicTarihi', 'FormTipIzinUpdateVM_BitisTarihi', data.izinTurId);
            } else {
                flatpickr("#TextTipIzinUpdateVM_BaslangicTarihi", flatpickrConfig).setDate(data.baslangicTarihi);

                flatpickr("#TextTipIzinUpdateVM_BitisTarihi", {
                    dateFormat: "d.m.Y"
                }).setDate(data.bitisTarihi);

                flatpickr("#TextTipIzinUpdateVM_IseBaslamaTarihi", flatpickrConfig).setDate(data.iseBaslamaTarihi);

                $("#TextTipIzinUpdateVM_IzinTurId").val(data.izinTurId);
                $("#TextTipIzinUpdateVM_Id").val(data.id);
                $("#TextTipIzinUpdateVM_Aciklama").text(data.aciklama);

                setInputDisabledAttr(sabitGunSayisi, 'TextTipIzinUpdateVM_BaslangicTarihi', 'TextTipIzinUpdateVM_BitisTarihi', data.izinTurId);
            }

            // Modalı Göster
            $(target).modal('show');
        },
        error: function (xhr, status, error) {
            toastr.error("Beklenmeyen bir hata oluştu!");
        }
    });

}

function setInputsForFormTip() {
    var baslangicTarihi = document.getElementById('FormTipIzinCreateVM_BaslangicTarihi').value;
    var bitisTarihi = document.getElementById('FormTipIzinCreateVM_BitisTarihi').value;
    var mahsubenBaslangicTarihi = document.getElementById('FormTipIzinCreateVM_MahsubenBaslangicTarihi').value;
    var izinTurleri = $('#izinTurleri').val();

    var gunSayisiInput = document.getElementById('FormTipIzinCreateVM_GunSayisi');
    var mahsubenGunSayisiInput = document.getElementById('FormTipIzinCreateVM_MahsubenGunSayisi');

    $.ajax({
        url: `/izin-tarihleri?baslangicTarihi=${baslangicTarihi}&bitisTarihi=${bitisTarihi}&izinTurId=${izinTurleri}&mahsubenBaslangicTarihi=${mahsubenBaslangicTarihi}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            gunSayisiInput.value = data.gunSayisi;
            mahsubenGunSayisiInput.value = data.mahsubenGunSayisi;

            flatpickr("#FormTipIzinCreateVM_IseBaslamaTarihi", flatpickrConfig).setDate(data.iseBaslamaTarihi);
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}

function setInputsForFormTipUpdate() {
    var baslangicTarihi = document.getElementById('FormTipIzinUpdateVM_BaslangicTarihi').value;
    var mahsubenBaslangicTarihi = document.getElementById('FormTipIzinUpdateVM_MahsubenBaslangicTarihi').value;
    var bitisTarihi = document.getElementById('FormTipIzinUpdateVM_BitisTarihi').value;
    var izinTurId = document.getElementById('FormTipIzinUpdateVM_IzinTurId').value;

    var gunSayisiInput = document.getElementById('FormTipIzinUpdateVM_GunSayisi');
    var mahsubenGunSayisiInput = document.getElementById('FormTipIzinUpdateVM_MahsubenGunSayisi');


    // Fetch data using AJAX
    $.ajax({
        url: `/izin-tarihleri?baslangicTarihi=${baslangicTarihi}&bitisTarihi=${bitisTarihi}&izinTurId=${izinTurId}&mahsubenBaslangicTarihi=${mahsubenBaslangicTarihi}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            console.log(data);

            gunSayisiInput.value = data.gunSayisi;
            mahsubenGunSayisiInput.value = data.mahsubenGunSayisi;

            flatpickr("#FormTipIzinUpdateVM_IseBaslamaTarihi", flatpickrConfig).setDate(data.iseBaslamaTarihi);
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}

function setInputsForTextTip() {
    var baslangicTarihi = document.getElementById('TextTipIzinCreateVM_BaslangicTarihi').value;
    var bitisTarihi = document.getElementById('TextTipIzinCreateVM_BitisTarihi').value;
    var izinTurleri = $('#izinTurleri').val();

    var gunSayisiInput = document.getElementById('TextTipIzinCreateVM_GunSayisi');

    // Fetch data using AJAX
    $.ajax({
        url: `/izin-tarihleri?baslangicTarihi=${baslangicTarihi}&bitisTarihi=${bitisTarihi}&izinTurId=${izinTurleri}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            gunSayisiInput.value = data.gunSayisi;

            flatpickr("#TextTipIzinCreateVM_IseBaslamaTarihi", flatpickrConfig).setDate(data.iseBaslamaTarihi);

            setTextTipIzinPreview();
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}

function setInputsForTextTipUpdate() {
    var baslangicTarihi = document.getElementById('TextTipIzinUpdateVM_BaslangicTarihi').value;
    var bitisTarihi = document.getElementById('TextTipIzinUpdateVM_BitisTarihi').value;
    var izinTurId = $('#TextTipIzinUpdateVM_IzinTurId').val();

    var gunSayisiInput = document.getElementById('TextTipIzinUpdateVM_GunSayisi');

    // Fetch data using AJAX
    $.ajax({
        url: `/izin-tarihleri?baslangicTarihi=${baslangicTarihi}&bitisTarihi=${bitisTarihi}&izinTurId=${izinTurId}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            gunSayisiInput.value = data.gunSayisi;

            flatpickr("#TextTipIzinUpdateVM_IseBaslamaTarihi", flatpickrConfig).setDate(data.iseBaslamaTarihi);

            setTextTipIzinUpdatePreview();
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}

function setFormTipIzinPreview() {
    var baslangicTarihi = document.getElementById('FormTipIzinCreateVM_BaslangicTarihi').value;
    var mahsubenBaslangicTarihi = document.getElementById('FormTipIzinCreateVM_MahsubenBaslangicTarihi').value;
    var mahsubenBitisTarihi = document.getElementById('mahsubenBitis').value;
    var gunSayisi = document.getElementById('FormTipIzinCreateVM_GunSayisi').value;
    var mahsubenGunSayisi = document.getElementById('FormTipIzinCreateVM_MahsubenGunSayisi').value;
    var bitisTarihi = document.getElementById('FormTipIzinCreateVM_BitisTarihi').value;
    var iseBaslamaTarihi = document.getElementById('FormTipIzinCreateVM_IseBaslamaTarihi').value;
    var telefon = document.getElementById('FormTipIzinCreateVM_Telefon').value;
    var yerineBakacakKisi = document.getElementById('FormTipIzinCreateVM_YerineBakacakKisi').value;
    var yillikIzinUcretiIstegi = document.getElementById('FormTipIzinCreateVM_YillikIzinUcretiIstegi').value;

    if (!mahsubenBaslangicTarihi) {
        document.getElementById('mahsubenBaslangicTarihiSpan').closest('tr').classList.add('d-none');
        document.getElementById('mahsubenGunSayisiSpan').closest('tr').classList.add('d-none');
    } else {
        document.getElementById('mahsubenBaslangicTarihiSpan').closest('tr').classList.remove('d-none');
        document.getElementById('mahsubenGunSayisiSpan').closest('tr').classList.remove('d-none');
    }

    document.getElementById('baslangicTarihiSpan').innerText = baslangicTarihi;
    document.getElementById('mahsubenBaslangicTarihiSpan').innerText = mahsubenBaslangicTarihi;
    document.getElementById('mahsubenBitisTarihiSpan').innerText = mahsubenBitisTarihi;
    document.getElementById('bitisTarihiSpan').innerText = bitisTarihi;
    document.getElementById('iseBaslamaTarihiSpan').innerText = iseBaslamaTarihi;
    document.getElementById('gunSayisiSpan').innerText = gunSayisi;
    document.getElementById('mahsubenGunSayisiSpan').innerText = mahsubenGunSayisi;
    document.getElementById('cepTelSpan').innerText = telefon;
    document.getElementById('yerineBakacakKisiSpan').innerText = yerineBakacakKisi;
    document.getElementById('yillikIzinUcretiIstegiSpan').innerText = yillikIzinUcretiIstegi === 'true' ? 'Evet' : 'Hayır';
}

function setFormTipIzinUpdatePreview() {
    var baslangicTarihi = document.getElementById('FormTipIzinUpdateVM_BaslangicTarihi').value;
    var mahsubenBaslangicTarihi = document.getElementById('FormTipIzinUpdateVM_MahsubenBaslangicTarihi').value;
    var bitisTarihi = document.getElementById('FormTipIzinUpdateVM_BitisTarihi').value;
    var mahsubenBitisTarihi = document.getElementById('mahsubenBitisUpdate').value;
    var iseBaslamaTarihi = document.getElementById('FormTipIzinUpdateVM_IseBaslamaTarihi').value;
    var gunSayisi = document.getElementById('FormTipIzinUpdateVM_GunSayisi').value;
    var mahsubenGunSayisi = document.getElementById('FormTipIzinUpdateVM_MahsubenGunSayisi').value;
    var telefon = document.getElementById('FormTipIzinUpdateVM_Telefon').value;
    var yerineBakacakKisi = document.getElementById('FormTipIzinUpdateVM_YerineBakacakKisi').value;
    var yillikIzinUcretiIstegi = document.getElementById('FormTipIzinUpdateVM_YillikIzinUcretiIstegi').value;

    if (!mahsubenBaslangicTarihi) {
        document.getElementById('mahsubenBaslangicTarihiFormUpdateSpan').closest('tr').classList.add('d-none');
        document.getElementById('mahsubenGunSayisiFormUpdateSpan').closest('tr').classList.add('d-none');
    } else {
        document.getElementById('mahsubenBaslangicTarihiFormUpdateSpan').closest('tr').classList.remove('d-none');
        document.getElementById('mahsubenGunSayisiFormUpdateSpan').closest('tr').classList.remove('d-none');
    }

    document.getElementById('baslangicTarihiFormUpdateSpan').innerText = baslangicTarihi;
    document.getElementById('mahsubenBaslangicTarihiFormUpdateSpan').innerText = mahsubenBaslangicTarihi;
    document.getElementById('bitisTarihiFormUpdateSpan').innerText = bitisTarihi;
    document.getElementById('mahsubenBitisTarihiFormUpdateSpan').innerText = mahsubenBitisTarihi;
    document.getElementById('iseBaslamaTarihiFormUpdateSpan').innerText = iseBaslamaTarihi;
    document.getElementById('gunSayisiFormUpdateSpan').innerText = gunSayisi;
    document.getElementById('mahsubenGunSayisiFormUpdateSpan').innerText = mahsubenGunSayisi;
    document.getElementById('cepTelFormUpdateSpan').innerText = telefon;
    document.getElementById('yerineBakacakKisiFormUpdateSpan').innerText = yerineBakacakKisi;
    document.getElementById('yillikIzinUcretiIstegiFormUpdateSpan').innerText = yillikIzinUcretiIstegi === 'true' ? 'Evet' : 'Hayır';
}

function setTextTipIzinPreview() {
    var baslangicTarihi = document.getElementById('TextTipIzinCreateVM_BaslangicTarihi').value;
    var gunSayisi = document.getElementById('TextTipIzinCreateVM_GunSayisi').value;
    var bitisTarihi = document.getElementById('TextTipIzinCreateVM_BitisTarihi').value;
    var iseBaslamaTarihi = document.getElementById('TextTipIzinCreateVM_IseBaslamaTarihi').value;
    var aciklama = document.getElementById('TextTipIzinCreateVM_Aciklama').value;

    document.getElementById('baslangicTarihiTextSpan').innerText = baslangicTarihi;
    document.getElementById('bitisTarihiTextSpan').innerText = bitisTarihi;
    document.getElementById('iseBaslamaTarihiTextSpan').innerText = iseBaslamaTarihi;
    document.getElementById('gunSayisiTextSpan').innerText = gunSayisi;
    document.getElementById('aciklama').innerText = aciklama;
}

function setTextTipIzinUpdatePreview() {
    var baslangicTarihi = document.getElementById('TextTipIzinUpdateVM_BaslangicTarihi').value;
    var gunSayisi = document.getElementById('TextTipIzinUpdateVM_GunSayisi').value;
    var bitisTarihi = document.getElementById('TextTipIzinUpdateVM_BitisTarihi').value;
    var iseBaslamaTarihi = document.getElementById('TextTipIzinUpdateVM_IseBaslamaTarihi').value;
    var aciklama = document.getElementById('TextTipIzinUpdateVM_Aciklama').value;

    document.getElementById('baslangicTarihiTextUpdateSpan').innerText = baslangicTarihi;
    document.getElementById('bitisTarihiTextUpdateSpan').innerText = bitisTarihi;
    document.getElementById('iseBaslamaTarihiTextUpdateSpan').innerText = iseBaslamaTarihi;
    document.getElementById('gunSayisiTextUpdateSpan').innerText = gunSayisi;
    document.getElementById('aciklamaTextUpdate').innerText = aciklama;
}

async function setFormTipIzinFields(currentStepIndex) {
    if (currentStepIndex === 1) {
        setInputsForFormTip();
    } else if (currentStepIndex === 2) {
        setFormTipIzinPreview();
    } else if (currentStepIndex === 4) {
        await dogrulamaFormTipCreate();
    }
}

async function dogrulamaFormTipCreate() {
    var data = await dogrulamaKoduGonder('FormTipIzinCreateVM.DogrulamaYontemi');

    var sonKullanimTarihi = data.sonKullanimTarihi;
    var yontem = data.yontem.toUpperCase();
    var iletisimBilgisi = data.iletisimBilgisi;


    $('#formTipExpiration').html(sonKullanimTarihi);
    $('#formTipYontem').html(yontem);
    $('#formTipIletisimBilgisi').html(iletisimBilgisi);

    startCountdown('formTipExpiration', 'formTipCountdown', 'formTipCountdownAlert');

    if (yontem == "SMS") {
        $('#dogrulamaImgFormTip').attr('src', '/assets/media/svg/misc/sms.svg');
        $('#dogrulamaImgFormTip').attr('alt', 'sms');
        $('#formTipDogrulamaMesaj').html("Yukarıda belirtilen telefon numarasına gönderilen doğrulama kodunu giriniz");

    } else if (yontem == "E-POSTA") {
        $('#dogrulamaImgFormTip').attr('src', '/assets/media/svg/misc/mailbox.svg');
        $('#dogrulamaImgFormTip').attr('alt', 'e-posta');
        $('#formTipDogrulamaMesaj').html("Yukarıda belirtilen e-posta adresine gönderilen doğrulama kodunu giriniz");
    }
}

async function setTextTipIzinFields(currentStepIndex) {
    if (currentStepIndex === 1) {
        setInputsForTextTip();
    } else if (currentStepIndex === 3) {
        await dogrulamaTextTipCreate();
    }
}

async function dogrulamaTextTipCreate() {
    var data = await dogrulamaKoduGonder('TextTipIzinCreateVM.DogrulamaYontemi');

    var sonKullanimTarihi = data.sonKullanimTarihi;
    var yontem = data.yontem.toUpperCase();
    var iletisimBilgisi = data.iletisimBilgisi;


    $('#textTipExpiration').html(sonKullanimTarihi);
    $('#textTipYontem').html(yontem);
    $('#textTipIletisimBilgisi').html(iletisimBilgisi);

    startCountdown('textTipExpiration', 'textTipCountdown', 'textTipCountdownAlert');

    if (yontem == "SMS") {
        $('#dogrulamaImg').attr('src', '/assets/media/svg/misc/sms.svg');
        $('#dogrulamaImg').attr('alt', 'sms');
        $('#textTipDogrulamaMesaj').html("Yukarıda belirtilen telefon numarasına gönderilen doğrulama kodunu giriniz");

    } else if (yontem == "E-POSTA") {
        $('#dogrulamaImg').attr('src', '/assets/media/svg/misc/mailbox.svg');
        $('#dogrulamaImg').attr('alt', 'e-posta');
        $('#textTipDogrulamaMesaj').html("Yukarıda belirtilen e-posta adresine gönderilen doğrulama kodunu giriniz");
    }
}

async function setTextTipIzinUpdateFields(currentStepIndex) {
    if (currentStepIndex === 1) {
        setInputsForTextTipUpdate();
    } else if (currentStepIndex === 3) {

        await dogrulamaTextTipUpdate();
    }
}

async function dogrulamaTextTipUpdate() {
    var data = await dogrulamaKoduGonder('TextTipIzinUpdateVM.DogrulamaYontemi');

    var sonKullanimTarihi = data.sonKullanimTarihi;
    var yontem = data.yontem.toUpperCase();
    var iletisimBilgisi = data.iletisimBilgisi;


    $('#textTipUpdateExpiration').html(sonKullanimTarihi);
    $('#textTipUpdateYontem').html(yontem);
    $('#textTipUpdateIletisimBilgisi').html(iletisimBilgisi);

    startCountdown('textTipUpdateExpiration', 'textTipUpdateCountdown', 'textTipUpdateCountdownAlert');

    if (yontem == "SMS") {
        $('#dogrulamaImgTextTipUpdate').attr('src', '/assets/media/svg/misc/sms.svg');
        $('#dogrulamaImgTextTipUpdate').attr('alt', 'sms');
        $('#textTipUpdateDogrulamaMesaj').html("Yukarıda belirtilen telefon numarasına gönderilen doğrulama kodunu giriniz");

    } else if (yontem == "E-POSTA") {
        $('#dogrulamaImgTextTipUpdate').attr('src', '/assets/media/svg/misc/mailbox.svg');
        $('#dogrulamaImgTextTipUpdate').attr('alt', 'e-posta');
        $('#textTipUpdateDogrulamaMesaj').html("Yukarıda belirtilen e-posta adresine gönderilen doğrulama kodunu giriniz");
    }
}

async function setFormTipIzinUpdateFields(currentStepIndex) {
    if (currentStepIndex === 1) {
        setInputsForFormTipUpdate();
    } else if (currentStepIndex === 2) {
        setFormTipIzinUpdatePreview();
    } else if (currentStepIndex === 4) {
        await dogrulamaFormTipUpdate();
    }
}

async function dogrulamaFormTipUpdate() {
    var data = await dogrulamaKoduGonder('FormTipIzinUpdateVM.DogrulamaYontemi');

    var sonKullanimTarihi = data.sonKullanimTarihi;
    var yontem = data.yontem.toUpperCase();
    var iletisimBilgisi = data.iletisimBilgisi;


    $('#formTipUpdateExpiration').html(sonKullanimTarihi);
    $('#formTipUpdateYontem').html(yontem);
    $('#formTipUpdateIletisimBilgisi').html(iletisimBilgisi);

    startCountdown('formTipUpdateExpiration', 'formTipUpdateCountdown', 'formTipUpdateCountdownAlert');

    if (yontem == "SMS") {
        $('#dogrulamaImgFormTipUpdate').attr('src', '/assets/media/svg/misc/sms.svg');
        $('#dogrulamaImgFormTipUpdate').attr('alt', 'sms');
        $('#formTipUpdateDogrulamaMesaj').html("Yukarıda belirtilen telefon numarasına gönderilen doğrulama kodunu giriniz");

    } else if (yontem == "E-POSTA") {
        $('#dogrulamaImgFormTipUpdate').attr('src', '/assets/media/svg/misc/mailbox.svg');
        $('#dogrulamaImgFormTipUpdate').attr('alt', 'e-posta');
        $('#formTipUpdateDogrulamaMesaj').html("Yukarıda belirtilen e-posta adresine gönderilen doğrulama kodunu giriniz");
    }
}

// Configure Validations
function configureValidationsForFormTipIzin() {
    var validations = [];

    var form = document.getElementById('form-tip-izin-form');

    // Step 1 Validation
    validations.push(FormValidation.formValidation(
        form,
        {
            fields: {
                'FormTipIzinCreateVM.BaslangicTarihi': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'FormTipIzinCreateVM.BitisTarihi': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'FormTipIzinCreateVM.MahsubenBaslangicTarihi': {
                    validators: {
                        callback: {
                            message: "İzin başlangıç ve bitiş Tarihleri aralığında bir değer seçilmelidir",
                            callback: function (input) {
                                var mahsubenIzinKullanmakIstiyorum = document.getElementById('mahsubenCreate').value;

                                if (mahsubenIzinKullanmakIstiyorum == 'true') {
                                    var baslangicTarihiInput = document.getElementById('FormTipIzinCreateVM_BaslangicTarihi');
                                    var bitisTarihiInput = document.getElementById('FormTipIzinCreateVM_BitisTarihi');

                                    var baslangicTarihi = new Date(formatDate(baslangicTarihiInput.value));
                                    var bitisTarihi = new Date(formatDate(bitisTarihiInput.value));
                                    var mahsubenBaslangicTarihi = new Date(formatDate(input.value));

                                    return mahsubenBaslangicTarihi.getTime() >= baslangicTarihi.getTime() && mahsubenBaslangicTarihi <= bitisTarihi;
                                }

                                return true;
                            }
                        }
                    }
                }
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap5({
                    rowSelector: '.fv-row'
                })
            }
        }
    ));

    // Step 2 Validation
    validations.push(FormValidation.formValidation(
        form,
        {
            fields: {
                'FormTipIzinCreateVM.Adres': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'FormTipIzinCreateVM.Telefon': {
                    validators: {
                        callback: {
                            message: "Lütfen geçerli bir cep telefonu giriniz",
                            callback: function (input) {
                                return Inputmask.isValid(input.value, maskOptions);
                            }
                        }
                    }
                },
                'FormTipIzinCreateVM.YerineBakacakKisi': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                }
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap5({
                    rowSelector: '.fv-row'
                })
            }
        }
    ));

    return validations;
}

function configureValidationsForTextTipIzin() {
    var validations = [];

    var form = document.getElementById('text-tip-izin-form');

    // Step 1 Validation
    validations.push(FormValidation.formValidation(
        form,
        {
            fields: {
                'TextTipIzinCreateVM.BaslangicTarihi': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'TextTipIzinCreateVM.BitisTarihi': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'TextTipIzinCreateVM.Aciklama': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                }
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap5({
                    rowSelector: '.fv-row'
                })
            }
        }
    ));

    return validations;
}

function configureValidationsForTextTipIzinUpdate() {
    var validations = [];

    var form = document.getElementById('text-tip-izin-update-form');

    // Step 1 Validation
    validations.push(FormValidation.formValidation(
        form,
        {
            fields: {
                'TextTipIzinUpdateVM.BaslangicTarihi': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'TextTipIzinUpdateVM.BitisTarihi': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'TextTipIzinUpdateVM.Aciklama': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                }
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap5({
                    rowSelector: '.fv-row'
                })
            }
        }
    ));

    return validations;
}

function configureValidationsForFormTipIzinUpdate() {
    var validations = [];

    var form = document.getElementById('form-tip-izin-update-form');

    // Step 1 Validation
    validations.push(FormValidation.formValidation(
        form,
        {
            fields: {
                'FormTipIzinUpdateVM.BaslangicTarihi': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'FormTipIzinUpdateVM.BitisTarihi': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'FormTipIzinUpdateVM.MahsubenBaslangicTarihi': {
                    validators: {
                        callback: {
                            message: "İzin başlangıç ve bitiş Tarihleri aralığında bir değer seçilmelidir",
                            callback: function (input) {
                                var mahsubenIzinKullanmakIstiyorum = document.getElementById('mahsubenUpdate').value;

                                if (mahsubenIzinKullanmakIstiyorum == 'true') {
                                    var baslangicTarihiInput = document.getElementById('FormTipIzinUpdateVM_BaslangicTarihi');
                                    var bitisTarihiInput = document.getElementById('FormTipIzinUpdateVM_BitisTarihi');

                                    var baslangicTarihi = new Date(formatDate(baslangicTarihiInput.value));
                                    var bitisTarihi = new Date(formatDate(bitisTarihiInput.value));
                                    var mahsubenBaslangicTarihi = new Date(formatDate(input.value));

                                    return mahsubenBaslangicTarihi.getTime() >= baslangicTarihi.getTime() && mahsubenBaslangicTarihi <= bitisTarihi;
                                }

                                return true;
                            }
                        }
                    }
                }
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap5({
                    rowSelector: '.fv-row'
                })
            }
        }
    ));

    // Step 2 Validation
    validations.push(FormValidation.formValidation(
        form,
        {
            fields: {
                'FormTipIzinUpdateVM.Adres': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'FormTipIzinUpdateVM.Telefon': {
                    validators: {
                        callback: {
                            message: "Lütfen geçerli bir cep telefonu giriniz",
                            callback: function (input) {
                                return Inputmask.isValid(input.value, maskOptions);
                            }
                        }
                    }
                },
                'FormTipIzinUpdateVM.YerineBakacakKisi': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                }
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap5({
                    rowSelector: '.fv-row'
                })
            }
        }
    ));

    return validations;
}

// init stepper
function initStepperForFormTipIzin() {
    // Get Validations
    var validations = configureValidationsForFormTipIzin('form-tip-izin-form');

    // Handle next step
    formTipIzinStepper.on("kt.stepper.next", function (stepper) {
        var currentStepIndex = stepper.getCurrentStepIndex();

        if (currentStepIndex === 1) {
            setTelefonValue();
        }

        var validator = validations[currentStepIndex - 1];
        if (validator) {
            validator.validate().then(function (status) {
                if (status == 'Valid') {
                    setFormTipIzinFields(currentStepIndex);
                    stepper.goNext();
                }
            });
        } else {
            setFormTipIzinFields(currentStepIndex);
            stepper.goNext();
        }
    });

    // Handle previous step
    formTipIzinStepper.on("kt.stepper.previous", function (stepper) {
        stepper.goPrevious(); // go previous step
    });
}

function initStepperForTextTipIzin() {
    // Get Validations
    var validations = configureValidationsForTextTipIzin('text-tip-izin-form');

    textTipIzinStepper.on("kt.stepper.next", function (stepper) {
        var currentStepIndex = stepper.getCurrentStepIndex();
        var validator = validations[currentStepIndex - 1];
        if (validator) {
            validator.validate().then(function (status) {
                if (status == 'Valid') {
                    setTextTipIzinFields(currentStepIndex);
                    stepper.goNext();
                }
            });
        } else {
            setTextTipIzinFields(currentStepIndex);
            stepper.goNext();
        }
    });

    // Handle previous step
    textTipIzinStepper.on("kt.stepper.previous", function (stepper) {
        stepper.goPrevious(); // go previous step
    });
}

function initStepperForFormTipIzinUpdate() {
    // Get Validations
    var validations = configureValidationsForFormTipIzinUpdate('form-tip-izin-update-form');

    // Handle next step
    formTipIzinUpdateStepper.on("kt.stepper.next", function (stepper) {
        var currentStepIndex = stepper.getCurrentStepIndex();
        var validator = validations[currentStepIndex - 1];
        if (validator) {
            validator.validate().then(function (status) {
                if (status == 'Valid') {
                    setFormTipIzinUpdateFields(currentStepIndex);
                    stepper.goNext();
                }
            });
        } else {
            setFormTipIzinUpdateFields(currentStepIndex);
            stepper.goNext();
        }
    });

    // Handle previous step
    formTipIzinUpdateStepper.on("kt.stepper.previous", function (stepper) {
        stepper.goPrevious(); // go previous step
    });
}

function initStepperForTextTipIzinUpdate() {
    // Get Validations
    var validations = configureValidationsForTextTipIzinUpdate('text-tip-izin-update-form');

    // Handle next step
    textTipIzinUpdateStepper.on("kt.stepper.next", function (stepper) {
        var currentStepIndex = stepper.getCurrentStepIndex();
        var validator = validations[currentStepIndex - 1];
        if (validator) {
            validator.validate().then(function (status) {
                if (status == 'Valid') {
                    setTextTipIzinUpdateFields(currentStepIndex);
                    stepper.goNext();
                }
            });
        } else {
            setTextTipIzinUpdateFields(currentStepIndex);
            stepper.goNext();
        }
    });

    // Handle previous step
    textTipIzinUpdateStepper.on("kt.stepper.previous", function (stepper) {
        stepper.goPrevious(); // go previous step
    });
}

function setIzinDetay(izinId) {
    $('#ret-detay').addClass('d-none');
    var row = $(`a[data-id="${izinId}"]`).closest("tr");
    var onaylandi = row.find('td:eq(5)').text() == 'Onaylandı';
    if (onaylandi) {
        $('#download-pdf-wrapper').removeClass('d-none');
    } else {
        $('#download-pdf-wrapper').addClass('d-none');
    }

    setIzinHareketleri(izinId);
    setIzinFormVerileri(izinId);
}

function getRetDetayByIzinHareketId(id) {
    $.ajax({
        url: `/ret-detay?izinhareketid=${id}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            document.getElementById('ret-aciklama').innerText = data.detay;
            document.getElementById('ret-sebep').innerText = data.aciklama;

            $('#ret-detay').removeClass('d-none');
        },
        error: function (xhr, status, error) {
            toastr.error("Beklenmeyen bir hata oluştu!");
        }
    });
}

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
                    var islemYapanCalisanUnvan = item.islemYapanCalisanUnvan;
                    var islemYapanCalisanAdSoyad = item.islemYapanCalisanAdSoyad;
                    var islemTarihi = item.islemTarihi;
                    var izinDurum = item.izinDurum;
                    var isleminIzinOnayTanimi = item.isleminIzinOnayTanimi == "" ? "-" : item.isleminIzinOnayTanimi;
                    var id = item.id;

                    var retGoruntuleBtn = "";

                    if (izinDurum == "Düzeltilmek Üzere Geri Gönderildi" || izinDurum == "Reddedildi") {
                        retGoruntuleBtn = `<button class="btn btn-sm btn-primary" type="button" onClick="getRetDetayByIzinHareketId(${id})">Detay Görüntüle</button>`;
                    }

                    tbody += `<tr>
                                    <th>${index + 1}</th >
                                    <td>${islemTarihi}</th >
                                    <td>${isleminIzinOnayTanimi}</td>
                                    <td class="text-center">${islemYapanCalisanAdSoyad}<br />${islemYapanCalisanUnvan}</th >
                                    <td>${getBadge(izinDurum)}</td>
                                    <td>${retGoruntuleBtn}</td>
                                 </tr>`
                });

                element.innerHTML = `<table class="table table-bordered border border-gray-300">
                                      <thead>
                                        <tr>
                                          <th>Sıralama</th>
                                          <th>İşlem Tarihi</th>
                                          <th>İşlem Yapanın Onay Tanım Grubu</th>
                                          <th class="text-center">İşlem Yapan Kişi</th>
                                          <th>Yapılan İşlem</th>
                                          <th></th>
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
            var aciklama = data.aciklama;
            var adSoyad = data.adSoyad;
            var adres = data.adres;
            var baslangicTarihi = data.baslangicTarihi;
            var birim = data.birim;
            var bitisTarihi = data.bitisTarihi;
            var formTip = data.formTip;
            var iseBaslamaTarihi = data.iseBaslamaTarihi;
            var istekTarihi = data.istekTarihi;
            var isyeri = data.isyeri;
            var izinDurum = data.izinDurum;
            var izinTur = data.izinTur;
            var sicilNo = data.sicilNo;
            var telefon = data.telefon;
            var unvan = data.unvan;
            var yerineBakacakKisi = data.yerineBakacakKisi;
            var toplamGunSayisi = data.toplamGunSayisi;
            var mahsubenGunSayisi = data.mahsubenGunSayisi;
            var yillikIzinUcretIstegi = data.yillikIzinUcretIstegi;
            var gorevGrubu = data.gorevGrubu;
            var dokumAlan = $('#personel-fullname').html();
            var birinciOnayVerenAdSoyad = data.birinciOnayVerenAdSoyad;
            var ikinciOnayVerenAdSoyad = data.ikinciOnayVerenAdSoyad;
            var merkezMuduruAdSoyad = data.merkezMuduruAdSoyad;

            var yillikIzinUcretIstegiText = "Yıllık İzin Dönemine İlişkin Ücretimi Peşin <strong>Almak İstiyorum<strong>";

            if (yillikIzinUcretIstegi !== "Evet") {
                yillikIzinUcretIstegiText = "Yıllık İzin Dönemine İlişkin Ücretimi Peşin <strong>Almak İstemiyorum<strong>";
            }


            var mahsubenBaslangicTarihi = data.mahsubenBaslangicTarihi;

            var content = '';

            if (formTip == 'T') {
                content = `<div class="p-15 m-auto">
                                        <p>İlgili makama,</p>
                                        <p>${aciklama}</p>
                                        <div>
                                            <p>
                                                <span class="fw-bolder">İzin Türü : </span>
                                                <span>${izinTur}</span>
                                            </p>

                                            <p>
                                                <span class="fw-bolder">İstek Tarihi : </span>
                                                <span>${istekTarihi}</span >
                                            </p>

                                            <p>
                                                <span class="fw-bolder">İzin Başlangıç Tarihi : </span>
                                                <span>${baslangicTarihi}</span>
                                            </p>

                                            <p>
                                                <span class="fw-bolder">İzin Bitiş Tarihi     : </span>
                                                <span>${bitisTarihi}</span>
                                            </p>

                                            <p>
                                                <span class="fw-bolder">İşe Başlama Tarihi     : </span>
                                                <span>${iseBaslamaTarihi}</span>
                                            </p>
                                        </div>

                                        <div class="d-flex justify-content-end">
                                            <div>
                                                <p>
                                                    <span class="fw-bolder">Personelin Adı Soyadı : </span>
                                                    <span>${adSoyad}</span>
                                                </p>

                                                <p>
                                                    <span class="fw-bolder">Sicil No : </span>
                                                    <span>${sicilNo}</span>
                                                </p>

                                                <p>
                                                    <span class="fw-bolder">İmza : </span>
                                                </p>
                                            </div>
                                        </div>

                                        <div class="fs-8">
                                            <span>${izinId}</span> - <span>${dokumAlan}</span>
                                        </div>
                                    </div>`
            }
            else {
                var yillik = `<table class="table table-bordered border border-gray-300" style="width: 100%;">
                                <tbody>
                                    <tr>
                                        <td style="width: 100%;" colspan="4">
                                            <div class="d-flex align-items-center row">
                                                <div class="col-2">
                                                    <img width="50px" src="/media/izin-form-logo.png">
                                                </div>
                                                <div class="col-8 text-center"><strong>BAŞKENT ÜNİVERSİTESİ İZİN FORMU</strong></div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 100%;" colspan="4"><strong>İzin İsteminde Bulunan Personelin</strong></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 25%;"><strong>Adı Soyadı</strong></td>
                                        <td style="width: 25%;">${adSoyad}</td>
                                        <td style="width: 25%;"><strong>Personel Sıra No.</strong></td>
                                        <td style="width: 25%;">${sicilNo}</td>
                                    </tr>
                                    <tr>
                                        <td style="width: 25%;"><strong>Birimi</strong></td>
                                        <td style="width: 25%;">${birim}</td>
                                        <td style="width: 25%;"><strong>Unvanı</strong></td>
                                        <td style="width: 25%;">${unvan}</td>
                                    </tr>
                                    <tr>
                                        <td style="width: 25%;"><strong>Görev Grubu</strong></td>
                                        <td style="width: 75%;" colspan="3">${gorevGrubu}</td>
                                    </tr>
                                </tbody>
                            </table>

                            <table class="table table-bordered border border-gray-300" style="width: 100%;">
                                <tbody>
                                    <tr>
                                        <td style="width: 100%;" colspan="3">
                                            <div style="text-align: center;"><strong>KULLANILACAK İZİN</strong></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 30%;"><strong>İzin Türü</strong></td>
                                        <td style="width: 35%;">${izinTur}</td>
                                        <td valign="middle" style="width: 35%;" rowspan="9">
                                            <div>
                                            <strong>Burada beyan ettiğim bilgilerin doğru olduğunu, herhangi bir değişiklikte yazıyla bildireceğimi ve aksi halde yasal işlem yapılmasını kabul ettiğimi taahh&uuml;t ederim.</strong>
                                            </div>
                                            <div style="text-align: center;"><strong>...../...../20..........</strong></div>
                                            <div style="text-align: center;"><strong>İMZA</strong></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 30;"><strong>İzin Süresi (İş günü* olarak)</strong></td>
                                        <td style="width: 35%;">${toplamGunSayisi}</td>
                                    </tr>
                                    <tr>
                                        <td style="width: 30%;"><strong>İzin Başlangıç Tarihi</strong></td>
                                        <td style="width: 35%;">${baslangicTarihi}</td>
                                    </tr>
                                    <tr>
                                        <td style="width: 30%;"><strong>İzin Bitiş Tarihi</strong></td>
                                        <td style="width: 35%;">${bitisTarihi}</td>
                                    </tr>
                                    <tr>
                                        <td style="width: 30%;"><strong>İzin Dönüşü Göreve Başlama Tarihi</strong></td>
                                        <td style="width: 35%;">${iseBaslamaTarihi}</td>
                                    </tr>
                                    <tr>
                                        <td style="width: 30%;"><strong>İzinde Bulunacağı Adres</strong></td>
                                        <td style="width: 35%;">${adres}</td>
                                    </tr>
                                    <tr>
                                        <td style="width: 30%;"><strong>Telefon</strong></td>
                                        <td style="width: 35%;">${telefon}</td>
                                    </tr>
                                    <tr>
                                        <td style="width: 30%;"><strong>İzinde Olduğu Sürede Görevini Yapacak Kişi</strong></td>
                                        <td style="width: 35%;">${yerineBakacakKisi}</td>
                                    </tr>
                                    <tr>
                                        <td style="width: 30%;"><strong>4857 Sayılı İş Kanununun 57. Maddesi gereğince</strong></td>
                                        <td style="width: 35%;">${yillikIzinUcretIstegiText}</td>
                                    </tr>
                                </tbody>
                            </table>
                            <p><em>* 4857 Sayılı iş Kanunu ve Çalışma Bakanlığı ilgili Yönetmelikleri gereğince, “haftada 1 gün hafta tatili verildiğinden” günlük çalışma saatlerinden bağımsız olarak yıllık izinler için her hafta 6 iş günü üzerinden hesaplanır. Diğer izinler gün sayısı üzerinden hesaplanır.</em></p>
                            <table class="table table-bordered border border-gray-300" style="width: 100%;">
                                <tbody>
                                    <tr>
                                        <td style="width: 100%;" colspan="6">
                                            <div style="text-align: center;"><strong>ONAYLAR</strong></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 33.3333%;" colspan="2">
                                            <div style="text-align: center;"><strong>I.AMİR</strong></div>
                                        </td>
                                        <td style="width: 33.3333%;" colspan="2">
                                            <div style="text-align: center;"><strong>II.AMİR</strong></div>
                                        </td>
                                        <td style="width: 33.3333%;" colspan="2">
                                            <div style="text-align: center;"><strong>SAĞLIK UYGULAMA ARAŞTIRMA MERKEZLERİ AMİRLERİ İÇİN</strong></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 10%;"><strong>Ad Soyad</strong></td>
                                        <td style="width: 23.3333%;">${birinciOnayVerenAdSoyad}</td>
                                        <td style="width: 10%;"><strong>Ad Soyad</strong><br></td>
                                        <td style="width: 23.3333%;">${ikinciOnayVerenAdSoyad}</td>
                                        <td style="width: 10%;"><strong>Ad Soyad</strong><br></td>
                                        <td style="width: 23.3333%;">${merkezMuduruAdSoyad}</td>
                                    </tr>
                                    <tr>
                                        <td style="width: 10%;"><strong>Tarih</strong></td>
                                        <td style="width: 23.3333%;"><br></td>
                                        <td style="width: 10%;"><strong>Tarih</strong><br></td>
                                        <td style="width: 23.3333%;"><br></td>
                                        <td style="width: 10%;"><strong>Tarih</strong></td>
                                        <td style="width: 23.3333%;"><br></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 10%;"><strong>İmza</strong></td>
                                        <td style="width: 23.3333%;"><br></td>
                                        <td style="width: 10%;"><strong>İmza</strong><br></td>
                                        <td style="width: 23.3333%;"><br></td>
                                        <td style="width: 10%;"><strong>İmza</strong></td>
                                        <td style="width: 23.3333%;"><br></td>
                                    </tr>
                                </tbody>
                            </table>
                            <ol>
                                <li><em>Yıllık izinler, iş günü üzerinden, diğer izinler takvim günü üzerinden hesaplanır. Dini ve Milli Bayramlar yılık ücretli izin süresinden sayılmaz.</em></li>
                                <li><em>Onaylanarak Personel Dairesine gönderilmiş yılık izin formu, çalışanın belirttiği tarihler arasında izin kullanmış olduğu anlamı taşır. Belirtilen günlerden önce ve sonra izinden dönen çalışanların Personel Dairesine bildirim yükümlülüğü imza atan II. Amire aittir.</em></li>
                                <li><em>Onaylar bölümündeki imzalar birebir sorumluluğu olan I.ve II. amirlere ait olmalıdır.</em></li>
                                <li><em>Yıllık İzin Ücretiniz, İdari Ve Mali işler Daire başkanlığı tarafından daha sonra maaşınızdan mahsup edilmek üzere talebiniz halinde onaylı izin formunuz ile birlikte peşin olarak ödenir.</em></li>
                                <li><em>Yıllık izin formuna <a href="http://www.baskent.edu.tr/belgeler/akademik_formlar/Izin_Formu_2022.doc" target="_blank">http://www.baskent.edu.tr/belgeler/akademik_formlar/Izin_Formu_2022.doc</a> adresinden ulaşılabilir.</em></li>
                            </ol>

                            <table style="width: 100%;">
                                <tbody>
                                    <tr>
                                        <td style="width: 100.0000%;">
                                            <div style="text-align: center;">Yukarıdaki bilgiler izin yönetmeliğine uygun olarak doldurulmuştur</div>
                                            <div class="my-2" style="text-align: center;">...../...../....................</div>
                                            <div style="text-align: center;">İmza</div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>`;



                content = `${yillik}`;

                var renderMahsuben = mahsubenBaslangicTarihi != null;
                if (renderMahsuben) {
                    var mahsuben = `<table class="table table-bordered border border-gray-300" style="width: 100%;">
                                    <tbody>
                                        <tr>
                                            <td style="width: 100%;" colspan="4">
                                                <div class="d-flex align-items-center row">
                                                    <div class="col-2">
                                                        <img width="50px" src="/media/izin-form-logo.png">
                                                    </div>
                                                    <div class="col-8" style="text-align: center;"><strong>BAŞKENT ÜNİVERSİTESİ MAHSUBEN İZİN TAAHHÜTNAMESİ</strong></div>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100%;" colspan="4"><strong>İzin İsteminde Bulunan Personelin</strong></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 25%;"><strong>Adı Soyadı</strong></td>
                                            <td style="width: 75%;" colspan="3">${adSoyad}</td>
                                        </tr>
                                        <tr>
                                            <td style="width: 25%;"><strong>Personel Sıra No.</strong></td>
                                            <td style="width: 25%;">${sicilNo}</td>
                                            <td style="width: 25%;"><strong>Unvan</strong></td>
                                            <td style="width: 25%;">${unvan}</td>
                                        </tr>
                                        <tr>
                                            <td style="width: 25%;"><strong>Görev Grubu</strong></td>
                                            <td style="width: 75%;" colspan="3">${gorevGrubu}</td>
                                        </tr>
                                    </tbody>
                                </table>

                                <table class="table table-bordered border border-gray-300" style="width: 100%;">
                                    <tbody>
                                        <tr>
                                            <td style="width: 50%;"><strong>İzin Süresi (İş günü* Olarak)</strong></td>
                                            <td style="width: 50%;">${mahsubenGunSayisi}</td>
                                        </tr>
                                        <tr>
                                            <td style="width: 50%;"><strong>İzin Başlangıç Tarihi</strong></td>
                                            <td style="width: 50%;">${mahsubenBaslangicTarihi}</td>
                                        </tr>
                                        <tr>
                                            <td style="width: 50%;"><strong>İzin Bitiş Tarihi</strong></td>
                                            <td style="width: 50%;">${bitisTarihi}</td>
                                        </tr>
                                        <tr>
                                            <td style="width: 50%;"><strong>İzin Dönüşü; Göreve Başlama Tarihi</strong></td>
                                            <td style="width: 50%;">${iseBaslamaTarihi}</td>
                                        </tr>
                                    </tbody>
                                </table>

                                <p>
                                    Yıllık ücretli iznimi, <span>……../……../…….</span>.  Tarihinde hak edeceğimi biliyorum. Buna karşılık, ekli form ile, izin hak edeceğim tarihten önce <span>${mahsubenGunSayisi}</span> gün yıllık ücretli izin kullanmak istiyorum.
                                    Kullanacağım izinin hak edeceğim iznimden mahsup edilmesini, mahsuben kullandığım izinlerin, izin hak ediş tarihinden önce görevimden ayrılmam durumunda, son alacağım maaşımdan kesileceğini kabul ve taahhüt ederim.
                                </p>

                                <div class="d-flex justify-content-between w-50">
                                    <div>
                                        <p><br /></p>
                                        <p><strong>AD SOYAD : </strong>${adSoyad}<span></span></p>
                                        <p><strong>TARİH : </strong><span></span></p>
                                        <p><strong>İMZA : </strong><span></span></p>
                                    </div>

                                    <div>
                                        <p><strong>BİRİM SORUMLUSU</strong></p>
                                        <p><strong>AD SOYAD : </strong><span>${birinciOnayVerenAdSoyad}</span></p>
                                        <p><strong>TARİH : </strong><span></span></p>
                                        <p><strong>İMZA : </strong><span></span></p>
                                    </div>
                                </div>`;
                    content += mahsuben;
                }
            }


            $('#print-izin-form').html(content);
        },
        error: function (xhr, status, error) {
            toastr.error("Beklenmeyen bir hata oluştu!");
        }
    });
}

function izinIsteginiIptalEt(izinId) {
    Swal.fire({
        html: "İzin talebinizi iptal etmek üzeresiniz. Devam etmek istiyor musunuz?",
        icon: "question",
        showCancelButton: true,
        buttonsStyling: false,
        confirmButtonText: "Evet",
        cancelButtonText: "Hayır, vazgeç",
        customClass: {
            confirmButton: "btn fw-bold btn-primary",
            cancelButton: "btn fw-bold btn-light"
        }
    }).then(function (result) {
        if (result.value) {
            $.ajax({
                url: `/islemler/iptal/izin/${izinId}`,
                type: 'POST',
                data: { __RequestVerificationToken: $('input[name=__RequestVerificationToken]').val() },
                success: function (result) {
                    if (result) {
                        Swal.fire({
                            text: "İzin iptal işlemi başarıyla gerçekleşti!",
                            icon: "success",
                            buttonsStyling: false,
                            confirmButtonText: "Tamam",
                            customClass: {
                                confirmButton: "btn btn-success"
                            }
                        }).then(function () {
                            window.location.href = "/islemler/izin";
                        });
                    } else {
                        Swal.fire({
                            text: "İşlem Başarısız.",
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Tamam",
                            customClass: {
                                confirmButton: "btn btn-danger"
                            }
                        }).then(function () {
                            console.log(result);
                        });
                    }

                },
                error: function (xhr, status, error) {
                    Swal.fire({
                        text: "İşlem Başarısız.",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Tamam",
                        customClass: {
                            confirmButton: "btn btn-danger"
                        }
                    }).then(function () {
                        console.log(error);
                    });
                }
            });
        }
    });
}

function ClearTextTipIzinForm() {
    $('#TextTipIzinCreateVM_BaslangicTarihi').val('');
    $('#TextTipIzinCreateVM_BitisTarihi').val('');
    $('#TextTipIzinCreateVM_GunSayisi').val('');
    $('#TextTipIzinCreateVM_IseBaslamaTarihi').val('');
    $('#TextTipIzinCreateVM_Aciklama').val('');
}

function ClearFormTipIzinForm() {
    $("#FormTipIzinCreateVM_BaslangicTarihi").val('');
    $("#FormTipIzinCreateVM_BitisTarihi").val('');
    $("#FormTipIzinCreateVM_GunSayisi").val('');
    $("#FormTipIzinCreateVM_MahsubenGunSayisi").val('');
    $("#FormTipIzinCreateVM_Adres").val('');
    $("#FormTipIzinCreateVM_Telefon").val('');
    $("#FormTipIzinCreateVM_YerineBakacakKisi").val('');
    $("#FormTipIzinCreateVM_YillikIzinUcretiIstegi").val('');
}

function setInputDisabledAttr(sabitGunSayisi, baslangicTarihiInputId, bitisTarihiInputId, izinTurId) {
    const bitisTarihiInput = document.getElementById(bitisTarihiInputId);
    const baslangicTarihiInput = document.getElementById(baslangicTarihiInputId);

    const existingHandler = eventHandlers[baslangicTarihiInputId];

    if (existingHandler) {
        baslangicTarihiInput.removeEventListener('change', existingHandler);
    }

    if (sabitGunSayisi != null) {
        console.log("sabit gün sayisi : " + sabitGunSayisi);
        bitisTarihiInput.disabled = true;

        const newHandler = handleChange(baslangicTarihiInputId, bitisTarihiInputId, izinTurId).bind(this);

        baslangicTarihiInput.addEventListener('change', newHandler);

        eventHandlers[baslangicTarihiInputId] = newHandler;
    } else {
        console.log("sabit gün sayisi null: " + sabitGunSayisi);
        bitisTarihiInput.disabled = false;
    }
}

const handleChange = function (baslangicTarihiInputId, bitisTarihiInputId, izinTurId) {
    return function () {
        console.log("çalıştı")
        const bitisTarihiInput = document.getElementById(bitisTarihiInputId);
        var baslangicTarihi = document.getElementById(baslangicTarihiInputId).value;

        $.ajax({
            url: `/izin-bitis-tarihi?baslangicTarihi=${baslangicTarihi}&izinTurId=${izinTurId}`,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                bitisTarihiInput.disabled = false;

                flatpickr(`#${bitisTarihiInputId}`, flatpickrConfig).setDate(data.bitisTarihi);

                bitisTarihiInput.disabled = true;
            },
            error: function (xhr, status, error) {
                toastr.error("Beklenmeyen bir hata oluştu!");
            }
        });
    }
}

function setIzinIdForDogrulama(izinId) {
    document.getElementById('CreateDogrulamaVM_IzinId').value = izinId;
}

function setTelefonValue() {
    var telefon = document.getElementById('telefonJsConverter').value;
    var telefonInput = document.getElementById('FormTipIzinCreateVM_Telefon');

    telefonInput.value = telefon.substring(1);
}

var intervalId;

function startCountdown(dateId, countdowndId, alertId) {
    if (intervalId) {
        clearInterval(intervalId);
    }

    document.getElementById(countdowndId).classList.remove('d-none');
    document.getElementById(alertId).classList.add('d-none');

    var date = document.getElementById(dateId).innerHTML;
    var countDownDate = new Date(date).getTime();

    intervalId = setInterval(function () {
        var now = new Date().getTime();
        var distance = countDownDate - now;

        var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        var seconds = Math.floor((distance % (1000 * 60)) / 1000);

        document.getElementById(countdowndId).innerHTML = minutes + "dk " + seconds + "s";

        if (distance < 0) {
            clearInterval(intervalId);

            document.getElementById(countdowndId).classList.add('d-none');
            document.getElementById(alertId).classList.remove('d-none');
        }
    }, 1000);
}

var TextTipTwoFactor = function () {
    var submitBtn;

    return {
        init: function () {
            submitBtn = document.querySelector("#text-tip-btn");

            submitBtn.addEventListener("click", function (event) {
                event.preventDefault();

                var isValid = true;
                var inputs = [].slice.call(document.querySelectorAll('.text-tip-auth-code'));

                inputs.map(function (input) {
                    if (input.value === "" || input.value.length === 0) {
                        isValid = false;
                    }
                });

                if (isValid) {
                    var kod1 = document.getElementById('text-tip-code-1').value;
                    var kod2 = document.getElementById('text-tip-code-2').value;
                    var kod3 = document.getElementById('text-tip-code-3').value;
                    var kod4 = document.getElementById('text-tip-code-4').value;
                    var kod5 = document.getElementById('text-tip-code-5').value;
                    var kod6 = document.getElementById('text-tip-code-6').value;

                    var kod = kod1 + kod2 + kod3 + kod4 + kod5 + kod6;
                    $('#TextTipIzinCreateVM_Kod').val(kod);

                    $('#TextTipIzinCreateVM_BitisTarihi').prop('disabled', false);

                    var form = $("#text-tip-izin-form");
                    var url = form.attr('action');

                    $.ajax({
                        url: url,
                        type: 'POST',
                        data: form.serialize(),
                        success: function (result) {
                            if (result.id) {
                                Swal.fire({
                                    text: "İzin başarıyla oluşturuldu!",
                                    icon: "success",
                                    buttonsStyling: false,
                                    confirmButtonText: "Tamam",
                                    customClass: {
                                        confirmButton: "btn btn-primary"
                                    }
                                }).then(function (result) {
                                    if (result.isConfirmed) {
                                        inputs.map(function (input) {
                                            input.value = "";
                                        });

                                        var redirectUrl = "/islemler/izin";
                                        if (redirectUrl) {
                                            location.href = redirectUrl;
                                        }
                                    }
                                });
                            } else {
                                swal.fire({
                                    text: "Lütfen geçerli güvenlik kodunu girdikten sonra tekrar deneyin",
                                    icon: "error",
                                    buttonsStyling: false,
                                    confirmButtonText: "Tamam",
                                    customClass: {
                                        confirmButton: "btn fw-bold btn-light-primary"
                                    }
                                }).then(function () {
                                    KTUtil.scrollTop();
                                });
                            }

                        },
                        error: function (xhr, status, error) {
                            swal.fire({
                                text: "Lütfen geçerli güvenlik kodunu girdikten sonra tekrar deneyin",
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Tamam",
                                customClass: {
                                    confirmButton: "btn fw-bold btn-light-primary"
                                }
                            }).then(function () {
                                KTUtil.scrollTop();
                            });
                        }
                    });

                } else {
                    swal.fire({
                        text: "Lütfen geçerli güvenlik kodunu girdikten sonra tekrar deneyin",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Tamam",
                        customClass: {
                            confirmButton: "btn fw-bold btn-light-primary"
                        }
                    }).then(function () {
                        KTUtil.scrollTop();
                    });
                }
            });
        }
    };
}();

var TextTipUpdateTwoFactor = function () {
    var submitBtn;

    return {
        init: function () {
            submitBtn = document.querySelector("#text-tip-update-btn");

            submitBtn.addEventListener("click", function (event) {
                event.preventDefault();

                var isValid = true;
                var inputs = [].slice.call(document.querySelectorAll('.text-tip-update-auth-code'));

                inputs.map(function (input) {
                    if (input.value === "" || input.value.length === 0) {
                        isValid = false;
                    }
                });

                if (isValid) {
                    var kod1 = document.getElementById('text-tip-update-code-1').value;
                    var kod2 = document.getElementById('text-tip-update-code-2').value;
                    var kod3 = document.getElementById('text-tip-update-code-3').value;
                    var kod4 = document.getElementById('text-tip-update-code-4').value;
                    var kod5 = document.getElementById('text-tip-update-code-5').value;
                    var kod6 = document.getElementById('text-tip-update-code-6').value;

                    var kod = kod1 + kod2 + kod3 + kod4 + kod5 + kod6;
                    $('#TextTipIzinUpdateVM_Kod').val(kod);

                    $('#TextTipIzinUpdateVM_BitisTarihi').prop('disabled', false);

                    var form = $("#text-tip-izin-update-form");
                    var url = form.attr('action');

                    $.ajax({
                        url: url,
                        type: 'POST',
                        data: form.serialize(),
                        success: function (result) {
                            if (result) {
                                Swal.fire({
                                    text: "İzin başarıyla güncellendi!",
                                    icon: "success",
                                    buttonsStyling: false,
                                    confirmButtonText: "Tamam",
                                    customClass: {
                                        confirmButton: "btn btn-primary"
                                    }
                                }).then(function (result) {
                                    if (result.isConfirmed) {
                                        inputs.map(function (input) {
                                            input.value = "";
                                        });

                                        var redirectUrl = "/islemler/izin";
                                        if (redirectUrl) {
                                            location.href = redirectUrl;
                                        }
                                    }
                                });
                            } else {
                                swal.fire({
                                    text: "Lütfen geçerli güvenlik kodunu girdikten sonra tekrar deneyin",
                                    icon: "error",
                                    buttonsStyling: false,
                                    confirmButtonText: "Tamam",
                                    customClass: {
                                        confirmButton: "btn fw-bold btn-light-primary"
                                    }
                                }).then(function () {
                                    KTUtil.scrollTop();
                                });
                            }

                        },
                        error: function (xhr, status, error) {
                            swal.fire({
                                text: "Lütfen geçerli güvenlik kodunu girdikten sonra tekrar deneyin",
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Tamam",
                                customClass: {
                                    confirmButton: "btn fw-bold btn-light-primary"
                                }
                            }).then(function () {
                                KTUtil.scrollTop();
                            });
                        }
                    });

                } else {
                    swal.fire({
                        text: "Lütfen geçerli güvenlik kodunu girdikten sonra tekrar deneyin",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Tamam",
                        customClass: {
                            confirmButton: "btn fw-bold btn-light-primary"
                        }
                    }).then(function () {
                        KTUtil.scrollTop();
                    });
                }
            });
        }
    };
}();

function getSelectedRadioValueByName(name) {
    var radios = document.getElementsByName(name);

    for (var i = 0, length = radios.length; i < length; i++) {
        if (radios[i].checked) {
            return radios[i].value;
        }
    }
}

var FormTipTwoFactor = function () {
    var submitBtn;

    return {
        init: function () {
            submitBtn = document.querySelector("#form-tip-btn");

            submitBtn.addEventListener("click", function (event) {
                event.preventDefault();

                var isValid = true;
                var inputs = [].slice.call(document.querySelectorAll('.form-tip-auth-code'));

                inputs.map(function (input) {
                    if (input.value === "" || input.value.length === 0) {
                        isValid = false;
                    }
                });

                if (isValid) {
                    var kod1 = document.getElementById('form-tip-code-1').value;
                    var kod2 = document.getElementById('form-tip-code-2').value;
                    var kod3 = document.getElementById('form-tip-code-3').value;
                    var kod4 = document.getElementById('form-tip-code-4').value;
                    var kod5 = document.getElementById('form-tip-code-5').value;
                    var kod6 = document.getElementById('form-tip-code-6').value;

                    var kod = kod1 + kod2 + kod3 + kod4 + kod5 + kod6;
                    $('#FormTipIzinCreateVM_Kod').val(kod);

                    var form = $("#form-tip-izin-form");
                    var url = form.attr('action');

                    $.ajax({
                        url: url,
                        type: 'POST',
                        data: form.serialize(),
                        success: function (result) {
                            if (result.id) {
                                Swal.fire({
                                    text: "İzin başarıyla oluşturuldu!",
                                    icon: "success",
                                    buttonsStyling: false,
                                    confirmButtonText: "Tamam",
                                    customClass: {
                                        confirmButton: "btn btn-primary"
                                    }
                                }).then(function (result) {
                                    if (result.isConfirmed) {
                                        inputs.map(function (input) {
                                            input.value = "";
                                        });

                                        var redirectUrl = "/islemler/izin";
                                        if (redirectUrl) {
                                            location.href = redirectUrl;
                                        }
                                    }
                                });
                            } else {
                                swal.fire({
                                    text: "Lütfen geçerli güvenlik kodunu girdikten sonra tekrar deneyin",
                                    icon: "error",
                                    buttonsStyling: false,
                                    confirmButtonText: "Tamam",
                                    customClass: {
                                        confirmButton: "btn fw-bold btn-light-primary"
                                    }
                                }).then(function () {
                                    KTUtil.scrollTop();
                                });
                            }

                        },
                        error: function (xhr, status, error) {
                            swal.fire({
                                text: "Lütfen geçerli güvenlik kodunu girdikten sonra tekrar deneyin",
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Tamam",
                                customClass: {
                                    confirmButton: "btn fw-bold btn-light-primary"
                                }
                            }).then(function () {
                                KTUtil.scrollTop();
                            });
                        }
                    });

                } else {
                    swal.fire({
                        text: "Lütfen geçerli güvenlik kodunu girdikten sonra tekrar deneyin",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Tamam",
                        customClass: {
                            confirmButton: "btn fw-bold btn-light-primary"
                        }
                    }).then(function () {
                        KTUtil.scrollTop();
                    });
                }
            });
        }
    };
}();

var FormTipUpdateTwoFactor = function () {
    var submitBtn;

    return {
        init: function () {
            submitBtn = document.querySelector("#form-tip-update-btn");

            submitBtn.addEventListener("click", function (event) {
                event.preventDefault();

                var isValid = true;
                var inputs = [].slice.call(document.querySelectorAll('.form-tip-update-auth-code'));

                inputs.map(function (input) {
                    if (input.value === "" || input.value.length === 0) {
                        isValid = false;
                    }
                });

                if (isValid) {
                    var kod1 = document.getElementById('form-tip-update-code-1').value;
                    var kod2 = document.getElementById('form-tip-update-code-2').value;
                    var kod3 = document.getElementById('form-tip-update-code-3').value;
                    var kod4 = document.getElementById('form-tip-update-code-4').value;
                    var kod5 = document.getElementById('form-tip-update-code-5').value;
                    var kod6 = document.getElementById('form-tip-update-code-6').value;

                    var kod = kod1 + kod2 + kod3 + kod4 + kod5 + kod6;
                    $('#FormTipIzinUpdateVM_Kod').val(kod);

                    var form = $("#form-tip-izin-update-form");
                    var url = form.attr('action');

                    $.ajax({
                        url: url,
                        type: 'POST',
                        data: form.serialize(),
                        success: function (result) {
                            if (result) {
                                Swal.fire({
                                    text: "İzin başarıyla güncellendi!",
                                    icon: "success",
                                    buttonsStyling: false,
                                    confirmButtonText: "Tamam",
                                    customClass: {
                                        confirmButton: "btn btn-primary"
                                    }
                                }).then(function (result) {
                                    if (result.isConfirmed) {
                                        inputs.map(function (input) {
                                            input.value = "";
                                        });

                                        var redirectUrl = "/islemler/izin";
                                        if (redirectUrl) {
                                            location.href = redirectUrl;
                                        }
                                    }
                                });
                            } else {
                                swal.fire({
                                    text: "Lütfen geçerli güvenlik kodunu girdikten sonra tekrar deneyin",
                                    icon: "error",
                                    buttonsStyling: false,
                                    confirmButtonText: "Tamam",
                                    customClass: {
                                        confirmButton: "btn fw-bold btn-light-primary"
                                    }
                                }).then(function () {
                                    KTUtil.scrollTop();
                                });
                            }

                        },
                        error: function (xhr, status, error) {
                            swal.fire({
                                text: "Lütfen geçerli güvenlik kodunu girdikten sonra tekrar deneyin",
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Tamam",
                                customClass: {
                                    confirmButton: "btn fw-bold btn-light-primary"
                                }
                            }).then(function () {
                                KTUtil.scrollTop();
                            });
                        }
                    });

                } else {
                    swal.fire({
                        text: "Lütfen geçerli güvenlik kodunu girdikten sonra tekrar deneyin",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Tamam",
                        customClass: {
                            confirmButton: "btn fw-bold btn-light-primary"
                        }
                    }).then(function () {
                        KTUtil.scrollTop();
                    });
                }
            });
        }
    };
}();

async function dogrulamaKoduGonder(name) {
    var yontem = getSelectedRadioValueByName(name);

    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: `/dogrulama/izin?yontem=${yontem}`,
            data: {
                __RequestVerificationToken: $('input[name=__RequestVerificationToken]').val(),
            },
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

async function tekrarGonderTextTipCreate() {
    await dogrulamaTextTipCreate();

    var yontem = getSelectedRadioValueByName('TextTipIzinCreateVM.DogrulamaYontemi');
    var mesaj = "";
    if (yontem == "SMS") {
        mesaj = "Doğrulama kodu telefon numaranıza SMS olarak gönderildi!";
    } else if (yontem == "E-POSTA") {
        mesaj = "Doğrulama kodu e-posta adresinize gönderildi!";
    }

    Swal.fire({
        html: mesaj,
        icon: "success",
        showCancelButton: false,
        buttonsStyling: false,
        confirmButtonText: "Tamam",
        customClass: {
            confirmButton: "btn fw-bold btn-primary",
            cancelButton: "btn fw-bold btn-light"
        }
    })
}

async function tekrarGonderTextTipUpdate() {
    await dogrulamaTextTipUpdate();

    var yontem = getSelectedRadioValueByName('TextTipIzinUpdateVM.DogrulamaYontemi');
    var mesaj = "";
    if (yontem == "SMS") {
        mesaj = "Doğrulama kodu telefon numaranıza SMS olarak gönderildi!";
    } else if (yontem == "E-POSTA") {
        mesaj = "Doğrulama kodu e-posta adresinize gönderildi!";
    }

    Swal.fire({
        html: mesaj,
        icon: "success",
        showCancelButton: false,
        buttonsStyling: false,
        confirmButtonText: "Tamam",
        customClass: {
            confirmButton: "btn fw-bold btn-primary",
            cancelButton: "btn fw-bold btn-light"
        }
    })
}

async function tekrarGonderFormTipCreate() {
    await dogrulamaFormTipCreate();

    var yontem = getSelectedRadioValueByName('FormTipIzinCreateVM.DogrulamaYontemi');
    var mesaj = "";
    if (yontem == "SMS") {
        mesaj = "Doğrulama kodu telefon numaranıza SMS olarak gönderildi!";
    } else if (yontem == "E-POSTA") {
        mesaj = "Doğrulama kodu e-posta adresinize gönderildi!";
    }

    Swal.fire({
        html: mesaj,
        icon: "success",
        showCancelButton: false,
        buttonsStyling: false,
        confirmButtonText: "Tamam",
        customClass: {
            confirmButton: "btn fw-bold btn-primary",
            cancelButton: "btn fw-bold btn-light"
        }
    })
}

async function tekrarGonderFormTipUpdate() {
    await dogrulamaFormTipUpdate();

    var yontem = getSelectedRadioValueByName('FormTipIzinUpdateVM.DogrulamaYontemi');
    var mesaj = "";
    if (yontem == "SMS") {
        mesaj = "Doğrulama kodu telefon numaranıza SMS olarak gönderildi!";
    } else if (yontem == "E-POSTA") {
        mesaj = "Doğrulama kodu e-posta adresinize gönderildi!";
    }

    Swal.fire({
        html: mesaj,
        icon: "success",
        showCancelButton: false,
        buttonsStyling: false,
        confirmButtonText: "Tamam",
        customClass: {
            confirmButton: "btn fw-bold btn-primary",
            cancelButton: "btn fw-bold btn-light"
        }
    })
}



$(function () {
    setCheckboxValuesOnChange();

    initDatatable();

    $("#mahsubenBitisUpdate, #mahsubenBitisCreate, #mahsubenBitis, #FormTipIzinCreateVM_BaslangicTarihi, #FormTipIzinCreateVM_BitisTarihi, #FormTipIzinCreateVM_IseBaslamaTarihi, #FormTipIzinCreateVM_MahsubenBaslangicTarihi, #TextTipIzinCreateVM_BaslangicTarihi, #TextTipIzinCreateVM_BitisTarihi, #TextTipIzinCreateVM_IseBaslamaTarihi, #FormTipIzinUpdateVM_BaslangicTarihi, #FormTipIzinUpdateVM_BitisTarihi, #FormTipIzinUpdateVM_MahsubenBaslangicTarihi, #FormTipIzinUpdateVM_IseBaslamaTarihi, #TextTipIzinUpdateVM_BaslangicTarihi, #TextTipIzinUpdateVM_BitisTarihi, #TextTipIzinUpdateVM_IseBaslamaTarihi")
        .flatpickr(flatpickrConfig);

    $("#izinTurleri").on("select2:select", function (e) {
        var selectedIzinTurId = e.params.data.id;

        if (selectedIzinTurId != 0) {
            modalBtn.removeAttribute('disabled');

            // İlk adıma git
            formTipIzinStepper.goFirst();
            textTipIzinStepper.goFirst();

            $.ajax({
                url: `/islemler/izinturleri/id=${selectedIzinTurId}`,
                type: 'GET',
                dataType: 'json',
                success: function (data) {
                    var formTipi = data.izinFormTipi;
                    var sabitGunSayisi = data.sabitGunSayisi;
                    var izinTurId = $('#izinTurleri').val();

                    if (formTipi == "F") {
                        modalBtn.setAttribute('data-bs-target', '#izin-modal-1');
                        setInputDisabledAttr(sabitGunSayisi, 'FormTipIzinCreateVM_BaslangicTarihi', 'FormTipIzinCreateVM_BitisTarihi', izinTurId);
                    } else if (formTipi == "T") {
                        modalBtn.setAttribute('data-bs-target', '#izin-modal-2');
                        setInputDisabledAttr(sabitGunSayisi, 'TextTipIzinCreateVM_BaslangicTarihi', 'TextTipIzinCreateVM_BitisTarihi', izinTurId);
                    }
                },
                error: function (xhr, status, error) {
                    toastr.error("Beklenmeyen bir hata oluştu!");
                }
            });
        }
    });

    modalBtn.addEventListener('click', function (e) {
        var selectedIzinTur = $('#izinTurleri').val();
        if (selectedIzinTur === null) {
            toastr.error("İzin talebi oluşturabilmek için bir izin türü seçiniz!");
        }
    })

    Inputmask(maskOptions).mask("#FormTipIzinCreateVM_Telefon");
    Inputmask(maskOptions).mask("#FormTipIzinUpdateVM_Telefon");

    // #izin-modal-1
    $("#izin-modal-1").on('shown.bs.modal', function () {
        var currentStepIndex = formTipIzinStepper.getCurrentStepIndex();

        if (currentStepIndex == 1) {
            ClearFormTipIzinForm();
        }

        var izinTurId = document.querySelector('#izinTurleri').value;
        document.querySelector('#FormTipIzinCreateVM_IzinTurId').value = izinTurId;
    });


    // #izin-modal-2
    $("#izin-modal-2").on('shown.bs.modal', function () {
        var currentStepIndex = textTipIzinStepper.getCurrentStepIndex();

        if (currentStepIndex == 1) {
            ClearTextTipIzinForm();
        }

        var izinTurId = document.querySelector('#izinTurleri').value;
        document.querySelector('#TextTipIzinCreateVM_IzinTurId').value = izinTurId;
    });

    initStepperForFormTipIzin();

    initStepperForTextTipIzin();

    initStepperForFormTipIzinUpdate();

    initStepperForTextTipIzinUpdate();

    formTipIzinForm.addEventListener('submit', function (event) {
        event.preventDefault();

        startPageLoadingOverlay();

        document.getElementById('FormTipIzinCreateVM_BitisTarihi').disabled = false;

        formTipIzinForm.submit();
    })

    formTipIzinUpdateForm.addEventListener('submit', function (event) {
        event.preventDefault();

        startPageLoadingOverlay();

        document.getElementById('FormTipIzinUpdateVM_BitisTarihi').disabled = false;

        formTipIzinUpdateForm.submit();
    })

    textTipIzinUpdateForm.addEventListener('submit', function (event) {
        event.preventDefault();

        startPageLoadingOverlay();

        document.getElementById('TextTipIzinUpdateVM_BitisTarihi').disabled = false;

        textTipIzinUpdateForm.submit();
    })

    const formTipBitisTarihiInput = document.getElementById('FormTipIzinCreateVM_BitisTarihi');
    formTipBitisTarihiInput.addEventListener('change', function () {
        flatpickr("#mahsubenBitis", flatpickrConfig).setDate(formTipBitisTarihiInput.value);
    });

    const mahsubenCreateInput = document.getElementById('mahsubenCreate');
    mahsubenCreateInput.addEventListener('change', function () {
        var mahsubenBaslangic = document.getElementById('FormTipIzinCreateVM_MahsubenBaslangicTarihi');

        console.log(mahsubenCreateInput.value);

        if (mahsubenCreateInput.value == 'true') {
            mahsubenBaslangic.disabled = false;
        } else {
            mahsubenBaslangic.value = '';
            mahsubenBaslangic.disabled = true;
        }

    });

    const formTipBitisTarihiUpdateInput = document.getElementById('FormTipIzinUpdateVM_BitisTarihi');
    formTipBitisTarihiUpdateInput.addEventListener('change', function () {
        flatpickr("#mahsubenBitisUpdate", flatpickrConfig).setDate(formTipBitisTarihiUpdateInput.value);
    });

    const mahsubenUpdateInput = document.getElementById('mahsubenUpdate');
    mahsubenUpdateInput.addEventListener('change', function () {
        var mahsubenBaslangic = document.getElementById('FormTipIzinUpdateVM_MahsubenBaslangicTarihi');

        if (mahsubenUpdateInput.value == 'true') {
            mahsubenBaslangic.disabled = false;
        } else {
            mahsubenBaslangic.value = '';
            mahsubenBaslangic.disabled = true;
        }

    });

    TextTipTwoFactor.init();

    TextTipUpdateTwoFactor.init();

    FormTipTwoFactor.init();

    FormTipUpdateTwoFactor.init();
});