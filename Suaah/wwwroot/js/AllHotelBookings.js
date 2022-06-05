var dataTable;

$(document).ready(function () {

    var url = window.location.search;

    if (url.includes("pending")) {
        loadDataTable("pending");
    }
    else if (url.includes("approved")) {
        loadDataTable("approved");
    }
    else if (url.includes("cancelled")) {
        loadDataTable("cancelled");
    }
    else if (url.includes("done")) {
        loadDataTable("done");
    }
    else {
        loadDataTable("all");
    }
});

function loadDataTable(status) {
    dataTable = $('#myTable').DataTable({
        "ajax": {
            "url": "/Admin/AllHotelBookingHeaders/GetAll?status=" + status
        },
        "columns": [
            { "data": "id", "width": "15%" },
            { "data": "name", "width": "15%" },
            { "data": "bookingDate", "width": "20%" },
            { "data": "totalPrice", "width": "15%" },
            { "data": "status", "width": "15%" },
            { "data": "payment", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group p-2" role="group">
                            <a class="mx-2" href="/Admin/AllHotelBookingHeaders/Details?id=${data}">Details</a> |
                            <a class="mx-2" href="/Admin/AllHotelBookingHeaders/Delete?id=${data}">Delete</a>
                        </div>
                    `
                },
                "width": "15%"
            }
        ]
    });
}