<!doctype html>
<html lang="{{ app()->getLocale() }}">
    <head>
        <meta charset="utf-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <title>test</title>

        <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>

    </head>
    <body>
        <br />
        <br />
        <h2> Test insert</h2>
        <input type="text" name="title" id="title" /><br />
        <input type="icon" name="icon" id="icon" /><br />
        <input type="file" name="file" id="file" multiple /><br />
        <input type="button" id="save" value="Save">

        <script>

            $('#save').click(function(){

                var formData = new FormData();
                formData.append("title", $('#title').val());
                formData.append("icon", $('#icon').val());
                var filesInput = document.getElementById('file').files;
                for(var i=0;i<filesInput.length;i++){
                    formData.append("files[]", filesInput[i]);
                }

                console.log(formData);
                $.ajax({
                    url: '/api/v1/story',
                    type: 'POST',
                    data: formData,
                    cache: false,
                    processData: false, // Don't process the files
                    contentType: false, // Set content type to false as jQuery will tell the server its a query string request
                    success: function(data, textStatus, jqXHR)
                    {
                        // do something
                    },
                    error: function(jqXHR, textStatus, errorThrown)
                    {
                        // Handle errors here
                        console.log('ERRORS: ' + textStatus);
                        // STOP LOADING SPINNER
                    }
                });
            });
        </script>
    </body>
</html>