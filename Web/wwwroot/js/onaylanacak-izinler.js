"use strict";

var initDatatable = function () {
    var datatable = $('#onaylanacak_izinler_table').DataTable({
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
            url: '/admin/islemler/izinler/islem-bekleyen',
            type: 'GET',
            dataType: 'json',
            error: function (xhr, status, error) {
                console.log(error);
            }
        },
        'columns': [
            { data: 'id' },
            { data: 'istekTarihi' },
            { data: 'adSoyad' },
            { data: 'unvan' },
            { data: 'birim' },
            { data: 'dogrulamaYontemi' },
            { data: 'izinDurumu' },
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

                    return `<a href="/admin/islemler/izin-talepleri/${row['id']}" class="btn btn-sm btn-light-primary">
                                    <i class="fa-sharp fa-solid fa-eye fs-5"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>
                                    <span>Görüntüle</span>
                                </a>`;
                },
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
                title: 'Onaylanacak İzinler'
            },
            {
                extend: 'excelHtml5',
                title: 'Onaylanacak İzinler'
            },
            {
                extend: 'csvHtml5',
                title: 'Onaylanacak İzinler'
            },
            {
                extend: 'pdfHtml5',
                title: 'Onaylanacak İzinler'
            }
        ]
    }).container().appendTo($('#onaylanacak_izinler_table_buttons'));

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
}

function parseDate(dateString) {
    var dateComponents = dateString.split(".");

    var day = parseInt(dateComponents[0], 10);
    var month = parseInt(dateComponents[1], 10) - 1;
    var year = parseInt(dateComponents[2], 10);

    var jsDate = new Date(year, month, day);

    return jsDate;
}

// On document ready
$(function () {
    initDatatable();
});