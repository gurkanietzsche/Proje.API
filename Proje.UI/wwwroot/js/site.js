// Bildirim fonksiyonu
function showNotification(message, type = 'success') {
    // Mevcut bildirimi kaldır
    $('.notification').remove();

    // Bildirim tiplerine göre CSS sınıfları
    const classes = {
        'success': 'bg-success',
        'error': 'bg-danger',
        'warning': 'bg-warning',
        'info': 'bg-info'
    };

    // Bildirim HTML'i
    const notificationHtml = `
        <div class="notification ${classes[type]} text-white shadow py-2 px-3 rounded">
            <div class="d-flex align-items-center">
                <i class="fas ${type === 'success' ? 'fa-check-circle' : type === 'error' ? 'fa-times-circle' : 'fa-exclamation-circle'} mr-2"></i>
                <span>${message}</span>
            </div>
        </div>
    `;

    // Bildirimi ekle
    $('body').append(notificationHtml);

    // Bildirimi konumlandır
    $('.notification').css({
        'position': 'fixed',
        'bottom': '20px',
        'right': '20px',
        'z-index': '9999',
        'min-width': '250px',
        'max-width': '350px',
        'opacity': '0',
        'transform': 'translateY(20px)'
    }).animate({
        'opacity': '1',
        'transform': 'translateY(0)'
    }, 300);

    // 5 saniye sonra bildirimi kaldır
    setTimeout(() => {
        $('.notification').animate({
            'opacity': '0',
            'transform': 'translateY(20px)'
        }, 300, function () {
            $(this).remove();
        });
    }, 5000);
}