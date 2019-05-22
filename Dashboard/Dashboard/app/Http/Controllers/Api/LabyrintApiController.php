<?php

namespace App\Http\Controllers\Api;



use App\Http\Controllers\Controller;
use App\Story;

class LabyrintApiController extends Controller
{


    public function documentation(){

        return file_get_contents("https://alwinkroesen.docs.apiary.io/")."
        <script>
            setTimeout(function(){
                if (window.history.replaceState) {
                   //prevents browser from storing history with each change:
                   window.history.replaceState(null, 'documentation', '/api/v1/');
                }
                var header = document.querySelector('#applicationHeader');
                    header.style.display = 'none';
                var h1 = document.createElement('h1');
                    h1.innerHTML = 'Documentation';
                    h1.style.color = '#FFF';
                    h1.style.paddingTop = '15px';
                    h1.style.paddingLeft = '20px';
                    
                    header.parentNode.appendChild(h1);
//                for(var i=0; i < header.length; i++){
//                }
            },500);
        </script>
        ";

    }

    public function getOverview(){

        $data = [];
        $stories = \App\Story::with('feedback')->get();
        $feedbacks = \App\Feedback::with('feedbackItems')->get();



        foreach ($stories as $story){



            $questions = [];
            foreach ($feedbacks as $feedback){
                $answers = [];
                foreach ($feedback->feedbackItems as $feedbackItem){
                    $answers[$feedbackItem->id] = [
                        'feedbackId' => $feedbackItem->id,
                        'answer' => $feedbackItem->feedback,
                        'count' => 0,
                    ];
                }
                foreach ($story->feedback as $feedbackItem){
                    if( isset($answers[$feedbackItem->id]) &&
                        $answers[$feedbackItem->id]['count'] ){
                        $answers[$feedbackItem->id]['count']++;
                    }
                }
                $answers = array_values($answers);
                usort($answers, function($a, $b) {
                    return $a['count'] - $b['count'];
                });

                $questions[] = [
                    'question' => $feedback->question,
                    'extraInfo' => $feedback->extraInfo,
                    'feedbackType' => $feedback->feedbackType,
                    'answers' => array_values($answers)
                ];
            }

            $data[] = [
                'storyId' => $story->id,
                'title' => $story->title,
                'description' => $story->description,
                'icon' => $story->icon,
                'active' => $story->active,
                'feedback' => $questions
            ];

        }

        return  response()->json($data);
    }

    public function getOrder(){


        return  response()->json([
            'order' => [
                [
                    'storyId' => 1,
                    'feedbackId' => 1
                ],
                [
                    'storyId' => 2,
                    'feedbackId' => 3
                ],
                [
                    'storyId' => 8,
                    'feedbackId' => 2
                ],
                [
                    'storyId' => 3,
                    'feedbackId' => 1
                ],
            ]
        ]);
    }


    public function getStories(){

// { stories:
//    [
//        {
//            storyId: 1,
//            type: "text | image | video | quiz | audio",
//            html: "<html></html>",
//
//        }
//    ]
//}
        return  response()->json([
            'stories' => [
                [
                    'storyId'   => 1,
                    'type'      => "text",
                    'html'      => "<!doctype html>
<html lang='en'>
<head>
<meta charset='UTF-8'>
             <meta name='viewport' content='width=device-width, user-scalable=no, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0'>
                         <meta http-equiv='X-UA-Compatible' content='ie=edge'>
             <title>Document</title>
</head>
<body>
  
</body>
</html>"
                ],
                [
                    'storyId'   => 2,
                    'type'      => "video",
                    'html'      => "<!doctype html>
<html lang='en'>
<head>
<meta charset='UTF-8'>
             <meta name='viewport' content='width=device-width, user-scalable=no, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0'>
                         <meta http-equiv='X-UA-Compatible' content='ie=edge'>
             <title>Document</title>
</head>
<body>
  video
</body>
</html>"
                ],
            ]
        ]);
    }


    public function getFeedback(){


        return  response()->json([
            'feedback' => [
                [
                    'feedbackId' => 1,
                    'question' => "Wat was je gevoel bij het verhaal?",
                    'answers' => [
                        [
                            'answerId' => 1,
                            'response' => '\u0029'
                        ],
                        [
                            'answerId' => 2,
                            'response' => '\u0030'
                        ]
                    ]
                ],
                [
                    'feedbackId' => 2,
                    'question' => "Was de tekst voor jou leesbaar?",
                    'answers' => [
                        [
                            'answerId' => 3,
                            'response' => 'Ja'
                        ],
                        [
                            'answerId' => 4,
                            'response' => 'Nee'
                        ]
                    ]
                ],
            ]
        ]);
    }

    public function getStatistics(){
        if(isset($_GET['onlyActive'])){
            $stories = \App\Story::with('feedback')->where('active', 1)->get();
        }else{
            $stories = \App\Story::with('feedback')->get();
        }
        $data = [];
        foreach ($stories as $story){
            $feedback = [];
            foreach ($story->feedback as $feedbackItem){
                if(isset($feedback[$feedbackItem->id])){
                    $feedback[$feedbackItem->id]['count']++;
                }else{
                    $feedback[$feedbackItem->id] = [
                        'feedbackId' => $feedbackItem->id,
                        'answer' => $feedbackItem->feedback,
                        'count' => 1
                    ];
                }

            }
            $data[] = [
                'storyId' => $story->id,
                'title' => $story->title,
                'active' => $story->active,
                'feedback' => $feedback
            ];
        }

        return  response()->json($data);
    }

}
