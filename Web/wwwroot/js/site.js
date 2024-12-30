function getBadge(data) {
    var switchData = data;
    const badgeClasses = {
        "Beklemede": "badge-light-primary",
        "Onaylandı": "badge-light-success",
        "Reddedildi": "badge-light-danger",
        "İptal Edildi": "badge-light-dark",
        "Düzeltilmek Üzere Geri Gönderildi": "badge-light-warning",
        "Düzenlendi": "badge-success",
    };

    if (switchData.endsWith("Bekleniyor")) {
        switchData = "Beklemede";
    }

    return `<span class="badge ${badgeClasses[switchData] || ''} fs-7 fw-bold">${data}</span>`;
}

function formatDate(tarih) {
    var dateParts = tarih.split(".");
    var formattedDate = dateParts[2] + '-' + dateParts[1] + '-' + dateParts[0];
    return formattedDate;
}

const loadingEl = document.createElement("div");
function startPageLoadingOverlay() {
    document.body.prepend(loadingEl);
    loadingEl.classList.add("page-loader");
    loadingEl.classList.add("flex-column");
    loadingEl.classList.add("bg-dark");
    loadingEl.classList.add("bg-opacity-25");
    loadingEl.innerHTML = `
        <span class="spinner-border text-primary" role="status"></span>
        <span class="text-gray-800 fs-1 fw-semibold mt-5">Yükleniyor...</span>
    `;

    // Show page loading
    KTApp.showPageLoading();
}

function stopPageLoadingOverlay() {
    setTimeout(function () {
        KTApp.hidePageLoading();
        loadingEl.remove();
    }, 500);
}

function setCheckboxValuesOnChange() {
    const checkboxes = document.querySelectorAll('input[type="checkbox"]');

    checkboxes.forEach(function (checkbox) {
        checkbox.addEventListener('change', function () {
            this.value = this.checked ? 'true' : 'false';
        });
    });
}

const flatpickrConfig = {
    dateFormat: "d.m.Y",
    locale: {
        firstDayOfWeek: 1,
        weekdays: {
            longhand: ['Pazar', 'Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi'],
            shorthand: ['Paz', 'Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt']
        },
        months: {
            longhand: ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'],
            shorthand: ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara']
        },
        today: 'Bugün',
        clear: 'Temizle'
    }
};

function setFlatpickrDate(selector, tarih) {
    flatpickr(selector, flatpickrConfig).setDate(tarih);
    $(selector).val(tarih);
}

function startPageLoadingOverlay() {
    const loadingEl = document.createElement("div");
    document.body.prepend(loadingEl);
    loadingEl.classList.add("page-loader");
    loadingEl.classList.add("flex-column");
    loadingEl.innerHTML = `
        <span class="spinner-border text-primary" role="status"></span>
        <span class="text-muted fs-6 fw-semibold mt-5">Lütfen bekleyin...</span>
    `;

    KTApp.showPageLoading();
}

function downloadPDF(id) {
    var prtContent = document.getElementById(id).innerHTML;
    var mywindow = window.open('', '', 'left=0,top=0,width=900,height=600,toolbar=1,scrollbars=1,status=0');

    mywindow.document.write('<html><head><title>İzin Çıktısı</title>');

    var links = document.getElementsByTagName('link');
    for (var i = 0; i < links.length; i++) {
        mywindow.document.write(links[i].outerHTML);
    }

    mywindow.document.write('<style>@media print { body { transform: scaleY(0.95); transform-origin: top; } }</style>');

    mywindow.document.write('</head><body>');
    mywindow.document.write(prtContent);
    mywindow.document.write('</body></html>');
    mywindow.document.close();
    mywindow.focus();

    setTimeout(function () {
        mywindow.print();
        mywindow.close();
    }, 100);

    prtContent.innerHTML = "";
}

function getQueryParam(parameter) {
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);

    if (urlParams.has(parameter)) {
        return urlParams.get(parameter);
    } else {
        return null;
    }
}
