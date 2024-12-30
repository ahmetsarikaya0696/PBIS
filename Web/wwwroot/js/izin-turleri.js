var initDatatable = function () {
    var datatable = $('#izin_tipleri_table').DataTable({
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
            url: '/admin/izin-turleri',
            type: 'GET',
            dataType: 'json',
            error: function (xhr, status, error) {
                console.log(error); // Handle error gracefully
            }
        },
        'columns': [
            { data: 'id' },
            { data: 'aciklama' },
            { data: 'izinFormTipi' },
            { data: 'sabitGunSayisi' },
            { data: 'tatilGunleriSayilir' },
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
                targets: 6,
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
                                            <a href="#" class="menu-link px-3" data-id='${row['id']}' onclick="fillIzinTurUpdateForm('${row['id']}')" data-bs-toggle="modal" data-bs-target="#updateİzinTipModal"><i class="ki-duotone ki-notepad-edit fs-5 me-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>Güncelle</a>
                                         </div>
                                         <div class="menu-item px-3">
                                            <a href="#" class="menu-link px-3" onclick="deleteIzinTurById('${row['id']}')"><i class="ki-duotone ki-trash fs-5 me-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>Sil</a>
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
    }).container().appendTo($('#izin_tipleri_table_buttons'));

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
function deleteIzinTurById(id) {
    var token = $('input[name="__RequestVerificationToken"]').val();

    Swal.fire({
        html: "İzin türü silinecektir. Onaylıyor musunuz?",
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
                url: `/admin/izinturleri/sil/${id}`,
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
                            window.location.href = "/admin/izinturleri";
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

function fillIzinTurUpdateForm(id) {
    var row = $(`a[data-id="${id}"]`).closest("tr");
    var aciklama = row.find('td:eq(0)').text();
    var formTipi = row.find('td:eq(1)').text() == "Form" ? 'F' : 'T';
    var sabitGunSayisi = row.find('td:eq(2)').text() == "-" ? "" : row.find('td:eq(2)').text();
    var tatilGunleriSayilir = row.find('td:eq(3)').text() == "Evet";
    var aktif = row.find('td:eq(4)').text() == "Evet";

    var idInput = document.getElementById('UpdateIzinTurVM_Id');
    var aciklamaInput = document.getElementById('UpdateIzinTurVM_Aciklama');
    var sabitGunSayisiInput = document.getElementById('UpdateIzinTurVM_SabitGunSayisi');
    var formTipiInput = document.getElementById('UpdateIzinTurVM_IzinFormTipi');
    var tatilGunleriSayilirInput = document.getElementById('UpdateIzinTurVM_TatilGunleriSayilir');
    var aktifInput = document.getElementById('UpdateIzinTurVM_Aktif');

    idInput.value = id;
    aciklamaInput.value = aciklama;
    sabitGunSayisiInput.value = sabitGunSayisi;

    for (var i = 0; i < formTipiInput.options.length; i++) {
        if (formTipiInput.options[i].value === formTipi) {
            formTipiInput.options[i].selected = true;
            break;
        }
    }

    tatilGunleriSayilirInput.value = tatilGunleriSayilir;
    tatilGunleriSayilirInput.checked = tatilGunleriSayilir;

    aktifInput.value = aktif;
    aktifInput.checked = aktif;

    formTipiInput.dispatchEvent(new Event('change'));
}

function updateIzinTur() {
    const updateform = document.getElementById('updateIzinTurForm');
    var updateFormValidator = FormValidation.formValidation(
        updateform,
        {
            fields: {
                'UpdateIzinTurVM.Aciklama': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'UpdateIzinTurVM.SabitGunSayisi': {
                    validators: {
                        callback: {
                            message: 'Boş bırakınız veya sıfırdan büyük bir sabit gün giriniz',
                            callback: function (input) {
                                var sabitGunSayisi = document.getElementById('UpdateIzinTurVM_SabitGunSayisi').value;
                                return sabitGunSayisi == '' || sabitGunSayisi > 0;
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

    document.getElementById('updateIzinTurForm').addEventListener('submit', function (event) {
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

function createIzinTur() {
    const createform = document.getElementById('createIzinTurForm');
    var createFormValidator = FormValidation.formValidation(
        createform,
        {
            fields: {
                'CreateIzinTurVM.Aciklama': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'CreateIzinTurVM.SabitGunSayisi': {
                    validators: {
                        callback: {
                            message: 'Boş bırakınız veya sıfırdan büyük bir sabit gün giriniz',
                            callback: function (input) {
                                var sabitGunSayisi = document.getElementById('CreateIzinTurVM_SabitGunSayisi').value;
                                return sabitGunSayisi == '' || sabitGunSayisi > 0;
                            }
                        }
                    }
                },
                'CreateIzinTurVM.IzinFormTipi': {
                    validators: {
                        callback: {
                            message: 'Bu alan zorunludur',
                            callback: function (input) {
                                var formtip = document.getElementById('CreateIzinTurVM_IzinFormTipi').value;
                                return formtip != '';
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

    document.getElementById('createIzinTurForm').addEventListener('submit', function (event) {
        event.preventDefault();

        if (createFormValidator) {
            createFormValidator.validate().then(function (status) {
                console.log(status)
                if (status == 'Valid') {
                    createform.submit();
                }
            });
        }
    })
}

$(function () {
    initDatatable();

    setCheckboxValuesOnChange();

    updateIzinTur();

    createIzinTur();
})