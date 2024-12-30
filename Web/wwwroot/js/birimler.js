const orgKoduMaskOptions = {
    "mask": "99999999",
};

const emailMaskOptions = {
    mask: "*{1,50}[.*{1,50}][.*{1,6}]@*{1,50}[.*{1,6}][.*{1,6}]",
    greedy: false,
    onBeforeMask: function (value, opts) {
        return value;
    },
    definitions: {
        '*': {
            validator: "[0-9A-Za-z!#$%&'*+/=?^_`{|}~-]",
            cardinality: 1,
            casing: "lower"
        }
    },
};

function init(birimId = 1) {
    const $ = go.GraphObject.make;

    myFullDiagram =
        new go.Diagram("fullDiagram",
            {
                initialAutoScale: go.Diagram.UniformToFill,
                contentAlignment: go.Spot.Center,
                isReadOnly: true,
                "animationManager.isEnabled": true,
                layout: $(go.TreeLayout,
                    { angle: 90, sorting: go.TreeLayout.SortingAscending }),
                maxSelectionCount: 1,
                "ChangedSelection": showLocalOnFullClick
            });

    myLocalDiagram =
        new go.Diagram("localDiagram",
            {
                initialAutoScale: go.Diagram.UniformToFill,
                contentAlignment: go.Spot.Center,
                isReadOnly: true,
                "animationManager.isInitial": false,
                layout: $(go.TreeLayout,
                    { angle: 90, sorting: go.TreeLayout.SortingAscending }),
                "LayoutCompleted": e => {
                    var sel = e.diagram.selection.first();
                    if (sel !== null) myLocalDiagram.scrollToRect(sel.actualBounds);
                },
                maxSelectionCount: 1,
                "ChangedSelection": showLocalOnLocalClick
            });

    var myNodeTemplate =
        $(go.Node, "Auto",
            { locationSpot: go.Spot.Center },
            new go.Binding("text", "key", go.Binding.toString),
            $(go.Shape, "Rectangle",
                { fill: "#FFE0D9", stroke: null },
                new go.Binding("fill", "color").makeTwoWay()),
            $(go.TextBlock,
                { margin: 5 },
                new go.Binding("text", "name"))
        );

    myFullDiagram.nodeTemplate = myNodeTemplate;
    myLocalDiagram.nodeTemplate = myNodeTemplate;

    var myLinkTemplate =
        $(go.Link,
            { routing: go.Link.Normal, selectable: false },
            $(go.Shape,
                { strokeWidth: 1 })
        );
    myFullDiagram.linkTemplate = myLinkTemplate;
    myLocalDiagram.linkTemplate = myLinkTemplate;

    setupDiagram(birimId);

    highlighter =
        $(go.Part, "Auto",
            {
                layerName: "Background",
                selectable: false,
                isInDocumentBounds: false,
                locationSpot: go.Spot.Center
            },
            $(go.Shape, "Ellipse",
                {
                    fill: $(go.Brush, "Radial", { 0.0: "gold", 0.5: "gold", 1.0: "white" }),
                    stroke: null,
                    desiredSize: new go.Size(400, 400)
                })
        );
    myFullDiagram.add(highlighter);

    myFullDiagram.addDiagramListener("InitialLayoutCompleted", e => {
        myFullDiagram.nodes.each(node => {
            if (node.data.parent === "none") {
                node.isSelected = true;
                return false;
            }
        });
        showLocalOnFullClick();
    });
}

function showLocalOnLocalClick() {
    var selectedLocal = myLocalDiagram.selection.first();
    if (selectedLocal !== null) {
        myFullDiagram.select(myFullDiagram.findPartForKey(selectedLocal.data.key));
    }
}

