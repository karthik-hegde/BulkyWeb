﻿$(document).ready(function () {
    loadDataTable();

});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            { data: 'title', "width": "25%" },
            { data: 'isbn' },
            { data: 'author' },
            { data: 'listPrice' },
            { data: 'category.name' },
            {
                data: 'id', render: (data) => {
                    return `<div class=w-75 btn-group role="group">
                    <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i>Edit</a>
                    <a href="/admin/product/delete?id=${data}" class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i>Delete</a>`

                },
                width: "25%"
            }
            
        ]
    });
}