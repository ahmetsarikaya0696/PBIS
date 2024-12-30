var initDatatable = function () {
    var datatable = $('#izin_gruplari_table').DataTable({
        "info": false,
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
            url: '/izingruplari',
            type: 'GET',
            dataType: 'json',
            error: function (xhr, status, error) {
                console.log(error); // Handle error gracefully
            }
        },
        'columns': [
            { data: 'id' },
            { data: 'aciklama' },
            { data: 'unvanAdSoyad' },
            { data: 'unvan' },
            { data: 'birim' },
            { data: 'isyeri' },
            { data: 'baslangicTarihi' },
            { data: 'bitisTarihi' },
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
                targets: 8,
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
                                            <a href="#" class="menu-link px-3" data-id='${row['id']}' onclick="fillIzinGrubuUpdateForm('${row['id']}')" data-bs-toggle="modal" data-bs-target="#updateIzinGrupModal"><i class="ki-duotone ki-notepad-edit fs-5 me-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>Güncelle</a>
                                          </div>
                                          <div class="menu-item px-3">
                                            <a href="#" class="menu-link px-3" onclick="deleteIzinGrupById('${row['id']}')"><i class="ki-duotone ki-trash fs-5 me-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>Sil</a>
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
    }).container().appendTo($('#izin_gruplari_table_buttons'));

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

function searchCalisanByAdSoyad() {
    var url = "/admin/calisanlar";

    $('#CreateIzinGrupVM_CalisanId').val(null).trigger('change');

    $('#CreateIzinGrupVM_CalisanId').select2({
        dropdownParent: $("#createIzinGrupModal"),
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

function initSelect2() {
    var url = "/admin/birim-list";

    $('#CreateIzinGrupVM_BirimId').val(null).trigger('change');

    $('#CreateIzinGrupVM_BirimId').select2({
        dropdownParent: $("#createIzinGrupModal"),
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

function searchIsyeriByAd() {
    var url = "/admin/isyerleri";

    $('#CreateIzinGrupVM_IsyeriId').val(null).trigger('change');

    $('#CreateIzinGrupVM_IsyeriId').select2({
        dropdownParent: $("#createIzinGrupModal"),
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

function searchUnvanByAd() {
    var url = "/admin/unvanlar";

    $('#CreateIzinGrupVM_UnvanId').val(null).trigger('change');

    $('#CreateIzinGrupVM_UnvanId').select2({
        dropdownParent: $("#createIzinGrupModal"),
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

function deleteIzinGrupById(id) {
    var token = $('input[name="__RequestVerificationToken"]').val();

    Swal.fire({
        html: "İzin grubu silinecektir. Onaylıyor musunuz?",
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
                url: `izingruplari/sil/${id}`,
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
                            text: `"${result}" açıklamalı izin grubu başarıyla silindi!`,
                            icon: "success",
                            buttonsStyling: false,
                            confirmButtonText: "Tamam",
                            customClass: {
                                confirmButton: "btn btn-success"
                            }
                        }).then(function () {
                            window.location.href = "/admin/izingruplari";
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

function updateSelectOption(selectId, optionText) {
    var select = document.getElementById(selectId);
    select.innerHTML = '';

    var option = document.createElement('option');
    option.text = optionText === "-" ? "" : optionText;
    select.add(option);
    option.selected = true;
}

function fillIzinGrubuUpdateForm(id) {
    var row = $(`a[data-id="${id}"]`).closest("tr");
    var aciklama = row.find('td:eq(0)').text();
    var calisan = row.find('td:eq(1)').text();
    var unvan = row.find('td:eq(2)').text();
    var birim = row.find('td:eq(3)').text();
    var isyeri = row.find('td:eq(4)').text();
    var baslangicTarihi = row.find('td:eq(5)').text();
    var bitisTarihi = row.find('td:eq(6)').text();

    document.getElementById('UpdateIzinGrupVM_Id').value = id;
    document.getElementById('UpdateIzinGrupVM_Aciklama').value = aciklama;

    updateSelectOption("UpdateIzinGrupVM_CalisanId", calisan);
    updateSelectOption("UpdateIzinGrupVM_UnvanId", unvan);
    updateSelectOption("UpdateIzinGrupVM_BirimId", birim);
    updateSelectOption("UpdateIzinGrupVM_IsyeriId", isyeri);

    document.getElementById('UpdateIzinGrupVM_BaslangicTarihi').value = baslangicTarihi;
    document.getElementById('UpdateIzinGrupVM_BitisTarihi').value = bitisTarihi;

    // Get Izin Onay Tanim Ad ve Sıra By izin grubu id
    $.ajax({
        url: `/admin/izinonaytanim?izinGrupId=${id}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            var content = "";
            data.forEach(function (element, index) {
                var sira = element.sira;
                var izinOnayTanim = element.izinOnayTanim;

                content += ` <div class="form-group row mb-6">
                                <div class="col-2 fv-row">
                                    <label for="update-sira-${sira}" class="form-label">Onay Sırası</label>
                                    <input type="number" class="form-control mb-2 mb-md-0 w-75" id="update-sira-${sira}" name="update-sira-${sira}" value="${sira}" disabled />
                                </div>
                                <div class="col-10 fv-row w-50">
                                    <label for="update-izin-onay-tanim-${sira}" class="form-label">İzin Onay Tanımı</label>
                                    <select id="update-izin-onay-tanim-${sira}" name="update-izin-onay-tanim-${sira}" class="form-select" disabled>
                                        <option selected>${izinOnayTanim}</option>
                                    </select>
                                </div>
                             </div>`
            });

            document.getElementById('izin-onay-tanim-ve-sira').innerHTML = content;
        },
        error: function (xhr, status, error) {
            console.log(error); // Handle error gracefully
        }
    });

}

function createIzinGrup() {
    const createform = document.getElementById('izinGrupForm');
    var createFormValidator = FormValidation.formValidation(
        createform,
        {
            fields: {
                'CreateIzinGrupVM.Aciklama': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'CreateIzinGrupVM.BaslangicTarihi': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'CreateIzinGrupVM.BitisTarihi': {
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

    document.getElementById('izinGrupForm').addEventListener('submit', function (event) {
        event.preventDefault();

        if (createFormValidator) {
            createFormValidator.validate().then(function (status) {
                if (status == 'Valid') {
                    // Custom validator begin
                    var calisanId = document.getElementById('CreateIzinGrupVM_CalisanId').value;
                    var unvanId = document.getElementById('CreateIzinGrupVM_UnvanId').value;
                    var birimId = document.getElementById('CreateIzinGrupVM_BirimId').value;
                    var isyeriId = document.getElementById('CreateIzinGrupVM_IsyeriId').value;

                    var errorMessage = '';

                    if (calisanId && (unvanId || birimId || isyeriId)) {
                        errorMessage = "Çalışan adı girildiğinde unvan, birim ve işyeri alanları doldurulmamalıdır!";
                    }
                    else if (unvanId && (!birimId || !isyeriId)) {
                        errorMessage = "Unvan girildiğinde birim ve işyeri alanları doldurulmamalıdır!";
                    }
                    else if (!calisanId && !unvanId && !(birimId || isyeriId)) {
                        errorMessage = "Çalışan ve Unvan girilmediğinde birim veya işyeri alanlarında en az biri doldurulmamalıdır!";
                    }

                    if (errorMessage) {
                        document.getElementById('errorMessage').innerHTML = errorMessage;
                        $('#alert-danger').removeClass('d-none');
                        return;
                    }

                    $('#alert-danger').addClass('d-none');
                    // Custom validator end


                    var onayTanimIdInputList = document.getElementsByClassName("onay-tanim");
                    var siraInputList = document.getElementsByClassName("sira");

                    siraInputList.forEach(function (element, index) {
                        element.name = `CreateIzinGrupVM.IzinOnayTanimVeSiralari[${index}].Sira`;
                    })

                    onayTanimIdInputList.forEach(function (element, index) {
                        element.name = `CreateIzinGrupVM.IzinOnayTanimVeSiralari[${index}].IzinOnayTanimId`;
                    })

                    createform.submit();
                }
            });
        }
    })
}

function updateIzinGrup() {
    const updateForm = document.getElementById('updateIzinGrupForm');
    var updateFormValidator = FormValidation.formValidation(
        updateForm,
        {
            fields: {
                'UpdateIzinGrupVM.Aciklama': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'UpdateIzinGrupVM.BaslangicTarihi': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'UpdateIzinGrupVM.BitisTarihi': {
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

    document.getElementById('updateIzinGrupForm').addEventListener('submit', function (event) {
        event.preventDefault();

        if (updateFormValidator) {
            updateFormValidator.validate().then(function (status) {
                if (status == 'Valid') {
                    updateForm.submit();
                }
            });
        }
    })
}


$(function () {
    initDatatable();

    $("#CreateIzinGrupVM_BaslangicTarihi, #CreateIzinGrupVM_BitisTarihi, #UpdateIzinGrupVM_BaslangicTarihi, #UpdateIzinGrupVM_BitisTarihi").flatpickr(flatpickrConfig);

    $('#repeater').repeater({
        initEmpty: false,

        defaultValues: {
            'text-input': 'foo'
        },

        show: function () {
            $(this).slideDown();
        },

        hide: function (deleteElement) {
            $(this).slideUp(deleteElement);
        }
    });


    searchCalisanByAdSoyad();
    initSelect2();
    searchIsyeriByAd();
    searchUnvanByAd();

    $("#CreateIzinGrupVM_CalisanId").on("change", function (e) {
        var calisanId = $(this).val();

        $("#CreateIzinGrupVM_IsyeriId, #CreateIzinGrupVM_BirimId, #CreateIzinGrupVM_UnvanId")
            .val('')
            .prop("disabled", !!calisanId)
            .trigger('change');
    });

    createIzinGrup();
    updateIzinGrup();
});
