<?php

namespace App\Http\Controllers\Api;



use App\Feedback;
use App\Http\Controllers\Controller;
use App\Story;

class LabyrintApiController extends Controller
{


    public function documentation(){

        return file_get_contents("https://alwinkroesen.docs.apiary.io/")."
        <script>
        function whenReady(){
            var header = document.querySelector('#applicationHeader');
            if(!header){
                
                setTimeout(function(){
                    whenReady()
                },500);
            }else{
                if (window.history.replaceState) {
                   //prevents browser from storing history with each change:
                   window.history.replaceState(null, 'documentation', '/api/v1/');
                }
//                var app = document.querySelector('#application');
//                    app.style.zIndex = '1';
//                    app.style.marginTop = '-15px';
//                    app.style.height = '40px';
//                    app.style.overflow = 'hidden';
                    
//                var headerContainer = document.querySelector('#applicationHeader');
////                    headerContainer
//                
//                var header = document.querySelector('#applicationHeader');
                var elements = document.querySelectorAll('#applicationHeader *');
                
                for(var i=0; i<elements.length;i++){
                    elements[i].style.display = 'none';
                }
                
//                    header.style.display = 'none';
                var h1 = document.createElement('h1');
                    h1.innerHTML = 'Api Documentation <br /><span style=\"font-style: italic;font-size: 14px;padding-top: 10px;display: inline-block;\">De Duikelaar</span>';
                    h1.style.color = '#FFF';
                    h1.style.paddingTop = '0px';
                    h1.style.paddingLeft = '20px';
                    
                    header.appendChild(h1);
            }
        }
        whenReady();
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
                    if($feedbackItem->question->id != $feedback->id) continue;
                    if( isset($answers[$feedbackItem->id]) &&
                        isset($answers[$feedbackItem->id]['count']) ){
                        $answers[$feedbackItem->id]['count'] += 1;
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
        // TODO: Implement API getting te order

        // Get the stories and feedback
        $stories = Story::with('feedback')->where('active', 1)->get();
        $feedbacks = Feedback::with('feedbackItems')->get();

        // Create a default array for the feedback for each story
        $fs = [];
        foreach ($feedbacks as $feedback){
            $fis = [];
            foreach ($feedback->feedbackItems as $fi){
                $fis[$fi->id] = [
                    'id' => $fi->id,
                    'count' => 0
                ];
            }
            $fs[$feedback->id] = [
                'id' => $feedback->id,
                'total' => 0,
                'items' => $fis
            ];
        }

        // Create a list of stories
        $a = [];
        foreach ($stories as $story){

            // Set default array in a temp variable
            $sfs = $fs;
            $total = 0;
            foreach ($story->feedback as $fi){
                // Calculate the amount of feedback given
                $sfs[$fi->question->id]['total']++;
                $sfs[$fi->question->id]['items'][$fi->id]['count']++;
            }

            foreach ($sfs as $k => $v){
                // Sort feedbackitems, lowest first
                usort($v['items'], function ($a, $b) {
//                    var_dump($a['count']);
                    if($a['count'] == $b['count']) return 0;
                    return ($a['count'] < $b['count']) ? -1 : 1;
                });
                $sfs[$k]['items'] = $v['items'];
                $total += $sfs[$k]['total'];
            }

//            $a['b'][$story->id] = $sfs;

            // Sort feedback, lowest first
            usort($sfs, function ($a, $b) {
                if($a['total'] == $b['total']) return 0;
                return ($a['total'] < $b['total']) ? -1 : 1;
            });

            // Set feedback
            $a[$story->id] = [
                'id' => $story->id,
                'total' => $total,
                'feedback' => $sfs
            ];
        }



        $order = [];
        $fbOrder = [];
        foreach ($fs as $f){
            $fbOrder[$f['id']] = [
                'id' => $f['id'],
                'count' => 0
            ];
        }

        // Return minimal 1000 stories
        for($i=0;$i<1000;){

            if($i>0){ // Skip sorting the first time (already sorted)

                foreach ($a as $j => $story){

                    $sfs = $story['feedback'];
                    foreach ($sfs as $k => $v){
                        // Sort feedbackitems, lowest first
                        usort($v['items'], function ($a, $b) {
                            if($a['count'] == $b['count']) return 0;
                            return ($a['count'] < $b['count']) ? -1 : 1;
                        });
                        $sfs[$k]['items'] = $v['items'];
                        $total += $sfs[$k]['total'];
                    }
                    // Sort feedback, lowest first
                    usort($sfs, function ($a, $b) {
                        if($a['total'] == $b['total']) return 0;
                        return ($a['total'] < $b['total']) ? -1 : 1;
                    });
                    $a[$j]['feedback'] = $sfs;
                }

                usort($a, function ($a, $b) {
                    if($a['total'] == $b['total']) return 0;
                    return ($a['total'] < $b['total']) ? -1 : 1;
                });
            }

            foreach ($a as $k => $b){
                $i++;
                $fid = $b['feedback'][0]['id'];
                $fbTempOrder = $fbOrder;
                usort($fbTempOrder, function($a, $b){
                    if($a['count'] == $b['count']) return 0;
                    return ($a['count'] < $b['count']) ? -1 : 1;
                });
                if($fbTempOrder[0]['id'] != $fid){
                    // NOT EVERY TIME THE SAME FEEDBACK
                    if($fbTempOrder[0]['count']+2 < $fbOrder[$fid]['count']){
                        $fid = $fbTempOrder[0]['id'];
                    }
                }
                $fbOrder[$fid]['count']++;
                $a[$k]['total']++;
                $fbnum = -1; // feebackNum
                $fbinum = -1; // feedbackItemNum
                foreach ($b['feedback'] as $n => $f){
                    $fbnum = $n;
                    foreach ($f['items'] as $m => $fi){
                        if($fi['id'] == $fid){
                            $fbinum = $m;
                            break;
                        }
                    }
                    if($fbinum >= 0) break;
                }

                $a[$k]['feedback'][$fbnum]['items'][$fbinum]['count']++;
                $order[] = [
                    'storyId' => $b['id'],
                    'feedbackId' => $fid
                ];
            }
        }
        return response()->json($order);

    }



    public function getStories(){

        $data = [];
        $stories = Story::with('storyItems')->where('active', 1)->get();

        foreach ($stories as $story){
            $data[] = [
                'storyId' => $story->id,
                'icon' => $story->icon,
                'html' => view('information-piece', compact('story'))->render()
            ];
        }
        return $data;
    }

    public function testStory(){

        $story = Story::with('storyItems')->where('active', 1)->find(1);
        return view('information-piece', compact('story'));
    }


    public function getFeedback(){

        $feedbacks = Feedback::with('feedbackItems')->get();

        $data = [];

        foreach ($feedbacks as $feedback){
            $answers = [];

            foreach ($feedback->feedbackItems as $feedbackItem){
                $answers[] = [
                    'answerId' => $feedbackItem->id,
                    'response' => $feedbackItem->feedback
                ];
            }

            $data[] = [
                'feedbackId' => $feedback->id,
                'question' => $feedback->question,
                'extraInfo' => $feedback->extraInfo,
                'feedbackType' => $feedback->feedbackType,
                'answers' => $answers
            ];
        }

        return  response()->json($data);
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


    public function deleteStory($storyId){
        $story = Story::find($storyId);
        if($story){
            $story->delete();
            return response()->json([
                'response' => 'success'
            ]);
        }
        return response()->json([
            'response' => 'failed'
        ]);
    }

}
