<!doctype html>
<html lang="{{ app()->getLocale() }}">
    <head>
        <meta charset="utf-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <title>test</title>

        <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>

        <style>
            tr:hover td{
                background: #EEEEEE;
            }
        </style>
    </head>
    <body>
    <br />
    <br />
    <h2>List of stories</h2>
    <h3><a href="{{ url('api/v1/testStory/new') }}" >New Story</a></h3>
    <table style="width: 90%;left: 5%;position: relative;">
        <tr>
            <th style="text-align: left;">#</th>
            <th style="text-align: left;">Title</th>
            <th style="text-align: left;">Items</th>
            <th style="text-align: left;">Files</th>
            <th style="text-align: left; width: 100px;">Example</th>
            <th style="text-align: left; width: 100px;">Change</th>
            <th style="text-align: left; width: 100px;">Delete</th>
        </tr>
        @foreach($stories as $story)
            <?php
                $countItems = 0;
                $countFiles = 0;
                foreach ($story->storyItems as $storyItem){
                    $countItems++;
                    if($storyItem->file){
                        $countFiles++;
                    }
                }
            ?>
            <tr>
                <td>{{ $story->id }}</td>
                <td>{{ $story->title }}</td>
                <td>{{ $countItems }}</td>
                <td>{{ $countFiles }}</td>
                <td><a href="{{url('api/v1/testStory/example/'.$story->id)}}">Example</a></td>
                <td><a href="{{url('api/v1/testStory/change/'.$story->id)}}">Change</a></td>
                <td><a href="#" data-id="{{ $story->id }}" class="delete">Delete</a></td>
            </tr>


        @endforeach
    </table>
    <script>
        $(".delete").click(function(e){
            $.ajax({
                url: '{{ url('api/v1/story/') }}/'+$(this).data('id'),
                type: 'DELETE',
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
            $(this).parent().parent().remove();
            e.preventDefault();
        })
    </script>
    </body>
</html>