function showLocalOnFullClick() {
    var node = myFullDiagram.selection.first();
    if (node !== null) {
        myFullDiagram.scrollToRect(node.actualBounds);
        highlighter.location = node.location;
        var model = new go.TreeModel();
        var nearby = node.findTreeParts(3);
        var parent = node.findTreeParentNode();
        if (parent !== null) {
            nearby.add(parent);
            var grandparent = parent.findTreeParentNode();
            if (grandparent !== null) {
                nearby.add(grandparent);
            }
        }
        nearby.each(n => {
            if (n instanceof go.Node) model.addNodeData(n.data);
        });
        myLocalDiagram.model = model;
        var selectedLocal = myLocalDiagram.findPartForKey(node.data.key);
        if (selectedLocal !== null) selectedLocal.isSelected = true;
    }
}

function showFullOnLocalClick() {
    var node = myLocalDiagram.selection.first();
    if (node !== null) {
        var model = new go.TreeModel();
        var nearby = node.findTreeParts(Infinity);
        nearby.each(n => {
            if (n instanceof go.Node) model.addNodeData(n.data);
        });
        myFullDiagram.model = model;
        var selectedFull = myFullDiagram.findPartForKey(node.data.key);
        if (selectedFull !== null) selectedFull.isSelected = true;
    }
}

function selectNodeById(id) {
    var node = myFullDiagram.findNodeForKey(id);

    if (node !== null) {
        myFullDiagram.scrollToRect(node.actualBounds);
        highlighter.location = node.location;
        var model = new go.TreeModel();
        var nearby = node.findTreeParts(3);
        var parent = node.findTreeParentNode();
        if (parent !== null) {
            nearby.add(parent);
            var grandparent = parent.findTreeParentNode();
            if (grandparent !== null) {
                nearby.add(grandparent);
            }
        }
        nearby.each(n => {
            if (n instanceof go.Node) model.addNodeData(n.data);
        });
        myLocalDiagram.model = model;

        var selectedLocal = myLocalDiagram.findPartForKey(node.data.key);
        if (selectedLocal !== null) selectedLocal.isSelected = true;
    }
}

function setupDiagram(birimId = 1) {
    myLocalDiagram.clear();

    $.ajax({
        url: `/admin/organizasyon-sema?birimId=${birimId}`,
        method: 'GET',
        dataType: 'json',
        success: function (data) {
            var total = data.length;
            var nodeDataArray = [...data];
            for (var i = 0; i < total; i++) {
                nodeDataArray.push({
                    key: nodeDataArray[i].key,
                    name: nodeDataArray[i].name,
                    parent: nodeDataArray[i].parent
                });
            }

            myFullDiagram.model = new go.TreeModel(nodeDataArray);
        },
        error: function (xhr, status, error) {
            // Handle errors
            console.error('Error:', status, error);
        }
    });
}

