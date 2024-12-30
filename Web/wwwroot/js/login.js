function validateLogin() {
    const loginForm = document.getElementById('loginForm');
    var loginFormValidator = FormValidation.formValidation(
        loginForm,
        {
            fields: {
                'Username': {
                    validators: {
                        notEmpty: {
                            message: 'Lütfen bir kullanıcı adı giriniz'
                        }
                    }
                },
                'Password': {
                    validators: {
                        notEmpty: {
                            message: 'Lütfen bir şifre giriniz'
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

    document.getElementById('loginForm').addEventListener('submit', function (event) {
        event.preventDefault();

        if (loginFormValidator) {
            loginFormValidator.validate().then(function (status) {
                if (status == 'Valid') {
                    loginForm.submit();
                }
            });
        }
    })
}

$(function () {
    validateLogin();
});
