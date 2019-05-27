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
                font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Oxygen-Sans, Ubuntu, Cantarell, "Helvetica Neue", sans-serif;
                font-size: 1rem;
            }

            .title{
                font-weight: 400;
                font-size: 1.8rem;
                margin-bottom: 60px;
                background-color: #aa9479;
                width: 70%;
            }

            .description{
                font-weight: normal;
                font-size: 1.3rem;
                background-color: #aa9479;
            }
        </style>
    </head>
    <body>
        <header>
            <h1 class="title">{{ $story->title }}</h1>
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