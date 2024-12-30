const selectElement = document.getElementById('UpdateIzinOnayTanimVM_CalisanIdleri');

var initDatatable = function () {
    var datatable = $('#izin_onay_tanimlari_table').DataTable({
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
            url: '/admin/izin-onay-tanimlari',
            type: 'GET',
            dataType: 'json',
            error: function (xhr, status, error) {
                console.log(error);
            }
        },
        'columns': [
            { data: 'id' },
            { data: 'aciklama' },
            { data: 'merkezMuduruYetkisi' },
            { data: 'personelSubeYetkisi' },
            { data: 'aktif' },
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
                orderable: false,
                render: function (data, type, row) {
                    return `<div class="d-flex justify-content-center">
                                <div class="ms-2">
                                    <button type="button" class="btn btn-sm btn-icon btn-light btn-active-light-primary me-2" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
                                        <span class="svg-icon svg-icon-5 m-0">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
                                                <rect x="10" y="10" width="4" height="4" rx="2" fill="black" />
                                                <rect x="17" y="10" width="4" height="4" rx="2" fill="black" />
                                                <rect x="3" y="10" width="4" height="4" rx="2" fill="black" />
                                            </svg>
                                        </span>
                                    </button>
                                    <div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-bold fs-7 w-150px py-4" data-kt-menu="true">
                                         <div class="menu-item px-3">
                                            <a href="#" class="menu-link px-3" data-id=${row.id} onclick="fillIzinOnayTanimUpdateForm('${row['id']}')"  data-bs-toggle="modal" data-bs-target="#updateİzinOnayTanimModal"><i class="ki-duotone ki-notepad-edit fs-5 me-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>Güncelle</a>
                                         </div>
                                         <div class="menu-item px-3">
                                            <a href="#" class="menu-link px-3" onclick="deleteIzinOnayTanimById('${row['id']}')"><i class="ki-duotone ki-trash fs-5 me-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>Sil</a>
                                         </div>
                                    </div>
                                </div>
                            </div>`;
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
                title: 'tatil günleri'
            },
            {
                extend: 'excelHtml5',
                title: 'tatil günleri'
            },
            {
                extend: 'csvHtml5',
                title: 'tatil günleri'
            },
            {
                extend: 'pdfHtml5',
                title: 'tatil günleri'
            }
        ]
    }).container().appendTo($('#izin_onay_tanimlari_table_buttons'));

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

