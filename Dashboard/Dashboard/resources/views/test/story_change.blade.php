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
            table tr:hover td{
                background: #EEEEEE;
            }
            table tr td{
                vertical-align: top;
                padding: 5px 20px;
            }
            textarea{
                width: 400px;
                height: 200px;
                display: block;
            }
            img{
                max-width: 200px;
            }
            input[type=file]{
                display: block;
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
                <td>Story item #{{ $storyItem->id }}</td>
                <td>

                    @if(!$storyItem->file)
                        <textarea name="texts[{{ $storyItem->id }}]" class="text">{{ $storyItem->text }}</textarea>
                    @else
                        <a href="{{ url('api/v1/file/'.$storyItem->file->id) }}">
                            @if(strpos($storyItem->file->fileType, 'image') !== false)
                                <img src="{{ url('api/v1/file/'.$storyItem->file->id) }}" alt="{{ $storyItem->file->realName }}"/>
                            @else
                                {{ $storyItem->file->realName }}
                            @endif

                        </a>
                    @endif
                </td>
                <td><button class="delete" data-id="{{ $storyItem->id }}">Verwijder</button></td>
            </tr>
            @endforeach
            <tr>

                <td>Texts</td>
                <td id="texts">
                    <textarea name="newTexts[]" class="text"></textarea>
                </td>
                <td><button id="addText">+</button></td>
            </tr>
            <tr>

                <td>Upload</td>
                <td id="files">
                    <input type="file" name="files[]" class="file" />
                </td>
                <td><button id="addFile">+</button></td>
            </tr>
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
            $("#addFile").click(function(){
                var f = document.createElement("input");
                    f.setAttribute('type', 'file');
                    f.setAttribute('class', 'file');
                    f.setAttribute('name', 'files[]');
                $("#files").append(f);
            });
            $("#addText").click(function(){
                var f = document.createElement("textarea");
                    f.setAttribute('class', 'text');
                    f.setAttribute('name', 'newTexts[]');
                $("#texts").append(f);
            });
            $('.delete').click(function(){
                if(confirm("Weet u zeker dat u dit wilt verwijderen?")){

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
                }
            });

            $('#save').click(function(){

                var formData = new FormData();
                formData.append("title", $('#title').val());
                formData.append("icon", $('#icon').val());
                formData.append("description", $('#description').val());

                $(".text").each(function(){
                    formData.append(this.name, $(this).val());
                });

                // alleen javascript gebruikt als voorbeeld
                var filesInput = document.querySelectorAll('.file');
                for(var i=0;i<filesInput.length;i++){
                    var files = filesInput[i].files;
                    var name = filesInput[i].name;
                    for(var j=0;j<files.length;j++){
                        formData.append(name, files[j]);
                    }
                }

                // formData.forEach(function(val, key){
                //     console.log(key + ": " + val);
                // });
                // Used javascript for test
                // var filesInput = document.getElementById('file').files;
                // for(var i=0;i<filesInput.length;i++){
                //     formData.append("files[]", filesInput[i]);
                // }

                $.ajax({
                    url: '/api/v1/story/{{ $story->id }}/change',
                    type: 'POST',
                    data: formData,
                    cache: false,
                    processData: false, // Don't process the files
                    contentType: false, // Set content type to false as jQuery will tell the server its a query string request
                    success: function(data, textStatus, jqXHR)
                    {
                        console.log(data);
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