$(function () {
    $("#MahsubenBaslangicTarihi, #MahsubenBitisTarihi, #IzinHakedisTarihi").flatpickr(flatpickrConfig);

    $('#submit_button_onayla').on('click', function (e) {
        e.preventDefault();
        const indicator = e.target.getAttribute('data-id');
        Swal.fire({
            html: "İzin talebi onaylanacaktır. Devam etmek istiyor musunuz?",
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
                    url: `/islemler/izin/onayla/${indicator}`,
                    type: 'POST',
                    data: {
                        __RequestVerificationToken: $('input[name=__RequestVerificationToken]').val(),
                        izinId: indicator
                    },
                    success: function (result) {
                        Swal.fire({
                            text: "İzin isteği onay işlemi başarıyla gerçekleşti!",
                            icon: "success",
                            buttonsStyling: false,
                            confirmButtonText: "Tamam",
                            customClass: {
                                confirmButton: "btn btn-success"
                            }
                        }).then(function () {
                            window.location.href = "/admin/islemler/izin-talepleri";
                        });

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


    });


    const izinRetForm = document.getElementById('izinRetForm');
    izinRetForm.addEventListener('submit', function (event) {
        event.preventDefault();

        Swal.fire({
            html: "İzin talebi reddedilecektir. Devam etmek istiyor musunuz?",
            icon: "question",
            showCancelButton: true,
            buttonsStyling: false,
            confirmButtonText: "Evet, Reddet!",
            cancelButtonText: "Hayır, vazgeç",
            customClass: {
                confirmButton: "btn fw-bold btn-primary",
                cancelButton: "btn fw-bold btn-light"
            }
        }).then(function (result) {
            if (result.value) {
                izinRetForm.submit();
            }
        });
    });

});