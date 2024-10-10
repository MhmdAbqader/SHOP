
    var myDataTableTable;
    $(document).ready(function(){
        loadData();
    });


    // please please plz plz plz don't forget ; at in any thing in js
    // //
    // انا بقالي نص ساعة واكتر مش عارف فين المشكلة بسب ال ;  خلي بالك عشان بتعمل مشكلة ف الجافاسكربيت وخصوصا الdatatable

debugger;
    function loadData(){

        myDataTableTable = $("#myTableOfDataTable").DataTable({
            "ajax": {
                "url": "/Admin/Order/GetDataByDatatable"
            },
            "columns": [
                { "result": "id" },
                { "result": "applicationUser.email" },
                
                { "result": "totalPrice" },
                { "result": "orderStatus" },
                { "result": "custmerName" },
                { "result": "phone" },
                {
                    "result": "id",
                    "render": function (result) {
                        return `<a href="/Admin/Order/Details/${result}" class="btn btn-dark">Details</a>
                                    `
                    }
                }
            ]
        });
        }