function searchCalisanByAdSoyad(id) {
    var url = "/admin/calisanlar";

    $(id).val(null).trigger('change');

    $(id).select2({
        minimumInputLength: 3,
        ajax: {
            url: url,
            delay: 500,
            data: function (params) {
                var query = {
                    search: params.term
                };

                return query;
            },
            processResults: function (data) {
                return {
                    results: data
                };
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        }
    });
}

function fillIzinOnayTanimUpdateForm(id) {
    var row = $(`a[data-id="${id}"]`).closest("tr");

    var aciklama = row.find('td:eq(0)').text();

    var merkezMuduruYetkisi = row.find('td:eq(1)').text() == "Evet";
    var personelSubeYetkisi = row.find('td:eq(2)').text() == "Evet";
    var aktif = row.find('td:eq(3)').text() == "Evet";

    var aktifInput = document.getElementById('UpdateIzinOnayTanimVM_Aktif');
    var merkezMuduruYetkisiInput = document.getElementById('UpdateIzinOnayTanimVM_MerkezMuduruYetkisi');
    var personelSubeYetkisiInput = document.getElementById('UpdateIzinOnayTanimVM_PersonelSubeYetkisi');

    document.getElementById('UpdateIzinOnayTanimVM_Id').value = id;
    document.getElementById('UpdateIzinOnayTanimVM_Aciklama').value = aciklama;

    aktifInput.value = aktif;
    aktifInput.checked = aktif;

    merkezMuduruYetkisiInput.value = merkezMuduruYetkisi;
    merkezMuduruYetkisiInput.checked = merkezMuduruYetkisi;

    personelSubeYetkisiInput.value = personelSubeYetkisi;
    personelSubeYetkisiInput.checked = personelSubeYetkisi;

    // optionları da selected olarak çek
    $.ajax({
        url: `/admin/calisanlar/izinonaytanim/${id}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            if (data && data.length > 0) {

                selectElement.innerHTML = '';

                data.forEach(item => {
                    let option = document.createElement('option');
                    option.value = item.id;
                    option.text = item.adSoyad;
                    option.selected = true;
                    selectElement.add(option);
                });
            }
        },
        error: function (xhr, status, error) {
            console.log(error); // Handle error gracefully
        }
    });


}

function deleteIzinOnayTanimById(id) {
    var token = $('input[name="__RequestVerificationToken"]').val();

    Swal.fire({
        html: "İzin onay tanımı silinecektir. Onaylıyor musunuz?",
        icon: "question",
        showCancelButton: true,
        buttonsStyling: false,
        confirmButtonText: "Evet, Onaylıyorum!",
        cancelButtonText: "Hayır, vazgeç",
        customClass: {
            confirmButton: "btn fw-bold btn-primary",
            cancelButton: "btn fw-bold btn-light"
        }
    }).then(function (result) {
        if (result.value) {
            $.ajax({
                url: `/admin/izinonaytanimlari/sil/${id}`,
                type: 'POST',
                data: {
                    id: id
                },
                headers: {
                    RequestVerificationToken: token
                },
                success: function (result) {
                    if (!result.errorMessage) {
                        Swal.fire({
                            text: `${result}`,
                            icon: "success",
                            buttonsStyling: false,
                            confirmButtonText: "Tamam",
                            customClass: {
                                confirmButton: "btn btn-success"
                            }
                        }).then(function () {
                            window.location.href = "/admin/izinonaytanimlari";
                        });
                    } else {
                        Swal.fire({
                            text: result.errorMessage + " İşlem başarısız",
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Tamam",
                            customClass: {
                                confirmButton: "btn btn-danger"
                            }
                        });
                    }


                },
                error: function (xhr, status, error) {
                    Swal.fire({
                        text: "İşlem Başarısız. Beklenmeyen bir hata meydana geldi!",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Tamam",
                        customClass: {
                            confirmButton: "btn btn-danger"
                        }
                    });
                }
            });
        }
    });
}

function updateIzinOnayTanim() {
    const updateform = document.getElementById('updateİzinOnayTanimForm');
    var updateFormValidator = FormValidation.formValidation(
        updateform,
        {
            fields: {
                'UpdateIzinOnayTanimVM.Aciklama': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'UpdateIzinOnayTanimVM.CalisanIdleri': {
                    validators: {
                        callback: {
                            message: 'En az 1 onay tanım yetkilisi seçilmek zorundadır',
                            callback: function (input) {
                                return document.getElementById('UpdateIzinOnayTanimVM_CalisanIdleri').selectedOptions.length > 0;
                            }
                        }
                    }
                }
            },

            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap5({
                    rowSelector: '.fv-row',
                    eleInvalidClass: '',
                    eleValidClass: ''
                })
            }
        }
    );

    document.getElementById('updateİzinOnayTanimForm').addEventListener('submit', function (event) {
        event.preventDefault();

        if (updateFormValidator) {
            updateFormValidator.validate().then(function (status) {
                if (status == 'Valid') {
                    updateform.submit();
                }
            });
        }
    })
}

function createIzinOnayTanim() {
    const createform = document.getElementById('createIzinOnayTanimForm');
    var createFormValidator = FormValidation.formValidation(
        createform,
        {
            fields: {
                'CreateIzinOnayTanimVM.Aciklama': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'CreateIzinOnayTanimVM.CalisanIdleri': {
                    validators: {
                        callback: {
                            message: 'En az 1 onay tanım yetkilisi seçilmek zorundadır',
                            callback: function (input) {
                                return document.getElementById('CreateIzinOnayTanimVM_CalisanIdleri').selectedOptions.length > 0;
                            }
                        }
                    }
                }
            },

            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap5({
                    rowSelector: '.fv-row',
                    eleInvalidClass: '',
                    eleValidClass: ''
                })
            }
        }
    );

    document.getElementById('createIzinOnayTanimForm').addEventListener('submit', function (event) {
        event.preventDefault();

        if (createFormValidator) {
            createFormValidator.validate().then(function (status) {
                if (status == 'Valid') {
                    createform.submit();
                }
            });
        }
    })
}

$(function () {
    initDatatable();

    searchCalisanByAdSoyad('#CreateIzinOnayTanimVM_CalisanIdleri');
    searchCalisanByAdSoyad('#UpdateIzinOnayTanimVM_CalisanIdleri');

    setCheckboxValuesOnChange();

    updateIzinOnayTanim();
    createIzinOnayTanim();
})