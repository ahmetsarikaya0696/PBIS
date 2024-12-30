var initDatatable = function () {
    var datatable = $('#tatil_gunleri_table').DataTable({
        "info": true,
        'order': [],
        'pageLength': 10,
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
            url: '/tatilgunleri',
            type: 'GET',
            dataType: 'json',
            error: function (xhr, status, error) {
                console.log(error); // Handle error gracefully
            }
        },
        'columns': [
            { data: 'id' },
            { data: 'aciklama' },
            { data: 'tarih' },
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
                targets: 3,
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
                                             <a href="#" class="menu-link px-3" data-id='${row['id']}' onclick="fillTatilUpdateForm('${row['id']}')" data-bs-toggle="modal" data-bs-target="#updateTatilModal"><i class="ki-duotone ki-notepad-edit fs-5 me-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>Güncelle</a>
                                           </div>
                                           <div class="menu-item px-3">
                                             <a href="#" class="menu-link px-3" onclick="deleteTatilById('${row['id']}')"><i class="ki-duotone ki-trash fs-5 me-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>Sil</a>
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
    }).container().appendTo($('#tatil_gunleri_table_buttons'));

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

function deleteTatilById(id) {
    var token = $('input[name="__RequestVerificationToken"]').val();

    Swal.fire({
        html: "Tatil günü silinecektir. Onaylıyor musunuz?",
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
                url: `/admin/tatilgunleri/sil/${id}`,
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
                            text: `${result.tarih} tarihli ${result.aciklama} tatil günü başarıyla silindi!`,
                            icon: "success",
                            buttonsStyling: false,
                            confirmButtonText: "Tamam",
                            customClass: {
                                confirmButton: "btn btn-success"
                            }
                        }).then(function () {
                            window.location.href = "/admin/tatilgunleri";
                        });
                    } else {
                        Swal.fire({
                            text: result.errorMessage + " İşlem Başarısız.",
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Tamam",
                            customClass: {
                                confirmButton: "btn btn-danger"
                            }
                        })
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
                    })
                }
            });
        }
    });
}

function fillTatilUpdateForm(id) {
    var row = $(`a[data-id="${id}"]`).closest("tr");
    var aciklama = row.find('td:eq(0)').text();
    var tarih = row.find('td:eq(1)').text();

    document.getElementById('UpdateId').value = id;
    document.getElementById('UpdateAciklama').value = aciklama;
    document.getElementById('UpdateTarih').value = tarih;
}

function updateTatil() {
    var token = $('input[name="__RequestVerificationToken"]').val();

    var UpdateId = document.getElementById('UpdateId').value;
    var UpdateAciklama = document.getElementById('UpdateAciklama').value;
    var UpdateTarih = document.getElementById('UpdateTarih').value;


    $.ajax({
        url: 'tatilgunleri/guncelle',
        type: 'POST',
        data: {
            UpdateId,
            UpdateAciklama,
            UpdateTarih
        },
        headers: {
            RequestVerificationToken: token
        },
        success: function (result) {
            if (!result.errorMessage) {
                window.location.href = "/admin/tatilgunleri";
            } else {
                toastr.error(result.errorMessage);
            }

        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });

}


$(function () {
    initDatatable();

    $(".tarih, #UpdateTarih").flatpickr(flatpickrConfig);

    const updateform = document.getElementById('updateTatilForm');

    const tatilGunleriForm = document.getElementById('tatilGunleriForm');

    var tatilGunleriFormValidator = FormValidation.formValidation(
        tatilGunleriForm,
        {
            fields: {
                'Aciklama': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'Tarih': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
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

    var updateFormValidator = FormValidation.formValidation(
        updateform,
        {
            fields: {
                'UpdateAciklama': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'UpdateTarih': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
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

    tatilGunleriForm.addEventListener('submit', function (event) {
        event.preventDefault();
        tatilGunleriFormValidator.validate().then(function (status) {
            if (status == 'Valid') {
                tatilGunleriForm.submit();
            }
        });
    })

    const btnGuncelle = document.getElementById('btnGuncelle');
    btnGuncelle.addEventListener('click', function (e) {
        e.preventDefault();

        if (updateFormValidator) {
            updateFormValidator.validate().then(function (status) {
                if (status == 'Valid') {
                    updateform.submit();
                }
            });
        }
    });
});
