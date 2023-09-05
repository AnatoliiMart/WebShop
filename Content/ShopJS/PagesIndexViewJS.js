$(function () {
    //Page delete confitmation
    $("a.delete").click(function () {
        if (!confirm("Confirm page deletion")) return false;
    });
    /////////////////////////////////////////////////////////////////
    /*Sotring */
    $("table#pages tbody").sortable({
        items: "tr:not(.home)",
        placeholder: "ui-state-highlight",
        update: function () {
            var ID = $("table#pages tbody").sortable("serialize");
            //console.log(ID);
            var url = "/Admin/Pages/ReorderPages";
            $.post(url, ID, function (data) {
            });
        }
    });
})