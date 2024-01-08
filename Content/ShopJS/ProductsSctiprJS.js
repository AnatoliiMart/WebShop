$(function () {

    /* Select product from specified category */

    $("#SelectCat").on("change", function () {
        var url = $(this).val();

        if (url) {
            window.location = "/admin/shop/ProductList?catId=" + url;
        }
        return false;
    });

    /* Confirm page deletion */

    $("a.delete").click(function () {
        if (!confirm("Confirm deletion")) return false;
    });

    /*-----------------------------------------------------------*/
});