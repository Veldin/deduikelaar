<!doctype html>
<html lang="{{ app()->getLocale() }}">
    <head>
        <meta charset="utf-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <title>{{$story->title}}</title>
        <style>
            /**{*/
                /*margin: 0;*/
                /*padding: 0;*/
            /*}*/

            *{
                max-width: 100%;
            }
            html, body{
                width: 100%;
                height: 100%;
                font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Oxygen-Sans, Ubuntu, Cantarell, "Helvetica Neue", sans-serif;
                font-size: @if(isset($_GET['overview'])) 0.5rem @else 1rem @endif;
                background-color: #aa9479;
                margin: 0;
                padding: 0;
            }
			
			/* Dont remove the transform below. This is necassary for the rotation in the Labyrint application */
			html{
				transform: rotate(0deg);
			}

            .title{
                font-weight: 400;
                font-size: 1.8rem;
                margin-bottom: @if(isset($_GET['overview'])) 20px @else 60px @endif;
                width: 70%;
            }
            audio, video{
                width: 100%;
                max-width: 100%;
            }
            img{
                max-width: 100%;
            }

            .description,
			.feedback{
                font-weight: normal;
                font-size: 1.3rem;
            }
			.feedback{
				display: block;
				width: 100%;
				text-align: center;
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
                {!! $storyItem->text !!}<br />
            @endforeach
        </div>
    </body>
</html>