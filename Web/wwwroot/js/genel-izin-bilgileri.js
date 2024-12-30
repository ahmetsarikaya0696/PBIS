var initDatatable = function (izinGrupId) {
    var url;
    var url = izinGrupId > 0 ? `/admin/izin-bilgileri?izinGrupId=${izinGrupId}` : `/admin/izin-bilgileri`;

    var datatable = $('#izin_bilgileri_table').DataTable({
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
            url: url,
            type: 'GET',
            dataType: 'json',
        },
        'columns': [
            { data: 'ad' },
            { data: 'soyad' },
            { data: 'kalanSenelikIzinGunSayisi' },
        ],
        columnDefs: [
            {
                targets: 0,
                visible: true,
                render: function (data, type, row) {
                    return data;
                }
            }
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
    }).container().appendTo($('#izin_bilgileri_table_buttons'));

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

$(function () {
    const urlParams = new URLSearchParams(window.location.search);
    var izinGrupId = urlParams.get('izingrupid');

    if (izinGrupId) {
        document.getElementById('table-card').classList.remove('d-none');
        document.getElementById('filter-card').classList.add('d-none');

        initDatatable(izinGrupId);
    }

    // filter-button id ' li butona tıklanıldığında izinGruplari select2' deki seçili değeri alıp bir sonraki sayfaya yönlendiriyoruz.
    $('#filter-button').on('click', function () {
        var izinGrupId = $('#izinGruplari').val();

        if (izinGrupId) {
            window.location.href = `/admin/genel-izin-bilgileri?izingrupid=${izinGrupId}`;
        }
    });

});