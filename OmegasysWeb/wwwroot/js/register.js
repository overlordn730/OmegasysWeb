function togglePasswordVisibility(inputId, toggleIconId) {
    var passwordInput = document.getElementById(inputId);
    var passwordToggleIcon = document.getElementById(toggleIconId);

    if (passwordInput.type === "password") {
        passwordInput.type = "text";
        passwordToggleIcon.classList.remove("bi-eye-slash");
        passwordToggleIcon.classList.add("bi-eye");
    } else {
        passwordInput.type = "password";
        passwordToggleIcon.classList.remove("bi-eye");
        passwordToggleIcon.classList.add("bi-eye-slash");
    }
}

// Agrega los event listeners a los íconos del ojo
document.getElementById('passwordInputToggleIcon').addEventListener('click', function () {
    togglePasswordVisibility('Input_Password', 'passwordInputToggleIcon');
});

document.getElementById('confirmPasswordInputToggleIcon').addEventListener('click', function () {
    togglePasswordVisibility('Input_ConfirmPassword', 'confirmPasswordInputToggleIcon');
});