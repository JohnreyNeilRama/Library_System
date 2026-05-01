// Borrow Modal JavaScript

function openBorrowModalFromCard(button) {
    var card = button.closest('.book-card');
    var bookId = card.dataset.bookId;
    var title = card.dataset.bookTitle;
    var author = card.dataset.bookAuthor;

    document.getElementById('modalBookTitle').textContent = title;
    document.getElementById('modalBookAuthor').textContent = 'by ' + author;
    document.getElementById('bookIdInput').value = bookId;
    // Set default borrow date to today
    var today = new Date();
    var yyyy = today.getFullYear();
    var mm = String(today.getMonth() + 1).padStart(2, '0');
    var dd = String(today.getDate()).padStart(2, '0');
    var dateString = yyyy + '-' + mm + '-' + dd;
    
    var borrowDateInput = document.getElementById('borrowDateInput');
    if (borrowDateInput) {
        borrowDateInput.value = dateString;
        borrowDateInput.min = dateString;
    }

    resetForm();
    document.getElementById('borrowModal').classList.add('active');
    document.body.style.overflow = 'hidden';
}

function closeBorrowModal() {
    document.getElementById('borrowModal').classList.remove('active');
    document.body.style.overflow = '';
    setTimeout(resetForm, 200);
}

function resetForm() {
    document.getElementById('borrowForm').reset();
    document.getElementById('formContent').style.display = 'block';
    document.getElementById('successMessage').style.display = 'none';
    var btn = document.getElementById('submitBtn');
    if (btn) {
        btn.disabled = false;
        btn.classList.remove('loading');
    }
}

document.getElementById('borrowForm').addEventListener('submit', async function(e) {
    e.preventDefault();
    var form = this;
    var data = new FormData(form);
    var btn = document.getElementById('submitBtn');
    
    btn.disabled = true;
    btn.classList.add('loading');
    
    document.getElementById('formContent').style.display = 'none';
    var formLoading = document.getElementById('formLoading');
    if (formLoading) formLoading.style.display = 'flex';

    try {
        var res = await fetch(form.action, {
            method: 'POST',
            body: data,
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });

        var contentType = res.headers.get('content-type');

        if (formLoading) formLoading.style.display = 'none';

        if (res.redirected) {
            handleRedirect(res.url);
        } else if (contentType && contentType.indexOf('application/json') !== -1) {
            var result = await res.json();
            if (result.success) {
                showSuccess(result.message || 'Borrowed successfully!');
            } else {
                document.getElementById('formContent').style.display = 'block';
                alert(result.message || 'Borrow failed');
                btn.disabled = false;
                btn.classList.remove('loading');
            }
        } else if (!res.ok) {
            document.getElementById('formContent').style.display = 'block';
            alert('Error occurred. Please try again.');
            btn.disabled = false;
            btn.classList.remove('loading');
        } else {
            // Assume success if no error and no JSON
            showSuccess('Borrowed successfully!');
        }
    } catch (err) {
        if (formLoading) formLoading.style.display = 'none';
        document.getElementById('formContent').style.display = 'block';
        alert('Network error. Please try again.');
        btn.disabled = false;
        btn.classList.remove('loading');
    }
});

function handleRedirect(url) {
    if (url.indexOf('BorrowedBooks') !== -1 || url.indexOf('Dashboard') !== -1) {
        showSuccess('Book borrowed successfully!');
    } else if (url.indexOf('Index') !== -1) {
        closeBorrowModal();
        alert('This book is currently out of stock or action cannot be completed.');
    } else {
        closeBorrowModal();
        window.location.href = url;
    }
}

function showSuccess(msg) {
    document.getElementById('successText').textContent = msg;
    document.getElementById('formContent').style.display = 'none';
    var formLoading = document.getElementById('formLoading');
    if (formLoading) formLoading.style.display = 'none';
    document.getElementById('successMessage').style.display = 'flex';
}

document.querySelector('.modal-overlay').addEventListener('click', closeBorrowModal);
document.addEventListener('keydown', function(e) {
    if (e.key === 'Escape') closeBorrowModal();
});
