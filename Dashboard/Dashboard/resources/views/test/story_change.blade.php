<!doctype html>
<html lang="{{ app()->getLocale() }}">
    <head>
        <meta charset="utf-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <title>test</title>

        <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>

        <style>
            table{

            }
            table tr td{
                vertical-align: top;
            }
            textarea{
                width: 99%;
                height: 200px;
            }
        </style>
    </head>
    <body>
        <br />
        <br />
        <h2> Test changing story #{{ $story->id }}</h2>
        <table>
            <tr>
                <td>Title</td>
                <td><input type="text" name="title" id="title" value="{{ $story->title }}"/></td>
                <td></td>
            </tr>
            <tr>
                <td>Icon</td>
                <td><input type="text" name="icon" id="icon" value="{{ $story->icon }}"/></td>
                <td></td>
            </tr>
            <tr>
                <td>Description</td>
                <td><textarea name="description" id="description">{{ $story->description }}</textarea></td>
                <td></td>
            </tr>
            <tr>
                <td>Active</td>
                <td>
                    <input type="checkbox" name="active" @if($story->active) checked @endif id="active" />
                <td></td>
                </td>
            </tr>
            @foreach($story->storyItems as $storyItem)
            <tr id="storyItem{{ $storyItem->id }}">
                <td></td>
                <td>

                    @if(!$storyItem->file || ($storyItem->file && strlen($storyItem->text) > 0))
                        <textarea name="texts[{{ $storyItem->id }}]" id="texts">{{ $storyItem->text }}</textarea>
                    @endif
                    @if($storyItem->file)
                        <br />
                        <a href="{{ url('api/v1/file/'.$storyItem->file->id) }}">Download</a>
                        <br />
                        <input type="file" name="files[{{ $storyItem->id }}]" />
                    @endif
                </td>
                <td><a href="#" id="delete" data-id="{{ $storyItem->id }}">Verwijder</a></td>
            </tr>
            @endforeach
            <tr>
                <td></td>
                <td><input type="button" id="save" value="Save"></td>
                <td></td>
            </tr>
        </table>


        {{--<input type="file" name="file" id="file" multiple /><br />--}}


        <script>

            $("#active").on("change", function(){

                var active = "activate";
                if(!$(this).is(":checked")){
                    active = "deactivate"
                }
                $.ajax({
                    url: '/api/v1/story/{{ $story->id }}/'+active,
                    type: 'GET',
                    success: function(data)
                    {
                        console.log(data);
                    },
                    error: function(jqXHR, textStatus, errorThrown)
                    {
                        // Handle errors here
                        console.log('ERRORS: ' + textStatus);
                        // STOP LOADING SPINNER
                    }
                });
            });

            $('#delete').click(function(){

                $.ajax({
                    url: '/api/v1/storyItem/'+$(this).data("id"),
                    type: 'DELETE',
                    success: function(data)
                    {
                        // Remove from table
                        if(data['storyItemId']){
                            $("#storyItem"+data['storyItemId']).remove();
                        }
                        console.log(data);
                    },
                    error: function(jqXHR, textStatus, errorThrown)
                    {
                        // Handle errors here
                        console.log('ERRORS: ' + textStatus);
                        // STOP LOADING SPINNER
                    }
                });
            });

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