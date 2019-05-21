<?php

namespace App\Http\Controllers\Api;



use App\Http\Controllers\Controller;
use App\Story;

class LabyrintApiController extends Controller
{



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
        //{
        //    statistics: [
        //        {
        //            storyID: 1,
        //            title: "De brief van karel",
        //            feedback: [
        //                {
        //                    question: "",
        //                    answers: [
        //                        {
        //                            answerId: 1,
        //                            answer: "",
        //                            count: 10
        //                        },
        //                        {
        //                            answerId: 2,
        //                            answer: "",
        //                            count: 10
        //                        },
        //                    ]
        //                }
        //            ]
        //        },
        //        {
        //            storyID: 2,
        //            title: "De brief van gert",
        //            feedback: [
        //                {
        //                    question: "",
        //                    answers: [
        //                        {
        //                            answerId: 1,
        //                            answer: "",
        //                            count: 10
        //                        },
        //                        {
        //                            answerId: 2,
        //                            answer: "",
        //                            count: 10
        //                        },
        //                    ]
        //                }
        //            ]
        //        }
        //    ]
        //}
        if(isset($_GET['onlyActive'])){
            $stories = \App\Story::with('feedback')->where('active', 0)->get();
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
