// PhoneStore Admin JavaScript

$(document).ready(function() {
    // Sidebar active link
    highlightActiveLink();
    
    // Mobile toggle
    $('.navbar-toggle').click(function() {
        $('.sidebar').toggleClass('active');
    });
    
    // Auto hide alerts after 5 seconds
    setTimeout(function() {
        $('.alert').fadeOut('slow');
    }, 5000);
    
    // Confirm delete
    $('.btn-delete').click(function(e) {
        if (!confirm('Bạn có chắc chắn muốn xóa?')) {
            e.preventDefault();
        }
    });
    
    // Image preview
    $('input[type="file"]').change(function(e) {
        var reader = new FileReader();
        var preview = $(this).closest('.form-group').find('.img-preview');
        
        reader.onload = function(e) {
            preview.attr('src', e.target.result).show();
        }
        
        if (this.files && this.files[0]) {
            reader.readAsDataURL(this.files[0]);
        }
    });
    
    // Dropdown toggle
    $('.dropdown-toggle').click(function(e) {
        e.preventDefault();
        $(this).next('.dropdown-menu').toggle();
    });
    
    // Close dropdown when clicking outside
    $(document).click(function(e) {
        if (!$(e.target).closest('.dropdown').length) {
            $('.dropdown-menu').hide();
        }
    });
});

function highlightActiveLink() {
    var currentPath = window.location.pathname;
    
    $('.sidebar ul li').removeClass('active');
    
    $('.sidebar ul li a').each(function() {
        var href = $(this).attr('href');
        if (currentPath.indexOf(href) !== -1 && href !== '/' && href !== '/Admin/Home/Index') {
            $(this).parent('li').addClass('active');
        } else if (currentPath === '/Admin/Home/Index' && href === '/Admin/Home/Index') {
            $(this).parent('li').addClass('active');
        }
    });
}
