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
    <h2>List of stories</h2>
    <h3><a href="{{ url('') }}" >New Story</a></h3>
    <table style="width: 90%;">
        <tr>
            <th style="text-align: left;">#</th>
            <th style="text-align: left;">Title</th>
            <th style="text-align: left;">Items</th>
            <th style="text-align: left;">Files</th>
            <th style="text-align: left;">Change</th>
            {{--<th>delete</th>--}}
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
                <td><a href="{{url('api/v1/testStory/change/'.$story->id)}}">change</a></td>
                {{--<td><a href="{{url('api/v1/testStory/edit/'.$story->id)}}">change</a></td>--}}
                {{--<td><a href="{{url('api/v1/story/delete/'.$story->id)}}">delete</a></td>--}}
            </tr>


        @endforeach
    </table>
    </body>
</html>