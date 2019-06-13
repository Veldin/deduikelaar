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
                position: absolute;
                width: 100%;
                height: 100%;
                font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Oxygen-Sans, Ubuntu, Cantarell, "Helvetica Neue", sans-serif;
                font-size: @if(isset($_GET['overview'])) 0.5rem @else 1rem @endif;
                background-color: #aa9479;
                margin: 0;
                padding: 0;
                overflow: auto;
                transform-origin: center center;
            }

            html{
                overflow: hidden;
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
        <script>

            @if(isset($_GET['overview']))
            var rotate = 0;
            @else
            var rotate = --rotatingAngle--;
            @endif

            document.addEventListener('DOMContentLoaded', function() {
                var w = window,
                    d = document,
                    e = d.documentElement,
                    g = d.getElementsByTagName('body')[0],
                    width = w.innerWidth || e.clientWidth || g.clientWidth,
                    height = w.innerHeight|| e.clientHeight|| g.clientHeight;

                if(rotate === 90){
                    document.querySelector('body').style.marginLeft = (width-height)/2 + "px";
                    document.querySelector('body').style.marginTop = "-"+(width-height)/2+"px";
                    document.querySelector('body').style.width = height+"px";
                    document.querySelector('body').style.height = width+"px";
                    document.querySelector('body').style.transform = "rotateZ(90deg) translate(0,0)";

                }else if(rotate === 180){
                    document.querySelector('body').style.transform = "rotateZ(180deg) translate(0,0)";
                }else if(rotate === 270){
                    document.querySelector('body').style.top = width/2+"px";
                    document.querySelector('body').style.left = height/2+"px";
                    document.querySelector('body').style.marginLeft = (width-height) + "px";
                    document.querySelector('body').style.marginTop = "-"+(width-height)+"px";
                    document.querySelector('body').style.width = height+"px";
                    document.querySelector('body').style.height = width+"px";
                    document.querySelector('body').style.transform = "rotateZ(270deg) translate(50%,-50%)";
                }

            });

        </script>
    </body>
</html>