var initDatatable = function () {
    var organizasyonId = getQueryParam("organizasyonId");
    var url = `/rapor/izinler?organizasyonId=${organizasyonId}`;

    var datatable = $('#izin_rapor_table').DataTable({
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
            { data: 'unvan' },
            { data: 'ad' },
            { data: 'soyad' },
            { data: 'birim' },
            { data: 'baslangic' },
            { data: 'bitis' },
            { data: 'izinTur' },
            { data: 'gunSayisi' },
        ]
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
    }).container().appendTo($('#izin_rapor_table_buttons'));

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
    initDatatable();

    $('#selectlist').val(getQueryParam("organizasyonId"));
    $('#selectlist').trigger('change');

    $('#filterBtn').on('click', function () {
        var organizasyonId = $('#selectlist').val();
        window.location.href = `/rapor/izin?organizasyonId=${organizasyonId}`;
    });
});