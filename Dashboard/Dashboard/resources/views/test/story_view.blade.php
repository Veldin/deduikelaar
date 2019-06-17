<!doctype html>
<html lang="{{ app()->getLocale() }}">
    <head>
        <meta charset="utf-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <title>test</title>

        <style>
            *{
                margin: 0;
                padding: 0;
            }
            html, body{
                width: 100%;
                height: 100%;
            }
            body{
                margin-top: -135px;
                transform: scale(0.7);
            }
            .background{
                position: relative;
                left: 50%;
                margin-left: -600px;
                width: 1200px;
                background-size: cover;
            }
            .background img{
                width: 100%;
            }
            iframe{
                position: absolute;
                top: 300px;
                left: 50%;
                margin-left: -500px;
                width: 1000px;
                height: 850px;

            }
        </style>
    </head>
    <body>
        <div class="background">
            <img src="{{ url('/images/letterBackgroundMaze.png') }}" alt="letter bg"/>
        </div>
        <iframe src="{{ url('/api/v1/testStory/view/'.$id) }}" frameborder="0"></iframe>
    </body>
</html>