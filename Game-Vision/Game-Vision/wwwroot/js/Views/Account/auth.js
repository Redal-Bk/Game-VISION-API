document.addEventListener('DOMContentLoaded', function () {
    // تابع برای آپدیت هدر
    function updateHeaderAfterLogin() {
        const token = localStorage.getItem('jwtToken');
        if (!token) return;

        const username = localStorage.getItem('username') || "کاربر";
        const role = localStorage.getItem('role') || "User";
        let avatarUrl = "https://cdn.discordapp.com/embed/avatars/0.png";

        const headerAuthSection = document.getElementById('user-auth-section');
        if (headerAuthSection) {
            headerAuthSection.innerHTML = `
                <div class="d-flex align-items-center gap-3">
                    <img src="${avatarUrl}" class="rounded-circle border border-2" 
                         style="width: 50px; height: 50px; object-fit: cover; border-color: var(--primary-neon)!important; box-shadow: 0 0 15px rgba(0, 240, 255, 0.4);"
                         alt="آواتار">
                    <div class="d-flex flex-column">
                        <span class="fw-bold text-white" style="text-shadow: 0 0 10px var(--primary-neon);">${username}</span>
                        <small class="text-secondary">${role}</small>
                    </div>
                    <button onclick="logout()" class="btn btn-sm btn-outline-neon">
                        <i class="bi bi-box-arrow-right"></i> خروج
                    </button>
                </div>
            `;
        }
    }

    // تابع خروج
    function logout() {
        localStorage.removeItem('jwtToken');
        localStorage.removeItem('username');
        localStorage.removeItem('role');
        window.location.href = '/';
    }

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
            const originalText = submitBtn.innerHTML;
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>در حال ورود...';

            const antiforgeryToken = loginForm.querySelector('input[name="__RequestVerificationToken"]').value;

            try {
                const response = await fetch('/Account/Login', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': antiforgeryToken
                    },
                    body: JSON.stringify({
                        usernameOrEmail: usernameOrEmail,
                        password: password
                    })
                });

                if (response.ok) {
                    const data = await response.json();

                    // ذخیره توکن و اطلاعات
                    localStorage.setItem('jwtToken', data.token);
                    localStorage.setItem('username', data.username);
                    localStorage.setItem('role', data.role);

                    showSuccess('login', 'ورود با موفقیت انجام شد!');

                    // آپدیت هدر فوری
                    updateHeaderAfterLogin();

                    setTimeout(() => {
                        window.location.href = '/';
                    }, 1000);
                } else {
                    const errorData = await response.json().catch(() => ({}));
                    showError('login', errorData.message || 'نام کاربری یا رمز عبور اشتباه است');
                }
            } catch (err) {
                console.error(err);
                showError('login', 'خطا در ارتباط با سرور');
            } finally {
                submitBtn.disabled = false;
                submitBtn.innerHTML = originalText;
            }
        });
    }

    // فرم ثبت‌نام (دقیقاً مثل بالا)
    const registerForm = document.getElementById('register');
    if (registerForm) {
        registerForm.addEventListener('submit', async function (e) {
            e.preventDefault();
            clearErrors('register');

            const username = registerForm.querySelector('input[placeholder="ali_gamer"]').value.trim();
            const email = registerForm.querySelector('input[type="email"]').value.trim();
            const password = registerForm.querySelectorAll('input[type="password"]')[0].value;
            const confirmPassword = registerForm.querySelectorAll('input[type="password"]')[1].value;

            if (!username || !email || !password || !confirmPassword) {
                showError('register', 'همه فیلدها الزامی هستند');
                return;
            }
            if (password !== confirmPassword) {
                showError('register', 'رمز عبور و تکرار آن مطابقت ندارند');
                return;
            }
            if (password.length < 6) {
                showError('register', 'رمز عبور باید حداقل ۶ کاراکتر باشد');
                return;
            }

            const submitBtn = registerForm.querySelector('button[type="submit"]');
            const originalText = submitBtn.innerHTML;
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>در حال ثبت‌نام...';

            const antiforgeryToken = registerForm.querySelector('input[name="__RequestVerificationToken"]').value;

            try {
                const response = await fetch('/Account/Register', {
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

                    showSuccess('register', 'ثبت‌نام با موفقیت انجام شد!');

                    // آپدیت هدر فوری
                    updateHeaderAfterLogin();

                    setTimeout(() => {
                        window.location.href = '/';
                    }, 1000);
                } else {
                    const errorData = await response.json().catch(() => ({}));
                    showError('register', errorData.message || 'خطایی رخ داد');
                }
            } catch (err) {
                console.error(err);
                showError('register', 'خطا در ارتباط با سرور');
            } finally {
                submitBtn.disabled = false;
                submitBtn.innerHTML = originalText;
            }
        });
    }

    // توابع کمکی (همون قبلی)
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

    // چک اولیه وقتی صفحه لود می‌شه
    updateHeaderAfterLogin();
});