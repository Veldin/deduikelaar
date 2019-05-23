<!doctype html>
<html lang="{{ app()->getLocale() }}">
    <head>
        <meta charset="utf-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <title>{{$story->title}}</title>
        <style>
            *{
                margin: 0;
                padding: 0;
            }
            html, body{
                width: 100%;
                height: 100%;
            }
            h1{
                font-size: 70px;
            }
            .description{
                font-size: 30px;
            }
        </style>
    </head>
    <body>
        <header>
            <h1>{{ $story->title }}</h1>
        </header>
        <div class="description">
            {{ $story->description }}
            <br /><br />

            @foreach($story->storyItems as $storyItem)
                {{ $storyItem->text }}<br />
            @endforeach
        </div>
    </body>
</html>