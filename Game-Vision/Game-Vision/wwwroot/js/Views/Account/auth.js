document.addEventListener('DOMContentLoaded', function () {
    // فرم ورود
    const loginForm = document.getElementById('login');
    if (loginForm) {
        loginForm.addEventListener('submit', async function (e) {
            e.preventDefault();
            clearErrors('login');

            const usernameOrEmail = loginForm.querySelector('input[type="text"], input[type="email"]').value.trim();
            const password = loginForm.querySelector('input[type="password"]').value.trim();

            if (!usernameOrEmail || !password) {
                showError('login', 'همه فیلدها الزامی هستند');
                return;
            }

            const submitBtn = loginForm.querySelector('button[type="submit"]');
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>در حال ورود...';

            // گرفتن Anti-Forgery Token از فرم (بهترین روش برای MVC)
            const antiforgeryToken = loginForm.querySelector('input[name="__RequestVerificationToken"]').value;

            try {
                const response = await fetch('/Account/Login', {  // مسیر درست برای MVC
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': antiforgeryToken  // این مهمه!
                    },
                    body: JSON.stringify({
                        usernameOrEmail: usernameOrEmail,
                        password: password
                    })
                });

                if (response.ok) {
                    const data = await response.json();
                    localStorage.setItem('jwtToken', data.token);
                    localStorage.setItem('username', data.username);
                    localStorage.setItem('role', data.role);

                    showSuccess('login', 'ورود با موفقیت انجام شد! در حال انتقال...');
                    setTimeout(() => {
                        window.location.href = '/';
                    }, 1500);
                } else {
                    const errorData = await response.json().catch(() => ({}));
                    showError('login', errorData.message || 'نام کاربری یا رمز عبور اشتباه است');
                }
            } catch (err) {
                console.error(err);
                showError('login', 'خطا در ارتباط با سرور. لطفاً دوباره تلاش کنید');
            } finally {
                submitBtn.disabled = false;
                submitBtn.innerHTML = '<i class="bi bi-box-arrow-in-left"></i> ورود';
            }
        });
    }

    // فرم ثبت‌نام
    const registerForm = document.getElementById('register');
    if (registerForm) {
        registerForm.addEventListener('submit', async function (e) {
            e.preventDefault();
            clearErrors('register');

            const inputs = registerForm.querySelectorAll('input');
            const username = inputs[1].value.trim(); // نام کاربری
            const email = inputs[2].value.trim();
            const password = inputs[3].value;
            const confirmPassword = inputs[4].value;

            if (!username || !email || !password || !confirmPassword) {
                showError('register', 'همه فیلدها الزامی هستند');
                return;
            }
            
            if (password.length < 6) {
                showError('register', 'رمز عبور باید حداقل ۶ کاراکتر باشد');
                return;
            }

            const submitBtn = registerForm.querySelector('button[type="submit"]');
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>در حال ثبت‌نام...';

            const antiforgeryToken = registerForm.querySelector('input[name="__RequestVerificationToken"]').value;

            try {
                const response = await fetch('/Account/Register', {  // مسیر درست برای MVC
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': antiforgeryToken
                    },
                    body: JSON.stringify({
                        username: username,
                        email: email,
                        password: password,
                        confirmPassword: confirmPassword
                    })
                });

                if (response.ok) {
                    const data = await response.json();
                    localStorage.setItem('jwtToken', data.token);
                    localStorage.setItem('username', data.username);
                    localStorage.setItem('role', data.role);

                    showSuccess('register', 'ثبت‌نام با موفقیت انجام شد! خوش آمدید');
                    setTimeout(() => {
                        window.location.href = '/';
                    }, 1500);
                } else {
                    const errorData = await response.json().catch(() => ({}));
                    showError('register', errorData.message || 'خطایی رخ داد');
                }
            } catch (err) {
                console.error(err);
                showError('register', 'خطا در ارتباط با سرور');
            } finally {
                submitBtn.disabled = false;
                submitBtn.innerHTML = '<i class="bi bi-person-plus"></i> ثبت‌نام';
            }
        });
    }

    // توابع کمکی (همون قبلی، تغییر نکردن)
    function showError(formId, message) {
        const form = document.getElementById(formId);
        let errorDiv = form.querySelector('.alert-danger');
        if (!errorDiv) {
            errorDiv = document.createElement('div');
            errorDiv.className = 'alert alert-danger mt-3';
            form.appendChild(errorDiv);
        }
        errorDiv.textContent = message;
    }

    function showSuccess(formId, message) {
        const form = document.getElementById(formId);
        let successDiv = form.querySelector('.alert-success');
        if (!successDiv) {
            successDiv = document.createElement('div');
            successDiv.className = 'alert alert-success mt-3';
            form.appendChild(successDiv);
        }
        successDiv.textContent = message;
    }

    function clearErrors(formId) {
        const form = document.getElementById(formId);
        form.querySelectorAll('.alert').forEach(el => el.remove());
    }
});