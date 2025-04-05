$(document).ready(function () {
    $("#dropdownMenuLink").click(function (e) {
        e.preventDefault();
        $(this).siblings(".dropdown-menu").slideToggle("fast");
    });
    $(document).click(function (e) {
        if (!$(e.target).closest(".user-menu").length) {
            $(".user-menu .dropdown-menu").slideUp("fast");
        }
    });
});