var initDatatable = function () {
    var datatable = $('#organizasyonlar_table').DataTable({
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
            url: '/admin/organizasyonlar',
            type: 'GET',
            dataType: 'json',
            error: function (xhr, status, error) {
                console.log(error);
            }
        },
        'columns': [
            { data: 'id' },
            { data: 'kod' },
            { data: 'aciklama_TR' },
            { data: 'aciklama_EN' },
            { data: 'organizasyonKodu' },
            { data: 'anaBirim' },
            { data: 'ustBirimAciklama' },
            { data: 'aktif' },
        ],
        columnDefs: [
            {
                targets: 0,
                visible: false,
                render: function (data, type, row) {
                    return data;
                },
            },
            {
                targets: 5,
                render: function (data, type, row) {
                    return data === true ? "Evet" : "Hayır";
                },
            },
            {
                targets: 7,
                render: function (data, type, row) {
                    return data === true ? "Evet" : "Hayır";
                },
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
                                    <div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-bold fs-7 w-225px py-4" data-kt-menu="true">
                                         <div class="menu-item px-3">
                                            <a href="#" class="menu-link px-3" data-id=${row.id} onclick="fillOrganizasyonUpdateForm('${row['id']}')"  data-bs-toggle="modal" data-bs-target="#updateOrganizasyonModal"><i class="ki-duotone ki-notepad-edit fs-5 me-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>Güncelle</a>
                                         </div>
                                         <div class="menu-item px-3">
                                            <a href="#" class="menu-link px-3" onclick="deleteOrganizasyonById('${row['id']}')"><i class="ki-duotone ki-trash fs-5 me-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>Sil</a>
                                         </div>
                                         <div class="menu-item px-3">
                                            <a href="#" class="menu-link px-3" onclick="selectNodeById('${row['id']}')" data-bs-toggle="modal" data-bs-target="#orgSchemaModal"><i class="ki-duotone ki-eye fs-5 me-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>Organizasyon Şeması</a>
                                         </div>
                                         <div class="menu-item px-3">
                                            <a href="#" class="menu-link px-3" onclick="setOrganizasyonHareketleri('${row['id']}')" data-bs-toggle="modal" data-bs-target="#organizasyonHareketModal"><i class="ki-duotone ki-information fs-5 me-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span></i>Organizasyon Hareketleri</a>
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
    }).container().appendTo($('#organizasyonlar_table_buttons'));

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

function initSelect2(selector, dropdownParentSelector, url) {
    $(selector).val(null).trigger('change');

    $(selector).select2({
        dropdownParent: $(dropdownParentSelector),
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

function searchOrganizasyonByAd(ustBirimIdSelector) {
    var url = "/admin/organizasyon-list";

    $(ustBirimIdSelector).val(null).trigger('change');

    $(ustBirimIdSelector).select2({
        dropdownParent: $("#createOrganizasyonModal"),
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

function deleteOrganizasyonById(id) {
    var token = $('input[name="__RequestVerificationToken"]').val();

    Swal.fire({
        html: "Birim ve ilgili alt birimler organizasyon şemasından kaldırılacaktır. Onaylıyor musunuz?",
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
                url: `/admin/organizasyonlar/sil/${id}`,
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
                            text: `"${result.aciklama_TR}" açıklamalı izin grubu başarıyla silindi!`,
                            icon: "success",
                            buttonsStyling: false,
                            confirmButtonText: "Tamam",
                            customClass: {
                                confirmButton: "btn btn-success"
                            }
                        }).then(function () {
                            window.location.href = "/admin/organizasyon-semasi-islemleri";
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

function setRadioButton(radioId, isChecked) {
    var myRadio = document.getElementById(radioId);
    myRadio.value = isChecked;
    myRadio.checked = isChecked;
}

function fillSelect2(selector, text, val) {
    var selectElement = $(selector);

    if (selectElement.find("option[value='" + val + "']").length) {
        selectElement.val(val).trigger('change');
    } else {
        var newOption = new Option(text, val, false, false);
        selectElement.append(newOption).val(val).trigger('change');
    }
}

function fillOrganizasyonUpdateForm(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/admin/organizasyonlar/${id}`,
            method: 'GET',
            dataType: 'json',
            success: function (data) {
                var { kod, aciklama_TR, aciklama_EN, adres, aktif, anaBirim, birimId, eposta, faks, id, organizasyonKodu, seviye, telefon, web, ustBirimId, ustBirim } = data;

                $('#UpdateOrganizasyonVM_Id').val(id);
                $('#UpdateOrganizasyonVM_Kod').val(kod);
                $('#UpdateOrganizasyonVM_OrganizasyonKodu').val(organizasyonKodu);
                $('#UpdateOrganizasyonVM_Aciklama_TR').val(aciklama_TR);
                $('#UpdateOrganizasyonVM_Aciklama_EN').val(aciklama_EN);
                $('#UpdateOrganizasyonVM_Adres').text(adres);
                $('#UpdateOrganizasyonVM_Telefon').val(telefon);
                $('#UpdateOrganizasyonVM_Eposta').val(eposta);
                $('#UpdateOrganizasyonVM_Web').val(web);
                $('#UpdateOrganizasyonVM_Faks').val(faks);
                $('#UpdateOrganizasyonVM_Seviye').val(seviye);
                $('#UpdateOrganizasyonVM_BirimId').val(birimId);
                $('#UpdateOrganizasyonVM_UstBirimId').val(ustBirimId)

                setRadioButton("UpdateOrganizasyonVM_AnaBirim", anaBirim);
                setRadioButton("UpdateOrganizasyonVM_Aktif", aktif);

                if (ustBirim == null) {
                    ustBirim = "Yok";
                }

                fillSelect2('#UpdateOrganizasyonVM_UstBirimId', ustBirim, ustBirimId);
            },
            error: function (xhr, status, error) {
                console.error('Error:', status, error);
                reject(new Error(`Request failed: ${status}`)); // Reject the promise with an error
            }
        });
    });
}

var orgOlusturElement = document.querySelector("#stepper_org_olustur");
var orgOlusturStepper = new KTStepper(orgOlusturElement);

var orgUpdateElement = document.querySelector("#stepper_org_update");
var orgUpdateStepper = new KTStepper(orgUpdateElement);

function configureValidationsOrgOlustur() {
    var validations = [];

    var form = document.getElementById('createOrganizasyonForm');

    // Step 1 Validation
    validations.push(FormValidation.formValidation(
        form,
        {
            fields: {
                'CreateOrganizasyonVM.OrganizasyonKodu': {
                    validators: {
                        callback: {
                            message: "Lütfen geçerli bir organizasyon kodu giriniz",
                            callback: function (input) {
                                if (input.value.trim() == '') {
                                    return true;
                                }

                                return Inputmask.isValid(input.value, orgKoduMaskOptions);
                            }
                        }
                    }
                },
                'CreateOrganizasyonVM.Aciklama_TR': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'CreateOrganizasyonVM.Aciklama_EN': {
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

function configureValidationsOrgGuncelle() {
    var validations = [];

    var form = document.getElementById('updateOrganizasyonForm');

    // Step 1 Validation
    validations.push(FormValidation.formValidation(
        form,
        {
            fields: {
                'UpdateOrganizasyonVM.OrganizasyonKodu': {
                    validators: {
                        callback: {
                            message: "Lütfen geçerli bir organizasyon kodu giriniz",
                            callback: function (input) {
                                if (input.value.trim() == '') {
                                    return true;
                                }

                                return Inputmask.isValid(input.value, orgKoduMaskOptions);
                            }
                        }
                    }
                },
                'UpdateOrganizasyonVM.Aciklama_TR': {
                    validators: {
                        notEmpty: {
                            message: 'Bu alan zorunludur'
                        }
                    }
                },
                'UpdateOrganizasyonVM.Aciklama_EN': {
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

function initStepperOrgOlustur() {
    var validations = configureValidationsOrgOlustur();

    orgOlusturStepper.on("kt.stepper.next", function (stepper) {
        var currentStepIndex = stepper.getCurrentStepIndex();

        var validator = validations[currentStepIndex - 1];
        if (validator) {
            validator.validate().then(function (status) {
                if (status == 'Valid') {
                    stepper.goNext();
                }
            });
        } else {
            stepper.goNext();
        }
    });

    // Handle previous step
    orgOlusturStepper.on("kt.stepper.previous", function (stepper) {
        stepper.goPrevious(); // go previous step
    });
}

function initStepperOrgUpdate() {
    var validations = configureValidationsOrgGuncelle();

    orgUpdateStepper.on("kt.stepper.next", function (stepper) {
        var currentStepIndex = stepper.getCurrentStepIndex();

        var validator = validations[currentStepIndex - 1];
        if (validator) {
            validator.validate().then(function (status) {
                if (status == 'Valid') {
                    stepper.goNext();
                }
            });
        } else {
            stepper.goNext();
        }
    });

    // Handle previous step
    orgUpdateStepper.on("kt.stepper.previous", function (stepper) {
        stepper.goPrevious(); // go previous step
    });
}

function setUstBirimId(treeSelector, ustBirimIdSelector) {
    $(treeSelector).on('select_node.jstree', function (e, data) {
        var treeInstance = $(treeSelector).jstree(true);
        var ustBirimId = data.selected[0];

        if (ustBirimId != undefined) {
            treeInstance.select_node(ustBirimId);
            $(ustBirimIdSelector).val(ustBirimId);
        } else {
            treeInstance.deselect_all();
            $(ustBirimIdSelector).val(null);
        }
    });
}

function toggleTree() {
    $(document).on("click", ".toggle", function (e) {
        e.preventDefault();

        var parent = $(this).parent();
        var nextElement = parent.next();
        nextElement.toggle();

        var oldIcon = parent.find('i');
        var newIcon = $('<i>').addClass('ki-duotone').addClass(nextElement.is(":visible") ? 'ki-minus-square toggle fs-1' : 'ki-plus-square toggle fs-1').addClass('toggle');

        newIcon.append($('<span>').addClass('path1'));
        newIcon.append($('<span>').addClass('path2'));
        if (!nextElement.is(":visible")) {
            newIcon.append($('<span>').addClass('path3'));
        }

        oldIcon.replaceWith(newIcon);
    });
}

function setOrganizasyonHareketleri(organizasyonId) {
    $.ajax({
        url: `/admin/organizasyon-hareketleri/${organizasyonId}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            $('#organizasyon-hareketleri-not-found').removeClass('d-none');
            $('#organizasyon-hareketleri-table-wrapper').removeClass('d-none');

            if (data && data.length > 0) {
                $('#organizasyon-hareketleri-not-found').addClass('d-none');

                var element = document.getElementById('organizasyon-hareketleri-table-wrapper');

                var tbody = "";

                console.log(data);

                data.forEach(function (item, index) {
                    var adSoyad = `${item.ad} ${item.soyad}`;
                    var unvan = item.unvan;
                    var islemTarihi = item.islemTarihi;
                    var islem = 0;

                    if (item.islem == 1) {
                        islem = `<span class="badge badge-success fs-7 fw-bold">Oluşturuldu</span>`;
                    } else if (item.islem == 2) {
                        islem = `<span class="badge badge-primary fs-7 fw-bold">Güncellendi</span>`;
                    }

                    tbody += `<tr>
                                    <th>${index + 1}</th>
                                    <td>${islemTarihi}</th >
                                    <td class="text-center">${adSoyad}<br />${unvan}</th >
                                    <td>${islem}</td>
                              </tr>`
                });

                element.innerHTML = `<table class="table table-bordered">
                                      <thead>
                                        <tr>
                                          <th>Sıralama</th>
                                          <th>İşlem Tarihi</th>
                                          <th class="text-center">İşlem Yapan Kişi</th>
                                          <th>Yapılan İşlem</th>
                                        </tr>
                                      </thead>
                                      <tbody>
                                            ${tbody}
                                      </tbody>
                                 </table>`;

            } else {
                $('#organizasyon-hareketleri-table-wrapper').addClass('d-none');
            }
        },
        error: function (xhr, status, error) {
            toastr.error("Beklenmeyen bir hata oluştu!");
        }
    });
}

$(function () {
    init();
    setupDiagram();

    initDatatable();

    setCheckboxValuesOnChange();

    initSelect2("#CreateOrganizasyonVM_BirimId", "#createOrganizasyonModal", "/admin/birim-list");
    initSelect2("#UpdateOrganizasyonVM_BirimId", "#updateOrganizasyonModal", "/admin/birim-list");

    initStepperOrgOlustur();
    initStepperOrgUpdate();

    $("#updateOrganizasyonForm").on('submit', function (event) {
        event.preventDefault();

        var formElements = this.elements;
        for (var i = 0; i < formElements.length; i++) {
            if (formElements[i].name === 'UpdateOrganizasyonVM.UstBirimId') {
                formElements[i].value = $("#UpdateOrganizasyonVM_UstBirimId").val();
            }
        }

        this.submit();
    });

    Inputmask(orgKoduMaskOptions).mask("#CreateOrganizasyonVM_OrganizasyonKodu");
    Inputmask(orgKoduMaskOptions).mask("#UpdateOrganizasyonVM_OrganizasyonKodu");

    toggleTree();
});
