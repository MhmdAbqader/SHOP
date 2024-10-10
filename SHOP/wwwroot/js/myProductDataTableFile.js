
    var myDataTableTable;
    $(document).ready(function(){
        loadData();
    });


    // please please plz plz plz don't forget ; at in any thing in js 
    // //  
    // انا بقالي نص ساعة واكتر مش عارف فين المشكلة بسب ال ;  خلي بالك عشان بتعمل مشكلة ف الجافاسكربيت وخصوصا الdatatable  

    function loadData(){
        myDataTableTable = $("#myTableOfDataTable").DataTable({
            "ajax": {
                "url": "/Admin/Product/GetDataByDatatable"
            },
            "columns": [
                { "data": "name" },
                { "data": "description" },
                { "data": "price" },
                { "data": "imageUrl" },
                { "data": "category.name" },
                {
                    "data": "id",
                    "render": function (data) {
                        return `
                                 <a href="/Admin/Product/Edit/${data}" class="btn btn-dark">Edit</a>
                                 <a href="/Admin/Product/Delete/${data}" class="btn btn-danger">Delete</a>
                            `
                    },
                }
            ]
        });
    }      
