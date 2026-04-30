// Borrow Modal JavaScript

function openBorrowModalFromCard(button) {
    var card = button.closest('.book-card');
    var bookId = card.dataset.bookId;
    var title = card.dataset.bookTitle;
    var author = card.dataset.bookAuthor;

    document.getElementById('modalBookTitle').textContent = title;
    document.getElementById('modalBookAuthor').textContent = 'by ' + author;
    document.getElementById('bookIdInput').value = bookId;
    resetForm();
    document.getElementById('borrowModal').classList.add('active');
    document.body.style.overflow = 'hidden';
    setTimeout(function() { 
        var firstInput = document.querySelector('#borrowModal input');
        if (firstInput) firstInput.focus();
    }, 100);
}

function closeBorrowModal() {
    document.getElementById('borrowModal').classList.remove('active');
    document.body.style.overflow = '';
    setTimeout(resetForm, 200);
}

function resetForm() {
    document.getElementById('borrowForm').reset();
    clearValidation();
    document.getElementById('formContent').style.display = 'block';
    document.getElementById('successMessage').style.display = 'none';
}

function clearValidation() {
    var groups = document.querySelectorAll('.form-group');
    for (var i = 0; i < groups.length; i++) {
        groups[i].classList.remove('has-error');
    }
    var msgs = document.querySelectorAll('.validation-message');
    for (var j = 0; j < msgs.length; j++) {
        msgs[j].textContent = '';
        msgs[j].classList.remove('visible');
    }
}

function showError(name, msg) {
    var el = document.querySelector('[data-for="' + name + '"]');
    if (el) {
        var group = el.closest('.form-group');
        group.classList.add('has-error');
        el.textContent = msg;
        el.classList.add('visible');
    }
}

document.getElementById('borrowForm').addEventListener('submit', async function(e) {
    e.preventDefault();
    var form = this;
    var data = new FormData(form);
    var btn = document.getElementById('submitBtn');
    var ok = true;
    clearValidation();

    var fields = ['FullName', 'IdNumber', 'Course', 'YearLevel', 'Email'];
    for (var f = 0; f < fields.length; f++) {
        var fieldName = fields[f];
        var inp = form.querySelector('[name="' + fieldName + '"]');
        var val = inp.value.trim();
        if (!val) {
            showError(fieldName, 'Required');
            ok = false;
        } else if (fieldName === 'Email' && !isValidEmail(val)) {
            showError(fieldName, 'Invalid email');
            ok = false;
        }
    }

    if (!ok) return;
    btn.disabled = true;
    btn.classList.add('loading');

    try {
        var res = await fetch(form.action, {
            method: 'POST',
            body: data,
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });

        var contentType = res.headers.get('content-type');

        if (res.redirected) {
            handleRedirect(res.url);
        } else if (contentType && contentType.indexOf('application/json') !== -1) {
            var result = await res.json();
            if (result.success) {
                showSuccess(result.message || 'Borrowed successfully!');
            } else {
                handleServerErrors(result.errors) || alert(result.message || 'Borrow failed');
            }
        } else if (!res.ok) {
            alert('Error occurred. Please try again.');
        }
    } catch (err) {
        alert('Network error. Please try again.');
    } finally {
        btn.disabled = false;
        btn.classList.remove('loading');
    }
});

function isValidEmail(email) {
    var at = email.indexOf('@');
    var dot = email.lastIndexOf('.');
    return at > 0 && dot > at + 1 && dot < email.length - 1;
}

function handleRedirect(url) {
    if (url.indexOf('BorrowedBooks') !== -1) {
        showSuccess('Book borrowed successfully!');
    } else if (url.indexOf('Index') !== -1) {
        closeBorrowModal();
        alert('This book is currently out of stock.');
    } else {
        closeBorrowModal();
        window.location.href = url;
    }
}

function handleServerErrors(errors) {
    if (!errors) return false;
    var handled = false;
    for (var key in errors) {
        if (errors.hasOwnProperty(key)) {
            var fieldName = key.replace('Model.', '').replace('.', '');
            var messages = errors[key];
            if (messages && messages.length > 0) {
                showError(fieldName, messages[0]);
                handled = true;
            }
        }
    }
    return handled;
}

function showSuccess(msg) {
    document.getElementById('successText').textContent = msg;
    document.getElementById('formContent').style.display = 'none';
    document.getElementById('successMessage').style.display = 'flex';
}

document.querySelector('.modal-overlay').addEventListener('click', closeBorrowModal);
document.addEventListener('keydown', function(e) {
    if (e.key === 'Escape') closeBorrowModal();
});

var inputs = document.querySelectorAll('#borrowForm input, #borrowForm select');
for (var i = 0; i < inputs.length; i++) {
    (function(inp) {
        inp.addEventListener('blur', function() { validate(inp); });
        inp.addEventListener('input', function() {
            var g = inp.closest('.form-group');
            if (g) {
                g.classList.remove('has-error');
                var msg = g.querySelector('.validation-message');
                if (msg) msg.classList.remove('visible');
            }
        });
    })(inputs[i]);
}

function validate(inp) {
    var field = inp.name;
    var val = inp.value.trim();
    var group = inp.closest('.form-group');
    var msg = group.querySelector('.validation-message');
    if (!val) {
        group.classList.add('has-error');
        msg.textContent = 'Required';
        msg.classList.add('visible');
        return false;
    } else if (field === 'Email' && !isValidEmail(val)) {
        group.classList.add('has-error');
        msg.textContent = 'Invalid email';
        msg.classList.add('visible');
        return false;
    } else {
        group.classList.remove('has-error');
        msg.classList.remove('visible');
        return true;
    }
}
